Public Class SimpleProgressBar

    Private Sub SimpleProgressBar_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SetValue(0)
    End Sub

    Public Delegate Sub setValue_Delegate(ByVal value As Int32)
    Public Sub SetValue(value As Int32)
        PictureBox1.Width = (Me.Width / 100) * value
    End Sub

End Class
