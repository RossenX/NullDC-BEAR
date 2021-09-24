Imports System.IO
Imports System.Threading
Imports NullDC_CvS2_BEAR.frmMain
Imports SDL2.SDL
Public Class MFlycastLauncher
    Public FlycastProc As Process
    Dim LoadRomThread As Thread
    Public Region As String

    Public Sub LoadGame()
        If Not LoadRomThread Is Nothing Then If LoadRomThread.IsAlive Then LoadRomThread.Abort() ' Abort the old thread if it exists
        LoadRomThread = New Thread(AddressOf LoadGame_Thread)
        LoadRomThread.IsBackground = True
        LoadRomThread.Start()

    End Sub

    Private Sub LoadGame_Thread()
        Try
            'FlycastProc.WaitForInputIdle()
            GameLoaded()

        Catch ex As Exception
            MsgBox("Romloader error: " & ex.Message)

        End Try

    End Sub

    Private Sub GameLoaded()
        ' If we're a host then send out call to my partner to join
        Console.WriteLine("Game Launched")
        If MainformRef.ConfigFile.Status = "Hosting" And MainformRef.Challenger IsNot Nothing Then
            ' eeprom causing issues, so not going to sync them for now.
            MainformRef.NetworkHandler.SendMessage("$," & MainformRef.ConfigFile.Name & ",," & MainformRef.ConfigFile.Port & "," & MainformRef.ConfigFile.Game & "," & MainformRef.ConfigFile.Delay & "," & Region & "," & MainformRef.ConfigFile.Peripheral & ",eeprom,", MainformRef.Challenger.ip)

        End If

    End Sub

    Public Sub LaunchFlycast(ByVal _romname As String, ByRef _region As String)

        ' Tell difference between dreamcast and naomi
        Region = _region

        If _romname.StartsWith("FC_") Then
            _romname = _romname.Remove(0, 3)
        End If

        RemoveAllTheShit()

        Dim FlycastInfo As New ProcessStartInfo
        FlycastInfo.FileName = MainformRef.NullDCPath & "\flycast\flycast.exe"
        Dim romdetails = MainformRef.GamesList(_romname)

        ' Bunch of bullshit here

        Dim ForceMono = "no"
        If MainformRef.ConfigFile.ForceMono = 1 Then ForceMono = "yes"
        Dim PlayerID = 0
        If MainformRef.ConfigFile.Status = "Client" Then PlayerID = 1

        Dim Vsync = "no"
        If MainformRef.ConfigFile.Vsync = 1 Then Vsync = "yes"
        'FlycastInfo.Arguments += "-config config:rend.vsync=" & Vsync & " "

        Dim CheckIfCreated As String() = {"GGPOAnalogAxes", "Stats", "maple_sdl_joystick_0", "maple_sdl_joystick_1", "maple_sdl_joystick_2", "maple_sdl_joystick_3", "maple_sdl_joystick_4", "p1Name", "p2Name"}
        Dim InSection As String() = {"network", "network", "input", "input", "input", "input", "input", "network", "network"}
        Dim ValueDefault As String() = {"2", "yes", "-1", "-1", "-1", "-1", "-1", "Player 1", "Player 2"}
        Dim WereCreated As Boolean() = {False, False, False, False, False, False, False, False, False}

        Dim ControlsFile = File.ReadAllLines(GetControlsFilePath())

        Dim tempPeripheral As String() = {"", ""}
        Dim tempJoystick As String() = {"", ""}

        For Each line As String In ControlsFile
            If line.StartsWith("Peripheral=") Then
                tempPeripheral(0) = (CInt(line.Split("=")(1).Split("|")(0)) * 4).ToString
                tempPeripheral(1) = (CInt(line.Split("=")(1).Split("|")(1)) * 4).ToString
            End If
            If line.StartsWith("Joystick=") Then
                tempJoystick(0) = line.Split("=")(1).Split("|")(0)
                tempJoystick(1) = line.Split("=")(1).Split("|")(1)
            End If
        Next

        Dim WeArePort = 0
        'We're port A
        If MainformRef.ConfigFile.Status = "Hosting" Or MainformRef.ConfigFile.Status = "Offline" Then
            WeArePort = 0
        Else ' We're port B
            WeArePort = 1
        End If

ReDoConfigs:

        'Some settings are locked some are not, these are not or maybe they're settings i know won't cause desync so i let people change
        Dim lines() As String = File.ReadAllLines(MainformRef.NullDCPath & "\flycast\emu.cfg")

        For Each line As String In lines
            For i = 0 To CheckIfCreated.Count - 1
                If line.StartsWith(CheckIfCreated(i)) Then WereCreated(i) = True
            Next
        Next

        Dim linenumber = 0
        For Each line As String In lines

            For i = 0 To CheckIfCreated.Count - 1
                If WereCreated(i) = False Then
                    If line.Trim = "[" & InSection(i) & "]" Then
                        lines(linenumber) = lines(linenumber) & vbNewLine & CheckIfCreated(i) & " = " & ValueDefault(i)
                        File.WriteAllLines(MainformRef.NullDCPath & "\flycast\emu.cfg", lines)
                        GoTo ReDoConfigs

                    End If
                End If
            Next

            If line.StartsWith("p1Name = ") Then
                If MainformRef.ConfigFile.Status = "Hosting" Then
                    lines(linenumber) = "p1Name = " & MainformRef.ConfigFile.Name
                ElseIf MainformRef.ConfigFile.Status = "Client" Then
                    lines(linenumber) = "p1Name = " & MainformRef.Challenger.name
                Else
                    lines(linenumber) = "p1Name = " & MainformRef.ConfigFile.Name
                End If
            End If

            If line.StartsWith("p2Name = ") Then
                If MainformRef.ConfigFile.Status = "Hosting" Then
                    lines(linenumber) = "p2Name = " & MainformRef.Challenger.name
                ElseIf MainformRef.ConfigFile.Status = "Client" Then
                    lines(linenumber) = "p2Name = " & MainformRef.ConfigFile.Name
                Else
                    lines(linenumber) = "p2Name = " & MainformRef.ConfigFile.P2Name
                End If
            End If

            If line.StartsWith("rend.vsync = ") Then lines(linenumber) = "rend.vsync = " & Vsync

            'For Now set them all To controller, since the savestates use two controllers. When savestatesa re all removed,i'll enable this
            If line.StartsWith("device1 = ") Then
                If romdetails(2) = "na" Or romdetails(2) = "fly_na" Then ' If this is Naomi then it's always the dreamcast controller
                    lines(linenumber) = "device1 = 0"

                Else
                    If MainformRef.ConfigFile.Status = "Offline" Then ' This is Offline
                        lines(linenumber) = "device1 = " & tempPeripheral(0)
                    ElseIf MainformRef.ConfigFile.Status = "Hosting" Then ' We're Hosting
                        lines(linenumber) = "device1 = " & tempPeripheral(0)
                    Else ' We're client (Since spectating isn't a thing yet
                        lines(linenumber) = "device1 = " & CInt(MainformRef.Challenger.peripheral) * 4
                    End If
                End If

            End If

            'If line.StartsWith("device1 = ") Then lines(linenumber) = "device1 = 0"
            If line.StartsWith("device1.1 = ") Then lines(linenumber) = "device1.1 = 1"
            If line.StartsWith("device1.2 = ") Then lines(linenumber) = "device1.2 = 10"

            If line.StartsWith("device2 = ") Then
                If romdetails(2) = "na" Or romdetails(2) = "fly_na" Then
                    lines(linenumber) = "device2 = 0"

                Else
                    If MainformRef.ConfigFile.Status = "Offline" Then
                        lines(linenumber) = "device2 = " & tempPeripheral(1)
                    ElseIf MainformRef.ConfigFile.Status = "Hosting" Then
                        lines(linenumber) = "device2 = " & CInt(MainformRef.Challenger.peripheral) * 4
                    Else
                        lines(linenumber) = "device2 = " & tempPeripheral(0)
                    End If
                End If

            End If

            'If line.StartsWith("device2 = ") Then lines(linenumber) = "device2 = 0"
            If line.StartsWith("device2.1 = ") Then lines(linenumber) = "device2.1 = 10"
                If line.StartsWith("device2.2 = ") Then lines(linenumber) = "device2.2 = 10"

                If line.StartsWith("device3 = ") Then lines(linenumber) = "device3 = 10"
                If line.StartsWith("device3.1 = ") Then lines(linenumber) = "device3.1 = 10"
                If line.StartsWith("device3.2 = ") Then lines(linenumber) = "device3.2 = 10"

                If line.StartsWith("device4 = ") Then lines(linenumber) = "device4 = 10"
                If line.StartsWith("device4.1 = ") Then lines(linenumber) = "device4.1 = 10"
                If line.StartsWith("device4.2 = ") Then lines(linenumber) = "device4.2 = 10"

                ' This one is here only so people can still edit it in game
                If line.StartsWith("aica.AudioVolume = ") Then lines(linenumber) = "aica.AudioVolume = " & MainformRef.ConfigFile.EmulatorVolume
                If line.StartsWith("aica.ForceMono = ") Then lines(linenumber) = "aica.ForceMono = " & ForceMono

                If line.StartsWith("maple_sdl_") Then
                    lines(linenumber) = lines(linenumber).Split("=")(0).Trim & " = " & PlayerID
                End If

                If line.StartsWith("Dreamcast.AutoLoadState = ") Then lines(linenumber) = "Dreamcast.AutoLoadState = yes"
                'Dreamcast.SavestateSlot = 0
                If line.StartsWith("Dreamcast.SavestateSlot = ") Then lines(linenumber) = "Dreamcast.SavestateSlot = 0"

                If line.StartsWith("pvr.AutoSkipFrame = ") Then lines(linenumber) = "pvr.AutoSkipFrame = 0"

                If line.StartsWith("rend.CrossHairColor1 = ") Then lines(linenumber) = "rend.CrossHairColor1 = 0"
                If line.StartsWith("rend.CrossHairColor2 = ") Then lines(linenumber) = "rend.CrossHairColor2 = 0"
                If line.StartsWith("rend.CrossHairColor3 = ") Then lines(linenumber) = "rend.CrossHairColor3 = 0"
                If line.StartsWith("rend.CrossHairColor4 = ") Then lines(linenumber) = "rend.CrossHairColor4 = 0"

                If line.StartsWith("GGPOAnalogAxes = ") Then
                    Select Case romdetails(2)
                        Case "na", "fc_na", "fly_na"
                            lines(linenumber) = "GGPOAnalogAxes = 0"
                        Case Else
                            lines(linenumber) = "GGPOAnalogAxes = 2"
                    End Select

                End If

                If MainformRef.ConfigFile.Status = "Offline" Then
                    If line.StartsWith("GGPO = ") Then lines(linenumber) = "GGPO = no"
                Else
                    If line.StartsWith("GGPO = ") Then lines(linenumber) = "GGPO = yes"

                End If

                ' Set Joysticks

                If line.StartsWith("maple_sdl_joystick_") Then

                    If line.StartsWith("maple_sdl_joystick_" & tempJoystick(0)) Then ' This is our Controller we want to use
                        lines(linenumber) = "maple_sdl_joystick_" & tempJoystick(0) & " = " & WeArePort

                    ElseIf line.StartsWith("maple_sdl_joystick_" & tempJoystick(1)) Then ' This is the second player controller, for offline we set it for online we don't

                        If MainformRef.ConfigFile.Status = "Offline" Then
                            lines(linenumber) = "maple_sdl_joystick_" & tempJoystick(1) & " = 1"
                        Else
                            lines(linenumber) = "maple_sdl_joystick_" & tempJoystick(1) & " = -1"
                        End If
                    Else

                        lines(linenumber) = line.Split("=")(0) & " = -1"
                    End If

                End If

                If line.StartsWith("maple_sdl_keyboard =") Then lines(linenumber) = "maple_sdl_keyboard = " & WeArePort
                If line.StartsWith("maple_sdl_mouse =") Then lines(linenumber) = "maple_sdl_mouse = -1" ' No Mouse For Now


                linenumber += 1
            Next

            ' Check if created stuff and if not then create them

            File.WriteAllLines(MainformRef.NullDCPath & "\flycast\emu.cfg", lines)

        FlycastInfo.Arguments += "-config config:rend.DelayFrameSwapping=no "
        ' FlycastInfo.Arguments += "-config config:rend.ThreadedRendering=no " Ok so this causes instant crash

        FlycastInfo.Arguments += "-config config:rend.UseMipmaps=no "

        Dim __Region = 0
        Dim Broadcast = 0
        Select Case _region
            Case "JPN"
                __Region = 0
            Case "USA"
                __Region = 1
            Case "EUR"
                __Region = 2
                Broadcast = 1
        End Select

        FlycastInfo.Arguments += "-config config:Dynarec.idleskip=yes "
        FlycastInfo.Arguments += "-config config:Dreamcast.Region=" & __Region & " "
        FlycastInfo.Arguments += "-config config:Dreamcast.Broadcast=" & Broadcast & " "
        FlycastInfo.Arguments += "-config config:Dreamcast.Language=1 "
        FlycastInfo.Arguments += "-config config:FastGDRomLoad=no "

        FlycastInfo.Arguments += "-config network:pvr.AutoSkipFrame=0 "

        ' Enable GGPO if it's online
        If MainformRef.ConfigFile.Status = "Offline" Then
            FlycastInfo.Arguments += "-config network:ActAsServer=no "
            ' FlycastInfo.Arguments += "-config network:DNS= "
            FlycastInfo.Arguments += "-config network:EmulateBBA=no "
            FlycastInfo.Arguments += "-config network:Enable=no "
            FlycastInfo.Arguments += "-config network:server= "

            ' Copy the VMU from NullDC to Flycast
            If File.Exists(MainformRef.NullDCPath & "\dc\vmu_data_host.bin") Then
                If romdetails(2) = "dc" Or romdetails(2) = "fly_dc" Then
                    FileSystem.FileCopy(MainformRef.NullDCPath & "\dc\vmu_data_host.bin", MainformRef.NullDCPath & "\flycast\data\vmu_save_A1.bin")
                    FlycastInfo.Arguments += "-config config:GGPOAnalogAxes=2 "

                Else
                    FlycastInfo.Arguments += "-config config:GGPOAnalogAxes=0 "
                End If
            End If

        Else

            ' Come stuff that should probably never be on while playing ONLINE
            FlycastInfo.Arguments += "-config config:rend.CustomTextures=no "
            FlycastInfo.Arguments += "-config config:rend.DumpTextures=no "

            ' This is Online lets check if we're the host
            FlycastInfo.Arguments += "-config network:GGPO=yes "
            FlycastInfo.Arguments += "-config network:GGPODelay=" & MainformRef.ConfigFile.Delay & " "

            ' This has to be forced of it'll cause desyncs
            FlycastInfo.Arguments += "-config config:rend.LimitFPS=yes "

            ' Yeah no automatic states when playing online cuz who knows whatafak those states are
            ' FlycastInfo.Arguments += "-config config:Dreamcast.AutoLoadState=no "
            ' FlycastInfo.Arguments += "-config config:Dreamcast.AutoSaveState=no "


            If MainformRef.ConfigFile.Status = "Hosting" Then
                FlycastInfo.Arguments += "-config network:ActAsServer=yes "
                FlycastInfo.Arguments += "-config network:server=" & MainformRef.Challenger.ip & " "

                ' Copy the VMU from NullDC to Flycast
                If File.Exists(MainformRef.NullDCPath & "\dc\vmu_data_host.bin") Then
                    If romdetails(2) = "dc" Or romdetails(2) = "fly_dc" Then
                        FileSystem.FileCopy(MainformRef.NullDCPath & "\dc\vmu_data_host.bin", MainformRef.NullDCPath & "\flycast\data\vmu_save_A1.bin")
                    End If
                End If


            Else
                FlycastInfo.Arguments += "-config network:ActAsServer=no "
                FlycastInfo.Arguments += "-config network:server=" & MainformRef.ConfigFile.Host & " "

                ' Copy the VMU from NullDC to Flycast
                If File.Exists(MainformRef.NullDCPath & "\dc\vmu_data_client.bin") Then
                    If romdetails(2) = "dc" Or romdetails(2) = "fly_dc" Then
                        FileSystem.FileCopy(MainformRef.NullDCPath & "\dc\vmu_data_client.bin", MainformRef.NullDCPath & "\flycast\data\vmu_save_A1_client.bin")
                    End If
                End If


            End If

        End If

        If MainformRef.ConfigFile.Game.Split("-")(0).ToLower = "fc_dc" Then
            FlycastInfo.Arguments += """" & MainformRef.NullDCPath & "\dc\" & romdetails(1) & """"
        ElseIf MainformRef.ConfigFile.Game.Split("-")(0).ToLower = "fc_na" Then
            FlycastInfo.Arguments += """" & MainformRef.NullDCPath & romdetails(1) & """"
        Else
            FlycastInfo.Arguments += """" & MainformRef.NullDCPath & romdetails(1) & """"
        End If

        Console.WriteLine("Flycast launched: " & FlycastInfo.Arguments)
        FlycastProc = Process.Start(FlycastInfo)
        FlycastProc.EnableRaisingEvents = True

        AddHandler FlycastProc.Exited, AddressOf FlycastProcExited
        LoadGame()

    End Sub

    Private Sub RemoveAllTheShit()

        For Each file In Directory.GetFiles(MainformRef.NullDCPath & "\flycast\data", "*")
            If file.Contains("nvmem") Then
                SafeDeleteFile(file)
            End If

        Next

        Dim _romname = MainformRef.ConfigFile.Game
        If _romname.ToLower.StartsWith("fc_") Then
            _romname = _romname.Remove(0, 3)
        End If

        Dim romdetails = MainformRef.GamesList(_romname)

        Dim FullRomPath = ""
        If MainformRef.ConfigFile.Game.Split("-")(0).ToLower = "fc_dc" Then
            FullRomPath = MainformRef.NullDCPath & "\dc\" & romdetails(1)
        ElseIf MainformRef.ConfigFile.Game.Split("-")(0).ToLower = "fc_na" Then
            FullRomPath = MainformRef.NullDCPath & romdetails(1)
        Else
            FullRomPath = MainformRef.NullDCPath & romdetails(1)
        End If

        If File.Exists(FullRomPath & ".nvmem") Then
            File.Delete(FullRomPath & ".nvmem")
        End If

        If File.Exists(FullRomPath & ".nvmem2") Then
            File.Delete(FullRomPath & ".nvmem2")
        End If

        If File.Exists(FullRomPath & ".eeprom_host") Then
            File.Delete(FullRomPath & ".eeprom_host")
        End If

        If File.Exists(FullRomPath & ".eeprom_client") Then
            File.Delete(FullRomPath & ".eeprom_client")
        End If

    End Sub

    Private Sub FlycastProcExited()

        FlycastProc = Nothing

        Rx.EEPROM = ""
        Rx.VMU = Nothing
        Rx.VMUPieces = New ArrayList From {"", "", "", "", "", "", "", "", "", ""}

        If Not MainformRef.Challenger Is Nothing And (MainformRef.ConfigFile.Status = "Hosting" Or MainformRef.ConfigFile.Status = "Client") Then
            Console.WriteLine("Telling challenger i quit")
            MainformRef.NetworkHandler.SendMessage(">,E", MainformRef.Challenger.ip)
        End If

        If Not MainformRef.IsClosing Then
            Dim INVOKATION As EndSession_delegate = AddressOf MainformRef.EndSession
            MainformRef.Invoke(INVOKATION, {"Window Closed", Nothing})
        End If

    End Sub

End Class
