Imports System.IO
Imports System.Net

Public Class frmDLCCreator
    '0|DLCv=1
    '1|Platform=<Platform, not really used also since the rom folder denotes the Platform mostly>
    '2|Disc=<Description, not used anywhere yet>
    '3|Tab=<TAB NAME>
    '4|Folder=<Relative path from BEAR.exe no \ at the start>
    '5|Extract=<0|1|2|3>
    '6|ExternalURL=<URL>
    '7-inf|Direct Links To Files

    Dim ExportCount = 0
    Public Sub New()
        InitializeComponent()
        Me.Icon = My.Resources.NewNullDCBearIcon

    End Sub

    Private Sub frmDLCCreator_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ApplyThemeToControl(TableLayoutPanel1)
        For Each child As Control In TableLayoutPanel1.Controls
            ApplyThemeToControl(child, 2)
        Next
        Me.CenterToParent()

        cb_extract.SelectedIndex = 0
        cb_platform.SelectedIndex = 0
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Button1.Text = "Getting..."
        If Not TextBox1.Text.StartsWith("https://archive.org/details/") Then
            MsgBox("URL Must Look Like This: https://archive.org/details/<NAME OF REPO>")
            Exit Sub
        End If

        ArchiveDotOrgParse(TextBox1.Text.Trim)
    End Sub

    Private Sub ArchiveDotOrgParse(ByVal URL As String)
        URL = URL & "&output=json"
        Dim StrippedURL = URL.Replace("&output=json", "")
        StrippedURL = StrippedURL.Replace("/details/", "/download/")
        ExportCount = 0

        Dim GamesListClient As New WebClient
        GamesListClient.Credentials = New NetworkCredential()
        GamesListClient.Headers.Add("user-agent", "MyRSSReader/1.0")

        GamesListClient.DownloadStringTaskAsync(URL)

        AddHandler GamesListClient.DownloadStringCompleted,
            Sub(ByVal sender As WebClient, e As DownloadStringCompletedEventArgs)

                Dim GameCount = 0
                Dim Exportedlist As New ArrayList

                If Not e.Error Is Nothing Then
                    MsgBox("Error Getting Downloadable Games List")
                    Exit Sub

                End If

                For Each _sp As String In e.Result.Split("""")
                    If _sp.Contains(".zip") Then
                        Dim _GameLink = StrippedURL & _sp.Replace("\/", "/").Replace("#", "%23")
                        Exportedlist.Add(_GameLink)
                        GameCount += 1

                    End If

                Next

                Dim ExportString = ""
                For i = 0 To Exportedlist.Count - 1
                    ExportString += Exportedlist(i) & vbNewLine
                Next
                RichTextBox1.Text += ExportString

                'File.WriteAllText(MainformRef.NullDCPath & "\Export" & URL.Split("/")(URL.Split("/").Count - 1) & ".freedlc", ExportString)
                ExportCount += 1
                Me.Invoke(Sub()
                              Button1.Text = "Done. Import Another"
                          End Sub)
            End Sub

    End Sub

    Private Sub btnSavePack_Click(sender As Object, e As EventArgs) Handles btnSavePack.Click
        If tb_tabname.Text.Trim.Length = 0 Then
            MsgBox("Tab Name Required")
            Exit Sub
        End If

        Dim SaveFileDialog1 As New SaveFileDialog
        SaveFileDialog1.Filter = "FreeDLC Files (*.freedlc*)|*.freedlc"
        SaveFileDialog1.RestoreDirectory = True
        SaveFileDialog1.InitialDirectory = MainformRef.NullDCPath & "\DLC"

        Dim FileToSave = "DLCv=1" & vbNewLine
        FileToSave += "Platform=" & cb_platform.Text & vbNewLine
        FileToSave += "Disc=" & tb_discription.Text & vbNewLine
        FileToSave += "Tab=" & tb_tabname.Text & vbNewLine

        Select Case cb_platform.Text
            Case "NA"
                FileToSave += "Folder=roms"
            Case "DC"
                FileToSave += "Folder=dc\roms"
            Case "PSX"
                FileToSave += "Folder=mednafen\roms\psx"
            Case "SS"
                FileToSave += "Folder=mednafen\roms\ss"
            Case "FDS"
                FileToSave += "Folder=mednafen\roms\fds"
            Case "NES"
                FileToSave += "Folder=mednafen\roms\nes"
            Case "SNES"
                FileToSave += "Folder=mednafen\roms\snes"
            Case "GBA"
                FileToSave += "Folder=mednafen\roms\gba"
            Case "GBC"
                FileToSave += "Folder=mednafen\roms\gbc"
            Case "SG"
                FileToSave += "Folder=mednafen\roms\sg"
            Case "NGP"
                FileToSave += "Folder=mednafen\roms\ngp"
            Case Else
                MsgBox("Platform not found, not sure how this is possible but ok.")
                Exit Sub
        End Select
        FileToSave += vbNewLine
        FileToSave += "Extract=" & cb_extract.SelectedIndex.ToString & vbNewLine
        FileToSave += "ExternalURL=" & TextBox1.Text & vbNewLine
        FileToSave += RichTextBox1.Text

        If SaveFileDialog1.ShowDialog = DialogResult.OK Then File.WriteAllText(SaveFileDialog1.FileName, FileToSave)

    End Sub

    Private Sub tb_KeyPress(sender As Object, e As KeyPressEventArgs) Handles tb_discription.KeyPress, tb_tabname.KeyPress
        If Not (Asc(e.KeyChar) = 8) Then
            Dim allowedChars As String = "abcdefghijklmnopqrstuvwxyz1234567890_ "
            If Not allowedChars.Contains(e.KeyChar.ToString.ToLower) Or (Asc(e.KeyChar) = 8) Then
                e.KeyChar = ChrW(0)
                e.Handled = True
            End If
        End If
    End Sub

End Class