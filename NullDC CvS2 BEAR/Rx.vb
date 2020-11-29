Imports System.Drawing.Drawing2D
Imports System.IO
Imports System.Net.NetworkInformation
Imports System.Text
Imports System.Threading

Module Rx
    Public MainformRef As frmMain ' Mainly here to have a constatn reference to the main form even after minimzing to tray
    Public EEPROM As String ' the EEPROM we're using saved here for people that wanna join as spectator
    Public VMU As String ' the p1 VMU
    Public VMUPieces As New ArrayList From {"", "", "", "", "", "", "", "", "", ""}
    Public platform As String = ""

    Public Function GenerateGameKey() As String
        Dim r As New Random
        Dim s As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"
        Dim sb As New StringBuilder
        For i As Integer = 1 To 20
            Dim idx As Integer = r.Next(0, s.Length)
            sb.Append(s.Substring(idx, 1))
        Next

        Return sb.ToString()
    End Function


    Public Function GetEEPROM(ByVal _romfullpath As String) As String
        Dim EEPROMPath As String = _romfullpath & ".eeprom"
        Dim FileBytes As String

        If File.Exists(EEPROMPath) Then
            FileBytes = BitConverter.ToString(File.ReadAllBytes(EEPROMPath)).Replace("-", String.Empty)
            Console.WriteLine("Read EEPROM:" & FileBytes.ToString)
        Else
            Return ""
        End If

        Return FileBytes
    End Function

    ' Write the client EEPROM, only used to sync, has no actual use outside of sync
    Public Sub WriteEEPROM(ByVal EEPROMString As String, ByVal _romfullpath As String)
        Dim EEPROMPath As String = _romfullpath & ".eeprom_client"

        If Not EEPROMString = "" Then
            Dim nBytes = EEPROMString.Length \ 2
            Dim EEPROMasByte(nBytes - 1) As Byte
            For i = 0 To nBytes - 1
                EEPROMasByte(i) = Convert.ToByte(EEPROMString.Substring(i * 2, 2), 16)
            Next
            File.WriteAllBytes(EEPROMPath, EEPROMasByte)
        End If

    End Sub

    ' No real use for these yet just trying shit out
    Public Function ReadVMU() As String

        Dim VMUFILE = MainformRef.NullDCPath & "\dc\vmu_data_host.bin"
        If Not File.Exists(VMUFILE) Then Return ""

        Dim bytes = File.ReadAllBytes(VMUFILE)

        If File.Exists(VMUFILE) Then
            Return Convert.ToBase64String(bytes)
        Else
            Return ""
        End If

    End Function

    Public Sub TestSend()

        Dim VMUPieceLength = Math.Floor(VMU.Length / 10)
        Console.WriteLine(VMUPieceLength)
        Console.WriteLine(VMU.Length)
        Dim VMUPiecesTosend As New ArrayList

        Dim CompleteSent As String = ""

        For i = 0 To 9
            If i = 9 Then
                ' This is we have odd number of splits, the last one will get any left over bytes
                VMUPiecesTosend.Add(VMU.Substring(i * VMUPieceLength, VMU.Length - (i * VMUPieceLength)))
            Else
                VMUPiecesTosend.Add(VMU.Substring(i * VMUPieceLength, VMUPieceLength))
            End If
        Next

        For i = 0 To 9
            CompleteSent += VMUPiecesTosend(i)
            Thread.Sleep(50)
        Next

        File.WriteAllBytes(MainformRef.NullDCPath + "\dc\vmu_data_client.bin", Convert.FromBase64String(CompleteSent))

    End Sub

    Public Sub SendVMU(ByVal _ip As String)

        Dim VMUSendingThread As Thread = New Thread(
            Sub()
                Dim VMUPieceLength = Math.Floor(VMU.Length / 10)
                Dim VMUPiecesTosend As New ArrayList

                For i = 0 To 9
                    If i = 9 Then
                        ' This is we have odd number of splits, the last one will get any left over bytes
                        VMUPiecesTosend.Add(VMU.Substring(i * VMUPieceLength, VMU.Length - (i * VMUPieceLength)))
                    Else
                        VMUPiecesTosend.Add(VMU.Substring(i * VMUPieceLength, VMUPieceLength))
                    End If
                Next

                For i = 0 To 9
                    MainformRef.NetworkHandler.SendMessage("#,9," & i & "," & VMUPiecesTosend(i), _ip)
                    Thread.Sleep(100)
                Next

                Console.WriteLine("Done Sending VMU to " & _ip)

            End Sub)

        VMUSendingThread.IsBackground = True
        VMUSendingThread.Start()

    End Sub

    Public Sub RecieveVMUPiece(ByVal _total As Int16, ByVal _piece As Int16, ByVal _data As String)
        'Console.WriteLine("Recivefd Piece: " & _data)
        VMUPieces(_piece) = _data

        Dim RecivedAllVMUPieces = True

        For i = 0 To 9
            If VMUPieces(i) = "" Then
                RecivedAllVMUPieces = False
                Exit For
            End If
        Next

        If RecivedAllVMUPieces Then
            CombineVMUPieces()
        End If

    End Sub

    Public Sub CombineVMUPieces()
        Dim CombinedVMU As String = ""

        For i = 0 To 9
            CombinedVMU += VMUPieces(i)
        Next

        File.WriteAllBytes(MainformRef.NullDCPath + "\dc\vmu_data_client.bin", Convert.FromBase64String(CombinedVMU))
        Rx.VMU = CombinedVMU
        MainformRef.NetworkHandler.SendMessage("G", MainformRef.Challenger.ip)

        MainformRef.Invoke(
            Sub()
                If MainformRef.WaitingForm.Visible Then
                    MainformRef.WaitingForm.lbWaitingForHost.Text = "Waiting for Host"
                    MainformRef.WaitingForm.btnRetryVMU.Visible = False
                    MainformRef.WaitingForm.VMUTimer.Stop()
                End If
            End Sub)

    End Sub

    Public Function RemoveAnnoyingRomNumbersFromString(ByVal RomName As String) As String
        Dim _romname = RomName
        If RomName.Length > 7 Then
            If IsNumeric(_romname.Split(" ")(0)) And _romname.Substring(4, 3) = " - " Then
                _romname = _romname.Substring(7, _romname.Length - 7)
            End If
        End If

        Return _romname
    End Function

End Module

Module BEARTheme


    Enum ThemeKeys
        PrimaryColor
        PrimaryFontColor
        PrimaryFontSize
        SecondaryColor
        SecondaryFontColor
        SecondaryFontSize
        TertiaryColor
        TertiaryFontColor
        TertiaryFontSize
        ButtonColor
        ButtonFontColor
        ButtonFontSize
        DropdownColor
        DropdownFontColor
        DropdownFontSize
        MenuStripColor
        MenuStripHighlightColor
        MenuStripFontColor
        MenuStripFontSize
        MainMenuBackground

        LogoImage
        WaitBackground
        WaitHostBackground
        ChallengeBackground
        NotificationBackground
        HostBackground

        WaitHostAnimation
        HostAnimation

        OverlayTop
        OverlayBottom

        PlayerListColor
        PlayerListFontColor
        PlayerListFontSize
        MatchListColor
        MatchListFontColor
        MatchListFontSize

    End Enum

    Private FontTracker As New Dictionary(Of Control, Font)
    Public Theme As New Dictionary(Of String, String)

    Public Sub ApplyThemeToControl(ByRef _control As Control, Optional _which As Single = 1)
        _control.SuspendLayout()

        If Not FontTracker.ContainsKey(_control) Then
            FontTracker.Add(_control, _control.Font)
        Else
            _control.Font = FontTracker(_control)
        End If

        Select Case _control.GetType()
            Case GetType(Button)
                ApplyButtonTheme(_control)
            Case GetType(CheckBox)
                ApplyCheckBoxTheme(_control, _which)
            Case GetType(ComboBox)
                ApplyComboBoxTheme(_control)
            Case GetType(Label)
                ApplyLabelTheme(_control, _which)
            Case GetType(GroupBox)
                ApplyGroupBoxTheme(_control, _which)
            Case GetType(MenuStrip)
                ApplyMenuStripTheme(_control)
            Case GetType(TableLayoutPanel)
                ApplyTableLayoutPanelTheme(_control, _which)
            Case GetType(ListView)
                ApplyListViewTheme(_control, _which)
            Case GetType(RichTextBox)
                ApplyRichTextBoxTheme(_control, _which)
        End Select
        _control.ResumeLayout()
    End Sub

    Private Sub ApplyButtonTheme(ByRef _button As Button)
        _button.BackColor = LoadColor(ThemeKeys.ButtonColor)
        _button.ForeColor = LoadColor(ThemeKeys.ButtonFontColor)
        _button.Font = New Font(_button.Font.FontFamily, _button.Font.Size * LoadSize(ThemeKeys.ButtonFontSize), _button.Font.Style)

    End Sub

    Private Sub ApplyRichTextBoxTheme(ByRef _Control As RichTextBox, ByVal _which As Single)
        Select Case _which
            Case 1
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.PrimaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.PrimaryFontColor)
                _Control.Font = New Font(_Control.Font.FontFamily, _Control.Font.Size * LoadSize(ThemeKeys.PrimaryFontSize), _Control.Font.Style)
            Case 2
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.SecondaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.SecondaryFontColor)
                _Control.Font = New Font(_Control.Font.FontFamily, _Control.Font.Size * LoadSize(ThemeKeys.SecondaryFontSize), _Control.Font.Style)
            Case 3
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.TertiaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.TertiaryFontColor)
                _Control.Font = New Font(_Control.Font.FontFamily, _Control.Font.Size * LoadSize(ThemeKeys.TertiaryFontSize), _Control.Font.Style)
        End Select
    End Sub

    Private Sub ApplyListViewTheme(ByRef _Control As ListView, ByVal _which As Single)
        Select Case _which
            Case 1
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.PrimaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.PrimaryFontColor)
                _Control.Font = New Font(_Control.Font.FontFamily, _Control.Font.Size * LoadSize(ThemeKeys.PrimaryFontSize), _Control.Font.Style)
            Case 2
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.SecondaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.SecondaryFontColor)
                _Control.Font = New Font(_Control.Font.FontFamily, _Control.Font.Size * LoadSize(ThemeKeys.SecondaryFontSize), _Control.Font.Style)
            Case 3
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.TertiaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.TertiaryFontColor)
                _Control.Font = New Font(_Control.Font.FontFamily, _Control.Font.Size * LoadSize(ThemeKeys.TertiaryFontSize), _Control.Font.Style)
            Case 4
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.PlayerListColor)
                _Control.ForeColor = LoadColor(ThemeKeys.PlayerListFontColor)
                _Control.Font = New Font(_Control.Font.FontFamily, _Control.Font.Size * LoadSize(ThemeKeys.PlayerListFontSize), _Control.Font.Style)
            Case 5
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.MatchListColor)
                _Control.ForeColor = LoadColor(ThemeKeys.MatchListFontColor)
                _Control.Font = New Font(_Control.Font.FontFamily, _Control.Font.Size * LoadSize(ThemeKeys.MatchListFontSize), _Control.Font.Style)
        End Select
    End Sub

    Private Sub ApplyTableLayoutPanelTheme(ByRef _Control As TableLayoutPanel, ByVal _which As Single)
        Select Case _which
            Case 1
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.PrimaryColor)
            Case 2
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.SecondaryColor)
            Case 3
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.TertiaryColor)
        End Select
    End Sub


    Private Sub ApplyMenuStripTheme(ByRef _Control As MenuStrip)
        _Control.BackColor = LoadColor(ThemeKeys.MenuStripColor)
        _Control.ForeColor = LoadColor(ThemeKeys.MenuStripFontColor)
        _Control.Font = New Font(_Control.Font.FontFamily, _Control.Font.Size * LoadSize(ThemeKeys.MenuStripFontSize), _Control.Font.Style)
        _Control.Renderer = New MenuStripRenderer
    End Sub

    Private Sub ApplyGroupBoxTheme(ByRef _Control As GroupBox, ByVal _which As Single)
        Select Case _which
            Case 1
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.PrimaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.PrimaryFontColor)
                _Control.Font = New Font(_Control.Font.FontFamily, _Control.Font.Size * LoadSize(ThemeKeys.PrimaryFontSize), _Control.Font.Style)
            Case 2
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.SecondaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.SecondaryFontColor)
                _Control.Font = New Font(_Control.Font.FontFamily, _Control.Font.Size * LoadSize(ThemeKeys.SecondaryFontSize), _Control.Font.Style)
            Case 3
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.TertiaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.TertiaryFontColor)
                _Control.Font = New Font(_Control.Font.FontFamily, _Control.Font.Size * LoadSize(ThemeKeys.TertiaryFontSize), _Control.Font.Style)
        End Select
    End Sub

    Private Sub ApplyCheckBoxTheme(ByRef _Control As CheckBox, ByVal _which As Single)
        Select Case _which
            Case 1
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.PrimaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.PrimaryFontColor)
                _Control.Font = New Font(_Control.Font.FontFamily, _Control.Font.Size * LoadSize(ThemeKeys.PrimaryFontSize), _Control.Font.Style)
            Case 2
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.SecondaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.SecondaryFontColor)
                _Control.Font = New Font(_Control.Font.FontFamily, _Control.Font.Size * LoadSize(ThemeKeys.SecondaryFontSize), _Control.Font.Style)
            Case 3
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.TertiaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.TertiaryFontColor)
                _Control.Font = New Font(_Control.Font.FontFamily, _Control.Font.Size * LoadSize(ThemeKeys.TertiaryFontSize), _Control.Font.Style)
        End Select

    End Sub

    Private Sub ApplyLabelTheme(ByRef _control As Label, ByVal _which As Single)
        Select Case _which
            Case 1
                If Not _control.BackColor = Color.Transparent Then _control.BackColor = LoadColor(ThemeKeys.PrimaryColor)
                _control.ForeColor = LoadColor(ThemeKeys.PrimaryFontColor)
                _control.Font = New Font(_control.Font.FontFamily, _control.Font.Size * LoadSize(ThemeKeys.PrimaryFontSize), _control.Font.Style)
            Case 2
                If Not _control.BackColor = Color.Transparent Then _control.BackColor = LoadColor(ThemeKeys.SecondaryColor)
                _control.ForeColor = LoadColor(ThemeKeys.SecondaryFontColor)
                _control.Font = New Font(_control.Font.FontFamily, _control.Font.Size * LoadSize(ThemeKeys.SecondaryFontSize), _control.Font.Style)
            Case 3
                If Not _control.BackColor = Color.Transparent Then _control.BackColor = LoadColor(ThemeKeys.TertiaryColor)
                _control.ForeColor = LoadColor(ThemeKeys.TertiaryFontColor)
                _control.Font = New Font(_control.Font.FontFamily, _control.Font.Size * LoadSize(ThemeKeys.TertiaryFontSize), _control.Font.Style)
        End Select

    End Sub

    Private Sub ApplyComboBoxTheme(ByRef _control As ComboBox)
        _control.BackColor = LoadColor(ThemeKeys.DropdownColor)
        _control.ForeColor = LoadColor(ThemeKeys.DropdownFontColor)
        _control.Font = New Font(_control.Font.FontFamily, _control.Font.Size * LoadSize(ThemeKeys.DropdownFontSize), _control.Font.Style)

    End Sub

    Public Function LoadSize(ByVal _font As ThemeKeys) As Decimal
        If Theme.ContainsKey(_font.ToString().ToLower) Then
            If Not Theme(_font.ToString().ToLower) = "" Then
                Dim a As Decimal
                Decimal.TryParse(Theme(_font.ToString().ToLower), a)
                Return a
            Else
                Return 8.2
            End If
        Else
            Return 8.2
        End If

    End Function

    Public Function LoadColor(ByVal _color As ThemeKeys) As Color
        If Theme.ContainsKey(_color.ToString().ToLower) Then
            If Not Theme(_color.ToString().ToLower) = "" Then
                Return ColorTranslator.FromHtml(Theme(_color.ToString().ToLower))
            Else
                Return Color.FromArgb(255, 0, 255)
            End If
        Else
            Return Color.FromArgb(255, 0, 255)
        End If

    End Function

    Public Function LoadImage(ByVal _image As ThemeKeys) As Image
        If Theme.ContainsKey(_image.ToString.ToLower) Then
            Return Bitmap.FromFile(MainformRef.NullDCPath & "\themes\" & MainformRef.ConfigFile.Theme & "\images\" & Theme(_image.ToString.ToLower))
        Else
            Return Nothing
        End If

    End Function

    Public Sub LoadNewTheme()

        MainformRef.LoadThemeSettings()

        For Each _form As Form In Application.OpenForms

            Select Case _form.GetType
                Case GetType(frmChallenge)
                    DirectCast(_form, frmChallenge).ReloadTheme()
                Case GetType(frmChallengeGameSelect)
                    DirectCast(_form, frmChallengeGameSelect).ReloadTheme()
                Case GetType(frmChallengeSent)
                    DirectCast(_form, frmChallengeSent).ReloadTheme()
                Case GetType(frmDelayHelp)
                    DirectCast(_form, frmDelayHelp).ReloadTheme()
                Case GetType(frmDLC)
                    DirectCast(_form, frmDLC).ReloadTheme()
                Case GetType(frmHostPanel)
                    DirectCast(_form, frmHostPanel).ReloadTheme()
                Case GetType(frmLoLNerd)
                    DirectCast(_form, frmLoLNerd).ReloadTheme()
                Case GetType(frmMain)
                    DirectCast(_form, frmMain).ReloadTheme()
                Case GetType(frmNotification)
                    DirectCast(_form, frmNotification).ReloadTheme()
                Case GetType(FrmPaidDLC)
                    DirectCast(_form, FrmPaidDLC).ReloadTheme()
                Case GetType(frmReplays)
                    DirectCast(_form, frmReplays).ReloadTheme()
                Case GetType(frmSetup)
                    DirectCast(_form, frmSetup).ReloadTheme()
                Case GetType(frmWaitingForHost)
                    DirectCast(_form, frmWaitingForHost).ReloadTheme()
            End Select
        Next

    End Sub


End Module

Class ListViewItemComparer
    Implements IComparer
    Private col As Integer
    Private order As SortOrder

    Public Sub New()
        col = 0
        order = SortOrder.Ascending
    End Sub

    Public Sub New(column As Integer, order As SortOrder)
        col = column
        Me.order = order
    End Sub

    Public Function Compare(x As Object, y As Object) As Integer _
                        Implements System.Collections.IComparer.Compare
        Dim returnVal As Integer = -1
        returnVal = [String].Compare(CType(x,
                        ListViewItem).SubItems(col).Text,
                        CType(y, ListViewItem).SubItems(col).Text)
        ' Determine whether the sort order is descending.
        If order = SortOrder.Descending Then
            ' Invert the value returned by String.Compare.
            returnVal *= -1
        End If

        Return returnVal
    End Function
End Class

Public Class MenuStripRenderer : Inherits ToolStripProfessionalRenderer

    Protected Overrides Sub OnRenderMenuItemBackground(ByVal e As System.Windows.Forms.ToolStripItemRenderEventArgs)
        If e.Item.Selected Then
            Dim rc As New Rectangle(Point.Empty, e.Item.Size)
            Dim Pen = New Pen(BEARTheme.LoadColor(ThemeKeys.SecondaryColor), 2)
            Pen.Alignment = PenAlignment.Inset

            'Set the highlight color
            e.Graphics.FillRectangle(New SolidBrush(BEARTheme.LoadColor(ThemeKeys.MenuStripHighlightColor)), rc)
            'e.Graphics.DrawRectangle(Pens.Beige, 1, 0, rc.Width - 2, rc.Height - 1)
        Else
            Dim rc As New Rectangle(Point.Empty, e.Item.Size)
            Dim Pen = New Pen(BEARTheme.LoadColor(ThemeKeys.SecondaryColor), 2)
            Pen.Alignment = PenAlignment.Inset

            'Set the default color
            e.Graphics.FillRectangle(New SolidBrush(BEARTheme.LoadColor(ThemeKeys.MenuStripColor)), rc)
            'e.Graphics.DrawRectangle(Pens.Gray, 1, 0, rc.Width - 2, rc.Height - 1)

        End If
    End Sub

End Class

