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

using System.Windows.Forms;

namespace BlanksObjectInFilter
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
