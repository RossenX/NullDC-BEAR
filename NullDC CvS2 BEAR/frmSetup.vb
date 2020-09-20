Imports System.Net
Imports System.Net.NetworkInformation

Public Class frmSetup

    Dim wavePlayer As New NAudio.Wave.WaveOut

    Private Sub frmSetup_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.NewNullDCBearIcon
        Me.CenterToParent()
        AddHandler cbUseCustomWindowSize.CheckedChanged, AddressOf CustomWindowSizeChanged

        FillSettings()
    End Sub

    Private Sub CustomWindowSizeChanged()
        GBCustomWindowSize.Visible = cbUseCustomWindowSize.Checked
        If cbUseCustomWindowSize.Checked Then
            Me.Height = 442
        Else
            Me.Height = 280
        End If


    End Sub

    Private Sub FillSettings()
        tbPlayerName.Text = MainformRef.ConfigFile.Name
        tb_Volume.Value = MainformRef.ConfigFile.Volume
        tb_eVolume.Value = MainformRef.ConfigFile.EmulatorVolume

        tbPort.Text = MainformRef.ConfigFile.Port
        cbShowConsole.Checked = MainformRef.ConfigFile.ShowConsole

        cbUseCustomWindowSize.Checked = CBool(MainformRef.ConfigFile.WindowSettings.Split("|")(0))
        tbCWX.Text = CInt(MainformRef.ConfigFile.WindowSettings.Split("|")(1))
        tbCWY.Text = CInt(MainformRef.ConfigFile.WindowSettings.Split("|")(2))
        tbCWWidth.Text = CInt(MainformRef.ConfigFile.WindowSettings.Split("|")(3))
        tbCWHeight.Text = CInt(MainformRef.ConfigFile.WindowSettings.Split("|")(4))
        CustomWindowSizeChanged()

        If MainformRef.ConfigFile.AllowSpectators = 1 Then
            cbAllowSpectators.Text = "Yes"
        Else
            cbAllowSpectators.Text = "No"
        End If
    End Sub

    Private Sub btnSaveExit_Click(sender As Object, e As EventArgs) Handles btnSaveExit.Click
        MainformRef.ConfigFile.Name = tbPlayerName.Text
        MainformRef.ConfigFile.Network = ""
        MainformRef.ConfigFile.Port = tbPort.Text
        MainformRef.ConfigFile.Volume = tb_Volume.Value
        MainformRef.ConfigFile.EmulatorVolume = tb_eVolume.Value
        MainformRef.ConfigFile.ShowConsole = Convert.ToInt32(cbShowConsole.Checked)
        MainformRef.ConfigFile.WindowSettings = CInt(cbUseCustomWindowSize.Checked).ToString & "|" & tbCWX.Text & "|" & tbCWY.Text & "|" & tbCWWidth.Text & "|" & tbCWHeight.Text

        If cbAllowSpectators.Text = "Yes" Then
            MainformRef.ConfigFile.AllowSpectators = 1
        Else
            MainformRef.ConfigFile.AllowSpectators = 0
        End If

        ' Get IP
        Dim nics As NetworkInterface() = NetworkInterface.GetAllNetworkInterfaces()
        For Each netadapter As NetworkInterface In nics
            ' Get the Valid IP
            If netadapter.Name = Rx.MainformRef.ConfigFile.Network Then
                Dim i = 0
                For Each Address In netadapter.GetIPProperties.UnicastAddresses
                    Dim OutAddress As IPAddress = New IPAddress(2130706433)
                    If IPAddress.TryParse(netadapter.GetIPProperties.UnicastAddresses(i).Address.ToString(), OutAddress) Then
                        If OutAddress.AddressFamily = System.Net.Sockets.AddressFamily.InterNetwork Then
                            Rx.MainformRef.ConfigFile.IP = netadapter.GetIPProperties.UnicastAddresses(i).Address.ToString()
                            Exit For
                        End If
                    End If
                    i += 1
                Next
            End If
        Next

        Rx.MainformRef.ConfigFile.SaveFile()
        Me.Close()
    End Sub

#Region "Text Field Limitation"
    Private Sub tbPlayerName_KeyPress(sender As Object, e As KeyPressEventArgs) Handles tbPlayerName.KeyPress
        If Not (Asc(e.KeyChar) = 8) Then
            Dim allowedChars As String = "abcdefghijklmnopqrstuvwxyz1234567890_ "
            If Not allowedChars.Contains(e.KeyChar.ToString.ToLower) Or (Asc(e.KeyChar) = 8) Then
                e.KeyChar = ChrW(0)
                e.Handled = True
            End If
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) 
        frmLoLNerd.Show()

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        AddEntryToFirewall()
    End Sub

    Private Sub AddEntryToFirewall()
        Dim process As New Process
        Dim processStartInfo As New ProcessStartInfo
        processStartInfo.FileName = "cmd.exe"
        processStartInfo.Verb = "runas"
        processStartInfo.UseShellExecute = True
        processStartInfo.CreateNoWindow = True
        processStartInfo.Arguments = String.Format("/c netsh advfirewall firewall delete rule name=""NullDC BEAR"" program=""{0}"" & netsh advfirewall firewall delete rule name=""nulldc.bear.exe"" program=""{0}"" & netsh advfirewall firewall add rule name=""NullDC BEAR"" dir=in action=allow program=""{0}"" enable=yes & netsh advfirewall firewall add rule name=""NullDC BEAR"" dir=out action=allow program=""{0}"" enable=yes", Application.ExecutablePath)
        processStartInfo.Arguments = processStartInfo.Arguments &
                                     String.Format(" & netsh advfirewall firewall delete rule name=""NullDC"" program=""{0}"" & netsh advfirewall firewall delete rule name=""nulldc_win32_release-notrace.exe"" program=""{0}"" & netsh advfirewall firewall add rule name=""NullDC"" dir=in action=allow program=""{0}"" enable=yes & netsh advfirewall firewall add rule name=""NullDC"" dir=out action=allow program=""{0}"" enable=yes", Application.StartupPath & "\nullDC_Win32_Release-NoTrace.exe")
        processStartInfo.Arguments = processStartInfo.Arguments &
                                     String.Format(" & netsh advfirewall firewall delete rule name=""NullDC DC"" program=""{0}"" & netsh advfirewall firewall delete rule name=""nulldc_win32_release-notrace.exe"" program=""{0}"" & netsh advfirewall firewall add rule name=""NullDC DC"" dir=in action=allow program=""{0}"" enable=yes & netsh advfirewall firewall add rule name=""NullDC DC"" dir=out action=allow program=""{0}"" enable=yes", Application.StartupPath & "\dc\nullDC_Win32_Release-NoTrace.exe")
        Dim Firewall = Process.Start(processStartInfo)

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If Not Application.OpenForms().OfType(Of frmKeyMapperSDL).Any Then
            frmKeyMapperSDL.Show(Me)
        End If
    End Sub

    Private Sub tb_Volume_MouseCaptureChanged(sender As TrackBar, e As EventArgs) Handles tb_Volume.MouseCaptureChanged, tb_eVolume.MouseCaptureChanged
        wavePlayer.Dispose()
        wavePlayer = New NAudio.Wave.WaveOut
        Dim ChallangeSound As New NAudio.Wave.WaveFileReader(My.Resources.NewChallanger)
        wavePlayer.Init(ChallangeSound)
        wavePlayer.Volume = sender.Value / 100
        wavePlayer.Play()

    End Sub

    Private Sub frmSetup_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        wavePlayer.Dispose()
    End Sub

    Private Sub tbPort_KeyPress(sender As Object, e As KeyPressEventArgs) Handles tbPort.KeyPress
        If Not (Asc(e.KeyChar) = 8) Then
            Dim allowedChars As String = "0123456789"
            If Not allowedChars.Contains(e.KeyChar.ToString.ToLower) Or (Asc(e.KeyChar) = 8) Then
                e.KeyChar = ChrW(0)
                e.Handled = True
            End If
        End If
    End Sub

    Private Sub tbCWHeight_KeyPress(sender As Object, e As KeyPressEventArgs) Handles tbCWHeight.KeyPress
        If Not (Asc(e.KeyChar) = 8) Then
            Dim allowedChars As String = "0123456789"
            If Not allowedChars.Contains(e.KeyChar.ToString.ToLower) Or (Asc(e.KeyChar) = 8) Then
                e.KeyChar = ChrW(0)
                e.Handled = True
            End If
        End If
    End Sub

    Private Sub tbCWWidth_KeyPress(sender As Object, e As KeyPressEventArgs) Handles tbCWWidth.KeyPress
        If Not (Asc(e.KeyChar) = 8) Then
            Dim allowedChars As String = "0123456789"
            If Not allowedChars.Contains(e.KeyChar.ToString.ToLower) Or (Asc(e.KeyChar) = 8) Then
                e.KeyChar = ChrW(0)
                e.Handled = True
            End If
        End If
    End Sub

    Private Sub tbCWX_KeyPress(sender As Object, e As KeyPressEventArgs) Handles tbCWX.KeyPress
        If Not (Asc(e.KeyChar) = 8) Then
            Dim allowedChars As String = "0123456789"
            If Not allowedChars.Contains(e.KeyChar.ToString.ToLower) Or (Asc(e.KeyChar) = 8) Then
                e.KeyChar = ChrW(0)
                e.Handled = True
            End If
        End If
    End Sub

    Private Sub tbCWY_KeyPress(sender As Object, e As KeyPressEventArgs) Handles tbCWY.KeyPress
        If Not (Asc(e.KeyChar) = 8) Then
            Dim allowedChars As String = "0123456789"
            If Not allowedChars.Contains(e.KeyChar.ToString.ToLower) Or (Asc(e.KeyChar) = 8) Then
                e.KeyChar = ChrW(0)
                e.Handled = True
            End If
        End If
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        tbCWX.Text = 250
        tbCWY.Text = 250
        tbCWWidth.Text = 656
        tbCWHeight.Text = 538
    End Sub

#End Region

End Class