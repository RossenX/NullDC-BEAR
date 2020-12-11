Public Class frmSecret
    Dim m_Rnd As New Random

    Private Sub frmSecret_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.CenterToParent()
        Me.Icon = My.Resources.NewNullDCBearIcon

        Randomize()
        btnColor.BackColor = RandomQBColor()
        Randomize()
        btnColor2.BackColor = RandomQBColor()
        Randomize()
        btnColor3.BackColor = RandomQBColor()

    End Sub

    Private Sub btnColor_Click(sender As Button, e As EventArgs) Handles btnColor.Click, btnColor2.Click, btnColor3.Click
        Rx.SecretSettings = ColorTranslator.ToHtml(sender.BackColor)
        MainformRef.ConfigFile.SaveFile()
        Me.Close()

    End Sub

    Public Function RandomQBColor() As Color

        Return Color.FromArgb(255, m_Rnd.Next(255), m_Rnd.Next(255), m_Rnd.Next(255))

    End Function

    Private Sub TableLayoutPanel1_Click(sender As Object, e As EventArgs) Handles TableLayoutPanel1.DoubleClick
        ColorDialog1.ShowDialog()
        Rx.SecretSettings = ColorTranslator.ToHtml(ColorDialog1.Color)
        MainformRef.ConfigFile.SaveFile()
        Me.Close()

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Close()
    End Sub

    Private Sub TableLayoutPanel2_Click(sender As Object, e As EventArgs) Handles TableLayoutPanel2.Click
        Randomize()
        btnColor.BackColor = RandomQBColor()
        Randomize()
        btnColor2.BackColor = RandomQBColor()
        Randomize()
        btnColor3.BackColor = RandomQBColor()

    End Sub

End Class