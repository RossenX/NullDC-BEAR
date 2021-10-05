Imports System.Runtime.InteropServices
Imports SDL2
Imports SDL2.SDL
Imports System.IO
Imports System.Text

Public Class frmKeyMapperSDL

    ' From the Configs
    Dim Joystick(2) As Short
    Dim Deadzone(2) As Short
    Dim Peripheral(2) As Short

    Public Joy As IntPtr
    Dim _InputThread As Threading.Thread
    Dim Currently_Binding As Button
    Dim ButtonsDown As New Dictionary(Of String, Boolean)

    ' Little things to know which systems were changed, so we don't have to always save settings that were not changed
    Public NaomiChanged As Boolean = False
    Public DreamcastChanged As Boolean = False
    Public MednafenChanged As Boolean = False
    Public MupenChanged As Boolean = False

    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
    Private Shared Function PostMessage(ByVal hWnd As IntPtr, ByVal Msg As UInteger, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As Boolean
    End Function

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

    Private Sub FrmKeyMapperSDL_VisibilityChange(sender As Object, e As EventArgs) Handles MyBase.VisibleChanged

        Try
            If Me.Visible Then

                SDL_SetHint(SDL_HINT_JOYSTICK_ALLOW_BACKGROUND_EVENTS, "1")
                SDL_SetHint(SDL_HINT_XINPUT_ENABLED, "0")

                Me.CenterToParent()
                Me.Icon = My.Resources.fan_icon_text
                ReloadTheme()

                If SDL_WasInit(SDL_INIT_GAMECONTROLLER) = 0 Then
                    SDL_InitSubSystem(SDL_INIT_GAMECONTROLLER)
                    Console.WriteLine("SDL_INIT GAME CONTROLLER")
                End If

                SDL_Delay(500)

                If SDL_WasInit(SDL_INIT_JOYSTICK) = 0 Then
                    SDL_InitSubSystem(SDL_INIT_JOYSTICK)
                    Console.WriteLine("SDL_INIT JOYSTICK")
                End If

                SDL_Delay(500)
                DoInitialSetupShit()
                LoadSettings()

                _InputThread = New Threading.Thread(AddressOf InputThread) With {
                    .IsBackground = True
                }
                _InputThread.Start()

                AddHandler cbSDL.SelectedIndexChanged, AddressOf SDLVersionChanged
                AddHandler DeadzoneTB.MouseCaptureChanged, AddressOf DeadzoneTB_MouseCaptureChanged
                AddHandler DeadzoneTB.ValueChanged, Sub() Deadzonetext.Text = "Deadzone: " & DeadzoneTB.Value
                AddHandler PeripheralCB.SelectedIndexChanged, AddressOf PeripheralCB_SelectedIndexChanged
                AddHandler PlayerTab.SelectedIndexChanged, AddressOf PlayerTab_SelectedIndexChanged
                AddHandler ControllersTab.SelectedIndexChanged, Sub() ActiveControl = Nothing
                AddHandler cbProfiles.SelectedIndexChanged, AddressOf CbProfileIndexChanged

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
            Else



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

        Dim lines(300) As String

        Dim MednafenControllerID = GetMednafenControllerIDs()

        lines(0) = MainformRef.Ver
        lines(1) = "Joystick=" & Joystick(0) & "|" & Joystick(1)
        lines(2) = "Deadzone=" & Deadzone(0) & "|" & Deadzone(1)
        lines(3) = "Peripheral=" & Peripheral(0) & "|" & Peripheral(1)
        lines(4) = "MednafenControllerID=0x0|0x0"

        ' Shift Mednafen IDs

        Dim CurrentIDs As New ArrayList
        Dim MednafenIDs As New ArrayList

        For i = 0 To SDL_NumJoysticks() - 1
            CurrentIDs.Add(SDL_JoystickGetDeviceGUID(i).ToString.Substring(0, 23))
        Next

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
        TabsToSave.Add(page_PCE_Gamepad)
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
            MupenChanged = True
        Else
            Dim _tmpConfigFile As String() = File.ReadAllLines(_filepath)

            For Each _line In lines
                If Not _tmpConfigFile.Contains(_line) And _line IsNot Nothing Then

                    ' Deadzone change applies to all
                    If _line.StartsWith("Deadzone=") Or _line.StartsWith("Joystick=") Then
                        NaomiChanged = True
                        DreamcastChanged = True
                        MednafenChanged = True
                        MupenChanged = True
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
                    End If

                    ' Mupen
                    If _line.StartsWith("mup_") Then
                        MupenChanged = True
                    End If

                End If
            Next

        End If

        File.WriteAllLines(_filepath, lines)

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
        If _profile IsNot Nothing Then
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

    Private Sub CbProfileIndexChanged(sender As Object, e As EventArgs)
        RemoveHandler cbProfiles.SelectedIndexChanged, AddressOf CbProfileIndexChanged
        NaomiChanged = True
        DreamcastChanged = True
        MednafenChanged = True
        MupenChanged = True

        LoadSettings(cbProfiles.Text.Trim)
        AddHandler cbProfiles.SelectedIndexChanged, AddressOf CbProfileIndexChanged
    End Sub


    Public Sub LoadSettings(Optional _profile As String = Nothing)

        Dim KeyProfileFile As String = GetControlsFilePath(_profile)

        If Not File.Exists(KeyProfileFile) Then
            GenerateDefaults(KeyProfileFile)
        Else
            Dim a = File.ReadAllLines(KeyProfileFile)
            If a.Count < 200 Then
                GenerateDefaults(KeyProfileFile)
            End If
        End If

        LoadProfiles(_profile)

        Dim configLines As String()
        Console.WriteLine("Loaded Profile: " & KeyProfileFile)

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

        If ControllerCB.SelectedValue > -1 Then
            Joy = SDL_GameControllerOpen(ControllerCB.SelectedValue)
            CheckForMappingFile(Joy)
            Console.WriteLine(Joy)
        End If

        cbSDL.SelectedItem = MainformRef.ConfigFile.SDLVersion
        DebugControlsCB.SelectedIndex = MainformRef.ConfigFile.DebugControls

        AddHandler ControllerCB.SelectedIndexChanged, AddressOf ControllerCB_SelectedIndexChanged

    End Sub

    Private Sub GetKeyCodeFromFile(ByRef _btn As keybindButton, ByRef _configlines As String())

        For Each line As String In _configlines
            If line.StartsWith(_btn.Name & "=") Or
                line.StartsWith("med_" & _btn.ConfigString.Split(",")(0) & "=") Or
                line.StartsWith("mup_" & _btn.ConfigString.Split(",")(0) & "=") Then

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

                            Me.Invoke(Sub()

                                          Console.WriteLine("Axis Motion: " & _event.caxis.axis & "|" & _event.caxis.axisValue & "|" & SDL_JoystickNumAxes(SDL_GameControllerGetJoystick(Joy)))

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
        TabsToReset.Add(Page_PCE_Gamepad)
        TabsToReset.Add(page_N64_Controller)

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

        _InputThread.Abort()
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

        ' We need the peripheral beforehand so we know which control settings to actually use
        Dim tempPeripheral As String() = {"", ""}
        Dim tempJoystick As String() = {"", ""}
        Dim TempDeadzone As String() = {"", ""}

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
        Next

        ' Naomi Controls
        Dim linenumber = 0
        Try
            If NaomiChanged Then
                Console.WriteLine("Saving Naomi Controls")
                Dim NaomiConfigs() As String = File.ReadAllLines(MainformRef.NullDCPath & "\nullDC.cfg")
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
            MsgBox("Failed to save NullDC-Naomi Controls. Error: " & ex.Message)

        End Try

        ' Dreamcast Controls
        Try
            linenumber = 0
            If DreamcastChanged Then
                Console.WriteLine("Saving Dreamcast Controls")
                Dim DreamcastConfigs() As String = File.ReadAllLines(MainformRef.NullDCPath & "\dc\nullDC.cfg")
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
            MsgBox("Failed to save NullDC-Dreamcast Controls. Error: " & ex.Message)
        End Try

        ' Mupen Controls
        Try

            If MupenChanged Then

                Console.WriteLine("Saving Mupen Controls")

                Dim TempMappingString As String() = {"", ""}
                Dim MupenControls As String() = {"", ""}

                ' Ok new IDEA. FULLY MANUAL

                ' Here's what we need in the configs
                ' version = 2.000000
                ' mode = 0
                ' device = {Joystick}
                ' name = {Name}
                ' plugged = True
                ' plugin = 2
                ' mouse = False
                ' MouseSensitivity = "2.00,2.00"
                ' AnalogDeadzone = "6553,6553"
                ' AnalogPeak = "32768,32768"

                ' DPad R
                ' DPad L
                ' DPad D
                ' DPad U
                ' Start
                ' Z Trig
                ' B Button
                ' A Button
                ' C Button R 
                ' C Button L
                ' C Button D 
                ' C Button U 
                ' R Trig
                ' L Trig
                ' Mempak switch
                ' Rumblepak switch
                ' X Axis
                ' Y Axis

                ' Initial Setup Stuff of values that are always the same
                For i = 0 To 1
                    MupenControls(i) += "Input-SDL-Control" & i + 1 & "]" & vbNewLine
                    MupenControls(i) += vbNewLine
                    MupenControls(i) += "version = 2.000000" & vbNewLine

                    MupenControls(i) += "mode = 0" & vbNewLine

                    MupenControls(i) += "device = " & Joystick(i) & vbNewLine

                    If tempJoystick(i) = -1 Then
                        MupenControls(i) += "name = ""Keyboard""" & vbNewLine
                    Else
                        TempMappingString(i) = SDL_GameControllerMappingForIndex(tempJoystick(i))
                        MupenControls(i) += "name = """ & SDL_GameControllerNameForIndex(tempJoystick(i)) & """" & vbNewLine
                    End If

                    MupenControls(i) += "plugged = True" & vbNewLine

                    MupenControls(i) += "plugin = 2" & vbNewLine
                    MupenControls(i) += "MouseSensitivity = ""2.00,2.00"" " & vbNewLine

                    MupenControls(i) += "AnalogDeadzone = " & Math.Floor(32768 * (TempDeadzone(i) / 100)) & "," & Math.Floor(32768 * (TempDeadzone(i) / 100)) & "" & vbNewLine
                    MupenControls(i) += "AnalogPeak = ""32768,32768"" " & vbNewLine

                Next

                ' Go through the configs and generate a valid string for each of the buttons
                ' Right so we save these axis for now since we need both of them before we can generate a string which we'll do in the end
                Dim Y_AxisPlus As String() = {"", ""}
                Dim Y_AxisMinus As String() = {"", ""}

                Dim X_AxisPlus As String() = {"", ""}
                Dim X_AxisMinus As String() = {"", ""}

                For Each control_line In ControlsConfigs
                    If control_line.StartsWith("mup_") Then
                        If control_line.Contains("Z Trig") Then
                            Console.WriteLine("hello")
                        End If

                        For i = 0 To 1
                            ' Check if this is the axis bind becuase that has to be handled differently
                            If control_line.StartsWith("mup_X Axis") Or control_line.StartsWith("mup_Y Axis") Then

                                If control_line.StartsWith("mup_X Axis+") Then
                                    X_AxisPlus(i) = BEARButtonToMupenButton(TempMappingString(i), control_line.Split("=")(1).Split("|")(i))

                                ElseIf control_line.StartsWith("mup_X Axis-") Then
                                    X_AxisMinus(i) = BEARButtonToMupenButton(TempMappingString(i), control_line.Split("=")(1).Split("|")(i))

                                ElseIf control_line.StartsWith("mup_Y Axis+") Then
                                    Y_AxisPlus(i) = BEARButtonToMupenButton(TempMappingString(i), control_line.Split("=")(1).Split("|")(i))

                                ElseIf control_line.StartsWith("mup_Y Axis-") Then
                                    Y_AxisMinus(i) = BEARButtonToMupenButton(TempMappingString(i), control_line.Split("=")(1).Split("|")(i))

                                End If

                            Else
                                MupenControls(i) += control_line.Split("=")(0).Replace("mup_", "") & " = " & BEARButtonToMupenButton(TempMappingString(i), control_line.Split("=")(1).Split("|")(i)) & vbNewLine

                            End If
                        Next
                    End If

                Next

                ' Do The Analog Stuff
                For i = 0 To 1

                    If X_AxisMinus(i) = "" Or X_AxisPlus(i) = "" Then
                        MupenControls(i) += "X Axis = "
                    Else
                        MupenControls(i) += "X Axis = " & X_AxisMinus(i).Split("(")(0) & "(" & X_AxisMinus(i).Split("(")(1).Replace(")", "") & "," & X_AxisPlus(i).Split("(")(1).Replace(")", "") & ")" & vbNewLine
                    End If


                    If Y_AxisMinus(i) = "" Or Y_AxisPlus(i) = "" Then
                        MupenControls(i) += "Y Axis = "
                    Else
                        MupenControls(i) += "Y Axis = " & Y_AxisMinus(i).Split("(")(0) & "(" & Y_AxisMinus(i).Split("(")(1).Replace(")", "") & "," & Y_AxisPlus(i).Split("(")(1).Replace(")", "") & ")" & vbNewLine
                    End If

                Next

                Dim MupenConfigs = File.ReadAllText(MainformRef.NullDCPath & "\Mupen64Plus\mupen64plus.cfg").Split("[")

                Dim PCount = 0
                For i = 0 To MupenConfigs.Count - 1

                    If MupenConfigs(i).StartsWith("Input-SDL-Control1]") Then MupenConfigs(i) = MupenControls(0)

                    If MupenConfigs(i).StartsWith("Input-SDL-Control2]") Then MupenConfigs(i) = MupenControls(1) : Exit For

                    ' Just in case we make it down this far somehow
                    If MupenConfigs(i).StartsWith("Input-SDL-Control3]") Then Exit For

                Next

                For i = 1 To MupenConfigs.Count - 1
                    MupenConfigs(i) = "[" & MupenConfigs(i)

                Next

                For i = 0 To MupenConfigs.Count - 1
                    If i > 0 Then
                        MupenConfigs(i) = vbNewLine & MupenConfigs(i).Trim
                    Else
                        MupenConfigs(i) = MupenConfigs(i).Trim
                    End If

                Next

                File.WriteAllLines(MainformRef.NullDCPath & "\Mupen64Plus\mupen64plus.cfg", MupenConfigs)

            End If

        Catch ex As Exception
            MsgBox("Failed to save Mupen Controls. Error: " & ex.Message)

        End Try

        ' Flycast
        Try
            Console.WriteLine("Saving Flycast Controls")
            For _playerID = 0 To 1 ' p1 p2
                For _JoystickID = 0 To SDL_NumJoysticks() - 1
                    If tempJoystick(_playerID) = _JoystickID Then
                        GenerateFlycastMapping(SDL_JoystickNameForIndex(_JoystickID), "arcade", ControlsConfigs, _playerID, tempPeripheral(_playerID))
                        GenerateFlycastMapping(SDL_JoystickNameForIndex(_JoystickID), "dc", ControlsConfigs, _playerID, tempPeripheral(_playerID))

                    End If
                Next

                If _playerID = 0 Then
                    GenerateFlycastMapping("Keyboard", "dc", ControlsConfigs, 0, tempPeripheral(_playerID))
                    GenerateFlycastMapping("Keyboard", "arcade", ControlsConfigs, 0, tempPeripheral(_playerID))

                End If

            Next
        Catch ex As Exception
            MsgBox("Failed to save Flycast Controls. Error: " & ex.Message)

        End Try

        ' Mednafen Controls
        Try

            btn_Close.Text = "Saving..."

            ' Open GameController this time but with xinput on, to fix the mednafen IDs

            Dim MednafenControllerIDs = Rx.GetMednafenControllerIDs
            Dim CurrentIDs As New ArrayList
            Dim MednafenIDs As New ArrayList

            Dim OriginalGUID As New ArrayList

            Dim TempMappingStrings = {"", ""}

            For i = 0 To 1
                If Not tempJoystick(i) = "-1" Then
                    TempMappingStrings(i) = SDL_GameControllerMappingForGUID(SDL_JoystickGetDeviceGUID(tempJoystick(i)))
                End If
            Next

            For i = 0 To SDL_NumJoysticks() - 1
                CurrentIDs.Add(SDL_JoystickGetDeviceGUID(i).ToString.Substring(0, 23))
                OriginalGUID.Add(SDL_JoystickGetDeviceGUID(i))
            Next

            ' Give unique names to things with the same GUID

            For i = 0 To CurrentIDs.Count - 1
                Dim DuplicateCount = 0
                For j = 0 To CurrentIDs.Count - 1
                    If Not i = j Then
                        If CurrentIDs(j) = CurrentIDs(i) Then
                            CurrentIDs(j) += DuplicateCount.ToString
                            DuplicateCount += 1
                        End If
                    End If
                Next
            Next

            SDL_QuitSubSystem(SDL_INIT_GAMECONTROLLER)
            SDL_QuitSubSystem(SDL_INIT_JOYSTICK)
            SDL_Quit()
            SDL_Delay(2500)

            While SDL_WasInit(SDL_INIT_JOYSTICK) Or SDL_WasInit(SDL_INIT_GAMECONTROLLER)
                SDL_Delay(100)
            End While

            SDL_SetHint(SDL_HINT_XINPUT_ENABLED, "1")
            SDL_Init(SDL_INIT_GAMECONTROLLER)
            SDL_Init(SDL_INIT_JOYSTICK)

            For i = 0 To SDL_NumJoysticks() - 1
                MednafenIDs.Add(SDL_JoystickGetDeviceGUID(i).ToString.Substring(0, 23))
            Next

            For i = 0 To MednafenIDs.Count - 1
                Dim DuplicateCount = 0
                For j = 0 To MednafenIDs.Count - 1
                    If Not i = j Then
                        If MednafenIDs(j) = MednafenIDs(i) Then
                            MednafenIDs(j) += DuplicateCount.ToString
                            DuplicateCount += 1
                        End If
                    End If
                Next
            Next

            Dim MednafenControllerID = {"0x0", "0x0"}
            For i = 0 To 1
                If Not tempJoystick(i) = "-1" Then
                    Dim ShiftID = 0
                    For Each _medID In MednafenIDs
                        If CurrentIDs(CInt(tempJoystick(i))) = _medID Then
                            MednafenControllerID(i) = MednafenControllerIDs(ShiftID)
                            Exit For

                        End If
                        ShiftID += 1
                    Next
                Else
                    MednafenControllerID(i) = "0x0"

                End If
            Next

            If MednafenChanged Then
                Console.WriteLine("Saving Mednafen Controls")
                Dim _TranslatedControls(2) As Dictionary(Of String, String)

                For i = 0 To 1
                    If Not tempJoystick(i) = "-1" Then
                        Dim _isx = False
                        If MednafenControllerID(i).Contains("xinput_") Then _isx = True
                        Dim tmpJoy = SDL_JoystickOpen(CInt(tempJoystick(i)))
                        _TranslatedControls(i) = ConvertBEARMappingToMednafen(TempMappingStrings(i), SDL_JoystickNumAxes(tmpJoy), _isx)
                        SDL_JoystickClose(tmpJoy)

                    End If
                Next

                Dim MednafenConfigs() As String = File.ReadAllLines(MainformRef.NullDCPath & "\mednafen\mednafen.cfg")
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
                        MednafenConfigs(linenumber) = "input.joystick.axis_threshold " & TempDeadzone(0)
                        linenumber += 1
                        Continue For
                    End If

                    If MednafenChanged Then
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

                                            _tmpID = MednafenControllerID(_player - 1)
                                            If _tmpID.Contains("xinput_") Then
                                                _tmpID = _tmpID.Replace("xinput_", "")
                                            End If
                                            tmpControlString += "joystick " & _tmpID
                                            If _tmpID Is Nothing Then
                                                tmpControlString = control_line.Split("=")(0).Replace("<port>", _player.ToString)
                                            End If

                                            If _TranslatedControls(_player - 1) Is Nothing Then
                                                tmpControlString = control_line.Split("=")(0).Replace("<port>", _player.ToString)

                                            ElseIf _TranslatedControls(_player - 1).ContainsKey(_KeyCode) And _tmpID.Trim.Length > 1 Then ' Failsafe if we fail to get the controller ID then do NOT set controls, because it'll cause the whole config file to be useless and need to be reset before it can be used

                                                tmpControlString += " " & _TranslatedControls(_player - 1)(_KeyCode)

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
            MsgBox("Failed to save Mednafen Controls. Error: " & ex.Message)

        End Try

        ' Check if nullDC is running to hotload the settings this is getting to annoying to keep up, i'ma just disable it for now
        If MainformRef.IsNullDCRunning Then
            ' If MainformRef.ConfigFile.Game.Split("-")(0).ToLower = "na" Then SendMessage(MainformRef.NullDCLauncher.NullDCproc.MainWindowHandle, &H111, 141, 0)
            ' If MainformRef.ConfigFile.Game.Split("-")(0).ToLower = "dc" Then SendMessage(MainformRef.NullDCLauncher.NullDCproc.MainWindowHandle, &H111, 180, 0) ' 180 144 141

        End If

    End Sub

    Private Sub DoAnnoyingFlycastStuff(ByRef analogStuff As String,
                                       ByRef digitalStuff As String,
                                       ByRef EmulatorStuff As String,
                                       ByVal _ButtonAxisName As String,
                                       ByVal _ButtonName As String,
                                       ByVal _line As String,
                                       ByVal _player As Int16,
                                       ByVal _ControllerName As String,
                                       ByVal isAxis As Boolean)

        Dim _ButtonKey = _line.Split("=")(1).Split("|")(_player)

        If _line.Split("=")(1).Split("|")(_player).StartsWith("k") Then ' Key
            If _ControllerName = "Keyboard" Then ' Keyboard saves Keyboard keys, makes sense
                If Not KeyCodeToSDLScanCode(_ButtonKey.Remove(0, 1)) = "0" Then
                    If _ButtonName.Contains("analog") Or _ButtonName.Contains("trigger") Then
                        analogStuff += _ButtonName & " = " & KeyCodeToSDLScanCode(_ButtonKey.Remove(0, 1)) & vbNewLine
                    Else
                        digitalStuff += _ButtonName & " = " & KeyCodeToSDLScanCode(_ButtonKey.Remove(0, 1)) & vbNewLine
                    End If


                End If
            End If

        ElseIf _ButtonKey.StartsWith("b") Then ' Button
            If Not _ControllerName = "Keyboard" Then
                If isAxis Then
                    analogStuff += _ButtonName & " = " & _ButtonKey.Remove(0, 1) & vbNewLine

                Else
                    digitalStuff += _ButtonName & " = " & _ButtonKey.Remove(0, 1) & vbNewLine

                End If

            End If

        ElseIf _line.Split("=")(1).Split("|")(_player).StartsWith("a") Then ' Axis
            If Not _ControllerName = "Keyboard" Then
                Dim FlycastMap = _ButtonAxisName & " = " & _ButtonKey.Replace("+", "").Replace("-", "").Replace("~", "").Replace("a", "")

                If isAxis Then
                    digitalStuff += FlycastMap & vbNewLine

                Else
                    analogStuff += FlycastMap & vbNewLine

                End If

                If _ButtonKey.Contains("-") Then
                    analogStuff += FlycastMap.Split("=")(0).Trim & "_inverted = yes" & vbNewLine
                Else
                    analogStuff += FlycastMap.Split("=")(0).Trim & "_inverted = no" & vbNewLine
                End If

            End If

        End If


    End Sub

    ReadOnly BearToFlycast_Arcade As New Dictionary(Of String, String) From {
        {"I_BTN0_KEY", "btn_a"},
        {"I_BTN1_KEY", "btn_b"},
        {"I_TEST_KEY_1", "btn_dpad2_up"}, '
        {"I_COIN_KEY", "btn_d"},
        {"I_BTN2_KEY", "btn_c"}, '
        {"I_BTN3_KEY", "btn_x"}, '
        {"I_SERVICE_KEY_1", "btn_dpad2_down"}, '
        {"I_START_KEY", "btn_start"},
        {"I_RIGHT_KEY", "btn_dpad1_right"},
        {"I_LEFT_KEY", "btn_dpad1_left"},
        {"I_UP_KEY", "btn_dpad1_up"},
        {"I_DOWN_KEY", "btn_dpad1_down"},
        {"I_BTN4_KEY", "btn_y"}, '
        {"I_BTN5_KEY", "btn_z"}, '
        {"I_BC_03", "BC_03"},
        {"I_BC_14", "BC_14"},
        {"I_BC_25", "BC_25"},
        {"I_BC_01", "BC_01"},
        {"I_BC_12", "BC_12"},
        {"I_BC_34", "BC_34"},
        {"I_BC_45", "BC_45"},
        {"I_BC_012", "BC_012"},
        {"I_BC_345", "BC_345"}
    }

    ReadOnly BearToFlycast_DC As New Dictionary(Of String, String) From {
        {"CONT_A", "btn_a"},
        {"CONT_B", "btn_b"},
        {"CONT_C", "btn_c"}, ' These don't exist but just in case
        {"CONT_X", "btn_x"},
        {"CONT_Y", "btn_y"},
        {"CONT_Z", "btn_z"}, ' These don't exist but just in case
        {"CONT_START", "btn_start"},
        {"CONT_DPAD_RIGHT", "btn_dpad1_right"},
        {"CONT_DPAD_LEFT", "btn_dpad1_left"},
        {"CONT_DPAD_UP", "btn_dpad1_up"},
        {"CONT_DPAD_DOWN", "btn_dpad1_down"},
        {"CONT_ANALOG_LEFT", "btn_analog_left"},
        {"CONT_ANALOG_RIGHT", "btn_analog_right"},
        {"CONT_ANALOG_UP", "btn_analog_up"},
        {"CONT_ANALOG_DOWN", "btn_analog_down"},
        {"CONT_BC_XA", "BC_XA"},
        {"CONT_BC_YB", "BC_YB"},
        {"CONT_BC_CZ", "BC_LR"},
        {"CONT_BC_ABC", "BC_ABR"},
        {"CONT_BC_XYZ", "BC_XYL"},
        {"CONT_BC_AB", "BC_AB"},
        {"CONT_BC_BC", "BC_BR"},
        {"CONT_BC_XY", "BC_XY"},
        {"CONT_BC_YZ", "BC_YL"},
        {"CONT_LSLIDER", "btn_trigger_left"},
        {"CONT_RSLIDER", "btn_trigger_right"}
    }

    ReadOnly BearToFlycast_DC_ArcadeStick As New Dictionary(Of String, String) From {
        {"STICK_A", "btn_a"},
        {"STICK_B", "btn_b"},
        {"STICK_C", "btn_c"},
        {"STICK_X", "btn_x"},
        {"STICK_Y", "btn_y"},
        {"STICK_Z", "btn_z"},
        {"STICK_START", "btn_start"},
        {"STICK_DPAD_RIGHT", "btn_dpad1_right"},
        {"STICK_DPAD_LEFT", "btn_dpad1_left"},
        {"STICK_DPAD_UP", "btn_dpad1_up"},
        {"STICK_DPAD_DOWN", "btn_dpad1_down"},
        {"STICK_ANALOG_LEFT", "btn_analog_left"},
        {"STICK_ANALOG_RIGHT", "btn_analog_right"},
        {"STICK_ANALOG_UP", "btn_analog_up"},
        {"STICK_ANALOG_DOWN", "btn_analog_down"},
        {"STICK_BC_XA", "BC_XA"},
        {"STICK_BC_YB", "BC_YB"},
        {"STICK_BC_CZ", "BC_LR"},
        {"STICK_BC_ABC", "BC_ABR"},
        {"STICK_BC_XYZ", "BC_XYL"},
        {"STICK_BC_AB", "BC_AB"},
        {"STICK_BC_BC", "BC_BR"},
        {"STICK_BC_XY", "BC_XY"},
        {"STICK_BC_YZ", "BC_YL"},
        {"CONT_LSLIDER", "btn_trigger_left"}, ' These don't exist but just in case
        {"CONT_RSLIDER", "btn_trigger_right"} ' These don't exist but just in case
    }

    Private Sub BEARToFlycastButton(ByRef _Section As String, ByVal FlycastButtonName As String, ByVal FlycastButtonValue As String, ByRef _bindingCount As Int16)
        _Section += "bind" & _bindingCount & " = " & FlycastButtonValue & ":" & FlycastButtonName & vbNewLine
        _bindingCount += 1
    End Sub

    Private Sub GenerateFlycastMapping(ByVal _ControllerName As String, ByVal _System As String, ByVal _Controls As String(), ByVal _player As Int16, ByVal _Peripheral As String)

        Dim FlycastDreamcastMappingFile As String = ""
        Dim analogStuff = "[analog]" & vbNewLine
        Dim analogBindCount = 0
        Dim digitalStuff = "[digital]" & vbNewLine
        Dim digitalBindCount = 0
        Dim EmulatorStuff = "[emulator]" & vbNewLine & "version = 3" & vbNewLine & "mapping_name = " & _ControllerName & vbNewLine

        For Each _line In _Controls

            If _line.Split("=").Count = 1 Then Continue For

            If _line.StartsWith("Deadzone=") Then
                EmulatorStuff += "dead_zone = " & _line.Split("=")(1).Split("|")(_player) & vbNewLine
                Continue For
            End If

            Dim ButtonBEARName = _line.Split("=")(0)
            Dim BEARButtonValue = _line.Split("=")(1).Split("|")(_player)

            If BEARButtonValue = "k0" Or
                (_ControllerName = "Keyboard" And Not BEARButtonValue.StartsWith("k")) Or
                (Not _ControllerName = "Keyboard" And BEARButtonValue.StartsWith("k")) Then
                Continue For
            End If

            If _ControllerName = "Keyboard" Then
                BEARButtonValue = "k" & KeyCodeToSDLScanCode(BEARButtonValue.Remove(0, 1))
            End If

            If _System = "arcade" Then

                If Not BearToFlycast_Arcade.ContainsKey(ButtonBEARName) Then Continue For

                If _line.Split("=")(1).Split("|")(_player).StartsWith("a") Then
                    BEARToFlycastButton(analogStuff, BearToFlycast_Arcade(ButtonBEARName), BEARButtonValue.Substring(1, BEARButtonValue.Length - 1), analogBindCount)
                Else
                    BEARToFlycastButton(digitalStuff, BearToFlycast_Arcade(ButtonBEARName), BEARButtonValue.Substring(1, BEARButtonValue.Length - 1), digitalBindCount)
                End If

            Else

                If _Peripheral = "1" Then ' Arcade Stick
                    If Not BearToFlycast_DC_ArcadeStick.ContainsKey(ButtonBEARName) Then Continue For

                    If _line.Split("=")(1).Split("|")(_player).StartsWith("a") Then
                        BEARToFlycastButton(analogStuff, BearToFlycast_DC_ArcadeStick(ButtonBEARName), BEARButtonValue.Substring(1, BEARButtonValue.Length - 1), analogBindCount)
                    Else
                        BEARToFlycastButton(digitalStuff, BearToFlycast_DC_ArcadeStick(ButtonBEARName), BEARButtonValue.Substring(1, BEARButtonValue.Length - 1), digitalBindCount)
                    End If

                Else ' Controller
                    If Not BearToFlycast_DC.ContainsKey(ButtonBEARName) Then Continue For

                    If _line.Split("=")(1).Split("|")(_player).StartsWith("a") Then
                        BEARToFlycastButton(analogStuff, BearToFlycast_DC(ButtonBEARName), BEARButtonValue.Substring(1, BEARButtonValue.Length - 1), analogBindCount)
                    Else
                        BEARToFlycastButton(digitalStuff, BearToFlycast_DC(ButtonBEARName), BEARButtonValue.Substring(1, BEARButtonValue.Length - 1), digitalBindCount)
                    End If

                End If

            End If

        Next

        Dim isArcadeHuh = ""
        If _System = "arcade" Then
            isArcadeHuh = "_arcade"
        End If

        If _ControllerName = "Keyboard" Then
            digitalStuff += "bind" & digitalBindCount & "= 43:btn_menu" & vbNewLine
        End If

        Dim FullMappingFile = ""
        If Not analogStuff.Trim = "[analog]" Then FullMappingFile += analogStuff & vbNewLine
        If Not digitalStuff.Trim = "[digital]" Then FullMappingFile += digitalStuff & vbNewLine
        If Not EmulatorStuff.Trim = "[emulator]" Then FullMappingFile += EmulatorStuff

        Dim MappingFileName = _ControllerName & isArcadeHuh & ".cfg"
        MappingFileName = MappingFileName.Replace("/", "-")
        MappingFileName = MappingFileName.Replace("\\", "-")
        MappingFileName = MappingFileName.Replace(":", "-")
        MappingFileName = MappingFileName.Replace("?", "-")
        MappingFileName = MappingFileName.Replace("*", "-")
        MappingFileName = MappingFileName.Replace("|", "-")
        MappingFileName = MappingFileName.Replace("""", "-")
        MappingFileName = MappingFileName.Replace("<", "-")
        MappingFileName = MappingFileName.Replace(">", "-")


        File.WriteAllText(MainformRef.NullDCPath & "\flycast\mappings\SDL_" & MappingFileName, FullMappingFile)

        Console.WriteLine("Created Flycast Mapping file for {0} player {1}", _ControllerName, _player)

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
        'Console.WriteLine("Bind clicked " & sender.Name)
        If sender.KeyLocked Then
            MsgBox("this bind Is locked For now")
            Exit Sub
        End If

        If Currently_Binding IsNot Nothing Then
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
                CheckForMappingFile(Joy)
            End If

            AddHandler ControllerCB.SelectedIndexChanged, AddressOf ControllerCB_SelectedIndexChanged
            AddHandler DeadzoneTB.MouseCaptureChanged, AddressOf DeadzoneTB_MouseCaptureChanged
            AddHandler PeripheralCB.SelectedIndexChanged, AddressOf PeripheralCB_SelectedIndexChanged

            ActiveControl = Nothing
            UpdateButtonLabels()

        Catch ex As Exception
            MsgBox("Error Changing Controller " & ex.Message)
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

    Private Sub CheckForMappingFile(ByRef _joy As IntPtr)
        Dim map = SDL_GameControllerMapping(_joy)
        If map Is Nothing Then
            MsgBox("No Mapping Found for This Controller")
            If Not ControllerCB.SelectedIndex = 0 Then
                frmSDLMappingTool.ShowDialog(Me)
            End If
        End If
    End Sub

    Private Sub Btn_Close_Click(sender As Object, e As EventArgs) Handles btn_Close.Click
        If MainformRef.IsFileInUse(MainformRef.NullDCPath & "\mednafen\stdout.txt") And File.Exists(MainformRef.NullDCPath & "\mednafen\stdout.txt") Then
            MsgBox("Cannot Save While Mednafen Is Running")
        Else
            MainformRef.ConfigFile.KeyMapProfile = cbProfiles.Text.Trim
            MainformRef.ConfigFile.SaveFile(False)
            SaveEverything()

            If SDL_WasInit(SDL_INIT_JOYSTICK) Or SDL_WasInit(SDL_INIT_GAMECONTROLLER) Then
                For i = 0 To SDL_NumJoysticks() - 1
                    Console.WriteLine("Disconnecting: " & SDL_GameControllerNameForIndex(i))
                    SDL_GameControllerClose(SDL_GameControllerFromInstanceID(i))
                Next
                SDL_QuitSubSystem(SDL_INIT_GAMECONTROLLER)
                SDL_QuitSubSystem(SDL_INIT_JOYSTICK)

                SDL_Quit()
            End If

            Me.Close()

        End If

    End Sub

    Private Sub ControllerCB_SelectedIndexChanged(sender As Object, e As EventArgs)

        Try
            SDL_GameControllerClose(Joy)
            Joy = Nothing

            If ControllerCB.SelectedValue >= 0 Then
                Joy = SDL_GameControllerOpen(ControllerCB.SelectedValue)
                CheckForMappingFile(Joy)
            End If

            'Console.WriteLine("joystick " & ControllerCB.SelectedValue)
            Joystick(PlayerTab.SelectedIndex) = ControllerCB.SelectedValue
        Catch ex As Exception
            MsgBox("Error Opening Controller " & ex.Message)

        End Try


    End Sub

    Private Sub ButtonClicked(ByVal _keycode As String, ByVal _down As Boolean)

        If Not ButtonsDown.ContainsKey(_keycode) Then

            If Currently_Binding IsNot Nothing And (_down Or _keycode = "k13") Then
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
        If MainformRef.IsNullDCRunning Or MainformRef.MednafenLauncher.MednafenInstance IsNot Nothing Then
            MsgBox("Cannot remap While emulation Is running.")
            Exit Sub
        End If

        If ControllerCB.SelectedIndex = 0 Then
            MsgBox("Select a CONTROLLER from the list first. For Keyboard just click a button On the right, When it turns red press a key On your keyboard.")
        Else
            frmSDLMappingTool.ShowDialog(Me)
        End If

    End Sub

    Private Sub FrmKeyMapperSDL_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.fan_icon_text
    End Sub

    Private Sub BtnResetAll_Click(sender As Object, e As EventArgs) Handles ResetAllToolStripMenuItem.Click
        If MainformRef.IsNullDCRunning Or MainformRef.MednafenLauncher.MednafenInstance IsNot Nothing Then
            MsgBox("Cannot reset all While emulation Is running.")
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
        If MainformRef.IsNullDCRunning Or MainformRef.MednafenLauncher.MednafenInstance IsNot Nothing Then
            MsgBox("Cannot edit mapping String While emulation Is running.")
            Exit Sub
        End If

        If ControllerCB.SelectedIndex = 0 Then
            MsgBox("Mapping Strings are For Controllers only, Select one from the list below.")
        Else
            frmMappingString.ShowDialog(Me)
        End If

    End Sub

    Private Sub ExportMappingStringToolStripMenuItem_Click(sender As Object, e As EventArgs)
        If Not My.Computer.Clipboard.ContainsText Then
            MsgBox("Copy mapping text Then Try again.")
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
            MsgBox("Unable to delete profile: " & ex.Message)

        End Try


    End Sub

    Private Sub frmKeyMapperSDL_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If SDL_WasInit(SDL_INIT_JOYSTICK) Or SDL_WasInit(SDL_INIT_GAMECONTROLLER) Then
            For i = 0 To SDL_NumJoysticks() - 1
                Console.WriteLine("Disconnecting: " & SDL_GameControllerNameForIndex(i))
                SDL_GameControllerClose(SDL_GameControllerFromInstanceID(i))
            Next
            SDL_QuitSubSystem(SDL_INIT_GAMECONTROLLER)
            SDL_QuitSubSystem(SDL_INIT_JOYSTICK)

            SDL_Quit()
        End If
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