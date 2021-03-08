﻿Imports System.IO
Imports System.Net
Imports System.Threading
Imports PDFco.Downloader
Imports PDFco.Downloader.Contract.Events
Imports PDFco.Downloader.Download
Imports PDFco.Downloader.DownloadBuilder
Imports PDFco.Downloader.Observer
Imports PDFco.Downloader.Utils

Public Class ccDownload
    Dim finished As Boolean = False

    Public URL_String As String = ""

    Dim url As Uri
    Dim FileFormat As String = ""

    Dim requestBuilder = New SimpleWebRequestBuilder
    Dim dlChecker = New DownloadChecker

    Dim httpDlBuilder = New SimpleDownloadBuilder(requestBuilder, dlChecker)
    Dim timeForHeartbear = 3000
    Dim timeToRetry = 5000
    Dim maxRetries = 10
    Dim rdlBuilder = New ResumingDownloadBuilder(timeForHeartbear, timeToRetry, maxRetries, httpDlBuilder)
    Dim DownlaodRange = New List(Of Contract.DownloadRange)
    Dim bufferSize = 4096
    Dim numberOfParts = 1
    Dim maxRetryDownloadparts = 10

    Dim download As MultiPartDownload

    Dim fileinf As FileInfo
    Dim dlSaver As DownloadToFileSaver

    Dim ProgressMonitor As DownloadProgressMonitor = New DownloadProgressMonitor
    Dim SpeedMonitor As DownloadSpeedMonitor = New DownloadSpeedMonitor(128)

    Dim Extract = "0"

    Public Sub New(ByVal _URL As String, ByVal _filename As String, ByVal _extract As String)
        InitializeComponent()
        URL_String = _URL
        '_URL = WebUtility.UrlDecode(_URL)

        Console.WriteLine("Starting Download")
        Console.WriteLine(_URL)

        Extract = _extract

        url = New Uri(_URL)
        Console.WriteLine(url.AbsolutePath)
        FileFormat = "." & url.AbsolutePath.Split(".")(url.AbsolutePath.Split(".").Count - 1)

        download = New MultiPartDownload(url, bufferSize, numberOfParts, rdlBuilder, requestBuilder, dlChecker, DownlaodRange, maxRetryDownloadparts)
        fileinf = New FileInfo(_filename)
        dlSaver = New DownloadToFileSaver(fileinf)

        Dim _thread As New Thread(Sub() StartDownlaod())
        _thread.IsBackground = True
        _thread.Start()

        Label1.Text = "Starting..."
        Label2.Text = fileinf.Name.Replace(".honey", "")

    End Sub

    Public Sub New()
        InitializeComponent()

    End Sub

    Private Sub StartDownlaod()

        Try
            dlSaver.Attach(download)
            ProgressMonitor.Attach(download)
            SpeedMonitor.Attach(download)
            download.Start()

            AddHandler download.DownloadCancelled, AddressOf DownloadCanceled
            AddHandler download.DownloadCompleted, AddressOf DownloadCompleted
            AddHandler download.DownloadStopped, AddressOf DownloadStopped

        Catch ex As Exception
            If ex.Message.Contains("HTTP status code: 429") Then
                MsgBox("Download Error: Downloading too many files at once, refused by host." & vbNewLine & "Wait for downloads to finish, then try again.")

            Else
                MsgBox("Download Error:" & ex.Message)
            End If

            CleanUp(False)
            Me.Invoke(Sub()
                          Label1.Text = "Download Error: " & ex.Message
                      End Sub)
        End Try

        While Not finished

            Me.Invoke(Sub()
                          ProgressBar1.Value = ProgressMonitor.GetCurrentProgressPercentage(download) * 100
                          If Math.Round(ProgressMonitor.GetCurrentProgressInBytes(download) / 1000000, 2) = 0 Then
                              Label1.Text = "Preallocating..."
                          Else
                              Label1.Text = Math.Round(SpeedMonitor.GetCurrentBytesPerSecond / 1000, 2) & "kbp/s (" & Math.Round(ProgressMonitor.GetCurrentProgressInBytes(download) / 1000000, 2) & "mb/" & Math.Round(ProgressMonitor.GetTotalFilesizeInBytes(download) / 1000000, 2) & "mb)"
                          End If

                      End Sub)

            Thread.Sleep(100)

        End While

    End Sub

    Private Sub DownloadStopped(ByVal r As DownloadEventArgs)

        Console.WriteLine("Downlaod Stopped: " & url.AbsolutePath)

        CleanUp()
    End Sub

    Private Sub DownloadCompleted(ByVal r As DownloadEventArgs)
        InstallGame()

        Console.WriteLine("Downlaod Complete: " & url.AbsolutePath)

        CleanUp()
    End Sub

    Private Sub DownloadCanceled(ByVal r As DownloadEventArgs)

        Console.WriteLine("Downlaod Canceled: " & url.AbsolutePath)

        CleanUp()
    End Sub

    Private Sub InstallGame()

        Dim FolderToPlacein = fileinf.Directory.FullName & "\" & fileinf.Name.Replace(".honey", "")

        Select Case Extract
            Case "0" ' Do not Unzip

                If File.Exists(fileinf.FullName.Replace(".honey", FileFormat)) Then
                    File.Delete(fileinf.FullName.Replace(".honey", FileFormat))
                End If

                File.Copy(fileinf.FullName, fileinf.FullName.Replace(".honey", FileFormat))

            Case "1" ' Unzip

                If fileinf.Exists Then
                    ZipFile.ExtractToDirectory(fileinf.FullName, fileinf.Directory.FullName)
                End If

            Case "2" ' Unzip and place in own folder

                If Not Directory.Exists(FolderToPlacein) Then
                    Directory.CreateDirectory(FolderToPlacein)
                Else
                    Directory.Delete(FolderToPlacein, True)
                    Directory.CreateDirectory(FolderToPlacein)
                End If

                ZipFile.ExtractToDirectory(fileinf.FullName, FolderToPlacein)

            Case "3" ' Do not Unzip, but place in folder

                If Not Directory.Exists(FolderToPlacein) Then
                    Directory.CreateDirectory(FolderToPlacein)
                Else
                    Directory.Delete(FolderToPlacein, True)
                    Directory.CreateDirectory(FolderToPlacein)
                End If

                File.Copy(fileinf.FullName, FolderToPlacein & "\" & fileinf.Name.Replace(".honey", FileFormat))

        End Select

        Me.Invoke(Sub()
                      MainformRef.NotificationForm.ShowMessage("Enjoy " & fileinf.Name.Replace(".honey", ""))
                  End Sub)
    End Sub

    Private Sub CleanUp(Optional _remove As Boolean = True)
        finished = True
        download.DetachAllHandlers()
        dlSaver.DetachAll()
        ProgressMonitor.DetachAll()
        SpeedMonitor.DetachAll()
        download.Dispose()

        If fileinf.Exists Then
            While MainformRef.IsFileInUse(fileinf.FullName)
                If Not fileinf.Exists Then Exit While
                Thread.Sleep(500)
            End While
        End If

        Me.Invoke(Sub()
                      If fileinf.Exists Then fileinf.Delete()
                      If _remove Then
                          Me.Parent.Controls.Remove(Me)
                      End If
                  End Sub)

    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        If finished Then
            CleanUp()
        End If

        If Math.Round(ProgressMonitor.GetCurrentProgressInBytes(download) / 1000000, 2) > 0 Then

            btnCancel.Text = "Cleaning up"
            download.Stop()

        End If

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Process.Start(fileinf.Directory.ToString, "")

    End Sub

End Class