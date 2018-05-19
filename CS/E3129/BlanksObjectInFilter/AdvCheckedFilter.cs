// Developer Express Code Central Example:
// How to add custom items into the filter checked list.
// 
// This example demonstrates how to add the "Blank" and "Non Blank" items into the
// filter checked list (the FilterPopupMode property is set to CheckedList)
// 
// We
// also suggest that you track the http://www.devexpress.com/scid=S131291
// suggestion to be notified when this feature will be implemented out of the box.
// 
// You can find sample updates and versions for different programming languages here:
// http://www.devexpress.com/example=E3129

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

using DevExpress.Data.Filtering;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors;
using DevExpress.Utils.Win;

namespace BlanksObjectInFilter
{
    class AdvCheckedFilter
    {
        bool checkLock;

        GridView _view;
        BindingList<GridColumn> _advColumns;
        public GridView View
        {
            get
            {
                return _view;
            }
            set
            {
                if (_view == value)
                    return;
                _view = value;
            }
        }
        public BindingList<GridColumn> AdvColumns
        {
            get
            {
                return _advColumns;
            }
            set
            {
                if (_advColumns == value)
                    return;
                _advColumns = value;
            }
        }

        public AdvCheckedFilter(GridView View)
        {
            AdvColumns = new BindingList<GridColumn>();
            this.View = View;
            View.ShowFilterPopupCheckedListBox += new FilterPopupCheckedListBoxEventHandler(this.View_ShowFilterPopupCheckedListBox);

        }
        private void View_ShowFilterPopupCheckedListBox(object sender, DevExpress.XtraGrid.Views.Grid.FilterPopupCheckedListBoxEventArgs e)
        {
            if (!_advColumns.Contains(e.Column))
                return;
            e.CheckedComboBox.SelectAllItemVisible = false;
            FilterItem fi = new FilterItem("(Select All)", 0);
            CheckedListBoxItem item = new CheckedListBoxItem(fi);
            e.CheckedComboBox.Items.Insert(0, item);
            fi = new FilterItem("(Blanks)", 1);
            item = new CheckedListBoxItem(fi);
            e.CheckedComboBox.Items.Insert(1, item);
            fi = new FilterItem("(Non Blanks)", 2);
            item = new CheckedListBoxItem(fi);
            e.CheckedComboBox.Items.Insert(2, item);
            e.CheckedComboBox.Popup += new EventHandler(CheckedComboBox_Popup);
            e.CheckedComboBox.CloseUp += new CloseUpEventHandler(CheckedComboBox_CloseUp);
        }

        private void CheckedComboBox_Popup(object sender, EventArgs e)
        {
            IPopupControl popupControl = sender as IPopupControl;
            if (popupControl == null) return;
            PopupContainerForm form = popupControl.PopupWindow as PopupContainerForm;
            if (form == null) return;
            CheckedListBoxControl control = FindListBoxControl(form);
            if (control != null)
            {
                control.ItemCheck += new DevExpress.XtraEditors.Controls.ItemCheckEventHandler(Form_ItemCheck);
                CheckAllItemsChecked(control);
                List<string> operands = GetOperandsString(View.FocusedColumn.FilterInfo.FilterCriteria);
                CheckItems(operands, control);
            }
        }

        CheckedListBoxControl FindListBoxControl(Control container)
        {
            if (container is CheckedListBoxControl)
                return container as CheckedListBoxControl;
            foreach (Control control in container.Controls)
            {
                CheckedListBoxControl listBox = FindListBoxControl(control);
                if (listBox != null)
                    return listBox as CheckedListBoxControl;
            }
            return null;
        }

        void CheckItems(List<string> values, CheckedListBoxControl control)
        {
            foreach (string value in values)
            {
                foreach (CheckedListBoxItem item in control.Items)
                    if ((item.Value as FilterItem).Text == value)
                        item.CheckState = CheckState.Checked;
            }
        }

        List<string> GetOperandsString(CriteriaOperator crOperator)
        {
            List<string> operandList = new List<string>();
            if (crOperator == null)
                return operandList;
            if (crOperator is OperandValue)
            {
                operandList.Add((string)(crOperator as OperandValue).Value);
            }
            if (crOperator is InOperator)
            {
                InOperator group = crOperator as InOperator;
                foreach (CriteriaOperator operand in group.Operands)
                    operandList.AddRange(GetOperandsString(operand));
            }
            if (crOperator is UnaryOperator)
            {
                UnaryOperator unary = crOperator as UnaryOperator;
                if (unary.OperatorType == UnaryOperatorType.IsNull)
                    operandList.Add("(Blanks)");
                if (unary.OperatorType == UnaryOperatorType.Not)
                    operandList.Add("(Non Blanks)");
            }
            if (crOperator is GroupOperator)
            {
                GroupOperator group = crOperator as GroupOperator;
                foreach (CriteriaOperator operand in group.Operands)
                    operandList.AddRange(GetOperandsString(operand));
            }
            if (crOperator is BinaryOperator)
            {
                BinaryOperator binOperator = crOperator as BinaryOperator;
                operandList.AddRange(GetOperandsString(binOperator.RightOperand));
                operandList.AddRange(GetOperandsString(binOperator.LeftOperand));
            }
            return operandList;

        }

        void Form_ItemCheck(object sender, DevExpress.XtraEditors.Controls.ItemCheckEventArgs e)
        {
            if (checkLock)
                return;
            try
            {
                CheckedListBoxControl listBox = (sender as CheckedListBoxControl);
                checkLock = true;
                switch (e.Index)
                {
                    case 0: SelectAllChecked(listBox, e); break;
                    case 1:
                    case 2: BlankNonBlankChecked(listBox, e); break;
                    default: CheckAllItemsChecked(listBox); break;
                }
            }
            finally
            {
                checkLock = false;
            }

        }

        void SelectAllChecked(CheckedListBoxControl listBox, DevExpress.XtraEditors.Controls.ItemCheckEventArgs e)
        {
            for (int i = 3; i < listBox.Items.Count; i++)
                listBox.Items[i].CheckState = e.State;
        }

        void BlankNonBlankChecked(CheckedListBoxControl listBox, DevExpress.XtraEditors.Controls.ItemCheckEventArgs e)
        {
            if (e.State == CheckState.Checked)
            {
                if (e.Index == 1)
                    listBox.Items[2].CheckState = CheckState.Unchecked;
                else
                    listBox.Items[1].CheckState = CheckState.Unchecked;
            }
        }

        void CheckAllItemsChecked(CheckedListBoxControl listBox)
        {
            int checkedCount = 0;
            listBox.Items[0].CheckState = CheckState.Indeterminate;
            for (int i = 3; i < listBox.Items.Count; i++)
                if (listBox.Items[i].CheckState == CheckState.Checked)
                    checkedCount++;
            if (checkedCount == 0)
                listBox.Items[0].CheckState = CheckState.Unchecked;
            if (checkedCount == listBox.Items.Count - 3)
                listBox.Items[0].CheckState = CheckState.Checked;
        }

        void CheckedComboBox_CloseUp(object sender, CloseUpEventArgs e)
        {
            e.AcceptValue = false;
            GridColumn column = View.FocusedColumn;
            PopupContainerForm form = (sender as IPopupControl).PopupWindow as PopupContainerForm;
            const int PopupContainerControlIndex = 3; //"DevExpress.XtraEditors.PopupContainerControl"

            PopupContainerControl popupControl = form.Controls[PopupContainerControlIndex] as PopupContainerControl;
            if (popupControl == null) return;

            CheckedListBoxControl control = popupControl.Controls[0] as CheckedListBoxControl;	
            List<Object> checkedValues = control.Items.GetCheckedValues();
            if (checkedValues.Count == 0)
            {
                View.ActiveFilter.Add(column, new ColumnFilterInfo());
                return;
            }
            if (control != null)
            {
                ColumnFilterInfo cfiBlanks = GetBlanksFilter(column, e.Value, checkedValues);
                ColumnFilterInfo cfi = GetValuesFilter(column, e.Value, checkedValues);
                if ((cfi != null) && (cfiBlanks != null))
                {
                    View.ActiveFilter.Add(column, new ColumnFilterInfo(cfi.FilterCriteria | cfiBlanks.FilterCriteria));
                    return;
                }
                if (cfi != null)
                {
                    View.ActiveFilter.Add(column, cfi);
                    return;
                }
                if (cfiBlanks != null)
                {
                    View.ActiveFilter.Add(column, cfiBlanks);
                    return;
                }
            }
        }

        ColumnFilterInfo GetValuesFilter(GridColumn column, object stringValues, List<object> checkedValues)
        {
            InOperator oper = new InOperator(new OperandProperty(column.FieldName));
            ColumnFilterInfo filterInfo = null;
            foreach (object item in checkedValues)
            {
                FilterItem fi = item as FilterItem;
                if ((fi != null) && !(fi.Value is int))
                    oper.Operands.Add(new OperandValue(fi.Value));
            }
            switch (oper.Operands.Count)
            {
                case 0:
                    break;
                case 1:
                    filterInfo = new ColumnFilterInfo(oper.LeftOperand == ((CriteriaOperator)oper.Operands[0]));
                    break;
                case 2:
                    filterInfo = new ColumnFilterInfo(
                        oper.LeftOperand == ((CriteriaOperator)oper.Operands[0])
                        |
                        oper.LeftOperand == ((CriteriaOperator)oper.Operands[1])
                        );

                    break;
                default:
                    filterInfo = new ColumnFilterInfo(oper);
                    break;
            }
            return filterInfo;
        }

        ColumnFilterInfo GetBlanksFilter(GridColumn column, object stringValues, List<object> checkedValues)
        {
            ColumnFilterInfo cfiBlanks = null;
            foreach (object item in checkedValues)
            {
                FilterItem fi = item as FilterItem;
                if (fi.Value is int)
                {
                    if ((int)fi.Value == 1)
                    {
                        cfiBlanks = new ColumnFilterInfo(new OperandProperty(column.FieldName).IsNull());
                        break;
                    }
                    if ((int)fi.Value == 2)
                    {
                        cfiBlanks = new ColumnFilterInfo(new OperandProperty(column.FieldName).IsNotNull());
                        break;
                    }
                }
            }
            return cfiBlanks;
        }

    }
}
