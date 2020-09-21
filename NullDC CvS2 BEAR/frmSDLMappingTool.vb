Imports SDL2.SDL
Imports System.IO

Public Class frmSDLMappingTool

    Dim ListOfGamepadKeys As String() = {"a", "b", "x", "y", "leftshoulder", "rightshoulder",
        "lefttrigger", "righttrigger", "leftstick", "rightstick",
        "dpup", "dpdown", "dpleft", "dpright",
        "leftx", "lefty", "rightx", "righty",
        "back", "start", "guide"}
    Dim ListOfGamepadKeysProper As String() = {"A", "B", "X", "Y", "Left Shoulder(L1)", "Right Shoulder(R1)",
        "Left Trigger(L2)", "Right Trigger(R2)", "Left Stick(L3)", "Right Stick(R3)",
        "Digital Up (D-Pad or Digital Stick)", "Digital Down (D-Pad or Digital Stick)", "Digital Left (D-Pad or Digital Stick)", "Digital Right (D-Pad or Digital Stick)",
        "Left Analog Stick Left or Right", "Left Analog Stick Up or Down", "Right Analog Stick Left or Right", "Right Analog Stick Up or Down",
        "Back", "Start", "Guide/Logo Button"}

    Dim ListOfBinds(20) As String
    Dim _InputThread As Threading.Thread
    Dim AxisDown As Boolean = False
    Dim _currentBindIndex = 0

    Dim Joy

    Private Sub frmSDLMappingTool_VisibleChanged(sender As Object, e As EventArgs) Handles MyBase.VisibleChanged
        If Me.Visible Then
            Me.CenterToParent()

            If SDL_WasInit(SDL_INIT_GAMECONTROLLER) = 0 Then
                SDL_Init(SDL_INIT_GAMECONTROLLER)
                Console.WriteLine("SDL_INIT")
            End If

            If frmKeyMapperSDL.ControllerCB.SelectedValue >= 0 Then
                Joy = SDL_JoystickOpen(frmKeyMapperSDL.ControllerCB.SelectedValue)
            Else
                Joy = Nothing
            End If

            StartBinding()
        Else
            If Not _InputThread Is Nothing Then
                If _InputThread.IsAlive Then
                    _InputThread.Abort()
                End If
            End If
        End If


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

    Private Sub InputThread()
        While _currentBindIndex < ListOfGamepadKeys.Count
            Dim KeyPressed As String = ""

            Me.Invoke(Sub()
                          If Not ListOfGamepadKeysProper.Count - 1 < _currentBindIndex Then
                              If Not ListOfGamepadKeysProper(_currentBindIndex) Is Nothing Then
                                  Label1.Text = "Press " & ListOfGamepadKeysProper(_currentBindIndex) & vbNewLine & "ESCAPE to SKIP"
                              End If
                          End If
                      End Sub)

            Dim _event As SDL_Event
            While SDL_PollEvent(_event)
                Select Case _event.type
                    Case 1536 ' Axis Motion Down
                        If Math.Abs(_event.jaxis.axisValue - 1) > 16380 And Not AxisDown Then ' Bout 50% Deadzone should be safe for most stuff
                            AxisDown = True
                            Console.WriteLine("Axis Down")
                            Dim prefix As Boolean = True

                            If ListOfGamepadKeys(_currentBindIndex) = "rightx" Or
                                    ListOfGamepadKeys(_currentBindIndex) = "righty" Or
                                    ListOfGamepadKeys(_currentBindIndex) = "leftx" Or
                                    ListOfGamepadKeys(_currentBindIndex) = "lefty" Then
                                prefix = False
                            End If

                            If _event.jaxis.axisValue > 0 Then
                                If Not prefix Then
                                    KeyPressed = "a" & _event.jaxis.axis
                                Else
                                    KeyPressed = "+a" & _event.jaxis.axis
                                End If
                            Else
                                If Not prefix Then
                                    KeyPressed = "a" & _event.jaxis.axis
                                Else
                                    KeyPressed = "-a" & _event.jaxis.axis
                                End If
                            End If

                        Else
                            If Math.Abs(_event.jaxis.axisValue - 1) <= 16380 Then
                                AxisDown = False
                                Console.WriteLine("Axis Up")
                            End If

                        End If

                    Case 1538 ' Hat Motion
                        If Not _event.jhat.hatValue = 0 Then
                            KeyPressed = "h" & (_event.jhat.hatValue / 10)
                        End If
                    Case 1539 ' Button Down
                        KeyPressed = "b" & _event.jbutton.button
                End Select

            End While

            If KeyPressed.Length > 0 Then
                ListOfBinds(_currentBindIndex) = ListOfGamepadKeys(_currentBindIndex) & ":" & KeyPressed
                _currentBindIndex += 1
            End If

        End While

        Dim DeviceGUIDasString(40) As Byte

        Me.Invoke(Sub()
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
                SDL_JoystickClose(i)
            End If
        Next
    End Sub

    Private Sub frmSDLMappingTool_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If Not Joy Is Nothing Then
            SDL_JoystickClose(Joy)
            Joy = Nothing
        End If

    End Sub
End Class


Public Class SDLMapingPair
    Public JoystickKey
    Public GamepadKey

End Class