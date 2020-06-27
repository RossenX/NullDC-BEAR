Imports OpenTK
Imports System.Net.Sockets
Imports System.IO
Imports System.Text
Imports System.Net

Public Class NetPlayHandler

    ' Right ok so main goal of this class is to
    ' 1. Create a Mock Server on port to get incoming communications
    ' 2. Send that data to real host, but save a copy of it for replay
    ' 3. Host sending stuff to me, relay that back to the client after recording it
    ' This should create a perfect replay for a netplay sessions
    ' Possible use for spectator not sure yet
    ' IDEA: Have spectators connect to mock host, but then clients can only recieve p1 inputs, so that's a no.
    ' IDEA: Use as relay host, to get data from client send to host and vice versa to get full playback info? Kinda wasteful might cause lag and shit
    ' IDEA: Make mock client/host and allow people to connect to those if they want to save replays

    Dim RxClient As TcpClient
    Dim RxDedicatedServer As TcpListener
    Dim RxReader As StreamReader
    Dim RxWriter As StreamWriter
    Dim ClientCount = 0
    Dim Clients As New Dictionary(Of Int16, ClientHandler)
    Dim MainFormRef As frmMain

    ' Packet: 8 Bytes 
    '       0-3: 00 00 00 00<Frame Count 32 bit integer split into 4 bytes> 
    '       4-5: 00 00 <Inputs as Binary STUDLR123456???? > When Sending, sends your inputs. When recieving, recives opponent inputs. 
    '       6-7: 00 00 <Coin Data? Increases as the coin button is pressed, probably was meant to keep coin data in sync but wasn't finished>

    Public Sub New(ByRef _mf As frmMain)
        MainFormRef = _mf
        Try

            Dim localaddress As IPAddress = IPAddress.Parse("127.0.0.1")
            RxDedicatedServer = New TcpListener(localaddress, 27886)

            Dim DedicatedServerThread = New Threading.Thread(AddressOf StartServer)
            DedicatedServerThread.IsBackground = True
            DedicatedServerThread.Start()

            'RxClient = New TcpClient("127.0.0.1", 27886)
            'If RxClient.GetStream.CanRead = True Then
            'RxReader = New StreamReader(RxClient.GetStream)
            'RxWriter = New StreamWriter(RxClient.GetStream)
            'Threading.ThreadPool.QueueUserWorkItem(AddressOf Connected)
            'End If

        Catch ex As Exception
            MsgBox(ex.Message)

        End Try

    End Sub

    Public Sub InputRecieved(ByRef Client As ClientHandler)
        Console.WriteLine("Got Frame From: {0}:{1}", Client.ClientID, BitConverter.ToString(Client.LastFrameData, 0))
        Dim InputString = Convert.ToString(Client.LastFrameData(4), 2).PadLeft(8, "0")
        InputString += Convert.ToString(Client.LastFrameData(5), 2).PadLeft(8, "0")
        Console.WriteLine("Input Data: {0}", InputString)

        Dim s As New FileStream(MainFormRef.NullDCPath & "\playback.bearplay", FileMode.Append, FileAccess.Write, FileShare.ReadWrite)
        Dim Framedata(7) As Byte
        Framedata(0) = Client.LastFrameData(4)
        Framedata(1) = Client.LastFrameData(5)
        Framedata(2) = Client.LastFrameData(6)
        Framedata(3) = Client.LastFrameData(7)

        Framedata(4) = Client.LastFrameData(4)
        Framedata(5) = Client.LastFrameData(5)
        Framedata(6) = Client.LastFrameData(6)
        Framedata(7) = Client.LastFrameData(7)

        s.Write(Framedata, 0, Framedata.Length)
        s.Close()

        For i = 0 To Clients.Count - 1
            Clients(i).LoopBackLastInfo()
        Next

    End Sub

    Private Sub StartServer()
        RxDedicatedServer.Start()

        While True
            Try
                Dim IncomingClient = RxDedicatedServer.AcceptTcpClient()
                Clients.Add(ClientCount, New ClientHandler(IncomingClient, ClientCount, Me))
                ClientCount += 1

            Catch ex As Exception
                MsgBox(ex.Message)

            End Try

        End While

    End Sub

    Public Sub SeneMessage(ByVal sendBytes As Byte())

        Try
            RxClient.GetStream.Write(sendBytes, 0, sendBytes.Length)

            Console.Write(">")
            For Each _byte In sendBytes
                Console.Write(_byte & " ")
            Next
            Console.Write(vbNewLine)

        Catch ex As Exception
            MsgBox(ex.Message)

        End Try

    End Sub

End Class


Public Class ClientHandler
    Public Client As TcpClient
    Public ClientID As Int16
    Public LastFrameNumber As Integer = 0
    Public InputData As String = ""
    Public NetPlayHandler As NetPlayHandler
    Public LastFrameData(7) As Byte
    Public ClientConnected As Boolean = False

    Public Event InputDataRecieved(ByRef Client As ClientHandler)

    Public Sub New(ByRef _client As TcpClient, ByVal _clientID As Int16, ByRef _Netplayhandler As NetPlayHandler)
        Client = _client
        ClientID = _clientID
        NetPlayHandler = _Netplayhandler
        Console.WriteLine("Connected: {0}", _clientID)
        ClientConnected = True

        AddHandler Me.InputDataRecieved, AddressOf _Netplayhandler.InputRecieved

        Dim StreamThread As New Threading.Thread(AddressOf StartGettingStream)
        StreamThread.IsBackground = True
        StreamThread.Start()
    End Sub

    Public Sub LoopBackLastInfo()
        Client.GetStream.Write(LastFrameData, 0, 8)
    End Sub

    Private Sub StartGettingStream()
        Try
            While Client.GetStream.CanRead

                Dim NetStream As NetworkStream = Client.GetStream()
                Dim bytes(7) As Byte
                NetStream.Read(bytes, 0, 8)
                For i = 0 To LastFrameData.Count - 1
                    LastFrameData(i) = bytes(i)
                Next
                RaiseEvent InputDataRecieved(Me)

            End While

        Catch ex As Exception
            MsgBox(ex.Message)

        End Try
        Client.Close()

    End Sub

End Class