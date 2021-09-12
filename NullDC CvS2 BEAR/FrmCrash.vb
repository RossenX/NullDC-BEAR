Public Class FrmCrash
    Private Sub FrmCrash_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.NewNullDCBearIcon
    End Sub

    Public Sub ShowDialog2(ByVal Message)
        Label1.Text = Message
        Me.CenterToScreen()
        Me.ShowDialog()

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Close()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Process.Start("https://www.google.com/search?q=happy+puppies&hl=en&tbm=isch")
        Me.Close()
    End Sub

End Class