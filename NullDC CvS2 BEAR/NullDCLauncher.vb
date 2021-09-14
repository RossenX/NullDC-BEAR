Imports System.IO
Imports System.Runtime.InteropServices
Imports NullDC_CvS2_BEAR.frmMain
Imports System.Threading
Imports System.Globalization
Imports System.Text

Public Class NullDCLauncher

    Public Region As String
    Public NullDCproc As Process
    Public DoNotSendNextExitEvent As Boolean
    Public P1Name As String = ""
    Public P2Name As String = ""
    Public P1Peripheral As String = ""
    Public P2Peripheral As String = ""

    Dim AutoHotkey As Process
    Dim SingleInstance As Boolean = True
    Dim LoadRomThread As Thread
    Dim IsFullscreen = 0


#Region "API"

    <DllImport("user32.dll")>
    Private Shared Function GetForegroundWindow() As IntPtr
    End Function

    <DllImport("user32.dll")>
    Public Shared Function GetWindowThreadProcessId(ByVal hWnd As IntPtr, <Out()> ByRef lpdwProcessId As UInteger) As UInteger
    End Function

    <DllImport("user32.dll")>
    Public Shared Function MoveWindow(ByVal hWnd As IntPtr, ByVal x As Integer, ByVal y As Integer, ByVal nWidth As Integer, ByVal nHeight As Integer, ByVal bRepaint As Boolean) As Boolean
    End Function

    <DllImport("user32.dll")>
    Private Shared Function GetWindowRect(ByVal hWnd As IntPtr, ByRef lpRect As Rectangle) As Boolean
    End Function

#End Region

    Public Function IsNullDCWindowSelected() As Boolean
        If MainformRef.IsNullDCRunning Then
            Dim hWnd As IntPtr = GetForegroundWindow()
            Dim ProcessID As UInteger = 0
            GetWindowThreadProcessId(hWnd, ProcessID)
            If ProcessID = NullDCproc.Id Then Return True

        End If

        Return False

    End Function

    Public Sub LoadGame()
        If Not LoadRomThread Is Nothing Then If LoadRomThread.IsAlive Then LoadRomThread.Abort() ' Abort the old thread if it exists
        LoadRomThread = New Thread(AddressOf LoadGame_Thread)
        LoadRomThread.IsBackground = True
        LoadRomThread.Start()

    End Sub

    Private Sub LoadGame_Thread()
        Try
            While Not MainformRef.IsNullDCRunning
                Thread.Sleep(5)
            End While

            NullDCproc.WaitForInputIdle()
            If IsFullscreen = 0 Then
                Dim UseCustomWindowSettings = CBool(MainformRef.ConfigFile.WindowSettings.Split("|")(0))
                Dim x = CInt(MainformRef.ConfigFile.WindowSettings.Split("|")(1))
                Dim y = CInt(MainformRef.ConfigFile.WindowSettings.Split("|")(2))
                Dim width = CInt(MainformRef.ConfigFile.WindowSettings.Split("|")(3))
                Dim height = CInt(MainformRef.ConfigFile.WindowSettings.Split("|")(4))

                If UseCustomWindowSettings Then
                    MoveWindow(NullDCproc.MainWindowHandle, x, y, width, height, True)
                End If

            End If

            GameLoaded()

        Catch ex As Exception
            MsgBox("Romloader error: " & ex.Message)

        End Try

    End Sub

    Public Sub LaunchDreamcast(ByVal _romname As String, ByRef _region As String)
        'MainformRef.InputHandler.GetKeyboardConfigs("dc")
        'MainformRef.InputHandler.NeedConfigReload = True
        Region = _region

        If MainformRef.IsNullDCRunning And SingleInstance Then
            MainformRef.NotificationForm.ShowMessage("An Instance of NullDC online is already running.")
            Exit Sub
        Else
            StartDreamcastEmulator(_romname)
        End If

    End Sub

    Public Sub LaunchNaomi(ByVal _romname As String, ByRef _region As String)
        'MainformRef.InputHandler.GetKeyboardConfigs("na")
        'MainformRef.InputHandler.NeedConfigReload = True
        Region = _region

        If MainformRef.IsNullDCRunning And SingleInstance Then
            MainformRef.NotificationForm.ShowMessage("An Instance of NullDC online is already running.")
            Exit Sub
        Else
            StartNaomiEmulator(_romname)
        End If

    End Sub

    Private Sub EmulatorExited()
        Console.WriteLine("Emulator Exited")

        If Not DoNotSendNextExitEvent Then
            If Not MainformRef.Challenger Is Nothing And (MainformRef.ConfigFile.Status = "Hosting" Or MainformRef.ConfigFile.Status = "Client") Then
                Console.WriteLine("Telling challenger i quit")
                MainformRef.NetworkHandler.SendMessage(">,E", MainformRef.Challenger.ip)
            End If

            If Not MainformRef.IsClosing Then
                Dim INVOKATION As EndSession_delegate = AddressOf MainformRef.EndSession
                MainformRef.Invoke(INVOKATION, {"Window Closed", Nothing})
            End If

        Else
            ' Set State Back to None and Idle since Emulator Closed
            MainformRef.RemoveChallenger()
            MainformRef.ConfigFile.Game = "None"
            MainformRef.ConfigFile.Status = MainformRef.ConfigFile.AwayStatus
            MainformRef.ConfigFile.SaveFile()
        End If

        DoNotSendNextExitEvent = False
        P1Name = ""
        P2Name = ""
        P1Peripheral = ""
        P2Peripheral = ""
        Rx.EEPROM = ""
        Rx.VMU = Nothing
        Rx.VMUPieces = New ArrayList From {"", "", "", "", "", "", "", "", "", ""}

        NullDCproc = Nothing
        'MainformRef.InputHandler.GetKeyboardConfigs(Platform)
        'MainformRef.InputHandler.NeedConfigReload = True
    End Sub

    Private Sub StartDreamcastEmulator(ByVal _romname As String)
        ChangeSettings_Dreamcast()

        ' If we hosting save the VMU for sending to spectators
        If MainformRef.ConfigFile.Status = "Hosting" Or MainformRef.ConfigFile.Status = "Offline" Then
            Rx.VMU = Rx.ReadVMU()
        End If

        Dim NullDCInfo As New ProcessStartInfo
        NullDCInfo.FileName = MainformRef.NullDCPath & "\dc\nullDC_Win32_Release-NoTrace.exe"
        If MainformRef.ConfigFile.ShowConsole = 0 Then NullDCInfo.Arguments = "-NoConsole"
        NullDCproc = Process.Start(NullDCInfo)

        Select Case MainformRef.ConfigFile.NullDCPriority
            Case 0
                NullDCproc.PriorityClass = ProcessPriorityClass.Normal
            Case 1
                NullDCproc.PriorityClass = ProcessPriorityClass.AboveNormal
            Case 2
                NullDCproc.PriorityClass = ProcessPriorityClass.High
            Case 3
                NullDCproc.PriorityClass = ProcessPriorityClass.RealTime
        End Select
        NullDCproc.PriorityBoostEnabled = True
        NullDCproc.EnableRaisingEvents = True
        AddHandler NullDCproc.Exited, AddressOf EmulatorExited
        LoadGame()

    End Sub

    Private Sub StartNaomiEmulator(ByVal _romname As String)
        ChangeSettings_Naomi()

        ' If we hosting save the EEPROM for sending to spectators
        Rx.EEPROM = Rx.GetEEPROM(MainformRef.NullDCPath & MainformRef.GamesList(_romname)(1))

        Dim NullDCInfo As New ProcessStartInfo
        NullDCInfo.FileName = MainformRef.NullDCPath & "\nullDC_Win32_Release-NoTrace.exe"
        If MainformRef.ConfigFile.ShowConsole = 0 Then NullDCInfo.Arguments = "-NoConsole"
        NullDCproc = Process.Start(NullDCInfo)

        Select Case MainformRef.ConfigFile.NullDCPriority
            Case 0
                NullDCproc.PriorityClass = ProcessPriorityClass.Normal
            Case 1
                NullDCproc.PriorityClass = ProcessPriorityClass.AboveNormal
            Case 2
                NullDCproc.PriorityClass = ProcessPriorityClass.High
            Case 3
                NullDCproc.PriorityClass = ProcessPriorityClass.RealTime
        End Select
        NullDCproc.PriorityBoostEnabled = True
        NullDCproc.EnableRaisingEvents = True
        AddHandler NullDCproc.Exited, AddressOf EmulatorExited
        LoadGame()

    End Sub

    Private Shared Sub OutputHandler(sendingProcess As Object, outLine As DataReceivedEventArgs)
        If Not String.IsNullOrEmpty(outLine.Data) Then
            Console.WriteLine(outLine.Data)
        End If
    End Sub

    Private Sub GameLoaded()
        ' If we're a host then send out call to my partner to join
        Console.WriteLine("Game Launched")
        If MainformRef.ConfigFile.Status = "Hosting" And Not MainformRef.Challenger Is Nothing Then

            If MainformRef.ConfigFile.Game.Split("-")(0).ToLower = "dc" Then
                Rx.EEPROM = ""
            End If

            MainformRef.NetworkHandler.SendMessage("$," & MainformRef.ConfigFile.Name & ",," & MainformRef.ConfigFile.Port & "," & MainformRef.ConfigFile.Game & "," & MainformRef.ConfigFile.Delay & "," & Region & "," & MainformRef.ConfigFile.Peripheral & ",eeprom," & Rx.EEPROM, MainformRef.Challenger.ip)

        End If

    End Sub

    Private Sub DeleteNVMEM()
        ' Fuck nvMEM get rid of that shit
        Dim nvmemPath = MainformRef.NullDCPath & "\data\naomi_nvmem.bin"

        Try
            If File.Exists(nvmemPath) Then
                File.SetAttributes(nvmemPath, FileAttributes.Normal)
                File.Delete(nvmemPath)
            Else
                Console.WriteLine("No nvmem, we all good")
            End If
        Catch ex As Exception
            MsgBox("Couldn't backup nvmem: " & ex.Message)
        End Try

    End Sub

    Private Sub DealWithBios()

        ' Make sure there's SOME kinda bios in there
        Dim naomi_bios_path As String = MainformRef.NullDCPath & "\data\naomi_bios.bin"
        If Not File.Exists(naomi_bios_path) Then MainformRef.UnzipResToDir(My.Resources.bj, "naomi_bios.bin", MainformRef.NullDCPath & "\data")

        ' Now that we've taken care of BM's launcher bios stuff, lets check our own bios and what we need to do
        If Not Region = "JPN" Then ' Only check if it's NOT the JPN bios because those are the ones that use naomi_boot
            Dim BiosToUse = My.Resources.bu
            If Region = "EUR" Then BiosToUse = My.Resources.be
            MainformRef.UnzipResToDir(BiosToUse, "naomi_boot.bin", MainformRef.NullDCPath & "\data")

        Else
            If File.Exists(MainformRef.NullDCPath & "\data\naomi_boot.bin") Then
                File.SetAttributes(MainformRef.NullDCPath & "\data\naomi_boot.bin", FileAttributes.Normal)
                File.Delete(MainformRef.NullDCPath & "\data\naomi_boot.bin")
            End If
        End If

    End Sub

    Private Sub ChangeSettings_Dreamcast()
        Dim linenumber = 0

        ' Controls File
        Dim ControlsFile() As String = File.ReadAllLines(GetControlsFilePath)
        ' Get Rid of the settings we don't want

        Dim tempPeripheral As String() = {"", ""}
        Dim tempJoystick As String() = {"", ""}
        Dim TempDeadzone As String() = {"", ""}

        For Each line As String In ControlsFile
            If line.StartsWith("Peripheral=") Then
                tempPeripheral(0) = line.Split("=")(1).Split("|")(0)
                tempPeripheral(1) = line.Split("=")(1).Split("|")(1)
            End If
            If line.StartsWith("Joystick=") Then
                tempJoystick(0) = line.Split("=")(1).Split("|")(0)
                tempJoystick(1) = line.Split("=")(1).Split("|")(1)
            End If
            If line.StartsWith("Deadzone=") Then
                TempDeadzone(0) = line.Split("=")(1).Split("|")(0)
                TempDeadzone(1) = line.Split("=")(1).Split("|")(1)
            End If
        Next

        ' Put in the VMU to keep it in sync for now
        If Not File.Exists(MainformRef.NullDCPath & "\dc\vmu_data_host.bin") Then
            My.Computer.FileSystem.WriteAllBytes(MainformRef.NullDCPath & "\dc\vmu_data_host.bin", My.Resources.vmu_data_port01, False)
        End If

        My.Computer.FileSystem.WriteAllBytes(MainformRef.NullDCPath & "\dc\data\vmu_default.bin", My.Resources.vmu_data_port01, False)

        Dim EnableOnline = "0"
        If Not MainformRef.ConfigFile.Status = "Offline" Then EnableOnline = "1"

        Dim IsHosting = "0"
        If MainformRef.ConfigFile.Status = "Hosting" Then IsHosting = "1"

        Dim FPSLimiter = "0"
        If MainformRef.ConfigFile.Status = "Hosting" Or
            MainformRef.ConfigFile.Status = "Offline" Or
            MainformRef.ConfigFile.Status = "Spectator" Then FPSLimiter = "1"

        Dim IsSpectator = "0"
        If MainformRef.ConfigFile.Status = "Spectator" Then IsSpectator = 1

        ' Get P1/P2 Names
        If P1Name = "" Or P2Name = "" Then ' Player names were not set beforehand 
            If MainformRef.ConfigFile.Status = "Offline" Or MainformRef.ConfigFile.Status = "Spectator" Then
                P1Name = MainformRef.ConfigFile.Name
                P2Name = MainformRef.ConfigFile.P2Name
            Else
                If IsHosting Then
                    P1Name = MainformRef.ConfigFile.Name
                    If MainformRef.Challenger Is Nothing Then
                        P2Name = "Solo Host"
                    Else
                        P2Name = MainformRef.Challenger.name
                    End If

                Else
                    P1Name = MainformRef.Challenger.name
                    P2Name = MainformRef.ConfigFile.Name
                End If
            End If

        End If

        ' Get P1/P2 Peripherals
        If P1Peripheral = "" Or P2Peripheral = "" Then ' Player periperals were not set beforehand 
            If MainformRef.ConfigFile.Status = "Offline" Or MainformRef.ConfigFile.Status = "Spectator" Then

                P1Peripheral = tempPeripheral(0)
                P2Peripheral = tempPeripheral(1)

            Else
                If IsHosting Then
                    P1Peripheral = MainformRef.ConfigFile.Peripheral
                    If MainformRef.Challenger Is Nothing Then
                        P2Peripheral = tempPeripheral(1)
                    Else
                        P2Peripheral = MainformRef.Challenger.peripheral
                    End If

                Else
                    P1Peripheral = MainformRef.Challenger.peripheral
                    P2Peripheral = MainformRef.ConfigFile.Peripheral
                End If

            End If

        End If

        ' Get Game Specific Configs Here
        Dim SpecialSettings As New ArrayList
        Dim StartGettingSettings As Boolean = False

        For Each line As String In File.ReadAllLines(MainformRef.NullDCPath & "\dc\GameSpecificSettings.optibear")

            If line.Contains(MainformRef.GamesList(MainformRef.ConfigFile.Game)(3)) Then
                StartGettingSettings = True
                Continue For
            End If

            If line.Contains("::") And StartGettingSettings Then Exit For
            If StartGettingSettings = True And line.Trim.Length > 0 Then
                SpecialSettings.Add(line)
            End If
        Next

        Dim _regionID = 0
        Dim _broadcast = 0

        Select Case Region
            Case "JPN"
                _regionID = 0
                _broadcast = 0
            Case "USA"
                _regionID = 1
                _broadcast = 0
            Case "EUR"
                _regionID = 2
                _broadcast = 1
        End Select

        ' General Settings
        Dim lines() As String = File.ReadAllLines(MainformRef.NullDCPath & "\dc\nullDC.cfg")
        linenumber = 0
        For Each line As String In lines
            ' [nullDC]
            If line.StartsWith("Dynarec.Enabled=") Then lines(linenumber) = "Dynarec.Enabled=1"
            If line.StartsWith("Dynarec.DoConstantPropagation=") Then lines(linenumber) = "Dynarec.DoConstantPropagation=1"
            If line.StartsWith("Dynarec.UnderclockFpu=") Then lines(linenumber) = "Dynarec.UnderclockFpu=0"
            If line.StartsWith("Dreamcast.Cable=") Then lines(linenumber) = "Dreamcast.Cable=0"
            If line.StartsWith("Dreamcast.RTC=") Then lines(linenumber) = "Dreamcast.RTC=1543276807"
            If line.StartsWith("Dreamcast.Region=") Then lines(linenumber) = "Dreamcast.Region=" & _regionID
            If line.StartsWith("Dreamcast.Broadcast=") Then lines(linenumber) = "Dreamcast.Broadcast=" & _broadcast
            If line.StartsWith("Emulator.AutoStart=") Then lines(linenumber) = "Emulator.AutoStart=1"
            If line.StartsWith("Dynarec.SafeMode=") Then lines(linenumber) = "Dynarec.SafeMode=1"

            ' [nullDC_plugins]
            If line.StartsWith("GUI=") Then lines(linenumber) = "GUI=nullDC_GUI_Win32.dll"
            If line.StartsWith("Current_PVR=") Then lines(linenumber) = "Current_PVR=drkPvr_Win32.dll"
            If line.StartsWith("Current_GDR=") Then lines(linenumber) = "Current_GDR=ImgReader_Win32.dll"
            If line.StartsWith("Current_AICA=") Then lines(linenumber) = "Current_AICA=nullAica_Win32.dll"
            If line.StartsWith("Current_ARM=") Then lines(linenumber) = "Current_ARM=vbaARM_Win32.dll"
            If line.StartsWith("Current_ExtDevice=") Then lines(linenumber) = "Current_ExtDevice=nullExtDev_Win32.dll"

            ' [Plugins]
            If line.StartsWith("Current_maple") Then
                lines(linenumber) = line.Split("=")(0) & "=NULL"
                If line.StartsWith("Current_maple0_5") Then lines(linenumber) = "Current_maple0_5=BEARJamma_Win32.dll:0"
                If line.StartsWith("Current_maple0_0") Then lines(linenumber) = "Current_maple0_0=BEARJamma_Win32.dll:1"
                If line.StartsWith("Current_maple1_5") Then lines(linenumber) = "Current_maple1_5=BEARJamma_Win32.dll:0"
            End If

            ' [nullDC_GUI]
            If line.StartsWith("AutoHideMenu=") Then lines(linenumber) = "AutoHideMenu=0"
            If line.StartsWith("Fullscreen=") Then
                IsFullscreen = CInt(line.Split("=")(1))
            End If
            ' [drkpvr]
            If line.StartsWith("Emulation.AlphaSortMode=") Then lines(linenumber) = "Emulation.AlphaSortMode=1"
            If line.StartsWith("Emulation.PaletteMode=") Then lines(linenumber) = "Emulation.PaletteMode=2"
            If line.StartsWith("Emulation.ModVolMode=") Then lines(linenumber) = "Emulation.ModVolMode=2"
            If line.StartsWith("Emulation.ZBufferMode=") Then lines(linenumber) = "Emulation.ZBufferMode=0"
            If line.StartsWith("Emulation.TexCacheMode=") Then lines(linenumber) = "Emulation.TexCacheMode=0"

            If IsSpectator Then
                If line.StartsWith("Video.VSync=") Then lines(linenumber) = "Video.VSync=0"
            Else
                If line.StartsWith("Video.VSync=") Then lines(linenumber) = "Video.VSync=" & MainformRef.ConfigFile.Vsync
            End If

            If line.StartsWith("Enhancements.MultiSampleCount=") Then lines(linenumber) = "Enhancements.MultiSampleCount=0"
            If line.StartsWith("Enhancements.MultiSampleQuality=") Then lines(linenumber) = "Enhancements.MultiSampleQuality=0"
            If line.StartsWith("OSD.ShowVsNames=") Then lines(linenumber) = "OSD.ShowVsNames=" & MainformRef.ConfigFile.VsNames
            If line.StartsWith("Enhancements.ShowGameName=") Then lines(linenumber) = "Enhancements.ShowGameName=" & MainformRef.ConfigFile.ShowGameNameInTitle

            ' [ImageReader]
            If line.StartsWith("PatchRegion=") Then lines(linenumber) = "PatchRegion=1"
            If line.StartsWith("LoadDefaultImage=") Then lines(linenumber) = "LoadDefaultImage=1"
            If line.StartsWith("DefaultImage=") Then lines(linenumber) = "DefaultImage=" & MainformRef.GamesList(MainformRef.ConfigFile.Game)(1).ToString.Replace("\roms\", "roms\")
            If line.StartsWith("LastImage=") Then lines(linenumber) = "LastImage=" & MainformRef.GamesList(MainformRef.ConfigFile.Game)(1).ToString.Replace("\roms\", "roms\")

            ' [nullAica]
            If line.StartsWith("BufferSize=") Then lines(linenumber) = "BufferSize=2048"
            If line.StartsWith("HW_mixing=") Then lines(linenumber) = "HW_mixing=0"
            If line.StartsWith("SoundRenderer=") Then lines(linenumber) = "SoundRenderer=1"
            If line.StartsWith("BufferCount=") Then lines(linenumber) = "BufferCount=1"
            If line.StartsWith("CDDAMute=") Then lines(linenumber) = "CDDAMute=0"
            If line.StartsWith("DSPEnabled=") Then lines(linenumber) = "DSPEnabled=0"
            If line.StartsWith("GlobalFocus=") Then lines(linenumber) = "GlobalFocus=1"
            If line.StartsWith("ForceMono=") Then lines(linenumber) = "ForceMono=" & MainformRef.ConfigFile.ForceMono

            If line.StartsWith("LimitFPS=") Then lines(linenumber) = "LimitFPS=1" ' & FPSLimiter

            If line.StartsWith("Volume=") Then lines(linenumber) = "Volume=" & MainformRef.ConfigFile.EmulatorVolume

            ' [nullExtDev]
            If line.StartsWith("mode=") Then lines(linenumber) = "mode=0"
            If line.StartsWith("adapter=") Then lines(linenumber) = "adapter=0"

            ' [drkMaple]
            If line.StartsWith("Mouse.Sensitivity=") Then lines(linenumber) = "Mouse.Sensitivity=100"
            If line.StartsWith("ShowVMU=") Then lines(linenumber) = "ShowVMU=0"

            ' Change Netplay Shit
            If line.StartsWith("Online=") Then lines(linenumber) = "Online=" & EnableOnline
            If line.StartsWith("Host=") Then lines(linenumber) = "Host=" & MainformRef.ConfigFile.Host
            If line.StartsWith("Hosting=") Then lines(linenumber) = "Hosting=" & IsHosting
            If line.StartsWith("Port=") Then lines(linenumber) = "Port=" & MainformRef.ConfigFile.Port
            If line.StartsWith("Delay=") Then lines(linenumber) = "Delay=" & MainformRef.ConfigFile.Delay
            If line.StartsWith("Record=") Then lines(linenumber) = "Record=0" ' & MainformRef.ConfigFile.RecordReplay
            If line.StartsWith("Playback=") Then lines(linenumber) = "Playback=0" ' & IsReplay
            If line.StartsWith("AllowSpectators=") Then lines(linenumber) = "AllowSpectators=" & MainformRef.ConfigFile.AllowSpectators
            If line.StartsWith("Spectator=") Then lines(linenumber) = "Spectator=" & IsSpectator
            If line.StartsWith("P1Name=") Then lines(linenumber) = "P1Name=" & P1Name
            If line.StartsWith("P2Name=") Then lines(linenumber) = "P2Name=" & P2Name
            If line.StartsWith("File=") Then lines(linenumber) = "File=0" ' & MainformRef.ConfigFile.ReplayFile
            If line.StartsWith("GameName=") Then lines(linenumber) = "GameName=" & MainformRef.GamesList(MainformRef.ConfigFile.Game)(0)
            If line.StartsWith("GameRom=") Then lines(linenumber) = "GameRom=" & MainformRef.ConfigFile.Game
            If line.StartsWith("Region=") Then lines(linenumber) = "Region=" & Region
            If line.StartsWith("Debug=") Then lines(linenumber) = "Debug=" & MainformRef.ConfigFile.DebugControls

            If line.StartsWith("Emulator.NoConsole=") Then
                If MainformRef.ConfigFile.ShowConsole = 1 Then
                    lines(linenumber) = "Emulator.NoConsole=0" '& con
                Else
                    lines(linenumber) = "Emulator.NoConsole=1" '& con
                End If
            End If

            For Each SpecialSettingLine As String In SpecialSettings
                If line.Split("=")(0) = SpecialSettingLine.Split("=")(0) Then
                    lines(linenumber) = SpecialSettingLine
                    Console.WriteLine("Special Setting Applies: " & SpecialSettingLine)
                End If
            Next

            ' Controls DREAMCAST

            If Not ControlsFile Is Nothing Then
                If line.StartsWith("BPort") Then
                    Dim player = 0
                    Dim peri = P1Peripheral

                    If line.StartsWith("BPortB") Then player = 1
                    If player = 1 Then peri = P2Peripheral

                    Dim KeyFound = False
                    For Each control_line As String In ControlsFile
                        If tempPeripheral(player) = "1" Then
                            If control_line.StartsWith("STICK_") Then
                                If line.Contains(control_line.Split("=")(0).Replace("STICK_", "CONT_") & "=") And control_line.Length > 0 Then
                                    lines(linenumber) = line.Split("=")(0) & "=" & control_line.Split("=")(1).Split("|")(player)
                                    KeyFound = True
                                    Exit For
                                End If
                            End If

                        Else

                            If line.Contains(control_line.Split("=")(0) & "=") And control_line.Length > 0 Then
                                lines(linenumber) = line.Split("=")(0) & "=" & control_line.Split("=")(1).Split("|")(player)
                                KeyFound = True
                                Exit For
                            End If

                        End If


                    Next

                    If Not KeyFound Then lines(linenumber) = line.Split("=")(0) & "=k0"

                End If
            End If

            If line.StartsWith("BPortA_Joystick=") Then lines(linenumber) = "BPortA_Joystick=" & tempJoystick(0)
            If line.StartsWith("BPortB_Joystick=") Then lines(linenumber) = "BPortB_Joystick=" & tempJoystick(1)

            If line.StartsWith("BPortA_Deadzone=") Then lines(linenumber) = "BPortA_Deadzone=" & TempDeadzone(0)
            If line.StartsWith("BPortB_Deadzone=") Then lines(linenumber) = "BPortB_Deadzone=" & TempDeadzone(1)

            If line.StartsWith("BPortA_Peripheral=") Then lines(linenumber) = "BPortA_Peripheral=" & P1Peripheral
            If line.StartsWith("BPortB_Peripheral=") Then lines(linenumber) = "BPortB_Peripheral=" & P2Peripheral
            ' Arcadestick sync here
            linenumber += 1

        Next

        File.SetAttributes(MainformRef.NullDCPath & "\dc\nullDC.cfg", FileAttributes.Normal)
        File.WriteAllLines(MainformRef.NullDCPath & "\dc\nullDC.cfg", lines)

        ' Stuff to help Casters
        If File.Exists(MainformRef.NullDCPath & "\P1Name.txt") Then File.SetAttributes(MainformRef.NullDCPath & "\P1Name.txt", FileAttributes.Normal)
        File.WriteAllLines(MainformRef.NullDCPath & "\P1Name.txt", {P1Name})

        If File.Exists(MainformRef.NullDCPath & "\P2Name.txt") Then File.SetAttributes(MainformRef.NullDCPath & "\P2Name.txt", FileAttributes.Normal)
        File.WriteAllLines(MainformRef.NullDCPath & "\P2Name.txt", {P2Name})

    End Sub

    Private Sub ChangeSettings_Naomi()
        Dim linenumber = 0

        DealWithBios()
        DeleteNVMEM()
        lstCheck()

        Dim ControlsFile() As String = File.ReadAllLines(GetControlsFilePath)

        For Each line In ControlsFile
            If line.StartsWith("CONT_") Or line.StartsWith("STICK_") Then
                ControlsFile(linenumber) = ""
            End If
            linenumber += 1
        Next

        Dim IsReplay As Int16 = 0
        Dim IsSpectator As Int16 = 0
        Dim IsHosting = "0"
        Dim EnableOnline = "0"

        If Not MainformRef.ConfigFile.Status = "Offline" Then EnableOnline = "1"

        If Not MainformRef.ConfigFile.ReplayFile = "" Then IsReplay = 1
        If MainformRef.ConfigFile.Status = "Spectator" Then IsSpectator = 1
        If MainformRef.ConfigFile.Status = "Hosting" Then IsHosting = "1"

        ' Get P1/P2 Names
        If P1Name = "" Or P2Name = "" Then ' Player names were not set beforehand 
            If MainformRef.ConfigFile.Status = "Offline" Or MainformRef.ConfigFile.Status = "Spectator" Then
                P1Name = MainformRef.ConfigFile.Name
                P2Name = MainformRef.ConfigFile.P2Name
            Else
                If IsHosting Then
                    P1Name = MainformRef.ConfigFile.Name
                    If MainformRef.Challenger Is Nothing Then
                        P2Name = "Solo Host"
                    Else
                        P2Name = MainformRef.Challenger.name
                    End If

                Else
                    P1Name = MainformRef.Challenger.name
                    P2Name = MainformRef.ConfigFile.Name
                End If
            End If

        End If

        ' Wait for nvmem to be dealt with
        Dim SleepTime = 0
        While File.Exists(MainformRef.NullDCPath & "\data\naomi_nvmem.bin")
            Thread.Sleep(100)
            SleepTime += 100
            If SleepTime > 1000 Then Exit While ' Fuck it just continue
        End While

        Dim FPSLimiter = "0"
        If IsReplay = 1 Or IsSpectator = 1 Or MainformRef.ConfigFile.Status = "Hosting" Or MainformRef.ConfigFile.Status = "Offline" Then FPSLimiter = "1"

        Dim lines() As String = File.ReadAllLines(MainformRef.NullDCPath & "\nullDC.cfg")
        linenumber = 0
        For Each line As String In lines

            ' [nullDC]
            If line.StartsWith("Dynarec.Enabled=") Then lines(linenumber) = "Dynarec.Enabled=1"
            If line.StartsWith("Dynarec.DoConstantPropagation=") Then lines(linenumber) = "Dynarec.DoConstantPropagation=1"
            If line.StartsWith("Dynarec.UnderclockFpu=") Then lines(linenumber) = "Dynarec.UnderclockFpu=0"
            If line.StartsWith("Dreamcast.Cable=") Then lines(linenumber) = "Dreamcast.Cable=1"
            If line.StartsWith("Dreamcast.RTC=") Then lines(linenumber) = "Dreamcast.RTC=1917526710"
            If line.StartsWith("Dreamcast.Region=") Then lines(linenumber) = "Dreamcast.Region=0"
            If line.StartsWith("Dreamcast.Broadcast=") Then lines(linenumber) = "Dreamcast.Broadcast=0"
            If line.StartsWith("Emulator.AutoStart=") Then lines(linenumber) = "Emulator.AutoStart=0"
            If line.StartsWith("Dynarec.SafeMode=") Then lines(linenumber) = "Dynarec.SafeMode=1"

            ' [nullDC_GUI]
            If line.StartsWith("AutoHideMenu=") Then lines(linenumber) = "AutoHideMenu=0"
            If line.StartsWith("Fullscreen=") Then
                IsFullscreen = CInt(line.Split("=")(1))
            End If

            ' [drkpvr]
            If line.StartsWith("Emulation.AlphaSortMode=") Then lines(linenumber) = "Emulation.AlphaSortMode=1"
            If line.StartsWith("Emulation.PaletteMode=") Then lines(linenumber) = "Emulation.PaletteMode=2"
            If line.StartsWith("Emulation.ModVolMode=") Then lines(linenumber) = "Emulation.ModVolMode=2"
            If line.StartsWith("Emulation.ZBufferMode=") Then lines(linenumber) = "Emulation.ZBufferMode=0"
            If line.StartsWith("Emulation.TexCacheMode=") Then lines(linenumber) = "Emulation.TexCacheMode=0"

            If IsSpectator Then
                If line.StartsWith("Video.VSync=") Then lines(linenumber) = "Video.VSync=0"
            Else
                If line.StartsWith("Video.VSync=") Then lines(linenumber) = "Video.VSync=" & MainformRef.ConfigFile.Vsync
            End If


            If line.StartsWith("Enhancements.MultiSampleCount=") Then lines(linenumber) = "Enhancements.MultiSampleCount=0"
            If line.StartsWith("Enhancements.MultiSampleQuality=") Then lines(linenumber) = "Enhancements.MultiSampleQuality=0"
            If line.StartsWith("OSD.ShowVsNames=") Then lines(linenumber) = "OSD.ShowVsNames=" & MainformRef.ConfigFile.VsNames
            If line.StartsWith("Enhancements.ShowGameName=") Then lines(linenumber) = "Enhancements.ShowGameName=" & MainformRef.ConfigFile.ShowGameNameInTitle

            'If line.StartsWith("Enhancements.AspectRatioMode=") Then lines(linenumber) = "Enhancements.AspectRatioMode=1"

            ' [nullAica]
            If line.StartsWith("BufferSize=") Then lines(linenumber) = "BufferSize=2048"
            If line.StartsWith("HW_mixing=") Then lines(linenumber) = "HW_mixing=0"
            If line.StartsWith("SoundRenderer=") Then lines(linenumber) = "SoundRenderer=1"
            If line.StartsWith("BufferCount=") Then lines(linenumber) = "BufferCount=1"
            If line.StartsWith("CDDAMute=") Then lines(linenumber) = "CDDAMute=0"
            If line.StartsWith("DSPEnabled=") Then lines(linenumber) = "DSPEnabled=0"
            If line.StartsWith("GlobalFocus=") Then lines(linenumber) = "GlobalFocus=1"
            If line.StartsWith("ForceMono=") Then lines(linenumber) = "ForceMono=" & MainformRef.ConfigFile.ForceMono

            If line.StartsWith("LimitFPS=") Then lines(linenumber) = "LimitFPS=1" ' & FPSLimiter

            If line.StartsWith("Volume=") Then lines(linenumber) = "Volume=" & CInt((MainformRef.ConfigFile.EmulatorVolume / 10) + ((MainformRef.ConfigFile.EmulatorVolume - (MainformRef.ConfigFile.EmulatorVolume / 10)) * (MainformRef.ConfigFile.EmulatorVolume / 100)))

            ' [nullExtDev]
            If line.StartsWith("mode=") Then lines(linenumber) = "mode=0"
            If line.StartsWith("adapter=") Then lines(linenumber) = "adapter=0"

            ' [drkMaple]
            If line.StartsWith("VMU.Show=") Then lines(linenumber) = "VMU.Show=1"
            If line.StartsWith("Mouse.Sensitivity=") Then lines(linenumber) = "Mouse.Sensitivity=100"
            If line.StartsWith("ShowVMU=") Then lines(linenumber) = "ShowVMU=1"

            ' [ImageReader]
            If line.StartsWith("PatchRegion=") Then lines(linenumber) = "PatchRegion=1"
            If line.StartsWith("LoadDefaultImage=") Then lines(linenumber) = "LoadDefaultImage=0"
            If line.StartsWith("DefaultImage=") Then lines(linenumber) = "DefaultImage=default.gdi"
            If line.StartsWith("LastImage=") Then lines(linenumber) = "LastImage=c:\game.gdi"

            ' [nullDC_plugins]
            If line.StartsWith("GUI=") Then lines(linenumber) = "GUI=nullDC_GUI_Win32.dll"
            If line.StartsWith("Current_PVR=") Then lines(linenumber) = "Current_PVR=drkPvr_Win32.dll"
            If line.StartsWith("Current_GDR=") Then lines(linenumber) = "Current_GDR=ImgReader_Win32.dll"
            If line.StartsWith("Current_AICA=") Then lines(linenumber) = "Current_AICA=nullAica_Win32.dll"
            If line.StartsWith("Current_ARM=") Then lines(linenumber) = "Current_ARM=vbaARM_Win32.dll"
            If line.StartsWith("Current_ExtDevice=") Then lines(linenumber) = "Current_ExtDevice=nullExtDev_Win32.dll"

            ' [Naomi]
            If line.StartsWith("LoadDefaultRom=") Then lines(linenumber) = "LoadDefaultRom=1"
            If line.StartsWith("DefaultRom=") Then lines(linenumber) = "DefaultRom=" & MainformRef.GamesList(MainformRef.ConfigFile.Game)(1).ToString.Replace("\roms\", "roms\")

            ' [Plugins]
            If line.StartsWith("Current_maple") Then
                lines(linenumber) = line.Split("=")(0) & "=NULL"
                If line.StartsWith("Current_maple0_5") Then lines(linenumber) = "Current_maple0_5=BEARJamma_Win32.dll:0"
            End If

            ' Netplay
            If line.StartsWith("Online=") Then lines(linenumber) = "Online=" & EnableOnline
            If line.StartsWith("Host=") Then lines(linenumber) = "Host=" & MainformRef.ConfigFile.Host
            If line.StartsWith("Hosting=") Then lines(linenumber) = "Hosting=" & IsHosting
            If line.StartsWith("Port=") Then lines(linenumber) = "Port=" & MainformRef.ConfigFile.Port
            If line.StartsWith("Delay=") Then lines(linenumber) = "Delay=" & MainformRef.ConfigFile.Delay

            If EnableOnline Then
                If line.StartsWith("Record=") Then lines(linenumber) = "Record=" & MainformRef.ConfigFile.RecordReplay
            Else
                If MainformRef.ConfigFile.Status = "Spectator" Then
                    If line.StartsWith("Record=") Then lines(linenumber) = "Record=0"
                Else
                    If line.StartsWith("Record=") Then lines(linenumber) = "Record=" & MainformRef.ConfigFile.RecordReplay
                End If
            End If

            If line.StartsWith("Playback=") Then lines(linenumber) = "Playback=" & IsReplay
            If line.StartsWith("AllowSpectators=") Then lines(linenumber) = "AllowSpectators=" & MainformRef.ConfigFile.AllowSpectators
            If line.StartsWith("Spectator=") Then lines(linenumber) = "Spectator=" & IsSpectator
            If line.StartsWith("P1Name=") Then lines(linenumber) = "P1Name=" & P1Name
            If line.StartsWith("P2Name=") Then lines(linenumber) = "P2Name=" & P2Name
            If line.StartsWith("File=") Then lines(linenumber) = "File=" & MainformRef.ConfigFile.ReplayFile
            If line.StartsWith("GameName=") Then lines(linenumber) = "GameName=" & MainformRef.GamesList(MainformRef.ConfigFile.Game)(0)
            If line.StartsWith("GameRom=") Then lines(linenumber) = "GameRom=" & MainformRef.ConfigFile.Game
            If line.StartsWith("Region=") Then lines(linenumber) = "Region=" & Region
            If line.StartsWith("Debug=") Then lines(linenumber) = "Debug=" & MainformRef.ConfigFile.DebugControls

            If line.StartsWith("Emulator.NoConsole=") Then
                If MainformRef.ConfigFile.ShowConsole = 1 Then
                    lines(linenumber) = "Emulator.NoConsole=0" '& con
                Else
                    lines(linenumber) = "Emulator.NoConsole=1" '& con
                End If
            End If

            ' Check Buttons Configs

            If Not ControlsFile Is Nothing Then
                If line.StartsWith("BPort") Then
                    Dim player = 0
                    If line.StartsWith("BPortB") Then player = 1

                    Dim KeyCount = False
                    For Each control_line As String In ControlsFile
                        If line.Contains(control_line.Split("=")(0) & "=") And control_line.Length > 0 Then
                            lines(linenumber) = line.Split("=")(0) & "=" & control_line.Split("=")(1).Split("|")(player)
                            KeyCount = True
                            Exit For
                        End If
                    Next

                    If Not KeyCount Then lines(linenumber) = line.Split("=")(0) & "=k0"

                End If
            End If
            linenumber += 1
        Next

        'Emulator.NoConsole
        File.SetAttributes(MainformRef.NullDCPath & "\nullDC.cfg", FileAttributes.Normal)
        File.WriteAllLines(MainformRef.NullDCPath & "\nullDC.cfg", lines)

        ' Stuff to help Casters
        If File.Exists(MainformRef.NullDCPath & "\P1Name.txt") Then File.SetAttributes(MainformRef.NullDCPath & "\P1Name.txt", FileAttributes.Normal)
        File.WriteAllLines(MainformRef.NullDCPath & "\P1Name.txt", {P1Name})

        If File.Exists(MainformRef.NullDCPath & "\P2Name.txt") Then File.SetAttributes(MainformRef.NullDCPath & "\P2Name.txt", FileAttributes.Normal)
        File.WriteAllLines(MainformRef.NullDCPath & "\P2Name.txt", {P2Name})

    End Sub

    Private Sub lstCheck()
        Dim lstFile = MainformRef.NullDCPath & MainformRef.GamesList(MainformRef.ConfigFile.Game)(1)
        If File.Exists(lstFile) Then
            Dim lines = File.ReadAllLines(lstFile)
            Dim NameLine As String = "Game Name"
            Dim BinLine As String = "The Bin Stuff"
            Dim NameLineNumber = -1
            Dim BinLineNumber = -1

            Dim LineCount = 1
            For Each line As String In lines
                If line.Replace(" ", vbEmpty).Length > 0 Then
                    If line.StartsWith("""") Then
                        BinLine = line
                        BinLineNumber = LineCount
                    Else
                        NameLine = line
                        NameLineNumber = LineCount
                    End If
                End If
                LineCount += 1
            Next

            If Not NameLineNumber = 1 Or Not BinLineNumber = 2 Then
                File.SetAttributes(lstFile, FileAttributes.Normal)
                File.WriteAllLines(lstFile, {NameLine, BinLine})
                Console.WriteLine("Fixed lst file")
            End If

        End If
    End Sub

End Class