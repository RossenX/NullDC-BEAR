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
    Dim AutoHotkey As Process
    Dim SingleInstance As Boolean = True
    Dim LoadRomThread As Thread

#Region "API"
    Private Declare Function FindWindow Lib "user32" Alias "FindWindowA" (ByVal lpClassName As String, ByVal lpWindowName As String) As IntPtr

    Private Declare Function SendMessage Lib "user32" Alias "SendMessageA" (ByVal hWnd As Int32, ByVal wMsg As Int32, ByVal _wParam As Int32, lParam As String) As Int32

    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
    Private Shared Function PostMessage(ByVal hWnd As IntPtr, ByVal Msg As UInteger, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As Boolean
    End Function

    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
    Private Shared Function FindWindowEx(ByVal parentHandle As IntPtr, ByVal childAfter As IntPtr, ByVal lclassName As String, ByVal windowTitle As String) As IntPtr
    End Function

    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
    Public Shared Function SetWindowText(hWnd As IntPtr, lpString As String) As Boolean
    End Function
    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function IsWindowVisible(ByVal hWnd As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function
    <DllImport("user32.dll", EntryPoint:="FindWindowW")>
    Public Shared Function FindWindowW(<MarshalAs(UnmanagedType.LPTStr)> ByVal lpClassName As String, <MarshalAs(UnmanagedType.LPTStr)> ByVal lpWindowName As String) As IntPtr
    End Function

    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
    Private Shared Function GetDlgItem(ByVal hDlg As IntPtr, id As Integer) As IntPtr
    End Function
    <DllImport("user32.dll", CharSet:=CharSet.Auto)>
    Private Shared Function GetClassName(ByVal hWnd As System.IntPtr, ByVal lpClassName As System.Text.StringBuilder, ByVal nMaxCount As Integer) As Integer
    End Function
    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function SetActiveWindow(ByVal hWnd As IntPtr) As IntPtr
    End Function
    <DllImport("user32.dll", CharSet:=CharSet.Auto)>
    Private Shared Function EnumChildWindows(ByVal hWndParent As System.IntPtr, ByVal lpEnumFunc As EnumWindowsProc, ByVal lParam As Integer) As Boolean
    End Function
    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
    Private Shared Function GetWindowTextLength(ByVal hwnd As IntPtr) As Integer
    End Function
    Private Declare Function SendMessageByString Lib "user32.dll" Alias "SendMessageA" (ByVal hwnd As IntPtr, ByVal wMsg As Integer, ByVal wParam As Integer, ByVal lParam As String) As Integer
    Private Delegate Function EnumWindowsProc(ByVal hWnd As IntPtr, ByVal lParam As IntPtr) As Boolean

    Public Declare Function GetActiveWindow Lib "user32" Alias "GetActiveWindow" () As IntPtr
    Public Declare Function GetWindowText Lib "user32" Alias "GetWindowTextA" (ByVal hwnd As IntPtr, ByVal lpString As System.Text.StringBuilder, ByVal cch As Integer) As Integer
    Public Declare Function MoveWindow Lib "user32.dll" (ByVal hWnd As IntPtr, ByVal X As Int32, ByVal Y As Int32, ByVal nWidth As Int32, ByVal nHeight As Int32, ByVal bRepaint As Boolean) As Boolean
    Declare Function BlockInput Lib "user32" (ByVal fBlockIt As Boolean) As Boolean

    <DllImport("user32.dll")>
    Private Shared Function GetForegroundWindow() As IntPtr
    End Function

    <DllImport("user32.dll")>
    Public Shared Function GetWindowThreadProcessId(ByVal hWnd As IntPtr, <Out()> ByRef lpdwProcessId As UInteger) As UInteger
    End Function

    Private Const BM_CLICK As Integer = &HF5
    Private Const WM_ACTIVATE As Integer = &H6
    Private Const WA_ACTIVE As Integer = &H1
    Private Const WM_COMMAND As Integer = &H111
    Private Const WM_SETTEXT As Integer = &HC
    Private Const WM_CLOSE As Integer = &H10
    Private Const WM_GETTEXT As Integer = &HD
    Private Const WM_GETTEXTLENGTH As Integer = &HE
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

    Public Sub FastForward()
        If Not NullDCproc Is Nothing Then
            PostMessage(NullDCproc.MainWindowHandle, WM_COMMAND, 118, 0)
        End If

    End Sub

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
            GameLoaded()

        Catch ex As Exception
            MsgBox("Rom Loader Failed, woops.")
        End Try

    End Sub

    Public Sub LaunchDreamcast(ByVal _romname As String, ByRef _region As String)
        MainformRef.InputHandler.GetKeyboardConfigs("dc")
        MainformRef.InputHandler.NeedConfigReload = True
        Region = _region

        If MainformRef.IsNullDCRunning And SingleInstance Then
            frmMain.NotificationForm.ShowMessage("An Instance of NullDC online is already running.")
            Exit Sub
        Else
            StartDreamcastEmulator(_romname)
        End If

    End Sub

    Public Sub LaunchNaomi(ByVal _romname As String, ByRef _region As String)
        MainformRef.InputHandler.GetKeyboardConfigs("naomi")
        MainformRef.InputHandler.NeedConfigReload = True
        Region = _region

        If MainformRef.IsNullDCRunning And SingleInstance Then
            frmMain.NotificationForm.ShowMessage("An Instance of NullDC online is already running.")
            Exit Sub
        Else
            StartNaomiEmulator(_romname)
        End If

    End Sub

    Private Sub EmulatorExited()
        Console.Write("Emulator Exited")
        While MainformRef.IsNullDCRunning
            Thread.Sleep(10)
        End While

        RestoreNvmem()

        If Not DoNotSendNextExitEvent Then
            If Not MainformRef.Challenger Is Nothing And (MainformRef.ConfigFile.Status = "Hosting" Or MainformRef.ConfigFile.Status = "Client") Then
                Console.WriteLine("Telling challenger i quit")
                MainformRef.NetworkHandler.SendMessage(">,E", MainformRef.Challenger.ip)
            End If

            Dim INVOKATION As EndSession_delegate = AddressOf MainformRef.EndSession
            MainformRef.Invoke(INVOKATION, {"Window Closed", Nothing})

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
    End Sub

    Private Sub StartDreamcastEmulator(ByVal _romname As String)
        ChangeSettings_Dreamcast()
        NullDCproc = Process.Start(MainformRef.NullDCPath & "\dc\nullDC_Win32_Release-NoTrace.exe")
        NullDCproc.EnableRaisingEvents = True
        AddHandler NullDCproc.Exited, AddressOf EmulatorExited
        LoadGame()

    End Sub

    Private Sub StartNaomiEmulator(ByVal _romname As String)
        ChangeSettings_Naomi()

        Rx.EEPROM = Rx.GetEEPROM(MainformRef.NullDCPath & MainformRef.GamesList(_romname)(1))

        NullDCproc = Process.Start(MainformRef.NullDCPath & "\nullDC_Win32_Release-NoTrace.exe")
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
            MainformRef.NetworkHandler.SendMessage("$," & MainformRef.ConfigFile.Name & "," & MainformRef.ConfigFile.IP & "," & MainformRef.ConfigFile.Port & "," & MainformRef.ConfigFile.Game & "," & MainformRef.ConfigFile.Delay & "," & Region & ",eeprom," & Rx.EEPROM, MainformRef.Challenger.ip)
        End If
    End Sub

    Private Sub RestoreNvmem() ' Mostly so it doesn't fuck up blue's launcher
        Dim nvmemPath = MainformRef.NullDCPath & "\data\naomi_nvmem.bin"
        Dim nvmemPathBackup = MainformRef.NullDCPath & "\data\naomi_nvmem.bin_backup"

        Try
            If File.Exists(nvmemPath) Then ' nvmem exists
                File.SetAttributes(nvmemPath, FileAttributes.Normal)
                File.Delete(nvmemPath)

                If File.Exists(nvmemPathBackup) Then ' Backup Exists
                    File.SetAttributes(nvmemPathBackup, FileAttributes.Normal)
                    My.Computer.FileSystem.RenameFile(nvmemPathBackup, "naomi_nvmem.bin")
                    File.SetAttributes(nvmemPath, FileAttributes.ReadOnly)
                End If
            End If
        Catch ex As Exception
            MsgBox("Couldn't restore nvmem: " & ex.Message)
        End Try

        Rx.RestoreEEPROM(MainformRef.NullDCPath & MainformRef.GamesList(MainformRef.ConfigFile.Game)(1))

    End Sub

    Private Sub BackupNvmem()
        ' Fuck nvMEM get rid of that shit
        Dim nvmemPath = MainformRef.NullDCPath & "\data\naomi_nvmem.bin"
        Dim nvmemPathBackup = MainformRef.NullDCPath & "\data\naomi_nvmem.bin_backup"

        Try
            If File.Exists(nvmemPath) Then
                If File.Exists(nvmemPathBackup) Then
                    File.SetAttributes(nvmemPath, FileAttributes.Normal)
                    File.Delete(nvmemPath)
                Else
                    File.SetAttributes(nvmemPath, FileAttributes.Normal)
                    My.Computer.FileSystem.RenameFile(nvmemPath, "naomi_nvmem.bin_backup")
                    File.SetAttributes(nvmemPathBackup, FileAttributes.ReadOnly)
                End If
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

        ' Put in the VMU to keep it in sync for now
        My.Computer.FileSystem.WriteAllBytes(MainformRef.NullDCPath & "\dc\vmu_data_port01.bin", My.Resources.vmu_data_port01, False)

        Dim IsReplay As Int16 = 0
        Dim IsSpectator As Int16 = 0
        Dim IsHosting = "0"

        If Not MainformRef.ConfigFile.ReplayFile = "" Then IsReplay = 1
        If MainformRef.ConfigFile.Status = "Spectator" Then IsSpectator = 1
        If MainformRef.ConfigFile.Status = "Hosting" Then IsHosting = "1"

        Dim FPSLimiter = "0"
        If MainformRef.ConfigFile.Status = "Hosting" Or MainformRef.ConfigFile.Status = "Offline" Then FPSLimiter = "1"

        ' Get Game Specific Configs Here
        Dim SpecialSettings As New ArrayList
        Dim StartGettingSettings As Boolean = False

        For Each line As String In My.Resources.DreamcastGameOptimizations.Split(vbNewLine)
            line = line.Trim
            If line.Contains(MainformRef.GamesList(MainformRef.ConfigFile.Game)(3)) Then
                StartGettingSettings = True
                Continue For
            End If

            If line.Contains("::") And StartGettingSettings Then Exit For
            If StartGettingSettings Then SpecialSettings.Add(line)
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
        Dim linenumber = 0
        For Each line As String In lines
            ' [nullDC]
            If line.StartsWith("Dynarec.Enabled=") Then lines(linenumber) = "Dynarec.Enabled=1"
            If line.StartsWith("Dynarec.DoConstantPropagation=") Then lines(linenumber) = "Dynarec.DoConstantPropagation=1"
            If line.StartsWith("Dynarec.UnderclockFpu=") Then lines(linenumber) = "Dynarec.UnderclockFpu=0"
            If line.StartsWith("Dreamcast.Cable=") Then lines(linenumber) = "Dreamcast.Cable=2"
            If line.StartsWith("Dreamcast.RTC=") Then lines(linenumber) = "Dreamcast.RTC=1543276807" ' 1543276807
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

            If line.StartsWith("Current_maple") Then
                lines(linenumber) = line.Split("=")(0) & "=NULL"
                If line.StartsWith("Current_maple0_5") Then lines(linenumber) = "Current_maple0_5=BEARJamma_Win32.dll:0"
                If line.StartsWith("Current_maple0_0") Then lines(linenumber) = "Current_maple0_0=BEARJamma_Win32.dll:1"
                If line.StartsWith("Current_maple1_5") Then lines(linenumber) = "Current_maple1_5=BEARJamma_Win32.dll:0"

            End If

            ' [nullDC_GUI]
            If line.StartsWith("AutoHideMenu=") Then lines(linenumber) = "AutoHideMenu=0"

            ' [drkpvr]
            If line.StartsWith("Emulation.AlphaSortMode=") Then lines(linenumber) = "Emulation.AlphaSortMode=2"
            If line.StartsWith("Emulation.PaletteMode=") Then lines(linenumber) = "Emulation.PaletteMode=1"
            If line.StartsWith("Emulation.ModVolMode=") Then lines(linenumber) = "Emulation.ModVolMode=0"
            If line.StartsWith("Emulation.ZBufferMode=") Then lines(linenumber) = "Emulation.ZBufferMode=0"
            If line.StartsWith("Emulation.TexCacheMode=") Then lines(linenumber) = "Emulation.TexCacheMode=0"
            If line.StartsWith("Video.VSync=") Then lines(linenumber) = "Video.VSync=0"
            If line.StartsWith("Enhancements.MultiSampleCount=") Then lines(linenumber) = "Enhancements.MultiSampleCount=0"
            If line.StartsWith("Enhancements.MultiSampleQuality=") Then lines(linenumber) = "Enhancements.MultiSampleQuality=0"

            ' [ImageReader]
            If line.StartsWith("PatchRegion=") Then lines(linenumber) = "PatchRegion=0"
            If line.StartsWith("LoadDefaultImage=") Then lines(linenumber) = "LoadDefaultImage=1"
            If line.StartsWith("DefaultImage=") Then lines(linenumber) = "DefaultImage=roms\" & MainformRef.ConfigFile.Game
            If line.StartsWith("LastImage=") Then lines(linenumber) = "LastImage=roms\" & MainformRef.ConfigFile.Game

            ' [nullAica]
            If line.StartsWith("BufferSize=") Then lines(linenumber) = "BufferSize=2048"
            If line.StartsWith("HW_mixing=") Then lines(linenumber) = "HW_mixing=0"
            If line.StartsWith("SoundRenderer=") Then lines(linenumber) = "SoundRenderer=1"
            If line.StartsWith("BufferCount=") Then lines(linenumber) = "BufferCount=1"
            If line.StartsWith("CDDAMute=") Then lines(linenumber) = "CDDAMute=0"
            If line.StartsWith("DSPEnabled=") Then lines(linenumber) = "DSPEnabled=0"
            If line.StartsWith("GlobalFocus=") Then lines(linenumber) = "GlobalFocus=1"
            If line.StartsWith("LimitFPS=") Then lines(linenumber) = "LimitFPS=" & FPSLimiter
            If line.StartsWith("Volume=") Then lines(linenumber) = "Volume=" & CInt((MainformRef.ConfigFile.EmulatorVolume / 10) + ((MainformRef.ConfigFile.EmulatorVolume - (MainformRef.ConfigFile.EmulatorVolume / 10)) * (MainformRef.ConfigFile.EmulatorVolume / 100)))

            ' [nullExtDev]
            If line.StartsWith("mode=") Then lines(linenumber) = "mode=0"
            If line.StartsWith("adapter=") Then lines(linenumber) = "adapter=0"

            ' [drkMaple]
            If line.StartsWith("Mouse.Sensitivity=") Then lines(linenumber) = "Mouse.Sensitivity=100"
            If line.StartsWith("ShowVMU=") Then lines(linenumber) = "ShowVMU=0"

            ' Change Netplay Shit
            If line.StartsWith("[BEARPlay]") Then
                Dim EnableOnline = "0"
                If Not MainformRef.ConfigFile.Status = "Offline" Then EnableOnline = "1"

                lines(linenumber + 1) = "Online=" & EnableOnline
                lines(linenumber + 2) = "Host=" & MainformRef.ConfigFile.Host
                lines(linenumber + 3) = "Hosting=" & IsHosting
                lines(linenumber + 4) = "Port=" & MainformRef.ConfigFile.Port
                lines(linenumber + 5) = "Delay=" & MainformRef.ConfigFile.Delay
                lines(linenumber + 6) = "Record=0" ' & MainformRef.ConfigFile.RecordReplay
                lines(linenumber + 7) = "Playback=0" ' & IsReplay
                lines(linenumber + 8) = "AllowSpectators=0" ' & MainformRef.ConfigFile.AllowSpectators
                lines(linenumber + 9) = "Spectator=0" ' & IsSpectator
                lines(linenumber + 10) = "P1Name=" & P1Name
                lines(linenumber + 11) = "P2Name=" & P2Name
                lines(linenumber + 12) = "File=" & MainformRef.ConfigFile.ReplayFile
                lines(linenumber + 13) = "GameName=" & MainformRef.GamesList(MainformRef.ConfigFile.Game)(0)
                lines(linenumber + 14) = "GameRom=" & MainformRef.ConfigFile.Game
                lines(linenumber + 15) = "Region=" & Region
            End If

            For Each SpecialSettingLine As String In SpecialSettings
                If line.Split("=")(0) = SpecialSettingLine.Split("=")(0) Then
                    lines(linenumber) = SpecialSettingLine
                End If
            Next

            linenumber += 1
        Next

        File.SetAttributes(MainformRef.NullDCPath & "\dc\nullDC.cfg", FileAttributes.Normal)
        File.WriteAllLines(MainformRef.NullDCPath & "\dc\nullDC.cfg", lines)

    End Sub

    Private Sub ChangeSettings_Naomi()
        DealWithBios()
        BackupNvmem()
        lstCheck()

        Dim IsReplay As Int16 = 0
        Dim IsSpectator As Int16 = 0
        Dim IsHosting = "0"

        If Not MainformRef.ConfigFile.ReplayFile = "" Then IsReplay = 1
        If MainformRef.ConfigFile.Status = "Spectator" Then IsSpectator = 1
        If MainformRef.ConfigFile.Status = "Hosting" Then IsHosting = "1"

        ' Get P1/P2 Names
        If P1Name = "" Or P2Name = "" Then ' Player names were not set beforehand 
            If MainformRef.ConfigFile.Status = "Offline" Or MainformRef.ConfigFile.Status = "Spectator" Then
                P1Name = MainformRef.ConfigFile.Name
                P2Name = "Local Player 2"
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


        Dim thefile = MainformRef.NullDCPath & "\antilag.cfg"
        Dim FPSLimit = "90"
        Dim FPSLimiter = "0"

        If IsReplay = 1 Or IsSpectator = 1 Then ' Replays and Spectating are always in Audio Sync mode for best clarity plus it allows fast forward at x2 speed
            FPSLimiter = "1"
            FPSLimit = "120" ' Apperanltly replays can't go faster than 90fps
        Else
            If MainformRef.ConfigFile.Status = "Hosting" Or MainformRef.ConfigFile.Status = "Offline" Then
                If MainformRef.ConfigFile.HostType = "0" Then
                    FPSLimiter = "0"
                    FPSLimit = MainformRef.ConfigFile.FPSLimit
                Else
                    FPSLimiter = "1"
                End If
            End If
        End If

        If IsReplay = 1 Or IsSpectator = 1 Or MainformRef.ConfigFile.Status = "Hosting" Or MainformRef.ConfigFile.Status = "Offline" Then
            FPSLimiter = "1"
        Else
            FPSLimiter = "0"
        End If

        ' No Longer Need to rewrite the entire file
        File.WriteAllLines(thefile, {"[config]", "RenderAheadLimit=0", "FPSlimit=" & FPSLimit})

        thefile = MainformRef.NullDCPath & "\nullDC.cfg"
        Dim lines() As String = File.ReadAllLines(thefile)
        Dim linenumber = 0
        For Each line As String In lines

            ' [nullDC]
            If line.StartsWith("Dynarec.Enabled=") Then lines(linenumber) = "Dynarec.Enabled=1"
            If line.StartsWith("Dynarec.DoConstantPropagation=") Then lines(linenumber) = "Dynarec.DoConstantPropagation=1"
            If line.StartsWith("Dynarec.UnderclockFpu=") Then lines(linenumber) = "Dynarec.UnderclockFpu=0"
            If line.StartsWith("Dreamcast.Cable=") Then lines(linenumber) = "Dreamcast.Cable=1"
            If line.StartsWith("Dreamcast.RTC=") Then lines(linenumber) = "Dreamcast.RTC=1543298463" ' 1917526710
            If line.StartsWith("Dreamcast.Region=") Then lines(linenumber) = "Dreamcast.Region=0"
            If line.StartsWith("Dreamcast.Broadcast=") Then lines(linenumber) = "Dreamcast.Broadcast=0"
            If line.StartsWith("Emulator.AutoStart=") Then lines(linenumber) = "Emulator.AutoStart=0"
            If line.StartsWith("Dynarec.SafeMode=") Then lines(linenumber) = "Dynarec.SafeMode=1"

            ' [nullDC_GUI]
            If line.StartsWith("AutoHideMenu=") Then lines(linenumber) = "AutoHideMenu=0"

            ' [drkpvr]
            If line.StartsWith("Emulation.AlphaSortMode=") Then lines(linenumber) = "Emulation.AlphaSortMode=1"
            If line.StartsWith("Emulation.PaletteMode=") Then lines(linenumber) = "Emulation.PaletteMode=1"
            If line.StartsWith("Emulation.ModVolMode=") Then lines(linenumber) = "Emulation.ModVolMode=1"
            If line.StartsWith("Emulation.ZBufferMode=") Then lines(linenumber) = "Emulation.ZBufferMode=0"
            If line.StartsWith("Emulation.TexCacheMode=") Then lines(linenumber) = "Emulation.TexCacheMode=0"
            If line.StartsWith("Video.VSync=") Then lines(linenumber) = "Video.VSync=0"
            If line.StartsWith("Enhancements.MultiSampleCount=") Then lines(linenumber) = "Enhancements.MultiSampleCount=0"
            If line.StartsWith("Enhancements.MultiSampleQuality=") Then lines(linenumber) = "Enhancements.MultiSampleQuality=0"

            'If line.StartsWith("Enhancements.AspectRatioMode=") Then lines(linenumber) = "Enhancements.AspectRatioMode=1"

            ' [nullAica]
            If line.StartsWith("BufferSize=") Then lines(linenumber) = "BufferSize=2048"
            If line.StartsWith("HW_mixing=") Then lines(linenumber) = "HW_mixing=0"
            If line.StartsWith("SoundRenderer=") Then lines(linenumber) = "SoundRenderer=1"
            If line.StartsWith("BufferCount=") Then lines(linenumber) = "BufferCount=1"
            If line.StartsWith("CDDAMute=") Then lines(linenumber) = "CDDAMute=0"
            If line.StartsWith("DSPEnabled=") Then lines(linenumber) = "DSPEnabled=0"
            If line.StartsWith("GlobalFocus=") Then lines(linenumber) = "GlobalFocus=1"
            If line.StartsWith("LimitFPS=") Then lines(linenumber) = "LimitFPS=" & FPSLimiter
            If line.StartsWith("Volume=") Then lines(linenumber) = "Volume=" & CInt((MainformRef.ConfigFile.EmulatorVolume / 10) + ((MainformRef.ConfigFile.EmulatorVolume - (MainformRef.ConfigFile.EmulatorVolume / 10)) * (MainformRef.ConfigFile.EmulatorVolume / 100)))

            ' [nullExtDev]
            If line.StartsWith("mode=") Then lines(linenumber) = "mode=0"
            If line.StartsWith("adapter=") Then lines(linenumber) = "adapter=0"

            ' [drkMaple]
            If line.StartsWith("VMU.Show=") Then lines(linenumber) = "VMU.Show=1"
            If line.StartsWith("Mouse.Sensitivity=") Then lines(linenumber) = "Mouse.Sensitivity=100"
            If line.StartsWith("ShowVMU=") Then lines(linenumber) = "ShowVMU=1"

            ' [ImageReader]
            If line.StartsWith("PatchRegion=") Then lines(linenumber) = "PatchRegion=0"
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


            If line.StartsWith("Current_maple0_5=") Then
                lines(linenumber) = "Current_maple0_5=BEARJamma_Win32.dll:0" ' Make sure that BEAR  is only plugin
                lines(linenumber + 1) = "Current_maple1_5=NULL"
                lines(linenumber + 2) = "Current_maple2_5=NULL"
                lines(linenumber + 3) = "Current_maple3_5=NULL"
            End If


            ' Change Netplay Shit
            If line.StartsWith("[BEARPlay]") Then
                Dim EnableOnline = "0"
                If Not MainformRef.ConfigFile.Status = "Offline" Then EnableOnline = "1"

                lines(linenumber + 1) = "Online=" & EnableOnline
                lines(linenumber + 2) = "Host=" & MainformRef.ConfigFile.Host
                lines(linenumber + 3) = "Hosting=" & IsHosting
                lines(linenumber + 4) = "Port=" & MainformRef.ConfigFile.Port
                lines(linenumber + 5) = "Delay=" & MainformRef.ConfigFile.Delay
                lines(linenumber + 6) = "Record=" & MainformRef.ConfigFile.RecordReplay
                lines(linenumber + 7) = "Playback=" & IsReplay
                lines(linenumber + 8) = "AllowSpectators=" & MainformRef.ConfigFile.AllowSpectators
                lines(linenumber + 9) = "Spectator=" & IsSpectator
                lines(linenumber + 10) = "P1Name=" & P1Name
                lines(linenumber + 11) = "P2Name=" & P2Name
                lines(linenumber + 12) = "File=" & MainformRef.ConfigFile.ReplayFile
                lines(linenumber + 13) = "GameName=" & MainformRef.GamesList(MainformRef.ConfigFile.Game)(0)
                lines(linenumber + 14) = "GameRom=" & MainformRef.ConfigFile.Game
                lines(linenumber + 15) = "Region=" & Region
            End If

            ' Audio Sync on host only
            If line.StartsWith("Emulator.NoConsole=") Then
                If MainformRef.ConfigFile.ShowConsole = 1 Then
                    lines(linenumber) = "Emulator.NoConsole=0" '& con
                Else
                    lines(linenumber) = "Emulator.NoConsole=1" '& con
                End If
            End If

            linenumber += 1
        Next

        'Emulator.NoConsole
        File.SetAttributes(thefile, FileAttributes.Normal)
        File.WriteAllLines(thefile, lines)

        ' Stuff to help Casters
        If File.Exists(MainformRef.NullDCPath & "\replays\Player1Name.txt") Then File.SetAttributes(MainformRef.NullDCPath & "\replays\Player1Name.txt", FileAttributes.Normal)
        File.WriteAllLines(MainformRef.NullDCPath & "\replays\Player1Name.txt", {P1Name})

        If File.Exists(MainformRef.NullDCPath & "\replays\Player2Name.txt") Then File.SetAttributes(MainformRef.NullDCPath & "\replays\Player2Name.txt", FileAttributes.Normal)
        File.WriteAllLines(MainformRef.NullDCPath & "\replays\Player2Name.txt", {P2Name})

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