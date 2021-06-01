Imports System.IO
Imports System.Net
Imports System.Threading
Imports Downloader

Public Class ccDownload
    Dim finished As Boolean = False
    Dim canceled As Boolean = False
    Dim Started As Boolean = False

    Public URL_String As String = ""

    Dim FileFormat As String = ""

    Dim fileinf As FileInfo

    Dim Extract = "0"

    Dim DownloadConfigs As DownloadConfiguration
    Dim DownloadServ As DownloadService

    Public Sub New(ByVal _URL As String, ByVal _filename As String, ByVal _extract As String)
        InitializeComponent()
        URL_String = _URL
        fileinf = New FileInfo(_filename)
        Extract = _extract
        Dim url = New Uri(_URL)
        FileFormat = "." & url.AbsolutePath.Split(".")(url.AbsolutePath.Split(".").Count - 1)

    End Sub

    Public Sub New()
        InitializeComponent()

    End Sub

    Public Async Sub Init()
        Console.WriteLine("Starting Download")
        Console.WriteLine(URL_String)
        Label1.Text = "Starting..."
        Label2.Text = fileinf.Name.Replace(".honey", "")

        Try
            Await Task.Run(Sub() StartNewDownload())

        Catch ex As Exception
            MsgBox(ex.Message & " 2")

        End Try


    End Sub

    Private Async Sub StartNewDownload()

        DownloadConfigs = New DownloadConfiguration
        DownloadConfigs.BufferBlockSize = 10240
        DownloadConfigs.ChunkCount = 8
        DownloadConfigs.MaximumBytesPerSecond = 1024 * 1024
        DownloadConfigs.MaxTryAgainOnFailover = 100
        DownloadConfigs.OnTheFlyDownload = False
        DownloadConfigs.ParallelDownload = True
        DownloadConfigs.TempDirectory = My.Computer.FileSystem.SpecialDirectories.Temp
        DownloadConfigs.Timeout = 1000

        DownloadConfigs.RequestConfiguration.Accept = "*\*"
        DownloadConfigs.RequestConfiguration.AutomaticDecompression = DecompressionMethods.None
        DownloadConfigs.RequestConfiguration.CookieContainer = New CookieContainer()
        DownloadConfigs.RequestConfiguration.Headers = New WebHeaderCollection()
        DownloadConfigs.RequestConfiguration.KeepAlive = False
        DownloadConfigs.RequestConfiguration.ProtocolVersion = HttpVersion.Version11
        DownloadConfigs.RequestConfiguration.UseDefaultCredentials = True
        DownloadConfigs.RequestConfiguration.UserAgent = "BEAR"

        DownloadServ = New DownloadService(DownloadConfigs)

        AddHandler DownloadServ.DownloadFileCompleted, Sub(ByVal sender As DownloadService, ByVal a As ComponentModel.AsyncCompletedEventArgs)

                                                           ' File was Downlaoded so we're all good
                                                           If File.Exists(fileinf.FullName) Then
                                                               InstallGame()
                                                               CleanUp()

                                                               Me.Invoke(Sub()
                                                                             Label1.Text = "Finished"
                                                                             btnCancel.Text = "Ok"
                                                                         End Sub)

                                                           End If

                                                           If DownloadServ.IsCancelled Then
                                                               Me.Invoke(Sub()
                                                                             Label1.Text = "Canceled"
                                                                             btnCancel.Text = "Ok"
                                                                         End Sub)

                                                           End If

                                                       End Sub

        AddHandler DownloadServ.DownloadProgressChanged, Sub(ByVal sender As DownloadService, ByVal a As Downloader.DownloadProgressChangedEventArgs)
                                                             Me.Invoke(Sub()
                                                                           ProgressBar1.Value = a.ProgressPercentage
                                                                           Label1.Text = "(" & Math.Floor(a.ReceivedBytesSize * 0.000001) & "mb\" & Math.Floor(a.TotalBytesToReceive * 0.000001) & "mb) " & Math.Floor(a.AverageBytesPerSecondSpeed * 0.001) & "/kbps"
                                                                       End Sub)

                                                             If canceled Then
                                                                 Me.Invoke(Sub() Label1.Text = "Canceling...")
                                                             End If

                                                             If a.AverageBytesPerSecondSpeed = 0 Then
                                                                 Me.Invoke(Sub() Label1.Text = "Canceled")
                                                             End If

                                                         End Sub

        Try

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
            Dim req As HttpWebRequest = DirectCast(HttpWebRequest.Create(URL_String), HttpWebRequest)
            Dim response As HttpWebResponse
            response = req.GetResponse
            URL_String = response.ResponseUri.AbsoluteUri

            If Not canceled Then
                Started = True
                Await DownloadServ.DownloadFileTaskAsync(URL_String, fileinf.FullName)
            End If

        Catch ex As Exception
            Me.Invoke(Sub()
                          Label1.Text = "Error: " & ex.Message
                          btnCancel.Text = "Ok"
                      End Sub)
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

    Private Sub CleanUp()
        finished = True

        If fileinf.Exists Then
            While MainformRef.IsFileInUse(fileinf.FullName)
                If Not fileinf.Exists Then Exit While
                Thread.Sleep(500)
            End While
        End If

        Me.Invoke(Sub()
                      If fileinf.Exists Then
                          fileinf.Delete()
                          Label1.Text = "Complete"
                          btnCancel.Text = "Ok"
                      End If
                  End Sub)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Process.Start(fileinf.Directory.ToString, "")
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        If btnCancel.Text = "Ok" Then
            Me.Invoke(Sub()
                          If Me.Parent.Controls.Count = 1 Then
                              frmDownloading.Hide()
                          End If

                          Me.Parent.Controls.Remove(Me)

                      End Sub)
        Else
            canceled = True
            DownloadServ.CancelAsync()
            If Started Then
                Label1.Text = "Canceling..."
            Else
                Label1.Text = "Canceled"
                btnCancel.Text = "Ok"
            End If


        End If
    End Sub

End Class
