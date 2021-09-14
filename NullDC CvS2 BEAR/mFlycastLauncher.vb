Imports System.IO
Imports System.Threading
Imports NullDC_CvS2_BEAR.frmMain

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

        'Some settings are locked some are not, these are not or maybe they're settings i know won't cause desync so i let people change
        Dim lines() As String = File.ReadAllLines(MainformRef.NullDCPath & "\flycast\emu.cfg")
        Dim linenumber = 0
        For Each line As String In lines

            If line.StartsWith("rend.vsync = ") Then lines(linenumber) = "rend.vsync = " & Vsync

            If line.StartsWith("device1 = ") Then lines(linenumber) = "device1 = 0"
            If line.StartsWith("device1.1 = ") Then lines(linenumber) = "device1.1 = 1"
            If line.StartsWith("device1.2 = ") Then lines(linenumber) = "device1.2 = 10"

            If line.StartsWith("device2 = ") Then lines(linenumber) = "device2 = 0"
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

            If line.StartsWith("maple_sdl_") Then lines(linenumber) = lines(linenumber).Split("=")(0).Trim & " = " & PlayerID

            If line.StartsWith("Dreamcast.AutoLoadState = ") Then lines(linenumber) = "Dreamcast.AutoLoadState = yes"
            'Dreamcast.SavestateSlot = 0
            If line.StartsWith("Dreamcast.SavestateSlot = ") Then lines(linenumber) = "Dreamcast.SavestateSlot = 0"

            If MainformRef.ConfigFile.Status = "Offline" Then
                If line.StartsWith("GGPO = ") Then lines(linenumber) = "GGPO = no"
            Else
                If line.StartsWith("GGPO = ") Then lines(linenumber) = "GGPO = yes"
            End If
            linenumber += 1

        Next

        File.WriteAllLines(MainformRef.NullDCPath & "\flycast\emu.cfg", lines)

        ' FlycastInfo.Arguments += "-config config:rend.DelayFrameSwapping=no "
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
