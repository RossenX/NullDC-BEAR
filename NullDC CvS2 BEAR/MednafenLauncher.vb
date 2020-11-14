Imports NullDC_CvS2_BEAR.frmMain

Public Class MednafenLauncher
    Public MednafenInstance As Process = Nothing
    Public MednafenServerInstance As Process = Nothing

    Public Sub LaunchEmulator(ByVal _romname)
        If MednafenServerInstance Is Nothing Then
            StartServer()
        End If

        Dim t As Task = New Task(
            Async Sub()

                Dim _mednafenConfigs = IO.File.ReadAllLines(MainformRef.NullDCPath & "\mednafen\mednafen.cfg")
                Dim SpecialSettings = My.Resources.MednafenOptimizations.Split(vbNewLine)

                Dim LineCount = 0
                For Each _line In _mednafenConfigs
                    If _line.Length > 1 Then
                        For Each _optimization As String In SpecialSettings
                            If _line.Split(" ")(0).Trim = _optimization.Split(" ")(0).Trim Then
                                If Not _line.Split(" ")(1).Trim = _optimization.Split(" ")(1).Trim Then
                                    _mednafenConfigs(LineCount) = _optimization.Replace(vbNewLine, "")
                                End If
                            End If
                        Next
                    End If
                    LineCount += 1
                Next

                IO.File.WriteAllLines(MainformRef.NullDCPath & "\mednafen\mednafen.cfg", _mednafenConfigs)

                ' give the server a second to start
                Await Task.Delay(1000)
                Console.WriteLine("launching Mednagen: " & _romname)
                Dim MednafenInfo As New ProcessStartInfo
                MednafenInfo.FileName = MainformRef.NullDCPath & "\mednafen\mednafen.exe"
                MednafenInfo.Arguments = " -connect -netplay.host 127.0.0.1 -netplay.nick " & MainformRef.ConfigFile.Name & " "
                MednafenInfo.Arguments += """" & MainformRef.NullDCPath & "\" & MainformRef.GamesList(_romname)(1) & """"

                Console.WriteLine("Command: " & MednafenInfo.Arguments)
                MednafenInstance = Process.Start(MednafenInfo)
                MednafenInstance.EnableRaisingEvents = True

                AddHandler MednafenInstance.Exited,
                Sub()
                    If Not MednafenInstance Is Nothing Then
                        If Not MednafenServerInstance Is Nothing Then
                            MednafenServerInstance.CloseMainWindow()
                        End If

                        MednafenInstance = Nothing

                        Dim INVOKATION As EndSession_delegate = AddressOf MainformRef.EndSession
                        MainformRef.Invoke(INVOKATION, {"Window Closed", Nothing})

                    End If

                End Sub

            End Sub)

        t.Start()

    End Sub

    Public Sub StartServer()

        Console.WriteLine("Starting Mednafen Server")
        Dim MednafenServerInfo As New ProcessStartInfo
        MednafenServerInfo.FileName = MainformRef.NullDCPath & "\mednafen\server\mednafen-server.exe"
        MednafenServerInfo.Arguments = """" & MainformRef.NullDCPath & "\mednafen\server\standard.conf" & """"
        MednafenServerInfo.UseShellExecute = False

        MednafenServerInstance = Process.Start(MednafenServerInfo)
        MednafenServerInstance.EnableRaisingEvents = True

        AddHandler MednafenServerInstance.Exited,
                Sub()
                    MednafenServerInstance = Nothing
                End Sub
    End Sub

End Class
