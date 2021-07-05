Imports System.ComponentModel
Imports System.IO

Public Class MednafenSetting

    Public Property ProperName As String = ""
    Public Property ConfigString As String = ""
    <Editor(GetType(Design.MultilineStringEditor), GetType(Drawing.Design.UITypeEditor))> Public Property Limits As String = ""
    Public Property ChangeRate As Single = 1
    Public Property Emulator As String = "med"
    Public Property UseQuotes As Boolean = False

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
                If ConfigFile(i).StartsWith(cfgSplit.Trim & Separator) Then
                    _loadedValue = ConfigFile(i).Split(" ")(ConfigFile(i).Split.Count - 1).Replace("""", "")
                End If
            Next
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
                For Each _item In _configs : Controller.Items.Add(_item.Trim) : Next
                Controller.Dock = DockStyle.Fill
                Controller.DropDownStyle = ComboBoxStyle.DropDownList
                If Not _loadedValue = "" Then
                    For i = 0 To Controller.Items.Count - 1
                        If Controller.Items(i).trim = _loadedValue.Trim Then
                            Controller.SelectedIndex = i
                            Exit For
                        End If
                    Next
                Else
                    Controller.SelectedIndex = 0
                End If

                AddHandler Controller.SelectedIndexChanged, Sub()
                                                                UpdateCFG(Controller.Text)
                                                            End Sub

                SettingContainer.SetRow(Controller, 1)
                SettingContainer.SetColumn(Controller, 1)
                SettingContainer.Controls.Add(Controller)

        End Select

    End Sub

    Private Sub UpdateCFG(ByVal _newvalue As String)
        Dim EntryFound As Boolean = False
        Dim cfgSplit = ConfigString.Split(",")

        Dim ConfigFile As String() = {""}
        Dim ConfigFilePath As String = ""
        Dim UseEquals As Boolean = False

        Select Case Emulator
            Case "med"
                ConfigFile = Rx.MednafenConfigCache
                ConfigFilePath = MainformRef.NullDCPath & "\mednafen\mednafen.cfg"
            Case "mup"
                ConfigFile = Rx.MupenConfigCache
                ConfigFilePath = MainformRef.NullDCPath & "\Mupen64Plus\mupen64plus.cfg"
                UseEquals = True
        End Select

        Dim Separator As String = " "
        If UseQuotes Then
            Separator = " = "
        End If

        For i = 0 To ConfigFile.Count - 1
            For Each _cfg In cfgSplit

                If ConfigFile(i).StartsWith(_cfg.Trim & " ") Then

                    If UseEquals Then

                    End If
                    If UseQuotes Then
                        ConfigFile(i) = _cfg.Trim & Separator & """" & _newvalue & """"
                    Else
                        ConfigFile(i) = _cfg.Trim & Separator & _newvalue
                    End If


                    EntryFound = True
                    Exit For

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
