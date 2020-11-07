Imports SDL2.SDL
Imports System.IO

Public Class frmSDLMappingTool

    Dim ListOfGamepadKeys As String() = {"a", "b", "x", "y", "leftshoulder", "rightshoulder",
        "lefttrigger", "righttrigger", "leftstick", "rightstick",
        "dpup", "dpdown", "dpleft", "dpright",
        "back", "start", "guide",
        "leftx", "lefty", "rightx", "righty"}
    Dim ListOfGamepadKeysProper As String() = {"A (Bottom Button)", "B (Right Button)", "X (Left Button)", "Y (Top Button)", "Left Shoulder(L1)", "Right Shoulder(R1)",
        "Left Trigger(L2)", "Right Trigger(R2)", "Left Stick(L3)", "Right Stick(R3)",
        "Digital Up (D-Pad or Digital Stick)", "Digital Down (D-Pad or Digital Stick)", "Digital Left (D-Pad or Digital Stick)", "Digital Right (D-Pad or Digital Stick)",
        "Back", "Start", "Guide/Logo Button",
        "Left Analog Stick Left or Right", "Left Analog Stick Up or Down", "Right Analog Stick Left or Right", "Right Analog Stick Up or Down"}

    Dim ControllerImages As Image() = {My.Resources.Controller_A, My.Resources.Controller_B, My.Resources.Controller_X, My.Resources.Controller_Y, My.Resources.Controller_LB, My.Resources.Controller_RB,
        My.Resources.Controller_LT, My.Resources.Controller_RT, My.Resources.Controller_LeftStick, My.Resources.Controller_RightStick,
        My.Resources.Controller_Dpad_Up, My.Resources.Controller_Dpad_Down, My.Resources.Controller_Dpad_Left, My.Resources.Controller_Dpad_Right,
        My.Resources.Controller_Back, My.Resources.Controller_Start, My.Resources.Controller_Guide,
        My.Resources.Controller_LeftX, My.Resources.Controller_LeftY, My.Resources.Controller_RightX, My.Resources.Controller_RightY}

    Dim ListOfBinds(20) As String
    Dim _InputThread As Threading.Thread
    Dim _currentBindIndex = 0
    Dim Joy
    Dim AxisDown As New Dictionary(Of String, Integer)
    Dim AxisIdle As New ArrayList
    Dim _event As SDL_Event
    Dim _deadzonetotal As Decimal
    Dim ImageTimer As Timer = New Timer

    Private Sub frmSDLMappingTool_VisibleChanged(sender As Object, e As EventArgs) Handles MyBase.VisibleChanged
        Me.Icon = My.Resources.NewNullDCBearIcon

        If Me.Visible Then
            Me.CenterToParent()

            If SDL_WasInit(SDL_INIT_JOYSTICK) = 0 Then
                SDL_Init(SDL_INIT_JOYSTICK)
                Console.WriteLine("SDL_INIT")
            End If

            If SDL_WasInit(SDL_INIT_GAMECONTROLLER) = 0 Then
                SDL_Init(SDL_INIT_GAMECONTROLLER)
                Console.WriteLine("SDL_INIT")
            End If

            If frmKeyMapperSDL.ControllerCB.SelectedValue >= 0 Then
                Joy = SDL_JoystickOpen(frmKeyMapperSDL.ControllerCB.SelectedValue)
                Console.WriteLine(Joy)
            Else
                Joy = Nothing
            End If

            If Joy Is Nothing Then
                MsgBox("Couldn't open Joystick")
                Exit Sub
            End If

            Me.Invoke(Sub() _deadzonetotal = 32768 * Decimal.Divide(frmKeyMapperSDL.DeadzoneTB.Value, 100))
            _deadzonetotal += 256

            UpdateHelpTest()

            AxisDown.Clear()
            AxisDown = New Dictionary(Of String, Integer)
            StartBinding()

            ' Get the Default state of the axis
            AxisIdle.Clear()
            For i = 0 To SDL_JoystickNumAxes(Joy)
                AxisIdle.Add(SDL_JoystickGetAxis(Joy, i))
            Next

        Else
            If Not _InputThread Is Nothing Then
                If _InputThread.IsAlive Then
                    _InputThread.Abort()
                End If
            End If
        End If

        ImageTimer.Interval = 500
        ImageTimer.Start()
        AddHandler ImageTimer.Tick, Sub()
                                        If PictureBox1.Image Is Nothing Then
                                            If _currentBindIndex < ControllerImages.Count Then
                                                PictureBox1.Image = ControllerImages(_currentBindIndex)
                                            End If

                                        Else
                                            PictureBox1.Image = Nothing
                                        End If

                                    End Sub

    End Sub

    Private Sub StartBinding()
        If Not _InputThread Is Nothing Then
            If _InputThread.IsAlive Then
                _InputThread.Abort()
            End If
        End If

        _currentBindIndex = 0
        ListOfBinds = New String(20) {}

        _InputThread = New Threading.Thread(AddressOf InputThread)
        _InputThread.IsBackground = True
        _InputThread.Start()
    End Sub

    Private Sub UpdateHelpTest()
        Dim ButtonText = "Buttons: "

        ButtonText += SDL_JoystickNumButtons(Joy).ToString & " ("
        For i = 0 To SDL_JoystickNumButtons(Joy) - 1
            ButtonText += SDL_JoystickGetButton(Joy, i).ToString
        Next
        ButtonText += ")"

        Dim AxisText = "Axis: "
        AxisText += SDL_JoystickNumAxes(Joy).ToString & " ("
        For i = 0 To SDL_JoystickNumAxes(Joy) - 1
            AxisText += SDL_JoystickGetAxis(Joy, i).ToString & " "

        Next
        AxisText += ") Deadzone: " & (32768 * Decimal.Divide(frmKeyMapperSDL.DeadzoneTB.Value, 100)).ToString & vbNewLine & "If Deadzone is lower than your axis, raise the deadzone."

        Dim HatText = "Hats: "
        HatText += SDL_JoystickNumHats(Joy).ToString & " ("
        For i = 0 To SDL_JoystickNumHats(Joy) - 1
            HatText += SDL_JoystickGetHat(Joy, i).ToString & " "
        Next
        HatText += ")"

        Me.Invoke(
            Sub()
                lbl_buttons.Text = ButtonText
                lbl_axis.Text = AxisText
                lbl_hats.Text = HatText
            End Sub)
    End Sub

    Private Sub InputThread()

        While _currentBindIndex < ListOfGamepadKeys.Count
            Dim KeyPressed As String = ""

            Me.Invoke(
                Sub()
                    If Not ListOfGamepadKeysProper.Count - 1 < _currentBindIndex Then
                        If Not ListOfGamepadKeysProper(_currentBindIndex) Is Nothing Then
                            Label1.Text = "Press " & ListOfGamepadKeysProper(_currentBindIndex) & vbNewLine & "ESCAPE to SKIP"
                        End If
                    End If
                End Sub)

            While SDL_PollEvent(_event)
                ' Write the capabilities
                UpdateHelpTest()

                Select Case _event.type
                    Case SDL_EventType.SDL_JOYAXISMOTION ' Axis Motion Down
                        'Console.WriteLine("Axis: " & _event.jaxis.axisValue)
                        'Console.WriteLine("Device: " & _event.jdevice.which)

                        Dim _axisnorm As Int32 = _event.jaxis.axisValue
                        _axisnorm = Math.Abs(_axisnorm)

                        ' _axisnorm > _deadzonetotal And 
                        If Not AxisDown.ContainsKey(_event.jaxis.axis) Then
                            'Console.WriteLine("Axis Down")
                            Dim IsThumbStick As Boolean = False

                            If ListOfGamepadKeys(_currentBindIndex) = "leftx" Or
                                ListOfGamepadKeys(_currentBindIndex) = "lefty" Or
                                ListOfGamepadKeys(_currentBindIndex) = "rightx" Or
                                ListOfGamepadKeys(_currentBindIndex) = "righty" Then
                                IsThumbStick = True
                            End If

                            If Not IsThumbStick Then
                                ' ok so triggers are tricky so going to need to have all these checks to account for all the types of triggers

                                ' Idle is Between Deadzone (0) and current is above deadzone (1)
                                If AxisIdle(_event.jaxis.axis) >= -_deadzonetotal And AxisIdle(_event.jaxis.axis) <= _deadzonetotal And _event.jaxis.axisValue >= _deadzonetotal Then ' 0 to 1
                                    KeyPressed = "+a" & _event.jaxis.axis
                                    ' Idle is Between Deadzone (0) and current is below deadzone (-1)
                                ElseIf AxisIdle(_event.jaxis.axis) >= -_deadzonetotal And AxisIdle(_event.jaxis.axis) <= _deadzonetotal And _event.jaxis.axisValue <= -_deadzonetotal Then ' 0 to -1
                                    KeyPressed = "-a" & _event.jaxis.axis
                                    ' Idle is above deadzone (1) and current is in deadzone (0)
                                ElseIf AxisIdle(_event.jaxis.axis) >= _deadzonetotal And _event.jaxis.axisValue <= _deadzonetotal And _event.jaxis.axisValue >= -_deadzonetotal Then ' 1 to 0
                                    KeyPressed = "+a" & _event.jaxis.axis & "~"
                                    ' Idle is below deadzone (-1) and current is in deadzone (0)
                                ElseIf AxisIdle(_event.jaxis.axis) <= -_deadzonetotal And _event.jaxis.axisValue <= _deadzonetotal And _event.jaxis.axisValue >= -_deadzonetotal Then ' -1 to 0
                                    KeyPressed = "-a" & _event.jaxis.axis & "~"
                                    ' Idle is below deadzone (-1) and current is above deadzone (1)
                                ElseIf AxisIdle(_event.jaxis.axis) <= -_deadzonetotal And _event.jaxis.axisValue >= _deadzonetotal Then ' -1 to 1 (Full Range)
                                    KeyPressed = "a" & _event.jaxis.axis
                                    ' Idle is above deadzone (1) and current is below deadzone (-1)
                                ElseIf AxisIdle(_event.jaxis.axis) >= _deadzonetotal And _event.jaxis.axisValue <= -_deadzonetotal Then ' 1 to -1 (Full Range)
                                    KeyPressed = "a" & _event.jaxis.axis & "~"
                                End If

                            Else
                                ' Is is a thumbstick so just use full range -1 to 1
                                If Not AxisDown.ContainsKey("a" & _event.jaxis.axis) Then
                                    If _event.jaxis.axisValue < 0 Then
                                        KeyPressed = "a" & _event.jaxis.axis & "~"
                                        AxisDown.Add("a" & _event.jaxis.axis, _event.jaxis.axisValue)
                                    Else
                                        KeyPressed = "a" & _event.jaxis.axis
                                    End If
                                End If

                            End If

                            If Not AxisDown.ContainsKey(KeyPressed) Then
                                AxisDown.Add(KeyPressed, _event.jaxis.axisValue)
                            Else
                                KeyPressed = ""
                            End If

                        End If

                        SDL_FlushEvents(1536, 1536)
                        Exit While

                    Case SDL_EventType.SDL_JOYHATMOTION  ' Hat Motion
                        If Not _event.jhat.hatValue = 0 Then
                            KeyPressed = "h" & CDec(_event.jhat.hatValue / 10).ToString
                        End If

                    Case SDL_EventType.SDL_JOYBUTTONDOWN  ' Button Down
                        Console.WriteLine(_event.jbutton.button)
                        KeyPressed = "b" & _event.jbutton.button

                End Select

            End While

            If KeyPressed.Length > 0 Then
                ListOfBinds(_currentBindIndex) = ListOfGamepadKeys(_currentBindIndex) & ":" & KeyPressed
                _currentBindIndex += 1
                If _currentBindIndex < ControllerImages.Count Then
                    Me.Invoke(Sub()
                                  PictureBox1.Image = ControllerImages(_currentBindIndex)
                                  ImageTimer.Enabled = False
                                  ImageTimer.Enabled = True
                                  ImageTimer.Start()
                              End Sub)


                End If
            End If

        End While

        Dim DeviceGUIDasString(40) As Byte


        Me.Invoke(
            Sub()
                SDL_JoystickGetGUIDString(SDL_JoystickGetDeviceGUID(frmKeyMapperSDL.ControllerCB.SelectedValue), DeviceGUIDasString, 40)
            End Sub)

        Dim GUIDSTRING As String = System.Text.Encoding.ASCII.GetString(DeviceGUIDasString).ToString.Replace(vbNullChar, "").Trim

        Dim ConfigString = ""
        For i = 0 To ListOfBinds.Count - 1
            If ListOfBinds(i).Length > 0 Then
                ConfigString += ListOfBinds(i) & ","
            End If
        Next

        Dim ConfigStringFinal = ""
        Me.Invoke(Sub()
                      ConfigStringFinal = GUIDSTRING & "," & SDL_JoystickNameForIndex(frmKeyMapperSDL.ControllerCB.SelectedValue) & "," & ConfigString & "platform:Windows,"
                      'My.Computer.Clipboard.SetText(ConfigStringFinal)
                      Dim customControllerConfigLines() As String

                      If File.Exists(MainformRef.NullDCPath & "\bearcontrollerdb.txt") Then
                          customControllerConfigLines = File.ReadAllLines(MainformRef.NullDCPath & "\bearcontrollerdb.txt")
                          Console.WriteLine("read file")
                      Else
                          customControllerConfigLines = {""}
                      End If

                      Dim FoundExistingEntry As Boolean = False
                      For i = 0 To customControllerConfigLines.Count - 1
                          If customControllerConfigLines(i).StartsWith(GUIDSTRING) Then ' Config for this controller was found already, override it
                              customControllerConfigLines(i) = ConfigStringFinal
                              FoundExistingEntry = True
                              Console.WriteLine("Found Existing Controller Entry")
                              Exit For
                          End If
                      Next

                      If Not FoundExistingEntry Then
                          File.AppendAllLines(MainformRef.NullDCPath & "\bearcontrollerdb.txt", {ConfigStringFinal})
                          File.AppendAllLines(MainformRef.NullDCPath & "\dc\bearcontrollerdb.txt", {ConfigStringFinal})
                      Else
                          File.WriteAllLines(MainformRef.NullDCPath & "\bearcontrollerdb.txt", customControllerConfigLines)
                          File.WriteAllLines(MainformRef.NullDCPath & "\dc\bearcontrollerdb.txt", customControllerConfigLines)
                      End If

                      SDL_GameControllerAddMappingsFromFile(MainformRef.NullDCPath & "\bearcontrollerdb.txt")

                      frmKeyMapperSDL.UpdateControllersList()
                      Label1.Text = "Aight, you're all set"

                  End Sub)

        SDL_GameControllerAddMapping(ConfigStringFinal)
        Console.WriteLine(ConfigStringFinal)

        Me.Invoke(Sub() Me.Close())
    End Sub

    Private Sub frmSDLMappingTool_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Escape Then
            If _currentBindIndex < ListOfGamepadKeys.Count Then
                ListOfBinds(_currentBindIndex) = ""
                _currentBindIndex += 1
                If _currentBindIndex < ControllerImages.Count Then
                    Me.Invoke(Sub()
                                  PictureBox1.Image = ControllerImages(_currentBindIndex)
                                  ImageTimer.Enabled = False
                                  ImageTimer.Enabled = True
                                  ImageTimer.Start()
                              End Sub)
                End If
            End If

        End If

    End Sub

    Private Sub frmSDLMappingTool_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub frmSDLMappingTool_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        Console.WriteLine("Closed")
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Close()
    End Sub

    Private Sub cbDeviceList_SelectedIndexChanged(sender As Object, e As EventArgs)
        For i = 0 To SDL_NumJoysticks() - 1
            If SDL_JoystickGetAttached(i) Then
                'SDL_JoystickClose(i)
                Joy = Nothing
                ImageTimer.Stop()
                ImageTimer.Dispose()
            End If
        Next
    End Sub

    Private Sub frmSDLMappingTool_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If Not Joy Is Nothing Then
            'SDL_JoystickClose(Joy)
            Joy = Nothing
        End If

    End Sub
End Class


Public Class SDLMapingPair
    Public JoystickKey
    Public GamepadKey

End Class