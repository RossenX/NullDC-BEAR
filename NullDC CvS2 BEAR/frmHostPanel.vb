Imports System.Net.NetworkInformation
Imports System.Threading

Public Class frmHostPanel

    Private Ping As Int16 = 0
    Dim SuggestThread As Thread

    Public Sub New(ByRef _mf As frmMain)
        InitializeComponent()

    End Sub

    Public Sub BeginHost(Optional ByVal _challenger As NullDCPlayer = Nothing)
        MainformRef.Challenger = _challenger
        MainformRef.EndSession("New Host")
        Me.Show(MainformRef)

    End Sub

    Private Sub frmHostPanel_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Icon = My.Resources.NewNullDCBearIcon
        cbDelay.Text = "1"
        If cbGameList.Items.Count > 0 Then cbGameList.SelectedIndex = 0

    End Sub

    Private Sub frmHostPanel_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        e.Cancel = True
        Visible = False

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        If Not MainformRef.Challenger Is Nothing Then MainformRef.NetworkHandler.SendMessage(">,H", MainformRef.Challenger.ip)
        MainformRef.EndSession("Host Canceled")
        MainformRef.Focus()

    End Sub

    Private Sub HostPanel_VisibleChanged(sender As Object, e As EventArgs) Handles Me.VisibleChanged
        If Visible = True Then
            If cbRegion.Text = "" Then cbRegion.Text = "JPN"
            If Not MainformRef.Challenger Is Nothing Then
                lbInfo.Text = "Hosting " & MainformRef.Challenger.name
                cbDelay.Text = "1"
                lbPing.Text = ""
                cbGameList.Visible = False
                Dim Game = MainformRef.Challenger.game
                Console.WriteLine("Game Is:  " & Game)
                If MainformRef.Challenger.game = "None" Then cbGameList.Visible = True

                SuggestThread = New Thread(AddressOf SuggestDelay)
                SuggestThread.IsBackground = True
                SuggestThread.Start(True)
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
            Try
                SuggestThread = New Thread(AddressOf SuggestDelay)
                SuggestThread.IsBackground = True
                SuggestThread.Start(False)
            Catch ex As Exception

            End Try

        End If

    End Sub

    Private Sub SuggestDelay(ByVal AutoSuggest As Boolean)
        Try
            Dim ping As PingReply = New Ping().Send(MainformRef.Challenger.ip)
            If ping.RoundtripTime = 0 Then
                If Not AutoSuggest Then MainformRef.NotificationForm.ShowMessage("Coulnd't ping the player. Make sure you and your challanger are not behind a firewall or something.")
                Exit Sub
            End If
            Dim DelayFrameRate = 32.66 '32.66
            Dim delay = Math.Ceiling(ping.RoundtripTime / DelayFrameRate)
            If delay = 0 Then delay = 1
            cbDelay.Invoke(Sub() cbDelay.Text = delay)
            lbPing.Invoke(Sub() lbPing.Text = "Ping: " & ping.RoundtripTime & " | Delay rating: " & (ping.RoundtripTime / DelayFrameRate).ToString("0.##"))
        Catch ex As Exception

        End Try

    End Sub

    Private Sub btnStartHosting_Click(sender As Object, e As EventArgs) Handles btnStartHosting.Click
        If cbGameList.Text = "" Then
            MainformRef.NotificationForm.ShowMessage("No Game Selected")
            Exit Sub
        End If

        Dim HostType As String = "0"
        Dim RomName As String = cbGameList.SelectedValue
        If Not MainformRef.Challenger Is Nothing Then RomName = MainformRef.Challenger.game

        MainformRef.ConfigFile.Host = MainformRef.ConfigFile.IP
        MainformRef.ConfigFile.Status = "Hosting"
        MainformRef.ConfigFile.Delay = cbDelay.Text
        MainformRef.ConfigFile.Game = RomName
        MainformRef.ConfigFile.ReplayFile = ""
        MainformRef.ConfigFile.SaveFile()

        MainformRef.NullDCLauncher.LaunchNaomi(MainformRef.ConfigFile.Game, cbRegion.Text)
        Me.Close()

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Not Application.OpenForms().OfType(Of frmDelayHelp).Any Then
            frmDelayHelp.Show(Me)
        Else
            frmDelayHelp.Focus()
        End If
    End Sub

End Class