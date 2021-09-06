Public Class frmWaitingForHost

    Public VMUTimer As System.Windows.Forms.Timer = New System.Windows.Forms.Timer

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        AddHandler VMUTimer.Tick, Sub()
                                      If MainformRef.Challenger Is Nothing Then
                                          VMUTimer.Stop()
                                          btnRetryVMU.Visible = False
                                          Me.Close()
                                          Return
                                      End If

                                      VMUTimer.Stop()
                                      VMUTimer.Interval = 5000
                                      VMUTimer.Start()

                                      MainformRef.NetworkHandler.SendMessage("V", MainformRef.Challenger.ip)
                                      btnRetryVMU.Visible = True
                                  End Sub

    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        If Not MainformRef.Challenger Is Nothing Then MainformRef.NetworkHandler.SendMessage(">,BO", MainformRef.Challenger.ip)
        MainformRef.EndSession("Denied")
    End Sub

    Private Sub frmWaitingForHost_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.fan_icon_text
        ReloadTheme()

    End Sub

    Public Sub ReloadTheme()
        ' Add any initialization after the InitializeComponent() call.

        ApplyThemeToControl(lbWaitingForHost, 2)
        ApplyThemeToControl(btnRetryVMU)
        ApplyThemeToControl(btnCancel)
    End Sub

    Private Sub frmWaitingForHost_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        e.Cancel = True
        btnRetryVMU.Visible = False
        If VMUTimer.Enabled Then
            VMUTimer.Stop()
            VMUTimer.Interval = 5000
        End If
        Me.Visible = False
    End Sub


    Private Sub frmWaitingForHost_VisibleChanged(sender As Object, e As EventArgs) Handles MyBase.VisibleChanged
        ' If this is a CDI (Dreamcast game) ask for the VMU Data
        If Visible Then
            Me.BackgroundImage = BEARTheme.LoadImage(ThemeKeys.WaitHostBackground)
            PictureBox1.Image = BEARTheme.LoadImage(ThemeKeys.WaitHostAnimation)
            PictureBox1.Width = PictureBox1.Image.Width
            PictureBox1.Location = New Point(Me.Width / 2 - PictureBox1.Width / 2, 35)
            PictureBox1.Height = PictureBox1.Image.Height
            lbWaitingForHost.Location = New Point(Me.Width / 2 - lbWaitingForHost.Width / 2)

            btnRetryVMU.Visible = False
            Dim SyncVMU = False
            If MainformRef.Challenger.game.StartsWith("FC_DC") Or MainformRef.Challenger.game.StartsWith("FLY_DC") Or MainformRef.Challenger.game.StartsWith("DC") Then
                SyncVMU = True
            End If

            If SyncVMU Then ' Only if it's a DC game ask for VMU
                lbWaitingForHost.Text = "Syncing VMU..."
                MainformRef.NetworkHandler.SendMessage("V", MainformRef.Challenger.ip)
                VMUTimer.Interval = 5000
                VMUTimer.Start()
            End If
        End If

    End Sub

    Private Sub btnRetryVMU_Click(sender As Object, e As EventArgs) Handles btnRetryVMU.Click
        MainformRef.NetworkHandler.SendMessage("V", MainformRef.Challenger.ip)
    End Sub

End Class