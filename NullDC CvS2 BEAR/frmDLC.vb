Imports System.IO
Imports System.Net

Public Class frmDLC

    Dim GamesListClient As New WebClient
    Dim DownloadClient As New WebClient
    Dim DownloadingGameName = ""
    Dim DownloadingZipName = ""
    Dim DownloadCanceled As Boolean = False

    Private Sub frmDLC_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.NewNullDCBearIcon
        Me.CenterToParent()

        GetDownloadableGamesList()

        AddHandler DownloadClient.DownloadFileCompleted, AddressOf DownloadComplete
        AddHandler DownloadClient.DownloadProgressChanged, AddressOf DownloadProgress
        AddHandler GamesListClient.DownloadStringCompleted, AddressOf GotDownloadableGamesList
    End Sub

    Private Sub GotDownloadableGamesList(ByVal sender As WebClient, e As DownloadStringCompletedEventArgs)
        If Not e.Error Is Nothing Then
            MsgBox("Error Getting Downloadable Games List")
            Exit Sub
        End If

        lvGamesList.Items.Clear()

        For Each _sp As String In e.Result.Split("""")
            If _sp.Contains(".zip") Then
                Dim _tmp = _sp.Replace("\/", "").Replace(".zip", "").Replace("_", " ").Trim
                Dim _it As New ListViewItem(_tmp)
                _it.SubItems.Add("https://archive.org/download/NaomiRomsReuploadByGhostware/" & _sp.Replace("\/", ""))
                lvGamesList.Items.Add(_it)

            End If
        Next

    End Sub

    Private Sub GetDownloadableGamesList()
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

        Try
            GamesListClient.Credentials = New NetworkCredential()
            GamesListClient.Headers.Add("user-agent", "MyRSSReader/1.0")
            GamesListClient.DownloadStringTaskAsync("https://archive.org/details/NaomiRomsReuploadByGhostware&output=json")
        Catch ex As Exception

        End Try

    End Sub

    Private Sub btnDownload_Click(sender As Object, e As EventArgs) Handles btnDownload.Click
        If Not lvGamesList.SelectedItems.Count > 0 Then Exit Sub

        If DownloadClient.IsBusy Then
            DownloadCanceled = True
            DownloadClient.CancelAsync()
            Exit Sub
        End If

        DownloadingGameName = lvGamesList.SelectedItems(0).SubItems(0).Text
        Dim _tmp = lvGamesList.SelectedItems(0).SubItems(1).Text.Split("/")
        DownloadingZipName = _tmp(_tmp.Count - 1).Replace(".zip", ".honey")

        Try
            DownloadClient.Credentials = New NetworkCredential()
            DownloadClient.DownloadFileTaskAsync(lvGamesList.SelectedItems(0).SubItems(1).Text, MainformRef.NullDCPath & "\roms\" & DownloadingZipName)

            ProgressBar1.Visible = True
            btnDownload.Text = "Downloading... " & DownloadingGameName
            DownloadCanceled = False

            Console.WriteLine("Downloading: {0}", lvGamesList.SelectedItems(0).SubItems(1).Text)
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
        Dim zipPath = MainformRef.NullDCPath & "\roms\" & DownloadingZipName
        Dim RomDirectory = MainformRef.NullDCPath & "\roms\" & DownloadingGameName
        btnDownload.Invoke(Sub() btnDownload.Text = "Installing...")

        If Not DownloadCanceled Then
            Try
                If Not Directory.Exists(RomDirectory) Then Directory.CreateDirectory(RomDirectory)
                Using archive As ZipArchive = ZipFile.OpenRead(zipPath)
                    For Each entry As ZipArchiveEntry In archive.Entries
                        If entry.FullName.Split("\").Length > 1 Then
                            Directory.CreateDirectory(RomDirectory & "\" & entry.FullName.Split("\")(0))
                        End If
                        entry.ExtractToFile(RomDirectory & "\" & entry.FullName, True)
                    Next
                End Using
                MainformRef.NotificationForm.ShowMessage("Enjoy " & DownloadingGameName)

            Catch ex As Exception
                MsgBox("Rom install Error: " & ex.Message)
            End Try

        End If


        If File.Exists(zipPath) Then
            File.SetAttributes(zipPath, FileAttributes.Normal)
            File.Delete(zipPath)
        End If

        MainformRef.GetGamesList()
        ProgressBar1.Visible = False
        btnDownload.Text = "Download"
        DownloadCanceled = False

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
End Class