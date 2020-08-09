Imports System.IO
Imports System.Net
Imports System.Net.NetworkInformation
Imports System.Threading
Imports Open.Nat
Imports OpenTK

Public Class frmMain
    Dim IsBeta As Boolean = True

    ' Update Stuff
    Dim UpdateCheckClient As New WebClient

    Public Ver As String = "1.0h"
    Public InputHandler As InputHandling
    Public NetworkHandler As NetworkHandling
    Public NullDCLauncher As NaomiLauncher
    Public NullDCPath As String = Application.StartupPath
    Public GamesList As New Dictionary(Of String, Array)
    Public ConfigFile As Configs
    Public FirstRun As Boolean = True

    Public ChallengeForm As frmChallenge
    Public ChallengeSentForm As frmChallengeSent = New frmChallengeSent(Me)
    Public HostingForm As frmHostPanel = New frmHostPanel(Me)
    Public GameSelectForm As frmChallengeGameSelect = New frmChallengeGameSelect(Me)
    Public WaitingForm As frmWaitingForHost = New frmWaitingForHost
    Public NotificationForm As frmNotification = New frmNotification(Me)
    Public KeyMappingForm As frmKeyMapping

    Public Challenger As NullDCPlayer
    Private RefreshTimer As System.Windows.Forms.Timer = New System.Windows.Forms.Timer

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'ZipFile.CreateFromDirectory("D:\VS_Projects\NullDC-BEAR\NullDC CvS2 BEAR\bin\x86\Debug\nulldcClean", "NullNaomiClean.zip")
        'If Debugger.IsAttached Then NullDCPath = "D:\Games\Emulators\NullDC\nulldc-1-0-4-en-win"

        Me.Icon = My.Resources.NewNullDCBearIcon
        niBEAR.Icon = My.Resources.NewNullDCBearIcon
        Me.CenterToScreen()

        Rx.MainformRef = Me
        lbVer.Text = Ver

        If Not File.Exists(NullDCPath & "\nullDC_Win32_Release-NoTrace.exe") Then
            Dim result As DialogResult = MessageBox.Show("NullDC was not found in this folder, INSTALL NULLDC INTO THIS FOLDER?", "NullDC Install", MessageBoxButtons.YesNo)
            If result = DialogResult.Yes Then
                Dim result2 As DialogResult = MessageBox.Show("This will create a bunch of files in the same folder as NullDC BEAR.exe, OK?", "NullDC Extraction", MessageBoxButtons.YesNo)
                If result2 = DialogResult.Yes Then
                    Try
                        UnzipResToDir(My.Resources.NullNaomiClean, "bear_tmp_nulldc.zip", NullDCPath)
                    Catch ex As Exception
                        MsgBox(ex.StackTrace)
                    End Try

                End If
            Else
                MsgBox("I need to be in the NullDC folder where nullDC_Win32_Release-NoTrace.exe")
                End
            End If
        End If

        CheckFilesAndShit()
        ConfigFile = New Configs(NullDCPath)
        cbStatus.Text = ConfigFile.Status

        ' Update Stuff
        AddHandler UpdateCheckClient.DownloadStringCompleted, AddressOf UpdateCheckResult
        CheckForUpdate()

        ' Create all the usual shit
        ChallengeForm = New frmChallenge(Me)
        InputHandler = New InputHandling(Me)
        NetworkHandler = New NetworkHandling(Me)
        KeyMappingForm = New frmKeyMapping(Me)
        NullDCLauncher = New NaomiLauncher(Me)

        If ConfigFile.FirstRun Then frmSetup.ShowDialog(Me)
        AddHandler RefreshTimer.Tick, AddressOf RefreshTimer_tick

        RefreshPlayerList(False)

        If GamesList.Count = 0 Then
            NotificationForm.ShowMessage("You don't seem to have any games, click the Free DLC button to get some.")
        End If

        CreateCFGWatcher()
        CreateRomFolderWatcher()
    End Sub

    Dim RomFolderWatcher As FileSystemWatcher
    Private Sub CreateRomFolderWatcher()
        RomFolderWatcher = New FileSystemWatcher()
        RomFolderWatcher.IncludeSubdirectories = True
        RomFolderWatcher.Path = NullDCPath & "\roms"

        RomFolderWatcher.NotifyFilter = NotifyFilters.Attributes Or
            NotifyFilters.CreationTime Or
            NotifyFilters.DirectoryName Or
            NotifyFilters.FileName Or
            NotifyFilters.LastAccess Or
            NotifyFilters.LastWrite Or
            NotifyFilters.Security Or
            NotifyFilters.Size

        AddHandler RomFolderWatcher.Changed, AddressOf RomFolderChange
        AddHandler RomFolderWatcher.Created, AddressOf RomFolderChange
        AddHandler RomFolderWatcher.Renamed, AddressOf RomFolderChange
        AddHandler RomFolderWatcher.Deleted, AddressOf RomFolderChange

        RomFolderWatcher.EnableRaisingEvents = True
    End Sub

    Private Sub RomFolderChange(ByVal source As Object, ByVal e As FileSystemEventArgs)
        If Not e.Name.Contains("eeprom") Then ' As long as it has nothing to do with eeproms, then reload the games.
            Console.WriteLine("Roms folder changed, check if we have new games")
            RomFolderWatcher.EnableRaisingEvents = False
            Thread.Sleep(500)
            Me.Invoke(Sub() GetGamesList())
            RomFolderWatcher.EnableRaisingEvents = True
        End If
    End Sub

    Dim CFGWatcher As FileSystemWatcher
    Private Sub CreateCFGWatcher()
        CFGWatcher = New FileSystemWatcher()
        CFGWatcher.IncludeSubdirectories = False
        CFGWatcher.Path = NullDCPath
        CFGWatcher.NotifyFilter = NotifyFilters.LastWrite
        AddHandler CFGWatcher.Changed, AddressOf CFGChanged
        CFGWatcher.EnableRaisingEvents = True
    End Sub

    Private Sub CFGChanged(ByVal source As Object, ByVal e As FileSystemEventArgs)
        If e.Name = "nullDC.cfg" Then
            CFGWatcher.EnableRaisingEvents = False

            Thread.Sleep(1000)
            While IsFileInUse(NullDCPath & "/nulldc.cfg")
                Thread.Sleep(50)
                Continue While
            End While

            ' Check if something overrode the BEAR configs
            Dim ConfigLines() As String = File.ReadAllLines(MainformRef.NullDCPath & "\nullDC.cfg")
            Dim BEARJAMMAConfigsFound = False
            Dim NaomiConfigsFound = False

            For Each line In ConfigLines
                If line.StartsWith("[BEARJamma]") Then
                    BEARJAMMAConfigsFound = True
                End If
                If line.StartsWith("[Naomi]") Then
                    NaomiConfigsFound = True
                End If
            Next

            If Not BEARJAMMAConfigsFound Then ' no BEARPLAY configs so lets add them to the end and keep all the other settings
                Dim BearPlayLines = My.Resources.BEARPLAYlines
                File.AppendAllText(MainformRef.NullDCPath & "\nullDC.cfg", BearPlayLines)
            End If

            If Not NaomiConfigsFound Then
                File.AppendAllText(MainformRef.NullDCPath & "\nullDC.cfg", "[Naomi]" & vbNewLine & "LoadDefaultRom=1" & vbNewLine & "DefaultRom=0")
            End If



            InputHandler.GetKeyboardConfigs()
            InputHandler.NeedConfigReload = True
            CFGWatcher.EnableRaisingEvents = True
        End If
    End Sub

    Public Function IsFileInUse(sFile As String) As Boolean
        Try
            Using f As New IO.FileStream(sFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None)
            End Using
        Catch Ex As Exception
            Return True
        End Try
        Return False
    End Function

    Private Sub CheckForUpdate()
        imgBeta.Visible = IsBeta
        If IsBeta Then Exit Sub

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
        Try
            UpdateCheckClient.Credentials = New NetworkCredential()
            UpdateCheckClient.Headers.Add("user-agent", "MyRSSReader/1.0")
            UpdateCheckClient.DownloadStringTaskAsync("https://api.github.com/repos/RossenX/NullDC-BEAR/releases/latest")
        Catch ex As Exception
            ' some error when trying to update
        End Try
    End Sub

    Private Sub UpdateCheckResult(ByVal sender As WebClient, e As DownloadStringCompletedEventArgs)
        If Not e.Error Is Nothing Then Exit Sub ' Couldn't reach github for some reason so don't update

        Dim LatestVersion As String = ""

        For Each Line As String In e.Result.Split(",")
            If Line.StartsWith("""tag_name""") Then
                LatestVersion = Line.Split(":")(1).Replace("""", "")
                Exit For
            End If
        Next

        If LatestVersion = Ver Then
            ' Is up to date
            Console.WriteLine("Up To Date, Delete updater if it exists")
            If File.Exists(Application.StartupPath & "\NullDC-BEAR-UPDATER.exe") Then
                Dim UpdaterDeleted = False
                Dim TryingToDeleteFor = 0
                While Not UpdaterDeleted
                    ' Try for 2 seconds to delete the updater, if u can't then just leave it
                    If TryingToDeleteFor > 2000 Then Exit While

                    Try
                        File.SetAttributes(Application.StartupPath & "\NullDC-BEAR-UPDATER.exe", FileAttributes.Normal)
                        File.Delete(Application.StartupPath & "\NullDC-BEAR-UPDATER.exe")
                        UpdaterDeleted = True
                    Catch ex As Exception
                        TryingToDeleteFor += 50
                        Thread.Sleep(50)
                    End Try
                End While
            End If
        Else
            ' Is not up to date
            If Not File.Exists(NullDCPath & "\NullDC-BEAR-UPDATER.exe") Then File.WriteAllBytes(NullDCPath & "\NullDC-BEAR-UPDATER.exe", My.Resources.NullDC_BEAR_UPDATER)
            Process.Start(NullDCPath & "\NullDC-BEAR-UPDATER.exe")
            End
        End If

    End Sub

    Public Function GetGameInfoFromGameName(ByVal _gamename) As Array

        For Each key In GamesList.Keys
            If GamesList(key)(0) = _gamename Then
                Return {key, GamesList(key)(0), GamesList(key)(1)}
            End If
        Next

        Return Nothing
    End Function

    Public Sub CopyResourceToDirectoryThread(ByVal _arg As Array)
        File.WriteAllBytes(My.Computer.FileSystem.SpecialDirectories.Temp & "\" & _arg(1), _arg(0))
    End Sub

    Public Sub UnzipResToDir(ByVal _res As Byte(), ByVal _name As String, ByVal _dir As String)
        Dim FileWriteThread As Thread
        FileWriteThread = New Thread(AddressOf CopyResourceToDirectoryThread)
        FileWriteThread.IsBackground = True
        FileWriteThread.Start({_res, _name})

        While FileWriteThread.IsAlive
            Thread.Sleep(100)
        End While

        Using archive As ZipArchive = ZipFile.OpenRead(My.Computer.FileSystem.SpecialDirectories.Temp & "\" & _name)
            For Each entry As ZipArchiveEntry In archive.Entries
                Console.WriteLine("Extracting: " & entry.FullName)
                If entry.FullName.Split("\").Length > 1 Then
                    Directory.CreateDirectory(_dir & "\" & entry.FullName.Split("\")(0))
                End If
                If Not entry.FullName.EndsWith("\") Then
                    entry.ExtractToFile(_dir & "\" & entry.FullName, True)
                End If
            Next
        End Using

    End Sub

    Private Sub CheckFilesAndShit()

        ' Check BEARJamma Configs in FIle, WE NEED HIS FOR THE KEYBINDS TO WORK EVEN IF NULLDC IS NOT ON
        Dim ConfigLines() As String = File.ReadAllLines(MainformRef.NullDCPath & "\nullDC.cfg")
        Dim BEARJAMMAConfigsFound = False
        Dim NaomiConfigsFound = False

        For Each line In ConfigLines
            If line.StartsWith("[BEARJamma]") Then
                BEARJAMMAConfigsFound = True
            End If
            If line.StartsWith("[Naomi]") Then
                NaomiConfigsFound = True
            End If
        Next

        If Not BEARJAMMAConfigsFound Then ' no BEARPLAY configs so lets add them to the end and keep all the other settings
            Dim BearPlayLines = My.Resources.BEARPLAYlines
            File.AppendAllText(MainformRef.NullDCPath & "\nullDC.cfg", BearPlayLines)
        End If

        If Not NaomiConfigsFound Then
            File.AppendAllText(MainformRef.NullDCPath & "\nullDC.cfg", "[Naomi]" & vbNewLine & "LoadDefaultRom=1" & vbNewLine & "DefaultRom=0")
        End If

        ' Check Configs if there's BEARPlay entry, if not then add them just to make sure old configs work fine with BEARPLAY out of the box
        If Not File.Exists(NullDCPath & "\nullDC.cfg") Then My.Computer.FileSystem.WriteAllBytes(NullDCPath & "\nullDC.cfg", My.Resources.nullDC, False)

        ' Check the EXE name and all that shit from now on use the NullDC.BEAR.exe format since that's what github saves it as, since it hates spaces apperanly
        ' Why do this you may ask? Well mostly so people who downloaded it from github have the same exe name after they update, for firewall reasons
        Try
            If File.Exists(NullDCPath & "\NullDC BEAR.exe") Then
                ' NullDC BEAR.exe exist Copy it, start it up close this one
                If Application.ExecutablePath.Contains("NullDC BEAR.exe") Then
                    ' I am the exe with a space in it
                    File.Copy(NullDCPath & "\NullDC BEAR.exe", NullDCPath & "\NullDC.BEAR.exe")
                    While Not File.Exists(NullDCPath & "\NullDC.BEAR.exe")
                        Thread.Sleep(10)
                    End While
                    ' Start the correct exe name
                    Process.Start(NullDCPath & "\NullDC.BEAR.exe")
                    ' Close this BEAR, next start it should be deleted by the NullDC.BEAR.exe
                    End
                Else
                    ' I am not the exe with a space in it Delete the one with a space in it
                    Console.WriteLine("Deleting old NullDC Bear.exe")
                    File.Delete(NullDCPath & "\NullDC BEAR.exe")
                End If
            End If

        Catch ex As Exception
            MsgBox("Couldn't delete old NullDC BEAR.exe")
        End Try

        If Not My.Computer.FileSystem.DirectoryExists(NullDCPath & "\replays") Then
            My.Computer.FileSystem.CreateDirectory(NullDCPath & "\replays")
        End If

        'Install Dependencies if needed
        If Not File.Exists(NullDCPath & "\XInputInterface.dll") Or
            Not File.Exists(NullDCPath & "\XInputDotNetPure.dll") Or
            Not File.Exists(NullDCPath & "\OpenTK.dll") Or
            Not File.Exists(NullDCPath & "\SDL2.dll") Or
            Not File.Exists(NullDCPath & "\SDL2_net.dll") Or
            Not File.Exists(NullDCPath & "\NAudio.dll") Then
            UnzipResToDir(My.Resources.Deps, "bear_tmp_deps.zip", NullDCPath)
        End If

        ' Just copy the beargamma plugin everytime the launcher starts, to make sure w.e version is in the launcher is the one that's in the plugins folder
        Try
            My.Computer.FileSystem.WriteAllBytes(NullDCPath & "\Plugins\BEARJamma_Win32.dll", My.Resources.BEARJamma_Win32, False)
            My.Computer.FileSystem.WriteAllBytes(NullDCPath & "\nullDC_GUI_Win32.dll", My.Resources.nullDC_GUI_Win32, False)
            My.Computer.FileSystem.WriteAllBytes(NullDCPath & "\nullDC_Win32_Release-NoTrace.exe", My.Resources.nullDC_Win32_Release_NoTrace, False)
        Catch ex As Exception
            MsgBox("Could not access nullDC files, exit nullDC before starting BEAR.")
            End
        End Try

        ' FPS Limited Doesn't Exist lets Create it
        If Not File.Exists(NullDCPath & "\d3d9.dll") Then File.WriteAllBytes(NullDCPath & "\d3d9.dll", My.Resources.d3d9)
        File.WriteAllLines(NullDCPath & "\antilag.cfg", {"[config]", "RenderAheadLimit=0", "FPSlimit=60"})

        ' Remove any honey files that may have been left over if someone quit or it crashed or w.e reason, but if it fails then fuck it let em stay
        Try
            Dim _honey As String() = Directory.GetFiles(NullDCPath & "\roms", "*.honey")
            For Each _file In _honey
                File.SetAttributes(_file, FileAttributes.Normal)
                File.Delete(_file)
            Next
        Catch ex As Exception

        End Try

        GetGamesList()
    End Sub

    Public Sub GetGamesList()
        GamesList.Clear()
        GameSelectForm.cbGameList.ValueMember = "Rom"
        GameSelectForm.cbGameList.DisplayMember = "Game"
        HostingForm.cbGameList.ValueMember = "Rom"
        HostingForm.cbGameList.DisplayMember = "Game"


        Dim table As New DataTable
        table.Columns.Add("Rom", GetType(String))
        table.Columns.Add("Game", GetType(String))

        For Each Dir As String In Directory.GetDirectories(NullDCPath & "\roms")
            Dim Files = Directory.GetFiles(Dir)
            For Each file In Files
                Dim FileType = file.Split(".")
                If FileType(FileType.Count - 1) = "lst" Then
                    Dim GameName_Split = file.Split("\")

                    Dim GameName As String = GameName_Split(GameName_Split.Count - 2)
                    Dim RomName As String = GameName_Split(GameName_Split.Count - 1)
                    Dim RomPath As String = file.Replace(NullDCPath, "")

                    If Not GamesList.ContainsKey(RomName) Then
                        GamesList.Add(RomName, {GameName, RomPath})
                        table.Rows.Add({RomName, GameName})
                    End If

                End If
            Next
        Next

        GameSelectForm.cbGameList.DataSource = table
        HostingForm.cbGameList.DataSource = table

        'GameSelectForm.cbGameList.SelectedIndex = 0
        'HostingForm.cbGameList.SelectedIndex = 0

    End Sub

    Private Sub RefreshTimer_tick(sender As Object, e As EventArgs)
        RefreshTimer.Stop()

    End Sub

    Private Sub frmMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        ConfigFile.Status = ConfigFile.AwayStatus
        ConfigFile.SaveFile(False)

        NetworkHandler.SendMessage("&")

        niBEAR.Visible = False
        niBEAR.Icon = Nothing

        If IsNullDCRunning() Then NullDCLauncher.NullDCproc.CloseMainWindow()
        If Not Challenger Is Nothing Then NetworkHandler.SendMessage(">, Q", Challenger.ip)


        End

    End Sub

    Public Delegate Sub RemoveChallenger_delegate()
    Public Sub RemoveChallenger()
        Challenger = Nothing

    End Sub

    Public Delegate Sub SetGame_delegate(ByVal _game As String)
    Public Sub SetGame(Optional ByVal _game As String = Nothing)
        If _game Is Nothing Then
            ConfigFile.Game = "None"
        Else
            ConfigFile.Game = _game
        End If
        ConfigFile.SaveFile()

    End Sub

    Public Delegate Sub JoinHost_delegate(ByVal _name As String, ByVal _ip As String, ByVal _port As String, ByVal _game As String, ByVal _delay As Int16, ByVal _region As String, ByVal _eeprom As String)
    Public Sub JoinHost(ByVal _name As String, ByVal _ip As String, ByVal _port As String, ByVal _game As String, ByVal _delay As Int16, ByVal _region As String, ByVal _eeprom As String)
        If WaitingForm.Visible Then WaitingForm.Visible = False
        Rx.WriteEEPROM(_eeprom, MainformRef.NullDCPath & MainformRef.GamesList(_game)(1))
        Challenger = New NullDCPlayer(_name, _ip, _port, _game)
        ConfigFile.Host = _ip
        ConfigFile.Port = _port
        ConfigFile.Status = "Client"
        ConfigFile.Game = _game
        ConfigFile.Delay = _delay
        ConfigFile.ReplayFile = ""
        ConfigFile.SaveFile()
        NullDCLauncher.LaunchDC(ConfigFile.Game, _region)

    End Sub

    Public Delegate Sub JoinAsSpectator_delegate(ByVal _p1name As String, ByVal _p2name As String, ByVal _ip As String, ByVal _port As String, ByVal _game As String, ByVal _region As String, ByVal _eeprom As String)
    Public Sub JoinAsSpectator(ByVal _p1name As String, ByVal _p2name As String, ByVal _ip As String, ByVal _port As String, ByVal _game As String, ByVal _region As String, ByVal _eeprom As String)
        Rx.WriteEEPROM(_eeprom, MainformRef.NullDCPath & MainformRef.GamesList(_game)(1))
        Challenger = Nothing
        ConfigFile.Host = _ip
        ConfigFile.Port = _port
        ConfigFile.Status = "Spectator"
        ConfigFile.Game = _game
        ConfigFile.ReplayFile = ""
        ConfigFile.SaveFile()

        NullDCLauncher.P1Name = _p1name
        NullDCLauncher.P2Name = _p2name
        WaitingForm.Visible = False
        ChallengeSentForm.Visible = False
        NullDCLauncher.LaunchDC(ConfigFile.Game, _region)
    End Sub

    Public Delegate Sub RemovePlayerFromList_delegate(ByVal IP As String)
    Public Sub RemovePlayerFromList(ByVal IP As String)
        Console.WriteLine("Removing Player: " & IP)
        For Each playerentry As ListViewItem In Matchlist.Items
            If playerentry.SubItems(1).Text.Split(":")(0) = IP Then
                Matchlist.Items.Remove(playerentry)
                Exit For
            End If
        Next

    End Sub

    Public Delegate Sub AddPlayerToList_delegate(ByVal Player As NullDCPlayer)
    Public Sub AddPlayerToList(ByVal Player As NullDCPlayer)
        'Check if this IP:Port combo exists
        Dim FoundEntry As ListViewItem = Nothing
        Dim FoundEntryID = 0
        For Each playerentry As ListViewItem In Matchlist.Items
            If playerentry.SubItems(1).Text = Player.ip & ":" & Player.port Then
                FoundEntry = playerentry
                Exit For
            End If

            FoundEntryID += 1
        Next

        If FoundEntry Is Nothing Then

            Dim ItemNumber As Int16 = Matchlist.Items.Count
            Dim PlayerInfo As ListViewItem = New ListViewItem(Player.name, Player.name)
            PlayerInfo.SubItems.Add(Player.ip & ":" & Player.port)

            Dim pingthread As Thread = New Thread(Sub()
                                                      If My.Computer.Network.Ping(Player.ip) Then
                                                          Dim Pinger As Ping = New Ping()
                                                          Dim rep As PingReply = Pinger.Send(Player.ip, 1000)
                                                          Dim Ping = rep.RoundtripTime.ToString
                                                          MainformRef.Matchlist.Invoke(Sub() MainformRef.Matchlist.Items(ItemNumber).SubItems(2).Text = Ping)
                                                      End If
                                                  End Sub)
            pingthread.Start()

            PlayerInfo.SubItems.Add("T/O")
            PlayerInfo.SubItems.Add(Player.game)
            PlayerInfo.SubItems.Add(Player.status)
            PlayerInfo.BackColor = Color.FromArgb(1, 255, 250, 50)
            PlayerInfo.ForeColor = Color.FromArgb(1, 5, 5, 5)
            Matchlist.Items.Add(PlayerInfo)
        Else
            Matchlist.Items(FoundEntryID).SubItems(0).Text = Player.name
            Matchlist.Items(FoundEntryID).SubItems(1).Text = Player.ip & ":" & Player.port

            Dim pingthread As Thread = New Thread(Sub()
                                                      If My.Computer.Network.Ping(Player.ip) Then
                                                          Dim Pinger As Ping = New Ping()
                                                          Dim rep As PingReply = Pinger.Send(Player.ip, 1000)
                                                          Dim Ping = rep.RoundtripTime.ToString
                                                          MainformRef.Matchlist.Invoke(Sub() MainformRef.Matchlist.Items(FoundEntryID).SubItems(2).Text = Ping)
                                                      End If
                                                  End Sub)
            pingthread.Start()

            Matchlist.Items(FoundEntryID).SubItems(3).Text = Player.game
            Matchlist.Items(FoundEntryID).SubItems(4).Text = Player.status

        End If



    End Sub

    Public Delegate Sub BeingChallenged_delegate(ByVal _name As String, ByVal _ip As String, ByVal _port As String, ByVal _game As String, ByVal _hosting As String)
    Public Sub BeingChallenged(ByVal _name As String, ByVal _ip As String, ByVal _port As String, ByVal _game As String, ByVal _hosting As String)
        ChallengeForm.StartChallenge(New NullDCPlayer(_name, _ip, _port, _game))
    End Sub

    ' Partner has terminated the session with a reason, or maybe you did with no reason etherway this handles it
    Public Delegate Sub EndSession_delegate(ByVal Reason As String, ByVal _canceledby As String)
    Public Sub EndSession(ByVal Reason As String, Optional ByVal _canceledby As String = Nothing)
        Console.WriteLine("End Session: {0} -> {1}", Reason, _canceledby)
        'Canceled by self or other
        ChallengeForm.Visible = False
        ChallengeSentForm.Visible = False
        WaitingForm.Visible = False
        HostingForm.Visible = False

        If _canceledby Is Nothing Then
            ' Hide all the Panels

            Select Case Reason
                Case "Window Closed" ' This is only fired automatically by the emulator when it closes, sometimes we need to ignore this
                    Console.WriteLine("Window Closed")
                    RemoveChallenger() ' Remove Challenger Data in case we have any
                    ' Set game to none and back to idle
                    ConfigFile.Game = "None"
                    ConfigFile.Status = MainformRef.ConfigFile.AwayStatus
                    ConfigFile.SaveFile()
                Case "New Challenger" ' This is fired when you accept to fight a new challanger
                    Console.Write("New Challanger")
                    If IsNullDCRunning() Then
                        NullDCLauncher.DoNotSendNextExitEvent = True
                        NullDCLauncher.NullDCproc.CloseMainWindow()
                        While IsNullDCRunning() ' Just to make sure it's dead before we accept
                            NullDCLauncher.NullDCproc.CloseMainWindow()
                            Thread.Sleep(100)
                        End While
                    End If
                    ' Set out game to w.e our challenger wants to play and set outself to client
                    ConfigFile.Game = Challenger.game
                    ConfigFile.Status = "Client"
                    ConfigFile.SaveFile()
                Case "Denied", "TO" ' T/O or Denied
                    RemoveChallenger()
                    ConfigFile.Game = "None"
                    ConfigFile.Status = MainformRef.ConfigFile.AwayStatus
                    ConfigFile.SaveFile()
                Case "Host Canceled" ' You were hosting but then you stopped Clear the game info
                    RemoveChallenger()
                    ConfigFile.Game = "None"
                    ConfigFile.Status = MainformRef.ConfigFile.AwayStatus
                    ConfigFile.SaveFile()
                Case "New Host" ' You started a new host Just close the nullDC if it's open, disable raising it's event
                    If IsNullDCRunning() Then
                        NullDCLauncher.NullDCproc.EnableRaisingEvents = False ' Disable Event Raising so this doesn't automaticlly clear challenger data
                        NullDCLauncher.NullDCproc.CloseMainWindow()
                        While IsNullDCRunning() ' Just to make sure it's dead before we accept
                            NullDCLauncher.NullDCproc.CloseMainWindow()
                            Thread.Sleep(100)
                        End While
                    End If
                    ConfigFile.Status = "Hosting"
                    ConfigFile.Game = "None"
                    ConfigFile.SaveFile()
            End Select
        Else
            ' This end session was called by someone else, IE: person left the game etc
            Console.WriteLine("End Session :" & _canceledby)
            Dim Message = ""

            ' Very Rare instances where we need to send an error but not related to your challanger
            Select Case Reason
                Case "HO"
                    Challenger = Nothing
                    Message = "Played isn't hosting anymore"
                    NotificationForm.ShowMessage(Message)
                Case "HA"
                    Challenger = Nothing
                    Message = "Player is already hosting a game, refresh and join their game"
                    NotificationForm.ShowMessage(Message)
            End Select

            If Challenger Is Nothing Then
                Exit Sub ' Ok so someone told you to End Session but you don't have a challanger, so just ignore him
            Else
                If Not Message = "" Then
                    NotificationForm.ShowMessage(Message)
                    Exit Sub
                End If
            End If

            If Not Challenger.ip = _canceledby Then Exit Sub ' ok you do have a challanger, then check if the IP match before you cancel

            RemoveChallenger()
            ChallengeForm.Visible = False
            ChallengeSentForm.Visible = False
            WaitingForm.Visible = False
            HostingForm.Visible = False

            'If IsNullDCRunning() Then NullDCLauncher.NullDCproc.CloseMainWindow() ' Close the NullDC Window

            While IsNullDCRunning()
                NullDCLauncher.NullDCproc.CloseMainWindow()
                Thread.Sleep(100)
            End While

            Select Case Reason
                Case "D"
                    Message = "Player was too scared of you to accept."
                Case "T"
                    Message = "Player might be asleep, request timed out."
                Case "C"
                    Message = "Player changed their mind."
                Case "H"
                    Message = "The host quit"
                Case "E"
                    Message = "Other Player Left"
                Case "BB"
                    Message = "Player is currently challenging someone"
                Case "Q"
                    Message = "Player quit"
                Case "VM"
                    Message = "PLAYER IS USING OTHER BEAR VERSION"
                Case "BI"
                    Message = "Player is already in a game"
                Case "NG"
                    Message = "Player doesn't have this game or rom (lst file) do not match"
                Case "BO"
                    Message = "Challanger got bored of waiting"
                Case "HO"
                    Message = "Player Stopped Hosting, refresh the list plox"
                Case "BH"
                    Message = "Host hasn't started a game yet"
                Case "BW"
                    Message = "Player is connecting to someone else"
                Case "BB"
                    Message = "Player is challenging someone else"
                Case "BC"
                    Message = "Player is being challenged by someone else"
                Case "SP"
                    Message = "Player is setting up BEAR"
                Case "NS"
                    Message = "Player does not allow spectators"
                Case "NSS"
                    Message = "Player is spectating or watching a replay, can't spectate them."
                Case "DND"
                    Message = "Player is not accepting challenges right now."
            End Select

            If Not Message = "" Then NotificationForm.ShowMessage(Message)

            ConfigFile.Game = "None"
            ConfigFile.Status = MainformRef.ConfigFile.AwayStatus
            ConfigFile.SaveFile()
        End If

    End Sub

    Public Delegate Sub OpenHostingPanel_delegate(ByRef _player As NullDCPlayer)
    Public Sub OpenHostingPanel(Optional ByRef _player As NullDCPlayer = Nothing)
        HostingForm.BeginHost(_player)
    End Sub

#Region "Moving The Window Around"
    ' Moving the window
    Private newpoint As System.Drawing.Point
    Private xpos1 As Integer
    Private ypos1 As Integer

    Private Sub pnlTopBorder_MouseDown(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseDown, BEARTitle.MouseDown, imgBeta.MouseDown, lbVer.MouseDown
        xpos1 = Control.MousePosition.X - Me.Location.X
        ypos1 = Control.MousePosition.Y - Me.Location.Y
    End Sub

    Private Sub pnlTopBorder_MouseMove(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseMove, BEARTitle.MouseMove, imgBeta.MouseMove, lbVer.MouseMove
        If e.Button = MouseButtons.Left Then
            newpoint = Control.MousePosition
            newpoint.X -= (xpos1)
            newpoint.Y -= (ypos1)
            Me.Location = newpoint
        End If

    End Sub
#End Region

#Region "Button Clicks"
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If KeyMappingForm.Visible Then
            KeyMappingForm.Focus()
        Else
            KeyMappingForm.Show(Me)
        End If


    End Sub

    Private Sub btnOffline_Click(sender As Object, e As EventArgs) Handles btnOffline.Click
        GameSelectForm.StartChallenge()
    End Sub

    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        RefreshPlayerList()
    End Sub

    Private Sub RefreshPlayerList(Optional ByVal StartTimer As Boolean = True)

        If Not RefreshTimer.Enabled Then
            If StartTimer Then
                Matchlist.Items.Clear()
                RefreshTimer.Interval = 5000
                RefreshTimer.Start()
            End If
            NetworkHandler.SendMessage("?," & ConfigFile.IP)
            ' Send info on myself also

            Dim NameToSend As String = MainformRef.ConfigFile.Name
            If Not MainformRef.Challenger Is Nothing Then NameToSend = NameToSend & " vs " & MainformRef.Challenger.name

            Dim GameNameAndRomName = "None"
            If Not MainformRef.ConfigFile.Game = "None" Then GameNameAndRomName = MainformRef.GamesList(MainformRef.ConfigFile.Game)(0) & "|" & MainformRef.ConfigFile.Game
            NetworkHandler.SendMessage("<," & NameToSend & "," & MainformRef.ConfigFile.IP & "," & MainformRef.ConfigFile.Port & "," & GameNameAndRomName & "," & MainformRef.ConfigFile.Status)
        Else
            NotificationForm.ShowMessage("Slow down cowboy, wait at least 5 seconds between refreshing")
        End If

    End Sub

    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        For i = 0 To Application.OpenForms.Count - 1
            If i = 0 Then Continue For

            Application.OpenForms(i).Close()
        Next

        Me.Close()

    End Sub

    Private Sub btnMinimize_Click(sender As Object, e As EventArgs) Handles btnMinimize.Click
        Me.WindowState() = FormWindowState.Minimized

    End Sub

    Private Sub btnSetup_Click(sender As Object, e As EventArgs) Handles btnSetup.Click
        If Not Application.OpenForms().OfType(Of frmSetup).Any Then
            frmSetup.Show(Me)
        End If

    End Sub

    Private Sub BtnJoin_Click(sender As Object, e As EventArgs) Handles BtnJoin.Click
        TryToChallenge()
    End Sub

    Private Sub TryToChallenge()

        If Not Challenger Is Nothing Or GameSelectForm.Visible Then
            NotificationForm.ShowMessage("Already Challenging someone")
            Exit Sub
        End If

        If Matchlist.SelectedItems().Count = 0 Then
            NotificationForm.ShowMessage("You can't the fight wall, choose a player from the list")
            Exit Sub
        End If

        If Matchlist.SelectedItems(0).SubItems(1).Text.Split(":")(0) = ConfigFile.IP Then
            NotificationForm.ShowMessage("I can't really help you with your inner demons if you want to fight yourself.")
            Exit Sub
        End If

        ' Check if player is already hosting, if they are then just join and skip all the checks
        Dim c_name As String = Matchlist.SelectedItems(0).SubItems(0).Text
        Dim c_ip As String = Matchlist.SelectedItems(0).SubItems(1).Text.Split(":")(0)
        Dim c_port As String = Matchlist.SelectedItems(0).SubItems(1).Text.Split(":")(1)
        Dim c_gamerom As String = Matchlist.SelectedItems(0).SubItems(3).Text
        If Not c_gamerom = "None" Then c_gamerom = c_gamerom.Split("|")(1) ' Game is not None, so get what rom it is.
        Dim c_status As String = Matchlist.SelectedItems(0).SubItems(4).Text

        Console.WriteLine("Challange: " & c_ip)
        ' Skip game Selection if person is already in a game, try to spectate instead.
        If Not c_status = "Idle" Then ' this person is playing SOMETHING so lets try to challange them and see what they reply
            If c_status = "DND" Then
                NotificationForm.ShowMessage("Player is not accepting challenges right now.")
                Exit Sub
            End If
            ' Check if you have the game
            If Not GamesList.ContainsKey(c_gamerom) Then
                'MsgBox(c_gamerom)
                NotificationForm.ShowMessage("You don't have this game (" & c_gamerom & ") not found.")
                Exit Sub
            End If

            Challenger = New NullDCPlayer(c_name, c_ip, c_port, c_gamerom)
            NetworkHandler.SendMessage("!," & ConfigFile.Name & "," & ConfigFile.IP & "," & ConfigFile.Port & "," & Challenger.game & ",0", Challenger.ip)
            WaitingForm.Show()
        Else
            GameSelectForm.StartChallenge(New NullDCPlayer(c_name, c_ip, c_port)) ' If they not hosting then just start choosing game to challenge them to
        End If

    End Sub

    Private Sub btnHost_Click(sender As Object, e As EventArgs) Handles btnHost.Click
        If IsNullDCRunning() Then
            NotificationForm.ShowMessage("Can't Host and play at the same time!")
            Exit Sub
        End If

        If Challenger Is Nothing Then
            OpenHostingPanel()
        Else
            NotificationForm.ShowMessage("I mean i admire your multitasking you can't fight two people at once")
        End If

    End Sub

#End Region

    Public Function IsNullDCRunning() As Boolean
        If NullDCLauncher.NullDCproc Is Nothing Then Return False
        If NullDCLauncher.NullDCproc.HasExited Then Return False
        Return True

    End Function

    Private Sub Matchlist_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles Matchlist.MouseDoubleClick
        TryToChallenge()
    End Sub

    Private Sub frmMain_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize

        Matchlist.Columns(0).Width = Matchlist.Width * 0.32
        Matchlist.Columns(1).Width = 0
        Matchlist.Columns(2).Width = Matchlist.Width * 0.11
        Matchlist.Columns(3).Width = Matchlist.Width * 0.39
        Matchlist.Columns(4).Width = Matchlist.Width * 0.18 - 25

        If Me.WindowState = FormWindowState.Minimized Then
            niBEAR.Visible = True
            niBEAR.BalloonTipIcon = ToolTipIcon.None
            niBEAR.BalloonTipTitle = "NulDC BEAR"
            niBEAR.BalloonTipText = "Aight, I'll be here if you need me."
            niBEAR.ShowBalloonTip(50000)
            ShowInTaskbar = False ' this removes the form from the openforms... so gona disable it for now
        End If

    End Sub

    Private Sub niBEAR_DoubleClick(sender As Object, e As EventArgs) Handles niBEAR.DoubleClick
        ShowInTaskbar = True
        Me.WindowState = FormWindowState.Normal
        niBEAR.Visible = False

    End Sub

    Private Sub btnReplay_Click(sender As Object, e As EventArgs) Handles btnReplay.Click
        If Not Application.OpenForms().OfType(Of frmReplays).Any Then
            frmReplays.Show(Me)
        Else
            frmReplays.Focus()
        End If

    End Sub

    Private Sub btnDLC_Click(sender As Object, e As EventArgs) Handles btnDLC.Click
        If Not Application.OpenForms().OfType(Of frmDLC).Any Then
            frmDLC.Show(Me)
        Else
            frmDLC.Focus()
        End If

    End Sub

    Private Sub btnPatreon_Click(sender As Object, e As EventArgs) Handles btnPatreon.Click
        Process.Start("https://www.patreon.com/NullDCBEAR")
    End Sub

    Private Sub cbStatus_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbStatus.SelectedIndexChanged
        If Not NetworkHandler Is Nothing Then
            ConfigFile.AwayStatus = cbStatus.Text

            If Not IsNullDCRunning() Then
                ConfigFile.Status = cbStatus.Text
                ConfigFile.SaveFile()
            End If

        End If
        ActiveControl = Nothing
    End Sub
End Class

Public Class NullDCPlayer
    Public name As String
    Public ip As String
    Public port As String
    Public status As String
    Public game As String

    Public Sub New(ByVal _name As String, ByVal _ip As String, ByVal _port As String, Optional ByVal _game As String = "None", Optional ByVal _status As String = "Idle")
        name = _name
        ip = _ip
        port = _port
        status = _status
        game = _game

    End Sub

End Class

Public Class Configs

    Private _name = "Player"
    Private _network = "Radmin VPN"
    Private _port = "27886"
    Private _useremapper As Boolean = True
    Private _firstrun As Boolean = True
    Private _ip As String = "127.0.0.1"
    Private _host As String = "127.0.0.1"
    Private _status As String = "Idle"
    Private _delay As Int16 = 1
    Private _game As String = "None"
    Private _ver As String = frmMain.Ver
    Private _hosttype As String = "1"
    Private _fpslimit As String = "60"
    Private _keyprofile As String = "Default"
    Private _recordreplay As Int16 = 0
    Private _ReplayFile As String = ""
    Private _allowSpectators As Int16 = 1
    Private _awaystatus As String = "Idle"
    Private _volume As Int16 = 100

#Region "Properties"

    Public Property Host() As String
        Get
            Return _host
        End Get
        Set(ByVal value As String)
            _host = value
        End Set
    End Property

    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            _name = value
        End Set
    End Property

    Public Property Network() As String
        Get
            Return _network
        End Get
        Set(ByVal value As String)
            _network = value
        End Set

    End Property

    Public Property Port() As String
        Get
            Return _port
        End Get
        Set(ByVal value As String)
            _port = value
        End Set
    End Property

    Public Property IP() As String
        Get
            Return _ip
        End Get
        Set(ByVal value As String)
            _ip = value
        End Set
    End Property

    Public Property Status() As String
        Get
            Return _status
        End Get
        Set(ByVal value As String)
            _status = value
        End Set
    End Property

    Public Property Delay() As Int16
        Get
            Return _delay
        End Get
        Set(ByVal value As Int16)
            _delay = value
        End Set
    End Property

    Public Property UseRemap() As Boolean
        Get
            Return _useremapper
        End Get
        Set(ByVal value As Boolean)
            _useremapper = value
        End Set
    End Property

    Public Property FirstRun() As Boolean
        Get
            Return _firstrun
        End Get
        Set(ByVal value As Boolean)
            _firstrun = value
        End Set
    End Property

    Public Property Game() As String
        Get
            Return _game
        End Get
        Set(ByVal value As String)
            _game = value
        End Set
    End Property

    Public Property Version() As String
        Get
            Return _ver
        End Get
        Set(ByVal value As String)
            _ver = value
        End Set
    End Property

    Public Property HostType() As String
        Get
            Return _hosttype
        End Get
        Set(ByVal value As String)
            _hosttype = value
        End Set
    End Property

    Public Property FPSLimit() As String
        Get
            Return _fpslimit
        End Get
        Set(ByVal value As String)
            _fpslimit = value
        End Set
    End Property

    Public Property KeyMapProfile() As String
        Get
            Return _keyprofile
        End Get
        Set(ByVal value As String)
            _keyprofile = value
        End Set
    End Property

    Public Property RecordReplay() As Int16
        Get
            Return _recordreplay
        End Get
        Set(ByVal value As Int16)
            _recordreplay = value
        End Set
    End Property

    Public Property ReplayFile() As String
        Get
            Return _ReplayFile
        End Get
        Set(ByVal value As String)
            _ReplayFile = value
        End Set
    End Property

    Public Property AllowSpectators() As Int16
        Get
            Return _allowSpectators
        End Get
        Set(ByVal value As Int16)
            _allowSpectators = value
        End Set
    End Property

    Public Property AwayStatus() As String
        Get
            Return _awaystatus
        End Get
        Set(ByVal value As String)
            _awaystatus = value
        End Set
    End Property

    Public Property Volume() As Int16
        Get
            Return _volume
        End Get
        Set(ByVal value As Int16)
            _volume = value
        End Set
    End Property

#End Region

    Public Sub SaveFile(Optional ByVal SendIam As Boolean = True)
        Dim NullDCPath = frmMain.NullDCPath
        Dim lines() As String =
            {
                "[BEAR]",
                "Version=" & Version,
                "Name=" & Name,
                "Network=" & Network,
                "Port=" & Port,
                "Reclaw=" & UseRemap,
                "IP=" & IP,
                "Host=" & Host,
                "Status=" & Status,
                "Delay=" & Delay,
                "Game=" & Game,
                "HostType=" & HostType,
                "FPSLimit=" & FPSLimit,
                "KeyProfile=" & KeyMapProfile,
                "RecordReplay=" & RecordReplay,
                "ReplayFile=" & ReplayFile,
                "AllowSpectators=" & AllowSpectators,
                "AwayStatus=" & AwayStatus,
                "Volume=" & Volume
            }
        File.WriteAllLines(NullDCPath & "\NullDC_BEAR.cfg", lines)

        If Not MainformRef.NetworkHandler Is Nothing Then
            If AwayStatus = "Hidden" Then
                MainformRef.NetworkHandler.SendMessage("&")
            Else
                If SendIam Then
                    Dim GameNameAndRomName = "None"
                    If Not Game = "None" Then GameNameAndRomName = MainformRef.GamesList(MainformRef.ConfigFile.Game)(0) & "|" & MainformRef.ConfigFile.Game

                    Dim NameToSend As String = Name
                    If Not MainformRef.Challenger Is Nothing Then NameToSend = Name & " Vs " & MainformRef.Challenger.name

                    MainformRef.NetworkHandler.SendMessage("<," & NameToSend & "," & IP & "," & Port & "," & GameNameAndRomName & "," & Status)
                End If
            End If
        End If
    End Sub

    Public Sub New(ByRef NullDCPath As String)

        If Not File.Exists(NullDCPath & "\NullDC_BEAR.cfg") Then
            FirstRun = True
            SaveFile()
        Else
            FirstRun = False
            Dim thefile = NullDCPath & "\NullDC_BEAR.cfg"
            Dim lines() As String = File.ReadAllLines(thefile)
            For Each line As String In lines
                If line.Contains("Name") Then Name = line.Split("=")(1).Trim
                If line.Contains("Network") Then Network = line.Split("=")(1).Trim
                If line.Contains("Port") Then Port = line.Split("=")(1).Trim
                If line.Contains("Reclaw") Then UseRemap = line.Split("=")(1).Trim
                If line.Contains("IP") Then IP = line.Split("=")(1).Trim
                If line.Contains("Host") Then Host = line.Split("=")(1).Trim
                If line.Contains("Delay") Then Delay = line.Split("=")(1).Trim
                If line.Contains("HostType") Then HostType = line.Split("=")(1).Trim
                If line.Contains("FPSLimit") Then FPSLimit = line.Split("=")(1).Trim
                If line.Contains("KeyProfile") Then KeyMapProfile = line.Split("=")(1).Trim
                If line.Contains("RecordReplay") Then RecordReplay = line.Split("=")(1).Trim
                If line.Contains("ReplayFile") Then ReplayFile = line.Split("=")(1).Trim
                If line.Contains("AllowSpectators") Then AllowSpectators = line.Split("=")(1).Trim
                If line.Contains("AwayStatus") Then
                    AwayStatus = line.Split("=")(1).Trim
                    Status = line.Split("=")(1).Trim
                End If
                If line.Contains("Volume") Then Volume = line.Split("=")(1).Trim
            Next
            Game = "None"
            SaveFile()
            Exit Sub
        End If
    End Sub

End Class
