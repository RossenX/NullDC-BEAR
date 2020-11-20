﻿Imports System.IO
Imports NullDC_CvS2_BEAR.frmMain

Public Class MednafenLauncher
    Public MednafenInstance As Process = Nothing
    Public MednafenServerInstance As Process = Nothing

    Public Sub LaunchEmulator(ByVal _romname)

        If MednafenServerInstance Is Nothing Then
            If MainformRef.ConfigFile.Status = "Hosting" Then ' If we're set to host then we host
                StartServer()
            End If
        End If

        If Rx.EEPROM = "" Then Rx.EEPROM = GenerateGameKey()

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

                        If _line.StartsWith("sound.volume") Then
                            _mednafenConfigs(LineCount) = "sound.volume " & MainformRef.ConfigFile.EmulatorVolume
                        End If

                    End If

                    LineCount += 1
                Next

                IO.File.WriteAllLines(MainformRef.NullDCPath & "\mednafen\mednafen.cfg", _mednafenConfigs)

                ' give the server a second to start
                Await Task.Delay(1000)
                Console.WriteLine("launching Mednagen: " & _romname)
                Dim MednafenInfo As New ProcessStartInfo
                MednafenInfo.FileName = MainformRef.NullDCPath & "\mednafen\mednafen.exe"
                If MainformRef.ConfigFile.Status = "Hosting" Or MainformRef.ConfigFile.Status = "Public" Or MainformRef.ConfigFile.Status = "Client" Then
                    MednafenInfo.Arguments = " -connect -netplay.host " & MainformRef.ConfigFile.Host & " -netplay.gamekey " & Rx.EEPROM & " -netplay.nick " & MainformRef.ConfigFile.Name & " "
                End If
                MednafenInfo.Arguments += """" & MainformRef.NullDCPath & "\" & MainformRef.GamesList(_romname)(1) & """"

                Console.WriteLine("Command: " & MednafenInfo.Arguments)
                MednafenInstance = Process.Start(MednafenInfo)
                MednafenInstance.EnableRaisingEvents = True

                AddHandler MednafenInstance.Exited, AddressOf MednafenExited

                Await Task.Delay(2000)

                ' Check if we should tell someone else to also start up
                If Not MainformRef.Challenger Is Nothing And (MainformRef.ConfigFile.Status = "Hosting" Or MainformRef.ConfigFile.Status = "Public") Then
                    Select Case MainformRef.ConfigFile.Status
                        Case "Hosting"
                            MainformRef.NetworkHandler.SendMessage("$," &
                                                           MainformRef.ConfigFile.Name & ",," &
                                                           MainformRef.ConfigFile.Port & "," &
                                                           MainformRef.ConfigFile.Game & "," &
                                                           MainformRef.ConfigFile.Delay & ",0" &
                                                            ",eeprom," & Rx.EEPROM, MainformRef.Challenger.ip)
                        Case "Public"
                            MainformRef.NetworkHandler.SendMessage("$," &
                                                           MainformRef.ConfigFile.Name & "," &
                                                           MainformRef.ConfigFile.Host & "," &
                                                           MainformRef.ConfigFile.Port & "," &
                                                           MainformRef.ConfigFile.Game & "," &
                                                           MainformRef.ConfigFile.Delay & ",1" &
                                                            ",eeprom," & Rx.EEPROM, MainformRef.Challenger.ip)
                    End Select

                End If


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

    Private Sub MednafenExited()

        Rx.EEPROM = ""

        If Not MednafenServerInstance Is Nothing Then
            MednafenServerInstance.CloseMainWindow()
        End If

        MednafenInstance = Nothing
        Dim INVOKATION As EndSession_delegate = AddressOf MainformRef.EndSession
        MainformRef.Invoke(INVOKATION, {"Window Closed", Nothing})

        If Directory.Exists(MainformRef.NullDCPath & "\mednafen\sav_") Then
            If Directory.Exists(MainformRef.NullDCPath & "\mednafen\sav") Then
                Directory.Delete(MainformRef.NullDCPath & "\mednafen\sav", True)
            End If
            FileSystem.Rename(MainformRef.NullDCPath & "\mednafen\sav_", MainformRef.NullDCPath & "\mednafen\sav")

        End If

        If Directory.Exists(MainformRef.NullDCPath & "\mednafen\mcs_") Then
            If Directory.Exists(MainformRef.NullDCPath & "\mednafen\mcs") Then
                Directory.Delete(MainformRef.NullDCPath & "\mednafen\mcs", True)
            End If
            FileSystem.Rename(MainformRef.NullDCPath & "\mednafen\mcs_", MainformRef.NullDCPath & "\mednafen\mcs")

        End If

    End Sub



End Class