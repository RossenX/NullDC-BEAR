Public Class frmChallengeGameSelect

    Public _Challenger As NullDCPlayer

    Public Sub New(ByRef _mf As frmMain)
        InitializeComponent()
    End Sub

    Public Sub StartChallenge(Optional ByVal _challenger As NullDCPlayer = Nothing)
        Me._Challenger = _challenger

        If Me.Visible Then
            Me.Focus()
        Else
            Me.Show(MainFormRef)
        End If

    End Sub

    Private Sub btnLetsGo_Click(sender As Object, e As EventArgs) Handles btnLetsGo.Click
        If cbGameList.Text = "" Then
            MainFormRef.NotificationForm.ShowMessage("No Game Selected")
            Exit Sub
        End If

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
        MainFormRef.ConfigFile.ReplayFile = ""
        MainFormRef.ConfigFile.SaveFile()

        MainformRef.NullDCLauncher.LaunchDC(cbGameList.SelectedValue, cbRegion.Text)
        Me.Close()

    End Sub

    Public Sub StartOnline()
        Dim GameRom As String = cbGameList.SelectedValue
        MainformRef.Challenger = New NullDCPlayer(_Challenger.name, _Challenger.ip, _Challenger.port, GameRom)
        MainformRef.ChallengeSentForm.StartChallenge(MainformRef.Challenger)
        Me.Close()

    End Sub

    Private Sub frmChallengeGameSelect_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        e.Cancel = True
        Me.Visible = False

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Close()
        MainformRef.Focus()
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
                cbRegion.Visible = False
                Label4.Visible = False

            Else
                cbHostType.Visible = True
                cbHostType.Visible = True
                Button2.Visible = True
                Label2.Visible = True
                cbRegion.Visible = True
                Label4.Visible = True

            End If

            If cbRegion.Text = "" Then
                cbRegion.SelectedIndex = 0
            End If

            If cbGameList.Items.Count > 0 Then
                'cbGameList.SelectedIndex = 0
            End If

            cbHostType.SelectedIndex = 0

            If MainformRef.ConfigFile.HostType = "1" Then
                cbHostType.SelectedIndex = 0
            Else
                cbHostType.SelectedIndex = 1
            End If

            tbFPS.Text = MainformRef.ConfigFile.FPSLimit
            Me.CenterToParent()
        Else
            _Challenger = Nothing
        End If

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If Not Application.OpenForms().OfType(Of frmLimiterHelp).Any Then
            frmLimiterHelp.Show(Me)
        Else
            frmLimiterHelp.Focus()
        End If
    End Sub

    Private Sub btnDLC_Click(sender As Object, e As EventArgs) Handles btnDLC.Click
        If Not Application.OpenForms().OfType(Of frmDLC).Any Then
            frmDLC.Show(Me)
            Me.Close()
        Else
            frmDLC.Focus()
            Me.Close()
        End If
    End Sub

End Class