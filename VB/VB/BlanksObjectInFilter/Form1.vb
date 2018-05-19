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
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms

Namespace BlanksObjectInFilter
	Partial Public Class Form1
		Inherits Form
		Private dt As New DataTable()
		Public Sub New()
			dt.Columns.Add("Name", GetType(String))
			InitializeComponent()
			dt.Rows.Add(TryCast(Nothing, String))
			dt.Rows.Add("name2")
			dt.Rows.Add("name3")
			dt.Rows.Add("name4")
			dt.Rows.Add(TryCast(Nothing, String))
		End Sub

		Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
			gridControl1.DataSource = dt
			Dim newFilter As New AdvCheckedFilter(gridView1)
			newFilter.AdvColumns.Add(gridColumn1)
		End Sub
	End Class
End Namespace
