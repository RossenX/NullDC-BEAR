Imports System.IO
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

    Dim url As Uri

    Dim requestBuilder = New SimpleWebRequestBuilder
    Dim dlChecker = New DownloadChecker

    Dim httpDlBuilder = New SimpleDownloadBuilder(requestBuilder, dlChecker)
    Dim timeForHeartbear = 3000
    Dim timeToRetry = 5000
    Dim maxRetries = 5
    Dim rdlBuilder = New ResumingDownloadBuilder(timeForHeartbear, timeToRetry, maxRetries, httpDlBuilder)
    Dim DownlaodRange = New List(Of Contract.DownloadRange)
    Dim bufferSize = 4096
    Dim numberOfParts = 4
    Dim maxRetryDownloadparts = 2
    Dim download As MultiPartDownload

    Dim fileinf As FileInfo
    Dim dlSaver As DownloadToFileSaver

    Dim ProgressMonitor As DownloadProgressMonitor = New DownloadProgressMonitor
    Dim SpeedMonitor As DownloadSpeedMonitor = New DownloadSpeedMonitor(128)

    Dim Extract = "0"

    Public Sub New(ByVal _URL As String, ByVal _filename As String, ByVal _extract As String)
        InitializeComponent()

        Extract = _extract

        url = New Uri(WebUtility.UrlDecode(_URL))
        download = New MultiPartDownload(url, bufferSize, numberOfParts, rdlBuilder, requestBuilder, dlChecker, DownlaodRange, maxRetryDownloadparts)
        fileinf = New FileInfo(_filename)
        dlSaver = New DownloadToFileSaver(fileinf)

        Dim _thread As New Thread(Sub() StartDownlaod())
        _thread.IsBackground = True
        _thread.Start()

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

            While Not finished

                Me.Invoke(Sub()
                              ProgressBar1.Value = ProgressMonitor.GetCurrentProgressPercentage(download) * 100
                              Label1.Text = fileinf.Name.Split(".")(0) & " " & Math.Round(ProgressMonitor.GetCurrentProgressInBytes(download) / 1000000, 2) & "mb/" & Math.Round(ProgressMonitor.GetTotalFilesizeInBytes(download) / 1000000, 2) & "mb"
                          End Sub)

                Thread.Sleep(1000)

            End While

        Catch ex As Exception
            MsgBox(ex.Message)

        End Try

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

        Select Case Extract
            Case "0" ' Do not Unzip
                If File.Exists(fileinf.FullName.Replace(".honey", ".zip")) Then
                    File.Delete(fileinf.FullName.Replace(".honey", ".zip"))
                End If

                File.Copy(fileinf.FullName, fileinf.FullName.Replace(".honey", ".zip"))

            Case "1" ' Unzip
                If fileinf.Exists Then
                    ZipFile.ExtractToDirectory(fileinf.FullName, fileinf.Directory.FullName)
                End If

            Case "2" ' Unzip and place in own folder
                If Not Directory.Exists(fileinf.Directory.FullName & "\" & fileinf.Name) Then
                    Directory.CreateDirectory(fileinf.Directory.FullName & "\" & fileinf.Name.Replace(".honey", ""))
                Else
                    Directory.Delete(fileinf.Directory.FullName & "\" & fileinf.Name, True)
                    Directory.CreateDirectory(fileinf.Directory.FullName & "\" & fileinf.Name.Replace(".honey", ""))
                End If

                ZipFile.ExtractToDirectory(fileinf.FullName, fileinf.Directory.FullName & "\" & fileinf.Name.Replace(".honey", ""))
            Case "3" ' Do not Unzip, but place in folder
                If Not Directory.Exists(fileinf.Directory.FullName & "\" & fileinf.Name.Replace(".honey", "")) Then
                    Directory.CreateDirectory(fileinf.Directory.FullName & "\" & fileinf.Name.Replace(".honey", ""))
                Else
                    Directory.Delete(fileinf.Directory.FullName & "\" & fileinf.Name.Replace(".honey", ""), True)
                    Directory.CreateDirectory(fileinf.Directory.FullName & "\" & fileinf.Name.Replace(".honey", ""))
                End If

                File.Copy(fileinf.FullName, fileinf.Directory.FullName & "\" & fileinf.Name.Replace(".honey", ""))
        End Select

    End Sub

    Private Sub CleanUp()
        download.DetachAllHandlers()
        Me.Invoke(Sub()
                      File.Delete(fileinf.FullName)
                      Me.Parent.Controls.Remove(Me)
                  End Sub)
        finished = True

    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        download.Stop()

    End Sub

End Class
