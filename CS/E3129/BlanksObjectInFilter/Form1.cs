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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BlanksObjectInFilter
{
    public partial class Form1 : Form
    {
        DataTable dt = new DataTable();
        public Form1()
        {
            dt.Columns.Add("Name", typeof(string));
            InitializeComponent();
            dt.Rows.Add(null as string);
            dt.Rows.Add("name2");
            dt.Rows.Add("name3");
            dt.Rows.Add("name4");
            dt.Rows.Add(null as string);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            gridControl1.DataSource = dt;
            AdvCheckedFilter newFilter = new AdvCheckedFilter(gridView1);
            newFilter.AdvColumns.Add(gridColumn1);
        }
    }
}
