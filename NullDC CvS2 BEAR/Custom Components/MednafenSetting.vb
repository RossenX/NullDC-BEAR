Imports System.ComponentModel
Imports System.IO

Public Class MednafenSetting

    Public Property ProperName As String = ""
    Public Property ConfigString As String = ""
    <Editor(GetType(Design.MultilineStringEditor), GetType(Drawing.Design.UITypeEditor))> Public Property Limits As String = ""
    Public Property ChangeRate As Single = 1
    Public Property Emulator As String = "med"
    Public Property UseQuotes As Boolean = False
    Public Property SpecialFunction As Int16 = 0

    Dim Controller As Control

    Sub New()
        MyBase.New
        InitializeComponent()

    End Sub

    Private Function GetLabelName() As String

        If Not ProperName = "" Then
            Return ProperName
        Else
            Return ConfigString
        End If

    End Function

    Private Sub MednafenSetting_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SuspendLayout()

        Dim _configs = Limits.Split(vbNewLine)

        Dim _loadedValue = ""
        Dim cfgSplit = ConfigString.Split(",")(0)
        Dim ConfigFile As String() = {""}
        Dim Separator As String = " "

        Select Case Emulator
            Case "med"
                ConfigFile = Rx.MednafenConfigCache
                Separator = " "
            Case "mup"
                ConfigFile = Rx.MupenConfigCache
                Separator = " = "
        End Select

        If ConfigFile IsNot Nothing Then
            For i = 0 To ConfigFile.Count - 1
                If SpecialFunction = 1 Then
                    If ConfigFile(i).StartsWith("ScreenWidth = ") Then
                        Dim cw As Short = CInt(ConfigFile(i).Split("=")(1).Trim)
                        _loadedValue = cw / 320

                    End If

                Else
                    If ConfigFile(i).StartsWith(cfgSplit.Trim & Separator) Then
                        _loadedValue = ConfigFile(i).Split(" ")(ConfigFile(i).Split.Count - 1).Replace("""", "")

                    End If

                End If

            Next
        End If

        ' Check for Comma and make it a period
        If _loadedValue.Contains(",") Then
            _loadedValue = _loadedValue.Replace(",", ".")
        End If

        Setting_Label.Text = GetLabelName()
        Setting_Label.Dock = DockStyle.Fill
        Setting_Label.AutoSize = True
        Setting_Label.TextAlign = ContentAlignment.MiddleLeft

        SettingContainer.SetRow(Setting_Label, 1)
        SettingContainer.SetColumn(Setting_Label, 0)
        SettingContainer.Controls.Add(Setting_Label)

        Select Case _configs.Count
            Case 1 ' no config lines so this is a bool
                Dim Controller As New CheckBox
                Controller.Text = ""
                Controller.Dock = DockStyle.Fill

                If Not _loadedValue = "" Then
                    Controller.Checked = CBool(_loadedValue)
                End If

                AddHandler Controller.CheckedChanged, Sub()
                                                          UpdateCFG(Controller.CheckState)
                                                      End Sub

                SettingContainer.SetRow(Controller, 1)
                SettingContainer.SetColumn(Controller, 1)
                SettingContainer.Controls.Add(Controller)
            Case 2 And IsNumeric(_configs(0)) And IsNumeric(_configs(1)) ' Min/Max Value
                Dim Controller As New TrackBar
                Controller.Minimum = CInt(_configs(0)) * ChangeRate
                Controller.Maximum = CInt(_configs(1)) * ChangeRate
                Controller.TickFrequency = CDec(_configs(1)) * ChangeRate
                Controller.SmallChange = ChangeRate
                Controller.LargeChange = ChangeRate
                Controller.TickStyle = TickStyle.Both
                Controller.Dock = DockStyle.Fill

                If Not _loadedValue = "" Then

                    If (CInt(_loadedValue.Trim) * ChangeRate) > Controller.Maximum Then
                        Controller.Value = Controller.Maximum
                    ElseIf (CInt(_loadedValue.Trim) * ChangeRate) < Controller.Minimum Then
                        Controller.Value = Controller.Minimum
                    Else
                        Controller.Value = (CDec(_loadedValue) * ChangeRate)
                    End If

                End If

                AddHandler Controller.MouseCaptureChanged, Sub()
                                                               Setting_Label.Text = GetLabelName() & " " & Controller.Value / ChangeRate
                                                               'UpdateCFG(Controller.Value / ChangeRate)

                                                           End Sub

                AddHandler Controller.ValueChanged, Sub()
                                                        Setting_Label.Text = GetLabelName() & " " & Controller.Value / ChangeRate
                                                        UpdateCFG(Controller.Value / ChangeRate)

                                                    End Sub

                AddHandler Controller.MouseEnter, Sub()
                                                      Controller.Focus()

                                                  End Sub

                Setting_Label.Text = GetLabelName() & " " & Controller.Value / ChangeRate

                SettingContainer.SetRow(Controller, 1)
                SettingContainer.SetColumn(Controller, 1)
                SettingContainer.Controls.Add(Controller)

            Case Else ' Anything else is an enum
                Dim Controller As New ComboBox
                SettingContainer.SetRow(Controller, 1)
                SettingContainer.SetColumn(Controller, 1)
                SettingContainer.Controls.Add(Controller)

                Dim comboSource As New Dictionary(Of String, String)()

                For Each _item In _configs
                    If _item.Contains("|") Then
                        comboSource.Add(_item.Split("|")(1), _item.Split("|")(0))
                    Else
                        Controller.Items.Add(_item.Trim)
                    End If

                Next

                If _configs(0).Contains("|") Then
                    Controller.DataSource = New BindingSource(comboSource, Nothing)
                    Controller.DisplayMember = "Value"
                    Controller.ValueMember = "Key"

                End If

                Controller.Dock = DockStyle.Fill
                Controller.DropDownStyle = ComboBoxStyle.DropDownList
                Controller.Update()

                If Not _loadedValue = "" Then

                    If comboSource.Count > 0 Then

                        For Each _key In comboSource.Keys

                            If _key = _loadedValue.Trim Then
                                Controller.SelectedValue = _key

                            End If
                        Next

                    Else

                        For i = 0 To Controller.Items.Count - 1
                            If Controller.Items(i).trim = _loadedValue.Trim Then
                                Controller.SelectedIndex = i
                                Exit For

                            End If

                        Next

                    End If



                Else
                    Controller.SelectedIndex = 0

                End If

                AddHandler Controller.SelectedIndexChanged, Sub()
                                                                If _configs(0).Contains("|") Then
                                                                    UpdateCFG(Controller.SelectedValue)
                                                                Else
                                                                    UpdateCFG(Controller.Text)

                                                                End If


                                                            End Sub



        End Select

        ResumeLayout()
    End Sub

    Private Sub UpdateCFG(ByVal _newvalue As String)
        Dim EntryFound As Boolean = False
        Dim cfgSplit = ConfigString.Split(",")

        Dim ConfigFile As String() = {""}
        Dim ConfigFilePath As String = ""

        Select Case Emulator
            Case "med"
                ConfigFile = Rx.MednafenConfigCache
                ConfigFilePath = MainformRef.NullDCPath & "\mednafen\mednafen.cfg"
            Case "mup"
                ConfigFile = Rx.MupenConfigCache
                ConfigFilePath = MainformRef.NullDCPath & "\Mupen64Plus\mupen64plus.cfg"
        End Select

        Dim Separator As String = " "
        If Emulator = "mup" Then
            Separator = " = "
        End If

        For i = 0 To ConfigFile.Count - 1
            For Each _cfg In cfgSplit

                If SpecialFunction = 1 Then ' Special Functino for Mupen Resolution Handling

                    If ConfigFile(i).StartsWith("ScreenWidth = ") Then
                        ConfigFile(i) = "ScreenWidth = " & (320 * _newvalue)
                        EntryFound = True
                    ElseIf ConfigFile(i).StartsWith("ScreenHeight = ") Then
                        ConfigFile(i) = "ScreenHeight = " & (240 * _newvalue)
                        EntryFound = True
                    End If

                Else
                    If ConfigFile(i).StartsWith(_cfg.Trim & " ") Then

                        If UseQuotes Then
                            ConfigFile(i) = _cfg.Trim & Separator & """" & _newvalue & """"
                        Else
                            ConfigFile(i) = _cfg.Trim & Separator & _newvalue
                        End If

                        EntryFound = True
                        Exit For

                    End If

                End If



            Next

        Next

        If EntryFound Then
            File.WriteAllLines(ConfigFilePath, ConfigFile)
            Console.WriteLine("Changed: " & _newvalue)
        Else
            MsgBox("Could not find Config String: " & ConfigString)

        End If

    End Sub

End Class
