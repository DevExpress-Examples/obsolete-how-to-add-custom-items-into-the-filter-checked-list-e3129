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

Imports System.Windows.Forms

Namespace BlanksObjectInFilter
	Friend NotInheritable Class Program
		''' <summary>
		''' The main entry point for the application.
		''' </summary>
		Private Sub New()
		End Sub
		<STAThread> _
		Shared Sub Main()
			Application.EnableVisualStyles()
			Application.SetCompatibleTextRenderingDefault(False)
			Application.Run(New Form1())
		End Sub
	End Class
End Namespace
