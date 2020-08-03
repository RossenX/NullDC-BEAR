' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW
' DEPRECATED SINCE BEARJAMMA DOES THIS NOW




Imports OpenTK
Imports System.Net.Sockets
Imports System.IO
Imports System.Text
Imports System.Net
Imports System.Threading

Public Class deprecated_BearPlay

    ' Hookers Maybe Later
    Public Declare Function SetWindowsHookEx Lib "user32" _
    Alias "SetWindowsHookExA" (ByVal idHook As Integer,
    ByVal lpfn As KeyboardHookDelegate, ByVal hmod As Integer,
    ByVal dwThreadId As Integer) As Integer

    Public Delegate Function KeyboardHookDelegate(
    ByVal Code As Integer,
    ByVal wParam As Integer, ByRef lParam As KBDLLHOOKSTRUCT) As Integer

    Public Structure KBDLLHOOKSTRUCT
        Public vkCode As Integer
        Public scanCode As Integer
        Public flags As Integer
        Public time As Integer
        Public dwExtraInfo As Integer
    End Structure

    Dim MockServer As TcpListener
    Dim MockServerThread As Thread

    Dim IsHosting As Boolean = False

    Dim ClientsList As New Dictionary(Of Int16, ClientHandler)

    Dim ListeningAddress As IPAddress

    Dim IsListening As Boolean = False

    ' Replay Capture
    Public FrameDataToWrite As ArrayList = New ArrayList
    Dim CaptureReplay = False
    Dim GameRom As String
    Dim ReplayWriterThread As Thread
    Dim Region = "JPN"

    ' Packet: 8 Bytes 
    '       0-3: 00 00 00 00<Frame Count 32 bit integer split into 4 bytes> 
    '       4-5: 00 00 <Inputs as Binary STUDLR123456???? > When Sending, sends your inputs. When recieving, recives opponent inputs. 
    '       6-7: 00 00 <Coin Data? Increases as the coin button is pressed, probably was meant to keep coin data in sync but wasn't finished>


    Public Sub New(ByVal _region As String)
        Console.WriteLine("Bearplay Created")
        GameRom = MainformRef.ConfigFile.Game
        Region = _region

        ListeningAddress = IPAddress.Parse(MainformRef.ConfigFile.IP)
        MockServer = New TcpListener(ListeningAddress, MainformRef.ConfigFile.Port)

        MockServerThread = New Thread(AddressOf StartMocKServer)
        MockServerThread.IsBackground = True
        MockServerThread.Start()

        If MainformRef.ConfigFile.RecordReplay = "1" Then
            CaptureReplay = True
            StartReplayWriter()
        End If

    End Sub

    Private Sub StartReplayWriter()
        ReplayWriterThread = New Thread(AddressOf ReplayWriter)
        ReplayWriterThread.IsBackground = True
        ReplayWriterThread.Start()
    End Sub

    Public Sub CleanUp(ByVal _GameRom As String)
        GameRom = _GameRom
        CaptureReplay = False
        IsListening = False
        MockServer.Stop()
        For Each _client As ClientHandler In ClientsList.Values
            _client.IncomingClient.Close()
            _client.InternalRelay.Close()
        Next
    End Sub

    Private Sub StartMocKServer()
        MockServer.Start()
        IsListening = True

        While IsListening
            Try
                Dim ConnectingClient = MockServer.AcceptTcpClient()
                ClientsList.Add(ClientsList.Count, New ClientHandler(ConnectingClient, ClientsList.Count, Me))
            Catch ex As Exception
                'MsgBox(ex.Message)
            End Try

        End While

        MockServer.Stop()
        IsListening = False
        While ReplayWriterThread.IsAlive
            Thread.Sleep(50)
        End While
    End Sub

    Private Sub ReplayWriter()

        While MainformRef.Challenger Is Nothing
            If Not CaptureReplay Then
                Exit Sub
            End If
            Thread.Sleep(100)
        End While

        Dim TimeStamp = DateTime.Now.ToString("(MM-dd hh-mm)")
        Dim TimeStamp2 = DateTime.Now.ToString("MM-dd hh-mm")
        Dim ReplayFolder = MainformRef.NullDCPath & "\replays"
        Dim Player1 = ""
        Dim Player2 = ""
        If MainformRef.ConfigFile.Status = "Client" Then
            Player2 = MainformRef.ConfigFile.Name
            Player1 = MainformRef.Challenger.name
        Else
            Player2 = MainformRef.Challenger.name
            Player1 = MainformRef.ConfigFile.Name
        End If
        Dim FrameCount = 0

        Dim HostOffset = 0
        If MainformRef.ConfigFile.Status = "Hosting" Then
            HostOffset = 1
        End If

        Dim ReplayTitle = Player1 & " vs " & Player2 & " " & MainformRef.GamesList(MainformRef.ConfigFile.Game)(0) & " " & TimeStamp

        ' Write Replay FIle
        If Not Directory.Exists(ReplayFolder) Then Directory.CreateDirectory(ReplayFolder)

        Dim s As New FileStream(ReplayFolder & "\" & ReplayTitle & ".bearplay", FileMode.OpenOrCreate)

        Console.WriteLine("Starting Replay Recording")
        While CaptureReplay
            If FrameDataToWrite.Count = 0 Then
                Thread.Sleep(10)
                Continue While
            End If

            If Not FrameDataToWrite(0) Is Nothing Then
                Dim CurrentWritingFrame As FrameData = FrameDataToWrite(0)

                CurrentWritingFrame.Inputs(2) = CurrentWritingFrame.Inputs(2) Mod 2 + 1
                CurrentWritingFrame.Inputs(3) = 0

                Dim offset = (8 * (CurrentWritingFrame.Frame - 1)) + ((CurrentWritingFrame.Player - 1) * 4)
                If FrameCount < CurrentWritingFrame.Frame Then FrameCount = CurrentWritingFrame.Frame

                Try
                    s.Seek(offset, SeekOrigin.Begin)
                    For i = 0 To CurrentWritingFrame.Inputs.Count - 1
                        s.WriteByte(CurrentWritingFrame.Inputs(i))
                    Next

                Catch ex As Exception

                End Try

            Else
                FrameDataToWrite.RemoveAt(0)
                Continue While

            End If

            FrameDataToWrite.RemoveAt(0)

        End While
        Console.WriteLine("Ending Replay Recording")

        ' Write End Of File Data Here ADd 60 frames of nothing
        s.Seek(0, SeekOrigin.End)
        Dim EmptyFrame(7) As Byte
        EmptyFrame(0) = 0
        EmptyFrame(1) = 0
        EmptyFrame(2) = 2
        EmptyFrame(3) = 0
        EmptyFrame(4) = 0
        EmptyFrame(5) = 0
        EmptyFrame(6) = 2
        EmptyFrame(7) = 0

        For i = 0 To 240 ' Add some Trailing Frames
            s.Write(EmptyFrame, 0, 8)
        Next

        ' BearPlay|P1|P2|Gamename|GameRom|TimeStamp|Length in frames|Region

        Dim GameInfo = MainformRef.GamesList(GameRom)
        Dim MetaDataToAdd = "BearPlay|" & Player1 & "|" & Player2 & "|" & GameInfo(0) & "|" & GameRom & "|" & TimeStamp2 & "|" & FrameCount & "|" & Region
        Dim MetaDataToAddBytes As Byte() = Encoding.ASCII.GetBytes(MetaDataToAdd)

        s.Write(MetaDataToAddBytes, 0, MetaDataToAddBytes.Length)
        Console.WriteLine("Ended Replay Recording")

        s.Close()

    End Sub


End Class

Public Class ClientHandler

    Dim ClientID As Int16
    Public IncomingClient As TcpClient
    Public InternalRelay As TcpClient

    Public ClientConnected As Boolean = False

    Dim BearPlay As deprecated_BearPlay

    Dim FrameDataToWrite As ArrayList = New ArrayList
    Dim CaptureReplay As Boolean = True

    Dim StreamThread As Thread
    Dim InternalStreamThread As Thread

    Dim StreamPlayer = 0
    Dim InternalPlayer = 0

    Public Sub New(ByRef _client As TcpClient, ByVal _cid As Int16, ByRef _bp As deprecated_BearPlay)
        IncomingClient = _client
        ClientID = _cid
        BearPlay = _bp
        ClientConnected = True

        Console.WriteLine("Connected: {0}:{1}", _cid, IncomingClient.Client.RemoteEndPoint.ToString)

        Dim RelayToIP = MainformRef.ConfigFile.IP
        Dim RelayToPort = CInt(MainformRef.ConfigFile.Port) + 1

        ' Check if this is MY NULLDC sending data that needs to be relayed outside, or if it's outside nullDC that needs to relay data inside
        If IncomingClient.Client.RemoteEndPoint.ToString.Split(":")(0) = MainformRef.ConfigFile.IP Then
            ' CLIENT IS ME SO SEND TO OTHER PERSON
            RelayToPort = MainformRef.Challenger.port
            RelayToIP = MainformRef.Challenger.ip
        ElseIf IncomingClient.Client.RemoteEndPoint.ToString.Split(":")(0) = MainformRef.Challenger.ip Then
            ' THIS IS CHALLANGER

        Else
            ' THIS IS SPECTATOR

        End If

        StreamThread = New Thread(Sub() StartStream(IncomingClient))
        StreamThread.IsBackground = True
        StreamThread.Start()

        InternalRelay = New TcpClient()
        InternalRelay.Connect(RelayToIP, RelayToPort)
        InternalStreamThread = New Thread(Sub() StartStream(InternalRelay))
        InternalStreamThread.IsBackground = True
        InternalStreamThread.Start()

    End Sub

    Private Sub RelayData(ByVal _bytes() As Byte, ByRef _Client As TcpClient)
        Dim StreamToUse As NetworkStream

        Dim Player = 1
        If _Client Is IncomingClient Then
            StreamToUse = InternalRelay.GetStream
            If InternalPlayer = 0 Then
                Dim RemoteEndPoint As String = _Client.Client.RemoteEndPoint.ToString.Split(":")(0)
                If RemoteEndPoint = MainformRef.ConfigFile.IP Then
                    If Not MainformRef.ConfigFile.Status = "Hosting" Then Player = 2
                Else
                    If MainformRef.ConfigFile.Status = "Hosting" Then Player = 2
                End If
                InternalPlayer = Player
            Else
                Player = InternalPlayer
            End If

        Else
            StreamToUse = IncomingClient.GetStream
            If StreamPlayer = 0 Then

                Dim RemoteEndPoint As String = _Client.Client.RemoteEndPoint.ToString.Split(":")(0)
                If RemoteEndPoint = MainformRef.ConfigFile.IP Then
                    If Not MainformRef.ConfigFile.Status = "Hosting" Then Player = 2
                Else
                    If MainformRef.ConfigFile.Status = "Hosting" Then Player = 2
                End If
                StreamPlayer = Player
            Else
                Player = StreamPlayer
            End If

        End If

        StreamToUse.Write(_bytes, 0, _bytes.Count) ' Relay The Data

        ' Write to save frame buffer
        Dim Frame As Int32 = BitConverter.ToInt32({_bytes(0), _bytes(1), _bytes(2), _bytes(3)}, 0)
        BearPlay.FrameDataToWrite.Add(New FrameData(Frame, {_bytes(4), _bytes(5), _bytes(6), _bytes(7)}, Player))

    End Sub


    Private Sub StartStream(ByRef _client As TcpClient)
        Try
            While _client.GetStream.CanRead

                Dim NetStream As NetworkStream = _client.GetStream()
                Dim bytes(7) As Byte
                NetStream.Read(bytes, 0, 8)

                'Dim InputString = Convert.ToString(bytes(4), 2).PadLeft(8, "0")
                'InputString += Convert.ToString(bytes(5), 2).PadLeft(8, "0")

                RelayData(bytes, _client)

            End While

        Catch ex As Exception
            'MsgBox(ex.Message)

        End Try
        _client.Close()

    End Sub

End Class

Class FrameData
    Public Frame = 0
    Public Inputs As Byte()
    Public Player As Int16

    Public Sub New(ByVal _frame As Int32, ByVal _inputs As Byte(), ByVal _player As Int16)
        Frame = _frame
        Inputs = _inputs
        Player = _player
    End Sub

End Class