Imports System.IO
Imports System.Net
Imports System.Threading

' freedlc Format
'DLCv= <Version>
'Platform= <Short Platform Name> NA/DC
'Disc= <Description>
'Tab= <Tab Name>
'Folder= <Relative Rom Folder Path>
'Extract= <0 No Extract, 1 Extract, 2 Extract and place in folder, 3 do not extract and place in folder>
'ExternalURL= <URL when clicking the blue 'get manually' text>

Public Class frmDLC

    Dim DownloadCanceled As Boolean = False
    Dim CurrentlySelectedGame As DownloadableGame = Nothing
    Dim ExternalURLs As ArrayList = New ArrayList ' Used for the link to manually download games
    Dim RomFolders As ArrayList = New ArrayList ' Used for the Button to open the folder only
    Dim DownloadSize = 0
    Dim Downloaded = 0

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()
    End Sub

    Private Sub frmDLC_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.fan_icon_text
        Me.CenterToParent()

        GetDownloadableGamesList()

        ' Search Panel
        Search_ListView.Columns.Item(0).Width = Search_ListView.Width * 0.85
        Search_ListView.Columns.Item(1).Width = 0
        Search_ListView.Columns.Item(2).Width = Search_ListView.Width * 0.15 - 25
        Search_ListView.Columns.Item(3).Width = 0
        Search_ListView.Columns.Item(4).Width = 0

        AddHandler Search_ListView.SelectedIndexChanged, AddressOf SelectedNewGameFromList

        ReloadTheme()

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

    End Sub

    Public Sub ReloadTheme()
        ApplyThemeToControl(DLCContainer, 1)
        ApplyThemeToControl(btnDownload)
        ApplyThemeToControl(btnRomsFolder)
        ApplyThemeToControl(btnClose)
        ApplyThemeToControl(MenuStrip1)
        ApplyThemeToControl(lbDisclaimer)
        ApplyThemeToControl(lbSearch)
        ApplyThemeToControl(btnSearch)

        For Each _tab As TabPage In tc_games.TabPages
            ApplyThemeToControl(_tab.Controls.OfType(Of ListView).First, 2)
        Next
    End Sub

    Private Sub GetDownloadableGamesList()
        Try
            GetRomPacks()

        Catch ex As Exception
            MsgBox("Error Getting RomPacks: " & ex.Message)

        End Try
        btnRomsFolder.Text = "Open " & tc_games.SelectedTab.Text & " Roms Folder"

    End Sub

    Private Sub GetRomPacks()
        If Not Directory.Exists(MainformRef.NullDCPath & "\DLC") Then
            Directory.CreateDirectory(MainformRef.NullDCPath & "\DLC")
        End If

        Dim Files = Directory.GetFiles(MainformRef.NullDCPath & "\DLC", "*.freedlc", SearchOption.TopDirectoryOnly)
        ExternalURLs.Add("https://www.google.com/")
        RomFolders.Add("")
        ' Get allt he freedlc files and parse them into their own tabs and lists
        For Each _file In Files
            Dim _lines = File.ReadAllLines(_file)
            Dim TabName = _lines(3).Split("=")(1)

            tc_games.TabPages.Add(TabName)
            tc_games.TabPages.Item(tc_games.TabCount - 1).BackColor = BEARTheme.LoadColor(ThemeKeys.TertiaryColor)
            tc_games.TabPages.Item(tc_games.TabCount - 1).ForeColor = BEARTheme.LoadColor(ThemeKeys.TertiaryFontColor)

            Dim tmpListView As New ListView
            ApplyThemeToControl(tmpListView, 2)
            tmpListView.Dock = DockStyle.Fill
            tmpListView.MultiSelect = False
            tmpListView.View = View.Details
            tmpListView.HeaderStyle = ColumnHeaderStyle.None
            tmpListView.FullRowSelect = True
            tmpListView.HideSelection = False
            tmpListView.Parent = tc_games.TabPages.Item(tc_games.TabCount - 1)

            tmpListView.Columns.Add("Name")
            tmpListView.Columns.Add("URL")
            tmpListView.Columns.Add("Platform")
            tmpListView.Columns.Add("folder")
            tmpListView.Columns.Add("extract")
            tmpListView.Columns.Item(0).Width = tmpListView.Width - 25
            tmpListView.Columns.Item(1).Width = 0
            tmpListView.Columns.Item(2).Width = 0
            tmpListView.Columns.Item(3).Width = 0
            tmpListView.Columns.Item(4).Width = 0

            ExternalURLs.Add(_lines(6).Split("=")(1))
            RomFolders.Add(_lines(4).Split("=")(1))

            AddHandler tmpListView.SelectedIndexChanged, AddressOf SelectedNewGameFromList

            For i = 7 To _lines.Count - 1
                Dim url = _lines(i)
                Dim tmpSplit = _lines(i).Split("/")
                Dim tmpExtention = tmpSplit(tmpSplit.Count - 1).Split(".")
                Dim GameFormatedName = tmpSplit(tmpSplit.Count - 1).Replace("." & tmpExtention(tmpExtention.Count - 1), "")
                Dim name = WebUtility.UrlDecode(GameFormatedName.Replace("+", "%2B")).Replace("_", " ").Replace("# NES", "")
                name = RemoveAnnoyingRomNumbersFromString(name)

                Dim _it As New ListViewItem(name)
                _it.SubItems.Add(url)
                _it.SubItems.Add(_lines(1).Split("=")(1)) ' Platform
                _it.SubItems.Add(_lines(4).Split("=")(1)) ' Folder
                _it.SubItems.Add(_lines(5).Split("=")(1)) ' Extract
                tmpListView.Items.Add(_it)

            Next

            tmpListView.Sorting = SortOrder.Ascending
            tmpListView.Sort()
        Next

    End Sub

    Private Sub btnDownload_Click(sender As Object, e As EventArgs) Handles btnDownload.Click
        If CurrentlySelectedGame Is Nothing Then Exit Sub
        If CurrentlySelectedGame.Folder.Trim = "" Or CurrentlySelectedGame.URL.Trim = "" Or CurrentlySelectedGame.Name.Trim = "" Then Exit Sub

        Try
            If Not Application.OpenForms().OfType(Of frmDownloading).Any Then
                frmDownloading.Show()
                frmDownloading.AddDownload(CurrentlySelectedGame.URL, CurrentlySelectedGame.Folder & "\" & CurrentlySelectedGame.Name & ".honey", CurrentlySelectedGame.Extract)
            Else
                frmDownloading.AddDownload(CurrentlySelectedGame.URL, CurrentlySelectedGame.Folder & "\" & CurrentlySelectedGame.Name & ".honey", CurrentlySelectedGame.Extract)
            End If

        Catch ex As Exception
            Me.Invoke(Sub()
                          MsgBox("Downlaod Error: " & ex.Message)
                      End Sub)

            btnDownload.Text = "Download"
            ProgressBar1.Visible = False

        End Try

    End Sub

    Private Sub lnkRoms_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lnkRoms.LinkClicked

        If Uri.IsWellFormedUriString(ExternalURLs(tc_games.SelectedIndex), UriKind.Absolute) Then
            Process.Start(ExternalURLs(tc_games.SelectedIndex))
        Else
            MsgBox("No Valid External URL in the DLC Pack")
        End If

    End Sub

    Private Sub btnRomsFolder_Click(sender As Object, e As EventArgs) Handles btnRomsFolder.Click
        If Not Directory.Exists(MainformRef.NullDCPath & "\" & RomFolders(tc_games.SelectedIndex)) Then Directory.CreateDirectory(MainformRef.NullDCPath & "\" & RomFolders(tc_games.SelectedIndex))
        Process.Start(MainformRef.NullDCPath & "\" & RomFolders(tc_games.SelectedIndex))

    End Sub

    Private Sub SelectedNewGameFromList(sender As ListView, e As EventArgs)
        If sender.SelectedItems.Count = 0 Then
            CurrentlySelectedGame = Nothing
        Else
            CurrentlySelectedGame = New DownloadableGame(sender.SelectedItems(0).SubItems(1).Text, sender.SelectedItems(0).SubItems(0).Text, sender.SelectedItems(0).SubItems(2).Text, sender.SelectedItems(0).SubItems(3).Text, sender.SelectedItems(0).SubItems(4).Text)

        End If
    End Sub

    Private Sub tc_games_SelectedIndexChanged(sender As TabControl, e As EventArgs) Handles tc_games.SelectedIndexChanged
        btnRomsFolder.Text = "Open " & sender.SelectedTab.Text & " Roms Folder"

    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs)
        Process.Start("https://cdromance.com/dc-iso/?sorted=downloads")

    End Sub

    Private Sub frmDLC_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        My.Application.OpenForms(0).Activate()

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()

    End Sub

    Private Sub DLCCreatorToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DLCCreatorToolStripMenuItem.Click

        If Not Application.OpenForms().OfType(Of frmDLCCreator).Any Then
            frmDLCCreator.Show(Me)
        Else
            frmDLCCreator.Focus()
        End If
    End Sub

    Private Sub MultidiskPlaylistCreatorToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MultidiskPlaylistCreatorToolStripMenuItem.Click
        If Not Application.OpenForms().OfType(Of frmMultiDiskCreator).Any Then
            frmMultiDiskCreator.Show(Me)
        Else
            frmMultiDiskCreator.Focus()
        End If

    End Sub

    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        Search()
    End Sub

    Private Sub Search()
        Search_ListView.Items.Clear()
        tc_games.SelectedIndex = 0
        If TextBox1.Text.Trim = "" Then Exit Sub

        For Each _file As String In Directory.GetFiles(MainformRef.NullDCPath & "\DLC", "*.freedlc", SearchOption.TopDirectoryOnly)
            Console.WriteLine(_file)

            Dim _DLCFile = File.ReadAllLines(_file)
            For i = 7 To _DLCFile.Count - 1

                Dim url = _DLCFile(i)
                Dim tmpSplit = _DLCFile(i).Split("/")
                Dim tmpExtention = tmpSplit(tmpSplit.Count - 1).Split(".")
                Dim GameFormatedName = tmpSplit(tmpSplit.Count - 1).Replace("." & tmpExtention(tmpExtention.Count - 1), "")
                Dim name = WebUtility.UrlDecode(GameFormatedName.Replace("+", "%2B")).Replace("_", " ").Replace("# NES", "")
                name = RemoveAnnoyingRomNumbersFromString(name)

                Dim Match As Boolean = True
                For Each _searchwords In TextBox1.Text.Split(" ")
                    If Not name.ToLower.Replace(" ", "").Contains(_searchwords.ToLower) Then
                        Match = False
                        Exit For
                    End If
                Next

                If Match Then
                    Dim _it As New ListViewItem(name)
                    _it.SubItems.Add(url)
                    _it.SubItems.Add(_DLCFile(1).Split("=")(1)) ' Platform
                    _it.SubItems.Add(_DLCFile(4).Split("=")(1)) ' Folder
                    _it.SubItems.Add(_DLCFile(5).Split("=")(1)) ' Extract
                    Search_ListView.Items.Add(_it)
                End If

            Next
        Next

        Search_ListView.Sorting = SortOrder.Ascending
        Search_ListView.Sort()

    End Sub

    Private Sub TextBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox1.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.Handled = True
            e.SuppressKeyPress = True
            Search()
        End If

    End Sub

End Class

Class DownloadableGame

    Public URL As String = ""
    Public Name As String = ""
    Public Platform As String = ""
    Public Folder As String = ""
    Public Extract As String = ""

    Public Sub New(ByVal _url As String, ByVal _name As String, ByVal _platform As String, ByVal _folder As String, ByVal _extract As String)
        URL = _url
        Name = _name
        Platform = _platform
        Folder = _folder
        Extract = _extract

    End Sub

End Class