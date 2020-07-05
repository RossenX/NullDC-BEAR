Imports System.Net
Imports System.Net.NetworkInformation

Public Class frmSetup

    Private Sub frmSetup_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.NewNullDCBearIcon
        If Not Rx.MainformRef.ConfigFile.FirstRun Then
            Label1.Text = "Oh hey again, what's up? Missed a setting or something?"
            Label1.Refresh()
            btnT1B1.Visible = False
            btnT1B2.Text = "Yus"
        End If
        Me.CenterToParent()
    End Sub

    Private Sub btnT1B2_Click(sender As Object, e As EventArgs) Handles btnT1B2.Click
        tcSetup.SelectedIndex += 1
        tbPlayerName.Text = Rx.MainformRef.ConfigFile.Name
        For Each NetworkName As String In GetNetworkNames()
            cbNetworks.Items.Add(NetworkName)
        Next
        cbNetworks.Text = Rx.MainformRef.ConfigFile.Network
        tbPort.Text = Rx.MainformRef.ConfigFile.Port
    End Sub

    Private Function GetNetworkNames() As ArrayList
        Dim networkNames As ArrayList = New ArrayList
        Dim nics As NetworkInterface() = NetworkInterface.GetAllNetworkInterfaces()
        For Each netadapter As NetworkInterface In nics
            For Each Address In netadapter.GetIPProperties.UnicastAddresses
                Dim OutAddress As IPAddress = New IPAddress(2130706433)
                If IPAddress.TryParse(Address.Address.ToString, OutAddress) Then
                    If OutAddress.AddressFamily = System.Net.Sockets.AddressFamily.InterNetwork Then
                        networkNames.Add(netadapter.Name)
                    End If
                End If
            Next
        Next
        Return networkNames
    End Function

    Private Sub btnT1B1_Click(sender As Object, e As EventArgs) Handles btnT1B1.Click
        End
    End Sub

    Private Sub btnT2B1_Click(sender As Object, e As EventArgs) Handles btnT2B1.Click
        Rx.MainformRef.ConfigFile.Name = tbPlayerName.Text
        Rx.MainformRef.ConfigFile.Network = cbNetworks.Text
        Rx.MainformRef.ConfigFile.Port = tbPort.Text


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
        tcSetup.SelectedIndex += 1
    End Sub

    Private Sub btnT3B2_Click(sender As Object, e As EventArgs) Handles btnT3B2.Click
        tcSetup.SelectedIndex -= 1

    End Sub

    Private Sub btnT3B3_Click(sender As Object, e As EventArgs) Handles btnT3B3.Click
        Me.Close()
        Application.OpenForms(0).Activate()

    End Sub

    Private Sub btnT3B1_Click(sender As Object, e As EventArgs) Handles btnT3B1.Click
        If Rx.MainformRef.KeyMappingForm.Visible Then
            Rx.MainformRef.KeyMappingForm.Focus()
        Else
            Rx.MainformRef.KeyMappingForm.Show()
        End If

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

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
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
        Dim Firewall = Process.Start(processStartInfo)

    End Sub
#End Region

End Class