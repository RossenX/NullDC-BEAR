Imports System.Runtime.InteropServices
Imports SDL2
Imports SDL2.SDL
Imports System.IO
Imports System.Text

Public Class frmKeyMapperSDL

    ' From the Configs
    Dim Joystick(2) As Int16
    Dim Deadzone(2) As Int16
    Dim Peripheral(2) As Int16
    Dim MednafenControllerID(128) As String ' Support up to 16 Controllers at once

    Public Joy As IntPtr
    Dim _InputThread As Threading.Thread
    Dim Currently_Binding As Button
    Dim ButtonsDown As New Dictionary(Of String, Boolean)

    ' Little things to know which systems were changed, so we don't have to always save settings that were not changed
    Public NaomiChanged As Boolean = False
    Public DreamcastChanged As Boolean = False
    Public MednafenChanged As Boolean = False
    Public MednafenControlChanged As Boolean = False

    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
    Private Shared Function PostMessage(ByVal hWnd As IntPtr, ByVal Msg As UInteger, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As Boolean
    End Function

    Private Declare Function GetActiveWindow Lib "user32" Alias "GetActiveWindow" () As IntPtr

    Private Declare Function SendMessage Lib "user32.dll" _
        Alias "SendMessageA" (ByVal hWnd As IntPtr, ByVal wMsg As Integer, ByVal wParam As Integer, ByVal lParam As IntPtr) As IntPtr

    Public Sub ReloadTheme()
        ApplyThemeToControl(MenuStrip1)
        ApplyThemeToControl(TableLayoutPanel1, 1)

        For Each _control In TableLayoutPanel1.Controls
            ApplyThemeToControl(_control, 1)
        Next
        ApplyThemeToControl(TableLayoutPanel2, 1)
        For Each _control In TableLayoutPanel2.Controls
            ApplyThemeToControl(_control, 1)
        Next
        ApplyThemeToControl(TableLayoutPanel4, 1)
        For Each _control In TableLayoutPanel4.Controls
            ApplyThemeToControl(_control, 1)
        Next

        For Each _tab As TabPage In ControllersTab.TabPages
            ApplyThemeToControl(_tab, 1)
            For Each _tabtab As TabControl In _tab.Controls
                ApplyThemeToControl(_tabtab, 1)
                For Each _tabtabtab As TabPage In _tabtab.TabPages
                    ApplyThemeToControl(_tabtabtab, 5)
                Next
            Next
        Next

    End Sub

    Private Sub frmKeyMapperSDL_VisibilityChange(sender As Object, e As EventArgs) Handles MyBase.VisibleChanged

        Try
            If Me.Visible Then

                Me.CenterToParent()
                Me.Icon = My.Resources.fan_icon_text
                ReloadTheme()

                If SDL_WasInit(SDL_INIT_GAMECONTROLLER) = 0 Then
                    SDL_Init(SDL_INIT_GAMECONTROLLER)
                    Console.WriteLine("SDL_INIT GAME CONTROLLER")
                End If

                If SDL_WasInit(SDL_INIT_JOYSTICK) = 0 Then
                    SDL_Init(SDL_INIT_JOYSTICK)
                    Console.WriteLine("SDL_INIT JOYSTICK")
                End If


                SDL_Delay(100)
                DoInitialSetupShit()
                LoadSettings()

                _InputThread = New Threading.Thread(AddressOf InputThread)
                _InputThread.IsBackground = True
                _InputThread.Start()

                AddHandler cbSDL.SelectedIndexChanged, AddressOf SDLVersionChanged
                AddHandler DeadzoneTB.MouseCaptureChanged, AddressOf DeadzoneTB_MouseCaptureChanged
                AddHandler DeadzoneTB.ValueChanged, Sub() Deadzonetext.Text = "Deadzone: " & DeadzoneTB.Value
                AddHandler PeripheralCB.SelectedIndexChanged, AddressOf PeripheralCB_SelectedIndexChanged
                AddHandler PlayerTab.SelectedIndexChanged, AddressOf PlayerTab_SelectedIndexChanged
                AddHandler ControllersTab.SelectedIndexChanged, Sub() ActiveControl = Nothing
                AddHandler cbProfiles.SelectedIndexChanged, AddressOf cbProfileIndexChanged

                If MainformRef.IsNullDCRunning Then
                    PeripheralCB.Enabled = False
                    PeriWarning.Visible = True
                    cbSDL.Enabled = False
                    cbProfiles.Enabled = False
                    ProfileToolStripMenuItem.Enabled = False

                Else
                    PeripheralCB.Enabled = True
                    PeriWarning.Visible = False
                    cbSDL.Enabled = True
                    cbProfiles.Enabled = True
                    ProfileToolStripMenuItem.Enabled = True

                End If

            End If
        Catch ex As Exception
            Console.WriteLine(ex.InnerException)

        End Try

    End Sub

    Private Sub SDLVersionChanged(sender As ComboBox, e As EventArgs)
        Me.Invoke(Sub()
                      If Not cbSDL.SelectedItem = MainformRef.ConfigFile.SDLVersion Then
                          MainformRef.ConfigFile.SDLVersion = "+" & sender.SelectedItem
                          MainformRef.ConfigFile.SaveFile(False)
                          If MsgBox("Restart Required") = MsgBoxResult.Ok Then End
                      End If
                  End Sub)
    End Sub

    Private Sub DoInitialSetupShit()

        'AvailableControllersList.Rows.Add({-1, "Keyboard Only"})

        ControllerCB.ValueMember = "ID"
        ControllerCB.DisplayMember = "Name"

        ' Flush Initial Connection Events
        SDL_FlushEvents(SDL_EventType.SDL_CONTROLLERDEVICEADDED, SDL_EventType.SDL_CONTROLLERDEVICEREMOVED)
        UpdateControllersList()
    End Sub

    Private Sub GenerateDefaults(ByVal _filepath As String)

        Joystick(0) = 0
        Joystick(1) = -1
        Deadzone(0) = 25
        Deadzone(1) = 25
        DeadzoneTB.Value = 25
        Peripheral(0) = 0
        Peripheral(1) = 0

        SaveSettings(_filepath)
        UpdateButtonLabels()

    End Sub

    Public Sub UpdateButtonLabels()
        For Each _TabPages As TabPage In ControllersTab.TabPages
            For Each _TabPageControl As Control In _TabPages.Controls
                If TypeOf _TabPageControl Is TabControl Then
                    Dim _innerTabControl As TabControl = _TabPageControl
                    For Each _innerTabPages As TabPage In _innerTabControl.TabPages
                        For Each _innerTabPageControl As Control In _innerTabPages.Controls
                            If TypeOf _innerTabPageControl Is keybindButton Then
                                Dim _innerTabPageControl2 As keybindButton = _innerTabPageControl
                                _innerTabPageControl2.UpdateTextFromPortID(PlayerTab.SelectedIndex)
                            End If
                        Next
                    Next

                End If
            Next
        Next

    End Sub

    Private Sub SaveSettings(ByVal _filepath As String)

        MainformRef.ConfigFile.DebugControls = DebugControlsCB.SelectedIndex
        MainformRef.ConfigFile.SaveFile(False)

        Dim lines(262) As String

        Dim MednafenControllerID = GetMednafenControllerIDs()

        lines(0) = MainformRef.Ver
        lines(1) = "Joystick=" & Joystick(0) & "|" & Joystick(1)
        lines(2) = "Deadzone=" & Deadzone(0) & "|" & Deadzone(1)
        lines(3) = "Peripheral=" & Peripheral(0) & "|" & Peripheral(1)
        Dim Med_string = "MednafenControllerID="

        If Joystick(0) = -1 Then
            Med_string += "0x0"
        ElseIf MednafenControllerID(Joystick(0)) Is Nothing Then
            Med_string += "0x0"
        Else
            Med_string += MednafenControllerID(Joystick(0))
        End If

        Med_string += "|"
        If Joystick(1) = -1 Then
            Med_string += "0x0"
        ElseIf MednafenControllerID(Joystick(1)) Is Nothing Then
            Med_string += "0x0"
        Else
            Med_string += MednafenControllerID(Joystick(1))
        End If

        lines(4) = Med_string

        Dim TabsToSave As New ArrayList
        TabsToSave.Add(Page_Naomi_ArcadeStick)
        TabsToSave.Add(Page_dc_Controller)
        TabsToSave.Add(Page_dc_ArcadeStick)
        TabsToSave.Add(Page_PSX_Gamepad)
        TabsToSave.Add(Page_PSX_Dualshock)
        TabsToSave.Add(Page_PSX_GunCon)
        TabsToSave.Add(Page_Saturn_Gamepad)
        TabsToSave.Add(Page_SNES_Gamepad)
        TabsToSave.Add(Page_SNES_SuperScope)
        TabsToSave.Add(Page_SNES_Mouse)
        TabsToSave.Add(Page_Genesis_Gamepad3)
        TabsToSave.Add(Page_Genesis_Gamepad6)
        TabsToSave.Add(Page_NES_Gamepad)
        TabsToSave.Add(Page_NES_Zapper)
        TabsToSave.Add(Page_GBA_GBA)
        TabsToSave.Add(Page_GBC_GBC)
        TabsToSave.Add(Page_NGP_NGP)
        TabsToSave.Add(Page_SMS_Gamepad)
        TabsToSave.Add(page_N64_Controller)

        Dim KeyCount = 5

        For Each _tab As TabPage In TabsToSave
            For Each _cont As Control In _tab.Controls
                If TypeOf _cont Is keybindButton Then
                    Dim _btn As keybindButton = _cont

                    Dim ButtonConfigString = _btn.ConfigString.Split(",")
                    For Each _btnstr In ButtonConfigString
                        Select Case _btn.Emu
                            Case "nulldc"
                                lines(KeyCount) = _btnstr.Trim & "=" & _btn.GetKeyCode(0) & "|" & _btn.GetKeyCode(1)
                            Case "mednafen"
                                If _btnstr.Contains("<port>") Then
                                    lines(KeyCount) = "med_" & _btnstr.Trim & "=" & _btn.GetKeyCode(0) & "|" & _btn.GetKeyCode(1)
                                Else
                                    lines(KeyCount) = "med_" & _btnstr.Trim & "=" & _btn.GetKeyCode(0) & "|" & _btn.GetKeyCode(0)
                                End If

                            Case "mupen"
                                lines(KeyCount) = "mup_" & _btnstr.Trim & "=" & _btn.GetKeyCode(0) & "|" & _btn.GetKeyCode(1)

                        End Select
                        KeyCount += 1
                    Next

                End If
            Next
        Next

        If Not File.Exists(_filepath) Then
            NaomiChanged = True
            DreamcastChanged = True
            MednafenChanged = True
            MednafenControlChanged = True
        Else
            Dim _tmpConfigFile As String() = File.ReadAllLines(_filepath)

            For Each _line In lines
                If Not _tmpConfigFile.Contains(_line) And Not _line Is Nothing Then

                    ' Deadzone change applies to all
                    If _line.StartsWith("Deadzone=") Or _line.StartsWith("Joystick=") Then
                        NaomiChanged = True
                        DreamcastChanged = True
                        MednafenChanged = True
                        MednafenControlChanged = True
                    End If

                    ' Naomi
                    If _line.StartsWith("I_") Then
                        NaomiChanged = True
                    End If

                    ' Dreamcast
                    If _line.StartsWith("CONT_") Or _line.StartsWith("STICK_") Or _line.StartsWith("Peripheral=") Then
                        DreamcastChanged = True
                    End If

                    ' Mednafen
                    If _line.StartsWith("med_") Or _line.StartsWith("MednafenControllerID=") Then
                        MednafenChanged = True
                        MednafenControlChanged = True
                    End If

                End If
            Next

        End If

        File.WriteAllLines(_filepath, lines)

        ' Check if we have a mednafen mapping or if we should just make one on the fly
        Dim MednafenControllerConfigLines

        If File.Exists(MainformRef.NullDCPath & "\mednafenmapping.txt") Then
            MednafenControllerConfigLines = File.ReadAllLines(MainformRef.NullDCPath & "\mednafenmapping.txt")
        Else
            MednafenControllerConfigLines = {""}
            MednafenChanged = True
            MednafenControlChanged = True
        End If

        For i = 0 To 1
            If MednafenChanged = False And Not Joystick(i) = -1 Then

                Dim DeviceGUIDasString(40) As Byte
                SDL_JoystickGetGUIDString(SDL_JoystickGetDeviceGUID(Joystick(i)), DeviceGUIDasString, 40)
                Dim GUIDSTRING As String = Encoding.ASCII.GetString(DeviceGUIDasString).ToString.Replace(vbNullChar, "").Trim

                Dim MednafenMappingFound = False
                For Each _line In MednafenControllerConfigLines
                    If _line.StartsWith(GUIDSTRING) Then
                        MednafenMappingFound = True
                        Exit For
                    End If
                Next

                If Not MednafenMappingFound Then
                    MednafenChanged = True
                    MednafenControlChanged = True
                End If

            End If
        Next

    End Sub

    Private Sub LoadProfiles(Optional ByVal _profile As String = Nothing)

        cbProfiles.Items.Clear()
        cbProfiles.Items.Add("Default")

        For Each _file In Directory.GetFiles(MainformRef.NullDCPath, "*.bear")
            Dim FileName As String = _file.Split("\")(_file.Split("\").Length - 1)
            If Not FileName = "Controls.bear" And FileName.StartsWith("Controls_") Then
                cbProfiles.Items.Add(FileName.Replace("Controls_", "").Replace(".bear", ""))
            End If

        Next

        Dim SelectedProfile = ""
        If Not _profile Is Nothing Then
            SelectedProfile = _profile
        Else
            SelectedProfile = MainformRef.ConfigFile.KeyMapProfile
        End If

        Dim ProfileFound As Boolean = False
        For i = 0 To cbProfiles.Items.Count - 1

            If cbProfiles.Items(i) = SelectedProfile Then
                cbProfiles.SelectedItem = cbProfiles.Items(i)
                ProfileFound = True
                Exit For
            End If

        Next

        If Not ProfileFound Then
            cbProfiles.SelectedIndex = 0
            MainformRef.ConfigFile.KeyMapProfile = "Default"
            MainformRef.ConfigFile.SaveFile(False)

        End If

    End Sub

    Private Sub cbProfileIndexChanged(sender As Object, e As EventArgs)
        RemoveHandler cbProfiles.SelectedIndexChanged, AddressOf cbProfileIndexChanged
        NaomiChanged = True
        DreamcastChanged = True
        MednafenChanged = True
        MednafenControlChanged = True

        LoadSettings(cbProfiles.Text.Trim)
        AddHandler cbProfiles.SelectedIndexChanged, AddressOf cbProfileIndexChanged
    End Sub


    Public Sub LoadSettings(Optional _profile As String = Nothing)

        LoadProfiles(_profile)

        Dim configLines As String()

        Dim KeyProfileFile As String = GetControlsFilePath(_profile)
        Console.WriteLine("Loaded Profile: " & KeyProfileFile)

        If Not File.Exists(KeyProfileFile) Then
            GenerateDefaults(KeyProfileFile)
        End If

        Dim LoadingControls As Int16 = 0
        While MainformRef.IsFileInUse(KeyProfileFile)
            If LoadingControls > 20 Then
                MsgBox("Couldn't load Controls.bear, file is locked or used by another process.")
                Me.Close()
            End If
            SDL_Delay(100)
            LoadingControls += 1
        End While


        configLines = File.ReadAllLines(KeyProfileFile)
        RemoveHandler ControllerCB.SelectedIndexChanged, AddressOf ControllerCB_SelectedIndexChanged

        For Each line In configLines
            If line.StartsWith("Joystick=") Then

                Joystick(0) = Convert.ToInt16(line.Split("=")(1).Split("|")(0))
                Joystick(1) = Convert.ToInt16(line.Split("=")(1).Split("|")(1))

                If ControllerCB.Items.Count > (Joystick(PlayerTab.SelectedIndex) + 1) Then
                    ControllerCB.SelectedIndex = Joystick(PlayerTab.SelectedIndex) + 1
                Else
                    ControllerCB.SelectedIndex = 0
                    Joystick(PlayerTab.SelectedIndex) = -1
                End If

            End If

            If line.StartsWith("Deadzone=") Then
                Deadzone(0) = Convert.ToInt16(line.Split("=")(1).Split("|")(0))
                Deadzone(1) = Convert.ToInt16(line.Split("=")(1).Split("|")(1))
                DeadzoneTB.Value = Deadzone(PlayerTab.SelectedIndex)
                Deadzonetext.Text = "Deadzone: " & DeadzoneTB.Value

            End If

            If line.StartsWith("Peripheral=") Then
                Peripheral(0) = Convert.ToInt16(line.Split("=")(1).Split("|")(0))
                Peripheral(1) = Convert.ToInt16(line.Split("=")(1).Split("|")(1))
                PeripheralCB.SelectedIndex = Peripheral(PlayerTab.SelectedIndex)
            End If

        Next

        For Each _tab As TabPage In ControllersTab.TabPages
            For Each _cont As Control In _tab.Controls
                If TypeOf _cont Is TabControl Then
                    Dim _InnereTab As TabControl = _cont
                    AddHandler _InnereTab.KeyDown, Sub(senter As TabControl, e As KeyEventArgs)
                                                       e.Handled = True
                                                   End Sub

                    For Each _tab2 As TabPage In _InnereTab.TabPages
                        For Each _cont2 As Control In _tab2.Controls
                            If TypeOf _cont2 Is keybindButton Then
                                Dim _btn As keybindButton = _cont2
                                GetKeyCodeFromFile(_btn, configLines)
                                AddHandler _btn.Click, AddressOf ClickedBindButton
                                _btn.UpdateTextFromPortID(PlayerTab.SelectedIndex)

                            End If
                        Next
                    Next
                End If

            Next
        Next

        If ControllerCB.SelectedIndex > -1 Then
            Joy = SDL_GameControllerOpen(ControllerCB.SelectedValue)
            Console.WriteLine(Joy)
        End If

        cbSDL.SelectedItem = MainformRef.ConfigFile.SDLVersion
        DebugControlsCB.SelectedIndex = MainformRef.ConfigFile.DebugControls

        AddHandler ControllerCB.SelectedIndexChanged, AddressOf ControllerCB_SelectedIndexChanged

    End Sub

    Private Sub GetKeyCodeFromFile(ByRef _btn As keybindButton, ByRef _configlines As String())

        For Each line As String In _configlines
            If line.StartsWith(_btn.Name & "=") Or line.StartsWith("med_" & _btn.ConfigString.Split(",")(0) & "=") Then
                _btn.SetKeyCode(line.Split("=")(1).Split("|")(0), 0)
                _btn.SetKeyCode(line.Split("=")(1).Split("|")(1), 1)
                Exit Sub
            End If
        Next

        If _btn.KeyDefaults.Count >= 2 Then
            If _btn.KeyDefaults(0).Split(",")(0).Count >= 2 Then
                _btn.SetKeyCode(_btn.KeyDefaults(0).Split(",")(0), 0)
            Else
                _btn.SetKeyCode("k0", 0)
            End If

            If _btn.KeyDefaults(1).Split(",").Count >= 2 Then
                _btn.SetKeyCode(_btn.KeyDefaults(1).Split(",")(2), 1)
            Else
                _btn.SetKeyCode("k0", 1)
            End If


        Else
            _btn.SetKeyCode("k0", 0)
            _btn.SetKeyCode("k0", 1)

        End If

    End Sub

    Private Sub InputThread()
        Try
            While _InputThread.IsAlive

                Try
                    If Application.OpenForms().OfType(Of frmSDLMappingTool).Any Then
                        SDL_Delay(250)
                        Continue While
                    End If

                Catch ex As Exception
                    Console.WriteLine("yup")
                    SDL_Delay(250)
                    Continue While
                End Try

                SDL_Delay(16)

                Dim _event As SDL_Event
                While SDL_WaitEventTimeout(_event, 16)
                    'Console.WriteLine(_event.type)
                    Select Case _event.type
                        Case 1616 ' Axis Motion

                            Console.WriteLine("Axis Motion: " & _event.caxis.axis & "|" & _event.caxis.axisValue & "|" & SDL_JoystickNumAxes(SDL_GameControllerGetJoystick(Joy)))

                            Me.Invoke(Sub()

                                          Dim _deadzonetotal As Decimal = 32768 * Decimal.Divide(DeadzoneTB.Value, 100)
                                          Dim _axisnorm As Int32 = _event.caxis.axisValue
                                          _axisnorm = Math.Abs(_axisnorm)

                                          If _axisnorm > 256 And _axisnorm > _deadzonetotal Then
                                              If _event.caxis.axisValue > 0 Then
                                                  ButtonClicked("a" & _event.caxis.axis & "+", True)
                                              Else
                                                  ButtonClicked("a" & _event.caxis.axis & "-", True)
                                              End If

                                          Else
                                              ButtonsDown.Remove("a" & _event.caxis.axis & "+")
                                              ButtonsDown.Remove("a" & _event.caxis.axis & "-")
                                              ButtonClicked("a" & _event.caxis.axis & "+", False)
                                              ButtonClicked("a" & _event.caxis.axis & "-", False)

                                          End If

                                      End Sub)


                        Case 1617 ' Controller Button Down
                            'Console.WriteLine("Button Down: " & _event.cbutton.button & " | " & SDL_GameControllerNameForIndex(_event.cdevice.which))
                            Me.Invoke(Sub()
                                          ButtonClicked("b" & _event.cbutton.button, True)
                                      End Sub)

                        Case 1618 ' Controller Button Up
                            'Console.WriteLine("Button UP: " & _event.cbutton.button & " | " & SDL_GameControllerNameForIndex(_event.cdevice.which))
                            Me.Invoke(Sub()
                                          ButtonsDown.Remove("b" & _event.cbutton.button)
                                          ButtonClicked("b" & _event.cbutton.button, False)
                                      End Sub)

                        Case 1541 ' Connected
                            'Console.WriteLine("Connected: " & SDL_GameControllerNameForIndex(_event.jdevice.which))
                            Me.Invoke(Sub() UpdateControllersList())
                        Case 1542 ' Disconnected using Joystick event cuz other seems to only work with opened
                            'Console.WriteLine("Disconnected: " & SDL_JoystickNameForIndex(_event.jdevice.which))
                            Me.Invoke(Sub() UpdateControllersList())
                        Case 1538 ' Joystick HAT LEFT-(abs+1)/RIGHT+(abs+1) UP-(abs+2)/Down+(abs+2)
                            ' Actually what i think i'll do is let it show the controller mapping button in BEAR and then translate those 
                            ' to joystick buttons, not to confuse people why nulldc and mednafen have show different button numbers for the same button
                            ' This will also save me having to rewrite some shit

                        Case 1539 ' Joystick Button Down

                        Case 1540 ' Joystick Button Up

                        Case 1536 ' Joystick Axis

                        Case 1621 ' Connected
                            SDL_FlushEvents(0, SDL_EventType.SDL_LASTEVENT)
                        Case Else
                            Console.WriteLine("Unhandled SDL Event: " & _event.type)

                    End Select

                End While

                _event = Nothing
            End While
        Catch ex As Exception

        End Try


    End Sub

    ' Update Button Configs Based on the Config String for the Game Controller
    Public Sub AutoGenerateButtonConfigs(Optional ByVal _configString As String = "", Optional ByVal _player As Int16 = -1)
        Dim ControllerType = "gamepad"

        If Not _configString.Contains("leftx:") Or Not _configString.Contains("lefty:") Then
            ControllerType = "fightstick"
        End If

        If _configString = "gamepad" Then ControllerType = "gamepad"
        If _configString = "keyboard" Then ControllerType = "keyboard"
        If _configString = "fightstick" Then ControllerType = "fightstick"

        Dim TabsToReset As New ArrayList
        TabsToReset.Add(Page_Naomi_ArcadeStick)
        TabsToReset.Add(Page_dc_Controller)
        TabsToReset.Add(Page_dc_ArcadeStick)
        TabsToReset.Add(Page_PSX_Gamepad)
        TabsToReset.Add(Page_PSX_Dualshock)
        TabsToReset.Add(Page_PSX_GunCon)
        TabsToReset.Add(Page_Saturn_Gamepad)
        TabsToReset.Add(Page_SNES_Gamepad)
        TabsToReset.Add(Page_SNES_SuperScope)
        TabsToReset.Add(Page_SNES_Mouse)
        TabsToReset.Add(Page_Genesis_Gamepad3)
        TabsToReset.Add(Page_Genesis_Gamepad6)
        TabsToReset.Add(Page_NES_Gamepad)
        TabsToReset.Add(Page_NES_Zapper)
        TabsToReset.Add(Page_GBA_GBA)
        TabsToReset.Add(Page_GBC_GBC)
        TabsToReset.Add(Page_NGP_NGP)
        TabsToReset.Add(Page_SMS_Gamepad)

        For Each _tab As TabPage In TabsToReset
            For Each _tabCont As Control In _tab.Controls
                If TypeOf _tabCont Is keybindButton Then
                    Dim _kbb As keybindButton = _tabCont

                    If _player = -1 Then
                        _kbb.ResetToDefault(ControllerType)
                    Else
                        _kbb.ResetToDefault(ControllerType, _player)
                    End If

                End If
            Next

        Next

        If Not MainformRef.IsNullDCRunning Then
            Select Case ControllerType ' Directional Shit
                Case "gamepad"
                    PeripheralCB.SelectedIndex = 0

                Case "fightstick", "keyboard"
                    PeripheralCB.SelectedIndex = 1

            End Select
        End If

        'UpdateButtonLabels()
    End Sub

    Public Sub UpdateControllersList()
        RemoveHandler ControllerCB.SelectedIndexChanged, AddressOf ControllerCB_SelectedIndexChanged
        SDL_GameControllerAddMappingsFromFile(MainformRef.NullDCPath & "\gamecontrollerdb.txt")
        SDL_GameControllerAddMappingsFromFile(MainformRef.NullDCPath & "\bearcontrollerdb.txt")

        Dim OldConnectedIndex = ControllerCB.SelectedValue
        Dim FoundController As Boolean = False
        Dim AvailableControllersList As New DataTable
        AvailableControllersList.Columns.Add("ID")
        AvailableControllersList.Columns.Add("Name")
        AvailableControllersList.Rows.Add({-1, "Keyboard Only"})

        For i = 0 To SDL_NumJoysticks() - 1

            AvailableControllersList.Rows.Add({i, "(" & i & ") " & SDL_JoystickNameForIndex(i)})

            If i = OldConnectedIndex Then
                FoundController = True
            End If
        Next

        ControllerCB.DataSource = New DataView(AvailableControllersList)

        If FoundController Then
            ControllerCB.SelectedIndex = OldConnectedIndex + 1
        Else
            ControllerCB.SelectedIndex = 0
        End If

        AddHandler ControllerCB.SelectedIndexChanged, AddressOf ControllerCB_SelectedIndexChanged
    End Sub

    Private Sub SaveEverything()
        '_InputThread.Abort() ' Aight not accepting inputs anymore
        btn_Close.Text = "Saving..."
        Dim ControlFilePath = GetControlsFilePath()
        SaveSettings(ControlFilePath)
        Console.WriteLine("Saving profile: " & ControlFilePath)

        ' Disable SDL completly we dun need that shit in the background no more
        For i = 0 To SDL_NumJoysticks() - 1
            If SDL_GameControllerGetAttached(SDL_GameControllerFromInstanceID(i)) = SDL_bool.SDL_TRUE Then
                Console.WriteLine("Disconnecting: " & SDL_GameControllerNameForIndex(i))
                SDL_GameControllerClose(SDL_GameControllerFromInstanceID(i))

            End If
        Next

        ' Update Control Configs for all the shit and hot-load if possible
        Dim ControlsConfigs() As String = File.ReadAllLines(ControlFilePath)
        Dim NaomiConfigs() As String = File.ReadAllLines(MainformRef.NullDCPath & "\nullDC.cfg")
        Dim DreamcastConfigs() As String = File.ReadAllLines(MainformRef.NullDCPath & "\dc\nullDC.cfg")
        Dim MednafenConfigs() As String = File.ReadAllLines(MainformRef.NullDCPath & "\mednafen\mednafen.cfg")

        ' We need the peripheral beforehand so we know which control settings to actually use
        Dim tempPeripheral As String() = {"", ""}
        Dim tempJoystick As String() = {"", ""}
        Dim TempDeadzone As String() = {"", ""}
        Dim MednafenControllerID As String() = {"", ""}

        For Each line As String In ControlsConfigs
            If line.StartsWith("Peripheral=") Then
                tempPeripheral(0) = line.Split("=")(1).Split("|")(0)
                tempPeripheral(1) = line.Split("=")(1).Split("|")(1)
                MainformRef.ConfigFile.Peripheral = tempPeripheral(0)
                MainformRef.ConfigFile.SaveFile(False)
            End If
            If line.StartsWith("Joystick=") Then
                tempJoystick(0) = line.Split("=")(1).Split("|")(0)
                tempJoystick(1) = line.Split("=")(1).Split("|")(1)
            End If
            If line.StartsWith("Deadzone=") Then
                TempDeadzone(0) = line.Split("=")(1).Split("|")(0)
                TempDeadzone(1) = line.Split("=")(1).Split("|")(1)
            End If
            If line.StartsWith("MednafenControllerID=") Then
                MednafenControllerID(0) = line.Split("=")(1).Split("|")(0)
                MednafenControllerID(1) = line.Split("=")(1).Split("|")(1)
            End If
        Next

        ' Naomi Controls
        Dim linenumber = 0
        Try
            If NaomiChanged Then
                Console.WriteLine("Saving Naomi Controls")
                For Each line As String In NaomiConfigs
                    btn_Close.Text = "Saving Naomi..."
                    If line.StartsWith("BPort") Then ' Check if it's a BEAR Port So we ignore everything else
                        Dim player = 0 ' Default player 1 is index 0
                        If line.StartsWith("BPortB") Then player = 1 ' This is port B so it's player 2 index 1

                        Dim KeyFound = False
                        For Each control_line As String In ControlsConfigs
                            If line.Contains(control_line.Split("=")(0) & "=") And control_line.Length > 0 Then
                                NaomiConfigs(linenumber) = line.Split("=")(0) & "=" & control_line.Split("=")(1).Split("|")(player)
                                KeyFound = True
                                Exit For
                            End If
                        Next

                        If Not KeyFound Then NaomiConfigs(linenumber) = line.Split("=")(0) & "=k0" ' The key was not in the controls so just set it to nothing

                        If line.StartsWith("BPortA_Joystick=") Then NaomiConfigs(linenumber) = "BPortA_Joystick=" & tempJoystick(0)
                        If line.StartsWith("BPortB_Joystick=") Then NaomiConfigs(linenumber) = "BPortB_Joystick=" & tempJoystick(1)

                        If line.StartsWith("BPortA_Deadzone=") Then NaomiConfigs(linenumber) = "BPortA_Deadzone=" & TempDeadzone(0)
                        If line.StartsWith("BPortB_Deadzone=") Then NaomiConfigs(linenumber) = "BPortB_Deadzone=" & TempDeadzone(1)

                    End If
                    linenumber += 1
                Next

                File.SetAttributes(MainformRef.NullDCPath & "\nullDC.cfg", FileAttributes.Normal)
                File.WriteAllLines(MainformRef.NullDCPath & "\nullDC.cfg", NaomiConfigs)
            End If

        Catch ex As Exception
            MsgBox("Error Saving Naomi Controls: " & ex.InnerException.Message)

        End Try



        ' Dreamcast Controls
        Try
            linenumber = 0
            If DreamcastChanged Then
                Console.WriteLine("Saving Dreamcast Controls")
                For Each line As String In DreamcastConfigs ' Very similar to the Naomi configs, but different
                    btn_Close.Text = "Saving Dreamcast..."
                    If line.StartsWith("BPort") Then
                        Dim player = 0
                        If line.StartsWith("BPortB") Then player = 1

                        Dim KeyFound = False
                        For Each control_line As String In ControlsConfigs

                            If tempPeripheral(player) = "1" Then ' Joystick Peripheral so get the STICK_ instead of the CONT_ setting
                                If control_line.StartsWith("STICK_") Then
                                    If line.Contains(control_line.Split("=")(0).Replace("STICK_", "CONT_") & "=") And control_line.Length > 0 Then
                                        DreamcastConfigs(linenumber) = line.Split("=")(0) & "=" & control_line.Split("=")(1).Split("|")(player)
                                        KeyFound = True
                                        Exit For
                                    End If
                                End If

                            Else

                                If line.Contains(control_line.Split("=")(0) & "=") And control_line.Length > 0 Then
                                    DreamcastConfigs(linenumber) = line.Split("=")(0) & "=" & control_line.Split("=")(1).Split("|")(player)
                                    KeyFound = True
                                    Exit For
                                End If

                            End If


                        Next

                        If Not KeyFound Then DreamcastConfigs(linenumber) = line.Split("=")(0) & "=k0"

                        If line.StartsWith("BPortA_Joystick=") Then DreamcastConfigs(linenumber) = "BPortA_Joystick=" & tempJoystick(0)
                        If line.StartsWith("BPortB_Joystick=") Then DreamcastConfigs(linenumber) = "BPortB_Joystick=" & tempJoystick(1)

                        If line.StartsWith("BPortA_Deadzone=") Then DreamcastConfigs(linenumber) = "BPortA_Deadzone=" & TempDeadzone(0)
                        If line.StartsWith("BPortB_Deadzone=") Then DreamcastConfigs(linenumber) = "BPortB_Deadzone=" & TempDeadzone(1)

                    End If
                    linenumber += 1

                Next

                File.SetAttributes(MainformRef.NullDCPath & "\dc\nullDC.cfg", FileAttributes.Normal)
                File.WriteAllLines(MainformRef.NullDCPath & "\dc\nullDC.cfg", DreamcastConfigs)
            End If

        Catch ex As Exception
            MsgBox("Error Saving Dreamcast Controls: " & ex.InnerException.Message)
        End Try

        Try
            ' Mednafen Controls
            btn_Close.Text = "Saving..."

            If MednafenChanged Then
                Console.WriteLine("Saving Mednafen Controls")
                Dim _TranslatedControls(2) As Dictionary(Of String, String)

                For i = 0 To 1
                    If Not Joystick(i) = -1 Then
                        If _TranslatedControls(i) Is Nothing Then

                            Dim _MednafenMapping = GetFullMappingStringforIndex(Joystick(i)).Split("|")(1).Split(",")

                            For Each _split In _MednafenMapping
                                Dim _splitsplit = _split.Split(":")

                                If _TranslatedControls(i) Is Nothing Then
                                    _TranslatedControls(i) = New Dictionary(Of String, String)
                                End If

                                If _splitsplit.Count = 1 Then
                                    _TranslatedControls(i).Add(_splitsplit(0), _splitsplit(0))
                                Else
                                    _TranslatedControls(i).Add(_splitsplit(0), _splitsplit(1))
                                End If

                            Next
                        End If
                    End If
                Next

                linenumber = 0
                For Each line As String In MednafenConfigs
                    btn_Close.Text = "Saving Mednafen..."

                    ' Trim down the lines we're looking for to reduce saving time
                    If line.StartsWith(";") Or
                        line.Trim.Length = 0 Or
                        (Not line.Contains("input") And
                        Not line.Contains("command.")) Then
                        MednafenConfigs(linenumber) = line
                        linenumber += 1
                        Continue For
                    End If

                    ' Deadzone
                    If line.StartsWith("input.joystick.axis_threshold ") Then
                        MednafenConfigs(linenumber) = "input.joystick.axis_threshold " & DeadzoneTB.Value
                        linenumber += 1
                        Continue For
                    End If

                    If MednafenControlChanged Then
                        Dim tmpControlString = ""

                        If line.Contains("rapid_") Or
                                line.StartsWith("command.0 ") Or
                                line.StartsWith("command.1 ") Or
                                line.StartsWith("command.2 ") Or
                                line.StartsWith("command.3 ") Or
                                line.StartsWith("command.4 ") Or
                                line.StartsWith("command.5 ") Or
                                line.StartsWith("command.6 ") Or
                                line.StartsWith("command.7 ") Or
                                line.StartsWith("command.8 ") Or
                                line.StartsWith("command.9 ") Or
                                line.StartsWith("command.state_slot_dec ") Then
                            tmpControlString = line.Split(" ")(0) & " " ' Disable Rapid Control unless they are found in our configs
                            MednafenConfigs(linenumber) = tmpControlString
                            linenumber += 1
                            Continue For
                        End If

                        For Each control_line In ControlsConfigs
                            If control_line.StartsWith("med_") Then
                                control_line = control_line.Substring(4)

                                If line.StartsWith(control_line.Split("=")(0).Replace("<port>", "1") & " ") Or
                                   line.StartsWith(control_line.Split("=")(0).Replace("<port>", "2") & " ") Then

                                    Dim _player = 1

                                    If control_line.Contains("<port>") And line.StartsWith(control_line.Split("=")(0).Replace("<port>", "2") & " ") Then
                                        _player = 2
                                    End If

                                    tmpControlString = control_line.Split("=")(0).Replace("<port>", _player.ToString) & " " ' Initial String

                                    Dim _KeyCode = control_line.Split("=")(1).Split("|")(_player - 1)

                                    If _KeyCode.StartsWith("k") Then ' Keyboard

                                        If Not _KeyCode = "k0" Then
                                            tmpControlString += "keyboard 0x0 " & KeyCodeToSDLScanCode(control_line.Split("=")(1).Split("|")(_player - 1).Substring(1))
                                            Exit For
                                        End If

                                    ElseIf _KeyCode.StartsWith("m") Then
                                        If _KeyCode = "m1" Then
                                            tmpControlString += "mouse 0x0 button_left"
                                        ElseIf _KeyCode = "m2" Then
                                            tmpControlString += "mouse 0x0 button_right"
                                        ElseIf _KeyCode = "m3" Then
                                            tmpControlString += "mouse 0x0 button_middle"
                                        End If

                                    Else ' Joystick
                                        Dim _tmpID = ""
                                        If Not MednafenControllerID(_player - 1) = "0x0" Then
                                            Dim isXinput = False
                                            If MednafenControllerID(_player - 1).StartsWith("xinput_") Then
                                                _tmpID = MednafenControllerID(_player - 1).Replace("xinput_", "")
                                                isXinput = True
                                            Else
                                                _tmpID = MednafenControllerID(_player - 1)
                                            End If

                                            tmpControlString += "joystick " & _tmpID
                                            If _tmpID Is Nothing Then
                                                tmpControlString = control_line.Split("=")(0).Replace("<port>", _player.ToString)
                                            End If

                                            If _TranslatedControls(_player - 1) Is Nothing Then
                                                tmpControlString = control_line.Split("=")(0).Replace("<port>", _player.ToString)
                                            ElseIf _TranslatedControls(_player - 1).ContainsKey(_KeyCode) And _tmpID.Trim.Length > 1 Then ' Failsafe if we fail to get the controller ID then do NOT set controls, because it'll cause the whole config file to be useless and need to be reset before it can be used
                                                If isXinput Then
                                                    tmpControlString += " " & MednafenXinputButtonToSDLxInputButton(_TranslatedControls(_player - 1)(_KeyCode))
                                                Else
                                                    tmpControlString += " " & _TranslatedControls(_player - 1)(_KeyCode)
                                                End If

                                            Else
                                                tmpControlString = control_line.Split("=")(0).Replace("<port>", _player.ToString)
                                            End If
                                            Exit For

                                        Else
                                            tmpControlString = control_line.Split("=")(0).Replace("<port>", _player.ToString)
                                            Exit For
                                        End If
                                    End If

                                End If
                            End If
                        Next

                        If Not tmpControlString = "" Then
                            MednafenConfigs(linenumber) = tmpControlString
                        End If

                    End If

                    linenumber += 1
                Next

                Dim CompressedConfigFile As New List(Of String)
                For Each _line In MednafenConfigs
                    If Not _line.Trim.Length = 0 Then
                        If _line.StartsWith(";") Then
                            CompressedConfigFile.Add(vbNewLine & _line)
                        Else
                            CompressedConfigFile.Add(_line)
                        End If

                    End If
                Next

                File.SetAttributes(MainformRef.NullDCPath & "\mednafen\mednafen.cfg", FileAttributes.Normal)
                File.WriteAllLines(MainformRef.NullDCPath & "\mednafen\mednafen.cfg", CompressedConfigFile.ToArray)
            End If

        Catch ex As Exception
            MsgBox("Error Saving Mednafen Controls: " & ex.InnerException.Message)

        End Try

        ' Mupen Controls
        Try

        Catch ex As Exception

        End Try

        ' Check if nullDC is running to hotload the settings
        If MainformRef.IsNullDCRunning Then
            If platform = "na" Then SendMessage(MainformRef.NullDCLauncher.NullDCproc.MainWindowHandle, &H111, 141, 0)
            If platform = "dc" Then SendMessage(MainformRef.NullDCLauncher.NullDCproc.MainWindowHandle, &H111, 144, 0) ' 180 144

        End If

        ' Finally Turn Off SDL
        SDL_Quit() ' Turn off SDL make sure nothing changes before we write the controls to the configs.
    End Sub

    Private Sub frmKeyMapperSDL_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        ButtonClicked("k" & e.KeyCode, True)

    End Sub

    Private Sub frmKeyMapperSDL_KeyUp(sender As Object, e As KeyEventArgs) Handles MyBase.KeyUp
        ButtonsDown.Remove("k" & e.KeyCode)
        ButtonClicked("k" & e.KeyCode, False)

    End Sub

    Private Sub DeadzoneTB_MouseCaptureChanged(sender As Object, e As EventArgs)
        Deadzone(PlayerTab.SelectedIndex) = DeadzoneTB.Value

    End Sub

    Private Sub ClickedBindButton(sender As keybindButton, e As EventArgs)
        'Console.WriteLine("Bind clicked: " & sender.Name)
        If sender.KeyLocked Then
            MsgBox("this bind is locked for now")
            Exit Sub
        End If

        If Not Currently_Binding Is Nothing Then
            Currently_Binding.BackColor = Color.White
            Currently_Binding = Nothing
        End If

        Currently_Binding = sender
        Currently_Binding.BackColor = Color.Red
        ActiveControl = Nothing

    End Sub

    Private Sub PlayerTab_SelectedIndexChanged(sender As Object, e As EventArgs)
        Try
            RemoveHandler ControllerCB.SelectedIndexChanged, AddressOf ControllerCB_SelectedIndexChanged
            RemoveHandler DeadzoneTB.MouseCaptureChanged, AddressOf DeadzoneTB_MouseCaptureChanged
            RemoveHandler PeripheralCB.SelectedIndexChanged, AddressOf PeripheralCB_SelectedIndexChanged

            If Not Joy = Nothing Then
                If SDL_GameControllerGetAttached(Joy) = SDL_bool.SDL_TRUE Then
                    SDL_GameControllerClose(Joy)
                End If
                Joy = Nothing
            End If

            If Joystick(PlayerTab.SelectedIndex) < ControllerCB.Items.Count - 1 Then
                ControllerCB.SelectedIndex = Joystick(PlayerTab.SelectedIndex) + 1
            Else
                ControllerCB.SelectedIndex = 0
                Joystick(PlayerTab.SelectedIndex) = -1
            End If

            DeadzoneTB.Value = Deadzone(PlayerTab.SelectedIndex)
            If Not Peripheral(PlayerTab.SelectedIndex) = "0" And Not Peripheral(PlayerTab.SelectedIndex) = "1" Then
                PeripheralCB.SelectedIndex = 0
            Else
                PeripheralCB.SelectedIndex = Peripheral(PlayerTab.SelectedIndex)
            End If

            If Not Joystick(PlayerTab.SelectedIndex) = -1 Then
                Joy = SDL_GameControllerOpen(Joystick(PlayerTab.SelectedIndex))
            End If

            AddHandler ControllerCB.SelectedIndexChanged, AddressOf ControllerCB_SelectedIndexChanged
            AddHandler DeadzoneTB.MouseCaptureChanged, AddressOf DeadzoneTB_MouseCaptureChanged
            AddHandler PeripheralCB.SelectedIndexChanged, AddressOf PeripheralCB_SelectedIndexChanged

            ActiveControl = Nothing
            UpdateButtonLabels()

        Catch ex As Exception
            MsgBox("Error Changing Controller: " & ex.InnerException.Message)
            DeadzoneTB.Value = Deadzone(PlayerTab.SelectedIndex)

            ControllerCB.SelectedIndex = 0
            Joystick(PlayerTab.SelectedIndex) = -1

            If Not Peripheral(PlayerTab.SelectedIndex) = "0" And Not Peripheral(PlayerTab.SelectedIndex) = "1" Then
                PeripheralCB.SelectedIndex = 0
            Else
                PeripheralCB.SelectedIndex = Peripheral(PlayerTab.SelectedIndex)
            End If

            AddHandler ControllerCB.SelectedIndexChanged, AddressOf ControllerCB_SelectedIndexChanged
            AddHandler DeadzoneTB.MouseCaptureChanged, AddressOf DeadzoneTB_MouseCaptureChanged
            AddHandler PeripheralCB.SelectedIndexChanged, AddressOf PeripheralCB_SelectedIndexChanged

            ActiveControl = Nothing
            UpdateButtonLabels()

        End Try

    End Sub

    Private Sub btn_Close_Click(sender As Object, e As EventArgs) Handles btn_Close.Click
        If MainformRef.IsFileInUse(MainformRef.NullDCPath & "\mednafen\stdout.txt") And File.Exists(MainformRef.NullDCPath & "\mednafen\stdout.txt") Then
            MsgBox("Cannot Save While Mednafen Is Running")
        Else
            MainformRef.ConfigFile.KeyMapProfile = cbProfiles.Text.Trim
            MainformRef.ConfigFile.SaveFile(False)
            SaveEverything()
            Me.Close()

        End If

    End Sub

    Private Sub ControllerCB_SelectedIndexChanged(sender As Object, e As EventArgs)

        Try
            'Console.Write("Changing Controller: ")
            If Not Joy = Nothing Then
                If SDL_GameControllerGetAttached(Joy) = SDL_bool.SDL_TRUE Then
                    SDL_GameControllerClose(Joy)
                End If
                Joy = Nothing
            End If

            If ControllerCB.SelectedValue >= 0 Then
                Joy = SDL_GameControllerOpen(ControllerCB.SelectedValue)
            End If

            'Console.WriteLine("joystick: " & ControllerCB.SelectedValue)
            Joystick(PlayerTab.SelectedIndex) = ControllerCB.SelectedValue
        Catch ex As Exception
            MsgBox("Error Opening Controller: " & ex.InnerException.Message)

        End Try


    End Sub

    Private Sub ButtonClicked(ByVal _keycode As String, ByVal _down As Boolean)

        If Not ButtonsDown.ContainsKey(_keycode) Then

            If Not Currently_Binding Is Nothing And (_down Or _keycode = "k13") Then
                If TypeOf Currently_Binding Is keybindButton Then
                    Dim _tmp As keybindButton = Currently_Binding
                    _tmp.SetKeyCode(_keycode, PlayerTab.SelectedIndex)
                    _tmp.BackColor = Color.White
                End If

                Currently_Binding = Nothing
                ActiveControl = Nothing

            End If

            ' rn all this does is highlight
            For Each _btns As Control In ControllersTab.SelectedTab.Controls
                If TypeOf _btns Is TabControl Then
                    Dim _innerTabControl As TabControl = _btns
                    For Each _innercontrols In _innerTabControl.SelectedTab.Controls

                        If TypeOf _innercontrols Is keybindButton Then
                            Dim _btn As keybindButton = _innercontrols
                            If _btn.GetKeyCode(PlayerTab.SelectedIndex) = _keycode Then
                                If _down Then
                                    _btn.BackColor = Color.Green

                                Else
                                    _btn.BackColor = Color.White

                                End If
                            End If
                        End If

                    Next

                End If


            Next

            If _down Then
                ButtonsDown.Add(_keycode, True)
            End If

        End If
    End Sub

    Private Sub PeripheralCB_SelectedIndexChanged(sender As Object, e As EventArgs)
        Peripheral(PlayerTab.SelectedIndex) = PeripheralCB.SelectedIndex
        If PlayerTab.SelectedIndex = 0 Then
            MainformRef.ConfigFile.Peripheral = PeripheralCB.SelectedIndex
            MainformRef.ConfigFile.SaveFile(False)
        End If

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles btnSDL.Click
        If MainformRef.IsNullDCRunning Or Not MainformRef.MednafenLauncher.MednafenInstance Is Nothing Then
            MsgBox("Cannot remap while emulation is running.")
            Exit Sub
        End If

        If ControllerCB.SelectedIndex = 0 Then
            MsgBox("Select a CONTROLLER from the list first. For Keyboard just click a button on the right, when it turns red press a key on your keyboard.")
        Else
            frmSDLMappingTool.ShowDialog(Me)
        End If

    End Sub

    Private Sub frmKeyMapperSDL_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.fan_icon_text
    End Sub

    Private Sub btnResetAll_Click(sender As Object, e As EventArgs) Handles ResetAllToolStripMenuItem.Click
        If MainformRef.IsNullDCRunning Or Not MainformRef.MednafenLauncher.MednafenInstance Is Nothing Then
            MsgBox("Cannot reset all while emulation is running.")
            Exit Sub
        End If

        Dim result1 As DialogResult = MessageBox.Show("This will Reset ALL the controls, k?", "Reset All?", MessageBoxButtons.YesNo)

        If result1 = DialogResult.Yes Then
            GenerateDefaults(GetControlsFilePath)
            LoadSettings()
            'If File.Exists(MainformRef.NullDCPath & "\Controls.bear") Then File.Delete(MainformRef.NullDCPath & "\Controls.bear")
            If File.Exists(MainformRef.NullDCPath & "\bearcontrollerdb.txt") Then File.Delete(MainformRef.NullDCPath & "\bearcontrollerdb.txt")
            If File.Exists(MainformRef.NullDCPath & "\dc\bearcontrollerdb.txt") Then File.Delete(MainformRef.NullDCPath & "\dc\bearcontrollerdb.txt")
            If File.Exists(MainformRef.NullDCPath & "\mednafenmapping.txt") Then File.Delete(MainformRef.NullDCPath & "\mednafenmapping.txt")
            UpdateControllersList()
            Me.Close()

        End If

        If File.Exists(MainformRef.NullDCPath & "\mednafen\mednafen.cfg") Then
            Dim _cfg = File.ReadAllLines(MainformRef.NullDCPath & "\mednafen\mednafen.cfg")
            Dim LineCount = 0
            For Each _line As String In _cfg
                For i = 1 To 12
                    If _line.Contains(".port" & i & ".") Then
                        _cfg(LineCount) = _line.Split(" ")(0)
                    End If
                Next

                LineCount += 1
            Next

            File.WriteAllLines(MainformRef.NullDCPath & "\mednafen\mednafen.cfg", _cfg)

        End If

    End Sub

    Private Sub ControllersTab_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ControllersTab.SelectedIndexChanged
        If ControllersTab.SelectedIndex = 1 Then
            TabControl1.SelectedIndex = PeripheralCB.SelectedIndex

        End If

    End Sub

    Private Sub ImportMappingStringToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ImportMappingStringToolStripMenuItem.Click
        If MainformRef.IsNullDCRunning Or Not MainformRef.MednafenLauncher.MednafenInstance Is Nothing Then
            MsgBox("Cannot edit mapping string while emulation is running.")
            Exit Sub
        End If

        If ControllerCB.SelectedIndex = 0 Then
            MsgBox("Mapping Strings are for Controllers only, select one from the list below.")
        Else
            frmMappingString.ShowDialog(Me)
        End If

    End Sub

    Private Sub ExportMappingStringToolStripMenuItem_Click(sender As Object, e As EventArgs)
        If Not My.Computer.Clipboard.ContainsText Then
            MsgBox("Copy mapping text then try again.")
            Exit Sub
        End If

        Dim _KeyBindString = My.Computer.Clipboard.GetText










    End Sub

    Private Sub ControllerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ControllerToolStripMenuItem.Click
        If MainformRef.IsNullDCRunning Then
            MsgBox("Can't do that while NullDC is running.")
            Exit Sub
        End If
        AutoGenerateButtonConfigs("gamepad", PlayerTab.SelectedIndex + 1)
        UpdateButtonLabels()
    End Sub

    Private Sub ArcadeStickToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ArcadeStickToolStripMenuItem.Click
        If MainformRef.IsNullDCRunning Then
            MsgBox("Can't do that while NullDC is running.")
            Exit Sub
        End If
        AutoGenerateButtonConfigs("fightstick", PlayerTab.SelectedIndex + 1)
        UpdateButtonLabels()
    End Sub

    Private Sub KeyboardToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles KeyboardToolStripMenuItem.Click
        If MainformRef.IsNullDCRunning Then
            MsgBox("Can't do that while NullDC is running.")
            Exit Sub
        End If
        AutoGenerateButtonConfigs("keyboard", PlayerTab.SelectedIndex + 1)
        UpdateButtonLabels()
    End Sub

    Private Sub NewToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NewToolStripMenuItem.Click
        frmNewProfile.ShowDialog(Me)

    End Sub

    Private Sub DeleteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteToolStripMenuItem.Click
        Try
            If cbProfiles.Text = "Default" Then
                MsgBox("Cannot delete Default profile")
            Else
                If File.Exists(MainformRef.NullDCPath & "\Controls_" & cbProfiles.Text.Trim & ".bear") Then
                    File.Delete(MainformRef.NullDCPath & "\Controls_" & cbProfiles.Text.Trim & ".bear")
                    LoadSettings()
                End If

            End If
        Catch ex As Exception
            MsgBox("Unable to delete profile: " & ex.InnerException.Message)

        End Try


    End Sub

End Class

Public Class KeyBind
    Public KC As String
    Public Name As String
    Public Button As Button
    Public Platform As String

    Public Sub New(ByVal _kc As String, ByVal _name As String, ByRef _frm As frmKeyMapperSDL, ByVal _plat As String)
        KC = _kc
        Name = _name
        Button = _frm.Controls.Find(_name, True)(0)
        Platform = _plat

        If Button Is Nothing Then
            MsgBox("button not found: " & _name)
        End If

    End Sub

End Class