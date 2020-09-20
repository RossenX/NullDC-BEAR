Imports System.Threading
Imports NullDC_CvS2_BEAR.frmMain
Imports NullDC_CvS2_BEAR.NetworkHandling
Imports System.Windows
Imports System.Net.NetworkInformation

Public Class frmChallenge

    Dim Timeout As Forms.Timer = New Forms.Timer
    Dim wavePlayer As New NAudio.Wave.WaveOut

    Public Sub New(ByRef _mf As frmMain)
        InitializeComponent()

    End Sub

    Private Sub btnDeny_Click(sender As Object, e As EventArgs) Handles btnDeny.Click
        MainformRef.NetworkHandler.SendMessage(">,D", MainformRef.Challenger.ip)
        MainformRef.EndSession("Denied")
        MainformRef.Focus()
    End Sub

    Public Sub StartChallenge(ByRef _challenger As NullDCPlayer)
        MainformRef.Challenger = _challenger
        Me.Show()

    End Sub

    Private Sub frmChallenge_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.NewNullDCBearIcon
        AddHandler Timeout.Tick, AddressOf Timeout_tick

    End Sub

    Private Sub GetPing()
        Try
            Dim ping As PingReply = New Ping().Send(MainformRef.Challenger.ip)
            If ping.RoundtripTime = 0 Then
                Label1.Invoke(Sub() Label1.Text = "Ping: N/A")
                Exit Sub
            End If
            Dim DelayFrameRate = 32.66 '32.66
            Dim delay = Math.Ceiling(ping.RoundtripTime / DelayFrameRate)
            If delay = 0 Then delay = 1
            Label1.Invoke(Sub() Label1.Text = "Ping: " & ping.RoundtripTime & vbNewLine & "Delay: " & (ping.RoundtripTime / DelayFrameRate).ToString("0.##"))
        Catch ex As Exception
            Label1.Invoke(Sub() Label1.Text = "Ping: N/A")
        End Try

    End Sub

    Private Sub Timeout_tick(sender As Object, e As EventArgs)
        If Not Visible Then Exit Sub
        If Not MainformRef.Challenger Is Nothing Then
            MainformRef.NetworkHandler.SendMessage(">,T", MainformRef.Challenger.ip)
        End If

        Timeout.Stop()
        Dim INVOKATION As EndSession_delegate = AddressOf MainformRef.EndSession
        MainformRef.Invoke(INVOKATION, {"TO", Nothing})

        frmMain.ConfigFile.Game = "None"
        frmMain.ConfigFile.Status = MainformRef.ConfigFile.AwayStatus
        frmMain.ConfigFile.SaveFile()
    End Sub

#Region "Moving Window"
    ' Moving
    Private newpoint As System.Drawing.Point
    Private xpos1 As Integer
    Private ypos1 As Integer

    Private Sub pnlTopBorder_MouseDown(sender As Object, e As MouseEventArgs) Handles MyBase.MouseDown
        xpos1 = Control.MousePosition.X - Me.Location.X
        ypos1 = Control.MousePosition.Y - Me.Location.Y
    End Sub

    Private Sub pnlTopBorder_MouseMove(sender As Object, e As MouseEventArgs) Handles MyBase.MouseMove
        If e.Button = MouseButtons.Left Then
            newpoint = Control.MousePosition
            newpoint.X -= (xpos1)
            newpoint.Y -= (ypos1)
            Me.Location = newpoint
        End If

    End Sub
#End Region

    Private Sub btnAccept_Click(sender As Object, e As EventArgs) Handles btnAccept.Click
        If MainformRef.IsNullDCRunning Then MainformRef.EndSession("New Challenger")
        While MainformRef.IsNullDCRunning
            Thread.Sleep(10)
        End While

        MainformRef.ConfigFile.Port = MainformRef.Challenger.port
        MainformRef.ConfigFile.Status = "Client"
        MainformRef.ConfigFile.Game = MainformRef.Challenger.game
        MainformRef.ConfigFile.SaveFile()
        MainformRef.NetworkHandler.SendMessage("^," & MainformRef.ConfigFile.Name & "," & MainformRef.ConfigFile.IP & "," & MainformRef.ConfigFile.Port & "," & MainformRef.ConfigFile.Game & "," & MainformRef.ConfigFile.Peripheral, MainformRef.Challenger.ip)
        MainformRef.WaitingForm.Show()
        Me.Close()

    End Sub

    Private Sub frmChallenge_VisibleChanged(sender As Object, e As EventArgs) Handles MyBase.VisibleChanged
        If Visible Then
            Dim pingThread As Thread = New Thread(AddressOf GetPing)
            pingThread.IsBackground = True
            pingThread.Start()

            wavePlayer.Dispose()
            wavePlayer = New NAudio.Wave.WaveOut
            Dim ChallangeSound As New NAudio.Wave.WaveFileReader(My.Resources.NewChallanger)
            wavePlayer.Init(ChallangeSound)
            wavePlayer.Volume = MainformRef.ConfigFile.Volume / 100
            wavePlayer.Play()

            'My.Computer.Audio.Play(My.Resources.NewChallanger, AudioPlayMode.Background)
            Timeout.Interval = 10000
            'Timeout.Start()
            Dim GameName As String = MainformRef.GamesList(MainformRef.Challenger.game)(0)
            lbChallengeText.Text = MainformRef.Challenger.name & " Has challenged you to " & vbCrLf & GameName
        Else
            Timeout.Stop()

        End If

    End Sub

    Private Sub frmChallenge_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        e.Cancel = True
        Me.Visible = False
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim pingThread As Thread = New Thread(AddressOf GetPing)
        pingThread.IsBackground = True
        pingThread.Start()
    End Sub

End Class