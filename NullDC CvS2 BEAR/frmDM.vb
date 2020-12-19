Imports System.Net

Public Class frmDM

    Public UserIP As String = ""
    Public UserName As String = ""
    Dim wavePlayer As New NAudio.Wave.WaveOut

    Public Sub New(ByVal _ip As String, ByVal _name As String)
        UserIP = _ip
        UserName = _name

        InitializeComponent()



    End Sub

    Private Sub frmDM_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.NewNullDCBearIcon
        Me.Name = UserName

    End Sub

    Public Sub SendDM(ByVal _message)
        If Not InputBox.Text.Trim.Length > 0 Then
            MainformRef.NetworkHandler.SendMessage("M," & MainformRef.ConfigFile.Name & "," & WebUtility.UrlEncode(InputBox.Text), UserIP)

        End If

    End Sub

    Public Sub RecieveDM(ByVal _ip, ByVal _message)
        If Not Me.Focused Then
            WindowsApi.FlashWindow(Me.Handle, True, True, 1)

            Try
                wavePlayer.Dispose()
                wavePlayer = New NAudio.Wave.WaveOut
                wavePlayer.Init(New NAudio.Wave.WaveFileReader(My.Resources.MessagePop))
                wavePlayer.Volume = MainformRef.ConfigFile.Volume / 100
                wavePlayer.Play()
            Catch ex As Exception

            End Try

        End If







    End Sub

End Class