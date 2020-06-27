Imports System.IO
Imports System.Net
Imports System.Threading

Public Class frmMain
    Dim IsBeta As Boolean = True

    ' Update Stuff
    Dim UpdateCheckClient As New WebClient

    Public Ver As String = "0.98"
    Public InputHandler As InputHandling
    Public NetworkHandler As NetworkHandling
    Public NullDCLauncher As NullDCLauncher
    Public NullDCPath As String = Application.StartupPath
    Public GamesList As New Dictionary(Of String, Array)
    Public ConfigFile As Configs
    Public FirstRun As Boolean = True

    Public ChallengeForm As frmChallenge = New frmChallenge(Me)
    Public ChallengeSentForm As frmChallengeSent = New frmChallengeSent(Me)
    Public HostingForm As frmHostPanel = New frmHostPanel(Me)
    Public GameSelectForm As frmChallengeGameSelect = New frmChallengeGameSelect(Me)
    Public WaitingForm As frmWaitingForHost = New frmWaitingForHost
    Public NotificationForm As frmNotification = New frmNotification
    Public KeyMappingForm As frmKeyMapping

#Region "Beta"
    ' Fuck the replay shit for now
    ' Public NetplayHandler As NetPlayHandler

#End Region



    Public Challenger As NullDCPlayer
    Private RefreshTimer As System.Windows.Forms.Timer = New System.Windows.Forms.Timer

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.NewNullDCBearIcon
        lbVer.Text = Ver
        If Debugger.IsAttached Then NullDCPath = "D:\Games\Emulators\NullDC\nulldc-1-0-4-en-win"

        If Not File.Exists(NullDCPath & "\nullDC_Win32_Release-NoTrace.exe") Then
            Dim result As DialogResult = MessageBox.Show("NullDC was not found in this folder, INSTALL NULLDC INTO THIS FOLDER?", "NullDC Install", MessageBoxButtons.YesNo)
            If result = DialogResult.Yes Then
                Dim result2 As DialogResult = MessageBox.Show("This will create a bunch of files in the same folder as NullDC BEAR.exe, OK?", "NullDC Extraction", MessageBoxButtons.YesNo)
                If result2 = DialogResult.Yes Then
                    UnzipResToDir(My.Resources.nulldcCLEAN, "bear_tmp_nulldc.zip", NullDCPath)
                    MsgBox("NullDC Has been Extracted here, put some games in the ROMS folder then start BEAR up again")
                    End
                End If
            End If
            MsgBox("I need to be in the NullDC folder where nullDC_Win32_Release-NoTrace.exe")
            End
        End If

        CheckFilesAndShit()
        ConfigFile = New Configs(NullDCPath)

        ' Update Stuff
        AddHandler UpdateCheckClient.DownloadStringCompleted, AddressOf UpdateCheckResult
        CheckForUpdate()

        ' Create all the usual shit
        InputHandler = New InputHandling(Me)
        KeyMappingForm = New frmKeyMapping(Me)
        NetworkHandler = New NetworkHandling(Me)
        NullDCLauncher = New NullDCLauncher(Me)

        If ConfigFile.FirstRun Then frmSetup.ShowDialog()
        AddHandler RefreshTimer.Tick, AddressOf RefreshTimer_tick


        ' Beta Features Only
        If IsBeta Then
            ' NetPlayHandler = New NetPlayHandler(Me)

        End If


    End Sub

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
            End If
        Next

        If LatestVersion = Ver Then
            ' Is up to date
            Console.WriteLine("Up To Date, Delete updater if it exists")
            If File.Exists(Application.StartupPath & "\NullDC-BEAR-UPDATER.exe") Then File.Delete(Application.StartupPath & "\NullDC-BEAR-UPDATER.exe")
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

    Private Sub UnzipResToDir(ByVal _res As Byte(), ByVal _name As String, ByVal _dir As String)
        Dim FileWriteThread As Thread
        FileWriteThread = New Thread(AddressOf CopyResourceToDirectoryThread)
        FileWriteThread.IsBackground = True
        FileWriteThread.Start({_res, _name})

        While FileWriteThread.IsAlive
            Thread.Sleep(100)
        End While

        Using archive As ZipArchive = ZipFile.OpenRead(My.Computer.FileSystem.SpecialDirectories.Temp & "\" & _name)
            For Each entry As ZipArchiveEntry In archive.Entries
                If entry.FullName.Split("\").Length > 1 Then
                    Directory.CreateDirectory(_dir & "\" & entry.FullName.Split("\")(0))
                End If
                entry.ExtractToFile(_dir & "\" & entry.FullName, True)
            Next
        End Using

    End Sub

    Private Sub CheckFilesAndShit()

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

        ' Make the Keyboard File if it doens't exist, it should but i mean you never know
        If Not File.Exists(NullDCPath & "\qkoJAMMA\Keyboard.qkc") Then
            File.WriteAllLines(NullDCPath & "\qkoJAMMA\Keyboard.qkc", {"5=Start", "3=Test", "w=Up", "s=Down", "a=Left", "d=Right", "8=Button_1", "9=Button_2", "0=Button_3", "u=Button_4", "i=Button_5", "o=Button_6", "1=Coin"})
        End If

        'Install Dependencies if needed
        If Not File.Exists(NullDCPath & "\XInputInterface.dll") Or
            Not File.Exists(NullDCPath & "\XInputDotNetPure.dll") Or
            Not File.Exists(NullDCPath & "\OpenTK.dll") Then
            UnzipResToDir(My.Resources.Deps, "bear_tmp_deps.zip", NullDCPath)
        End If

        ' FPS Limited Doesn't Exist lets Create it
        If Not File.Exists(NullDCPath & "\d3d9.dll") Then File.WriteAllBytes(NullDCPath & "\d3d9.dll", My.Resources.d3d9)
        File.WriteAllLines(NullDCPath & "\antilag.cfg", {"[config]", "RenderAheadLimit=0", "FPSlimit=60"})

        GetGamesList()
    End Sub

    Private Sub GetGamesList()
        GameSelectForm.cbGameList.Items.Clear()
        HostingForm.cbGameList.Items.Clear()
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

        If GamesList.Count = 0 Then
            MsgBox("No Games Found in the /roms folder. Please put some games in there then start BEAR")
            End
        End If

        GameSelectForm.cbGameList.DataSource = table
        HostingForm.cbGameList.DataSource = table

        GameSelectForm.cbGameList.SelectedIndex = 0
        HostingForm.cbGameList.SelectedIndex = 0

    End Sub

    Private Sub RefreshTimer_tick(sender As Object, e As EventArgs)
        RefreshTimer.Stop()

    End Sub

    Private Sub frmMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        ConfigFile.SaveFile()
        If IsNullDCRunning() Then
            NullDCLauncher.NullDCproc.CloseMainWindow()
        End If

        If Not Challenger Is Nothing Then
            NetworkHandler.SendMessage(">, Q", Challenger.ip)
        End If
        Application.Exit()

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

    Public Delegate Sub JoinHost_delegate(ByVal _name As String, ByVal _ip As String, ByVal _port As String, ByVal _game As String, ByVal _delay As Int16)
    Public Sub JoinHost(ByVal _name As String, ByVal _ip As String, ByVal _port As String, ByVal _game As String, ByVal _delay As Int16)
        If WaitingForm.Visible Then WaitingForm.Visible = False
        Challenger = New NullDCPlayer(_name, _ip, _port, _game)
        ConfigFile.Host = _ip
        ConfigFile.Port = _port
        ConfigFile.Status = "Client"
        ConfigFile.Game = _game
        ConfigFile.Delay = _delay
        ConfigFile.SaveFile()
        NullDCLauncher.LaunchDC(ConfigFile.Game)

    End Sub

    Public Delegate Sub AddPlayerToList_delegate(ByVal Player As NullDCPlayer, ByVal ping As String)
    Public Sub AddPlayerToList(ByVal Player As NullDCPlayer, ByVal ping As String)
        Dim PlayerInfo As ListViewItem = New ListViewItem(Player.name, Player.name)
        PlayerInfo.SubItems.Add(Player.ip & ":" & Player.port)
        PlayerInfo.SubItems.Add(ping)
        PlayerInfo.SubItems.Add(Player.game)
        PlayerInfo.SubItems.Add(Player.status)
        PlayerInfo.BackColor = Color.FromArgb(1, 255, 250, 50)
        PlayerInfo.ForeColor = Color.FromArgb(1, 5, 5, 5)
        Matchlist.Items.Add(PlayerInfo)

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
                    Console.Write("Window Closed")
                    RemoveChallenger() ' Remove Challenger Data in case we have any
                    ' Set game to none and back to idle
                    ConfigFile.Game = "None"
                    ConfigFile.Status = "Idle"
                    ConfigFile.SaveFile()
                Case "New Challenger" ' This is fired when you accept to fight a new challanger
                    Console.Write("New Challanger")
                    If IsNullDCRunning() Then
                        NullDCLauncher.DoNotSendNextExitEvent = True
                        NullDCLauncher.NullDCproc.CloseMainWindow()
                    End If
                    ' Set out game to w.e our challenger wants to play and set outself to client
                    ConfigFile.Game = Challenger.game
                    ConfigFile.Status = "Client"
                    ConfigFile.SaveFile()
                Case "Denied", "TO" ' T/O or Denied
                    RemoveChallenger()
                Case "Host Canceled" ' You were hosting but then you stopped Clear the game info
                    RemoveChallenger()
                    ConfigFile.Game = "None"
                    ConfigFile.Status = "Idle"
                    ConfigFile.SaveFile()
                Case "New Host" ' You started a new host Just close the nullDC if it's open, disable raising it's event
                    If IsNullDCRunning() Then
                        NullDCLauncher.NullDCproc.EnableRaisingEvents = False ' Disable Event Raising so this doesn't automaticlly clear challenger data
                        NullDCLauncher.NullDCproc.CloseMainWindow()
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
            ConfigFile.Game = "None"
            ConfigFile.Status = "Idle"
            ConfigFile.SaveFile()

            If IsNullDCRunning() Then NullDCLauncher.NullDCproc.CloseMainWindow() ' Close the NullDC Window

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
                    Message = "Player does not have this game"
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
            End Select

            If Not Message = "" Then NotificationForm.ShowMessage(Message)

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

    Private Sub pnlTopBorder_MouseDown(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseDown
        xpos1 = Control.MousePosition.X - Me.Location.X
        ypos1 = Control.MousePosition.Y - Me.Location.Y
    End Sub

    Private Sub pnlTopBorder_MouseMove(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseMove
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
        KeyMappingForm.Show()

    End Sub

    Private Sub btnOffline_Click(sender As Object, e As EventArgs) Handles btnOffline.Click
        GameSelectForm.StartChallenge()

    End Sub

    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click

        If Not RefreshTimer.Enabled Then
            Matchlist.Items.Clear()
            NetworkHandler.SendMessage("?," & ConfigFile.IP)
            RefreshTimer.Interval = 5000
            RefreshTimer.Start()
        Else
            NotificationForm.ShowMessage("Slow down cowboy, wait at least 5 seconds between refreshing")
        End If

    End Sub

    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        Me.Close()

    End Sub

    Private Sub btnMinimize_Click(sender As Object, e As EventArgs) Handles btnMinimize.Click
        Me.WindowState() = FormWindowState.Minimized

    End Sub

    Private Sub btnSetup_Click(sender As Object, e As EventArgs) Handles btnSetup.Click
        frmSetup.Show()

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

        ' Skip game Selection
        If c_status = "Hosting" And Not c_gamerom = "None" Then ' Person is hosting a game, so try to join them
            ' Check if you have the game
            If Not GamesList.ContainsKey(c_gamerom) Then
                MsgBox(c_gamerom)
                NotificationForm.ShowMessage("You don't have this game")
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

#End Region

    Public Sub SaveFile()
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
                "KeyProfile=" & KeyMapProfile
            }
        File.WriteAllLines(NullDCPath & "\NullDC_BEAR.cfg", lines)
    End Sub

    Public Sub New(ByRef NullDCPath As String)

        'Check Config FIle
        If Not File.Exists(NullDCPath & "\NullDC_BEAR.cfg") Then
            FirstRun = True
            ' Save That Shit
            SaveFile()
        Else
            FirstRun = False
            ' Not first Run but has different version Configs
            Dim thefile = NullDCPath & "\NullDC_BEAR.cfg"
            Dim lines() As String = File.ReadAllLines(thefile)

            If Not lines(1).Split("=")(1).Trim = frmMain.Ver Then
                ' This is when they already have a version of the Config file so lets try to get as much info from it as we can before we create a proper one
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

                Next
                Status = "Idle"
                Game = "None"
                SaveFile()
                Exit Sub

            Else
                Version = lines(1).Split("=")(1).Trim
                Name = lines(2).Split("=")(1).Trim
                Network = lines(3).Split("=")(1).Trim
                Port = lines(4).Split("=")(1).Trim
                UseRemap = lines(5).Split("=")(1).Trim
                IP = lines(6).Split("=")(1).Trim
                Host = lines(7).Split("=")(1).Trim
                Status = "Idle"
                Delay = lines(9).Split("=")(1).Trim
                Game = "None"
                HostType = lines(11).Split("=")(1).Trim
                FPSLimit = lines(12).Split("=")(1).Trim
                KeyMapProfile = lines(13).Split("=")(1).Trim

            End If


        End If

    End Sub

End Class
