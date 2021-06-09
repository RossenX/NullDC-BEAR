Imports System.IO

Public Class frmChallengeGameSelect

    Public _Challenger As BEARPlayer
    Dim SelectedGame = {"", ""}

    Public Sub New(ByRef _mf As frmMain)
        InitializeComponent()

        tb_nulldc.Visible = False
        tb_mednafen.Visible = False
    End Sub

    Public Sub StartChallenge(Optional ByVal _challenger As BEARPlayer = Nothing)
        Me._Challenger = _challenger

        If Me.Visible Then
            Me.Focus()
        Else
            Me.Show(MainformRef)
        End If

    End Sub

    Public Sub btnLetsGo_Click(sender As Object, e As EventArgs) Handles btnLetsGo.Click
        If SelectedGame(0) = "" Then
            MainformRef.NotificationForm.ShowMessage("No Game Selected")
            Exit Sub
        End If

        Rx.MultiTap = cb_Multitap.SelectedIndex
        Console.WriteLine("Multitap=" & Rx.MultiTap)
        Rx.platform = MainformRef.GamesList(SelectedGame(0))(2)
        MainformRef.MednafenLauncher.IsHost = True

        'File.ReadAllLines(MainformRef.NullDCPath & "\recent.glist")
        If File.Exists(MainformRef.NullDCPath & "\recent.glist") Then
            Dim RecentGames = File.ReadAllText(MainformRef.NullDCPath & "\recent.glist").Replace(SelectedGame(0) & vbNewLine, "")
            Dim RecentGamesCount = RecentGames.Split(vbNewLine)

            RecentGames = ""

            For i = 0 To 49
                If i < RecentGamesCount.Count - 1 Then
                    RecentGames += RecentGamesCount(i) & vbNewLine
                End If
            Next
            RecentGames = SelectedGame(0) & vbNewLine & RecentGames
            File.WriteAllText(MainformRef.NullDCPath & "\recent.glist", RecentGames)
        Else
            File.WriteAllText(MainformRef.NullDCPath & "\recent.glist", SelectedGame(0) & vbNewLine)
        End If

        If tb_gamekey.Text.Trim.Length > 0 And MainformRef.ConfigFile.NoKey = 0 Then
            Rx.EEPROM = tb_gamekey.Text.Trim
        Else
            Rx.EEPROM = ""
        End If

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
            Case "n64" 'Mupen64Plus
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

    Private Sub frmChallengeGameSelect_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.fan_icon_text
        tb_mednafen.Visible = False
        tb_nulldc.Visible = False
        cb_Multitap.SelectedIndex = 0

        ReloadTheme()

    End Sub

    Public Sub ReloadTheme()
        GameSelectContainer.BackColor = BEARTheme.LoadColor(ThemeKeys.PrimaryColor)

        ApplyThemeToControl(btnLetsGo)
        ApplyThemeToControl(cbDelay)
        ApplyThemeToControl(Label2)
        ApplyThemeToControl(Label3)
        ApplyThemeToControl(Label4)
        ApplyThemeToControl(btnLetsGo)
        ApplyThemeToControl(MenuStrip1)
        ApplyThemeToControl(cbRegion)
        ApplyThemeToControl(cb_Serverlist)
        ApplyThemeToControl(Label1)
        ApplyThemeToControl(Label5)
        ApplyThemeToControl(cb_Multitap)
        ApplyThemeToControl(tb_gamekey)
        ApplyThemeToControl(cb_nokey)

        For Each _tab As TabPage In tc_games.TabPages
            ApplyThemeToControl(_tab.Controls.OfType(Of ListView).First, 2)
        Next

    End Sub

    Public Sub SelectedIndexChange(sender As ListView, e As EventArgs)
        If sender.SelectedItems.Count = 0 Then
            SelectedGame = {"", ""}
        Else
            SelectedGame = {sender.SelectedItems(0).SubItems(1).Text, sender.SelectedItems(0).SubItems(0).Text}
        End If

        ShowExtraSettings(sender, e)
    End Sub

    Private Sub frmChallengeGameSelect_VisibleChanged(sender As Object, e As EventArgs) Handles MyBase.VisibleChanged
        If Visible Then
            PopulateGameLists()

            If _Challenger Is Nothing Then
                cbDelay.Text = MainformRef.ConfigFile.SimulatedDelay
                cbRegion.SelectedIndex = MainformRef.ConfigFile.Region
                cb_nokey.Checked = MainformRef.ConfigFile.NoKey
                Label3.Visible = True
                cb_Serverlist.Visible = True
            Else
                cbDelay.Text = MainformRef.ConfigFile.SimulatedDelay
                cbRegion.SelectedIndex = MainformRef.ConfigFile.Region
                cb_nokey.Checked = MainformRef.ConfigFile.NoKey
                Label3.Visible = False
                cb_Serverlist.Visible = False
            End If

            If cbRegion.Text = "" Then
                cbRegion.SelectedIndex = 0
            End If

            'ShowExtraSettings()
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
        For Each _tab As TabPage In tc_games.TabPages
            Dim tmp2ListView As ListView = _tab.Controls.OfType(Of ListView).First
            tmp2ListView.SelectedItems.Clear()

        Next
        SelectedGame = {"", ""}
        ShowExtraSettings(Nothing, Nothing)
    End Sub

    Private Sub ShowExtraSettings(ByRef sender As ListView, ByRef e As EventArgs)

        If sender Is Nothing Then
            tb_mednafen.Visible = False
            tb_nulldc.Visible = False
            Exit Sub
        End If

        If sender.SelectedItems.Count > 0 Then
            Console.WriteLine(sender.SelectedItems(0).SubItems(1).Text)
            If MainformRef.GamesList.ContainsKey(sender.SelectedItems(0).SubItems(1).Text) Then

                Select Case MainformRef.GamesList(sender.SelectedItems(0).SubItems(1).Text)(2)
                    Case "na", "dc"
                        tb_mednafen.Visible = False

                        If _Challenger Is Nothing Then
                            tb_nulldc.Visible = True
                        Else
                            tb_nulldc.Visible = False
                        End If
                    Case "n64"
                        tb_nulldc.Visible = False
                        tb_mednafen.Visible = False

                    Case Else
                        tb_nulldc.Visible = False
                        tb_mednafen.Visible = True

                End Select

            Else
                tb_nulldc.Visible = False
                tb_mednafen.Visible = False

            End If
        Else
            tb_nulldc.Visible = False
            tb_mednafen.Visible = False

        End If

    End Sub

    Private Sub DLCToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DLCToolStripMenuItem.Click
        If Not Application.OpenForms().OfType(Of frmDLC).Any Then
            frmDLC.Show(Me)
        Else
            frmDLC.Focus()
        End If

    End Sub

    Public Sub PopulateGameLists()

        tc_games.TabPages.Clear()
        SelectedGame = {"", ""}

        ' Add the Recent GamesList
        If Not File.Exists(MainformRef.NullDCPath & "\recent.glist") Then File.CreateText(MainformRef.NullDCPath & "\recent.glist").Close()

        tc_games.TabPages.Add("Recent")
        Dim RecentListVIew As New ListView
        ApplyThemeToControl(RecentListVIew, 2)
        RecentListVIew.Dock = DockStyle.Fill
        RecentListVIew.MultiSelect = False
        RecentListVIew.View = View.Details
        RecentListVIew.HeaderStyle = ColumnHeaderStyle.None
        RecentListVIew.FullRowSelect = True
        RecentListVIew.Parent = tc_games.TabPages.Item(tc_games.TabCount - 1)
        RecentListVIew.HideSelection = False

        RecentListVIew.Columns.Add("Game Name")
        RecentListVIew.Columns.Add("Rom Name")
        RecentListVIew.Columns.Item(0).Width = RecentListVIew.Width - 25
        RecentListVIew.Columns.Item(1).Width = 0

        For Each _line In File.ReadAllLines(MainformRef.NullDCPath & "\recent.glist")
            If MainformRef.GamesList.ContainsKey(_line) Then
                Dim _it As New ListViewItem(MainformRef.GamesList(_line)(0).ToString)
                _it.SubItems.Add(_line)
                RecentListVIew.Items.Add(_it)

            End If
        Next

        AddHandler RecentListVIew.SelectedIndexChanged, AddressOf MainformRef.GameSelectForm.SelectedIndexChange
        'AddHandler RecentListVIew.Click, AddressOf MainformRef.GameSelectForm.SelectedIndexChange
        AddHandler RecentListVIew.DoubleClick, AddressOf MainformRef.GameSelectForm.btnLetsGo_Click

        'RomFileName - Game Name, Rom Path, Platform, Hash
        For i = 0 To MainformRef.GamesList.Count - 1

            Dim TabIndex = -1
            Dim TabName = ""

            Select Case MainformRef.GamesList(MainformRef.GamesList.Keys(i))(2)
                Case "na"
                    TabName = "Naomi"
                Case "dc"
                    TabName = "Dreamcast"
                Case "sg"
                    TabName = "Genesis"
                Case "ss"
                    TabName = "Saturn"
                Case "psx"
                    TabName = "PSX"
                Case "nes"
                    TabName = "NES"
                Case "snes"
                    TabName = "SNES"
                Case "ngp"
                    TabName = "Neo-Geo Pocket"
                Case "gba"
                    TabName = "GBA"
                Case "gbc"
                    TabName = "GBC"
                Case "fds"
                    TabName = "Famicom Disk System"
                Case "sms"
                    TabName = "Sega Master System"
                Case "n64"
                    TabName = "N64"
                Case Else
                    TabName = "Unknown"
                    Console.WriteLine("No System?")
            End Select

            ' Check if Tab Exists
            For Each _tab As TabPage In tc_games.TabPages
                If _tab.Text = TabName Then
                    TabIndex = _tab.TabIndex
                    Exit For
                End If
            Next

            ' Tab not found, create the tab
            If TabIndex = -1 Then
                tc_games.TabPages.Add(TabName)

                Dim tmpListView As New ListView
                ApplyThemeToControl(tmpListView, 2)
                tmpListView.Dock = DockStyle.Fill
                tmpListView.MultiSelect = False
                tmpListView.View = View.Details
                tmpListView.HeaderStyle = ColumnHeaderStyle.None
                tmpListView.FullRowSelect = True
                tmpListView.Parent = tc_games.TabPages.Item(tc_games.TabCount - 1)
                tmpListView.HideSelection = False

                tmpListView.Columns.Add("Game Name")
                tmpListView.Columns.Add("Rom Name")
                tmpListView.Columns.Item(0).Width = tmpListView.Width - 25
                tmpListView.Columns.Item(1).Width = 0

                TabIndex = tc_games.TabPages.Count - 1

                AddHandler tmpListView.SelectedIndexChanged, AddressOf MainformRef.GameSelectForm.SelectedIndexChange
                'AddHandler tmpListView.Click, AddressOf MainformRef.GameSelectForm.Click
                AddHandler tmpListView.DoubleClick, AddressOf MainformRef.GameSelectForm.btnLetsGo_Click
            End If

            Dim tmp2ListView = tc_games.TabPages.Item(TabIndex).Controls.OfType(Of ListView).First

            Dim RomName = ""
            Dim GameName = ""

            Dim _it As New ListViewItem(MainformRef.GamesList(MainformRef.GamesList.Keys(i))(0).ToString)

            _it.SubItems.Add(MainformRef.GamesList.Keys(i).ToString)
            tmp2ListView.Items.Add(_it)
        Next

    End Sub

    Private Sub MultidiscPlaylistCreatorToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MultidiscPlaylistCreatorToolStripMenuItem.Click
        If Not Application.OpenForms().OfType(Of frmMultiDiskCreator).Any Then
            frmMultiDiskCreator.Show(Me)
        Else
            frmMultiDiskCreator.Focus()
        End If

    End Sub

    Private Sub tb_gamekey_KeyPress(sender As Object, e As KeyPressEventArgs) Handles tb_gamekey.KeyPress
        If Not (Asc(e.KeyChar) = 8) Then
            Dim allowedChars As String = "abcdefghijklmnopqrstuvwxyz1234567890"
            If Not allowedChars.Contains(e.KeyChar.ToString.ToLower) Or (Asc(e.KeyChar) = 8) Then
                e.KeyChar = ChrW(0)
                e.Handled = True
            End If
        End If
    End Sub

    Private Sub tc_games_SelectedIndexChanged(sender As Object, e As EventArgs) Handles tc_games.SelectedIndexChanged

    End Sub

    Private Sub cbDelay_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbDelay.SelectedIndexChanged
        MainformRef.ConfigFile.SimulatedDelay = cbDelay.Text.Trim
        MainformRef.ConfigFile.SaveFile(False)

    End Sub

    Private Sub cbRegion_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbRegion.SelectedIndexChanged
        MainformRef.ConfigFile.Region = cbRegion.SelectedIndex
        MainformRef.ConfigFile.SaveFile(False)

    End Sub

    Private Sub cb_nokey_CheckedChanged(sender As CheckBox, e As EventArgs) Handles cb_nokey.CheckedChanged
        MainformRef.ConfigFile.NoKey = sender.Checked

    End Sub
End Class