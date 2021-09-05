Imports System.IO
Imports NullDC_CvS2_BEAR.frmMain

Public Class MFlycastLauncher
    Public FlycastProc As Process

    Public Sub LaunchFlycast(ByVal _romname As String, ByRef _region As String)

        RemoveAllTheShit()

        Dim FlycastInfo As New ProcessStartInfo
        FlycastInfo.FileName = MainformRef.NullDCPath & "\flycast\flycast.exe"
        Dim romdetails = MainformRef.GamesList(_romname)

        ' Bunch of bullshit here

        'So some of these don't change in the command line so have to do them manually
        Dim lines() As String = File.ReadAllLines(MainformRef.NullDCPath & "\flycast\emu.cfg")
        Dim linenumber = 0
        For Each line As String In lines

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

            linenumber += 1
        Next

        File.WriteAllLines(MainformRef.NullDCPath & "\flycast\emu.cfg", lines)

        FlycastInfo.Arguments += "-config config:rend.DelayFrameSwapping=no "
        FlycastInfo.Arguments += "-config config:rend.DumpTextures=no "
        ' FlycastInfo.Arguments += "-config config:rend.ThreadedRendering=no " Ok so this causes instant crash
        FlycastInfo.Arguments += "-config config:rend.UseMipmaps=no "

        Dim Vsync = "no"
        If MainformRef.ConfigFile.Vsync = 1 Then Vsync = "yes"
        FlycastInfo.Arguments += "-config config:rend.vsync=" & Vsync & " "

        Dim Region = 0
        Dim Broadcast = 0
        Select Case _region
            Case "JPN"
                Region = 0
            Case "USA"
                Region = 1
            Case "EUR"
                Region = 2
                Broadcast = 1
        End Select

        FlycastInfo.Arguments += "-config config:Dreamcast.Region=" & Region & " "
        FlycastInfo.Arguments += "-config config:Dreamcast.Broadcast=" & Broadcast & " "
        FlycastInfo.Arguments += "-config config:Dreamcast.Language=1 "
        FlycastInfo.Arguments += "-config config:FastGDRomLoad=no "

        Dim ForceMono = "no"
        If MainformRef.ConfigFile.ForceMono = 1 Then ForceMono = "yes"
        FlycastInfo.Arguments += "-config config:ForceMono=" & ForceMono & " "

        ' Enable GGPO if it's online
        If MainformRef.ConfigFile.Status = "Offline" Then
            FlycastInfo.Arguments += "-config network:GGPO=no "
            FlycastInfo.Arguments += "-config network:ActAsServer=no "
            ' FlycastInfo.Arguments += "-config network:DNS= "
            FlycastInfo.Arguments += "-config network:EmulateBBA=no "
            FlycastInfo.Arguments += "-config network:Enable=no "
            FlycastInfo.Arguments += "-config network:server= "

            ' Copy the VMU from NullDC to Flycast
            FileSystem.FileCopy(MainformRef.NullDCPath & "\dc\vmu_data_host.bin", MainformRef.NullDCPath & "\flycast\data\vmu_save_A1.bin")

        Else

            ' This is Online lets check if we're the host
            FlycastInfo.Arguments += "-config network:GGPO=yes "
            FlycastInfo.Arguments += "-config network:GGPODelay=" & MainformRef.ConfigFile.SimulatedDelay & " "

            ' Yeah no automatic states when playing online cuz who knows whatafak those states are
            FlycastInfo.Arguments += "-config config:Dreamcast.AutoLoadState=no "
            FlycastInfo.Arguments += "-config config:Dreamcast.AutoSaveState=no "


            If MainformRef.ConfigFile.Status = "Hosting" Then
                FlycastInfo.Arguments += "-config network:ActAsServer=yes "
                FlycastInfo.Arguments += "-config network:server= """ & MainformRef.Challenger.ip & ":" & MainformRef.ConfigFile.Port & """"

                ' Copy the VMU from NullDC to Flycast
                FileSystem.FileCopy(MainformRef.NullDCPath & "\dc\vmu_data_host.bin", MainformRef.NullDCPath & "\flycast\data\vmu_save_A1.bin")

            Else
                FlycastInfo.Arguments += "-config network:ActAsServer=no "
                FlycastInfo.Arguments += "-config network:server= " & MainformRef.ConfigFile.Host & ":" & MainformRef.ConfigFile.Port & """"

                ' Copy the VMU from NullDC to Flycast
                FileSystem.FileCopy(MainformRef.NullDCPath & "\dc\vmu_data_client.bin", MainformRef.NullDCPath & "\flycast\data\vmu_save_A1.bin")

            End If

        End If

        If Rx.platform = "fc_dc" Then
            FlycastInfo.Arguments += """" & MainformRef.NullDCPath & "\dc\" & romdetails(1) & """"
        Else
            FlycastInfo.Arguments += """" & MainformRef.NullDCPath & romdetails(1) & """"
        End If

        FlycastProc = Process.Start(FlycastInfo)
        FlycastProc.EnableRaisingEvents = True

        AddHandler FlycastProc.Exited, AddressOf FlycastProcExited

    End Sub

    Private Sub RemoveAllTheShit()

        For Each file In Directory.GetFiles(MainformRef.NullDCPath & "\flycast\data", "*")
            If file.Contains("nvmem") Then
                SafeDeleteFile(file)
            End If

        Next

    End Sub

    Private Sub FlycastProcExited()

        Rx.EEPROM = ""

        If Not MainformRef.IsClosing Then
            Dim INVOKATION As EndSession_delegate = AddressOf MainformRef.EndSession
            MainformRef.Invoke(INVOKATION, {"Window Closed", Nothing})
        End If

        FlycastProc = Nothing

    End Sub

End Class
