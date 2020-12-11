Public Class frmSecret

    Private Sub frmSecret_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.CenterToParent()
        Me.Icon = My.Resources.NewNullDCBearIcon

    End Sub

    Private Sub btnColor_Click(sender As Object, e As EventArgs) Handles btnColor.Click
        ColorDialog1.ShowDialog()
        Rx.SecretSettings = ColorTranslator.ToHtml(ColorDialog1.Color)
        MainformRef.ConfigFile.SaveFile()

    End Sub

End Class