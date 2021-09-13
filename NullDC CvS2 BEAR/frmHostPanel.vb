Imports System.Net.NetworkInformation
Imports System.Threading

Public Class frmHostPanel

    Private Ping As Int16 = 0
    Dim SuggestThread As Thread

    Public Sub New(ByRef _mf As frmMain)
        InitializeComponent()

    End Sub

    Public Sub BeginHost(Optional ByVal _challenger As BEARPlayer = Nothing)
        MainformRef.Challenger = _challenger

        MainformRef.WaitingForm.Visible = False
        MainformRef.ChallengeSentForm.Visible = False

        MainformRef.ConfigFile.Game = _challenger.game
        MainformRef.ConfigFile.Status = "Hosting"
        MainformRef.ConfigFile.SaveFile()
        Me.Show(MainformRef)

    End Sub

    Private Sub frmHostPanel_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Icon = My.Resources.NewNullDCBearIcon
        cbDelay.Text = "1"
        ReloadTheme()

    End Sub

    Public Sub ReloadTheme()

        ApplyThemeToControl(btnExit)
        ApplyThemeToControl(btnStartHosting)
        ApplyThemeToControl(TableLayoutPanel1, 3)

        ApplyThemeToControl(lbInfo, 3)
        ApplyThemeToControl(lbPing, 3)

        ApplyThemeToControl(Label2, 3)
        ApplyThemeToControl(cb_Serverlist)

        ApplyThemeToControl(Label1, 3)
        ApplyThemeToControl(cbDelay)
        ApplyThemeToControl(btnSuggestDelay)
        ApplyThemeToControl(Button1)
        ApplyThemeToControl(cbRegion)
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
            PictureBox1.Image = BEARTheme.LoadImage(ThemeKeys.HostAnimation)
            PictureBox1.Width = PictureBox1.Image.Width
            PictureBox1.Location = New Point(Me.Width / 2 - PictureBox1.Width / 2, 1)

            PictureBox1.Height = PictureBox1.Image.Height

            Me.BackgroundImage = BEARTheme.LoadImage(ThemeKeys.HostBackground)

            cbRegion.SelectedIndex = MainformRef.ConfigFile.Region

            If Not MainformRef.Challenger Is Nothing Then
                lbInfo.Text = "Hosting " & MainformRef.Challenger.name
                cbDelay.Text = "1"
                lbPing.Text = ""
                Dim Game = MainformRef.Challenger.game
                Console.WriteLine("Game Is:  " & Game)

                Try
                    SuggestThread = New Thread(AddressOf SuggestDelay)
                    SuggestThread.IsBackground = True
                    SuggestThread.Start(True)

                Catch ex As Exception
                    MsgBox("Error Pinging Player")

                End Try

                Dim tempGameName = MainformRef.ConfigFile.Game
                If MainformRef.ConfigFile.Game.StartsWith("FC_") Then
                    tempGameName = tempGameName.Remove(0, 3)
                End If

                Select Case MainformRef.GamesList(tempGameName)(2)
                    Case "na", "fly_na"
                        tb_nulldc.Visible = True
                        tb_mednafen.Visible = False

                    Case "dc", "fly_dc"
                        tb_nulldc.Visible = True
                        tb_mednafen.Visible = False

                        If Rx.VMU Is Nothing Then
                            Rx.VMU = Rx.ReadVMU()
                        End If

                    Case Else
                        tb_nulldc.Visible = False
                        tb_mednafen.Visible = True

                End Select

            Else
                lbInfo.Text = "Hosting Solo"
                cbDelay.Text = "1"
                lbPing.Text = ""
            End If
            Me.CenterToParent()

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

            If MainformRef.ConfigFile.Game.StartsWith("FC_") Or MainformRef.ConfigFile.Game.StartsWith("FLY_") Then
                Dim DelayFrameRate = 100
                Dim delay = Math.Floor(ping.RoundtripTime / DelayFrameRate)
                cbDelay.Invoke(Sub() cbDelay.Text = delay)
                lbPing.Invoke(Sub() lbPing.Text = "Ping: " & ping.RoundtripTime & " | Delay rating: " & (ping.RoundtripTime / DelayFrameRate).ToString("0.##"))

            Else
                Dim DelayFrameRate = 32 '32.66
                Dim delay = Math.Ceiling(ping.RoundtripTime / DelayFrameRate)
                If delay < 2 Then delay = 2
                cbDelay.Invoke(Sub() cbDelay.Text = delay)
                lbPing.Invoke(Sub() lbPing.Text = "Ping: " & ping.RoundtripTime & " | Delay rating: " & (ping.RoundtripTime / DelayFrameRate).ToString("0.##"))
            End If



        Catch ex As Exception

        End Try

    End Sub

    Private Sub btnStartHosting_Click(sender As Object, e As EventArgs) Handles btnStartHosting.Click

        If MainformRef.ConfigFile.Game = "None" Then
            MainformRef.NotificationForm.ShowMessage("No Game Selected")
            Exit Sub
        End If


        Dim tempGameName = MainformRef.ConfigFile.Game.Split("-")(0).ToLower

        Select Case tempGameName
            Case "na", "dc"
                tb_nulldc.Visible = True
                tb_mednafen.Visible = False
                MainformRef.ConfigFile.Host = "127.0.0.1"
                MainformRef.ConfigFile.Status = "Hosting"
            Case "fc_na", "fc_dc", "fly_dc", "fly_na"
                tb_nulldc.Visible = True
                tb_mednafen.Visible = False
                MainformRef.ConfigFile.Host = "127.0.0.1"
                MainformRef.ConfigFile.Status = "Hosting"

            Case Else
                tb_nulldc.Visible = False
                tb_mednafen.Visible = True
                MainformRef.MednafenLauncher.IsHost = True

                If cb_Serverlist.Text = "Localhost" Then
                    MainformRef.ConfigFile.Host = "127.0.0.1"
                    MainformRef.ConfigFile.Status = "Hosting"
                Else
                    MainformRef.ConfigFile.Host = cb_Serverlist.SelectedValue
                    MainformRef.ConfigFile.Status = "Public"
                End If

        End Select

        Dim HostType As String = "0"
        Dim RomName As String = MainformRef.ConfigFile.Game
        If Not MainformRef.Challenger Is Nothing Then RomName = MainformRef.Challenger.game

        MainformRef.ConfigFile.Delay = cbDelay.Text
        MainformRef.ConfigFile.Game = RomName
        MainformRef.ConfigFile.ReplayFile = ""
        MainformRef.ConfigFile.SaveFile()

        MainformRef.GameLauncher(MainformRef.ConfigFile.Game, cbRegion.Text)
        Me.Close()

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Not Application.OpenForms().OfType(Of frmDelayHelp).Any Then
            frmDelayHelp.Show(Me)
        Else
            frmDelayHelp.Focus()
        End If
    End Sub

    Private Sub cbRegion_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbRegion.SelectedIndexChanged
        MainformRef.ConfigFile.Region = cbRegion.SelectedIndex
        MainformRef.ConfigFile.SaveFile(False)

    End Sub
End Class