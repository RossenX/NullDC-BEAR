Imports System.IO
Imports System.Net

Public Class frmUpdate

    Dim GifLoop As New Timer
    Dim GifEnd As New Timer
    Dim DownloadClient As New WebClient

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        MyBase.Icon = My.Resources.NewNullDCBearIcon
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

        Dim remoteUri As String = "https://github.com//RossenX/NullDC-BEAR/releases/latest/download/NullDC.BEAR.exe"

        GifLoop.Interval = 1300
        AddHandler GifLoop.Tick, AddressOf _GifLoop
        GifLoop.Start()

        AddHandler DownloadClient.DownloadFileCompleted, AddressOf DownloadComplete
        AddHandler DownloadClient.DownloadProgressChanged, AddressOf DownloadProgress

        Try
            DownloadClient.Credentials = New NetworkCredential()
            DownloadClient.DownloadFileTaskAsync(remoteUri, My.Computer.FileSystem.SpecialDirectories.Temp & "\NullDC BEAR.exe")
        Catch ex As Exception
            MsgBox(ex.Message)
            End
        End Try
    End Sub

    Private Sub DownloadProgress(ByVal sender As WebClient, e As DownloadProgressChangedEventArgs)
        ProgressBar1.Maximum = e.TotalBytesToReceive
        ProgressBar1.Value = e.BytesReceived

    End Sub

    Private Sub DownloadComplete()
        LoadingGif.Image = My.Resources.ken_terry03
        GifEnd.Interval = 2400
        AddHandler GifEnd.Tick, AddressOf _GifEnd
        GifEnd.Start()
        If File.Exists(Application.StartupPath & "\NullDC.BEAR.exe") Then File.Delete(Application.StartupPath & "\NullDC.BEAR.exe")
        File.Copy(My.Computer.FileSystem.SpecialDirectories.Temp & "\NullDC BEAR.exe", Application.StartupPath & "\NullDC BEAR.exe", True)
    End Sub

    Private Sub _GifLoop()
        GifLoop.Stop()
        LoadingGif.Image = My.Resources.ken_terry02

    End Sub

    Private Sub _GifEnd()
        GifEnd.Stop()
        LoadingGif.Image = My.Resources.ken_terry04
        Process.Start(Application.StartupPath & "\NullDC BEAR.exe")
        End

    End Sub

End Class