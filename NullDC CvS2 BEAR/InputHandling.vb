Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.Timers
Imports System.Xml
Imports XInputDotNetPure
Imports OpenTK
Imports OpenTK.Input
Imports System.IO

Public Class InputHandling

    Public ProfileName As String = "Default"
    Public KeyBoardConfigs As New Dictionary(Of String, String)
    Public PoV As Decimal = 0 ' PoV / Dpad
    Public PoVRest As Decimal = 0 ' PoV / Dpad
    Public DeadZone As Int16 = 10
    Public NeedConfigReload As Boolean = False
    Public KeyCache As New Dictionary(Of String, Array) ' Keeps data for the key presses last frame to avoid redoing the same thing over and over
    Public RxAxis As New Dictionary(Of String, Array) ' Current, Idle, Min, Max, Last Frame
    Public PollThread As Thread
    Public myjoyEX As JOYINFOEX
    Public Event _KeyPressed(ByVal Key As String)
    Public Event _KeyReleased(ByVal Key As String)
    Public KeybindConfigs As ArrayList = New ArrayList 'Key Configs from XML
    Dim kc As KeysConverter = New KeysConverter
    Public ControllerID As Int16 = 0
    Dim TurnedOn As Boolean = True
    Dim PollRate As Int16 = 16
    Dim RxButtons As New BitArray(32, False)
    Dim LF_RxButtons As New BitArray(32, False)
    Dim MainFormRef As frmMain 'Rebind Vars
    Dim CoinKeyDown As Boolean = False

    ' Virtual Keyboard Mapping
    Private Declare Function joyGetPosEx Lib "winmm.dll" (ByVal uJoyID As Integer, ByRef pji As JOYINFOEX) As Integer ' JoyStick Mapping
    Private Declare Sub keybd_event Lib "user32.dll" (ByVal bVk As Byte, ByVal bScan As Byte, ByVal dwFlags As Integer, ByVal dwExtraInfo As Integer)
    <DllImport("User32.dll", SetLastError:=False, CallingConvention:=CallingConvention.StdCall,
           CharSet:=CharSet.Auto)>
    Public Shared Function MapVirtualKey(ByVal uCode As UInt32, ByVal uMapType As UInt32) As UInt32
    End Function

    <DllImport("user32.dll", EntryPoint:="VkKeyScanExW")>
    Public Shared Function VkKeyScanExW(ByVal ch As Char, ByVal dwhkl As IntPtr) As Short
    End Function

    <StructLayout(LayoutKind.Sequential)>
    Public Structure JOYINFOEX
        Public dwSize As Integer
        Public dwFlags As Integer
        Public dwXpos As Integer
        Public dwYpos As Integer
        Public dwZpos As Integer
        Public dwTpos As Integer
        Public dwUpos As Integer
        Public dwVpos As Integer
        Public dwButtons As Integer
        Public dwButtonNumber As Integer
        Public dwPOV As Integer
        Public dwReserved1 As Integer
        Public dwReserved2 As Integer
    End Structure

    Public Sub New(ByRef mf As frmMain)
        MainFormRef = mf
        myjoyEX.dwSize = 64
        myjoyEX.dwFlags = &HFF

        GetKeyboardConfigs()
        ReloadConfigs()

        TurnedOn = MainFormRef.ConfigFile.UseRemap

        PollThread = New Thread(AddressOf InputRoll)
        PollThread.IsBackground = True
        PollThread.Start()
    End Sub

    Public Sub TurnOnOff(ByVal OnOff As Boolean)
        If OnOff Then
            TurnedOn = True
        Else
            TurnedOn = False
        End If
    End Sub

    Public Function GetXMLFile(Optional ByVal FullPath As Boolean = False) As String
        Dim _path = ""
        If FullPath Then _path = MainFormRef.NullDCPath & "\"

        If ProfileName = "Default" Then
            _path += "KeyMapReBinds.xml"
        Else
            _path += "KeyMapReBinds_" & ProfileName & ".xml"
        End If

        Return _path
    End Function

    Public Sub GetKeyboardConfigs()

        KeyBoardConfigs.Clear()
        Dim KeyboardLines() As String = File.ReadAllLines(MainFormRef.NullDCPath & "\qkoJAMMA\Keyboard.qkc")
        Dim KeyList() As String = {"Start", "Test", "Up", "Down", "Left", "Right", "Button_1", "Button_2", "Button_3", "Button_4", "Button_5", "Button_6", "Coin"}
        Dim KeyListx() As String = {"start", "Test", "up", "down", "left", "right", "LP", "MP", "HP", "LK", "MK", "HK", "coin"}

        'Check if any of them are blank
        Dim ReCreateKeyboardqkc As Boolean = False
        Dim FoundKeys() As Boolean = {False, False, False, False, False, False, False, False, False, False, False, False, False}
        Dim FoundAllKeys As Boolean = False
        For Each line In KeyboardLines
            If line.Split("=").Count = 0 Then Continue For
            Dim CurrentKeyChecking = line.Split("=")(1)
            If Array.IndexOf(KeyList, CurrentKeyChecking) = -1 Then ' If we don't find the keyboard button somehow then create it
                File.WriteAllLines(MainFormRef.NullDCPath & "\qkoJAMMA\Keyboard.qkc", {"5=Start", "3=Test", "w=Up", "s=Down", "a=Left", "d=Right", "8=Button_1", "9=Button_2", "0=Button_3", "u=Button_4", "i=Button_5", "o=Button_6", "1=Coin"})
                KeyboardLines = File.ReadAllLines(MainFormRef.NullDCPath & "\qkoJAMMA\Keyboard.qkc")
                FoundAllKeys = True
                Exit For
            Else
                FoundKeys(Array.IndexOf(KeyList, CurrentKeyChecking)) = True
            End If

        Next

        If Not FoundAllKeys Then
            For Each foundkey In FoundKeys ' Second Check if we dind't find ALL the keys
                If Not foundkey Then
                    File.WriteAllLines(MainFormRef.NullDCPath & "\qkoJAMMA\Keyboard.qkc", {"5=Start", "3=Test", "w=Up", "s=Down", "a=Left", "d=Right", "8=Button_1", "9=Button_2", "0=Button_3", "u=Button_4", "i=Button_5", "o=Button_6", "1=Coin"})
                    KeyboardLines = File.ReadAllLines(MainFormRef.NullDCPath & "\qkoJAMMA\Keyboard.qkc")
                    Exit For
                End If
            Next

        End If

        For Each line As String In KeyboardLines
            If line.Contains("Test") Then Continue For
            Dim SplitKeyEntry = line.Split("=")
            Dim KeyIndex = Array.IndexOf(KeyList, SplitKeyEntry(1))
            KeyBoardConfigs.Add(KeyListx(KeyIndex), SplitKeyEntry(0))

        Next

        KeyBoardConfigs.Add("LPLK", KeyBoardConfigs("LP") & KeyBoardConfigs("LK"))
        KeyBoardConfigs.Add("MPMK", KeyBoardConfigs("MP") & KeyBoardConfigs("MK"))
        KeyBoardConfigs.Add("HPHK", KeyBoardConfigs("HP") & KeyBoardConfigs("HK"))
        KeyBoardConfigs.Add("AP", KeyBoardConfigs("LP") & KeyBoardConfigs("MP") & KeyBoardConfigs("HP"))
        KeyBoardConfigs.Add("AK", KeyBoardConfigs("LK") & KeyBoardConfigs("MK") & KeyBoardConfigs("HK"))

    End Sub

    Public Sub ReloadConfigs()

        ' Get Profile Name
        ProfileName = MainFormRef.ConfigFile.KeyMapProfile

        ' Check if XML exists, if not make it
        If Not File.Exists(GetXMLFile(True)) Then CreateKeyMapConfigs()

        'Read configs
        Dim cfg As XDocument = XDocument.Load(GetXMLFile(True))

        'load configs to an easier to access variable
        KeybindConfigs.Clear()
        For Each node As XElement In cfg.<Configs>.<KeyMap>.Nodes
            KeybindConfigs.Add(New KeyBind(node.Name.ToString, node.<button>.Value, KeyBoardConfigs(node.Name.ToString)))
        Next

        ' Axis Cache
        RxAxis.Clear()
        For Each node As XElement In cfg.<Configs>.<AxisMap>.Nodes
            RxAxis.Add(node.Name.ToString, {0, node.<rest>.Value, node.<min>.Value, node.<max>.Value, 0})
        Next

        ' Key Cache
        KeyCache.Clear()
        For Each keybind As KeyBind In KeybindConfigs
            If Not KeyCache.ContainsKey(keybind.Button) Then
                KeyCache.Add(keybind.Button, {keybind.tKeychar, False})
            End If
        Next

        DeadZone = cfg.<Configs>.<DeadZone>.Value
        ControllerID = cfg.<Configs>.<ControllerID>.Value
        PoVRest = cfg.<Configs>.<PoV>.Value


        If Not MainFormRef.KeyMappingForm Is Nothing Then
            MainFormRef.KeyMappingForm.LoadingSettings = True
            MainFormRef.KeyMappingForm.cbControllerID.Invoke(Sub() MainFormRef.KeyMappingForm.cbControllerID.SelectedIndex = ControllerID)
            MainFormRef.KeyMappingForm.LoadingSettings = False

        End If

        NeedConfigReload = False

    End Sub

    Public Sub CreateKeyMapConfigs()
        ' Push the Defaults into the Array Setup so it's default XInput
        KeybindConfigs.Clear()
        KeybindConfigs.Add(New KeyBind("up", "y+", KeyBoardConfigs("up")))
        KeybindConfigs.Add(New KeyBind("down", "y-", KeyBoardConfigs("down")))
        KeybindConfigs.Add(New KeyBind("left", "x-", KeyBoardConfigs("left")))
        KeybindConfigs.Add(New KeyBind("right", "x+", KeyBoardConfigs("right")))

        KeybindConfigs.Add(New KeyBind("LP", "2", KeyBoardConfigs("LP")))
        KeybindConfigs.Add(New KeyBind("MP", "3", KeyBoardConfigs("MP")))
        KeybindConfigs.Add(New KeyBind("HP", "5", KeyBoardConfigs("HP")))

        KeybindConfigs.Add(New KeyBind("LK", "0", KeyBoardConfigs("LK")))
        KeybindConfigs.Add(New KeyBind("MK", "1", KeyBoardConfigs("MK")))
        KeybindConfigs.Add(New KeyBind("HK", "4", KeyBoardConfigs("HK")))

        KeybindConfigs.Add(New KeyBind("LPLK", "", KeyBoardConfigs("LPLK")))
        KeybindConfigs.Add(New KeyBind("MPMK", "", KeyBoardConfigs("MPMK")))
        KeybindConfigs.Add(New KeyBind("HPHK", "", KeyBoardConfigs("HPHK")))
        KeybindConfigs.Add(New KeyBind("AP", "", KeyBoardConfigs("AP")))
        KeybindConfigs.Add(New KeyBind("AK", "", KeyBoardConfigs("AK")))

        KeybindConfigs.Add(New KeyBind("start", "8", KeyBoardConfigs("start")))
        KeybindConfigs.Add(New KeyBind("coin", "9", KeyBoardConfigs("coin")))

        RxAxis.Clear()
        ' Axis ' {Current, Rest, Min, Max, Lastframe}
        RxAxis.Add("x", {0, 0, -1, 1, 0})
        RxAxis.Add("y", {0, 0, -1, 1, 0})
        RxAxis.Add("z", {0, 0, -1, 1, 0})
        RxAxis.Add("t", {0, 0, -1, 1, 0})
        RxAxis.Add("u", {0, 0, 0, 1, 0})
        RxAxis.Add("v", {0, 0, 0, 1, 0})

        WriteXMLConfigFile()
        MainFormRef.KeyMappingForm.cbProfile.Invoke(Sub() MainFormRef.KeyMappingForm.UpdateProfileList())


    End Sub

    Public Sub WriteXMLConfigFile()
        Dim writer As New XmlTextWriter(GetXMLFile(True), Text.Encoding.UTF8)
        writer.WriteStartDocument(True)
        writer.Formatting = Formatting.Indented
        writer.Indentation = 2
        writer.WriteStartElement("Configs")

        writer.WriteStartElement("ControllerID")
        writer.WriteString("0")
        writer.WriteEndElement()

        writer.WriteStartElement("KeyMap")
        For Each Rebind As KeyBind In KeybindConfigs
            WriteKeyCodeXML(Rebind, writer)
        Next
        writer.WriteEndElement()

        writer.WriteStartElement("AxisMap")
        For Each key As String In RxAxis.Keys
            CreateAxisXMLEntry(key, RxAxis(key)(2), RxAxis(key)(3), RxAxis(key)(1), writer)
        Next
        writer.WriteEndElement()

        writer.WriteStartElement("PoV")
        writer.WriteString(PoVRest)
        writer.WriteEndElement()

        writer.WriteStartElement("DeadZone")
        writer.WriteString(DeadZone)
        writer.WriteEndElement()

        writer.WriteEndElement()
        writer.WriteEndDocument()
        writer.Close()

    End Sub

    Private Sub CreateAxisXMLEntry(ByVal Axis As String, ByVal min As String, ByVal max As String, ByVal rest As String, ByRef writer As XmlTextWriter)

        writer.WriteStartElement(Axis)

        writer.WriteStartElement("min")
        writer.WriteString(min)
        writer.WriteEndElement()

        writer.WriteStartElement("max")
        writer.WriteString(max)
        writer.WriteEndElement()

        writer.WriteStartElement("rest")
        writer.WriteString(rest)
        writer.WriteEndElement()

        writer.WriteEndElement()

    End Sub

    Private Sub WriteKeyCodeXML(key As KeyBind, ByRef writer As XmlTextWriter)
        writer.WriteStartElement(key.Name)

        writer.WriteStartElement("button")
        writer.WriteString(key.Button)
        writer.WriteEndElement()

        writer.WriteEndElement()
    End Sub

    Private Sub DoXInputRoll(ByRef xinputstate As XInputDotNetPure.GamePadState)

        RxButtons(0) = Not Convert.ToBoolean(xinputstate.Buttons.A) ' 0
        RxButtons(1) = Not Convert.ToBoolean(xinputstate.Buttons.B) ' 1
        RxButtons(2) = Not Convert.ToBoolean(xinputstate.Buttons.X) ' 2
        RxButtons(3) = Not Convert.ToBoolean(xinputstate.Buttons.Y) ' 3

        RxButtons(4) = Not Convert.ToBoolean(xinputstate.Buttons.LeftShoulder) ' 4
        RxButtons(5) = Not Convert.ToBoolean(xinputstate.Buttons.RightShoulder) ' 5

        RxButtons(6) = Not Convert.ToBoolean(xinputstate.Buttons.LeftStick) ' 6
        RxButtons(7) = Not Convert.ToBoolean(xinputstate.Buttons.RightStick) ' 7

        RxButtons(8) = Not Convert.ToBoolean(xinputstate.Buttons.Start) ' 8
        RxButtons(9) = Not Convert.ToBoolean(xinputstate.Buttons.Back) ' 9

        RxButtons(10) = Not Convert.ToBoolean(xinputstate.DPad.Up) ' 10
        RxButtons(11) = Not Convert.ToBoolean(xinputstate.DPad.Down) ' 11
        RxButtons(12) = Not Convert.ToBoolean(xinputstate.DPad.Left) ' 12
        RxButtons(13) = Not Convert.ToBoolean(xinputstate.DPad.Right) ' 13

        ' Axis Handling
        RxAxis("x")(0) = xinputstate.ThumbSticks.Left.X
        RxAxis("y")(0) = xinputstate.ThumbSticks.Left.Y
        RxAxis("z")(0) = xinputstate.ThumbSticks.Right.X
        RxAxis("t")(0) = xinputstate.ThumbSticks.Right.Y
        RxAxis("u")(0) = xinputstate.Triggers.Left - 1
        RxAxis("v")(0) = xinputstate.Triggers.Right - 1

    End Sub

    Private Sub DoWinMMRoll()
        'Console.WriteLine("WinMM Input ROll")
        Call joyGetPosEx(ControllerID, myjoyEX)
        With myjoyEX
            ' Map out the inputs PoV and Key
            PoV = .dwPOV
            Dim PoVMap = "0000"
            ' This isn't the PoV's idle position
            If Not PoVRest = PoV Then
                Select Case PoV / 100 / 45
                    Case 0
                        PoVMap = "1000"
                    Case 1
                        PoVMap = "1100"
                    Case 2
                        PoVMap = "0100"
                    Case 3
                        PoVMap = "0110"
                    Case 4
                        PoVMap = "0010"
                    Case 5
                        PoVMap = "0011"
                    Case 6
                        PoVMap = "0001"
                    Case 7
                        PoVMap = "1001"
                    Case Else
                        PoVMap = "0000"
                End Select
            End If

            Dim keymap = StrReverse(Convert.ToString(.dwButtons, 2).PadLeft(12, "0"))
            keymap += PoVMap

            ' Convert them to RxButton
            For i = 0 To keymap.Count - 1
                RxButtons(i) = keymap(i).ToString
            Next

            RxAxis("x")(0) = .dwXpos
            RxAxis("y")(0) = .dwYpos
            RxAxis("z")(0) = .dwZpos
            RxAxis("t")(0) = .dwTpos
            RxAxis("u")(0) = .dwUpos
            RxAxis("v")(0) = .dwVpos

        End With
    End Sub

    Private Sub DoOpenTKInputRoll(ByRef State As JoystickState)

        'Console.WriteLine("OpenTK Input ROll")
        'State = Joystick.GetState(ControllerID)

        Dim Capabilities = Joystick.GetCapabilities(ControllerID)
        Dim ButtonIndex = 0
        For i = 0 To Capabilities.ButtonCount - 1
            RxButtons(i) = State.IsButtonDown(i)
            ButtonIndex += 1
        Next

        ' Get Hat Data
        For i = 0 To Capabilities.HatCount - 1
            RxButtons(i + ButtonIndex) = State.GetHat(i).IsUp
            RxButtons(i + ButtonIndex + 1) = State.GetHat(i).IsDown
            RxButtons(i + ButtonIndex + 2) = State.GetHat(i).IsLeft
            RxButtons(i + ButtonIndex + 3) = State.GetHat(i).IsRight
            ButtonIndex += 4
        Next
        Dim rawr = RxButtons

        ' Fill the rest of the array with false
        For i = ButtonIndex To RxButtons.Count - 1
            RxButtons(i) = False
        Next


        For i = 0 To 5 ' 0-5 | 6 Axis Support
            If Not State.GetAxis(i) = 0 Then
                RxAxis(RxAxis.Keys(i))(0) = Math.Round(State.GetAxis(i), 2)
            End If
        Next

    End Sub

    Public Sub InputRoll()
        Console.WriteLine("Started Input Rolling")
        ' Poll the Inptus once at start just to get their natural states

        ' While loop is < 1ms
        While True
            Thread.Sleep(8)

            ' If the mapping is off then just pause it here, but lets not remove the thread because a bitch to deal with threads
            While Not TurnedOn
                Thread.Sleep(1000)
            End While

            Dim xinputstate As XInputDotNetPure.GamePadState = XInputDotNetPure.GamePad.GetState(ControllerID) ' Check XInput
            Dim RawrInputState As JoystickState = Joystick.GetState(ControllerID) ' Check RawInput

            ' Go with Xinput over RawInput
            If xinputstate.IsConnected Then
                DoXInputRoll(xinputstate)
            ElseIf RawrInputState.IsConnected Then
                DoOpenTKInputRoll(RawrInputState)
            Else
                DoWinMMRoll()
            End If

            For i = 0 To RxButtons.Count - 1
                If Not RxButtons(i) = LF_RxButtons(i) Then
                    If RxButtons(i) Then
                        RaiseEvent _KeyPressed(i)
                    Else
                        RaiseEvent _KeyReleased(i)
                    End If
                End If
            Next

            ' Done and Done Ez Pz now for the Axis
            For Each key As String In RxAxis.Keys
                If Not RxAxis(key)(4) = RxAxis(key)(0) Then
                    If RxAxis(key)(0) > (RxAxis(key)(1) + (RxAxis(key)(3) * (DeadZone / 100))) Then
                        RaiseEvent _KeyReleased(key & "-")
                        RaiseEvent _KeyPressed(key & "+")

                    ElseIf RxAxis(key)(0) < (RxAxis(key)(1) + (RxAxis(key)(2) * (DeadZone / 100))) Then
                        RaiseEvent _KeyReleased(key & "+")
                        RaiseEvent _KeyPressed(key & "-")

                    Else
                        RaiseEvent _KeyReleased(key & "-")
                        RaiseEvent _KeyReleased(key & "+")
                    End If
                End If
                RxAxis(key)(4) = RxAxis(key)(0)
            Next

            If NeedConfigReload Then
                ReloadConfigs()
                RxButtons = New BitArray(32, False)
                LF_RxButtons = New BitArray(32, False)
            End If

            ' Put current inputs in array of last roll
            For i = 0 To RxButtons.Count - 1
                LF_RxButtons(i) = RxButtons(i)
            Next

        End While

    End Sub

    Public Sub KeyPressed(Button As String) Handles Me._KeyPressed
        Console.WriteLine("Button Pressed: {0}", Button)
        TranslateButtonToKey(Button, True)
    End Sub

    Public Sub KeyReleased(Button As String) Handles Me._KeyReleased
        TranslateButtonToKey(Button, False)
    End Sub

    Private Sub TranslateButtonToKey(Button As String, Down As Boolean)
        ' If We're rebinding then don't fire anything, let it handle it
        If MainFormRef.KeyMappingForm.Rebinding Then Exit Sub
        Dim upordown = 2
        If Down Then upordown = 0
        If KeyCache.ContainsKey(Button) Then
            If KeyCache(Button)(1) = Down Then Exit Sub
            For Each Key As Keys In KeyCache(Button)(0)
                'Console.WriteLine("Button: " & Button & " " & Down)
                If KeyBoardConfigs("coin") = kc.ConvertToString(Key) And MainFormRef.IsNullDCRunning Then

                    If Down Then
                        If Not CoinKeyDown Then
                            keybd_event(Key, MapVirtualKey(Key, 0), 0, 0)
                            Thread.Sleep(16)
                            keybd_event(Key, MapVirtualKey(Key, 0), 2, 0)
                            CoinKeyDown = True
                        End If
                    Else
                        CoinKeyDown = False
                    End If

                Else
                    keybd_event(Key, MapVirtualKey(Key, 0), upordown, 0)
                End If

            Next
            KeyCache(Button)(1) = Down
        End If

    End Sub

    Private Function GetKeyFromChar(c As Char) As Keys
        Dim vkKeyCode As Short = VkKeyScanExW(c, InputLanguage.CurrentInputLanguage.Handle)
        Return CType((((vkKeyCode And &HFF00) << 8) Or (vkKeyCode And &HFF)), Keys)
    End Function

    Public Sub UpdateAxisMap(NewMap As Dictionary(Of String, Array))
        RxAxis = NewMap
        WriteXMLConfigFile()
        NeedConfigReload = True
    End Sub

End Class

Public Class KeyBind

    <DllImport("user32.dll", EntryPoint:="VkKeyScanExW")>
    Public Shared Function VkKeyScanExW(ByVal ch As Char, ByVal dwhkl As IntPtr) As Short
    End Function

    Public Name As String = ""
    Public Button As String
    Public Rebind As String
    Public tKeychar As ArrayList = New ArrayList
    Public Pressed As Boolean

    Public Sub New(_Name As String)
        Name = _Name
        Button = ""
        Rebind = "a"
        For Each Key As Char In Rebind.ToCharArray
            tKeychar.Add(GetKeyFromChar(Key))
        Next

    End Sub

    Public Sub New(_Name As String, _Button As String, _Rebind As String)
        Name = _Name
        Button = _Button
        Rebind = _Rebind
        For Each Key As Char In Rebind.ToCharArray
            tKeychar.Add(GetKeyFromChar(Key))
        Next
    End Sub

    Private Function GetKeyFromChar(c As Char) As Keys
        Dim vkKeyCode As Short = VkKeyScanExW(c, InputLanguage.CurrentInputLanguage.Handle) 'get the vertial key code value for the Char according to the keyboard layout for this thread
        'Get the vkKey value from the low order byte and the Shift state from the high order byte from the vkKeyCode value and add them together using the Bitwise OR operator.
        Return CType((((vkKeyCode And &HFF00) << 8) Or (vkKeyCode And &HFF)), Keys) ''Then cast that value to a Keys type.
    End Function

End Class