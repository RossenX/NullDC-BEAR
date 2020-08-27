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

        MainformRef.ConfigFile.Status = "Offline"
        MainFormRef.ConfigFile.Game = cbGameList.SelectedValue
        MainformRef.ConfigFile.ReplayFile = ""
        MainformRef.ConfigFile.Host = "127.0.0.1"
        MainformRef.ConfigFile.SaveFile()

        MainformRef.GameLauncher(cbGameList.SelectedValue, cbRegion.Text)
        'MainformRef.NullDCLauncher.LaunchDC(cbGameList.SelectedValue, cbRegion.Text)

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

    Private Sub frmChallengeGameSelect_VisibleChanged(sender As Object, e As EventArgs) Handles MyBase.VisibleChanged
        If Visible Then
            If Not _Challenger Is Nothing Then
                cbRegion.Visible = False
                Label4.Visible = False
            Else
                cbRegion.Visible = True
                Label4.Visible = True
            End If

            If cbRegion.Text = "" Then
                cbRegion.SelectedIndex = 0
            End If

            Me.CenterToParent()
        Else
            _Challenger = Nothing
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