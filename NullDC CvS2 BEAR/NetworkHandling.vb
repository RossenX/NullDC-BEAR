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
        InitializeReceiver()
    End Sub

    Public Sub SendMessage(ByRef message As String, Optional SendtoIP As String = "255.255.255.255")
        ' Don't send any I AM messages if you are hidden, but send everything else.
        If message.StartsWith("<") And MainformRef.ConfigFile.AwayStatus = "Hidden" Then
            Console.WriteLine("<-SendMessage->" & "  <ignored an IAM request>")
            Exit Sub
        End If

        If message.Length < 500 Then
            Console.WriteLine("<-SendMessage->" & message & "->" & SendtoIP)
        Else
            Console.WriteLine("<-SendMessage-> Long ass Message ->" & SendtoIP)
        End If

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
        If message.Length > 500 Then
            Console.WriteLine("<-Recieved-> long ass message from " & senderip & ":" & port)
        Else
            Console.WriteLine("<-Recieved->" & message & " from " & senderip & ":" & port)
        End If

        'If senderip = MainFormRef.ConfigFile.IP Then Exit Sub ' Ignore Own Messages

        Dim Split = message.Split(":")
        If Not MainformRef.Ver = Split(0) Then
            Exit Sub
        End If

        ' Get the message string
        message = Split(1)

        If MainformRef.ConfigFile.Status = "Spectator" And message.StartsWith("!") Then ' I'm spectating so don't let other people spectate my spectating
            SendMessage(">,NSS", senderip)
            Exit Sub
        End If

        If Not MainformRef.Challenger Is Nothing Then ' Only accept who is and challenge messages from none-challangers
            If Not message.StartsWith("<") And ' I am Request
                Not message.StartsWith("?") And ' Who Is Request
                Not message.StartsWith("!") And ' Being Challanged
                Not message.StartsWith("&") And ' Someone left BEAR
                Not message.StartsWith("V") And ' Asking for VMU
                Not message.StartsWith("G") And
                Not message.StartsWith("$") Then ' Successfuly got VMU
                ' Message is not from challanger
                If Not MainformRef.Challenger.ip = senderip Then
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
        ' V - Requesting VMU DATA
        ' # - VMU DATA
        ' G - VMU DATA RECIVED BY CLIENT SUCCESSFULLY

        ' Who is ' ?(0) ' Sending IP is Reduntant now it always uses w.e IP you could connect to but kept it in there for now untill later cleanup
        If message.StartsWith("?") Then
            Dim Status As String = MainformRef.ConfigFile.Status
            Dim NameToSend As String = MainformRef.ConfigFile.Name
            If Not MainformRef.Challenger Is Nothing Then NameToSend = NameToSend & " vs " & MainformRef.Challenger.name
            Dim GameNameAndRomName = "None"

            If Not MainformRef.ConfigFile.Game = "None" Then GameNameAndRomName = MainformRef.GamesList(MainformRef.ConfigFile.Game)(0) & "|" & MainformRef.ConfigFile.Game
            SendMessage("<," & NameToSend & ",," & MainformRef.ConfigFile.Port & "," & GameNameAndRomName & "," & Status, senderip)

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




        ' Joining Games in Progress
        If message.StartsWith("!") Then
            If MainformRef.IsNullDCRunning And (Not MainformRef.Challenger Is Nothing Or MainformRef.ConfigFile.Status = "Offline") Then ' I'm running nullDC so obviously can't accept the challenge.
                If MainformRef.ConfigFile.AllowSpectators = 1 Then
                    'This is NULLDC and it allows spectators
                    If MainformRef.ConfigFile.Status = "Spectator" Then ' I'm running it as a spectator/replay
                        SendMessage(">,NSS", senderip)
                        Exit Sub ' KEEP THIS HERE
                    End If

                    ' Naomi Tells em to join right away, DC uses "V" to ask for VMU and confirm VMU sync, then join
                    If Rx.platform = "na" Then
                        SendMessage("@," & MainformRef.NullDCLauncher.P1Name & "," & MainformRef.NullDCLauncher.P2Name & ",," & MainformRef.ConfigFile.Port & "," & MainformRef.ConfigFile.Game & "," & MainformRef.NullDCLauncher.Region & "," & MainformRef.NullDCLauncher.P1Peripheral & "," & MainformRef.NullDCLauncher.P2Peripheral & ",eeprom," & Rx.EEPROM, senderip)
                    End If

                    Exit Sub ' KEEP THIS HERE
                Else
                    SendMessage(">,NS", senderip)
                    Exit Sub ' KEEP THIS HERE
                End If





            ElseIf Not MainformRef.MednafenLauncher.MednafenInstance Is Nothing Then ' We're on Mednafen
                ' Mednafen "Spectator" and normal player are no different.

                Select Case MainformRef.ConfigFile.Status
                    Case "Offline"
                        SendMessage(">,MDN", senderip)
                        Exit Sub ' KEEP THIS HERE
                    Case "Client"
                        SendMessage(">,MDH", senderip)
                        Exit Sub ' KEEP THIS HERE
                    Case "Hosting"
                        MainformRef.NetworkHandler.SendMessage("$," &
                                                               MainformRef.ConfigFile.Name & ",," &
                                                               MainformRef.ConfigFile.Port & "," &
                                                               MainformRef.ConfigFile.Game & "," &
                                                               MainformRef.ConfigFile.Delay & ",0" &
                                                               ",eeprom," & Rx.EEPROM, senderip)
                        Exit Sub ' KEEP THIS HERE
                    Case "Public"
                        MainformRef.NetworkHandler.SendMessage("$," &
                                                               MainformRef.ConfigFile.Name & "," &
                                                               MainformRef.ConfigFile.Host & "," &
                                                               MainformRef.ConfigFile.Port & "," &
                                                               MainformRef.ConfigFile.Game & "," &
                                                               MainformRef.ConfigFile.Delay & ",1" &
                                                               ",eeprom," & Rx.EEPROM, senderip)
                        Exit Sub ' KEEP THIS HERE
                End Select

            End If

            ' ok so at this point we're not playing anything so continue down to the challange request
        End If


        ' Someone requested VMU DATA V(0)
        If message.StartsWith("V") Then
            Console.WriteLine("<-VMU Data request recieved->" & message)
            If MainformRef.ConfigFile.Status = "Hosting" Or MainformRef.ConfigFile.Status = "Offline" Or MainformRef.ConfigFile.Status = "Client" Then
            Else
                SendMessage(">,NSS", senderip)
                Exit Sub
            End If

            If MainformRef.Challenger Is Nothing Then ' Offline Spectating
                If MainformRef.ConfigFile.AllowSpectators = 1 Then ' Check if we allow spectators
                    Rx.SendVMU(senderip) ' If we do then start sending them the VMU
                Else
                    SendMessage(">,NS", senderip)
                End If

            Else
                If MainformRef.Challenger.ip = senderip Then ' We're sending this to our Challanger
                    Rx.SendVMU(senderip)
                Else ' We're sending a VMU to someone else, a spectator probably
                    If MainformRef.ConfigFile.AllowSpectators = 1 Then ' Check if we allow spectators
                        Rx.SendVMU(senderip) ' If we do then start sending them the VMU
                    Else
                        SendMessage(">,NS", senderip)
                    End If
                End If
            End If

            Exit Sub
        End If

        ' VMU G Recived depending on who it is and which stage of the hosting we're on do stuff
        If message.StartsWith("G") Then
            Console.WriteLine("<-VMU DATA RECIVED BY CLIENT SUCCESSFULLY->" & message)

            If MainformRef.ConfigFile.Status = "Offline" Then ' We're Offline, so this can only come from a spectator
                SendMessage("@," & MainformRef.NullDCLauncher.P1Name & "," & MainformRef.NullDCLauncher.P2Name & ",," & MainformRef.ConfigFile.Port & "," & MainformRef.ConfigFile.Game & "," & MainformRef.NullDCLauncher.Region & "," & MainformRef.NullDCLauncher.P1Peripheral & "," & MainformRef.NullDCLauncher.P2Peripheral & ",eeprom,", senderip)

            Else ' We're online, lets check if this is a spectator or my challanger and send them the appropriate response
                If senderip = MainformRef.Challenger.ip Then
                    If MainformRef.IsNullDCRunning Then ' We started the game, so tell em to join ' this is only if the game starts before the VMU is finished sending
                        MainformRef.NetworkHandler.SendMessage("$," & MainformRef.ConfigFile.Name & ",," & MainformRef.ConfigFile.Port & "," & MainformRef.ConfigFile.Game & "," & MainformRef.ConfigFile.Delay & "," & MainformRef.NullDCLauncher.Region & ",eeprom,", MainformRef.Challenger.ip)
                    End If
                Else ' This a Spectator
                    SendMessage("@," & MainformRef.NullDCLauncher.P1Name & "," & MainformRef.NullDCLauncher.P2Name & ",," & MainformRef.ConfigFile.Port & "," & MainformRef.ConfigFile.Game & "," & MainformRef.NullDCLauncher.Region & "," & MainformRef.NullDCLauncher.P1Peripheral & "," & MainformRef.NullDCLauncher.P2Peripheral & ",eeprom,", senderip)
                End If
            End If

            Exit Sub
        End If

        ' Being told to join a game, inprogress or not.
        ' It's up above the DND stuff, so it can bypass the DND
        ' EEPROM value is used in Mednafen for the gamekey
        ' Host Started $(0),<name>(1),<ip>(2),<port>(3),<gamerom>(4),<delay>(5),<region>(6),<peripheral>(7), <EEPROM>(8)
        If message.StartsWith("$") Then

            If MainformRef.IsNullDCRunning Or Not MainformRef.MednafenLauncher.MednafenInstance Is Nothing Then ' We were told to join a game but we're already in a game
                Exit Sub
            End If

            Console.WriteLine("<-Host Started->" & message)
            Dim INVOKATION As JoinHost_delegate = AddressOf MainformRef.JoinHost

            Rx.EEPROM = message.Split(New String() {",eeprom,"}, StringSplitOptions.None)(1)
            Dim delay As Int16 = CInt(Split(5))

            ' If we were not send an IP to join we join the senderip, otherwise we join w.e IP the host told u sto
            If Split(2) = "" Then
                MainformRef.Invoke(INVOKATION, {Split(1), senderip, Split(3), Split(4), delay, Split(6), Split(7), Rx.EEPROM})

            Else
                MainformRef.Invoke(INVOKATION, {Split(1), Split(2), Split(3), Split(4), delay, Split(6), Split(7), Rx.EEPROM})

            End If

            Exit Sub

        End If

        If message.StartsWith("!") And (MainformRef.ConfigFile.AwayStatus = "DND" Or MainformRef.ConfigFile.AwayStatus = "Hidden") Then
            SendMessage(">,DND", senderip)
            Exit Sub
        End If

        ' Wanna Fight !(0),<name>(1),<ip>(2),<port>(3),<gamerom>(4),<host>(5),<peripheral>(6)
        If message.StartsWith("!") Then
            Console.WriteLine("<-Being Challenged->")

            If Not MainformRef.GamesList.ContainsKey(Split(4)) Then ' Check if they have the game before anything else
                SendMessage(">,NG", senderip)
                Exit Sub

            End If

            If MainformRef.ChallengeForm.Visible Or MainformRef.GameSelectForm.Visible Then
                SendMessage(">,BB", senderip) ' Chalanging someone
            ElseIf MainformRef.ChallengeSentForm.Visible Then
                SendMessage(">,BC", senderip) ' Being Chalanged by someone
            ElseIf MainformRef.HostingForm.Visible Then
                SendMessage(">,BH", senderip) ' Starting a host
            ElseIf MainformRef.WaitingForm.Visible Then
                SendMessage(">,BW", senderip) ' Waiting for a 
            ElseIf frmSetup.Visible Then
                SendMessage(">,SP", senderip) ' In Setup
            ElseIf Split(5) = "0" Then ' Challanger isn't hosting, send them my host info if i have any

                If MainformRef.ConfigFile.Status = "Hosting" Then ' Check if i'm still hosting
                    MainformRef.Challenger = New NullDCPlayer(Split(1), Split(2), Split(3), Split(4), Split(5))
                    SendMessage("$," & MainformRef.ConfigFile.Name & ",," & MainformRef.ConfigFile.Port & "," & MainformRef.ConfigFile.Game & "," & MainformRef.ConfigFile.Delay & "," & MainformRef.NullDCLauncher.Region & "," & MainformRef.ConfigFile.Peripheral & ",eeprom," & Rx.EEPROM, senderip)
                Else
                    SendMessage(">,HO", senderip)
                    Exit Sub
                End If

            ElseIf Split(5) = "1" Then ' ok they are going to host it, just show the challange accept window

                If Not MainformRef.IsNullDCRunning Then
                    Dim INVOKATION As BeingChallenged_delegate = AddressOf MainformRef.BeingChallenged
                    MainformRef.Invoke(INVOKATION, {Split(1), senderip, Split(3), Split(4), Split(5), Split(6)})
                Else
                    ' I have nullDC open check if i'm hosting SOLO or if i'm fighting someone else
                    If MainformRef.ConfigFile.Status = "Hosting" Then
                        SendMessage(">,HA", senderip)
                    Else
                        SendMessage(">,BB", senderip)
                    End If
                End If

            End If
            Exit Sub ' Bye bye
        End If

        ' Below Commands are only valid from your challanger

        If MainformRef.Challenger Is Nothing Then Exit Sub ' You shouldn't have a challanger, ignore
        If Not MainformRef.Challenger.ip = senderip Then Exit Sub ' The person sending request is not your challanger

        ' Accept Fight ^(0),<name>(1),<ip>(2),<port>(3),<gamerom>(4),<peripheral>(5)
        If message.StartsWith("^") Then
            Console.WriteLine("<-Accepted Challenged->" & message)
            Dim INVOKATION As OpenHostingPanel_delegate = AddressOf MainformRef.OpenHostingPanel
            MainformRef.Invoke(INVOKATION, New NullDCPlayer(Split(1), senderip, Split(3), Split(4),, Split(5)))
            Exit Sub
        End If

        ' Ended Session >(0),<reason>(1)
        If message.StartsWith(">") Then
            Console.WriteLine("<-End Session->")
            Dim INVOKATION As EndSession_delegate = AddressOf MainformRef.EndSession
            MainformRef.Invoke(INVOKATION, {Split(1), senderip})
            Exit Sub
        End If

        ' Join As Spectator @(0),<p1name>(1),<p2name>(2),<ip>(3),<port>(4),<game>(5),<region>(6),<p1peripheral>(7),<p2peripheral>(8), <EEPROM>(9)  CHECK HERE FIX THIS GET BOTH PLAYER PERIPHERALS
        ' Delay not required, spectating will always add delay based on how smooth it is.
        If message.StartsWith("@") Then
            Console.WriteLine("<-Join As Spectator->" & message)
            Dim INVOKATION As JoinAsSpectator_delegate = AddressOf MainformRef.JoinAsSpectator
            Rx.EEPROM = message.Split(New String() {",eeprom,"}, StringSplitOptions.None)(1)
            MainformRef.Invoke(INVOKATION, {Split(1), Split(2), senderip, Split(4), Split(5), Split(6), Split(7), Split(8), Rx.EEPROM})
        End If

        ' VMU DATA #(0),<total_pieces>(1),<this_piece>(2),<VMU DATA PIECE>(3)
        If message.StartsWith("#") Then
            Console.WriteLine("<-VMU DATA-> " & Split(2) & "/" & Split(1))
            Rx.RecieveVMUPiece(CInt(Split(1)), CInt(Split(2)), Split(3))
        End If

    End Sub

End Class
