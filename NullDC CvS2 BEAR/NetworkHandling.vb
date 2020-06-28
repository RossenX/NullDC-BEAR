Imports System.Net
Imports System.Net.NetworkInformation
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Imports NullDC_CvS2_BEAR.frmMain

Public Class NetworkHandling

    'Lobby System port
    Private Const port As Integer = 8002

    Public receivingClient As UdpClient
    Public sendingClient As UdpClient
    Private receivingThread As Thread
    Dim localpt = New IPEndPoint(IPAddress.Any, port)

    Dim MainFormRef As frmMain

    Public Sub New(ByVal mf As frmMain)
        MainFormRef = mf
        Dim MyIPAddress As String = ""

        ' Get IP
        Dim nics As NetworkInterface() = NetworkInterface.GetAllNetworkInterfaces()
        For Each netadapter As NetworkInterface In nics
            ' Get the Valid IP
            If netadapter.Name = MainFormRef.ConfigFile.Network Then

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

        MainFormRef.ConfigFile.IP = MyIPAddress
        MainFormRef.ConfigFile.SaveFile()

        If MyIPAddress = "" Then
            MsgBox("Yo, i couldn't find your IP, you sure this network is all good, dawg?")
        End If

        InitializeReceiver()
    End Sub




    Public Sub SendMessage(ByRef message As String, Optional SendtoIP As String = "255.255.255.255")
        Console.WriteLine("<-SendMessage->" & message & "->" & SendtoIP)
        Dim toSend As String = MainFormRef.ConfigFile.Version & ":" & message
        Dim data() As Byte = Encoding.ASCII.GetBytes(toSend)
        InitializeSender(SendtoIP)
        sendingClient.Send(data, data.Length)
    End Sub

    Private Sub InitializeSender(Optional SendtoIP As String = "255.255.255.255")
        Console.WriteLine("<-Initilize Sender->")
        sendingClient = New UdpClient(SendtoIP, port)
        sendingClient.EnableBroadcast = True
    End Sub

    Public Sub InitializeReceiver()
        receivingClient = New UdpClient(port)
        Dim start As ThreadStart = New ThreadStart(AddressOf Receiver)
        receivingThread = New Thread(start)
        receivingThread.IsBackground = True
        receivingThread.Start()
    End Sub

    Private Sub Receiver()
        Dim endPoint As IPEndPoint = New IPEndPoint(IPAddress.Any, port)
        Dim messageDelegate As MessageReceived_delegate = AddressOf MessageReceived
        While (True)
            Dim data() As Byte
            data = receivingClient.Receive(endPoint)
            Dim message As String = Encoding.ASCII.GetString(data)
            MessageReceived(message, endPoint.Address.ToString, endPoint.Port.ToString)
        End While
    End Sub

    Delegate Sub MessageReceived_delegate(ByRef message As String, ByRef senderip As String, ByRef port As String)
    Private Sub MessageReceived(ByRef message As String, ByRef senderip As String, ByRef port As String)
        Console.WriteLine("<-Recieved->" & message & " from " & senderip & ":" & port)
        'If senderip = MainFormRef.ConfigFile.IP Then Exit Sub ' Ignore Own Messages



        Dim Split = message.Split(":")
        ' Ignore data from other version of the software
        If Not MainFormRef.Ver = Split(0) Then
            'SendMessage(">,VM", senderip)
            Exit Sub
        End If
        ' Get the message string
        message = Split(1)

        If Not MainFormRef.Challenger Is Nothing And Not message.StartsWith("?") Then ' Only accept who is messages from anyone that is not your challenger
            ' Message is not from challanger
            If Not MainFormRef.Challenger.ip = senderip Then
                Console.WriteLine("<-DENIED->")
                SendMessage(">,BB", senderip)
                Exit Sub
            End If

        End If

        ' Ignore your own packets

        Split = message.Split(",")
        ' Messages
        ' ? - WHO IS
        ' < - I AM
        ' ! - Wanna Fite
        ' ^ - Lets FITE
        ' > - I am a Coward
        ' $ - Server Started Notification

        ' Who is ' ?(0)
        If message.StartsWith("?") Then
            Dim Status As String = MainFormRef.ConfigFile.Status
            Dim NameToSend As String = MainFormRef.ConfigFile.Name
            If Not MainFormRef.Challenger Is Nothing Then NameToSend = NameToSend & " vs " & MainFormRef.Challenger.name
            Dim GameNameAndRomName = "None"
            If Not MainFormRef.ConfigFile.Game = "None" Then GameNameAndRomName = MainFormRef.GamesList(MainFormRef.ConfigFile.Game)(0) & "|" & MainFormRef.ConfigFile.Game

            SendMessage("<," & NameToSend & "," & MainFormRef.ConfigFile.IP & "," & MainFormRef.ConfigFile.Port & "," & GameNameAndRomName & "," & Status, senderip)
            Exit Sub

        End If

        ' I am <(0),<name>(1),<ip>(2),<port>(3),<gamename|gamerom>(4),<status>(5)
        If message.StartsWith("<") Then
            Dim tmpthread As New Thread(
                Sub(senderip_)
                    Dim INVOKATION As AddPlayerToList_delegate = AddressOf MainFormRef.AddPlayerToList
                    Dim Pinger As Ping = New Ping()
                    'senderip
                    Dim rep As PingReply = Pinger.Send(senderip_, 1000)
                    Dim Ping = rep.RoundtripTime.ToString
                    If rep.RoundtripTime = 0 Then
                        Ping = "T/O"
                    End If
                    Dim Status = Split(5)
                    MainFormRef.Matchlist.Invoke(INVOKATION, {New NullDCPlayer(Split(1), Split(2), Split(3), Split(4), Status), Ping})
                End Sub
                )
            tmpthread.Start(senderip)
            Exit Sub
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
            ElseIf frmSetup.visible Then
                SendMessage(">,SP", senderip) ' In Setup
            ElseIf Split(5) = "0" Then ' Challanger isn't hosting, send them my host info if i have any

                If MainFormRef.ConfigFile.Status = "Hosting" Then ' Check if i'm still hosting
                    MainFormRef.Challenger = New NullDCPlayer(Split(1), Split(2), Split(3), Split(4), Split(5))
                    'Dim INVOKATION As SetChallenger_delegate = AddressOf MainFormRef.SetChallenger
                    'MainFormRef.Invoke(INVOKATION, {Split(1), Split(2), Split(3), Split(4), Split(5)})
                    SendMessage("$," & MainFormRef.ConfigFile.Name & "," & MainFormRef.ConfigFile.IP & "," & MainFormRef.ConfigFile.Port & "," & MainFormRef.ConfigFile.Game & "," & MainFormRef.ConfigFile.Delay, senderip)

                Else
                    SendMessage(">,HO", senderip) ' No Longer Hosting
                    Exit Sub

                End If

            ElseIf Split(5) = "1" Then ' ok they are going to host it, let me check if i'm already hosting something

                If MainFormRef.ConfigFile.Status = "Hosting" Or MainFormRef.ConfigFile.Status = "Client" Then ' I'm Already Hosting or Client of Someone DENY
                    SendMessage(">,HA", senderip)
                    Exit Sub
                Else ' Ok so i'm not hosting or client of someone, so start the challenge
                    Dim INVOKATION As BeingChallenged_delegate = AddressOf MainFormRef.BeingChallenged
                    MainFormRef.Invoke(INVOKATION, {Split(1), Split(2), Split(3), Split(4), Split(5)})
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
            MainFormRef.Invoke(INVOKATION, New NullDCPlayer(Split(1), Split(2), Split(3), Split(4)))
            Exit Sub
        End If

        ' Ended Session >(0),<reason>(1)
        If message.StartsWith(">") Then
            Console.WriteLine("<-End Session->")
            Dim INVOKATION As EndSession_delegate = AddressOf MainFormRef.EndSession
            MainFormRef.Invoke(INVOKATION, {Split(1), senderip})
            Exit Sub
        End If

        ' Host Started $(0),<name>(1),<ip>(2),<port>(3),<gamerom>(4),<delay>(5)
        If message.StartsWith("$") Then
            Console.WriteLine("<-Host Started->" & message)
            Dim INVOKATION As JoinHost_delegate = AddressOf MainFormRef.JoinHost
            Dim delay As Int16 = CInt(Split(5))
            MainFormRef.Invoke(INVOKATION, {Split(1), Split(2), Split(3), Split(4), delay})
            Exit Sub
        End If

    End Sub
End Class
