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

        ' Ignore absolutely everything from a gagged user
        If IsUserGagged(senderip) Then Exit Sub

        'If senderip = MainFormRef.ConfigFile.IP Then Exit Sub ' Ignore Own Messages

        Dim Split = message.Split(":")
        If Not MainformRef.Ver = Split(0) Then
            Exit Sub
        End If

        ' Get the message string
        message = Split(1)

        ' I'm spectating so don't let other people spectate my spectating
        If MainformRef.ConfigFile.Status = "Spectator" And message.StartsWith("!") Then SendMessage(">,NSS", senderip) : Exit Sub

        ' I should really clean this up and make it a little nicer
        If Not MainformRef.Challenger Is Nothing Then ' Only accept who is and challenge messages from none-challangers

            If Not message.StartsWith("<") And ' I am Request
                Not message.StartsWith("?") And ' Who Is Request
                Not message.StartsWith("!") And ' Being Challanged
                Not message.StartsWith("&") And ' Someone left BEAR
                Not message.StartsWith("V") And ' Asking for VMU
                Not message.StartsWith("G") And ' Successfuly got VMU
                Not message.StartsWith("$") Then ' Join a Host

                ' Message is not from challanger
                If Not MainformRef.Challenger.ip = senderip Then
                    ' Check if they are my currently Challanging
                    If MainformRef.ChallengeForm._Challenger Is Nothing Then
                        Console.WriteLine("<-DENIED->")
                        SendMessage(">,BB", senderip)
                        Exit Sub

                    ElseIf Not MainformRef.ChallengeForm._Challenger.ip = senderip Then
                        Console.WriteLine("<-DENIED->")
                        SendMessage(">,BB", senderip)
                        Exit Sub

                    End If
                End If

            End If

        End If

        Split = message.Split(",")

        ' Messages
        ' ? - WHO IS                    ?(0)
        ' < - I AM                      <(0),<name>(1),<ip>(2),<port>(3),<gamename|gamerom>(4),<status>(5)
        ' ! - Wanna Fite                !(0),<name>(1),<ip>(2),<port>(3),<gamerom>(4),<host>(5),<peripheral>(6)
        ' ^ - Lets FITE                 ^(0),<name>(1),<ip>(2),<port>(3),<gamerom>(4),<peripheral>(5)
        ' > - Session Ending            >(0),<reason>(1)
        ' $ - Server Started            $(0),<name>(1),<ip>(2),<port>(3),<gamerom>(4),<delay>(5),<region>(6),<peripheral>(7), <EEPROM>(8)
        ' & - Exited BEAR               &(0)
        ' @ - Join as spectator         @(0),<p1name>(1),<p2name>(2),<ip>(3),<port>(4),<game>(5),<region>(6),<p1peripheral>(7),<p2peripheral>(8), <EEPROM>(9)
        ' V - Requesting VMU DATA       V(0)
        ' # - VMU DATA                  #(0),<total_pieces>(1),<this_piece>(2),<VMU DATA PIECE>(3)
        ' G - VMU DATA RECIVED          G(0)
        ' M - DM Message                M(0),<Name>(1),<Message>(2)
        ' MR - Message Recieved         MR(0),<Message ID>(1)
        ' MO - Message Over             MO(0),<Reason>(1)

        Select Case Split(0)
            Case "?"
                Dim Status As String = MainformRef.ConfigFile.Status
                Dim NameToSend As String = MainformRef.ConfigFile.Name
                If Not MainformRef.Challenger Is Nothing Then NameToSend = NameToSend & " & " & MainformRef.Challenger.name
                Dim GameNameAndRomName = "None"

                If Not MainformRef.ConfigFile.Game = "None" Then GameNameAndRomName = MainformRef.GamesList(MainformRef.ConfigFile.Game)(0) & "|" & MainformRef.ConfigFile.Game
                SendMessage("<," & NameToSend & "," & SecretSettings & "," & MainformRef.ConfigFile.Port & "," & GameNameAndRomName & "," & Status, senderip)

            Case "<"
                MainformRef.AddPlayerToList(New BEARPlayer(Split(1), senderip, Split(3), Split(4), Split(5),, Split(2)))

            Case "!"

                Console.WriteLine("<-Being Challenged->")
                If MainformRef.IsNullDCRunning And (Not MainformRef.Challenger Is Nothing Or MainformRef.ConfigFile.Status = "Offline") Then
                    If MainformRef.ConfigFile.AllowSpectators = 0 Then SendMessage(">,NS", senderip) : Exit Sub
                    If Rx.platform = "na" Then SendMessage("@," & MainformRef.NullDCLauncher.P1Name & "," & MainformRef.NullDCLauncher.P2Name & ",," & MainformRef.ConfigFile.Port & "," & MainformRef.ConfigFile.Game & "," & MainformRef.NullDCLauncher.Region & "," & MainformRef.NullDCLauncher.P1Peripheral & "," & MainformRef.NullDCLauncher.P2Peripheral & ",eeprom," & Rx.EEPROM, senderip)

                ElseIf Not MainformRef.MednafenLauncher.MednafenInstance Is Nothing Then

                    Select Case MainformRef.ConfigFile.Status
                        Case "Offline" : SendMessage(">,MDN", senderip) : Exit Sub
                        Case "Client" : SendMessage(">,MDH", senderip) : Exit Sub
                        Case "Hosting"
                            MainformRef.NetworkHandler.SendMessage("$," & MainformRef.ConfigFile.Name & ",," & MainformRef.ConfigFile.Port & "," & MainformRef.ConfigFile.Game & "," & "0,0," & Rx.GetCurrentPeripherals & ",eeprom," & Rx.EEPROM, senderip)
                            Exit Sub
                        Case "Public"
                            MainformRef.NetworkHandler.SendMessage("$," & MainformRef.ConfigFile.Name & "," & MainformRef.ConfigFile.Host & "," & MainformRef.ConfigFile.Port & "," & MainformRef.ConfigFile.Game & "," & "1,0," & Rx.GetCurrentPeripherals & ",eeprom," & Rx.EEPROM, senderip)
                            Exit Sub
                        Case "Private" : SendMessage(">,MDP", senderip) : Exit Sub
                    End Select

                Else

                    If Not MainformRef.GamesList.ContainsKey(Split(4)) Then SendMessage(">,NG", senderip) : Exit Sub

                    If MainformRef.ConfigFile.AwayStatus = "DND" Or MainformRef.ConfigFile.AwayStatus = "Hidden" Then
                        SendMessage(">,DND", senderip)
                        Exit Sub
                    End If

                    If MainformRef.ChallengeForm.Visible Or MainformRef.GameSelectForm.Visible Then : SendMessage(">,BB", senderip) ' Chalanging someone
                    ElseIf MainformRef.ChallengeSentForm.Visible Then : SendMessage(">,BC", senderip) ' Being Chalanged by someone
                    ElseIf MainformRef.HostingForm.Visible Then : SendMessage(">,BH", senderip) ' Starting a host
                    ElseIf MainformRef.WaitingForm.Visible Then : SendMessage(">,BW", senderip) ' Waiting for a player
                    ElseIf frmSetup.Visible Then : SendMessage(">,SP", senderip) ' In Setup
                    Else ' We're not doing anything important so get challanged
                        Dim INVOKATION As BeingChallenged_delegate = AddressOf MainformRef.BeingChallenged
                        MainformRef.Invoke(INVOKATION, {Split(1), senderip, Split(3), Split(4), Split(5), Split(6)})
                    End If

                End If

            Case "^"
                If MainformRef.Challenger Is Nothing Then Exit Sub ' You didn't challange anyone, who tf accepted it
                If Not MainformRef.Challenger.ip = senderip Then Exit Sub ' you didn't challange THIS person why he accepting.

                Console.WriteLine("<-Accepted Challenged->" & message)
                Dim INVOKATION As OpenHostingPanel_delegate = AddressOf MainformRef.OpenHostingPanel
                MainformRef.Invoke(INVOKATION, New BEARPlayer(Split(1), senderip, Split(3), Split(4),, Split(5)))

            Case ">"

                If Not MainformRef.Challenger Is Nothing Then
                    If Not MainformRef.Challenger.ip = senderip Then Exit Sub
                ElseIf Not MainformRef.ChallengeForm._Challenger Is Nothing Then
                    If Not MainformRef.ChallengeForm._Challenger.ip = senderip Then Exit Sub
                Else : Exit Sub
                End If

                Console.WriteLine("<-End Session->")
                Dim INVOKATION As EndSession_delegate = AddressOf MainformRef.EndSession
                MainformRef.Invoke(INVOKATION, {Split(1), senderip})

            Case "$"

                If MainformRef.IsNullDCRunning Or Not MainformRef.MednafenLauncher.MednafenInstance Is Nothing Then ' We were told to join a game but we're already in a game
                    Exit Sub
                End If

                Console.WriteLine("<-Host Started->" & message)
                Dim INVOKATION As JoinHost_delegate = AddressOf MainformRef.JoinHost

                Rx.EEPROM = message.Split(New String() {",eeprom,"}, StringSplitOptions.None)(1)
                Dim delay As Int16 = CInt(Split(5))

                ' If we were not send an IP to join we join the senderip, otherwise we join w.e IP the host told us to
                If Split(2) = "" Then
                    MainformRef.Invoke(INVOKATION, {Split(1), senderip, Split(3), Split(4), delay, Split(6), Split(7), Rx.EEPROM})
                Else
                    MainformRef.Invoke(INVOKATION, {Split(1), Split(2), Split(3), Split(4), delay, Split(6), Split(7), Rx.EEPROM})
                End If

            Case "@"

                If MainformRef.Challenger Is Nothing Then Exit Sub ' You didn't challange anyone, who tf accepted it
                If Not MainformRef.Challenger.ip = senderip Then Exit Sub ' you didn't challange THIS person why he accepting.

                Console.WriteLine("<-Join As Spectator->" & message)
                Dim INVOKATION As JoinAsSpectator_delegate = AddressOf MainformRef.JoinAsSpectator
                Rx.EEPROM = message.Split(New String() {",eeprom,"}, StringSplitOptions.None)(1)
                MainformRef.Invoke(INVOKATION, {Split(1), Split(2), senderip, Split(4), Split(5), Split(6), Split(7), Split(8), Rx.EEPROM})

            Case "V" ' Someone requested VMU DATA V(0)

                Console.WriteLine("<-VMU Data request recieved->" & message)
                If Not (MainformRef.ConfigFile.Status = "Hosting" Or MainformRef.ConfigFile.Status = "Offline" Or MainformRef.ConfigFile.Status = "Client") Then
                    SendMessage(">,NSS", senderip) : Exit Sub
                End If

                If MainformRef.Challenger Is Nothing Then
                    If MainformRef.ConfigFile.AllowSpectators = 1 Then : Rx.SendVMU(senderip)
                    Else : SendMessage(">,NS", senderip) : End If

                Else
                    If MainformRef.Challenger.ip = senderip Then : Rx.SendVMU(senderip)
                    Else
                        If MainformRef.ConfigFile.AllowSpectators = 1 Then : Rx.SendVMU(senderip)
                        Else : SendMessage(">,NS", senderip) : End If
                    End If

                End If

            Case "G" ' VMU G Recived depending on who it is and which stage of the hosting we're on do stuff

                Console.WriteLine("<-VMU DATA RECIVED BY CLIENT SUCCESSFULLY->" & message)
                If MainformRef.ConfigFile.Status = "Offline" Then
                    SendMessage("@," & MainformRef.NullDCLauncher.P1Name & "," & MainformRef.NullDCLauncher.P2Name & ",," & MainformRef.ConfigFile.Port & "," & MainformRef.ConfigFile.Game & "," & MainformRef.NullDCLauncher.Region & "," & MainformRef.NullDCLauncher.P1Peripheral & "," & MainformRef.NullDCLauncher.P2Peripheral & ",eeprom,", senderip)

                Else
                    If senderip = MainformRef.Challenger.ip Then
                        If MainformRef.IsNullDCRunning Then
                            MainformRef.NetworkHandler.SendMessage("$," & MainformRef.ConfigFile.Name & ",," & MainformRef.ConfigFile.Port & "," & MainformRef.ConfigFile.Game & "," & MainformRef.ConfigFile.Delay & "," & MainformRef.NullDCLauncher.Region & ",eeprom,", MainformRef.Challenger.ip)

                        End If
                    Else
                        SendMessage("@," & MainformRef.NullDCLauncher.P1Name & "," & MainformRef.NullDCLauncher.P2Name & ",," & MainformRef.ConfigFile.Port & "," & MainformRef.ConfigFile.Game & "," & MainformRef.NullDCLauncher.Region & "," & MainformRef.NullDCLauncher.P1Peripheral & "," & MainformRef.NullDCLauncher.P2Peripheral & ",eeprom,", senderip)

                    End If

                End If

            Case "#"
                If MainformRef.Challenger Is Nothing Then Exit Sub ' You didn't challange anyone, who tf accepted it
                If Not MainformRef.Challenger.ip = senderip Then Exit Sub ' you didn't challange THIS person why he accepting.

                Console.WriteLine("<-VMU DATA-> " & Split(2) & "/" & Split(1))
                Rx.RecieveVMUPiece(CInt(Split(1)), CInt(Split(2)), Split(3))

            Case "&"
                Dim INVOKATION As RemovePlayerFromList_delegate = AddressOf MainformRef.RemovePlayerFromList
                MainformRef.Matchlist.Invoke(INVOKATION, {senderip})

            Case "M"
                Console.WriteLine("Text Message")
                If IsUserGagged(senderip) Then
                    SendMessage("MO,0", senderip) ' Computer says no
                    Exit Sub ' Check if User Is Blocked, if they are then just ignore them completely
                End If
                Dim Foundwindow = FindMessangerWindowFromIP(senderip)

                If Not MainformRef.ConfigFile.AwayStatus = "Idle" Then ' We are Hidden or in DND
                    If Foundwindow Is Nothing Then ' If we're in DND or Hidden we can only be messaged if we already engaged in a message with someone
                        If MainformRef.ConfigFile.AwayStatus = "DND" Then SendMessage("MO,1", senderip) ' Computer says no
                        Exit Sub

                    End If

                End If

                MainformRef.Invoke(
                        Sub(_senderip)
                            If Foundwindow Is Nothing Then
                                Dim _ChatForm As frmDM = New frmDM(_senderip, Split(1))
                                _ChatForm.Show(MainformRef)
                                _ChatForm.RecieveDM(_senderip, WebUtility.UrlDecode(Split(1) & ": " & Split(2)))

                            Else
                                Foundwindow.RecieveDM(_senderip, WebUtility.UrlDecode(Split(1) & ": " & Split(2)))

                            End If

                        End Sub, senderip)

            Case "MR"
                Console.WriteLine("Message Confirmation")

            Case "MO"
                Console.WriteLine("Message Over")
                Dim Foundwindow = FindMessangerWindowFromIP(senderip)

                If Not Foundwindow Is Nothing Then
                    Foundwindow.Invoke(
                        Sub()
                            Select Case Split(1)
                                Case "0"
                                    Foundwindow.AddMessageToWindow("    System: Player has you gagged.")

                                Case "1"
                                    Foundwindow.AddMessageToWindow("    System: Player is DND")

                            End Select
                            Foundwindow.over = True

                        End Sub)
                End If

        End Select

    End Sub

End Class
