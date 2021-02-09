Imports System.IO
Imports System.Net
Imports System.Net.NetworkInformation

Public Class frmSetup

    Dim wavePlayer As New NAudio.Wave.WaveOut
    Dim FormFilled As Boolean = False

    Private Sub frmSetup_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.NewNullDCBearIcon

        FillSettings()
        ReloadTheme()

        Me.CenterToParent()

    End Sub

    Public Sub ReloadTheme()

        ApplyThemeToControl(OptionsContainer, 2)
        For i = 0 To OptionsContainer.Controls.Count - 1
            ApplyThemeToControl(OptionsContainer.Controls(i), 2)
        Next

        For i = 0 To CustomSizeContainer.Controls.Count - 1
            ApplyThemeToControl(CustomSizeContainer.Controls(i), 2)
        Next
        ApplyThemeToControl(Button2)

        Button2.BackColor = Color.FromArgb(0, 255, 0)
        Button2.ForeColor = Color.Black
        Button2.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 255, 50)
        'Me.Height = OptionsContainer.Height
        'Me.Width = OptionsContainer.Width


    End Sub

    Private Sub FillSettings()
        tbPlayerName.Text = MainformRef.ConfigFile.Name
        tbP2Name.Text = MainformRef.ConfigFile.P2Name
        tb_Volume.Value = MainformRef.ConfigFile.Volume
        tb_eVolume.Value = MainformRef.ConfigFile.EmulatorVolume

        tbPort.Text = MainformRef.ConfigFile.Port
        cbShowConsole.Checked = MainformRef.ConfigFile.ShowConsole

        cbUseCustomWindowSize.Checked = CBool(MainformRef.ConfigFile.WindowSettings.Split("|")(0))
        tbCWX.Text = CInt(MainformRef.ConfigFile.WindowSettings.Split("|")(1))
        tbCWY.Text = CInt(MainformRef.ConfigFile.WindowSettings.Split("|")(2))
        tbCWWidth.Text = CInt(MainformRef.ConfigFile.WindowSettings.Split("|")(3))
        tbCWHeight.Text = CInt(MainformRef.ConfigFile.WindowSettings.Split("|")(4))

        If MainformRef.ConfigFile.AllowSpectators = 1 Then
            cbAllowSpectators.Text = "Yes"
        Else
            cbAllowSpectators.Text = "No"
        End If

        cbOverlay.SelectedIndex = MainformRef.ConfigFile.VsNames
        cb_ShowGameInTitle.Checked = MainformRef.ConfigFile.ShowGameNameInTitle
        cbVsync.SelectedIndex = MainformRef.ConfigFile.Vsync

        ' Themes
        Dim ThemesFound As New DataTable
        ThemesFound.Columns.Add("Theme", GetType(String))
        ThemesFound.Columns.Add("ThemeName", GetType(String))

        cbThemes.ValueMember = "Theme"
        cbThemes.DisplayMember = "ThemeName"

        For Each Dir As String In Directory.GetDirectories(MainformRef.NullDCPath & "\themes")
            ThemesFound.Rows.Add(Dir.Split("\")(Dir.Split("\").Count - 1), Dir.Split("\")(Dir.Split("\").Count - 1))
        Next

        cbThemes.DataSource = ThemesFound
        cbThemes.SelectedValue = MainformRef.ConfigFile.Theme
        cb_nullDCPriority.SelectedIndex = MainformRef.ConfigFile.NullDCPriority
        FormFilled = True
    End Sub

    Private Sub btnSaveExit_Click(sender As Object, e As EventArgs) Handles btnSaveExit.Click
        If tbPlayerName.Text.Trim.Length = 0 Or tbP2Name.Text.Trim.Length = 0 Then
            MsgBox("Player Name Cannot Be Empty")
            Exit Sub
        End If

        MainformRef.ConfigFile.Name = tbPlayerName.Text
        MainformRef.ConfigFile.P2Name = tbP2Name.Text

        MainformRef.ConfigFile.Network = ""
        MainformRef.ConfigFile.Port = tbPort.Text
        MainformRef.ConfigFile.Volume = tb_Volume.Value
        MainformRef.ConfigFile.EmulatorVolume = tb_eVolume.Value
        MainformRef.ConfigFile.ShowConsole = Convert.ToInt32(cbShowConsole.Checked)
        MainformRef.ConfigFile.WindowSettings = CInt(cbUseCustomWindowSize.Checked).ToString & "|" & tbCWX.Text & "|" & tbCWY.Text & "|" & tbCWWidth.Text & "|" & tbCWHeight.Text
        MainformRef.ConfigFile.ShowGameNameInTitle = Convert.ToInt32(cb_ShowGameInTitle.Checked)
        MainformRef.ConfigFile.Vsync = cbVsync.SelectedIndex
        MainformRef.ConfigFile.Theme = cbThemes.SelectedValue
        MainformRef.ConfigFile.NullDCPriority = cb_nullDCPriority.SelectedIndex

        If cbAllowSpectators.Text = "Yes" Then
            MainformRef.ConfigFile.AllowSpectators = 1
        Else
            MainformRef.ConfigFile.AllowSpectators = 0
        End If

        MainformRef.ConfigFile.VsNames = cbOverlay.SelectedIndex

        Rx.MainformRef.ConfigFile.SaveFile()
        Me.Close()
    End Sub

#Region "Text Field Limitation"

    Private Sub tbPlayerName_KeyPress(sender As Object, e As KeyPressEventArgs) Handles tbPlayerName.KeyPress, tbP2Name.KeyPress
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
        ' Mednafen
        processStartInfo.Arguments = processStartInfo.Arguments &
                                     String.Format(" & netsh advfirewall firewall delete rule name=""Mednafen"" program=""{0}"" & netsh advfirewall firewall delete rule name=""mednafen.exe"" program=""{0}"" & netsh advfirewall firewall add rule name=""Mednafen"" dir=in action=allow program=""{0}"" enable=yes & netsh advfirewall firewall add rule name=""Mednafen"" dir=out action=allow program=""{0}"" enable=yes", Application.StartupPath & "\mednafen\mednafen.exe")
        ' Mednafen Server
        processStartInfo.Arguments = processStartInfo.Arguments &
                                     String.Format(" & netsh advfirewall firewall delete rule name=""Mednafen Server"" program=""{0}"" & netsh advfirewall firewall delete rule name=""mednafen-server.exe"" program=""{0}"" & netsh advfirewall firewall add rule name=""Mednafen Server"" dir=in action=allow program=""{0}"" enable=yes & netsh advfirewall firewall add rule name=""Mednafen Server"" dir=out action=allow program=""{0}"" enable=yes", Application.StartupPath & "\mednafen\server\mednafen-server.exe")
        Dim Firewall = Process.Start(processStartInfo)

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs)
        If Not Application.OpenForms().OfType(Of frmKeyMapperSDL).Any Then
            frmKeyMapperSDL.Show(Me)
        End If
    End Sub

    Private Sub tb_Volume_MouseCaptureChanged(sender As TrackBar, e As EventArgs) Handles tb_Volume.MouseCaptureChanged, tb_eVolume.MouseCaptureChanged
        Try
            wavePlayer.Dispose()
            wavePlayer = New NAudio.Wave.WaveOut
            Dim ChallangeSound As New NAudio.Wave.WaveFileReader(My.Resources.NewChallanger)
            wavePlayer.Init(ChallangeSound)
            wavePlayer.Volume = sender.Value / 100
            wavePlayer.Play()
        Catch ex As Exception

        End Try


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

    Private Sub cbThemes_SelectedIndexChanged(sender As ComboBox, e As EventArgs) Handles cbThemes.SelectedIndexChanged
        If Not sender.SelectedValue = "" And Not sender.SelectedValue = MainformRef.ConfigFile.Theme And FormFilled Then
            MainformRef.ConfigFile.Theme = sender.SelectedValue
            MainformRef.ConfigFile.SaveFile(False)
            MainformRef.LoadThemeSettings()
            BEARTheme.LoadNewTheme()
        End If

    End Sub

#End Region

End Class