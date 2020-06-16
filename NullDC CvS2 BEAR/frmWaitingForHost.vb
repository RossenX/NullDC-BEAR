Public Class frmWaitingForHost

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        frmMain.NetworkHandler.SendMessage(">,BO", frmMain.Challenger.ip)
        frmMain.EndSession("Denied")
    End Sub

    Private Sub frmWaitingForHost_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.NewNullDCBearIcon
    End Sub

    Private Sub frmWaitingForHost_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        e.Cancel = True
        Me.Visible = False
    End Sub

End Class