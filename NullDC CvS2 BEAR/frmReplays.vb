Imports System.IO

Public Class frmReplays
    Dim OpenFileDialog As New OpenFileDialog

    Private Sub frmReplays_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.NewNullDCBearIcon
        Me.CenterToParent()

        If Not Directory.Exists(MainformRef.NullDCPath & "\replays") Then Directory.CreateDirectory(MainformRef.NullDCPath & "\replays")

        OpenFileDialog.Filter = "Bear Replay (*.bearplay2)|*.bearplay2"

        If MainformRef.ConfigFile.RecordReplay = 1 Then
            btnOn.BackColor = Color.Green
            btnOn.Text = "ON"
        Else
            btnOn.BackColor = Color.Red
            btnOn.Text = "OFF"
        End If

        GetReplayList()

    End Sub

    Private Sub GetReplayList()
        Dim files As String() = Directory.GetFiles(MainformRef.NullDCPath & "\replays", "*.bearplay2")
        lvReplays.Items.Clear()

        For Each _bearplay In files
            Try
                Dim FileAsStrings = File.ReadAllText(_bearplay)
                Dim FileStringSplitUp = FileAsStrings.Split("|")

                Dim listviewItem As ListViewItem = New ListViewItem(_bearplay)
                listviewItem.SubItems.Add(FileStringSplitUp(1))
                listviewItem.SubItems.Add(FileStringSplitUp(2))
                listviewItem.SubItems.Add(FileStringSplitUp(3) & "|" & FileStringSplitUp(4))
                listviewItem.SubItems.Add(FileStringSplitUp(6))
                lvReplays.Items.Add(listviewItem)
            Catch ex As Exception

            End Try

        Next

    End Sub

    Private Sub btnOpen_Click(sender As Object, e As EventArgs) Handles btnOpen.Click
        If Not Directory.Exists(MainformRef.NullDCPath & "\replays") Then Directory.CreateDirectory(MainformRef.NullDCPath & "\replays")
        OpenFileDialog.InitialDirectory = MainformRef.NullDCPath & "\replays"

        Dim result As DialogResult = OpenFileDialog.ShowDialog()

        If result = DialogResult.OK Then
            If Not OpenFileDialog.CheckPathExists Then Exit Sub
            PlayReplay(OpenFileDialog.FileName)

        End If

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Not Directory.Exists(MainformRef.NullDCPath & "\replays") Then Directory.CreateDirectory(MainformRef.NullDCPath & "\replays")
        Process.Start(MainformRef.NullDCPath & "\replays")

    End Sub

    Private Sub btnOn_Click(sender As Object, e As EventArgs) Handles btnOn.Click
        OpenFileDialog.Filter = "Bear Replay (*.bearplay2)|*.bearplay2"

        If MainformRef.ConfigFile.RecordReplay = 1 Then
            btnOn.BackColor = Color.Red
            btnOn.Text = "OFF"
            MainformRef.ConfigFile.RecordReplay = 0
        Else
            btnOn.BackColor = Color.Green
            btnOn.Text = "ON"
            MainformRef.ConfigFile.RecordReplay = 1
        End If
        MainformRef.ConfigFile.SaveFile()

    End Sub

    Private Sub btnPlay_Click(sender As Object, e As EventArgs) Handles btnPlay.Click
        If lvReplays.SelectedItems.Count > 0 Then
            PlayReplay(lvReplays.SelectedItems(0).SubItems(0).Text)
        End If

    End Sub

    Private Sub PlayReplay(ByVal ReplayPath As String)
        Try

            ' Extract Info
            Dim FileAsStrings = File.ReadAllText(ReplayPath)
            Dim FileStringSplitUp = FileAsStrings.Split("|")

            If Not FileStringSplitUp(4).StartsWith("NA-") Then
                FileStringSplitUp(4) = "NA-" & FileStringSplitUp(4)
            End If

            If Not MainformRef.GamesList.ContainsKey(FileStringSplitUp(4)) Then
                MsgBox("Couldn't fine game: " & FileStringSplitUp(3) & vbNewLine & "Romfile: " & FileStringSplitUp(4))
                Exit Sub
            End If

            MainformRef.ConfigFile.Status = "Spectator"
            MainformRef.ConfigFile.Game = FileStringSplitUp(4)
            MainformRef.ConfigFile.ReplayFile = ReplayPath
            MainformRef.ConfigFile.SaveFile()
            MainformRef.NullDCLauncher.P1Name = FileStringSplitUp(1)
            MainformRef.NullDCLauncher.P2Name = FileStringSplitUp(2)

            If FileAsStrings.Contains("|eeprom|") Then
                Dim eeprom As String() = FileAsStrings.Split(New String() {"|eeprom|"}, StringSplitOptions.None)
                Rx.WriteEEPROM(eeprom(1), MainformRef.NullDCPath & MainformRef.GamesList(MainformRef.ConfigFile.Game)(1))
            Else
                Rx.WriteEEPROM("", MainformRef.NullDCPath & MainformRef.GamesList(MainformRef.ConfigFile.Game)(1))
            End If

            MainformRef.NullDCLauncher.LaunchNaomi(FileStringSplitUp(4), FileStringSplitUp(5))
        Catch ex As Exception
            MainformRef.ConfigFile.Status = MainformRef.Configfile.awaystatus
            MainformRef.ConfigFile.Game = "None"
            MainformRef.ConfigFile.ReplayFile = ""
            MainformRef.ConfigFile.SaveFile()
            MsgBox("Unable to play Replay: " & ex.Message)

        End Try


    End Sub

    Private Sub lvReplays_DoubleClick(sender As Object, e As EventArgs) Handles lvReplays.DoubleClick
        If lvReplays.SelectedItems.Count > 0 Then
            PlayReplay(lvReplays.SelectedItems(0).SubItems(0).Text)
        End If

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.Close()
        MainformRef.Focus()

    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        If lvReplays.SelectedItems.Count > 0 Then
            If File.Exists(lvReplays.SelectedItems(0).SubItems(0).Text) Then
                File.SetAttributes(lvReplays.SelectedItems(0).SubItems(0).Text, FileAttributes.Normal)
                File.Delete(lvReplays.SelectedItems(0).SubItems(0).Text)
                GetReplayList()
            End If
        End If

    End Sub
End Class