Public Class frmChallengeSent

#Region "Properties"
#End Region

    Public Sub New(ByRef _mf As frmMain)
        InitializeComponent()

    End Sub

    Public Sub StartChallenge(ByVal _challanger As BEARPlayer)

        MainformRef.Challenger = _challanger
        With MainformRef
            .NetworkHandler.SendMessage("!," & .ConfigFile.Name & ",," & .ConfigFile.Port & "," & _challanger.game & ",1," & .ConfigFile.Peripheral, _challanger.ip)
        End With

        Me.Show()
    End Sub

    Private Sub frmChallengeSent_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.fan_icon_text
        ReloadTheme()

    End Sub

    Public Sub ReloadTheme()
        Me.BackgroundImage = BEARTheme.LoadImage(ThemeKeys.WaitBackground)
        ApplyThemeToControl(Label1, 2)
        ApplyThemeToControl(btnNope)
    End Sub

    Private Sub btnNope_Click(sender As Object, e As EventArgs) Handles btnNope.Click
        MainFormRef.NetworkHandler.SendMessage(">,C", MainFormRef.Challenger.ip)
        MainFormRef.EndSession("Denied")
        MainformRef.Focus()
    End Sub

#Region "Moving Window"
    Private newpoint As Point
    Private xpos1 As Integer
    Private ypos1 As Integer

    Private Sub pnlTopBorder_MouseDown(sender As Object, e As MouseEventArgs) Handles MyBase.MouseDown
        xpos1 = Control.MousePosition.X - Me.Location.X
        ypos1 = Control.MousePosition.Y - Me.Location.Y
    End Sub

    Private Sub pnlTopBorder_MouseMove(sender As Object, e As MouseEventArgs) Handles MyBase.MouseMove
        If e.Button = MouseButtons.Left Then
            newpoint = Control.MousePosition
            newpoint.X -= (xpos1)
            newpoint.Y -= (ypos1)
            Me.Location = newpoint
        End If
    End Sub

    Private Sub frmChallengeSent_VisibleChanged(sender As Object, e As EventArgs) Handles MyBase.VisibleChanged
        If Me.Visible Then
            Label1.Text = "Waiting for reply from " & vbCrLf & MainformRef.Challenger.name
        End If

    End Sub

    Private Sub frmChallengeSent_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        e.Cancel = True
        Me.Visible = False
    End Sub

#End Region


End Class