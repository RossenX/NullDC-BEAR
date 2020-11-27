Public Class frmChallengeGameSelect

    Public _Challenger As BEARPlayer
    Dim SelectedGame = {"", ""}

    Public Sub New(ByRef _mf As frmMain)
        InitializeComponent()


        GameSelectContainer.BackColor = BEARTheme.LoadColor(ThemeKeys.PrimaryColor)
        Label1.BackColor = BEARTheme.LoadColor(ThemeKeys.SecondaryColor)
        Label1.ForeColor = BEARTheme.LoadColor(ThemeKeys.SecondaryFontColor)

        btnCancel.BackColor = BEARTheme.LoadColor(ThemeKeys.ButtonColor)
        btnCancel.ForeColor = BEARTheme.LoadColor(ThemeKeys.ButtonFontColor)

        btnLetsGo.BackColor = BEARTheme.LoadColor(ThemeKeys.ButtonColor)
        btnLetsGo.ForeColor = BEARTheme.LoadColor(ThemeKeys.ButtonFontColor)

        cbDelay.BackColor = BEARTheme.LoadColor(ThemeKeys.DropdownColor)
        cbDelay.ForeColor = BEARTheme.LoadColor(ThemeKeys.DropdownFontColor)

        Label2.ForeColor = BEARTheme.LoadColor(ThemeKeys.PrimaryFontColor)
        Label3.ForeColor = BEARTheme.LoadColor(ThemeKeys.PrimaryFontColor)
        Label4.ForeColor = BEARTheme.LoadColor(ThemeKeys.PrimaryFontColor)
    End Sub

    Public Sub StartChallenge(Optional ByVal _challenger As BEARPlayer = Nothing)
        Me._Challenger = _challenger

        If Me.Visible Then
            Me.Focus()
        Else
            Me.Show(MainFormRef)
        End If

    End Sub

    Public Sub btnLetsGo_Click(sender As Object, e As EventArgs) Handles btnLetsGo.Click
        If SelectedGame(0) = "" Then
            MainformRef.NotificationForm.ShowMessage("No Game Selected")
            Exit Sub
        End If

        Rx.platform = MainformRef.GamesList(SelectedGame(0))(2)

        If _Challenger Is Nothing Then
            StartOffline()
        Else
            StartOnline()
        End If

    End Sub

    Public Sub StartOffline()

        Select Case Rx.platform
            Case "na", "dc" 'NullDC
                MainformRef.ConfigFile.Status = "Offline"
                MainformRef.ConfigFile.Host = ""
            Case Else ' Mednafen
                Select Case cb_Serverlist.Text
                    Case "Offline"
                        MainformRef.ConfigFile.Status = "Offline"
                        MainformRef.ConfigFile.Host = ""
                    Case "Localhost"
                        MainformRef.ConfigFile.Status = "Hosting"
                        MainformRef.ConfigFile.Host = "127.0.0.1"
                    Case Else
                        MainformRef.ConfigFile.Status = "Public"
                        MainformRef.ConfigFile.Host = cb_Serverlist.SelectedValue
                End Select

        End Select

        MainformRef.ConfigFile.Game = SelectedGame(0)
        MainformRef.ConfigFile.ReplayFile = ""
        MainformRef.ConfigFile.SaveFile()
        MainformRef.ConfigFile.Delay = cbDelay.Text.Trim

        MainformRef.GameLauncher(SelectedGame(0), cbRegion.Text)

        Me.Close()

    End Sub

    Public Sub StartOnline()

        MainformRef.ConfigFile.Game = SelectedGame(0)

        MainformRef.Challenger = New BEARPlayer(_Challenger.name, _Challenger.ip, _Challenger.port, SelectedGame(0))
        MainformRef.ChallengeSentForm.StartChallenge(MainformRef.Challenger)
        Me.Close()

    End Sub

    Private Sub frmChallengeGameSelect_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        e.Cancel = True
        Me.Visible = False

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.Close()
        MainformRef.Focus()
    End Sub

    Private Sub frmChallengeGameSelect_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.NewNullDCBearIcon
        tb_mednafen.Visible = False
        tb_nulldc.Visible = False
    End Sub

    Public Sub SelectedIndexChange(sender As ListView, e As EventArgs)
        If sender.SelectedItems.Count = 0 Then
            SelectedGame = {"", ""}
        Else
            SelectedGame = {sender.SelectedItems(0).SubItems(1).Text, sender.SelectedItems(0).SubItems(0).Text}
        End If

        If SelectedGame(0) = "" Then
            Label1.Text = "Game Select"
        Else
            Label1.Text = SelectedGame(1)
        End If
    End Sub

    Private Sub frmChallengeGameSelect_VisibleChanged(sender As Object, e As EventArgs) Handles MyBase.VisibleChanged
        If Visible Then
            If Not _Challenger Is Nothing Then
                cbRegion.Visible = False
                Label4.Visible = False
                tb_nulldc.Visible = False
            Else
                cbRegion.Visible = True
                Label4.Visible = True
                tb_nulldc.Visible = True
                cbDelay.Text = "0"
            End If

            If cbRegion.Text = "" Then
                cbRegion.SelectedIndex = 0
            End If

            ShowExtraSettings()

            Me.CenterToParent()
        Else
            _Challenger = Nothing
        End If

    End Sub

    Private Sub btnDLC_Click(sender As Object, e As EventArgs)
        If Not Application.OpenForms().OfType(Of frmDLC).Any Then
            frmDLC.Show(Me)
            Me.Close()
        Else
            frmDLC.Focus()
            Me.Close()
        End If
    End Sub

    Private Sub cbDelay_KeyPress(sender As Object, e As KeyPressEventArgs) Handles cbDelay.KeyPress
        If Not (Asc(e.KeyChar) = 8) Then
            Dim allowedChars As String = "0123456789"
            If Not allowedChars.Contains(e.KeyChar.ToString.ToLower) Or (Asc(e.KeyChar) = 8) Then
                e.KeyChar = ChrW(0)
                e.Handled = True
            End If
        End If

    End Sub

    Private Sub tc_games_SelectedIndexChanged(sender As TabControl, e As EventArgs) Handles tc_games.SelectedIndexChanged
        ShowExtraSettings()
    End Sub

    Private Sub ShowExtraSettings()
        If _Challenger Is Nothing Then
            If Not tc_games.SelectedTab Is Nothing Then
                If tc_games.SelectedTab.Text = "Naomi" Or tc_games.SelectedTab.Text = "Dreamcast" Then
                    tb_nulldc.Visible = True
                    tb_mednafen.Visible = False
                Else
                    tb_nulldc.Visible = False
                    tb_mednafen.Visible = True
                End If
            End If
        Else
            tb_nulldc.Visible = False
            tb_mednafen.Visible = False
        End If

    End Sub


End Class