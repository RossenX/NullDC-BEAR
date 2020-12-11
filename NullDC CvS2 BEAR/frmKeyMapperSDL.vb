Imports System.Runtime.InteropServices
Imports SDL2
Imports SDL2.SDL
Imports System.IO

Public Class frmKeyMapperSDL

    ' From the Configs
    Dim Joystick(2) As Int16
    Dim Deadzone(2) As Int16
    Dim Peripheral(2) As Int16

    Public Joy As IntPtr
    Dim KeyConv As New KeysConverter
    Dim AvailableControllersList As New DataTable

    Dim _InputThread As Threading.Thread

    Dim Currently_Binding As keybindButton

    Dim ButtonsDown As New Dictionary(Of String, Boolean)

    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
    Private Shared Function PostMessage(ByVal hWnd As IntPtr, ByVal Msg As UInteger, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As Boolean
    End Function

    Private Sub frmKeyMapperSDL_Load(sender As Object, e As EventArgs) Handles MyBase.VisibleChanged
        If Me.Visible Then

            Me.CenterToParent()
            Me.Icon = My.Resources.NewNullDCBearIcon

            If SDL_WasInit(SDL_INIT_JOYSTICK) = 0 Then
                SDL_Init(SDL_INIT_JOYSTICK)
                Console.WriteLine("SDL_INIT JOYSTICK")
            End If

            If SDL_WasInit(SDL_INIT_GAMECONTROLLER) = 0 Then
                SDL_Init(SDL_INIT_GAMECONTROLLER)
                Console.WriteLine("SDL_INIT GAME CONTROLLER")
            End If

            DoInitialSetupShit()
            LoadSettings()

            _InputThread = New Threading.Thread(AddressOf InputThread)
            _InputThread.IsBackground = True
            _InputThread.Start()

            AddHandler DeadzoneTB.MouseCaptureChanged, AddressOf DeadzoneTB_MouseCaptureChanged
            AddHandler DeadzoneTB.ValueChanged, Sub() Deadzonetext.Text = "Deadzone: " & DeadzoneTB.Value
            AddHandler PeripheralCB.SelectedIndexChanged, AddressOf PeripheralCB_SelectedIndexChanged
            AddHandler PlayerTab.SelectedIndexChanged, AddressOf PlayerTab_SelectedIndexChanged
            AddHandler ControllersTab.SelectedIndexChanged, Sub() ActiveControl = Nothing
            AddHandler cbSDL.SelectedIndexChanged, AddressOf SDLVersionChanged

            If MainformRef.IsNullDCRunning Then
                PeripheralCB.Enabled = False
                PeriWarning.Visible = True
                cbSDL.Enabled = False
            Else
                PeripheralCB.Enabled = True
                PeriWarning.Visible = False
                cbSDL.Enabled = True
            End If

        End If


    End Sub

    Private Sub SDLVersionChanged(sender As ComboBox, e As EventArgs)
        If Not cbSDL.SelectedItem = MainformRef.ConfigFile.SDLVersion Then
            MainformRef.ConfigFile.SDLVersion = "+" & sender.SelectedItem
            MainformRef.ConfigFile.SaveFile(False)
            If MsgBox("Restart Required") = MsgBoxResult.Ok Then End
        End If

    End Sub

    Private Sub DoInitialSetupShit()

        AvailableControllersList.Columns.Add("ID")
        AvailableControllersList.Columns.Add("Name")

        'AvailableControllersList.Rows.Add({-1, "Keyboard Only"})

        ControllerCB.ValueMember = "ID"
        ControllerCB.DisplayMember = "Name"

        ' Flush Initial Connection Events
        SDL_FlushEvents(SDL_EventType.SDL_CONTROLLERDEVICEADDED, SDL_EventType.SDL_CONTROLLERDEVICEREMOVED)
        UpdateControllersList()

    End Sub

    Private Sub GenerateDefaults()

        Joystick(0) = 0
        Joystick(1) = -1
        Deadzone(0) = 15
        Deadzone(1) = 15
        DeadzoneTB.Value = 15
        Peripheral(0) = 0
        Peripheral(1) = 0

        I_BTN5_KEY.KC = {"a4+", "k79"}
        I_BTN4_KEY.KC = {"b1", "k73"}
        I_BTN3_KEY.KC = {"b0", "k85"}
        I_BTN2_KEY.KC = {"a5+", "k48"}
        I_BTN1_KEY.KC = {"b3", "k57"}
        I_BTN0_KEY.KC = {"b2", "k56"}
        I_TEST_KEY_1.KC = {"k116", "k116"}
        I_SERVICE_KEY_1.KC = {"k115", "k115"}
        I_COIN_KEY.KC = {"b4", "k49"}
        I_START_KEY.KC = {"b6", "k53"}
        I_RIGHT_KEY.KC = {"a0+", "k68"}
        I_LEFT_KEY.KC = {"a0-", "k65"}
        I_DOWN_KEY.KC = {"a1+", "k83"}
        I_UP_KEY.KC = {"a1-", "k87"}
        CONT_A.KC = {"b0", "k85"}
        CONT_B.KC = {"b1", "k73"}
        CONT_Y.KC = {"b3", "k57"}
        CONT_X.KC = {"b2", "k56"}
        CONT_START.KC = {"b6", "k53"}
        CONT_RSLIDER.KC = {"a5+", "k79"}
        CONT_LSLIDER.KC = {"a4+", "k48"}
        CONT_ANALOG_DOWN.KC = {"a1+", "k83"}
        CONT_ANALOG_RIGHT.KC = {"a0+", "k68"}
        CONT_ANALOG_UP.KC = {"a1-", "k87"}
        CONT_ANALOG_LEFT.KC = {"a0-", "k65"}
        CONT_DPAD_DOWN.KC = {"b12", "k71"}
        CONT_DPAD_RIGHT.KC = {"b14", "k72"}
        CONT_DPAD_UP.KC = {"b11", "k84"}
        CONT_DPAD_LEFT.KC = {"b13", "k70"}
        STICK_C.KC = {"a5+", "k79"}
        STICK_B.KC = {"b1", "k73"}
        STICK_A.KC = {"b0", "k85"}
        STICK_Z.KC = {"a4+", "k48"}
        STICK_Y.KC = {"b3", "k57"}
        STICK_X.KC = {"b2", "k56"}
        STICK_START.KC = {"b6", "k53"}
        STICK_DPAD_RIGHT.KC = {"a0+", "k68"}
        STICK_DPAD_LEFT.KC = {"a0-", "k65"}
        STICK_DPAD_DOWN.KC = {"a1+", "k83"}
        STICK_DPAD_UP.KC = {"a1-", "k87"}

        SaveSettings()

    End Sub

    Private Sub UpdateButtonLabels()
        For Each _tab As TabPage In ControllersTab.TabPages
            For Each _cont As Control In _tab.Controls

                If TypeOf _cont Is keybindButton Then
                    Dim _btn As keybindButton = _cont
                    _btn.UpdateTextFromKeycode(PlayerTab.SelectedIndex)

                End If

            Next
        Next

    End Sub

    Private Sub SaveSettings()
        Dim lines(128) As String
        ' BPortA_I_
        ' BPortA_CONT_
        ' get GUIDs
        Dim DeviceGUIDasString1(40) As Byte
        Dim DeviceGUIDasString2(40) As Byte

        SDL_JoystickGetGUIDString(SDL_JoystickGetDeviceGUID(Joystick(0)), DeviceGUIDasString1, 40)
        SDL_JoystickGetGUIDString(SDL_JoystickGetDeviceGUID(Joystick(1)), DeviceGUIDasString2, 40)

        Dim Joy1GUID = System.Text.Encoding.ASCII.GetString(DeviceGUIDasString1).ToString.Replace(vbNullChar, "").Trim
        Dim Joy2GUID = System.Text.Encoding.ASCII.GetString(DeviceGUIDasString2).ToString.Replace(vbNullChar, "").Trim

        lines(0) = MainformRef.Ver
        lines(1) = "Joystick=" & Joystick(0) & "|" & Joystick(1)
        lines(2) = "Deadzone=" & Deadzone(0) & "|" & Deadzone(1)
        lines(3) = "Peripheral=" & Peripheral(0) & "|" & Peripheral(1)

        Dim KeyCount = 4
        For Each _tab As TabPage In ControllersTab.TabPages
            For Each _cont As Control In _tab.Controls
                If TypeOf _cont Is keybindButton Then
                    Dim _btn As keybindButton = _cont
                    lines(KeyCount) = _btn.Name & "=" & _btn.KC(0) & "|" & _btn.KC(1)
                    KeyCount += 1
                End If

            Next
        Next

        File.WriteAllLines(MainformRef.NullDCPath & "\Controls.bear", lines)

    End Sub

    Private Sub LoadSettings()

        Dim configLines As String()

        If Not File.Exists(MainformRef.NullDCPath & "\Controls.bear") Then
            GenerateDefaults()
        End If

        configLines = File.ReadAllLines(MainformRef.NullDCPath & "\Controls.bear")

        RemoveHandler ControllerCB.SelectedIndexChanged, AddressOf ControllerCB_SelectedIndexChanged

        For Each line In configLines
            If line.StartsWith("Joystick") Then

                Joystick(0) = Convert.ToInt16(line.Split("=")(1).Split("|")(0))
                Joystick(1) = Convert.ToInt16(line.Split("=")(1).Split("|")(1))

                If ControllerCB.Items.Count > (Joystick(PlayerTab.SelectedIndex) + 1) Then
                    ControllerCB.SelectedIndex = Joystick(PlayerTab.SelectedIndex) + 1
                Else
                    ControllerCB.SelectedIndex = 0
                    Joystick(PlayerTab.SelectedIndex) = -1
                End If

            End If

            If line.StartsWith("Deadzone") Then
                Deadzone(0) = Convert.ToInt16(line.Split("=")(1).Split("|")(0))
                Deadzone(1) = Convert.ToInt16(line.Split("=")(1).Split("|")(1))
                DeadzoneTB.Value = Deadzone(PlayerTab.SelectedIndex)
                Deadzonetext.Text = "Deadzone: " & DeadzoneTB.Value

            End If

            If line.StartsWith("Peripheral") Then
                Peripheral(0) = Convert.ToInt16(line.Split("=")(1).Split("|")(0))
                Peripheral(1) = Convert.ToInt16(line.Split("=")(1).Split("|")(1))
                PeripheralCB.SelectedIndex = Peripheral(PlayerTab.SelectedIndex)
            End If

        Next

        AddHandler ControllerCB.SelectedIndexChanged, AddressOf ControllerCB_SelectedIndexChanged

        For Each _tab As TabPage In ControllersTab.TabPages
            For Each _cont As Control In _tab.Controls
                If TypeOf _cont Is keybindButton Then
                    Dim _btn As keybindButton = _cont

                    GetKeyCodeFromFile(_btn, configLines)

                    AddHandler _btn.Click, AddressOf ClickedBindButton
                    _btn.UpdateTextFromKeycode(PlayerTab.SelectedIndex)
                End If
            Next
        Next

        If ControllerCB.SelectedIndex > -1 Then
            Joy = SDL_GameControllerOpen(ControllerCB.SelectedValue)
            Console.WriteLine(Joy)
        End If

        cbSDL.SelectedItem = MainformRef.ConfigFile.SDLVersion

    End Sub

    Private Sub GetKeyCodeFromFile(ByRef _btn As keybindButton, ByRef _configlines As String())
        For Each line As String In _configlines
            If line.StartsWith(_btn.Name & "=") Then
                _btn.KC(0) = line.Split("=")(1).Split("|")(0)
                _btn.KC(1) = line.Split("=")(1).Split("|")(1)
                If _btn.KC(0) = "" Then _btn.KC(0) = "k0"
                If _btn.KC(1) = "" Then _btn.KC(1) = "k0"

            End If
        Next
    End Sub

    Private Sub InputThread()
        While _InputThread.IsAlive

            Try
                If Application.OpenForms().OfType(Of frmSDLMappingTool).Any Then
                    SDL_Delay(100)
                    Continue While
                End If

            Catch ex As Exception
                Console.WriteLine("yup")
            End Try

            Dim _event As SDL_Event
            While SDL_PollEvent(_event)
                'Console.WriteLine(_event.type)
                Select Case _event.type
                    Case 1616 ' Axis Motion
                        Console.WriteLine("Axis Motion: " & _event.caxis.axis & "|" & _event.caxis.axisValue)
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


                    Case 1617 ' Button Down
                        'Console.WriteLine("Button Down: " & _event.cbutton.button & " | " & SDL_GameControllerNameForIndex(_event.cdevice.which))
                        Me.Invoke(Sub()
                                      ButtonClicked("b" & _event.cbutton.button, True)
                                  End Sub)

                    Case 1618 ' Button Up
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
                    Case 1539
                        'MsgBox(_event.jbutton.button)
                End Select

            End While

            _event = Nothing
        End While

    End Sub

    Public Sub UpdateControllersList()
        RemoveHandler ControllerCB.SelectedIndexChanged, AddressOf ControllerCB_SelectedIndexChanged
        SDL_GameControllerAddMappingsFromFile(MainformRef.NullDCPath & "\gamecontrollerdb.txt")
        SDL_GameControllerAddMappingsFromFile(MainformRef.NullDCPath & "\bearcontrollerdb.txt")

        Dim OldConnectedIndex = ControllerCB.SelectedValue
        Dim FoundController As Boolean = False
        AvailableControllersList.Rows.Clear()
        AvailableControllersList.Rows.Add({-1, "Keyboard Only"})

        For i = 0 To SDL_NumJoysticks() - 1

            AvailableControllersList.Rows.Add({i, "(" & i & ") " & SDL_JoystickNameForIndex(i)})

            If i = OldConnectedIndex Then
                FoundController = True
            End If

            'Console.WriteLine("Added: " & i & " | " & SDL_GameControllerNameForIndex(i))
        Next

        ControllerCB.DataSource = New DataView(AvailableControllersList)

        If FoundController Then
            ControllerCB.SelectedIndex = OldConnectedIndex + 1
        Else
            ControllerCB.SelectedIndex = 0
        End If

        AddHandler ControllerCB.SelectedIndexChanged, AddressOf ControllerCB_SelectedIndexChanged
    End Sub

    Private Sub Button34_Click(sender As Object, e As EventArgs)
        Me.Close()

    End Sub

    Private Sub frmKeyMapperSDL_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        SaveSettings()
        ' Disable SDL completly we dun need that shit in the background no more
        For i = 0 To SDL_NumJoysticks() - 1
            If SDL_GameControllerGetAttached(SDL_GameControllerFromInstanceID(i)) = SDL_bool.SDL_TRUE Then
                Console.WriteLine("Disconnecting: " & SDL_GameControllerNameForIndex(i))
                SDL_GameControllerClose(SDL_GameControllerFromInstanceID(i))

            End If
        Next

        _InputThread.Abort()
        SDL_Quit()

        ' Update the configs for hotloading new configs
        If MainformRef.IsNullDCRunning Then
            Select Case Rx.platform
                Case "na"

                    Dim ControlsFile() As String = File.ReadAllLines(MainformRef.NullDCPath & "\Controls.bear")
                    Dim linenumber = 0
                    For Each line In ControlsFile
                        If line.StartsWith("CONT_") Or line.StartsWith("STICK_") Then
                            ControlsFile(linenumber) = ""
                        End If
                        linenumber += 1
                    Next


                    Dim lines() As String = File.ReadAllLines(MainformRef.NullDCPath & "\nullDC.cfg")
                    linenumber = 0
                    For Each line As String In lines
                        If Not ControlsFile Is Nothing Then
                            If line.StartsWith("BPort") Then
                                Dim player = 0
                                If line.StartsWith("BPortB") Then player = 1

                                Dim KeyCount = False
                                For Each control_line As String In ControlsFile
                                    If line.Contains(control_line.Split("=")(0) & "=") And control_line.Length > 0 Then
                                        lines(linenumber) = line.Split("=")(0) & "=" & control_line.Split("=")(1).Split("|")(player)
                                        KeyCount = True
                                        Exit For
                                    End If
                                Next

                                If Not KeyCount Then lines(linenumber) = line.Split("=")(0) & "=k0"

                            End If
                        End If
                        linenumber += 1
                    Next

                    File.SetAttributes(MainformRef.NullDCPath & "\nullDC.cfg", FileAttributes.Normal)
                    File.WriteAllLines(MainformRef.NullDCPath & "\nullDC.cfg", lines)

                    PostMessage(MainformRef.NullDCLauncher.NullDCproc.MainWindowHandle, &H111, 141, 0)

                Case "dc"

                    ' Controls File
                    Dim ControlsFile() As String = File.ReadAllLines(MainformRef.NullDCPath & "\Controls.bear")
                    ' Get Rid of the settings we don't want

                    Dim tempPeripheral As String() = {"", ""}
                    Dim tempJoystick As String() = {"", ""}
                    Dim TempDeadzone As String() = {"", ""}

                    For Each line As String In ControlsFile
                        If line.StartsWith("Peripheral=") Then
                            tempPeripheral(0) = line.Split("=")(1).Split("|")(0)
                            tempPeripheral(1) = line.Split("=")(1).Split("|")(1)
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

                    ' General Settings
                    Dim lines() As String = File.ReadAllLines(MainformRef.NullDCPath & "\dc\nullDC.cfg")
                    Dim linenumber = 0
                    For Each line As String In lines
                        ' Controls DREAMCAST

                        If Not ControlsFile Is Nothing Then
                            If line.StartsWith("BPort") Then
                                Dim player = 0
                                If line.StartsWith("BPortB") Then player = 1

                                Dim KeyFound = False
                                For Each control_line As String In ControlsFile
                                    If tempPeripheral(player) = "1" Then
                                        If control_line.StartsWith("STICK_") Then
                                            If line.Contains(control_line.Split("=")(0).Replace("STICK_", "CONT_") & "=") And control_line.Length > 0 Then
                                                lines(linenumber) = line.Split("=")(0) & "=" & control_line.Split("=")(1).Split("|")(player)
                                                KeyFound = True
                                                Exit For
                                            End If
                                        End If

                                    Else

                                        If line.Contains(control_line.Split("=")(0) & "=") And control_line.Length > 0 Then
                                            lines(linenumber) = line.Split("=")(0) & "=" & control_line.Split("=")(1).Split("|")(player)
                                            KeyFound = True
                                            Exit For
                                        End If

                                    End If


                                Next

                                If Not KeyFound Then lines(linenumber) = line.Split("=")(0) & "=k0"

                            End If
                        End If

                        If line.StartsWith("BPortA_Joystick=") Then lines(linenumber) = "BPortA_Joystick=" & tempJoystick(0)
                        If line.StartsWith("BPortB_Joystick=") Then lines(linenumber) = "BPortB_Joystick=" & tempJoystick(1)

                        If line.StartsWith("BPortA_Deadzone=") Then lines(linenumber) = "BPortA_Deadzone=" & TempDeadzone(0)
                        If line.StartsWith("BPortB_Deadzone=") Then lines(linenumber) = "BPortB_Deadzone=" & TempDeadzone(1)

                        ' Arcadestick sync here
                        linenumber += 1
                    Next

                    File.SetAttributes(MainformRef.NullDCPath & "\dc\nullDC.cfg", FileAttributes.Normal)
                    File.WriteAllLines(MainformRef.NullDCPath & "\dc\nullDC.cfg", lines)

                    PostMessage(MainformRef.NullDCLauncher.NullDCproc.MainWindowHandle, &H111, 144, 0) ' 180 144

            End Select

        End If

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
        SaveSettings()

    End Sub

    Private Sub ClickedBindButton(sender As Button, e As EventArgs)
        'Console.WriteLine("Bind clicked: " & sender.Name)
        If Not Currently_Binding Is Nothing Then
            Currently_Binding.BackColor = Color.White
            Currently_Binding = Nothing
        End If

        Currently_Binding = sender
        Currently_Binding.BackColor = Color.Red
        ActiveControl = Nothing

    End Sub

    Private Sub PlayerTab_SelectedIndexChanged(sender As Object, e As EventArgs)
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
        PeripheralCB.SelectedIndex = Peripheral(PlayerTab.SelectedIndex)

        If Not Joystick(PlayerTab.SelectedIndex) = -1 Then
            Joy = SDL_GameControllerOpen(Joystick(PlayerTab.SelectedIndex))
        End If

        AddHandler ControllerCB.SelectedIndexChanged, AddressOf ControllerCB_SelectedIndexChanged
        AddHandler DeadzoneTB.MouseCaptureChanged, AddressOf DeadzoneTB_MouseCaptureChanged
        AddHandler PeripheralCB.SelectedIndexChanged, AddressOf PeripheralCB_SelectedIndexChanged

        ActiveControl = Nothing
        UpdateButtonLabels()

    End Sub

    Private Sub btn_Close_Click(sender As Object, e As EventArgs) Handles btn_Close.Click
        Me.Close()

    End Sub

    Private Sub ControllerCB_SelectedIndexChanged(sender As Object, e As EventArgs)

        'Console.Write("Changing Controller: ")
        If Not Joy = Nothing Then
            If SDL_GameControllerGetAttached(Joy) = SDL_bool.SDL_TRUE Then
                SDL_GameControllerClose(Joy)
            End If
            Joy = Nothing
        End If

        If ControllerCB.SelectedValue >= 0 Then
            'Console.Write(ControllerCB.SelectedValue & " | ")
            Joy = SDL_GameControllerOpen(ControllerCB.SelectedValue)
            'Console.WriteLine(SDL_GameControllerName(Joy))
        Else
            'Console.WriteLine("Keyboard")
        End If

        'Console.WriteLine("joystick: " & ControllerCB.SelectedValue)
        Joystick(PlayerTab.SelectedIndex) = ControllerCB.SelectedValue

        SaveSettings()
    End Sub

    Private Sub ButtonClicked(ByVal _keycode As String, ByVal _down As Boolean)
        If Not ButtonsDown.ContainsKey(_keycode) Then
            If Not Currently_Binding Is Nothing And (_down Or _keycode = "k13") Then
                Currently_Binding.ChangeKeyCode(_keycode, PlayerTab.SelectedIndex)
                Currently_Binding.BackColor = Color.White
                Currently_Binding = Nothing
                ActiveControl = Nothing
                SaveSettings()
            End If

            ' rn all this does is highlight
            For Each _btns As Control In ControllersTab.SelectedTab.Controls
                If TypeOf _btns Is keybindButton Then
                    Dim _btn As keybindButton = _btns
                    'Console.WriteLine("Pressed: " & _keycode & " | " & _btn.KC(PlayerTab.SelectedIndex))
                    If _btn.KC(PlayerTab.SelectedIndex) = _keycode Then
                        If _down Then
                            _btn.BackColor = Color.Green

                        Else
                            _btn.BackColor = Color.White

                        End If
                    End If
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

        SaveSettings()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles btnSDL.Click
        If ControllerCB.SelectedIndex = 0 Then
            MsgBox("Select a CONTROLLER from the list first. For Keyboard just click a button on the right, when it turns red press a key on your keyboard.")
        Else
            frmSDLMappingTool.ShowDialog(Me)
        End If

    End Sub

    Private Sub frmKeyMapperSDL_Load_1(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.NewNullDCBearIcon

    End Sub

    Private Sub btnResetAll_Click(sender As Object, e As EventArgs) Handles btnResetAll.Click
        Dim result1 As DialogResult = MessageBox.Show("This will Reset ALL the controls, k?", "Reset All?", MessageBoxButtons.YesNo)

        If result1 = DialogResult.Yes Then
            GenerateDefaults()
            LoadSettings()
            If File.Exists(MainformRef.NullDCPath & "\bearcontrollerdb.txt") Then File.Delete(MainformRef.NullDCPath & "\bearcontrollerdb.txt")
            UpdateControllersList()
            Me.Close()

        End If

    End Sub
End Class

Public Class KeyBind
    Public KC As String
    Public Name As String
    Public Button As Button
    Public Platform As String

    Dim KeyConv As New KeysConverter

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