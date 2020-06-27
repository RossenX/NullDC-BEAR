Imports System.Net.NetworkInformation
Imports NullDC_CvS2_BEAR.frmMain

Public Class frmHostPanel

    Dim MainformRef As frmMain
    Private Ping As Int16 = 0

    Public Sub New(ByRef _mf As frmMain)
        InitializeComponent()
        MainformRef = _mf

    End Sub

    Public Sub BeginHost(Optional ByVal _challenger As NullDCPlayer = Nothing)
        MainformRef.Challenger = _challenger
        MainformRef.EndSession("New Host")
        Me.Show(MainformRef)
    End Sub

    Private Sub frmHostPanel_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Icon = My.Resources.NewNullDCBearIcon
        cbDelay.Text = "1"
        cbGameList.SelectedText = 0

    End Sub

    Private Sub frmHostPanel_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        e.Cancel = True
        Visible = False
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        If Not MainformRef.Challenger Is Nothing Then MainformRef.NetworkHandler.SendMessage(">,H", MainformRef.Challenger.ip)
        MainformRef.EndSession("Host Canceled")
    End Sub

    Private Sub HostPanel_VisibleChanged(sender As Object, e As EventArgs) Handles Me.VisibleChanged
        If Visible = True Then
            cbHostType.SelectedIndex = 0
            If MainformRef.ConfigFile.HostType = "1" Then
                cbHostType.SelectedIndex = 0
            Else
                cbHostType.SelectedIndex = 1
            End If
            tbFPS.Text = MainformRef.ConfigFile.FPSLimit
            If Not MainformRef.Challenger Is Nothing Then
                lbInfo.Text = "Hosting " & MainformRef.Challenger.name
                cbDelay.Text = "1"
                lbPing.Text = ""
                cbGameList.Visible = False
                Dim Game = MainformRef.Challenger.game
                Console.WriteLine("Game Is:  " & Game)
                If MainformRef.Challenger.game = "None" Then cbGameList.Visible = True
                SuggestDelay()
            Else
                lbInfo.Text = "Hosting Solo"
                cbDelay.Text = "1"
                lbPing.Text = ""
                cbGameList.Visible = True
            End If

            Me.CenterToParent()
        Else
        End If

    End Sub

    Private Sub btnSuggestDelay_Click(sender As Object, e As EventArgs) Handles btnSuggestDelay.Click
        If MainformRef.Challenger Is Nothing Then
            MainformRef.NotificationForm.ShowMessage("I can't predict the future, unless you're hosting someone i can't suggest a delay for you")
        Else
            SuggestDelay()
        End If

    End Sub

    Private Sub SuggestDelay()
        Dim ping = New Ping().Send(MainformRef.Challenger.ip).RoundtripTime
        If ping = 0 Then
            MainformRef.NotificationForm.ShowMessage("Coulnd't ping the player. Make sure you and your challanger are not behind a firewall or something.")
            Exit Sub
        End If
        Dim DelayFrameRate = 32.66
        Dim delay = Math.Ceiling(ping / DelayFrameRate)
        If delay = 0 Then delay = 1
        cbDelay.Text = delay
        lbPing.Text = "Ping: " & ping & " | Delay rating: " & (ping / DelayFrameRate).ToString("0.##")
        If delay > 7 Then
            MainformRef.NotificationForm.ShowMessage("Delay > 7 has a VERY HIGH chance of desync!")
        End If
    End Sub

    Private Sub btnStartHosting_Click(sender As Object, e As EventArgs) Handles btnStartHosting.Click
        Dim HostType As String = "0"
        Dim RomName As String = cbGameList.SelectedValue
        If Not MainformRef.Challenger Is Nothing Then RomName = MainformRef.Challenger.game

        MainformRef.ConfigFile.Host = MainformRef.ConfigFile.IP
        MainformRef.ConfigFile.Status = "Hosting"
        MainformRef.ConfigFile.Delay = cbDelay.Text
        MainformRef.ConfigFile.FPSLimit = tbFPS.Text
        MainformRef.ConfigFile.Game = RomName

        If cbHostType.Text = "Audio Sync" Then HostType = "1"

        MainformRef.ConfigFile.HostType = HostType
        MainformRef.ConfigFile.SaveFile()

        MainformRef.NullDCLauncher.LaunchDC(MainformRef.ConfigFile.Game)
        Me.Close()

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        frmDelayHelp.Show()
    End Sub

    Private Sub tbFPS_KeyPress(sender As TextBox, e As KeyPressEventArgs) Handles tbFPS.KeyPress
        If Not (Asc(e.KeyChar) = 8) Then
            Dim allowedChars As String = "0123456789"
            If Not allowedChars.Contains(e.KeyChar.ToString.ToLower) Or (Asc(e.KeyChar) = 8) Then
                e.KeyChar = ChrW(0)
                e.Handled = True
            End If
        End If
    End Sub

    Private Sub cbHostType_SelectedIndexChanged(sender As ComboBox, e As EventArgs) Handles cbHostType.SelectedIndexChanged
        If sender.Text = "Audio Sync" Then
            tbFPS.Visible = False
            Label3.Visible = False
        Else
            tbFPS.Visible = True
            Label3.Visible = True
        End If
    End Sub

    Private Sub Button2_Click_1(sender As Object, e As EventArgs) Handles Button2.Click
        frmLimiterHelp.Show()
    End Sub
End Class