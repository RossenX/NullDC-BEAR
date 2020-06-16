Public Class frmChallengeGameSelect

    Public _Challenger As NullDCPlayer
    Dim MainFormRef As frmMain

    Public Sub New(ByRef _mf As frmMain)
        InitializeComponent()
        MainFormRef = _mf
    End Sub

    Public Sub StartChallenge(Optional ByVal _challenger As NullDCPlayer = Nothing)
        Me._Challenger = _challenger
        Visible = True

    End Sub

    Private Sub btnLetsGo_Click(sender As Object, e As EventArgs) Handles btnLetsGo.Click
        If _Challenger Is Nothing Then
            StartOffline()
        Else
            StartOnline()
        End If

    End Sub

    Public Sub StartOffline()

        Dim HostType As String = "0"
        If cbHostType.Text = "Audio Sync" Then HostType = "1"

        MainFormRef.ConfigFile.FPSLimit = tbFPS.Text
        MainFormRef.ConfigFile.HostType = HostType
        MainFormRef.ConfigFile.Status = "Offline"
        MainFormRef.ConfigFile.Game = cbGameList.SelectedValue
        MainFormRef.ConfigFile.SaveFile()

        MainFormRef.NullDCLauncher.LaunchDC(cbGameList.SelectedValue)
        Me.Close()

    End Sub

    Public Sub StartOnline()
        Dim GameRom As String = cbGameList.SelectedValue
        MainFormRef.Challenger = New NullDCPlayer(_Challenger.name, _Challenger.ip, _Challenger.port, GameRom)
        'MainFormRef.NetworkHandler.SendMessage("!," & MainFormRef.ConfigFile.Name & "," & MainFormRef.ConfigFile.IP & "," & MainFormRef.ConfigFile.Port & "," & GameRom & ",1", _Challenger.ip)
        MainFormRef.ChallengeSentForm.StartChallenge(MainFormRef.Challenger)

        Me.Close()
    End Sub

    Private Sub frmChallengeGameSelect_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        e.Cancel = True
        Me.Visible = False

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Close()

    End Sub

    Private Sub frmChallengeGameSelect_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.NewNullDCBearIcon

    End Sub

    Private Sub cbHostType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbHostType.SelectedIndexChanged
        If _Challenger Is Nothing Then
            If sender.Text = "Audio Sync" Then
                tbFPS.Visible = False
                Label3.Visible = False
            Else
                tbFPS.Visible = True
                Label3.Visible = True
            End If
        Else
            tbFPS.Visible = False
            Label3.Visible = False
        End If

    End Sub

    Private Sub frmChallengeGameSelect_VisibleChanged(sender As Object, e As EventArgs) Handles MyBase.VisibleChanged
        If Visible Then
            If Not _Challenger Is Nothing Then
                cbHostType.Visible = False
                cbHostType.Visible = False
                Button2.Visible = False
                Label2.Visible = False

            Else
                cbHostType.Visible = True
                cbHostType.Visible = True
                Button2.Visible = True
                Label2.Visible = True

            End If

            cbGameList.SelectedIndex = 0
            cbHostType.SelectedIndex = 0
            If MainFormRef.ConfigFile.HostType = "1" Then
                cbHostType.SelectedIndex = 0
            Else
                cbHostType.SelectedIndex = 1
            End If
            tbFPS.Text = MainFormRef.ConfigFile.FPSLimit
        Else
            _Challenger = Nothing
        End If

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        frmLimiterHelp.Show()

    End Sub

End Class