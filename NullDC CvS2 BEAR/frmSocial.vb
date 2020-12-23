Public Class frmSocial

    Private Sub frmSocial_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.NewNullDCBearIcon
        Me.CenterToParent()


        ReloadUsers()

    End Sub

    Private Sub ReloadUsers()
        GaggedUsers.Items.Clear()
        For Each _g In Rx.GaggedUsers.Keys
            Dim _tmp As New ListViewItem(Rx.GaggedUsers(_g))
            _tmp.SubItems.Add(_g)
            GaggedUsers.Items.Add(_tmp)

        Next

    End Sub

    Private Sub ContextMenuStrip1_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles ContextMenuStrip1.Opening
        If GaggedUsers.SelectedItems.Count = 0 Then SendKeys.Send("{ESC}")

    End Sub

    Private Sub ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem1.Click
        If GaggedUsers.SelectedItems.Count > 0 Then
            UnGagUser(GaggedUsers.SelectedItems(0).SubItems(1).Text)
            SaveGaggedUsers()
            ReloadUsers()
            MainformRef.NetworkHandler.SendMessage("?,")

        End If

    End Sub

End Class