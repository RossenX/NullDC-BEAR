Imports System.IO
Imports System.Net
Imports System.Threading

Public Class frmDLC

    Dim DownloadClient As New WebClient
    Dim DownloadingGameName As String = ""
    Dim DownloadingZipName As String = ""
    Dim DownloadCanceled As Boolean = False
    Dim SelectedGameURL As DownloadableGame = Nothing
    Dim CurrentDownloadingPlatform As String = ""

    Private Sub frmDLC_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.NewNullDCBearIcon
        Me.CenterToParent()

        GetDownloadableGamesList()

        AddHandler DownloadClient.DownloadFileCompleted, AddressOf DownloadComplete
        AddHandler DownloadClient.DownloadProgressChanged, AddressOf DownloadProgress

    End Sub

    Private Sub AddDirectLink(ByVal _url As String, ByVal _name As String, ByRef _listtoadd As ListView)
        Dim _it As New ListViewItem(_name)
        _it.SubItems.Add(_url)
        _listtoadd.Items.Add(_it)
    End Sub

    Private Sub ArchiveDotOrgParse(ByVal URL As String, ListView As ListView)
        Dim StrippedURL = URL.Replace("&output=json", "")
        StrippedURL = StrippedURL.Replace("/details/", "/download/")

        Dim GamesListClient As New WebClient
        GamesListClient.Credentials = New NetworkCredential()
        GamesListClient.Headers.Add("user-agent", "MyRSSReader/1.0")

        GamesListClient.DownloadStringTaskAsync(URL)

        AddHandler GamesListClient.DownloadStringCompleted,
            Sub(ByVal sender As WebClient, e As DownloadStringCompletedEventArgs)

                If Not e.Error Is Nothing Then
                    MsgBox("Error Getting Downloadable Games List")
                    Exit Sub
                End If

                For Each _sp As String In e.Result.Split("""")
                    If _sp.Contains(".zip") Then
                        Dim _gameName = _sp.Split("/")(_sp.Split("/").Count - 1).Replace(".zip", "").Replace("_", " ").Trim
                        Dim _GameLink = StrippedURL & _sp.Replace("\/", "/").Replace("#", "%23")

                        If Not _gameName.Contains("(Windows CE)") Then
                            Dim _it As New ListViewItem(_gameName)
                            _it.SubItems.Add(_GameLink)
                            'Console.WriteLine("Added Game: " & _gameName & " URL: " & _GameLink)
                            ListView.Items.Add(_it)
                        End If

                    End If
                Next

                ListView.Sorting = SortOrder.Ascending
                ListView.Sort()
            End Sub

    End Sub

    Private Sub GetDownloadableGamesList()
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

        Try

            ArchiveDotOrgParse("https://archive.org/details/NaomiRomsReuploadByGhostware&output=json", lvGamesList_naomi)
            ArchiveDotOrgParse("https://archive.org/details/AtomiswaveReuploadByGhostware&output=json", lvGamelist_Atomiswave)
            ArchiveDotOrgParse("https://archive.org/details/DreamcastSelfBoot&output=json", lvGamesList_Dreamcast)
            'GetRomPacks()
            AddDirectLink("https://archive.org/download/neo-geo-battle-coliseum-unlocked/NeoGeo%20Battle%20Coliseum%20-%20Unlocked.zip", "Neo Geo Battle Coliseum Unlocked", lvGamelist_Atomiswave)
            AddDirectLink("https://archive.org/download/capcom-vs-snk-millenium-fight-2000-unlocked_202010/Capcom_VS_SNK_Millenium_Fight_2000_Unlocked.zip", "Capcom VS SNK Millenium Fight 2000 Unlocked", lvGamesList_naomi)
            AddDirectLink("https://archive.org/download/king-of-fighters-xi/King%20of%20Fighters%20XI.zip", "King of Fighters XI", lvGamesList_Dreamcast)
        Catch ex As Exception
            MsgBox(ex.InnerException)
        End Try


    End Sub

    'Naomi.bearpack
    'Atomiswave.bearpack
    'Dreamcast.bearpack
    '
    Private Sub GetRomPacks()
        Dim Files = Directory.GetFiles(MainformRef.NullDCPath, "*.bearpack", SearchOption.AllDirectories)

        For Each _file In Files
            ArchiveDotOrgParse(_file, lvGamesList_naomi)
        Next

    End Sub

    Private Sub btnDownload_Click(sender As Object, e As EventArgs) Handles btnDownload.Click
        If SelectedGameURL Is Nothing Then
            Exit Sub
        End If

        If DownloadClient.IsBusy Then
            DownloadCanceled = True
            DownloadClient.CancelAsync()
            Exit Sub
        End If

        DownloadingGameName = SelectedGameURL.Name
        Dim _tmp As String() = SelectedGameURL.URL.Split("/")
        DownloadingZipName = _tmp(_tmp.Count - 1).Replace(".zip", ".honey")
        CurrentDownloadingPlatform = SelectedGameURL.Platform

        Try
            DownloadClient.Credentials = New NetworkCredential()
            Select Case CurrentDownloadingPlatform
                Case "NA"
                    DownloadClient.DownloadFileTaskAsync(SelectedGameURL.URL, MainformRef.NullDCPath & "\roms\" & DownloadingZipName)

                Case "DC"
                    DownloadClient.DownloadFileTaskAsync(SelectedGameURL.URL, MainformRef.NullDCPath & "\dc\roms\" & DownloadingZipName)

            End Select

            ProgressBar1.Visible = True
            btnDownload.Text = "Downloading... " & DownloadingGameName
            DownloadCanceled = False

            Console.WriteLine("Downloading: {0}", SelectedGameURL.Name)
            ProgressBar1.Value = 0
        Catch ex As Exception
            MsgBox("Downlaod Error: " & ex.Message)
            btnDownload.Text = "Download"
            ProgressBar1.Visible = False
        End Try

    End Sub

    Private Sub DownloadProgress(ByVal sender As WebClient, e As DownloadProgressChangedEventArgs)
        'Console.WriteLine(e.BytesReceived)
        ProgressBar1.Maximum = e.TotalBytesToReceive
        ProgressBar1.Value = e.BytesReceived
        ProgressBar1.Visible = True
        Dim _NameLength = 20
        If DownloadingGameName.Length < 20 Then
            _NameLength = DownloadingGameName.Length
        End If
        btnDownload.Text = String.Format("Downloading... {0} (Click again to Cancel)" & vbNewLine & "({1}mb/{2}mb)", DownloadingGameName.Substring(0, _NameLength), Math.Round(e.BytesReceived / 1000000, 2), Math.Round(e.TotalBytesToReceive / 1000000, 2))
    End Sub

    Private Sub DownloadComplete()
        Console.WriteLine("Download Done")

        Dim zipPath = ""
        Dim RomDirectory = ""

        Select Case CurrentDownloadingPlatform
            Case "NA"
                zipPath = MainformRef.NullDCPath & "\roms\" & DownloadingZipName
                RomDirectory = MainformRef.NullDCPath & "\roms\" & DownloadingGameName
            Case "DC"
                zipPath = MainformRef.NullDCPath & "\dc\roms\" & DownloadingZipName
                RomDirectory = MainformRef.NullDCPath & "\dc\roms"
        End Select

        'btnDownload.Text = "Installing..."

        Dim InstallThread As Thread =
            New Thread(Sub()
                           If Not DownloadCanceled Then
                               Try
                                   MainformRef.Invoke(Sub() btnDownload.Text = "Installing...")

                                   If CurrentDownloadingPlatform = "NA" Then ' Naomi has to delete the whole folder if it exists
                                       If Not Directory.Exists(RomDirectory) Then
                                           Directory.CreateDirectory(RomDirectory)
                                       Else
                                           Directory.Delete(RomDirectory, True)
                                           Directory.CreateDirectory(RomDirectory)
                                       End If

                                   End If

                                   If File.Exists(zipPath) Then
                                       ZipFile.ExtractToDirectory(zipPath, RomDirectory)
                                   End If

                                   MainformRef.Invoke(Sub() MainformRef.NotificationForm.ShowMessage("Enjoy " & DownloadingGameName))
                               Catch ex As Exception
                                   MsgBox("Rom install Error: " & ex.Message)
                               End Try

                           End If

                           If File.Exists(zipPath) Then
                               File.SetAttributes(zipPath, FileAttributes.Normal)
                               File.Delete(zipPath)
                           End If

                           MainformRef.Invoke(Sub()
                                                  If Me.Visible Then
                                                      'MainformRef.GetGamesList()
                                                      ProgressBar1.Visible = False
                                                      btnDownload.Text = "Download"
                                                      DownloadCanceled = False
                                                  End If
                                              End Sub)

                       End Sub)

        InstallThread.Start()

    End Sub

    Private Sub frmDLC_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If DownloadClient.IsBusy Then
            DownloadCanceled = True
            DownloadClient.CancelAsync()
        End If
        My.Application.OpenForms(0).Activate()
    End Sub

    Private Sub lnkRoms_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lnkRoms.LinkClicked
        If tc_games.SelectedIndex = 0 Or tc_games.SelectedIndex = 1 Then
            Process.Start("https://archive.org/details/NaomiRomsReuploadByGhostware")
        Else
            Process.Start("https://archive.org/details/DreamcastSelfBoot")
        End If

    End Sub

    Private Sub btnRomsFolder_Click(sender As Object, e As EventArgs) Handles btnRomsFolder.Click
        Select Case tc_games.SelectedIndex
            Case 0
                If Not Directory.Exists(MainformRef.NullDCPath & "\roms") Then Directory.CreateDirectory(MainformRef.NullDCPath & "\roms")
                Process.Start(MainformRef.NullDCPath & "\roms")
            Case 1
                If Not Directory.Exists(MainformRef.NullDCPath & "\roms") Then Directory.CreateDirectory(MainformRef.NullDCPath & "\roms")
                Process.Start(MainformRef.NullDCPath & "\roms")
            Case 2
                If Not Directory.Exists(MainformRef.NullDCPath & "\dc\roms") Then Directory.CreateDirectory(MainformRef.NullDCPath & "\roms")
                Process.Start(MainformRef.NullDCPath & "\dc\roms")
        End Select


    End Sub

    Private Sub lvGamelist_Atomiswave_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lvGamelist_Atomiswave.SelectedIndexChanged
        If lvGamelist_Atomiswave.SelectedItems.Count = 0 Then
            SelectedGameURL = Nothing
        Else
            SelectedGameURL = New DownloadableGame(lvGamelist_Atomiswave.SelectedItems(0).SubItems(1).Text, lvGamelist_Atomiswave.SelectedItems(0).SubItems(0).Text, "NA")
        End If

    End Sub

    Private Sub lvGamesList_naomi_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lvGamesList_naomi.SelectedIndexChanged
        If lvGamesList_naomi.SelectedItems.Count = 0 Then
            SelectedGameURL = Nothing
        Else
            SelectedGameURL = New DownloadableGame(lvGamesList_naomi.SelectedItems(0).SubItems(1).Text, lvGamesList_naomi.SelectedItems(0).SubItems(0).Text, "NA")
        End If

    End Sub

    Private Sub lvGamesList_Dreamcast_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lvGamesList_Dreamcast.SelectedIndexChanged
        If lvGamesList_Dreamcast.SelectedItems.Count = 0 Then
            SelectedGameURL = Nothing
        Else
            SelectedGameURL = New DownloadableGame(lvGamesList_Dreamcast.SelectedItems(0).SubItems(1).Text, lvGamesList_Dreamcast.SelectedItems(0).SubItems(0).Text, "DC")
        End If

    End Sub

    Private Sub tc_games_SelectedIndexChanged(sender As Object, e As EventArgs) Handles tc_games.SelectedIndexChanged
        Select Case tc_games.SelectedIndex
            Case 0
                btnRomsFolder.Text = "Open Naomi/Atomiswave Roms Folder"
            Case 1
                btnRomsFolder.Text = "Open Naomi/Atomiswave Roms Folder"
            Case 2
                btnRomsFolder.Text = "Open Dreamcast Roms Folder"
        End Select
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs)
        Process.Start("https://cdromance.com/dc-iso/?sorted=downloads")
    End Sub
End Class

Class DownloadableGame

    Public URL = ""
    Public Name = ""
    Public Platform = ""

    Public Sub New(ByVal _url As String, ByVal _name As String, ByVal _platform As String)
        URL = _url
        Name = _name
        Platform = _platform

    End Sub

End Class