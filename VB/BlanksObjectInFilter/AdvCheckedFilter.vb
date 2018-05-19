' Developer Express Code Central Example:
' How to add custom items into the filter checked list.
' 
' This example demonstrates how to add the "Blank" and "Non Blank" items into the
' filter checked list (the FilterPopupMode property is set to CheckedList)
' 
' We
' also suggest that you track the http://www.devexpress.com/scid=S131291
' suggestion to be notified when this feature will be implemented out of the box.
' 
' You can find sample updates and versions for different programming languages here:
' http://www.devexpress.com/example=E3129


Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Text
Imports System.Windows.Forms

Imports DevExpress.Data.Filtering
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraGrid.Columns
Imports DevExpress.XtraEditors.Controls
Imports DevExpress.XtraEditors.Popup
Imports DevExpress.XtraEditors
Imports DevExpress.Utils.Win

Namespace BlanksObjectInFilter
	Friend Class AdvCheckedFilter
		Private checkLock As Boolean

		Private _view As GridView
		Private _advColumns As BindingList(Of GridColumn)
		Public Property View() As GridView
			Get
				Return _view
			End Get
			Set(ByVal value As GridView)
				If _view Is value Then
					Return
				End If
				_view = value
			End Set
		End Property
		Public Property AdvColumns() As BindingList(Of GridColumn)
			Get
				Return _advColumns
			End Get
			Set(ByVal value As BindingList(Of GridColumn))
				If _advColumns Is value Then
					Return
				End If
				_advColumns = value
			End Set
		End Property

		Public Sub New(ByVal View As GridView)
			AdvColumns = New BindingList(Of GridColumn)()
			Me.View = View
			AddHandler View.ShowFilterPopupCheckedListBox, AddressOf View_ShowFilterPopupCheckedListBox

		End Sub
		Private Sub View_ShowFilterPopupCheckedListBox(ByVal sender As Object, ByVal e As DevExpress.XtraGrid.Views.Grid.FilterPopupCheckedListBoxEventArgs)
			If (Not _advColumns.Contains(e.Column)) Then
				Return
			End If
			e.CheckedComboBox.SelectAllItemVisible = False
			Dim fi As New FilterItem("(Select All)", 0)
			Dim item As New CheckedListBoxItem(fi)
			e.CheckedComboBox.Items.Insert(0, item)
			fi = New FilterItem("(Blanks)", 1)
			item = New CheckedListBoxItem(fi)
			e.CheckedComboBox.Items.Insert(1, item)
			fi = New FilterItem("(Non Blanks)", 2)
			item = New CheckedListBoxItem(fi)
			e.CheckedComboBox.Items.Insert(2, item)
			AddHandler e.CheckedComboBox.Popup, AddressOf CheckedComboBox_Popup
			AddHandler e.CheckedComboBox.CloseUp, AddressOf CheckedComboBox_CloseUp
		End Sub

		Private Sub CheckedComboBox_Popup(ByVal sender As Object, ByVal e As EventArgs)
			Dim popupControl As IPopupControl = TryCast(sender, IPopupControl)
			If popupControl Is Nothing Then
				Return
			End If
			Dim form As PopupContainerForm = TryCast(popupControl.PopupWindow, PopupContainerForm)
			If form Is Nothing Then
				Return
			End If
			Dim control As CheckedListBoxControl = FindListBoxControl(form)
			If control IsNot Nothing Then
				AddHandler control.ItemCheck, AddressOf Form_ItemCheck
				CheckAllItemsChecked(control)
				Dim operands As List(Of String) = GetOperandsString(View.FocusedColumn.FilterInfo.FilterCriteria)
				CheckItems(operands, control)
			End If
		End Sub

		Private Function FindListBoxControl(ByVal container As Control) As CheckedListBoxControl
			If TypeOf container Is CheckedListBoxControl Then
				Return TryCast(container, CheckedListBoxControl)
			End If
			For Each control As Control In container.Controls
				Dim listBox As CheckedListBoxControl = FindListBoxControl(control)
				If listBox IsNot Nothing Then
					Return TryCast(listBox, CheckedListBoxControl)
				End If
			Next control
			Return Nothing
		End Function

		Private Sub CheckItems(ByVal values As List(Of String), ByVal control As CheckedListBoxControl)
			For Each value As String In values
				For Each item As CheckedListBoxItem In control.Items
					If (TryCast(item.Value, FilterItem)).Text = value Then
						item.CheckState = CheckState.Checked
					End If
				Next item
			Next value
		End Sub

		Private Function GetOperandsString(ByVal crOperator As CriteriaOperator) As List(Of String)
			Dim operandList As New List(Of String)()
			If crOperator Is Nothing Then
				Return operandList
			End If
			If TypeOf crOperator Is OperandValue Then
				operandList.Add(CStr((TryCast(crOperator, OperandValue)).Value))
			End If
			If TypeOf crOperator Is InOperator Then
				Dim group As InOperator = TryCast(crOperator, InOperator)
				For Each operand As CriteriaOperator In group.Operands
					operandList.AddRange(GetOperandsString(operand))
				Next operand
			End If
			If TypeOf crOperator Is UnaryOperator Then
				Dim unary As UnaryOperator = TryCast(crOperator, UnaryOperator)
				If unary.OperatorType = UnaryOperatorType.IsNull Then
					operandList.Add("(Blanks)")
				End If
				If unary.OperatorType = UnaryOperatorType.Not Then
					operandList.Add("(Non Blanks)")
				End If
			End If
			If TypeOf crOperator Is GroupOperator Then
				Dim group As GroupOperator = TryCast(crOperator, GroupOperator)
				For Each operand As CriteriaOperator In group.Operands
					operandList.AddRange(GetOperandsString(operand))
				Next operand
			End If
			If TypeOf crOperator Is BinaryOperator Then
				Dim binOperator As BinaryOperator = TryCast(crOperator, BinaryOperator)
				operandList.AddRange(GetOperandsString(binOperator.RightOperand))
				operandList.AddRange(GetOperandsString(binOperator.LeftOperand))
			End If
			Return operandList

		End Function

		Private Sub Form_ItemCheck(ByVal sender As Object, ByVal e As DevExpress.XtraEditors.Controls.ItemCheckEventArgs)
			If checkLock Then
				Return
			End If
			Try
				Dim listBox As CheckedListBoxControl = (TryCast(sender, CheckedListBoxControl))
				checkLock = True
				Select Case e.Index
					Case 0
						SelectAllChecked(listBox, e)
					Case 1, 2
						BlankNonBlankChecked(listBox, e)
					Case Else
						CheckAllItemsChecked(listBox)
				End Select
			Finally
				checkLock = False
			End Try

		End Sub

		Private Sub SelectAllChecked(ByVal listBox As CheckedListBoxControl, ByVal e As DevExpress.XtraEditors.Controls.ItemCheckEventArgs)
			For i As Integer = 3 To listBox.Items.Count - 1
				listBox.Items(i).CheckState = e.State
			Next i
		End Sub

		Private Sub BlankNonBlankChecked(ByVal listBox As CheckedListBoxControl, ByVal e As DevExpress.XtraEditors.Controls.ItemCheckEventArgs)
			If e.State = CheckState.Checked Then
				If e.Index = 1 Then
					listBox.Items(2).CheckState = CheckState.Unchecked
				Else
					listBox.Items(1).CheckState = CheckState.Unchecked
				End If
			End If
		End Sub

		Private Sub CheckAllItemsChecked(ByVal listBox As CheckedListBoxControl)
			Dim checkedCount As Integer = 0
			listBox.Items(0).CheckState = CheckState.Indeterminate
			For i As Integer = 3 To listBox.Items.Count - 1
				If listBox.Items(i).CheckState = CheckState.Checked Then
					checkedCount += 1
				End If
			Next i
			If checkedCount = 0 Then
				listBox.Items(0).CheckState = CheckState.Unchecked
			End If
			If checkedCount = listBox.Items.Count - 3 Then
				listBox.Items(0).CheckState = CheckState.Checked
			End If
		End Sub

		Private Sub CheckedComboBox_CloseUp(ByVal sender As Object, ByVal e As CloseUpEventArgs)
			e.AcceptValue = False
			Dim column As GridColumn = View.FocusedColumn
			Dim form As PopupContainerForm = TryCast((TryCast(sender, IPopupControl)).PopupWindow, PopupContainerForm)
			Const PopupContainerControlIndex As Integer = 3 '"DevExpress.XtraEditors.PopupContainerControl"

			Dim popupControl As PopupContainerControl = TryCast(form.Controls(PopupContainerControlIndex), PopupContainerControl)
			If popupControl Is Nothing Then
				Return
			End If

			Dim control As CheckedListBoxControl = TryCast(popupControl.Controls(0), CheckedListBoxControl)
			Dim checkedValues As List(Of Object) = control.Items.GetCheckedValues()
			If checkedValues.Count = 0 Then
				View.ActiveFilter.Add(column, New ColumnFilterInfo())
				Return
			End If
			If control IsNot Nothing Then
				Dim cfiBlanks As ColumnFilterInfo = GetBlanksFilter(column, e.Value, checkedValues)
				Dim cfi As ColumnFilterInfo = GetValuesFilter(column, e.Value, checkedValues)
				If (cfi IsNot Nothing) AndAlso (cfiBlanks IsNot Nothing) Then
					View.ActiveFilter.Add(column, New ColumnFilterInfo(cfi.FilterCriteria Or cfiBlanks.FilterCriteria))
					Return
				End If
				If cfi IsNot Nothing Then
					View.ActiveFilter.Add(column, cfi)
					Return
				End If
				If cfiBlanks IsNot Nothing Then
					View.ActiveFilter.Add(column, cfiBlanks)
					Return
				End If
			End If
		End Sub

		Private Function GetValuesFilter(ByVal column As GridColumn, ByVal stringValues As Object, ByVal checkedValues As List(Of Object)) As ColumnFilterInfo
			Dim oper As New InOperator(New OperandProperty(column.FieldName))
			Dim filterInfo As ColumnFilterInfo = Nothing
			For Each item As Object In checkedValues
				Dim fi As FilterItem = TryCast(item, FilterItem)
				If (fi IsNot Nothing) AndAlso Not(TypeOf fi.Value Is Integer) Then
					oper.Operands.Add(New OperandValue(fi.Value))
				End If
			Next item
			Select Case oper.Operands.Count
				Case 0
				Case 1
					filterInfo = New ColumnFilterInfo(oper.LeftOperand Is (CType(oper.Operands(0), CriteriaOperator)))
				Case 2
					filterInfo = New ColumnFilterInfo(oper.LeftOperand Is (CType(oper.Operands(0), CriteriaOperator)) Or oper.LeftOperand Is (CType(oper.Operands(1), CriteriaOperator)))

				Case Else
					filterInfo = New ColumnFilterInfo(oper)
			End Select
			Return filterInfo
		End Function

		Private Function GetBlanksFilter(ByVal column As GridColumn, ByVal stringValues As Object, ByVal checkedValues As List(Of Object)) As ColumnFilterInfo
			Dim cfiBlanks As ColumnFilterInfo = Nothing
			For Each item As Object In checkedValues
				Dim fi As FilterItem = TryCast(item, FilterItem)
				If TypeOf fi.Value Is Integer Then
					If CInt(Fix(fi.Value)) = 1 Then
						cfiBlanks = New ColumnFilterInfo(New OperandProperty(column.FieldName).IsNull())
						Exit For
					End If
					If CInt(Fix(fi.Value)) = 2 Then
						cfiBlanks = New ColumnFilterInfo(New OperandProperty(column.FieldName).IsNotNull())
						Exit For
					End If
				End If
			Next item
			Return cfiBlanks
		End Function

	End Class
End Namespace
