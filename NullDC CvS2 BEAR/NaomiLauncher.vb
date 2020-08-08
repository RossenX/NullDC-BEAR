Imports System.IO
Imports System.Runtime.InteropServices
Imports NullDC_CvS2_BEAR.frmMain
Imports System.Threading
Imports System.Globalization
Imports System.Text

Public Class NaomiLauncher

    Public Region As String
    Public NullDCproc As Process
    Public DoNotSendNextExitEvent As Boolean
    Public P1Name As String = ""
    Public P2Name As String = ""
    Dim AutoHotkey As Process
    Dim SingleInstance As Boolean = True
    'Dim MainFormRef As frmMain
    Dim LoadRomThread As Thread

    ' ------------------------------------

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
    Public Declare Function SetForegroundWindow Lib "user32.dll" (ByVal hwnd As Integer) As Integer
    Public Declare Function MoveWindow Lib "user32.dll" (ByVal hWnd As IntPtr, ByVal X As Int32, ByVal Y As Int32, ByVal nWidth As Int32, ByVal nHeight As Int32, ByVal bRepaint As Boolean) As Boolean
    Declare Function BlockInput Lib "user32" (ByVal fBlockIt As Boolean) As Boolean

    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function GetForegroundWindow() As IntPtr
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

    Public Sub LoadRom(ByRef RomPath As String)
        If Not LoadRomThread Is Nothing Then If LoadRomThread.IsAlive Then LoadRomThread.Abort() ' Abort the old thread if it exists
        LoadRomThread = New Thread(AddressOf LoadRom_Thread)
        LoadRomThread.IsBackground = True
        LoadRomThread.Start(RomPath)
    End Sub

    Private Sub LoadRom_Thread(ByVal RomPath As String)
        Try

            Dim DialogLoopSleepTimer = 0 ' Little buffer that will go higher and higher to compensate for slower PCs
            Dim FoundTheRightWindow = False
            Dim MainHwnd = &H0
            Dim d = &H0
            Dim d_c = &H0
            Dim D_c_c = &H0
            Dim d_c_c_e = &H0
            Dim d_b = &H0

            While Not MainformRef.IsNullDCRunning
                Thread.Sleep(5)
            End While

            NullDCproc.WaitForInputIdle() ' Wait for NullDC to be open and idle
            MainHwnd = NullDCproc.MainWindowHandle ' Get the Main nullDC Window Handle
            PostMessage(MainHwnd, WM_COMMAND, &H17, 0) ' Send the open normal boot message

            While Not FoundTheRightWindow ' Wait untill the dialog opens and you found it's handle
                DialogLoopSleepTimer += 1
                Console.WriteLine("A: {0}", DialogLoopSleepTimer)
                Thread.Sleep(DialogLoopSleepTimer)

                d = GetForegroundWindow() 'FindWindowW("#32770", Nothing)
                Thread.Sleep(DialogLoopSleepTimer)

                Dim length = GetWindowTextLength(d)
                Thread.Sleep(DialogLoopSleepTimer)

                Dim sb As New StringBuilder("", length)
                Thread.Sleep(DialogLoopSleepTimer)

                GetWindowText(d, sb, sb.Capacity + 1).ToString()
                Thread.Sleep(DialogLoopSleepTimer)

                d_c = FindWindowEx(d, 0, "ComboBoxEx32", vbNullString)
                Thread.Sleep(DialogLoopSleepTimer)

                D_c_c = FindWindowEx(d_c, 0, "ComboBox", vbNullString)
                Thread.Sleep(DialogLoopSleepTimer)

                d_c_c_e = FindWindowEx(D_c_c, 0, "Edit", vbNullString)
                Thread.Sleep(DialogLoopSleepTimer)

                d_b = GetDlgItem(d, 1)
                Thread.Sleep(DialogLoopSleepTimer)

                If Not d_c = 0 And Not D_c_c = 0 And Not d_c_c_e = 0 And Not d_b = 0 Then
                    Thread.Sleep(50)
                    FoundTheRightWindow = True
                End If

                If DialogLoopSleepTimer > 2000 Then
                    MsgBox("Rom Loader Failed")
                    NullDCproc.CloseMainWindow()
                    Exit Sub
                End If

            End While

            'DialogLoopSleepTimer = 0

            Dim BoxWritenIn = False
            While Not BoxWritenIn
                DialogLoopSleepTimer += 1
                Console.WriteLine("B: {0}", DialogLoopSleepTimer)
                Thread.Sleep(DialogLoopSleepTimer)
                SendMessage(d_c_c_e, WM_SETTEXT, Len(RomPath) + 1, RomPath)
                Thread.Sleep(DialogLoopSleepTimer)

                Dim TextLen As Integer = SendMessage(d_c_c_e, WM_GETTEXTLENGTH, 0, 0) + 1
                Thread.Sleep(DialogLoopSleepTimer)
                Dim Buffer As String = New String(" "c, TextLen)
                Thread.Sleep(DialogLoopSleepTimer)
                SendMessageByString(d_c_c_e, WM_GETTEXT, TextLen, Buffer)
                Thread.Sleep(DialogLoopSleepTimer)
                If Buffer.Contains(RomPath) Then
                    Thread.Sleep(50)
                    SendMessage(d_b, BM_CLICK, IntPtr.Zero, IntPtr.Zero)
                    BoxWritenIn = True
                End If
                Thread.Sleep(DialogLoopSleepTimer)

                If DialogLoopSleepTimer > 2000 Then
                    MsgBox("Rom Loader Failed")
                    NullDCproc.CloseMainWindow()
                    Exit Sub
                End If

            End While

            GameLaunched(RomPath)

        Catch ex As Exception
            MsgBox("Rom Loader Failed, woops.")
        End Try

    End Sub

    Public Sub New(ByVal mf As frmMain)
        MainFormRef = mf
    End Sub

    Public Sub LaunchDC(ByVal RomName As String, ByRef _region As String)
        Region = _region
        If MainFormRef.IsNullDCRunning And SingleInstance Then
            frmMain.NotificationForm.ShowMessage("An Instance of NullDC online is already running.")
            Exit Sub
        Else
            StartEmulator(RomName)
        End If

    End Sub

    Private Sub EmulatorExited()
        Console.Write("Emulator Exited")
        While MainformRef.IsNullDCRunning
            Thread.Sleep(10)
        End While

        RestoreNvmem()

        If Not DoNotSendNextExitEvent Then
            If Not MainFormRef.Challenger Is Nothing And (MainFormRef.ConfigFile.Status = "Hosting" Or MainFormRef.ConfigFile.Status = "Client") Then
                Console.WriteLine("Telling challenger i quit")
                MainFormRef.NetworkHandler.SendMessage(">,E", MainFormRef.Challenger.ip)
            End If

            Dim INVOKATION As EndSession_delegate = AddressOf MainFormRef.EndSession
            MainFormRef.Invoke(INVOKATION, {"Window Closed", Nothing})

        End If
        DoNotSendNextExitEvent = False

        ' Set State Back to None and Idle since Emulator Closed
        MainformRef.RemoveChallenger()
        MainformRef.ConfigFile.Game = "None"
        MainformRef.ConfigFile.Status = MainformRef.ConfigFile.AwayStatus
        MainformRef.ConfigFile.SaveFile()
        P1Name = ""
        P2Name = ""
    End Sub

    Private Sub StartEmulator(ByVal RomName As String)
        ChangeSettings()

        NullDCproc = Process.Start(MainformRef.NullDCPath & "\nullDC_Win32_Release-NoTrace.exe")
        NullDCproc.EnableRaisingEvents = True
        AddHandler NullDCproc.Exited, AddressOf EmulatorExited
        LoadRom(MainformRef.NullDCPath & MainformRef.GamesList(RomName)(1))

    End Sub

    Private Shared Sub OutputHandler(sendingProcess As Object, outLine As DataReceivedEventArgs)
        If Not String.IsNullOrEmpty(outLine.Data) Then
            Console.WriteLine(outLine.Data)
        End If
    End Sub

    Private Sub GameLaunched(ByVal FullRomPath)
        ' If we're a host then send out call to my partner to join
        If MainformRef.ConfigFile.Status = "Hosting" And Not MainformRef.Challenger Is Nothing Then
            Rx.EEPROM = Rx.GetEEPROM(FullRomPath) ' Save EEPROM for sending to spectators or if we're just hosting solo and waiting.
            MainformRef.NetworkHandler.SendMessage("$," & MainformRef.ConfigFile.Name & "," & MainformRef.ConfigFile.IP & "," & MainformRef.ConfigFile.Port & "," & MainformRef.ConfigFile.Game & "," & MainformRef.ConfigFile.Delay & "," & Region & ",eeprom," & Rx.EEPROM, MainformRef.Challenger.ip)
        End If

        ' Game is loaded, might as well delete the boot don't need it anymore
        If File.Exists(MainformRef.NullDCPath & "\data\naomi_boot.bin") Then
            File.SetAttributes(MainformRef.NullDCPath & "\data\naomi_boot.bin", FileAttributes.Normal)
            File.Delete(MainformRef.NullDCPath & "\data\naomi_boot.bin")

        End If

    End Sub

    Private Sub RestoreNvmem() ' Mostly so it doesn't fuck up blue's launcher
        Dim nvmemPath = MainFormRef.NullDCPath & "\data\naomi_nvmem.bin"
        Dim nvmemPathBackup = MainFormRef.NullDCPath & "\data\naomi_nvmem.bin_backup"

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
        Dim nvmemPath = MainFormRef.NullDCPath & "\data\naomi_nvmem.bin"
        Dim nvmemPathBackup = MainFormRef.NullDCPath & "\data\naomi_nvmem.bin_backup"

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
        ' Boot Dealing
        Dim naomi_boot_Path As String = MainFormRef.NullDCPath & "\data\naomi_boot.bin"
        Dim naomi_boot_Path_Inactive As String = MainFormRef.NullDCPath & "\data\naomi_boot.bin.inactive"

        If File.Exists(naomi_boot_Path) Then
            If File.Exists(naomi_boot_Path_Inactive) Then

                File.SetAttributes(naomi_boot_Path_Inactive, FileAttributes.Normal)
                File.Delete(naomi_boot_Path_Inactive)

                File.SetAttributes(naomi_boot_Path, FileAttributes.Normal)
                My.Computer.FileSystem.RenameFile(naomi_boot_Path, "naomi_boot.bin.inactive")

            Else
                File.SetAttributes(naomi_boot_Path, FileAttributes.Normal)
                My.Computer.FileSystem.RenameFile(naomi_boot_Path, "naomi_boot.bin.inactive")

            End If

        End If

        ' Make sure there's SOME kinda bios in there
        Dim naomi_bios_path As String = MainFormRef.NullDCPath & "\data\naomi_bios.bin"
        If Not File.Exists(naomi_bios_path) Then MainFormRef.UnzipResToDir(My.Resources.bj, "naomi_bios.bin", MainFormRef.NullDCPath & "\data")

        ' Now that we've taken care of BM's launcher bios stuff, lets check our own bios and what we need to do
        If Not Region = "JPN" Then ' Only check if it's NOT the JPN bios because those are the ones that use naomi_boot
            Dim BiosToUse = My.Resources.bu
            If Region = "EUR" Then BiosToUse = My.Resources.be
            MainFormRef.UnzipResToDir(BiosToUse, "naomi_boot.bin", MainFormRef.NullDCPath & "\data")

        End If

    End Sub

    Private Sub ChangeSettings()
        ' Check Configs for BEARJamma data

        ' Set the Playernames
        ' naomi_boot.bin.inactive
        DealWithBios()
        BackupNvmem()

        ' Check if this is a Replay
        Dim IsReplay As Int16 = 0
        Dim IsSpectator As Int16 = 0
        Dim IsHosting = "0"

        If Not MainFormRef.ConfigFile.ReplayFile = "" Then IsReplay = 1
        If MainFormRef.ConfigFile.Status = "Spectator" Then IsSpectator = 1
        If MainFormRef.ConfigFile.Status = "Hosting" Then IsHosting = "1"

        ' Get P1/P2 Names
        If P1Name = "" Or P2Name = "" Then ' Player names were not set beforehand 
            If MainFormRef.ConfigFile.Status = "Offline" Or MainFormRef.ConfigFile.Status = "Spectator" Then
                P1Name = MainFormRef.ConfigFile.Name
                P2Name = "Local Player 2"
            Else
                If IsHosting Then
                    P1Name = MainFormRef.ConfigFile.Name
                    If MainFormRef.Challenger Is Nothing Then
                        P2Name = "Solo Host"
                    Else
                        P2Name = MainFormRef.Challenger.name
                    End If

                Else
                    P1Name = MainFormRef.Challenger.name
                    P2Name = MainFormRef.ConfigFile.Name
                End If
            End If

        End If

        ' Wait for nvmem to be dealt with
        Dim SleepTime = 0
        While File.Exists(MainFormRef.NullDCPath & "\data\naomi_nvmem.bin")
            Thread.Sleep(100)
            SleepTime += 100
            If SleepTime > 1000 Then Exit While ' Fuck it just continue
        End While


        Dim thefile = MainFormRef.NullDCPath & "\antilag.cfg"
        Dim FPSLimit = "90"
        Dim FPSLimiter = "0"

        If IsReplay = 1 Or IsSpectator = 1 Then ' Replays and Spectating are always in Audio Sync mode for best clarity plus it allows fast forward at x2 speed
            FPSLimiter = "1"
            FPSLimit = "120" ' Apperanltly replays can't go faster than 90fps
        Else
            If MainFormRef.ConfigFile.Status = "Hosting" Or MainFormRef.ConfigFile.Status = "Offline" Then
                If MainFormRef.ConfigFile.HostType = "0" Then
                    FPSLimiter = "0"
                    FPSLimit = MainFormRef.ConfigFile.FPSLimit
                Else
                    FPSLimiter = "1"
                End If
            End If
        End If

        ' No Longer Need to rewrite the entire file
        File.WriteAllLines(thefile, {"[config]", "RenderAheadLimit=0", "FPSlimit=" & FPSLimit})

        thefile = MainFormRef.NullDCPath & "\nullDC.cfg"
        Dim lines() As String = File.ReadAllLines(thefile)

        Dim linenumber = 0
        For Each line As String In lines

            ' [nullDC]
            If line.StartsWith("Dynarec.Enabled=") Then lines(linenumber) = "Dynarec.Enabled=1"
            If line.StartsWith("Dynarec.DoConstantPropagation=") Then lines(linenumber) = "Dynarec.DoConstantPropagation=1"
            If line.StartsWith("Dynarec.UnderclockFpu=") Then lines(linenumber) = "Dynarec.UnderclockFpu=0"
            If line.StartsWith("Dreamcast.Cable=") Then lines(linenumber) = "Dreamcast.Cable=1"
            If line.StartsWith("Dreamcast.RTC=") Then lines(linenumber) = "Dreamcast.RTC=1917526710" ' 1917526710
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

            If line.StartsWith("Current_maple0_5=") Then
                lines(linenumber) = "Current_maple0_5=BEARJamma_Win32.dll:0" ' Make sure that BEAR  is only plugin
                lines(linenumber + 1) = "Current_maple1_5=NULL"
                lines(linenumber + 2) = "Current_maple2_5=NULL"
                lines(linenumber + 3) = "Current_maple3_5=NULL"
            End If


            ' Change Netplay Shit
            If line.StartsWith("[BEARPlay]") Then
                Dim EnableOnline = "0"
                If Not MainFormRef.ConfigFile.Status = "Offline" Then EnableOnline = "1"

                lines(linenumber + 1) = "Online=" & EnableOnline
                lines(linenumber + 2) = "Host=" & MainFormRef.ConfigFile.Host
                lines(linenumber + 3) = "Hosting=" & IsHosting
                lines(linenumber + 4) = "Port=" & MainFormRef.ConfigFile.Port
                lines(linenumber + 5) = "Delay=" & MainFormRef.ConfigFile.Delay
                lines(linenumber + 6) = "Record=" & MainFormRef.ConfigFile.RecordReplay
                lines(linenumber + 7) = "Playback=" & IsReplay
                lines(linenumber + 8) = "AllowSpectators=" & MainFormRef.ConfigFile.AllowSpectators
                lines(linenumber + 9) = "Spectator=" & IsSpectator
                lines(linenumber + 10) = "P1Name=" & P1Name
                lines(linenumber + 11) = "P2Name=" & P2Name
                lines(linenumber + 12) = "File=" & MainFormRef.ConfigFile.ReplayFile
                lines(linenumber + 13) = "GameName=" & MainFormRef.GamesList(MainFormRef.ConfigFile.Game)(0)
                lines(linenumber + 14) = "GameRom=" & MainFormRef.ConfigFile.Game
                lines(linenumber + 15) = "Region=" & Region
            End If

            ' Audio Sync on host only
            If line.StartsWith("Emulator.NoConsole=") Then
                Dim con As String = "1"
                If MainFormRef.ConfigFile.Status = "Hosting" Or MainFormRef.ConfigFile.Status = "Offline" Then con = "0"
                lines(linenumber) = "Emulator.NoConsole=0" '& con
            End If

            linenumber += 1
        Next

        'Emulator.NoConsole
        File.WriteAllLines(thefile, lines)

        ' Stuff to help Casters
        File.WriteAllLines(MainFormRef.NullDCPath & "\replays\Player1Name.txt", {P1Name})
        File.WriteAllLines(MainFormRef.NullDCPath & "\replays\Player2Name.txt", {P2Name})

    End Sub

End Class