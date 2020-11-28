Public Class frmDelayHelp
    Private Sub frmDelayHelp_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.NewNullDCBearIcon
        ApplyThemeToControl(RichTextBox1)
    End Sub
End Class