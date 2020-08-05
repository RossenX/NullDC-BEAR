Imports System.Threading
Imports NullDC_CvS2_BEAR.frmMain
Imports NullDC_CvS2_BEAR.NetworkHandling
Imports System.Windows

Public Class frmChallenge

    Dim Timeout As Forms.Timer = New Forms.Timer
    Dim wavePlayer As New NAudio.Wave.WaveOut

    Public Sub New(ByRef _mf As frmMain)
        InitializeComponent()

    End Sub

    Private Sub btnDeny_Click(sender As Object, e As EventArgs) Handles btnDeny.Click
        MainFormRef.NetworkHandler.SendMessage(">,D", MainFormRef.Challenger.ip)
        MainFormRef.EndSession("Denied")
        MainformRef.Focus()
    End Sub

    Public Sub StartChallenge(ByRef _challenger As NullDCPlayer)
        MainFormRef.Challenger = _challenger
        Me.Show()

    End Sub

    Private Sub frmChallenge_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.NewNullDCBearIcon
        AddHandler Timeout.Tick, AddressOf Timeout_tick

    End Sub

    Private Sub Timeout_tick(sender As Object, e As EventArgs)
        If Not Visible Then Exit Sub
        If Not MainFormRef.Challenger Is Nothing Then
            MainFormRef.NetworkHandler.SendMessage(">,T", MainFormRef.Challenger.ip)
        End If

        Timeout.Stop()
        Dim INVOKATION As EndSession_delegate = AddressOf MainFormRef.EndSession
        MainFormRef.Invoke(INVOKATION, {"TO", Nothing})

        frmMain.ConfigFile.Game = "None"
        frmMain.ConfigFile.Status = MainformRef.Configfile.awaystatus
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
        If MainFormRef.IsNullDCRunning Then MainFormRef.EndSession("New Challenger")
        While MainFormRef.IsNullDCRunning
            Thread.Sleep(10)
        End While

        MainFormRef.ConfigFile.Port = MainFormRef.Challenger.port
        MainFormRef.ConfigFile.Status = "Client"
        MainFormRef.ConfigFile.Game = MainFormRef.Challenger.game
        MainFormRef.ConfigFile.SaveFile()
        MainFormRef.NetworkHandler.SendMessage("^," & MainFormRef.ConfigFile.Name & "," & MainFormRef.ConfigFile.IP & "," & MainFormRef.ConfigFile.Port & "," & MainFormRef.ConfigFile.Game, MainFormRef.Challenger.ip)
        MainFormRef.WaitingForm.Show()
        Me.Close()

    End Sub

    Private Sub frmChallenge_VisibleChanged(sender As Object, e As EventArgs) Handles MyBase.VisibleChanged
        If Visible Then

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
        wavePlayer.Dispose()
    End Sub


End Class