Imports System.Net.NetworkInformation

Module BEARPinger
    Dim PingQueue

    Public Sub PingPlayer(ByVal _ip As String, ByRef _ListView As ListViewItem)
        Dim ping = New Ping().SendPingAsync(_ip, 5000)

    End Sub

End Module
