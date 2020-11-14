Public Class MednafenLauncher
    Public MednafenInstance As Process = Nothing
    Public MednaGenServerInstance As Process = Nothing

    Public Sub New()

    End Sub

    Public Sub LaunchEmulator(ByVal _romname)
        If MednaGenServerInstance Is Nothing Then
            StartServer()
        End If

        Dim t As Task = New Task(
            Async Sub()
                ' give the server a second to start
                Await Task.Delay(1000)
                Console.WriteLine("launching Mednagen: " & _romname)
                Dim MednafenInfo As New ProcessStartInfo
                MednafenInfo.FileName = MainformRef.NullDCPath & "\mednafen\mednafen.exe"
                MednafenInfo.Arguments = " -connect -netplay.host 127.0.0.1 -netplay.nick " & MainformRef.ConfigFile.Name & " "
                MednafenInfo.Arguments += """" & MainformRef.NullDCPath & "\" & MainformRef.GamesList(_romname)(1) & """"

                Console.WriteLine("Command: " & MednafenInfo.Arguments)
                MednafenInstance = Process.Start(MednafenInfo)
            End Sub)

        t.Start()

    End Sub

    Public Sub StartServer()

        Console.WriteLine("Starting Mednafen Server")
        Dim MednafenServerInfo As New ProcessStartInfo
        MednafenServerInfo.FileName = MainformRef.NullDCPath & "\mednafen\server\mednafen-server.exe"
        MednafenServerInfo.Arguments = """" & MainformRef.NullDCPath & "\mednafen\server\standard.conf" & """"
        MednafenServerInfo.UseShellExecute = False

        MednaGenServerInstance = Process.Start(MednafenServerInfo)

    End Sub

End Class
