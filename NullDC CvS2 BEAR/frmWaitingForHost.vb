Public Class frmWaitingForHost

    Public VMUTimer As System.Windows.Forms.Timer = New System.Windows.Forms.Timer

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        If Not MainformRef.Challenger Is Nothing Then frmMain.NetworkHandler.SendMessage(">,BO", frmMain.Challenger.ip)
        frmMain.EndSession("Denied")
    End Sub

    Private Sub frmWaitingForHost_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.NewNullDCBearIcon
        AddHandler VMUTimer.Tick, Sub()
                                      btnRetryVMU.Visible = True
                                  End Sub

        ' Add any initialization after the InitializeComponent() call.
        Me.BackgroundImage = BEARTheme.LoadImage(ThemeKeys.WaitHostBackground)
        PictureBox1.Image = BEARTheme.LoadImage(ThemeKeys.WaitHostAnimation)

        ApplyThemeToControl(Label1)
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
            btnRetryVMU.Visible = False
            If MainformRef.GamesList(MainformRef.Challenger.game)(2) = "dc" Then ' Only if it's a DC game ask for VMU
                Label1.Text = "Syncing VMU..."
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