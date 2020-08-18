Imports System.IO
Imports System.Net
Imports System.Threading

Public Class frmDLC

    Dim DownloadClient As New WebClient
    Dim DownloadingGameName = ""
    Dim DownloadingZipName = ""
    Dim DownloadCanceled As Boolean = False
    Dim SelectedGameURL As DownloadableGame = Nothing

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
        Dim StrippedURL = URL.Replace("&output=json", "/")
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
                        Dim _tmp = _sp.Replace("\/", "").Replace(".zip", "").Replace("_", " ").Trim
                        Dim _it As New ListViewItem(_tmp)
                        _it.SubItems.Add(StrippedURL & _sp.Replace("\/", ""))
                        ListView.Items.Add(_it)
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
            AddDirectLink("https://archive.org/download/neo-geo-battle-coliseum-unlocked/NeoGeo%20Battle%20Coliseum%20-%20Unlocked.zip", "Neo Geo Battle Coliseum Unlocked", lvGamelist_Atomiswave)
        Catch ex As Exception
            MsgBox(ex.InnerException)
        End Try


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

        Try
            DownloadClient.Credentials = New NetworkCredential()
            DownloadClient.DownloadFileTaskAsync(SelectedGameURL.URL, MainformRef.NullDCPath & "\roms\" & DownloadingZipName)

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
        btnDownload.Text = String.Format("Downloading... {0} (Click again to Cancel)" & vbNewLine & "({1}mb/{2}mb)", DownloadingGameName, Math.Round(e.BytesReceived / 1000000, 2), Math.Round(e.TotalBytesToReceive / 1000000, 2))
    End Sub

    Private Sub DownloadComplete()
        Console.WriteLine("Download Done")
        Dim zipPath = MainformRef.NullDCPath & "\roms\" & DownloadingZipName
        Dim RomDirectory = MainformRef.NullDCPath & "\roms\" & DownloadingGameName
        'btnDownload.Text = "Installing..."

        Dim InstallThread As Thread =
            New Thread(Sub()
                           If Not DownloadCanceled Then
                               Try
                                   MainformRef.Invoke(Sub() btnDownload.Text = "Installing...")
                                   If Not Directory.Exists(RomDirectory) Then Directory.CreateDirectory(RomDirectory)
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

                           MainformRef.Invoke(
                           Sub()
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
        Process.Start("https://archive.org/details/NaomiRomsReuploadByGhostware")
    End Sub

    Private Sub btnRomsFolder_Click(sender As Object, e As EventArgs) Handles btnRomsFolder.Click
        If Not Directory.Exists(MainformRef.NullDCPath & "\roms") Then Directory.CreateDirectory(MainformRef.NullDCPath & "\roms")
        Process.Start(MainformRef.NullDCPath & "\roms")
    End Sub

    Private Sub lvGamelist_Atomiswave_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lvGamelist_Atomiswave.SelectedIndexChanged
        If lvGamelist_Atomiswave.SelectedItems.Count = 0 Then
            SelectedGameURL = Nothing
        Else
            SelectedGameURL = New DownloadableGame(lvGamelist_Atomiswave.SelectedItems(0).SubItems(1).Text, lvGamelist_Atomiswave.SelectedItems(0).SubItems(0).Text)
        End If

    End Sub

    Private Sub lvGamesList_naomi_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lvGamesList_naomi.SelectedIndexChanged
        If lvGamesList_naomi.SelectedItems.Count = 0 Then
            SelectedGameURL = Nothing
        Else
            SelectedGameURL = New DownloadableGame(lvGamesList_naomi.SelectedItems(0).SubItems(1).Text, lvGamesList_naomi.SelectedItems(0).SubItems(0).Text)
        End If

    End Sub

End Class

Class DownloadableGame

    Public URL = ""
    Public Name = ""

    Public Sub New(ByVal _url As String, ByVal _name As String)
        URL = _url
        Name = _name
    End Sub

End Class