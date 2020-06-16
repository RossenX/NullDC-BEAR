Public Class frm360ce

    Private Sub RichTextBox1_LinkClicked(sender As Object, e As LinkClickedEventArgs) Handles RichTextBox1.LinkClicked
        Dim webAddress As String = "https://www.x360ce.com/"
        Process.Start(webAddress)
    End Sub

End Class
