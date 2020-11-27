Public Class frmDelayHelp
    Private Sub frmDelayHelp_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.NewNullDCBearIcon
        RichTextBox1.BackColor = BEARTheme.LoadColor(ThemeKeys.PrimaryColor)
        RichTextBox1.ForeColor = BEARTheme.LoadColor(ThemeKeys.PrimaryFontColor)

    End Sub
End Class