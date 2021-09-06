Imports System.Threading
Imports System.Net.NetworkInformation

Public Class frmChallenge

    Public _Challenger As BEARPlayer
    Dim wavePlayer As New NAudio.Wave.WaveOut

    Public Sub New(ByRef _mf As frmMain)
        InitializeComponent()

    End Sub

    Private Sub btnDeny_Click(sender As Object, e As EventArgs) Handles btnDeny.Click
        MainformRef.NetworkHandler.SendMessage(">,D", _Challenger.ip)
        MainformRef.EndSession("Denied")
        MainformRef.Focus()
    End Sub

    Public Sub StartChallenge(ByRef _challengerInc As BEARPlayer)
        _Challenger = _challengerInc
        Me.Show()

    End Sub

    Private Sub frmChallenge_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.fan_icon_text
        ReloadTheme()


    End Sub

    Public Sub ReloadTheme()
        Me.BackgroundImage = BEARTheme.LoadImage(ThemeKeys.ChallengeBackground)
        ApplyThemeToControl(btnAccept)
        ApplyThemeToControl(btnDeny)
        ApplyThemeToControl(btnPing)

        ApplyThemeToControl(Label1, 2)
        ApplyThemeToControl(lbChallengeText, 2)
    End Sub

    Private Sub GetPing()
        Try
            Dim ping As PingReply = New Ping().Send(_Challenger.ip)
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
        While MainformRef.IsNullDCRunning
            Thread.Sleep(100)
        End While

        MainformRef.Challenger = _Challenger
        ' Ok so this causes crashes on some systems so lets try to find a workaround MOVED TO VisibleChanged
        MainformRef.ConfigFile.Port = MainformRef.Challenger.port
        MainformRef.ConfigFile.Status = "Client"
        MainformRef.ConfigFile.Game = MainformRef.Challenger.game
        MainformRef.ConfigFile.SaveFile()

        MainformRef.NetworkHandler.SendMessage("^," & MainformRef.ConfigFile.Name & ",," & MainformRef.ConfigFile.Port & "," & MainformRef.ConfigFile.Game & "," & MainformRef.ConfigFile.Peripheral, MainformRef.Challenger.ip)

        MainformRef.WaitingForm.Show()
        Me.Close()

    End Sub

    Private Sub frmChallenge_VisibleChanged(sender As Object, e As EventArgs) Handles MyBase.VisibleChanged
        If Visible Then

            Try
                Dim pingThread As Thread = New Thread(AddressOf GetPing)
                pingThread.IsBackground = True
                pingThread.Start()
                'pingThread.Join()

            Catch ex As Exception
                MsgBox("Error Pinging Player")
            End Try

            Try
                wavePlayer.Dispose()
                wavePlayer = New NAudio.Wave.WaveOut
                Dim ChallangeSound As New NAudio.Wave.WaveFileReader(My.Resources.NewChallanger)
                wavePlayer.Init(ChallangeSound)
                wavePlayer.Volume = MainformRef.ConfigFile.Volume / 100
                wavePlayer.Play()
            Catch ex As Exception

            End Try

            Dim TempGameName = _Challenger.game
            If TempGameName.StartsWith("FC_") Then
                TempGameName = TempGameName.Remove(0, 3)
            End If

            Dim GameName As String = MainformRef.GamesList(TempGameName)(0)
            lbChallengeText.Text = _Challenger.name & " Has challenged you to " & vbCrLf & GameName
        Else
            _Challenger = Nothing
        End If

    End Sub

    Private Sub frmChallenge_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        e.Cancel = True
        Me.Visible = False
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles btnPing.Click
        Dim pingThread As Thread = New Thread(AddressOf GetPing)
        pingThread.IsBackground = True
        pingThread.Start()
    End Sub

End Class