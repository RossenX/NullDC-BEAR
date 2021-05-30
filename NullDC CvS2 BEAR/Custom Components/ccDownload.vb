Imports System.IO
Imports System.Net
Imports System.Threading
Imports Downloader

Public Class ccDownload
    Dim finished As Boolean = False

    Public URL_String As String = ""

    Dim FileFormat As String = ""

    Dim fileinf As FileInfo

    Dim Extract = "0"

    Dim d1 As DownloadConfiguration
    Dim d2 As DownloadService

    Public Sub New(ByVal _URL As String, ByVal _filename As String, ByVal _extract As String)
        InitializeComponent()
        URL_String = _URL
        fileinf = New FileInfo(_filename)
        Extract = _extract

    End Sub

    Public Sub New()
        InitializeComponent()

    End Sub

    Public Sub Init()
        Console.WriteLine("Starting Download")
        Console.WriteLine(URL_String)
        Label1.Text = "Starting..."
        Label2.Text = fileinf.Name.Replace(".honey", "")

        StartNewDownload()

    End Sub

    Private Async Sub StartNewDownload()

        'ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
        'Dim req As HttpWebRequest = DirectCast(HttpWebRequest.Create(URL_String), HttpWebRequest)
        'Dim response As HttpWebResponse
        'response = req.GetResponse
        'URL_String = response.ResponseUri.AbsoluteUri

        d1 = New DownloadConfiguration
        d1.BufferBlockSize = 10240
        d1.ChunkCount = 8
        d1.MaximumBytesPerSecond = 1024 * 1024
        d1.MaxTryAgainOnFailover = 100
        d1.OnTheFlyDownload = False
        d1.ParallelDownload = True
        d1.TempDirectory = My.Computer.FileSystem.SpecialDirectories.Temp
        d1.Timeout = 1000

        d1.RequestConfiguration.Accept = "*\*"
        d1.RequestConfiguration.AutomaticDecompression = DecompressionMethods.None
        d1.RequestConfiguration.CookieContainer = New CookieContainer()
        d1.RequestConfiguration.Headers = New WebHeaderCollection()
        d1.RequestConfiguration.KeepAlive = False
        d1.RequestConfiguration.ProtocolVersion = HttpVersion.Version11
        d1.RequestConfiguration.UseDefaultCredentials = False
        d1.RequestConfiguration.UserAgent = "BEAR"

        d2 = New DownloadService(d1)

        AddHandler d2.DownloadStarted, Sub(ByVal sender As DownloadService, ByVal a As Downloader.DownloadStartedEventArgs)
                                           Me.Invoke(Sub() Label1.Text = "Preallocating...")

                                       End Sub

        AddHandler d2.DownloadFileCompleted, Sub(ByVal sender As DownloadService, ByVal a As ComponentModel.AsyncCompletedEventArgs)

                                                 ' File was Downlaoded so we're all good
                                                 If File.Exists(fileinf.FullName) Then
                                                     InstallGame()
                                                     CleanUp()

                                                 End If

                                             End Sub

        AddHandler d2.DownloadProgressChanged, Sub(ByVal sender As DownloadService, ByVal a As Downloader.DownloadProgressChangedEventArgs)
                                                   Me.Invoke(Sub() Label2.Text = a.ProgressPercentage)

                                               End Sub

        Try
            Await d2.DownloadFileTaskAsync(URL_String, fileinf.FullName)
        Catch ex As Exception
            MsgBox("Error:" & ex.Message)

        End Try

    End Sub

    Private Sub UnZipFile(ByVal FileName As String, ByVal ExtractTo As String)

        Using ext As New SevenZip.SevenZipExtractor(fileinf.FullName)
            ext.ExtractArchive(ExtractTo)
            ext.Dispose()
        End Using

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
                    UnZipFile(fileinf.FullName, fileinf.Directory.FullName)
                End If

            Case "2" ' Unzip and place in own folder

                If Not Directory.Exists(FolderToPlacein) Then
                    Directory.CreateDirectory(FolderToPlacein)
                Else
                    Directory.Delete(FolderToPlacein, True)
                    Directory.CreateDirectory(FolderToPlacein)
                End If

                UnZipFile(fileinf.FullName, FolderToPlacein)

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

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Process.Start(fileinf.Directory.ToString, "")

    End Sub

End Class
