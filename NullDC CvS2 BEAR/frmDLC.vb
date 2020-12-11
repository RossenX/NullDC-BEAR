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

    Dim DownloadClient As New WebClient
    Dim DownloadCanceled As Boolean = False
    Dim CurrentlySelectedGame As DownloadableGame = Nothing
    Dim CurrentlyDownloadingGame As DownloadableGame = Nothing
    Dim ExternalURLs As ArrayList = New ArrayList ' Used for the link to manually download games
    Dim RomFolders As ArrayList = New ArrayList ' Used for the Button to open the folder only
    Dim ExportCount = 0
    Dim DownloadSize = 0
    Dim Downloaded = 0

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()
    End Sub

    Private Sub frmDLC_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.NewNullDCBearIcon
        Me.CenterToParent()

        GetDownloadableGamesList()

        AddHandler DownloadClient.DownloadFileCompleted, AddressOf DownloadComplete
        AddHandler DownloadClient.DownloadProgressChanged, AddressOf DownloadProgress

        ' Search Panel
        Search_ListView.Columns.Item(0).Width = Search_ListView.Width * 0.85
        Search_ListView.Columns.Item(1).Width = 0
        Search_ListView.Columns.Item(2).Width = Search_ListView.Width * 0.15 - 25
        Search_ListView.Columns.Item(3).Width = 0
        Search_ListView.Columns.Item(4).Width = 0

        AddHandler Search_ListView.SelectedIndexChanged, AddressOf SelectedNewGameFromList

        ReloadTheme()

        
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

    Private Sub ArchiveDotOrgParse(ByVal URL As String)
        Dim StrippedURL = URL.Replace("&output=json", "")
        StrippedURL = StrippedURL.Replace("/details/", "/download/")

        Dim GamesListClient As New WebClient
        GamesListClient.Credentials = New NetworkCredential()
        GamesListClient.Headers.Add("user-agent", "MyRSSReader/1.0")

        GamesListClient.DownloadStringTaskAsync(URL)

        AddHandler GamesListClient.DownloadStringCompleted,
            Sub(ByVal sender As WebClient, e As DownloadStringCompletedEventArgs)

                Dim GameCount = 0
                Dim Exportedlist As New ArrayList
                Exportedlist.Add("DLCv=1")
                Exportedlist.Add("Platform=FDS")
                Exportedlist.Add("Disc=Nintendo Famicom Disk System DLC")
                Exportedlist.Add("Tab=Famicom Disk System")
                Exportedlist.Add("Folder=mednafen\roms\fds")
                Exportedlist.Add("Extract=0")
                Exportedlist.Add("ExternalURL=" & URL.Replace("&output=json", ""))

                If Not e.Error Is Nothing Then
                    MsgBox("Error Getting Downloadable Games List")
                    Exit Sub

                End If

                For Each _sp As String In e.Result.Split("""")
                    If _sp.Contains(".zip") Then
                        Dim _GameLink = StrippedURL & _sp.Replace("\/", "/").Replace("#", "%23")
                        Exportedlist.Add(_GameLink)
                        GameCount += 1

                    End If

                Next

                Dim ExportString = ""
                For i = 0 To Exportedlist.Count - 1
                    ExportString += Exportedlist(i) & vbNewLine
                Next

                'File.WriteAllText(MainformRef.NullDCPath & "\Export" & URL.Split("/")(URL.Split("/").Count - 1) & ".freedlc", ExportString)
                ExportCount += 1

            End Sub

    End Sub

    Private Sub GetDownloadableGamesList()
        Try
            GetRomPacks()
            If MainformRef.IsBeta Then ' Just in case i forget to remove this in the release
                'ArchiveDotOrgParse("https://archive.org/details/SonyPlaystation-EUR-Part2-Redump.org2019-06-05&output=json")
            End If

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

        If DownloadClient.IsBusy Then
            DownloadCanceled = True
            DownloadClient.CancelAsync()
            Exit Sub

        End If

        CurrentlyDownloadingGame = New DownloadableGame(CurrentlySelectedGame.URL, CurrentlySelectedGame.Name, CurrentlySelectedGame.Platform, CurrentlySelectedGame.Folder, CurrentlySelectedGame.Extract)

        Dim _tmp As String() = CurrentlyDownloadingGame.URL.Split("/")
        Dim DownloadingZipName = _tmp(_tmp.Count - 1).Replace(".zip", ".honey")

        Try
            DownloadClient.Credentials = New NetworkCredential()
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

            If Not Directory.Exists(MainformRef.NullDCPath & "\" & CurrentlyDownloadingGame.Folder) Then Directory.CreateDirectory(MainformRef.NullDCPath & "\" & CurrentlyDownloadingGame.Folder)

            DownloadClient.DownloadFileTaskAsync(CurrentlyDownloadingGame.URL, MainformRef.NullDCPath & "\" & CurrentlyDownloadingGame.Folder & "\" & DownloadingZipName)

            ProgressBar1.Visible = True
            btnDownload.Text = "Downloading... " & CurrentlyDownloadingGame.Name
            DownloadCanceled = False

            Console.WriteLine("Downloading: {0} {1}", CurrentlyDownloadingGame.Name, CurrentlyDownloadingGame.URL)
            ProgressBar1.Value = 0

        Catch ex As Exception
            MsgBox("Downlaod Error: " & ex.Message)
            btnDownload.Text = "Download"
            ProgressBar1.Visible = False

        End Try

    End Sub

    Private Sub DownloadProgress(ByVal sender As WebClient, e As DownloadProgressChangedEventArgs)
        Try
            ProgressBar1.Maximum = e.TotalBytesToReceive
            ProgressBar1.Value = e.BytesReceived
            Downloaded = e.BytesReceived
            DownloadSize = e.TotalBytesToReceive

            ProgressBar1.Visible = True

            Dim _NameLength = 20
            If CurrentlyDownloadingGame.Name.Length < 20 Then
                _NameLength = CurrentlyDownloadingGame.Name.Length

            End If
            btnDownload.Text = String.Format("Downloading... {0} (Click again to Cancel)" & vbNewLine & "({1}mb/{2}mb)", CurrentlyDownloadingGame.Name.Substring(0, _NameLength), Math.Round(e.BytesReceived / 1000000, 2), Math.Round(e.TotalBytesToReceive / 1000000, 2))

        Catch ex As Exception
            MsgBox("Error Downlaoding Game: " & ex.Message)

        End Try


    End Sub

    Private Sub DownloadComplete()
        Console.WriteLine("Download Done")
        Try
            Dim HoneyFilePath = ""
            Dim RomDirectory = ""

            Dim _tmp As String() = CurrentlyDownloadingGame.URL.Split("/")
            Dim DownloadedHoneyFile = _tmp(_tmp.Count - 1).Replace(".zip", ".honey")

            HoneyFilePath = MainformRef.NullDCPath & "\" & CurrentlyDownloadingGame.Folder & "\" & DownloadedHoneyFile
            RomDirectory = MainformRef.NullDCPath & "\" & CurrentlyDownloadingGame.Folder

            Dim InstallThread As Thread =
                New Thread(Sub()
                               If Not DownloadCanceled Then
                                   Try
                                       MainformRef.Invoke(Sub() btnDownload.Text = "Installing...")
                                       Dim RomFileName = RemoveAnnoyingRomNumbersFromString(WebUtility.UrlDecode(CurrentlyDownloadingGame.URL.Split("/")(CurrentlyDownloadingGame.URL.Split("/").Count - 1)))

                                       Select Case CurrentlyDownloadingGame.Extract
                                           Case "0" ' Do not Unzip
                                               If File.Exists(RomDirectory & "\" & RomFileName) Then
                                                   File.Delete(RomDirectory & "\" & RomFileName)
                                               End If
                                               File.Copy(HoneyFilePath, RomDirectory & "\" & RomFileName)
                                           Case "1" ' Unzip
                                               If File.Exists(HoneyFilePath) Then
                                                   ZipFile.ExtractToDirectory(HoneyFilePath, RomDirectory)
                                               End If

                                           Case "2" ' Unzip and place in own folder
                                               If Not Directory.Exists(RomDirectory & "\" & CurrentlyDownloadingGame.Name) Then
                                                   Directory.CreateDirectory(RomDirectory & "\" & CurrentlyDownloadingGame.Name)
                                               Else
                                                   Directory.Delete(RomDirectory & "\" & CurrentlyDownloadingGame.Name, True)
                                                   Directory.CreateDirectory(RomDirectory & "\" & CurrentlyDownloadingGame.Name)
                                               End If

                                               If File.Exists(HoneyFilePath) Then
                                                   ZipFile.ExtractToDirectory(HoneyFilePath, RomDirectory & "\" & CurrentlyDownloadingGame.Name)
                                               End If
                                           Case "3" ' Do not Unzip, but place in folder
                                               If Not Directory.Exists(RomDirectory & "\" & CurrentlyDownloadingGame.Name) Then
                                                   Directory.CreateDirectory(RomDirectory & "\" & CurrentlyDownloadingGame.Name)
                                               Else
                                                   Directory.Delete(RomDirectory & "\" & CurrentlyDownloadingGame.Name, True)
                                                   Directory.CreateDirectory(RomDirectory & "\" & CurrentlyDownloadingGame.Name)
                                               End If

                                               File.Copy(HoneyFilePath, RomDirectory & "\" & CurrentlyDownloadingGame.Name & "\" & RomFileName)
                                       End Select

                                       MainformRef.Invoke(Sub() MainformRef.NotificationForm.ShowMessage("Enjoy " & CurrentlyDownloadingGame.Name))
                                   Catch ex As Exception
                                       MsgBox("Rom install Error: " & ex.Message)
                                       CurrentlyDownloadingGame = Nothing

                                   End Try

                               End If

                               If File.Exists(HoneyFilePath) Then
                                   File.SetAttributes(HoneyFilePath, FileAttributes.Normal)
                                   File.Delete(HoneyFilePath)

                               End If

                               MainformRef.Invoke(
                               Sub()
                                   If Me.Visible Then
                                       ProgressBar1.Visible = False
                                       btnDownload.Text = "Download"
                                       DownloadCanceled = False
                                       CurrentlyDownloadingGame = Nothing
                                   End If
                                   CurrentlyDownloadingGame = Nothing
                               End Sub)
                               CurrentlyDownloadingGame = Nothing
                           End Sub)

            If Not Downloaded = DownloadSize And Not DownloadCanceled And Not Downloaded = 0 Then
                MsgBox("Download closed by the server, try again.")

                If File.Exists(HoneyFilePath) Then
                    File.SetAttributes(HoneyFilePath, FileAttributes.Normal)
                    File.Delete(HoneyFilePath)

                End If

                MainformRef.Invoke(
                               Sub()
                                   If Me.Visible Then
                                       ProgressBar1.Visible = False
                                       btnDownload.Text = "Download"
                                       DownloadCanceled = False
                                       CurrentlyDownloadingGame = Nothing
                                   End If
                                   CurrentlyDownloadingGame = Nothing
                               End Sub)

                Exit Sub
            Else
                InstallThread.Start()
            End If

        Catch ex As Exception
            MsgBox("Error Installing rom: " & ex.Message)

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
        If DownloadClient.IsBusy Then
            DownloadCanceled = True
            DownloadClient.CancelAsync()

        End If
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