Imports System.Net
Imports System.Threading


Public Class frmDM

    Public UserIP As String = ""
    Dim wavePlayer As New NAudio.Wave.WaveOut

    Public Sub New(ByVal _ip As String, ByVal _name As String)
        UserIP = _ip
        InitializeComponent()

        Me.Text = _name

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
    End Sub

    Public Sub SendDM()
        If InputBox.Text.Trim.Length > 0 Then
            AddMessageToWindow(MainformRef.ConfigFile.Name & ": " & InputBox.Text)
            MainformRef.NetworkHandler.SendMessage("M," & MainformRef.ConfigFile.Name & "," & WebUtility.UrlEncode(InputBox.Text), UserIP)
            InputBox.Clear()
        End If

    End Sub

    Public Sub RecieveDM(ByVal _ip As String, ByVal _message As String)
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

                Console.WriteLine(TLP_Messages.RowCount)
                FlowLayoutPanel1.VerticalScroll.Value = FlowLayoutPanel1.VerticalScroll.Maximum
                Console.WriteLine(FlowLayoutPanel1.VerticalScroll.Maximum)

            End Sub)

    End Sub

    Private Sub btnSend_Click(sender As Object, e As EventArgs) Handles btnSend.Click
        SendDM()

    End Sub

    Private Sub InputBox_KeyDown(sender As Object, e As KeyEventArgs) Handles InputBox.KeyDown
        If e.KeyCode = Keys.Enter Then
            SendDM()
            e.Handled = True
            e.SuppressKeyPress = True

        End If

    End Sub

    Private Sub AddMessageToWindow(ByVal _message As String)

        Dim NewMessage As Label = New Label
        NewMessage.Text = _message

        NewMessage.AutoSize = True
        NewMessage.TextAlign = ContentAlignment.BottomLeft
        NewMessage.Font = New Font("Arial", 10, FontStyle.Bold)

        TLP_Messages.Controls.Add(NewMessage, 0, TLP_Messages.RowCount - 1)
        TLP_Messages.RowStyles.Add(New RowStyle(SizeType.AutoSize, 50.0F))
        TLP_Messages.RowCount += 1
        FlowLayoutPanel1.VerticalScroll.Visible = True

    End Sub

    Private Sub TLP_Messages_MouseHover(sender As Object, e As EventArgs) Handles TLP_Messages.MouseEnter, FlowLayoutPanel1.MouseEnter, TLP_Messages.MouseMove, FlowLayoutPanel1.MouseMove
        FlowLayoutPanel1.Focus()

    End Sub

    Private Sub frmDM_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If Char.IsLetterOrDigit(ChrW(e.KeyCode).ToString) Then

            If Not InputBox.Focused Then
                InputBox.Focus()
                If e.Shift Then
                    InputBox.Text += ChrW(e.KeyValue).ToString.ToUpper
                Else
                    InputBox.Text += ChrW(e.KeyValue).ToString.ToLower
                End If

                InputBox.SelectionStart = InputBox.Text.Length
            End If
        End If

    End Sub

End Class