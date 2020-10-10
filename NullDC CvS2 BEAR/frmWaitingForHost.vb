Public Class frmWaitingForHost

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        If Not MainformRef.Challenger Is Nothing Then frmMain.NetworkHandler.SendMessage(">,BO", frmMain.Challenger.ip)
        frmMain.EndSession("Denied")
    End Sub

    Private Sub frmWaitingForHost_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.NewNullDCBearIcon
    End Sub

    Private Sub frmWaitingForHost_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        e.Cancel = True
        Me.Visible = False
    End Sub

    Private Sub frmWaitingForHost_VisibleChanged(sender As Object, e As EventArgs) Handles MyBase.VisibleChanged
        ' If this is a CDI (Dreamcast game) ask for the VMU Data
        If Visible Then
            If MainformRef.Challenger.game.ToLower.EndsWith(".cdi") Then
                Label1.Text = "Syncing VMU..."
                MainformRef.NetworkHandler.SendMessage("V", MainformRef.Challenger.ip)

            End If
        End If

    End Sub

End Class