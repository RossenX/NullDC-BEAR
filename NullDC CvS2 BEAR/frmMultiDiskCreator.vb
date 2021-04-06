Imports System.IO

Public Class frmMultiDiskCreator

    Private Sub frmMultiDiskCreator_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.fan_icon_text
        Me.CenterToParent()
        cb_Platform.SelectedIndex = 0

        ApplyThemeToControl(TableLayoutPanel1)
        For Each _control As Control In TableLayoutPanel1.Controls
            ApplyThemeToControl(_control, 2)
        Next

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        Try
            Dim BrowseFileDialog1 As New OpenFileDialog
            BrowseFileDialog1.Filter = "cue Files (*.cue*)|*.cue"
            BrowseFileDialog1.RestoreDirectory = True

            Dim SelectedFolder = ""
            Select Case cb_Platform.Text
                Case "PSX"
                    SelectedFolder = MainformRef.NullDCPath & "\mednafen\roms\psx\"
                Case "SS"
                    SelectedFolder = MainformRef.NullDCPath & "\mednafen\roms\ss\"
            End Select
            BrowseFileDialog1.InitialDirectory = SelectedFolder

            If BrowseFileDialog1.ShowDialog = DialogResult.OK Then
                If Not BrowseFileDialog1.FileName.StartsWith(SelectedFolder) Then
                    MsgBox("Incorrect file, .cue file must be in the rom folder of the selected platform.")
                Else
                    RichTextBox1.Text += BrowseFileDialog1.FileName.Replace(SelectedFolder, "") & vbNewLine
                End If
            End If

        Catch ex As Exception
            MsgBox("Error: " & ex.Message)

        End Try

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim SelectedFolder = ""
        Select Case cb_Platform.Text
            Case "PSX"
                SelectedFolder = MainformRef.NullDCPath & "\mednafen\roms\psx\"
            Case "SS"
                SelectedFolder = MainformRef.NullDCPath & "\mednafen\roms\ss\"
        End Select
        Dim RomFirstName = RichTextBox1.Text.Split(vbLf)
        Dim GameName = RomFirstName(0).Split("(")(0)
        Dim DiskCount = 1
        GameName += "(Disk "
        For i = 0 To RomFirstName.Count - 1
            If Not RomFirstName(i).Trim = "" Then
                If Not DiskCount = 1 Then GameName += "+"
                GameName += DiskCount.ToString
                DiskCount += 1
            End If

        Next
        GameName += ")"

        File.WriteAllText(SelectedFolder & GameName & ".m3u", RichTextBox1.Text)
        MsgBox("Saved to: " & GameName)
        Me.Close()

    End Sub

    Private Sub btnHelp_Click(sender As Object, e As EventArgs) Handles btnHelp.Click
        MsgBox("Mednafen Requires a playlist file .m3u to do multiple disk so you can easily swap mid-game.  (F8 opens/closes the disc tray, F6 Swaps the Disk)" & vbNewLine & "This little tool will make it very easy to create those files, so you can play multi-disk games.")
    End Sub

End Class