Imports NullDC_CvS2_BEAR.frmMain

Public Class MFlycastLauncher
    Public FlycastProc As Process

    Public Sub LaunchFlycast(ByVal _romname As String, ByRef _region As String)

        Dim FlycastInfo As New ProcessStartInfo
        FlycastInfo.FileName = MainformRef.NullDCPath & "\flycast\flycast.exe"
        Dim romdetails = MainformRef.GamesList(_romname)

        ' Bunch of bullshit here

        ' Enable GGPO if it's online
        If MainformRef.ConfigFile.Status = "Offline" Then
            FlycastInfo.Arguments += "-config network:GGPO=no "
            FlycastInfo.Arguments += "-config network:ActAsServer=no "
            FlycastInfo.Arguments += "-config network:DNS= "
            FlycastInfo.Arguments += "-config network:EmulateBBA=no "
            FlycastInfo.Arguments += "-config network:Enable=no "
            FlycastInfo.Arguments += "-config network:server= "
            FlycastInfo.Arguments += "-config Dreamcast.RTC=1917526726 "


        Else

            ' This is Online lets check if we're the host
            FlycastInfo.Arguments += "-config network:GGPO=yes "
            If MainformRef.ConfigFile.Status = "Hosting" Then
                FlycastInfo.Arguments += "-config network:ActAsServer=yes "

            Else
                FlycastInfo.Arguments += "-config network:ActAsServer=no "

            End If

            FlycastInfo.Arguments += "-config network:GGPO=no "
            FlycastInfo.Arguments += "-config network:GGPO=no "
            FlycastInfo.Arguments += "-config network:GGPO=no "
            FlycastInfo.Arguments += "-config network:GGPO=no "
            FlycastInfo.Arguments += "-config network:GGPO=no "
            FlycastInfo.Arguments += "-config network:GGPO=no "

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

    Private Sub FlycastProcExited()

        Rx.EEPROM = ""

        If Not MainformRef.IsClosing Then
            Dim INVOKATION As EndSession_delegate = AddressOf MainformRef.EndSession
            MainformRef.Invoke(INVOKATION, {"Window Closed", Nothing})
        End If

        FlycastProc = Nothing

    End Sub

End Class
