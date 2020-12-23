Imports System.Net
Imports System.Threading


Public Class frmDM

    Public UserIP As String = ""
    Public UserName As String = ""
    Public over As Boolean = False

    Dim wavePlayer As New NAudio.Wave.WaveOut
    Private MessageTimer As System.Windows.Forms.Timer = New System.Windows.Forms.Timer

    Protected Overrides ReadOnly Property ShowWithoutActivation() As Boolean
        Get
            Return True
        End Get
    End Property

    Public Sub New(ByVal _ip As String, ByVal _name As String)
        InitializeComponent()

        UserIP = _ip
        UserName = _name

        Me.Text = UserName

        MessageTimer.Interval = 2000

        AddHandler MessageTimer.Tick, Sub()
                                          MessageTimer.Stop()
                                          btnSend.Enabled = True
                                      End Sub

    End Sub

    Private Sub frmDM_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.NewNullDCBearIcon
        Me.CenterToParent()
        ReloadTheme()
    End Sub

    Public Sub ReloadTheme()
        ApplyThemeToControl(tlpContainer)
        ApplyThemeToControl(btnSend)
        ApplyThemeToControl(FlowLayoutPanel1, 2)
        ApplyThemeToControl(MenuStrip1)

    End Sub

    Public Sub SendDM()
        If InputBox.Text.Trim.Length > 0 Then
            If MessageTimer.Enabled Then
                Exit Sub
            End If

            AddMessageToWindow(MainformRef.ConfigFile.Name & ": " & InputBox.Text)
            If Not over Then
                MainformRef.NetworkHandler.SendMessage("M," & MainformRef.ConfigFile.Name & "," & WebUtility.UrlEncode(InputBox.Text), UserIP)
            End If
            MessageTimer.Start()
            btnSend.Enabled = False
            InputBox.Clear()

        End If

    End Sub

    Public Sub RecieveDM(ByVal _ip As String, ByVal _message As String)
        over = False

        Me.Invoke(
            Sub()
                If Not Me.ContainsFocus Then
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

                AddMessageToWindow(_message)

            End Sub)

    End Sub

    Private Sub btnSend_Click(sender As Object, e As EventArgs) Handles btnSend.Click
        SendDM()

    End Sub

    Public Sub AddMessageToWindow(ByVal _message As String)

        Dim NewMessage As Label = New Label
        NewMessage.Text = _message

        NewMessage.AutoSize = True
        NewMessage.TextAlign = ContentAlignment.BottomLeft
        NewMessage.Font = New Font("Arial", 10, FontStyle.Bold)

        TLP_Messages.Controls.Add(NewMessage, 0, TLP_Messages.RowCount - 1)
        TLP_Messages.RowStyles.Add(New RowStyle(SizeType.AutoSize, 50.0F))
        TLP_Messages.RowCount += 1

        FlowLayoutPanel1.VerticalScroll.Value = FlowLayoutPanel1.VerticalScroll.Maximum

    End Sub

    Private Sub TLP_Messages_MouseHover(sender As Object, e As EventArgs) Handles TLP_Messages.MouseEnter, FlowLayoutPanel1.MouseEnter, TLP_Messages.MouseMove, FlowLayoutPanel1.MouseMove
        If Me.ContainsFocus Then
            FlowLayoutPanel1.Focus()
        End If

    End Sub

    Private Sub frmDM_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown

        If e.KeyCode = Keys.Menu Then
            e.Handled = True
            e.SuppressKeyPress = True
            Exit Sub
        End If

        ' If it's a character and the box is not focused, add the text to it then focus the box for additional text, if it's already focused we skip this
        If Char.IsLetterOrDigit(ChrW(e.KeyCode).ToString) And Not InputBox.Focused Then
            If e.Shift Then
                InputBox.Text += ChrW(e.KeyValue).ToString.ToUpper
            Else
                InputBox.Text += ChrW(e.KeyValue).ToString.ToLower
            End If
            InputBox.SelectionStart = InputBox.Text.Length

        End If

        InputBox.Focus()

        If e.KeyCode = Keys.Enter Then
            e.Handled = True
            e.SuppressKeyPress = True
            SendDM()
        End If

    End Sub

    Private Sub GagToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles GagToolStripMenuItem1.Click
        If IsUserGagged(UserIP) Then
            Me.Close()

        Else
            GagUser(UserIP, UserName)
            Me.Close()

        End If
        MainformRef.NotificationForm.ShowMessage("Player Gagged, That's the end of that.")

    End Sub

End Class