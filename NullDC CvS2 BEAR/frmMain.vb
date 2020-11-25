Imports System.IO
Imports System.Net
Imports System.Net.NetworkInformation
Imports System.Security.Cryptography
Imports System.Text
Imports System.Threading

Public Class frmMain
    Public IsBeta As Boolean = True

    ' Update Stuff
    Dim UpdateCheckClient As New WebClient

    Public Ver As String = "1.75c" 'Psst make sure to also change DreamcastGameOptimizations.txt

    ' Public InputHandler As InputHandling
    Public NetworkHandler As NetworkHandling
    Public NullDCLauncher As NullDCLauncher
    Public MednafenLauncher As New MednafenLauncher
    Public NullDCPath As String = Application.StartupPath
    Public GamesList As New Dictionary(Of String, Array)
    Public ConfigFile As Configs
    Public FirstRun As Boolean = True
    Public ChallengeForm As frmChallenge
    Public ChallengeSentForm As frmChallengeSent
    Public HostingForm As frmHostPanel
    Public GameSelectForm As frmChallengeGameSelect
    Public WaitingForm As frmWaitingForHost
    Public NotificationForm As frmNotification
    'Public KeyMappingForm As frmKeyMapping
    Public Challenger As NullDCPlayer
    Private RefreshTimer As System.Windows.Forms.Timer = New System.Windows.Forms.Timer
    Private FormLoaded As Boolean = False
    Public FinishedLoading As Boolean = False

    Dim needsUpdate As Boolean = False

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'ZipFile.CreateFromDirectory("D:\VS_Projects\NullDC-BEAR\NullDC CvS2 BEAR\bin\x86\Debug\zip", "mednafen-server.zip")
        'If Debugger.IsAttached Then NullDCPath = "D:\Games\Emulators\NullDC\nulldc-1-0-4-en-win"

        Me.Icon = My.Resources.NewNullDCBearIcon
        niBEAR.Icon = My.Resources.NewNullDCBearIcon
        Me.CenterToScreen()

        imgBeta.Visible = IsBeta

        Rx.MainformRef = Me
        lbVer.Text = Ver

        If Not IsBeta Then
            Dim files As String() = Directory.GetFiles(NullDCPath & "\", "*.exe")
            For Each file In files
                Dim tempFileName = file.Split("\")(file.Split("\").Length - 1)
                If tempFileName.ToLower.StartsWith("nulldc.bear") Then
                    If Not tempFileName.ToLower = "nulldc.bear.exe" Then
                        MsgBox("Found a weird bear exe, please remove/rename   " & vbNewLine & tempFileName & "    to    NullDC.BEAR.exe   " & vbNewLine & "or you might lose settings, or keep updating every time you start it and other weird shit.")
                        Process.Start(file.Replace(tempFileName, ""))
                        End
                    End If
                End If
            Next
        End If

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
                Else
                    MsgBox("Oh ok, see ya.")
                    End
                End If
            Else
                MsgBox("I need to be in the NullDC folder where nullDC_Win32_Release-NoTrace.exe")
                End
            End If
        End If

        ChallengeSentForm = New frmChallengeSent(Me)
        HostingForm = New frmHostPanel(Me)
        GameSelectForm = New frmChallengeGameSelect(Me)
        WaitingForm = New frmWaitingForHost
        NotificationForm = New frmNotification(Me)

        CheckFilesAndShit()

        ConfigFile = New Configs(NullDCPath)
        cbStatus.Text = ConfigFile.Status

        Try
            ' Check SDL Version Swap
            If MainformRef.ConfigFile.SDLVersion.StartsWith("+") Or needsUpdate Then ' We Need to Change SDL Version
                If MainformRef.ConfigFile.SDLVersion = "+Stable" Or MainformRef.ConfigFile.SDLVersion = "Stable" Then
                    UnzipResToDir(My.Resources.SDL_Stable, "bear_tmp_sdl_stable.zip", NullDCPath & "\dc")
                    UnzipResToDir(My.Resources.SDL_Stable, "bear_tmp_sdl_stable.zip", NullDCPath)
                    MainformRef.ConfigFile.SDLVersion = "Stable"
                    MainformRef.ConfigFile.SaveFile(False)
                    Console.WriteLine("SDL Changed to: Stable")

                Else
                    UnzipResToDir(My.Resources.Deps, "bear_tmp_deps.zip", NullDCPath)
                    UnzipResToDir(My.Resources.Deps, "bear_tmp_deps.zip", NullDCPath & "\dc")
                    MainformRef.ConfigFile.SDLVersion = "Dev"
                    MainformRef.ConfigFile.SaveFile(False)
                    Console.WriteLine("SDL Changed to: Dev")
                End If

            End If
        Catch ex As Exception
            MsgBox("Couldn't change SDL version make sure nullDC is closed. Error: " & vbNewLine & ex.Message)
            End
        End Try


        ' Update Stuff
        AddHandler UpdateCheckClient.DownloadStringCompleted, AddressOf UpdateCheckResult
        CheckForUpdate()

        ' Create all the usual shit
        ChallengeForm = New frmChallenge(Me)
        'InputHandler = New InputHandling(Me)
        NetworkHandler = New NetworkHandling(Me)
        'KeyMappingForm = New frmKeyMapping(Me)
        NullDCLauncher = New NullDCLauncher
        ' 

        If ConfigFile.FirstRun Then frmSetup.ShowDialog(Me)
        AddHandler RefreshTimer.Tick, AddressOf RefreshTimer_tick

        If GamesList.Count = 0 Then
            NotificationForm.ShowMessage("You don't seem to have any games, click the Free DLC button to get some.")
        End If

        CreateRomFolderWatcher()
        RefreshPlayerList(False)

        ' Sus
        Randomize()
        Dim sus As Decimal = Rnd() * 10
        If sus <= 1 Then
            sus_i.Visible = True
        Else
            sus_i.Visible = False
        End If

        Console.WriteLine(sus)
        FinishedLoading = True

    End Sub

    Dim RomFolderWatcher As FileSystemWatcher
    Dim RomFolderWatcher_Dreamcast As FileSystemWatcher

    Private Sub CreateRomFolderWatcher()

        Dim RomFolders As String() = {
            NullDCPath & "\roms",
            NullDCPath & "\dc\roms",
            NullDCPath & "\mednafen\roms"
        }

        Dim Watchers(RomFolders.Count) As FileSystemWatcher
        Dim RomFoldersCount = 0
        For Each _romfolder In RomFolders
            Watchers(RomFoldersCount) = New FileSystemWatcher()
            Watchers(RomFoldersCount).IncludeSubdirectories = True
            Watchers(RomFoldersCount).Path = RomFolders(RomFoldersCount)

            Watchers(RomFoldersCount).NotifyFilter =
            NotifyFilters.CreationTime Or
            NotifyFilters.DirectoryName Or
            NotifyFilters.FileName Or
            NotifyFilters.LastWrite Or
            NotifyFilters.Size

            AddHandler Watchers(RomFoldersCount).Changed, AddressOf RomFolderChange
            AddHandler Watchers(RomFoldersCount).Created, AddressOf RomFolderChange
            AddHandler Watchers(RomFoldersCount).Renamed, AddressOf RomFolderChange
            AddHandler Watchers(RomFoldersCount).Deleted, AddressOf RomFolderChange

            Watchers(RomFoldersCount).EnableRaisingEvents = True
            RomFoldersCount += 1
        Next

    End Sub

    Private Sub RomFolderChange(ByVal source As FileSystemWatcher, ByVal e As FileSystemEventArgs)
        If Not e.Name.Contains("eeprom") Then ' As long as it has nothing to do with eeproms, then reload the games.
            Console.WriteLine("Roms folder changed, check if we have new games")
            source.EnableRaisingEvents = False
            Thread.Sleep(500)
            Me.Invoke(Sub() GetGamesList())
            source.EnableRaisingEvents = True
        End If
    End Sub

    Public Function IsFileInUse(sFile As String) As Boolean
        Try
            Using f As New FileStream(sFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None)
                f.Close()
            End Using

        Catch Ex As Exception
            Return True
        End Try

        Return False

    End Function

    Private Sub CheckForUpdate()
        ' Check if updating is turned off
        For Each CommandLine In Environment.GetCommandLineArgs
            If CommandLine = "-noupdate" Then Exit Sub

        Next

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
                Dim FolderOnly As Boolean = False
                If entry.FullName.EndsWith("\") Or entry.FullName.EndsWith("/") Then
                    FolderOnly = True
                End If
                Dim _fdir = entry.FullName.Replace("/", "\")
                Dim _dirbuilt = ""

                For Each _subdir In _fdir.Split("\")

                    If Not Directory.Exists(_dir & "\" & _dirbuilt & _subdir) Then
                        If Not FolderOnly And _subdir = _fdir.Split("\")(_fdir.Split("\").Length - 1) Then
                            Exit For
                        End If
                        Directory.CreateDirectory(_dir & "\" & _dirbuilt & "\" & _subdir)
                    End If

                    _dirbuilt += _subdir & "\"
                Next

                If Not FolderOnly Then
                    If File.Exists(_dir & "\" & entry.FullName.Replace("/", "\")) Then
                        File.SetAttributes(_dir & "\" & entry.FullName.Replace("/", "\"), FileAttribute.Normal)
                        File.Delete(_dir & "\" & entry.FullName.Replace("/", "\"))
                    End If

                    entry.ExtractToFile(_dir & "\" & entry.FullName.Replace("/", "\"), True)
                End If

            Next
        End Using

    End Sub

    Private Sub CheckFilesAndShit()

        ' Check the EXE name and all that shit from now on use the NullDC.BEAR.exe format since that's what github saves it as, since it hates spaces apperanly
        ' Why do this you may ask? Well mostly so people who downloaded it from github have the same exe name after they update, for firewall reasons


        If File.Exists(NullDCPath & "\NullDC_BEAR.cfg") Then
            Dim thefile = NullDCPath & "\NullDC_BEAR.cfg"
            Dim lines() As String = File.ReadAllLines(thefile)
            Dim tmpVersion = ""

            For Each line In lines
                If line.StartsWith("Version") Then tmpVersion = line.Split("=")(1).Trim
            Next

            If Not tmpVersion = Ver Then needsUpdate = True

        Else
            needsUpdate = True
        End If

        If Not My.Computer.FileSystem.DirectoryExists(NullDCPath & "\replays") Then
            My.Computer.FileSystem.CreateDirectory(NullDCPath & "\replays")
        End If

        ' FPS Limiter exists, lets remove it.
        If File.Exists(NullDCPath & "\antilag.cfg") Then
            File.SetAttributes(NullDCPath & "\antilag.cfg", FileAttributes.Normal)
            File.Delete(NullDCPath & "\antilag.cfg")

            If File.Exists(NullDCPath & "\d3d9.dll") Then
                File.SetAttributes(NullDCPath & "\d3d9.dll", FileAttributes.Normal)
                File.Delete(NullDCPath & "\d3d9.dll")
            End If
        End If

        ' Remove any honey files that may have been left over if someone quit or it crashed or w.e reason, but if it fails then fuck it let em stay
        Try
            Dim _honey As String() = Directory.GetFiles(NullDCPath & "\roms", "*.honey")
            For Each _file In _honey
                File.SetAttributes(_file, FileAttributes.Normal)
                File.Delete(_file)
            Next

            _honey = Directory.GetFiles(NullDCPath & "\dc\roms", "*.honey")
            For Each _file In _honey
                File.SetAttributes(_file, FileAttributes.Normal)
                File.Delete(_file)
            Next

        Catch ex As Exception

        End Try

        ' Extract Dreamcast Emulator if it does not exist
        If Not Directory.Exists(NullDCPath & "\dc") Then
            Directory.CreateDirectory(NullDCPath & "\dc")
            UnzipResToDir(My.Resources.DcClean, "bear_tmp_dreamcast_clean.zip", NullDCPath & "\dc")
        End If

        If Not File.Exists(NullDCPath & "\dc\GameSpecificSettings.optibear") Then
            File.WriteAllText(NullDCPath & "\dc\GameSpecificSettings.optibear", My.Resources.DreamcastGameOptimizations)

        Else
            If Not File.ReadAllLines(NullDCPath & "\dc\GameSpecificSettings.optibear")(0) = Ver Or IsBeta Then
                File.SetAttributes(NullDCPath & "\dc\GameSpecificSettings.optibear", FileAttributes.Normal)
                File.WriteAllText(NullDCPath & "\dc\GameSpecificSettings.optibear", My.Resources.DreamcastGameOptimizations)
                Console.WriteLine("Updated Optibear")
            End If

        End If

        If Not File.Exists(NullDCPath & "\Controls.bear") Then
            File.WriteAllBytes(NullDCPath & "\Controls.bear", My.Resources.Controls)
        End If

        ' Check if we need to add the new BEARJamma configs
        Dim redoNaomiConfigs As Boolean = True
        If File.Exists(NullDCPath & "\nullDC.cfg") Then
            For Each line In File.ReadAllLines(NullDCPath & "\nullDC.cfg")
                If line.StartsWith("BPortA_Joystick=") Then
                    redoNaomiConfigs = False
                    Exit For
                End If
            Next
        End If

        Dim redoDreamcastConfigs As Boolean = True
        If File.Exists(NullDCPath & "\dc\nullDC.cfg") Then
            For Each line In File.ReadAllLines(NullDCPath & "\dc\nullDC.cfg")
                If line.StartsWith("BPortA_Joystick=") Then
                    redoDreamcastConfigs = False
                    Exit For
                End If
            Next
        End If

        If redoNaomiConfigs Then
            Console.WriteLine("Re-extracted Naomi Configs")
            File.WriteAllBytes(NullDCPath & "\nullDC.cfg", My.Resources.default_nullDC_naomi)
        End If

        If redoDreamcastConfigs Then
            Console.WriteLine("Re-extracted Dreamcast Configs")
            File.WriteAllBytes(NullDCPath & "\dc\nullDC.cfg", My.Resources.default_nullDC_dreamcast)
        End If

        ' FIX MEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE
        ' Mednafen unpack
        If Not Directory.Exists(NullDCPath & "\mednafen") Then
            Directory.CreateDirectory(NullDCPath & "\mednafen")
            UnzipResToDir(My.Resources.mednafen, "bear_tmp_mednafen.zip", NullDCPath & "\mednafen")
        End If


        ' Mednafen Server
        If Not Directory.Exists(NullDCPath & "\mednafen\server") Then
            Directory.CreateDirectory(NullDCPath & "\mednafen\server")
            UnzipResToDir(My.Resources.mednafen_server, "bear_tmp_mednafen-server.zip", NullDCPath & "\mednafen\server")
        End If


        ' Just copy the beargamma plugin everytime the launcher starts, to make sure w.e version is in the launcher is the one that's in the plugins folder
        Try
            If needsUpdate Or IsBeta Then
                UnzipResToDir(My.Resources.Updates, "bear_tmp_updates.zip", NullDCPath)
                UnzipResToDir(My.Resources.Deps, "bear_tmp_deps.zip", NullDCPath)
                UnzipResToDir(My.Resources.Deps, "bear_tmp_deps.zip", NullDCPath & "\dc")
            End If

        Catch ex As Exception
            MsgBox("Could not access nullDC files, exit nullDC before starting BEAR.")
            End

        Finally
            'If needsUpdate Or IsBeta Then
            'NotificationForm.ShowMessage("Yo we have a new control system, go into the controls panel.")
            'End If
        End Try

        ' Put in the VMU to keep it in sync for now
        If Not File.Exists(MainformRef.NullDCPath & "\dc\vmu_data_host.bin") Then
            My.Computer.FileSystem.WriteAllBytes(MainformRef.NullDCPath & "\dc\vmu_data_host.bin", My.Resources.vmu_data_port01, False)
        End If

        If Not File.Exists(NullDCPath & "\Vs.png") Then My.Resources.Vs.Save(NullDCPath & "\Vs.png")
        If Not File.Exists(NullDCPath & "\Vs_2.png") Then My.Resources.Vs_2.Save(NullDCPath & "\Vs_2.png")
        If Not File.Exists(NullDCPath & "\dc\Vs.png") Then My.Resources.Vs.Save(NullDCPath & "\dc\Vs.png")
        If Not File.Exists(NullDCPath & "\dc\Vs_2.png") Then My.Resources.Vs_2.Save(NullDCPath & "\dc\Vs_2.png")

        GetGamesList()
        GetServerList()
    End Sub

    Private Sub GetServerList()
        Dim Files = Directory.GetFiles(NullDCPath, "*.slist", SearchOption.TopDirectoryOnly)
        Dim GameSelectServerList As New DataTable
        GameSelectServerList.Columns.Add("IP", GetType(String))
        GameSelectServerList.Columns.Add("Name", GetType(String))

        GameSelectForm.cb_Serverlist.ValueMember = "IP"
        GameSelectForm.cb_Serverlist.DisplayMember = "Name"

        GameSelectServerList.Rows.Add({"Offline", "Offline"})
        GameSelectServerList.Rows.Add({"Localhost", "Localhost"})

        Dim HostPanelServerList As New DataTable
        HostPanelServerList.Columns.Add("IP", GetType(String))
        HostPanelServerList.Columns.Add("Name", GetType(String))

        HostingForm.cb_Serverlist.ValueMember = "IP"
        HostingForm.cb_Serverlist.DisplayMember = "Name"

        HostPanelServerList.Rows.Add({"Localhost", "Localhost"})

        For Each _file In Files
            For Each _line In File.ReadAllLines(_file)
                If _line.Length > 2 Then
                    GameSelectServerList.Rows.Add({_line.Split(":")(1), _line.Split(":")(0)})
                    HostPanelServerList.Rows.Add({_line.Split(":")(1), _line.Split(":")(0)})
                End If
            Next
        Next

        GameSelectForm.cb_Serverlist.DataSource = GameSelectServerList
        HostingForm.cb_Serverlist.DataSource = HostPanelServerList
    End Sub

    Public Sub GetGamesList(Optional ByVal _system As String = "all")
        GamesList.Clear()

        ' New Get All Roms code that includes subfolders
        Dim Files As String()

        If _system = "all" Or _system = "na" Then
            If Not Directory.Exists(NullDCPath & "\roms") Then Directory.CreateDirectory(NullDCPath & "\roms")
            Files = Directory.GetFiles(NullDCPath & "\roms", "*.lst", SearchOption.AllDirectories)
            For Each _file In Files
                Dim GameName_Split As String() = _file.Split("\")

                Dim GameName As String = GameName_Split(GameName_Split.Count - 2).Trim.Replace(",", ".")
                Dim RomName As String = GameName_Split(GameName_Split.Count - 1).Replace(",", ".")
                Dim RomPath As String = _file.Replace(NullDCPath, "")

                If Not GamesList.ContainsKey("NA-" & RomName) Then
                    GamesList.Add("NA-" & RomName, {GameName, RomPath, "na", ""})
                End If
            Next
        End If

        If _system = "all" Or _system = "dc" Then
            If Not Directory.Exists(NullDCPath & "\dc\roms") Then Directory.CreateDirectory(NullDCPath & "\dc\roms")
            Dim hasher As MD5 = MD5.Create()
            Files = Directory.GetFiles(NullDCPath & "\dc\roms", "*.cdi", SearchOption.AllDirectories)
            Dim GDIFiles = Directory.GetFiles(NullDCPath & "\dc\roms", "*.gdi", SearchOption.AllDirectories)

            Dim InsertAtIndex = 0
            If Files.Count > 0 Then InsertAtIndex = Files.Count

            Array.Resize(Files, Files.Count + GDIFiles.Count + 1)
            GDIFiles.CopyTo(Files, InsertAtIndex)


            For Each _file In Files
                If IsFileInUse(_file) Then
                    Continue For
                End If

                Dim GameName_Split As String() = _file.Split("\")

                ' hash Generation
                Dim fs As New FileStream(_file, FileMode.Open)
                Dim binary_reader As New BinaryReader(fs)
                fs.Position = 0
                Dim BytesToHash = binary_reader.ReadBytes(5000000)
                ' GDI files put their actual name in the hash, because or how small the files are and they could have 2 files that are the same
                If GameName_Split(GameName_Split.Count - 1).ToLower.EndsWith(".gdi") Then
                    Dim NameAsByte = Encoding.ASCII.GetBytes(GameName_Split(GameName_Split.Count - 1).ToLower)
                    NameAsByte.CopyTo(BytesToHash, 0)
                End If


                Dim bytes() As Byte = hasher.ComputeHash(BytesToHash)

                Dim sBuilder As New StringBuilder()
                For n As Integer = 0 To bytes.Length - 1
                    sBuilder.Append(bytes(n).ToString("X2"))
                Next n
                fs.Close()

                Dim GameName As String = StrConv(GameName_Split(GameName_Split.Count - 1).ToLower.Replace(".cdi", "").Replace(".gdi", "").Replace(",", "."), vbProperCase).Split("[")(0).Split("(")(0).Trim
                Dim RomName As String = GameName_Split(GameName_Split.Count - 1).Replace(",", ".")

                Dim RomPath As String = _file.Replace(NullDCPath & "\dc\", "")

                If Not GamesList.ContainsKey("DC-" & RomName) Then
                    GamesList.Add("DC-" & RomName, {GameName, RomPath, "dc", sBuilder.ToString()})
                    Console.WriteLine("Found Game: RomName:" & RomName & " GameName:" & GameName & " RomPath:" & RomPath & " Platform:dc" & " " & sBuilder.ToString())
                End If

            Next

            ' Generate the hash file in the dc roms
            Dim _hashes As String = ""
            For Each _key In GamesList.Keys
                If Not GamesList(_key)(3) = "" Then
                    If _hashes = "" Then
                        _hashes = GamesList(_key)(0) & "::" & GamesList(_key)(3)
                    Else
                        _hashes = _hashes & vbNewLine & GamesList(_key)(0) & "::" & GamesList(_key)(3)
                    End If
                End If
            Next

            If _hashes.Length > 0 Then
                File.WriteAllLines(NullDCPath & "\dc\roms\romhashes.txt", _hashes.Split(vbNewLine))
            End If

        End If

        If _system = "all" Or _system = "ss" Then
            If Not Directory.Exists(NullDCPath & "\mednafen\roms\ss") Then Directory.CreateDirectory(NullDCPath & "\mednafen\roms\ss")
            Files = Directory.GetFiles(NullDCPath & "\mednafen\roms\ss", "*.cue", SearchOption.AllDirectories)
            For Each _file In Files
                Dim GameName_Split As String() = _file.Split("\")
                Dim GameName As String = GameName_Split(GameName_Split.Count - 1).Trim.Replace(".cue", "").Replace(",", ".")
                Dim RomName As String = GameName_Split(GameName_Split.Count - 1).Replace(",", ".")
                Dim RomPath As String = _file.Replace(NullDCPath, "")

                If Not GamesList.ContainsKey("SS-" & RomName) Then
                    GamesList.Add("SS-" & RomName, {GameName, RomPath, "ss", ""})
                End If
            Next

        End If

        If _system = "all" Or _system = "sg" Then
            If Not Directory.Exists(NullDCPath & "\mednafen\roms\sg") Then Directory.CreateDirectory(NullDCPath & "\mednafen\roms\sg")
            Files = Directory.GetFiles(NullDCPath & "\mednafen\roms\sg", "*.zip", SearchOption.AllDirectories)
            For Each _file In Files
                Dim GameName_Split As String() = _file.Split("\")
                Dim GameName As String = GameName_Split(GameName_Split.Count - 1).Trim.Replace(".zip", "").Replace(",", ".")
                Dim RomName As String = GameName_Split(GameName_Split.Count - 1).Replace(",", ".")
                Dim RomPath As String = _file.Replace(NullDCPath, "")

                If Not GamesList.ContainsKey("SG-" & RomName) Then
                    GamesList.Add("SG-" & RomName, {GameName, RomPath, "sg", ""})
                End If
            Next

        End If

        If _system = "all" Or _system = "psx" Then
            If Not Directory.Exists(NullDCPath & "\mednafen\roms\psx") Then Directory.CreateDirectory(NullDCPath & "\mednafen\roms\psx")
            Files = Directory.GetFiles(NullDCPath & "\mednafen\roms\psx", "*.cue", SearchOption.AllDirectories)
            For Each _file In Files
                Dim GameName_Split As String() = _file.Split("\")
                Dim GameName As String = GameName_Split(GameName_Split.Count - 1).Trim.Replace(".cue", "").Replace(",", ".")
                Dim RomName As String = GameName_Split(GameName_Split.Count - 1).Replace(",", ".")
                Dim RomPath As String = _file.Replace(NullDCPath, "")

                If Not GamesList.ContainsKey("PSX-" & RomName) Then
                    GamesList.Add("PSX-" & RomName, {GameName, RomPath, "psx", ""})
                End If
            Next

        End If

        If _system = "all" Or _system = "nes" Then
            If Not Directory.Exists(NullDCPath & "\mednafen\roms\nes") Then Directory.CreateDirectory(NullDCPath & "\mednafen\roms\nes")
            Files = Directory.GetFiles(NullDCPath & "\mednafen\roms\nes", "*.nes", SearchOption.AllDirectories)
            For Each _file In Files
                Dim GameName_Split As String() = _file.Split("\")
                Dim GameName As String = GameName_Split(GameName_Split.Count - 1).Trim.Replace(".nes", "").Replace(",", ".").Replace(".NES", "").Replace("# NES", "")
                Dim RomName As String = GameName_Split(GameName_Split.Count - 1).Replace(",", ".")
                Dim RomPath As String = _file.Replace(NullDCPath, "")

                If Not GamesList.ContainsKey("NES-" & RomName) Then
                    GamesList.Add("NES-" & RomName, {GameName, RomPath, "nes", ""})
                End If
            Next

        End If

        If _system = "all" Or _system = "snes" Then
            If Not Directory.Exists(NullDCPath & "\mednafen\roms\snes") Then Directory.CreateDirectory(NullDCPath & "\mednafen\roms\snes")
            Files = Directory.GetFiles(NullDCPath & "\mednafen\roms\snes", "*.zip", SearchOption.AllDirectories)
            For Each _file In Files
                Dim GameName_Split As String() = _file.Split("\")
                Dim GameName As String = GameName_Split(GameName_Split.Count - 1).Trim.Replace(".zip", "").Replace(",", ".")
                Dim RomName As String = GameName_Split(GameName_Split.Count - 1).Replace(",", ".")
                Dim RomPath As String = _file.Replace(NullDCPath, "")

                If Not GamesList.ContainsKey("SNES-" & RomName) Then
                    GamesList.Add("SNES-" & RomName, {GameName, RomPath, "snes", ""})
                End If
            Next

        End If

        If _system = "all" Or _system = "fds" Then
            If Not Directory.Exists(NullDCPath & "\mednafen\roms\fds") Then Directory.CreateDirectory(NullDCPath & "\mednafen\roms\fds")
            Files = Directory.GetFiles(NullDCPath & "\mednafen\roms\fds", "*.zip", SearchOption.AllDirectories)
            For Each _file In Files
                Dim GameName_Split As String() = _file.Split("\")
                Dim GameName As String = GameName_Split(GameName_Split.Count - 1).Trim.Replace(".zip", "").Replace(",", ".")
                Dim RomName As String = GameName_Split(GameName_Split.Count - 1).Replace(",", ".")
                Dim RomPath As String = _file.Replace(NullDCPath, "")

                If Not GamesList.ContainsKey("FDS-" & RomName) Then
                    GamesList.Add("FDS-" & RomName, {GameName, RomPath, "fds", ""})
                End If
            Next

        End If

        If _system = "all" Or _system = "ngp" Then
            If Not Directory.Exists(NullDCPath & "\mednafen\roms\ngp") Then Directory.CreateDirectory(NullDCPath & "\mednafen\roms\ngp")
            Files = Directory.GetFiles(NullDCPath & "\mednafen\roms\ngp", "*.zip", SearchOption.AllDirectories)
            For Each _file In Files
                Dim GameName_Split As String() = _file.Split("\")
                Dim GameName As String = GameName_Split(GameName_Split.Count - 1).Trim.Replace(".zip", "").Replace(",", ".")
                Dim RomName As String = GameName_Split(GameName_Split.Count - 1).Replace(",", ".")
                Dim RomPath As String = _file.Replace(NullDCPath, "")

                If Not GamesList.ContainsKey("NGP-" & RomName) Then
                    GamesList.Add("NGP-" & RomName, {GameName, RomPath, "ngp", ""})
                End If
            Next

        End If

        If _system = "all" Or _system = "gba" Then
            If Not Directory.Exists(NullDCPath & "\mednafen\roms\gba") Then Directory.CreateDirectory(NullDCPath & "\mednafen\roms\gba")
            Files = Directory.GetFiles(NullDCPath & "\mednafen\roms\gba", "*.zip", SearchOption.AllDirectories)
            For Each _file In Files
                Dim GameName_Split As String() = _file.Split("\")
                Dim GameName As String = GameName_Split(GameName_Split.Count - 1).Trim.Replace(".zip", "").Replace(",", ".")
                Dim RomName As String = GameName_Split(GameName_Split.Count - 1).Replace(",", ".")
                Dim RomPath As String = _file.Replace(NullDCPath, "")

                If Not GamesList.ContainsKey("GBA-" & RomName) Then
                    GamesList.Add("GBA-" & RomName, {GameName, RomPath, "gba", ""})
                End If
            Next

        End If

        If _system = "all" Or _system = "gbc" Then
            If Not Directory.Exists(NullDCPath & "\mednafen\roms\gbc") Then Directory.CreateDirectory(NullDCPath & "\mednafen\roms\gbc")
            Files = Directory.GetFiles(NullDCPath & "\mednafen\roms\gbc", "*.zip", SearchOption.AllDirectories)
                For Each _file In Files
                    Dim GameName_Split As String() = _file.Split("\")
                    Dim GameName As String = GameName_Split(GameName_Split.Count - 1).Trim.Replace(".zip", "").Replace(",", ".")
                    Dim RomName As String = GameName_Split(GameName_Split.Count - 1).Replace(",", ".")
                    Dim RomPath As String = _file.Replace(NullDCPath, "")

                If Not GamesList.ContainsKey("GBC-" & RomName) Then
                    GamesList.Add("GBC-" & RomName, {GameName, RomPath, "gbc", ""})
                End If
            Next

            End If

        ' New Games List Code
        PopulateGameLists(GameSelectForm.tc_games)

    End Sub

    Private Sub PopulateGameLists(ByRef TabControl As TabControl)
        TabControl.TabPages.Clear()

        'RomFileName - Game Name, Rom Path, Platform, Hash
        For i = 0 To GamesList.Count - 1

            Dim TabIndex = -1
            Dim TabName = ""

            Select Case GamesList(GamesList.Keys(i))(2)
                Case "na"
                    TabName = "Naomi"
                Case "dc"
                    TabName = "Dreamcast"
                Case "sg"
                    TabName = "Genesis"
                Case "ss"
                    TabName = "Saturn"
                Case "psx"
                    TabName = "PSX"
                Case "nes"
                    TabName = "NES"
                Case "snes"
                    TabName = "SNES"
                Case "ngp"
                    TabName = "Neo-Geo Pocket"
                Case "gba"
                    TabName = "GBA"
                Case "gbc"
                    TabName = "GBC"
                Case "fds"
                    TabName = "Famicom Disk System"
                Case Else
                    TabName = "Unknown"
                    Console.WriteLine("No System?")
            End Select

            ' Check if Tab Exists
            For Each _tab As TabPage In TabControl.TabPages
                If _tab.Text = TabName Then
                    TabIndex = _tab.TabIndex
                    Exit For
                End If
            Next

            ' Tab not found, create the tab
            If TabIndex = -1 Then
                TabControl.TabPages.Add(TabName)
                TabControl.TabPages.Item(TabControl.TabCount - 1).BackColor = Color.FromArgb(250, 200, 0)

                Dim tmpListView As New ListView
                tmpListView.Dock = DockStyle.Fill
                tmpListView.MultiSelect = False
                tmpListView.View = View.Details
                tmpListView.HeaderStyle = ColumnHeaderStyle.None
                tmpListView.BackColor = Color.FromArgb(250, 200, 0)
                tmpListView.FullRowSelect = True
                tmpListView.Parent = TabControl.TabPages.Item(TabControl.TabCount - 1)

                tmpListView.Columns.Add("Game Name")
                tmpListView.Columns.Add("Rom Name")
                tmpListView.Columns.Item(0).Width = tmpListView.Width - 25
                tmpListView.Columns.Item(1).Width = 0

                TabIndex = TabControl.TabPages.Count - 1

                AddHandler tmpListView.SelectedIndexChanged, AddressOf MainformRef.GameSelectForm.SelectedIndexChange
                AddHandler tmpListView.Click, AddressOf MainformRef.GameSelectForm.SelectedIndexChange
                AddHandler tmpListView.DoubleClick, AddressOf MainformRef.GameSelectForm.btnLetsGo_Click
            End If

            Dim tmp2ListView = TabControl.TabPages.Item(TabIndex).Controls.OfType(Of ListView).First

            Dim RomName = ""
            Dim GameName = ""

            Dim _it As New ListViewItem(GamesList(GamesList.Keys(i))(0).ToString)
            _it.SubItems.Add(GamesList.Keys(i).ToString)
            tmp2ListView.Items.Add(_it)
        Next



    End Sub

    Private Sub RefreshTimer_tick(sender As Object, e As EventArgs)
        RefreshTimer.Stop()

    End Sub

    Public Sub GameLauncher(ByVal _romname, ByVal _region)
        Dim Emulator As String = GamesList(_romname)(2)

        Select Case Emulator
            Case "dc"
                NullDCLauncher.LaunchDreamcast(_romname, _region)
            Case "na"
                NullDCLauncher.LaunchNaomi(_romname, _region)
            Case "sg", "ss", "nes", "ngp", "snes", "psx", "gba", "gbc", "fds"
                MednafenLauncher.LaunchEmulator(_romname)
            Case Else
                MsgBox("Missing emulator type: " & Emulator)
        End Select

    End Sub

    Private Sub frmMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        ConfigFile.Status = ConfigFile.AwayStatus
        ConfigFile.SaveFile(False)

        NetworkHandler.SendMessage("&")

        niBEAR.Visible = False
        niBEAR.Icon = Nothing

        If IsNullDCRunning() Then NullDCLauncher.NullDCproc.Kill()
        If Not Challenger Is Nothing Then NetworkHandler.SendMessage(">, Q", Challenger.ip)

        If Not MednafenLauncher.MednafenServerInstance Is Nothing Then
            MednafenLauncher.MednafenServerInstance.Kill()
            MednafenLauncher.MednafenServerInstance = Nothing
        End If

        If Not MednafenLauncher.MednafenInstance Is Nothing Then
            MednafenLauncher.MednafenInstance.CloseMainWindow()
            MednafenLauncher.MednafenInstance = Nothing
        End If

        ' Iono if i wanna have this to auto stop Radmin when BEAR closes
        ' Dim processStartInfo As New ProcessStartInfo
        ' ProcessStartInfo.FileName = "cmd.exe"
        ' ProcessStartInfo.Verb = "runas"
        ' ProcessStartInfo.UseShellExecute = True
        ' ProcessStartInfo.CreateNoWindow = True
        ' ProcessStartInfo.Arguments = "/c net stop RvControlSvc"
        ' Process.Start(processStartInfo)

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

    ' Notable Differences for Mednafen
    ' EEPROM = Gamekey
    ' IP =  can be host IP or a public server
    ' Delay = Public or Client or Private (TODO)
    ' Port =  Never used using the default port because that's what all the servers use
    ' Region = USE FOR PASSWORD (?)

    Public Delegate Sub JoinHost_delegate(ByVal _name As String, ByVal _ip As String, ByVal _port As String, ByVal _game As String, ByVal _delay As Int16, ByVal _region As String, ByVal _peripheral As String, ByVal _eeprom As String)
    Public Sub JoinHost(ByVal _name As String, ByVal _ip As String, ByVal _port As String, ByVal _game As String, ByVal _delay As Int16, ByVal _region As String, ByVal _peripheral As String, ByVal _eeprom As String)

        ' Ignore being told to join the host if we havn't gotten the VMU yet
        If MainformRef.GamesList(_game)(2) = "dc" And Rx.VMU Is Nothing Then
            Console.WriteLine("Host started before VMU Data was Recieved, wait")
            Exit Sub
        End If

        Select Case MainformRef.GamesList(_game)(2)
            Case "na"
                Rx.WriteEEPROM(_eeprom, MainformRef.NullDCPath & MainformRef.GamesList(_game)(1))
                ConfigFile.Status = "Client"
                ConfigFile.Port = _port
                ConfigFile.Delay = _delay
            Case "dc"
                ConfigFile.Status = "Client"
                ConfigFile.Port = _port
                ConfigFile.Delay = _delay
            Case Else ' For Mednafen the 'region' setting is used as an indicator if it's public server or not the eeprom setting is used as the gamekey in mednafen
                Select Case _delay
                    Case 0
                        ConfigFile.Status = "Client"
                        Rx.EEPROM = _eeprom
                    Case 1
                        ConfigFile.Status = "Public"
                        Rx.EEPROM = _eeprom
                    Case 2
                        ConfigFile.Status = "Private"
                        Rx.EEPROM = _eeprom
                    Case Else

                End Select
                ' Don't use the nullDC port for Mednafen
                ' ConfigFile.Port = "4046" ' Mednafen always uses this for now maybe i'll change it later but all the public servers use this IP

                If Directory.Exists(NullDCPath & "\mednafen\sav") Then
                    If Directory.Exists(NullDCPath & "\mednafen\sav_") Then
                        Directory.Delete(NullDCPath & "\mednafen\sav", True)
                    Else
                        FileSystem.Rename(NullDCPath & "\mednafen\sav", NullDCPath & "\mednafen\sav_")
                    End If

                End If

                If Directory.Exists(NullDCPath & "\mednafen\mcs") Then
                    If Directory.Exists(NullDCPath & "\mednafen\mcs_") Then
                        Directory.Delete(NullDCPath & "\mednafen\mcs", True)
                    Else
                        FileSystem.Rename(NullDCPath & "\mednafen\mcs", NullDCPath & "\mednafen\mcs_")
                    End If

                End If

        End Select

        If WaitingForm.Visible Then WaitingForm.Visible = False

        Challenger = New NullDCPlayer(_name, _ip, _port, _game,, _peripheral)
        ConfigFile.Host = _ip
        ConfigFile.Game = _game
        ConfigFile.ReplayFile = ""
        ConfigFile.SaveFile()

        GameLauncher(ConfigFile.Game, _region)

    End Sub

    Public Delegate Sub JoinAsSpectator_delegate(ByVal _p1name As String, ByVal _p2name As String, ByVal _ip As String, ByVal _port As String, ByVal _game As String, ByVal _region As String, ByVal _p1p As String, ByVal _p2p As String, ByVal _eeprom As String)
    Public Sub JoinAsSpectator(ByVal _p1name As String, ByVal _p2name As String, ByVal _ip As String, ByVal _port As String, ByVal _game As String, ByVal _region As String, ByVal _p1p As String, ByVal _p2p As String, ByVal _eeprom As String)
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
        NullDCLauncher.P1Peripheral = _p1p
        NullDCLauncher.P2Peripheral = _p2p

        WaitingForm.Visible = False
        ChallengeSentForm.Visible = False

        GameLauncher(ConfigFile.Game, _region)
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

        Matchlist.Invoke(Sub()
                             For Each playerentry As ListViewItem In Matchlist.Items
                                 If playerentry.SubItems(1).Text.Split(":")(0).Trim = Player.ip.Trim Then
                                     FoundEntry = playerentry
                                     Exit For
                                 End If
                                 FoundEntryID += 1
                             Next
                         End Sub)


        If FoundEntry Is Nothing Then

            Dim ItemNumber As Int16 = Matchlist.Items.Count
            Dim PlayerInfo As ListViewItem = New ListViewItem(Player.name, Player.name)
            PlayerInfo.SubItems.Add(Player.ip & ":" & Player.port)
            PlayerInfo.SubItems.Add("T/O")
            PlayerInfo.SubItems.Add(Player.game)
            PlayerInfo.SubItems.Add(Player.status)
            PlayerInfo.BackColor = Color.FromArgb(1, 255, 250, 50)
            PlayerInfo.ForeColor = Color.FromArgb(1, 5, 5, 5)
            Matchlist.Invoke(Sub() Matchlist.Items.Add(PlayerInfo))

            If Not Player.name.StartsWith(MainformRef.ConfigFile.Name) Then
                Try
                    Dim pingthread As Thread = New Thread(
                        Sub()
                            If My.Computer.Network.Ping(Player.ip) Then
                                Thread.Sleep(250 + (250 * ItemNumber))
                                Dim ping As PingReply = New Ping().Send(Player.ip)
                                MainformRef.Matchlist.Invoke(
                                Sub()
                                    Try
                                        MainformRef.Matchlist.Items(ItemNumber).SubItems(2).Text = ping.RoundtripTime
                                    Catch ex As Exception

                                    End Try

                                End Sub)
                            End If
                        End Sub)

                    pingthread.IsBackground = True
                    pingthread.Start()

                Catch ex As Exception

                End Try
            Else
                MainformRef.Invoke(Sub() MainformRef.Matchlist.Items(ItemNumber).SubItems(2).Text = "0")

            End If


        Else
            Matchlist.Invoke(
                Sub()
                    Matchlist.Items(FoundEntryID).SubItems(0).Text = Player.name
                    Matchlist.Items(FoundEntryID).SubItems(1).Text = Player.ip & ":" & Player.port
                End Sub)

            If Not Player.name.StartsWith(MainformRef.ConfigFile.Name) Then
                Try
                    Dim pingthread As Thread = New Thread(
                        Sub()
                            If My.Computer.Network.Ping(Player.ip) Then
                                Thread.Sleep(250 + (250 * FoundEntryID))
                                Dim ping As PingReply = New Ping().Send(Player.ip)
                                MainformRef.Matchlist.Invoke(
                                Sub()
                                    Try
                                        MainformRef.Matchlist.Items(FoundEntryID).SubItems(2).Text = ping.RoundtripTime
                                    Catch ex As Exception

                                    End Try

                                End Sub)
                            End If
                        End Sub)

                    pingthread.IsBackground = True
                    pingthread.Start()

                Catch ex As Exception

                End Try
            Else
                MainformRef.Invoke(Sub() MainformRef.Matchlist.Items(FoundEntryID).SubItems(2).Text = "0")

            End If

            Matchlist.Invoke(
                Sub()
                    Matchlist.Items(FoundEntryID).SubItems(3).Text = Player.game
                    Matchlist.Items(FoundEntryID).SubItems(4).Text = Player.status

                End Sub)
        End If

    End Sub

    Public Delegate Sub BeingChallenged_delegate(ByVal _name As String, ByVal _ip As String, ByVal _port As String, ByVal _game As String, ByVal _hosting As String, ByVal _peripheral As String)
    Public Sub BeingChallenged(ByVal _name As String, ByVal _ip As String, ByVal _port As String, ByVal _game As String, ByVal _hosting As String, ByVal _peripheral As String)
        ChallengeForm.StartChallenge(New NullDCPlayer(_name, _ip, _port, _game,, _peripheral))
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
        Rx.EEPROM = ""
        Rx.VMU = Nothing
        Rx.VMUPieces = New ArrayList From {"", "", "", "", "", "", "", "", "", ""}

        If _canceledby Is Nothing Then
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
                        NullDCLauncher.NullDCproc.Kill()
                        While IsNullDCRunning() ' Just to make sure it's dead before we accept
                            NullDCLauncher.NullDCproc.Kill()
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

            'If IsNullDCRunning() Then NullDCLauncher.NullDCproc.kill() ' Close the NullDC Window

            While IsNullDCRunning()
                'NullDCLauncher.NullDCproc.kill()
                NullDCLauncher.NullDCproc.Kill()
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
                    Message = "Player doesn't have this game or rom names do not match"
                Case "BO"
                    Message = "Challanger got bored of waiting"
                Case "HO"
                    Message = "Player Stopped Hosting, refresh the list plox"
                Case "BH"
                    Message = "Host hasn't started a game yet"
                Case "BW"
                    Message = "Player is connecting to someone else"
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
                Case "NDC"
                    Message = "Cannot Spectate Offline Dreamcast game (Yet)"
                Case "MDH"
                    Message = "Try connecting to the HOST"
                Case "MDN"
                    Message = "Player is playing offline"
                Case "MDP"
                    Message = "You cannot join Private Games"
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

    Private Sub pnlTopBorder_MouseDown(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseDown, BEARTitle.MouseDown, imgBeta.MouseDown, lbVer.MouseDown, TableLayoutPanel4.MouseDown
        xpos1 = Control.MousePosition.X - Me.Location.X
        ypos1 = Control.MousePosition.Y - Me.Location.Y
    End Sub

    Private Sub pnlTopBorder_MouseMove(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseMove, BEARTitle.MouseMove, imgBeta.MouseMove, lbVer.MouseMove, TableLayoutPanel4.MouseMove
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
        If Not Application.OpenForms().OfType(Of frmKeyMapperSDL).Any Then
            frmKeyMapperSDL.Show(Me)
        End If
    End Sub

    Private Sub btnOffline_Click(sender As Object, e As EventArgs) Handles btnOffline.Click
        If MainformRef.IsNullDCRunning Or Not MainformRef.MednafenLauncher.MednafenInstance Is Nothing Then
            NotificationForm.ShowMessage("You're already playing...")
            Exit Sub
        End If

        If GamesList.Count = 0 Then
            NotificationForm.ShowMessage("You don't have any games, click the freeDLC to get some.")
            Exit Sub
        End If
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
            NetworkHandler.SendMessage("?,")

            Dim NameToSend As String = MainformRef.ConfigFile.Name
            If Not MainformRef.Challenger Is Nothing Then NameToSend = NameToSend & " Vs " & MainformRef.Challenger.name

            Dim GameNameAndRomName = "None"
            If Not MainformRef.ConfigFile.Game = "None" Then GameNameAndRomName = MainformRef.GamesList(MainformRef.ConfigFile.Game)(0) & "|" & MainformRef.ConfigFile.Game
            NetworkHandler.SendMessage("<," & NameToSend & ",," & MainformRef.ConfigFile.Port & "," & GameNameAndRomName & "," & MainformRef.ConfigFile.Status)
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

        If Matchlist.SelectedItems(0).SubItems(0).Text.Trim = ConfigFile.Name.Trim Then
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
        If Not c_status = "Idle" And Not c_status = "LFG" Then ' this person is playing SOMETHING so lets try to challange them and see what they reply

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

            ' Don't instantly spectate a DC game, get the VMU first
            If Not MainformRef.GamesList(MainformRef.Challenger.game)(2) = "dc" Then
                NetworkHandler.SendMessage("!," & ConfigFile.Name & ",," & ConfigFile.Port & "," & Challenger.game & ",0," & ConfigFile.Peripheral, Challenger.ip)
            End If

            WaitingForm.Show()
        Else
            GameSelectForm.StartChallenge(New NullDCPlayer(c_name, c_ip, c_port)) ' If they not hosting then just start choosing game to challenge them to
        End If

    End Sub

    Private Sub btnHost_Click(sender As Object, e As EventArgs)
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

            If Not IsNullDCRunning() And MednafenLauncher.MednafenInstance Is Nothing Then
                ConfigFile.Status = cbStatus.Text
                ConfigFile.SaveFile()
            End If

        End If
        ActiveControl = Nothing
    End Sub

    Private Sub PictureBox1_MouseClick(sender As Object, e As MouseEventArgs) Handles sus_i.MouseClick
        Process.Start("http://www.innersloth.com/gameAmongUs.php")
    End Sub

    Private Sub btnDiscord_Click(sender As Object, e As EventArgs) Handles btnDiscord.Click
        Process.Start("https://discord.gg/u2YzdNB6SN")
    End Sub

End Class

Public Class NullDCPlayer
    Public name As String
    Public ip As String
    Public port As String
    Public status As String
    Public game As String
    Public peripheral As String

    Public Sub New(ByVal _name As String, ByVal _ip As String, ByVal _port As String, Optional ByVal _game As String = "None", Optional ByVal _status As String = "Idle", Optional ByVal _peripheral As String = "0")
        name = _name
        ip = _ip
        port = _port
        status = _status
        game = _game
        peripheral = _peripheral
    End Sub

End Class

Public Class Configs

    Private _name = "Player"
    Private _network = "Radmin VPN"
    Private _port = "27886"
    Private _useremapper As Boolean = True
    Private _firstrun As Boolean = True
    Private _host As String = "127.0.0.1"
    Private _status As String = "Idle"
    Private _delay As Int16 = 1
    Private _game As String = "None"
    Private _ver As String = frmMain.Ver
    Private _keyprofile As String = "Default"
    Private _recordreplay As Int16 = 0
    Private _ReplayFile As String = ""
    Private _allowSpectators As Int16 = 1
    Private _awaystatus As String = "Idle"
    Private _volume As Int16 = 100
    Private _showconsole As Int16 = 1
    Private _evolume As Int16 = 100
    Private _peripheral As Int16 = 0
    Private _windowsettings As String = "0|200|200|656|538"
    Private _vsnames As Int16 = 3
    Private _ShowGameNameInTitle As Int16 = 1
    Private _sdlversopm As String = "Dev"
    Private _vsync As Int16 = 0

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

    Public Property ShowConsole() As Int16
        Get
            Return _showconsole
        End Get
        Set(ByVal value As Int16)
            _showconsole = value
        End Set

    End Property

    Public Property EmulatorVolume() As Int16
        Get
            Return _evolume
        End Get
        Set(ByVal value As Int16)
            _evolume = value
        End Set
    End Property

    Public Property Peripheral() As Int16
        Get
            Return _peripheral
        End Get
        Set(ByVal value As Int16)
            _peripheral = value
        End Set

    End Property

    Public Property WindowSettings() As String
        Get
            Return _windowsettings
        End Get
        Set(ByVal value As String)
            _windowsettings = value
        End Set
    End Property

    Public Property VsNames() As Int16
        Get
            Return _vsnames
        End Get
        Set(ByVal value As Int16)
            _vsnames = value
        End Set

    End Property

    Public Property ShowGameNameInTitle() As Int16
        Get
            Return _ShowGameNameInTitle
        End Get
        Set(ByVal value As Int16)
            _ShowGameNameInTitle = value
        End Set

    End Property

    Public Property SDLVersion() As String
        Get
            Return _sdlversopm
        End Get

        Set(ByVal value As String)
            _sdlversopm = value
        End Set

    End Property

    Public Property Vsync() As Int16
        Get
            Return _vsync
        End Get
        Set(ByVal value As Int16)
            _vsync = value
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
                "Host=" & Host,
                "Status=" & Status,
                "Delay=" & Delay,
                "Game=" & Game,
                "KeyProfile=" & KeyMapProfile,
                "RecordReplay=" & RecordReplay,
                "ReplayFile=" & ReplayFile,
                "AllowSpectators=" & AllowSpectators,
                "AwayStatus=" & AwayStatus,
                "Volume=" & Volume,
                "eVolume=" & EmulatorVolume,
                "ShowConsole=" & ShowConsole,
                "Peripheral=" & Peripheral,
                "WindowSettings=" & WindowSettings,
                "VsNames=" & VsNames,
                "ShowGameNameInTitle=" & ShowGameNameInTitle,
                "SDLVersion=" & SDLVersion,
                "Vsync=" & Vsync
            }
        File.WriteAllLines(NullDCPath & "\NullDC_BEAR.cfg", lines)

        If Not MainformRef.NetworkHandler Is Nothing Then
            If AwayStatus = "Hidden" Then
                MainformRef.NetworkHandler.SendMessage("&")
            Else
                If SendIam Then
                    Dim GameNameAndRomName = "None"
                    If Not Game = "None" Then GameNameAndRomName = MainformRef.GamesList(MainformRef.ConfigFile.Game)(0).ToString.Replace(",", ".") & "|" & MainformRef.ConfigFile.Game

                    Dim NameToSend As String = Name
                    If Not MainformRef.Challenger Is Nothing Then NameToSend = Name & " Vs " & MainformRef.Challenger.name

                    ' Delayed Task to hopefully remove some crashes related to this being send along with other message too quickly
                    Dim t As Task = New Task(Async Sub()
                                                 ' 2 Second Delay just to avoid crashing some rare PCs that don't like to send data too quickly
                                                 Await Task.Delay(2000)
                                                 MainformRef.NetworkHandler.SendMessage("<," & NameToSend & ",," & Port & "," & GameNameAndRomName & "," & Status)
                                             End Sub)
                    t.Start()
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
            Dim tmpVersion = ""
            For Each line As String In lines
                If line.StartsWith("Version") Then tmpVersion = line.Split("=")(1).Trim
                If line.StartsWith("Name") Then Name = line.Split("=")(1).Trim
                If line.StartsWith("Network") Then Network = line.Split("=")(1).Trim
                If line.StartsWith("Port") Then Port = line.Split("=")(1).Trim
                If line.StartsWith("Reclaw") Then UseRemap = line.Split("=")(1).Trim
                If line.StartsWith("Host") Then Host = line.Split("=")(1).Trim
                If line.StartsWith("Delay") Then Delay = line.Split("=")(1).Trim
                If line.StartsWith("KeyProfile") Then KeyMapProfile = line.Split("=")(1).Trim
                If line.StartsWith("RecordReplay") Then RecordReplay = line.Split("=")(1).Trim
                If line.StartsWith("ReplayFile") Then ReplayFile = line.Split("=")(1).Trim
                If line.StartsWith("AllowSpectators") Then AllowSpectators = line.Split("=")(1).Trim
                If line.StartsWith("AwayStatus") Then
                    AwayStatus = line.Split("=")(1).Trim
                    Status = line.Split("=")(1).Trim
                End If

                If line.StartsWith("Volume") Then
                    Volume = line.Split("=")(1).Trim
                    EmulatorVolume = line.Split("=")(1).Trim
                End If

                If line.StartsWith("eVolume") Then EmulatorVolume = line.Split("=")(1).Trim
                If line.StartsWith("ShowConsole") Then ShowConsole = line.Split("=")(1).Trim
                If line.StartsWith("Peripheral") Then Peripheral = line.Split("=")(1).Trim
                If line.StartsWith("WindowSettings") Then WindowSettings = line.Split("=")(1).Trim
                If line.StartsWith("VsNames") Then VsNames = line.Split("=")(1).Trim
                If line.StartsWith("ShowGameNameInTitle") Then ShowGameNameInTitle = line.Split("=")(1).Trim
                If line.StartsWith("SDLVersion") Then SDLVersion = line.Split("=")(1).Trim
                If line.StartsWith("Vsync") Then Vsync = line.Split("=")(1).Trim
            Next

            Game = "None"
            SaveFile()
            Exit Sub
        End If
    End Sub

End Class
