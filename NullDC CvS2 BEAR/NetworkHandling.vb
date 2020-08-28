Imports System.IO
Imports System.Net
Imports System.Net.NetworkInformation
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Imports NullDC_CvS2_BEAR.frmMain

Public Class NetworkHandling

    'Lobby System port
    Private Const port As Integer = 8002 ' 8002

    Public BEAR_UDPReceiver As UdpClient
    Public BEAR_UDPSender As UdpClient
    Private receivingThread As Thread

    Public Sub New(ByVal mf As frmMain)
        Dim MyIPAddress As String = ""

        ' Get IP
        Dim nics As NetworkInterface() = NetworkInterface.GetAllNetworkInterfaces()
        For Each netadapter As NetworkInterface In nics
            ' Get the Valid IP
            If netadapter.Name = MainformRef.ConfigFile.Network Then

                Dim i = 0
                For Each Address In netadapter.GetIPProperties.UnicastAddresses
                    Dim OutAddress As IPAddress = New IPAddress(2130706433)
                    If IPAddress.TryParse(netadapter.GetIPProperties.UnicastAddresses(i).Address.ToString(), OutAddress) Then
                        If OutAddress.AddressFamily = System.Net.Sockets.AddressFamily.InterNetwork Then
                            MyIPAddress = netadapter.GetIPProperties.UnicastAddresses(i).Address.ToString()
                            Exit For
                        End If
                    End If
                    i += 1
                Next
            End If
        Next

        MainformRef.ConfigFile.IP = MyIPAddress
        MainformRef.ConfigFile.SaveFile()

        'If MyIPAddress = "" Then
        ' MsgBox("Yo, i couldn't find your IP, you sure this network is all good, dawg?")
        'End If

        InitializeReceiver()
    End Sub

    Public Sub SendMessage(ByRef message As String, Optional SendtoIP As String = "255.255.255.255")
        ' Don't send any I AM messages if you are hidden, but send everything else.
        If message.StartsWith("<") And MainformRef.ConfigFile.AwayStatus = "Hidden" Then
            Console.WriteLine("<-SendMessage->" & "  <ignored an IAM request>")
            Exit Sub
        End If

        Console.WriteLine("<-SendMessage->" & message & "->" & SendtoIP)
        Dim toSend As String = MainformRef.ConfigFile.Version & ":" & message
        Dim data() As Byte = Encoding.ASCII.GetBytes(toSend)

        Try
            BEAR_UDPSender = New UdpClient(SendtoIP, port)
            BEAR_UDPSender.EnableBroadcast = True
            BEAR_UDPSender.SendAsync(data, data.Length)

        Catch ex As Exception
            Console.WriteLine("Failed to send")
        End Try

    End Sub

    Public Sub InitializeReceiver()
        If BEAR_UDPReceiver Is Nothing Then
            BEAR_UDPReceiver = New UdpClient(port)
            BEAR_UDPReceiver.EnableBroadcast = True
        End If

        Dim RecieverThread As Thread = New Thread(
            Sub()
                While (True)
                    Dim endPoint As IPEndPoint = New IPEndPoint(IPAddress.Any, port)
                    Dim data() As Byte = BEAR_UDPReceiver.Receive(endPoint)
                    Dim message As String = Encoding.ASCII.GetString(data)
                    MessageReceived(message, endPoint.Address.ToString, endPoint.Port.ToString)
                End While

            End Sub)

        RecieverThread.IsBackground = True
        RecieverThread.Start()

    End Sub

    Delegate Sub MessageReceived_delegate(ByRef message As String, ByRef senderip As String, ByRef port As String)
    Private Sub MessageReceived(ByRef message As String, ByRef senderip As String, ByRef port As String)
        Console.WriteLine("<-Recieved->" & message & " from " & senderip & ":" & port)
        'If senderip = MainFormRef.ConfigFile.IP Then Exit Sub ' Ignore Own Messages

        Dim Split = message.Split(":")
        If Not MainformRef.Ver = Split(0) Then
            Exit Sub
        End If

        ' Get the message string
        message = Split(1)

        If message.StartsWith("!") And (MainformRef.ConfigFile.AwayStatus = "DND" Or MainformRef.ConfigFile.AwayStatus = "Hidden") Then
            SendMessage(">,DND", senderip)
            Exit Sub
        End If

        If MainFormRef.ConfigFile.Status = "Spectator" And message.StartsWith("!") Then ' I'm spectating so don't let other people spectate my spectating
            SendMessage(">,NSS", senderip)
            Exit Sub
        End If

        If Not MainFormRef.Challenger Is Nothing Then ' Only accept who is and challenge messages from none-challangers
            If Not message.StartsWith("<") And Not message.StartsWith("?") And Not message.StartsWith("!") And Not message.StartsWith("&") Then
                ' Message is not from challanger
                If Not MainformRef.Challenger.ip = senderip And Not MainformRef.ConfigFile.IP = senderip Then
                    Console.WriteLine("<-DENIED->")
                    SendMessage(">,BB", senderip)
                    Exit Sub
                End If
            End If
        End If

        Split = message.Split(",")
        ' Messages
        ' ? - WHO IS
        ' < - I AM
        ' ! - Wanna Fite
        ' ^ - Lets FITE
        ' > - Session Ending or decline or w.e, just end the session for w.e reason.
        ' $ - Server Started Notification
        ' & - Exited BEAR, remove from loby.
        ' @ - Join as spectator

        ' Who is ' ?(0) ' Sending IP is Reduntant now it always uses w.e IP you could connect to but kept it in there for now untill later cleanup
        If message.StartsWith("?") Then
            Dim Status As String = MainFormRef.ConfigFile.Status
            Dim NameToSend As String = MainFormRef.ConfigFile.Name
            If Not MainFormRef.Challenger Is Nothing Then NameToSend = NameToSend & " vs " & MainFormRef.Challenger.name
            Dim GameNameAndRomName = "None"
            If Not MainformRef.ConfigFile.Game = "None" Then GameNameAndRomName = MainformRef.GamesList(MainformRef.ConfigFile.Game)(0) & "|" & MainformRef.ConfigFile.Game
            SendMessage("<," & NameToSend & "," & MainFormRef.ConfigFile.IP & "," & MainFormRef.ConfigFile.Port & "," & GameNameAndRomName & "," & Status, senderip)
            Exit Sub

        End If

        ' I am <(0),<name>(1),<ip>(2),<port>(3),<gamename|gamerom>(4),<status>(5)
        If message.StartsWith("<") Then
            'Dim INVOKATION As AddPlayerToList_delegate = AddressOf MainFormRef.AddPlayerToList
            'MainformRef.Matchlist.Invoke(INVOKATION, {New NullDCPlayer(Split(1), senderip, Split(3), Split(4), Split(5))})
            MainformRef.AddPlayerToList(New NullDCPlayer(Split(1), senderip, Split(3), Split(4), Split(5)))
            Exit Sub
        End If

        ' Left BEAR
        If message.StartsWith("&") Then
            Dim INVOKATION As RemovePlayerFromList_delegate = AddressOf MainformRef.RemovePlayerFromList
            MainformRef.Matchlist.Invoke(INVOKATION, {senderip})
            Exit Sub
        End If

        ' Check if we should tell them to spectate
        If message.StartsWith("!") Then
            If MainFormRef.IsNullDCRunning And (Not MainFormRef.Challenger Is Nothing Or MainFormRef.ConfigFile.Status = "Offline") Then ' I'm running nullDC so obviously can't accept the challenge.
                If MainFormRef.ConfigFile.AllowSpectators = 1 Then

                    If MainFormRef.ConfigFile.Status = "Spectator" Then ' I'm running it as a spectator/replay
                        SendMessage(">,NSS", senderip)
                        Exit Sub
                    End If

                    Dim tmp_p1name = ""
                    Dim tmp_p2name = ""

                    If MainFormRef.ConfigFile.Status = "Client" Then
                        tmp_p1name = MainFormRef.Challenger.name
                        tmp_p2name = MainFormRef.ConfigFile.Name
                    ElseIf MainFormRef.ConfigFile.Status = "Hosting" Then
                        tmp_p1name = MainFormRef.ConfigFile.Name
                        tmp_p2name = MainFormRef.Challenger.name
                    ElseIf MainFormRef.ConfigFile.Status = "Offline" Then
                        tmp_p1name = MainFormRef.ConfigFile.Name
                        tmp_p2name = "Local Player 2"
                    End If

                    If MainformRef.NullDCLauncher.Platform = "dc" Then
                        SendMessage(">,NDC", senderip)
                    ElseIf MainformRef.NullDCLauncher.Platform = "na" Then
                        SendMessage("@," & tmp_p1name & "," & tmp_p2name & "," & MainformRef.ConfigFile.IP & "," & MainformRef.ConfigFile.Port & "," & MainformRef.ConfigFile.Game & "," & MainformRef.NullDCLauncher.Region & ",eeprom," & Rx.EEPROM, senderip)
                    End If

                    Exit Sub
                Else
                    SendMessage(">,NS", senderip)
                    Exit Sub
                End If
            End If
        End If

        ' Wanna Fight !(0),<name>(1),<ip>(2),<port>(3),<gamerom>(4),<host>(5)
        If message.StartsWith("!") Then
            Console.WriteLine("<-Being Challenged->")

            If Not MainFormRef.GamesList.ContainsKey(Split(4)) Then ' Check if they have the game before anything else
                SendMessage(">,NG", senderip)
                Exit Sub
            End If

            If MainFormRef.ChallengeForm.Visible Or MainFormRef.GameSelectForm.Visible Then
                SendMessage(">,BB", senderip) ' Chalanging someone
            ElseIf MainFormRef.ChallengeSentForm.Visible Then
                SendMessage(">,BC", senderip) ' Being Chalanged by someone
            ElseIf MainFormRef.HostingForm.Visible Then
                SendMessage(">,BH", senderip) ' Starting a host
            ElseIf MainFormRef.WaitingForm.Visible Then
                SendMessage(">,BW", senderip) ' Waiting for a 
            ElseIf frmSetup.Visible Then
                SendMessage(">,SP", senderip) ' In Setup
            ElseIf Split(5) = "0" Then ' Challanger isn't hosting, send them my host info if i have any

                If MainFormRef.ConfigFile.Status = "Hosting" Then ' Check if i'm still hosting
                    MainFormRef.Challenger = New NullDCPlayer(Split(1), Split(2), Split(3), Split(4), Split(5))
                    SendMessage("$," & MainformRef.ConfigFile.Name & "," & MainformRef.ConfigFile.IP & "," & MainformRef.ConfigFile.Port & "," & MainformRef.ConfigFile.Game & "," & MainformRef.ConfigFile.Delay & "," & MainformRef.NullDCLauncher.Region & ",eeprom," & Rx.EEPROM, senderip)
                Else
                    SendMessage(">,HO", senderip)
                    Exit Sub
                End If

            ElseIf Split(5) = "1" Then ' ok they are going to host it, just show the challange accept window

                If Not MainFormRef.IsNullDCRunning Then
                    Dim INVOKATION As BeingChallenged_delegate = AddressOf MainFormRef.BeingChallenged
                    MainformRef.Invoke(INVOKATION, {Split(1), senderip, Split(3), Split(4), Split(5)})
                Else
                    ' I have nullDC open check if i'm hosting SOLO or if i'm fighting someone else
                    If MainFormRef.ConfigFile.Status = "Hosting" Then
                        SendMessage(">,HA", senderip)
                    Else
                        SendMessage(">,BB", senderip)
                    End If
                End If

            End If
            Exit Sub ' Bye bye
        End If

        ' Below Commands are only valid from your challanger

        If MainFormRef.Challenger Is Nothing Then Exit Sub ' You shouldn't have a challanger, ignore
        If Not MainFormRef.Challenger.ip = senderip Then Exit Sub ' The person sending request is not your challanger

        ' Accept Fight ^(0),<name>(1),<ip>(2),<port>(3),<gamerom>(4)
        If message.StartsWith("^") Then
            Console.WriteLine("<-Accepted Challenged->" & message)
            Dim INVOKATION As OpenHostingPanel_delegate = AddressOf MainFormRef.OpenHostingPanel
            MainformRef.Invoke(INVOKATION, New NullDCPlayer(Split(1), senderip, Split(3), Split(4)))
            Exit Sub
        End If

        ' Ended Session >(0),<reason>(1)
        If message.StartsWith(">") Then
            Console.WriteLine("<-End Session->")
            Dim INVOKATION As EndSession_delegate = AddressOf MainFormRef.EndSession
            MainFormRef.Invoke(INVOKATION, {Split(1), senderip})
            Exit Sub
        End If

        ' Host Started $(0),<name>(1),<ip>(2),<port>(3),<gamerom>(4),<delay>(5),<region>(6),<EEPROM>(7) EEPROM HANDLING NOT YET IMPLEMENTED
        If message.StartsWith("$") Then
            Console.WriteLine("<-Host Started->" & message)
            Dim INVOKATION As JoinHost_delegate = AddressOf MainformRef.JoinHost
            Rx.EEPROM = message.Split(New String() {",eeprom,"}, StringSplitOptions.None)(1)
            Dim delay As Int16 = CInt(Split(5))
            MainformRef.Invoke(INVOKATION, {Split(1), senderip, Split(3), Split(4), delay, Split(6), Rx.EEPROM})
            Exit Sub
        End If

        ' Join As Spectator @(0),<p1name>(1),<p2name>(2),<ip>(3),<port>(4),<game>(5),<region>(6),<EEPROM>(7) EEPROM HANDLING NOT YET IMPLEMENTED
        ' Delay not required, spectating will always add delay based on how smooth it is.
        If message.StartsWith("@") Then
            Console.WriteLine("<-Join As Spectator->" & message)
            Dim INVOKATION As JoinAsSpectator_delegate = AddressOf MainformRef.JoinAsSpectator
            Rx.EEPROM = message.Split(New String() {",eeprom,"}, StringSplitOptions.None)(1)
            MainformRef.Invoke(INVOKATION, {Split(1), Split(2), senderip, Split(4), Split(5), Split(6), Rx.EEPROM})
        End If

    End Sub
End Class
