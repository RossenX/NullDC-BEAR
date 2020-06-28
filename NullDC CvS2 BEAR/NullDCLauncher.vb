Imports System.IO
Imports System.Runtime.InteropServices
Imports NullDC_CvS2_BEAR.frmMain
Imports System.Threading
Imports System.Globalization
Imports System.Text

Public Class NullDCLauncher


    Public NullDCproc As Process
    Dim AutoHotkey As Process
    Dim SingleInstance As Boolean = True
    Public DoNotSendNextExitEvent As Boolean

    Dim MainFormRef As frmMain
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
        Dim DialogLoopSleepTimer = 0 ' Little buffer that will go higher and higher to compensate for slower PCs
        Dim FoundTheRightWindow = False
        Dim MainHwnd = &H0
        Dim d = &H0
        Dim d_c = &H0
        Dim D_c_c = &H0
        Dim d_c_c_e = &H0
        Dim d_b = &H0

        NullDCproc.WaitForInputIdle() ' Wait for NullDC to be open and idle
        MainHwnd = NullDCproc.MainWindowHandle ' Get the Main nullDC Window Handle
        PostMessage(MainHwnd, WM_COMMAND, &H17, 0) ' Send the open normal boot message

        While Not FoundTheRightWindow ' Wait untill the dialog opens and you found it's handle
            DialogLoopSleepTimer += 5
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
            DialogLoopSleepTimer += 5
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

        GameLaunched()

    End Sub

    Public Sub New(ByVal mf As frmMain)
        MainFormRef = mf
    End Sub

    Public Sub LaunchDC(ByVal RomName As String)
        If MainFormRef.IsNullDCRunning And SingleInstance Then
            frmMain.NotificationForm.ShowMessage("An Instance of NullDC online is already running.")
            Exit Sub
        Else
            StartEmulator(RomName)
        End If

    End Sub

    Private Sub EmulatorExited()
        Console.Write("Emulator Exited")
        While MainFormRef.IsNullDCRunning
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

    End Sub

    Private Function StartEmulator(ByVal RomName As String) As Boolean
        ' Override settings to new ones to make sure everyone matches
        File.WriteAllBytes(MainFormRef.NullDCPath & "\nullDC.cfg", My.Resources.nullDC)
        ' Change what settings need changing inthe cfg
        ChangeSettings()

        NullDCproc = Process.Start(MainFormRef.NullDCPath & "\nullDC_Win32_Release-NoTrace.exe")
        NullDCproc.EnableRaisingEvents = True
        AddHandler NullDCproc.Exited, AddressOf EmulatorExited
        LoadRom(MainFormRef.NullDCPath & MainFormRef.GamesList(RomName)(1))
        Return True

    End Function

    Private Shared Sub OutputHandler(sendingProcess As Object, outLine As DataReceivedEventArgs)
        If Not String.IsNullOrEmpty(outLine.Data) Then
            Console.WriteLine(outLine.Data)
        End If
    End Sub

    Private Sub GameLaunched()
        ' If we're a host then send out call to my partner to join
        If MainFormRef.ConfigFile.Status = "Hosting" And Not MainFormRef.Challenger Is Nothing Then
            MainFormRef.NetworkHandler.SendMessage("$," & MainFormRef.ConfigFile.Name & "," & MainFormRef.ConfigFile.IP & "," & MainFormRef.ConfigFile.Port & "," & MainFormRef.ConfigFile.Game & "," & MainFormRef.ConfigFile.Delay, MainFormRef.Challenger.ip)
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

    End Sub

    Private Sub BackupNvmem()
        ' Fuck nvMEM get rid of that shit
        Dim nvmemPath = MainFormRef.NullDCPath & "\data\naomi_nvmem.bin"
        Dim nvmemPathBackup = MainFormRef.NullDCPath & "\data\naomi_nvmem.bin_backup"

        Try
            If File.Exists(nvmemPath) Then ' nvmem exists... shocker.

                If File.Exists(nvmemPathBackup) Then ' Backup Exists nullDC probably crashed or something
                    ' Delete w.e nvmem is currenly in there, leave backup
                    File.SetAttributes(nvmemPath, FileAttributes.Normal)
                    File.Delete(nvmemPath)

                Else
                    ' No backup, so backup the nvmem
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

    End Sub

    Private Sub ChangeSettings()

        ' naomi_boot.bin.inactive
        DealWithBios()
        BackupNvmem()

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
        If MainFormRef.ConfigFile.Status = "Hosting" Or MainFormRef.ConfigFile.Status = "Offline" Then
            If MainFormRef.ConfigFile.HostType = "0" Then
                FPSLimiter = "0"
                FPSLimit = MainFormRef.ConfigFile.FPSLimit
            Else
                FPSLimiter = "2"
            End If
        End If
        File.WriteAllLines(thefile, {"[config]", "RenderAheadLimit=0", "FPSlimit=" & FPSLimit})

        thefile = MainFormRef.NullDCPath & "\nullDC.cfg"
        Dim lines() As String = File.ReadAllLines(thefile)

        Dim linenumber = 0
        For Each line As String In lines

            ' Change Netplay Shit
            If line.StartsWith("[Netplay]") Then
                Dim EnableOnline = "0"
                Dim IsHosting = "0"
                If MainFormRef.ConfigFile.Status = "Hosting" Then IsHosting = "1"
                If Not MainFormRef.ConfigFile.Status = "Offline" Then EnableOnline = "1"
                lines(linenumber + 1) = "Enabled=" & EnableOnline
                lines(linenumber + 2) = "Hosting=" & IsHosting
                lines(linenumber + 3) = "Host=" & MainFormRef.ConfigFile.Host
                lines(linenumber + 4) = "Port=" & MainFormRef.ConfigFile.Port
                lines(linenumber + 5) = "Delay=" & MainFormRef.ConfigFile.Delay
            End If

            ' Audio Sync on host only
            If line.StartsWith("[nullAica]") Then
                lines(linenumber + 2) = "LimitFPS=" & FPSLimiter
            End If

            ' Controls
            If line.StartsWith("player1=") Then
                Dim KeyboardOrJoystick As String = "keyboard"
                If frmMain.ConfigFile.UseRemap = "0" Then KeyboardOrJoystick = "joy1"
                lines(linenumber) = "player1=" & KeyboardOrJoystick
            End If
            If line.StartsWith("Emulator.NoConsole=") Then
                Dim con As String = "1"
                If MainFormRef.ConfigFile.Status = "Hosting" Or MainFormRef.ConfigFile.Status = "Offline" Then con = "0"
                lines(linenumber) = "Emulator.NoConsole=" & con
            End If

            linenumber += 1
        Next
        'Emulator.NoConsole
        File.WriteAllLines(thefile, lines)

    End Sub

End Class