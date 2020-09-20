﻿'Imports System.Runtime.InteropServices
'Imports System.Threading
'Imports System.Timers
'Imports System.Xml
'Imports XInputDotNetPure
'Imports OpenTK
'Imports OpenTK.Input
'Imports System.IO

'Public Class InputHandling

'    Public ProfileName As String = "Default"
'    Public KeyBoardConfigs As New Dictionary(Of String, Array)
'    Public PoV As Decimal = 0 ' PoV / Dpad
'    Public PoVRest As Decimal = 0 ' PoV / Dpad
'    Public DeadZone As Int16 = 10
'    Public NeedConfigReload As Boolean = False
'    Public KeyCache As New Dictionary(Of String, Array) ' Keeps data for the key presses last frame to avoid redoing the same thing over and over
'    Public RxAxis As New Dictionary(Of String, Array) ' Current, Idle, Min, Max, Last Frame
'    Public PollThread As Thread
'    Public myjoyEX As JOYINFOEX
'    Public Event _KeyPressed(ByVal Key As String)
'    Public Event _KeyReleased(ByVal Key As String)
'    Public KeybindConfigs As ArrayList = New ArrayList 'Key Configs from XML
'    Dim kc As KeysConverter = New KeysConverter
'    Public ControllerID As Int16 = 0
'    Dim TurnedOn As Boolean = True

'    Dim RxButtons As New BitArray(32, False)
'    Dim LF_RxButtons As New BitArray(32, False)

'    Dim MainFormRef As frmMain 'Rebind Vars
'    Dim CoinKeyDown As Boolean = False

'    Dim SpectatorControls_FastForward_Pressed As Boolean = False

'    <DllImport("winmm.dll", EntryPoint:="joyGetPosEx")>
'    Private Shared Function joyGetPosEx(ByVal uJoyID As Integer, ByRef pji As JOYINFOEX) As Integer
'    End Function

'    <DllImport("user32.dll")>
'    Private Shared Sub keybd_event(bVk As Byte, bScan As Byte, dwFlags As UInteger, dwExtraInfo As UIntPtr)
'    End Sub

'    <DllImport("User32.dll", SetLastError:=False, CallingConvention:=CallingConvention.StdCall, CharSet:=CharSet.Auto)>
'    Public Shared Function MapVirtualKey(ByVal uCode As UInt32, ByVal uMapType As UInt32) As UInt32
'    End Function

'    <DllImport("user32.dll", EntryPoint:="VkKeyScanExW")>
'    Public Shared Function VkKeyScanExW(ByVal ch As Char, ByVal dwhkl As IntPtr) As Short
'    End Function

'    <DllImport("user32.dll")>
'    Public Shared Function GetAsyncKeyState(ByVal vKey As System.Windows.Forms.Keys) As Short
'    End Function

'    <StructLayout(LayoutKind.Sequential)>
'    Public Structure JOYINFOEX
'        Public dwSize As Integer
'        Public dwFlags As Integer
'        Public dwXpos As Integer
'        Public dwYpos As Integer
'        Public dwZpos As Integer
'        Public dwTpos As Integer
'        Public dwUpos As Integer
'        Public dwVpos As Integer
'        Public dwButtons As Integer
'        Public dwButtonNumber As Integer
'        Public dwPOV As Integer
'        Public dwReserved1 As Integer
'        Public dwReserved2 As Integer
'    End Structure

'    Public Sub New(ByRef mf As frmMain)
'        ' Disable SDL2 Backend for OpenTK because i do not want to use it
'        Dim OpenTKOptions = New ToolkitOptions
'        OpenTKOptions.Backend = PlatformBackend.PreferNative
'        Toolkit.Init(OpenTKOptions)

'        MainFormRef = mf
'        myjoyEX.dwSize = 64
'        myjoyEX.dwFlags = &HFF

'        GetKeyboardConfigs("na")
'        ReloadConfigs()

'        TurnedOn = MainFormRef.ConfigFile.UseRemap

'        PollThread = New Thread(AddressOf InputRoll)
'        PollThread.IsBackground = True
'        PollThread.Start()

'    End Sub

'    Public Sub TurnOnOff(ByVal OnOff As Boolean)
'        If OnOff Then
'            TurnedOn = True
'        Else
'            TurnedOn = False
'        End If
'    End Sub

'    Public Function GetXMLFile(Optional ByVal FullPath As Boolean = False) As String
'        Dim _path = ""
'        If FullPath Then _path = MainFormRef.NullDCPath & "\"

'        If ProfileName = "Default" Then
'            _path += "KeyMapReBinds.xml"
'        Else
'            _path += "KeyMapReBinds_" & ProfileName & ".xml"
'        End If

'        Return _path
'    End Function

'    Public Sub GetKeyboardConfigs(ByVal _platform As String)
'        Console.WriteLine("Got Keyboard Configs for: " & _platform)

'        KeyBoardConfigs.Clear()

'        While MainFormRef.IsFileInUse(MainFormRef.NullDCPath & "\nullDC.cfg") Or MainFormRef.IsFileInUse(MainFormRef.NullDCPath & "\dc\nullDC.cfg")
'            Thread.Sleep(500)
'        End While

'        Dim KeyboardLines_Naomi() As String = File.ReadAllLines(MainFormRef.NullDCPath & "\nullDC.cfg")
'        Dim KeyboardLines_Dreamcast() As String = File.ReadAllLines(MainFormRef.NullDCPath & "\dc\nullDC.cfg")

'        Select Case _platform

'            Case "dc"

'                For Each line As String In KeyboardLines_Dreamcast
'                    If line.StartsWith("BPortA_CONT_START=") Then KeyBoardConfigs.Add("start", {CInt(line.Split("=")(1))})
'                    If line.StartsWith("BPortA_CONT_ANALOG_UP=") Then KeyBoardConfigs.Add("up", {CInt(line.Split("=")(1))})
'                    If line.StartsWith("BPortA_CONT_ANALOG_DOWN=") Then KeyBoardConfigs.Add("down", {CInt(line.Split("=")(1))})
'                    If line.StartsWith("BPortA_CONT_ANALOG_LEFT=") Then KeyBoardConfigs.Add("left", {CInt(line.Split("=")(1))})
'                    If line.StartsWith("BPortA_CONT_ANALOG_RIGHT=") Then KeyBoardConfigs.Add("right", {CInt(line.Split("=")(1))})
'                    If line.StartsWith("BPortA_CONT_X=") Then KeyBoardConfigs.Add("LP", {CInt(line.Split("=")(1))})
'                    If line.StartsWith("BPortA_CONT_Y=") Then KeyBoardConfigs.Add("MP", {CInt(line.Split("=")(1))})
'                    If line.StartsWith("BPortA_CONT_LSLIDER=") Then KeyBoardConfigs.Add("HP", {CInt(line.Split("=")(1))})
'                    If line.StartsWith("BPortA_CONT_A=") Then KeyBoardConfigs.Add("LK", {CInt(line.Split("=")(1))})
'                    If line.StartsWith("BPortA_CONT_B=") Then KeyBoardConfigs.Add("MK", {CInt(line.Split("=")(1))})
'                    If line.StartsWith("BPortA_CONT_RSLIDER=") Then KeyBoardConfigs.Add("HK", {CInt(line.Split("=")(1))})
'                    If line.StartsWith("BPortA_CONT_DPAD_UP=") Then KeyBoardConfigs.Add("dpadup", {CInt(line.Split("=")(1))})
'                    If line.StartsWith("BPortA_CONT_DPAD_DOWN=") Then KeyBoardConfigs.Add("dpaddown", {CInt(line.Split("=")(1))})
'                    If line.StartsWith("BPortA_CONT_DPAD_LEFT=") Then KeyBoardConfigs.Add("dpadleft", {CInt(line.Split("=")(1))})
'                    If line.StartsWith("BPortA_CONT_DPAD_RIGHT=") Then KeyBoardConfigs.Add("dpadright", {CInt(line.Split("=")(1))})
'                Next

'                For Each line As String In KeyboardLines_Naomi
'                    If line.StartsWith("BPortA_I_COIN_KEY") Then KeyBoardConfigs.Add("coin", {CInt(line.Split("=")(1))})
'                Next

'            Case "na"

'                For Each line As String In KeyboardLines_Naomi
'                    If line.StartsWith("BPortA_I_START_KEY=") Then KeyBoardConfigs.Add("start", {CInt(line.Split("=")(1))})
'                    If line.StartsWith("BPortA_I_COIN_KEY=") Then KeyBoardConfigs.Add("coin", {CInt(line.Split("=")(1))})
'                    If line.StartsWith("BPortA_I_UP_KEY=") Then KeyBoardConfigs.Add("up", {CInt(line.Split("=")(1))})
'                    If line.StartsWith("BPortA_I_DOWN_KEY=") Then KeyBoardConfigs.Add("down", {CInt(line.Split("=")(1))})
'                    If line.StartsWith("BPortA_I_LEFT_KEY=") Then KeyBoardConfigs.Add("left", {CInt(line.Split("=")(1))})
'                    If line.StartsWith("BPortA_I_RIGHT_KEY=") Then KeyBoardConfigs.Add("right", {CInt(line.Split("=")(1))})
'                    If line.StartsWith("BPortA_I_BTN0_KEY=") Then KeyBoardConfigs.Add("LP", {CInt(line.Split("=")(1))})
'                    If line.StartsWith("BPortA_I_BTN1_KEY=") Then KeyBoardConfigs.Add("MP", {CInt(line.Split("=")(1))})
'                    If line.StartsWith("BPortA_I_BTN2_KEY=") Then KeyBoardConfigs.Add("HP", {CInt(line.Split("=")(1))})
'                    If line.StartsWith("BPortA_I_BTN3_KEY=") Then KeyBoardConfigs.Add("LK", {CInt(line.Split("=")(1))})
'                    If line.StartsWith("BPortA_I_BTN4_KEY=") Then KeyBoardConfigs.Add("MK", {CInt(line.Split("=")(1))})
'                    If line.StartsWith("BPortA_I_BTN5_KEY=") Then KeyBoardConfigs.Add("HK", {CInt(line.Split("=")(1))})
'                Next

'                For Each line As String In KeyboardLines_Dreamcast
'                    If line.StartsWith("BPortA_CONT_DPAD_UP=") Then KeyBoardConfigs.Add("dpadup", {CInt(line.Split("=")(1))})
'                    If line.StartsWith("BPortA_CONT_DPAD_DOWN=") Then KeyBoardConfigs.Add("dpaddown", {CInt(line.Split("=")(1))})
'                    If line.StartsWith("BPortA_CONT_DPAD_LEFT=") Then KeyBoardConfigs.Add("dpadleft", {CInt(line.Split("=")(1))})
'                    If line.StartsWith("BPortA_CONT_DPAD_RIGHT=") Then KeyBoardConfigs.Add("dpadright", {CInt(line.Split("=")(1))})
'                Next

'        End Select

'        Dim keycount = 0
'        Dim keycount2 = 0
'        For Each key As KeyValuePair(Of String, Array) In KeyBoardConfigs
'            For Each key2 As KeyValuePair(Of String, Array) In KeyBoardConfigs
'                If key.Value(0) = key2.Value(0) And Not key.Key = key2.Key And keycount2 > keycount Then
'                    MsgBox("Double Input Found: " & vbNewLine & key.Key & " and " & key2.Key & vbNewLine & "Please rebind them to something else in NullDC")
'                End If
'                keycount2 += 1
'            Next
'            keycount2 = 0
'            keycount += 1
'        Next

'        KeyBoardConfigs.Add("LPLK", {KeyBoardConfigs("LP")(0), KeyBoardConfigs("LK")(0)})
'        KeyBoardConfigs.Add("MPMK", {KeyBoardConfigs("MP")(0), KeyBoardConfigs("MK")(0)})
'        KeyBoardConfigs.Add("HPHK", {KeyBoardConfigs("HP")(0), KeyBoardConfigs("HK")(0)})

'        KeyBoardConfigs.Add("LPMP", {KeyBoardConfigs("LP")(0), KeyBoardConfigs("MP")(0)})
'        KeyBoardConfigs.Add("MPHP", {KeyBoardConfigs("MP")(0), KeyBoardConfigs("HP")(0)})
'        KeyBoardConfigs.Add("LKMK", {KeyBoardConfigs("LK")(0), KeyBoardConfigs("MK")(0)})
'        KeyBoardConfigs.Add("MKHK", {KeyBoardConfigs("MK")(0), KeyBoardConfigs("HK")(0)})

'        KeyBoardConfigs.Add("AP", {KeyBoardConfigs("LP")(0), KeyBoardConfigs("MP")(0), KeyBoardConfigs("HP")(0)})
'        KeyBoardConfigs.Add("AK", {KeyBoardConfigs("LK")(0), KeyBoardConfigs("MK")(0), KeyBoardConfigs("HK")(0)})



'    End Sub

'    Public Sub ReloadConfigs()
'        Console.WriteLine("InputHandling:ReloadConfigs")

'        ProfileName = MainFormRef.ConfigFile.KeyMapProfile
'        If Not File.Exists(GetXMLFile(True)) Then CreateKeyMapConfigs()
'        Dim cfg As XDocument = XDocument.Load(GetXMLFile(True))

'        'load configs to an easier to access variable
'        KeybindConfigs.Clear()
'        For Each node As XElement In cfg.<Configs>.<KeyMap>.Nodes
'            If Not node.<button>.Value = "" Then
'                KeybindConfigs.Add(New KeyBind(node.Name.ToString, node.<button>.Value, KeyBoardConfigs(node.Name.ToString)))
'            End If
'        Next

'        ' Axis Cache
'        RxAxis.Clear()
'        For Each node As XElement In cfg.<Configs>.<AxisMap>.Nodes
'            RxAxis.Add(node.Name.ToString, {0, node.<rest>.Value, node.<min>.Value, node.<max>.Value, 0})
'        Next

'        ' Key Cache
'        KeyCache.Clear()
'        For Each keybind As KeyBind In KeybindConfigs
'            If Not KeyCache.ContainsKey(keybind.Button) Then
'                KeyCache.Add(keybind.Button, {keybind.Rebind, False, keybind.Name})
'            End If
'        Next

'        DeadZone = cfg.<Configs>.<DeadZone>.Value
'        ControllerID = cfg.<Configs>.<ControllerID>.Value
'        PoVRest = cfg.<Configs>.<PoV>.Value

'        ' Check Need to Resave the file to a newer version type
'        UpdateKeyMapConfigs()

'        If Not MainFormRef.KeyMappingForm Is Nothing Then
'            MainFormRef.KeyMappingForm.LoadingSettings = True
'            MainFormRef.KeyMappingForm.cbControllerID.Invoke(Sub()
'                                                                 MainFormRef.KeyMappingForm.cbControllerID.SelectedIndex = ControllerID
'                                                             End Sub)
'            MainFormRef.KeyMappingForm.LoadingSettings = False
'        End If

'        NeedConfigReload = False

'    End Sub

'    Public Sub UpdateKeyMapConfigs()

'        Dim ListOfAllBinds As String() = {"up", "down", "left", "right", "LP", "MP", "HP", "LK", "MK", "HK", "LPLK", "MPMK", "HPHK", "LPMP", "MPHP", "LKMK", "MKHK", "AP", "AK", "start", "coin", "dpadup", "dpaddown", "dpadleft", "dpadright"}
'        For Each KeyBind As KeyBind In KeybindConfigs
'            For i = 0 To ListOfAllBinds.Count - 1
'                If KeyBind.Name = ListOfAllBinds(i) Then
'                    ListOfAllBinds(i) = Nothing
'                    Exit For
'                End If
'            Next
'        Next

'        For Each MissingKey As String In ListOfAllBinds
'            If Not MissingKey Is Nothing Then
'                KeybindConfigs.Add(New KeyBind(MissingKey, "", KeyBoardConfigs(MissingKey)))
'                Console.WriteLine("Missing Key: {0}", MissingKey)
'            End If
'        Next

'        WriteXMLConfigFile()

'    End Sub

'    Public Sub CreateKeyMapConfigs()
'        ' Push the Defaults into the Array Setup so it's default XInput
'        KeybindConfigs.Clear()
'        KeybindConfigs.Add(New KeyBind("up", "y+", KeyBoardConfigs("up")))
'        KeybindConfigs.Add(New KeyBind("down", "y-", KeyBoardConfigs("down")))
'        KeybindConfigs.Add(New KeyBind("left", "x-", KeyBoardConfigs("left")))
'        KeybindConfigs.Add(New KeyBind("right", "x+", KeyBoardConfigs("right")))

'        KeybindConfigs.Add(New KeyBind("LP", "2", KeyBoardConfigs("LP")))
'        KeybindConfigs.Add(New KeyBind("MP", "3", KeyBoardConfigs("MP")))
'        KeybindConfigs.Add(New KeyBind("HP", "4", KeyBoardConfigs("HP")))

'        KeybindConfigs.Add(New KeyBind("LK", "0", KeyBoardConfigs("LK")))
'        KeybindConfigs.Add(New KeyBind("MK", "1", KeyBoardConfigs("MK")))
'        KeybindConfigs.Add(New KeyBind("HK", "5", KeyBoardConfigs("HK")))

'        KeybindConfigs.Add(New KeyBind("LPLK", "", KeyBoardConfigs("LPLK")))
'        KeybindConfigs.Add(New KeyBind("MPMK", "", KeyBoardConfigs("MPMK")))
'        KeybindConfigs.Add(New KeyBind("HPHK", "", KeyBoardConfigs("HPHK")))
'        ' New Micros
'        KeybindConfigs.Add(New KeyBind("LPMP", "", KeyBoardConfigs("LPMP")))
'        KeybindConfigs.Add(New KeyBind("MPHP", "", KeyBoardConfigs("MPHP")))
'        KeybindConfigs.Add(New KeyBind("LKMK", "", KeyBoardConfigs("LKMK")))
'        KeybindConfigs.Add(New KeyBind("MKHK", "", KeyBoardConfigs("MKHK")))

'        KeybindConfigs.Add(New KeyBind("AP", "", KeyBoardConfigs("AP")))
'        KeybindConfigs.Add(New KeyBind("AK", "", KeyBoardConfigs("AK")))

'        KeybindConfigs.Add(New KeyBind("start", "8", KeyBoardConfigs("start")))
'        KeybindConfigs.Add(New KeyBind("coin", "9", KeyBoardConfigs("coin")))

'        KeybindConfigs.Add(New KeyBind("dpadup", "10", KeyBoardConfigs("dpadup")))
'        KeybindConfigs.Add(New KeyBind("dpaddown", "11", KeyBoardConfigs("dpaddown")))
'        KeybindConfigs.Add(New KeyBind("dpadleft", "12", KeyBoardConfigs("dpadleft")))
'        KeybindConfigs.Add(New KeyBind("dpadright", "13", KeyBoardConfigs("dpadright")))

'        RxAxis.Clear()
'        ' Axis ' {Current, Rest, Min, Max, Lastframe}
'        RxAxis.Add("x", {0, 0, -1, 1, 0})
'        RxAxis.Add("y", {0, 0, -1, 1, 0})
'        RxAxis.Add("z", {0, 0, -1, 1, 0})
'        RxAxis.Add("t", {0, 0, -1, 1, 0})
'        RxAxis.Add("u", {0, 0, 0, 1, 0})
'        RxAxis.Add("v", {0, 0, 0, 1, 0})

'        WriteXMLConfigFile()

'        If Not MainFormRef.KeyMappingForm Is Nothing Then
'            MainFormRef.KeyMappingForm.cbProfile.Invoke(Sub() MainFormRef.KeyMappingForm.UpdateProfileList())
'        End If

'    End Sub

'    Public Sub WriteXMLConfigFile()
'        Dim writer As New XmlTextWriter(GetXMLFile(True), Text.Encoding.UTF8)
'        writer.WriteStartDocument(True)
'        writer.Formatting = Formatting.Indented
'        writer.Indentation = 2
'        writer.WriteStartElement("Configs")

'        writer.WriteStartElement("ControllerID")
'        writer.WriteString(ControllerID)
'        writer.WriteEndElement()

'        writer.WriteStartElement("KeyMap")
'        For Each Rebind As KeyBind In KeybindConfigs
'            WriteKeyCodeXML(Rebind, writer)
'        Next
'        writer.WriteEndElement()

'        writer.WriteStartElement("AxisMap")
'        For Each key As String In RxAxis.Keys
'            CreateAxisXMLEntry(key, RxAxis(key)(2), RxAxis(key)(3), RxAxis(key)(1), writer)
'        Next
'        writer.WriteEndElement()

'        writer.WriteStartElement("PoV")
'        writer.WriteString(PoVRest)
'        writer.WriteEndElement()

'        writer.WriteStartElement("DeadZone")
'        writer.WriteString(DeadZone)
'        writer.WriteEndElement()

'        writer.WriteStartElement("BEARver")
'        writer.WriteString(MainFormRef.Ver)
'        writer.WriteEndElement()

'        writer.WriteEndElement()
'        writer.WriteEndDocument()
'        writer.Close()

'    End Sub

'    Private Sub CreateAxisXMLEntry(ByVal Axis As String, ByVal min As String, ByVal max As String, ByVal rest As String, ByRef writer As XmlTextWriter)

'        writer.WriteStartElement(Axis)

'        writer.WriteStartElement("min")
'        writer.WriteString(min)
'        writer.WriteEndElement()

'        writer.WriteStartElement("max")
'        writer.WriteString(max)
'        writer.WriteEndElement()

'        writer.WriteStartElement("rest")
'        writer.WriteString(rest)
'        writer.WriteEndElement()

'        writer.WriteEndElement()

'    End Sub

'    Private Sub WriteKeyCodeXML(key As KeyBind, ByRef writer As XmlTextWriter)
'        writer.WriteStartElement(key.Name)

'        writer.WriteStartElement("button")
'        writer.WriteString(key.Button)
'        writer.WriteEndElement()

'        writer.WriteEndElement()
'    End Sub

'    Private Sub DoXInputRoll(ByRef xinputstate As XInputDotNetPure.GamePadState)

'        RxButtons(0) = Not Convert.ToBoolean(xinputstate.Buttons.A) ' 0
'        RxButtons(1) = Not Convert.ToBoolean(xinputstate.Buttons.B) ' 1
'        RxButtons(2) = Not Convert.ToBoolean(xinputstate.Buttons.X) ' 2
'        RxButtons(3) = Not Convert.ToBoolean(xinputstate.Buttons.Y) ' 3

'        RxButtons(4) = Not Convert.ToBoolean(xinputstate.Buttons.LeftShoulder) ' 4
'        RxButtons(5) = Not Convert.ToBoolean(xinputstate.Buttons.RightShoulder) ' 5

'        RxButtons(6) = Not Convert.ToBoolean(xinputstate.Buttons.LeftStick) ' 6
'        RxButtons(7) = Not Convert.ToBoolean(xinputstate.Buttons.RightStick) ' 7

'        RxButtons(8) = Not Convert.ToBoolean(xinputstate.Buttons.Start) ' 8
'        RxButtons(9) = Not Convert.ToBoolean(xinputstate.Buttons.Back) ' 9

'        RxButtons(10) = Not Convert.ToBoolean(xinputstate.DPad.Up) ' 10
'        RxButtons(11) = Not Convert.ToBoolean(xinputstate.DPad.Down) ' 11
'        RxButtons(12) = Not Convert.ToBoolean(xinputstate.DPad.Left) ' 12
'        RxButtons(13) = Not Convert.ToBoolean(xinputstate.DPad.Right) ' 13

'        ' Axis Handling
'        RxAxis("x")(0) = Math.Round(xinputstate.ThumbSticks.Left.X, 2)
'        RxAxis("y")(0) = Math.Round(xinputstate.ThumbSticks.Left.Y, 2)
'        RxAxis("z")(0) = Math.Round(xinputstate.ThumbSticks.Right.X, 2)
'        RxAxis("t")(0) = Math.Round(xinputstate.ThumbSticks.Right.Y, 2)
'        RxAxis("u")(0) = Math.Round(xinputstate.Triggers.Left, 2)
'        RxAxis("v")(0) = Math.Round(xinputstate.Triggers.Right, 2)

'    End Sub

'    Private Sub DoWinMMRoll()
'        'Console.WriteLine("WinMM Input ROll")
'        Call joyGetPosEx(ControllerID, myjoyEX)
'        With myjoyEX
'            ' Map out the inputs PoV and Key
'            PoV = .dwPOV
'            Dim PoVMap = "0000"
'            ' This isn't the PoV's idle position
'            If Not PoVRest = PoV Then
'                Select Case PoV / 100 / 45
'                    Case 0
'                        PoVMap = "1000"
'                    Case 1
'                        PoVMap = "1100"
'                    Case 2
'                        PoVMap = "0100"
'                    Case 3
'                        PoVMap = "0110"
'                    Case 4
'                        PoVMap = "0010"
'                    Case 5
'                        PoVMap = "0011"
'                    Case 6
'                        PoVMap = "0001"
'                    Case 7
'                        PoVMap = "1001"
'                    Case Else
'                        PoVMap = "0000"
'                End Select
'            End If

'            Dim keymap = StrReverse(Convert.ToString(.dwButtons, 2).PadLeft(12, "0"))
'            keymap += PoVMap

'            ' Convert them to RxButton
'            For i = 0 To keymap.Count - 1
'                RxButtons(i) = keymap(i).ToString
'            Next

'            RxAxis("x")(0) = Math.Round((.dwXpos / 256) / 256, 2)
'            RxAxis("y")(0) = Math.Round((.dwYpos / 256) / 256, 2)
'            RxAxis("z")(0) = Math.Round((.dwZpos / 256) / 256, 2)
'            RxAxis("t")(0) = Math.Round((.dwTpos / 256) / 256, 2)
'            RxAxis("u")(0) = Math.Round((.dwUpos / 256) / 256, 2)
'            RxAxis("v")(0) = Math.Round((.dwVpos / 256) / 256, 2)

'        End With
'    End Sub

'    Private Sub DoOpenTKInputRoll(ByRef State As JoystickState)

'        Dim Capabilities = Joystick.GetCapabilities(ControllerID)

'        Dim ButtonIndex = 0
'        For i = 0 To Capabilities.ButtonCount - 1
'            RxButtons(i) = State.IsButtonDown(i)
'            ButtonIndex += 1
'        Next

'        ' Get Hat Data
'        For i = 0 To Capabilities.HatCount - 1
'            RxButtons(i + ButtonIndex) = State.GetHat(i).IsUp
'            RxButtons(i + ButtonIndex + 1) = State.GetHat(i).IsDown
'            RxButtons(i + ButtonIndex + 2) = State.GetHat(i).IsLeft
'            RxButtons(i + ButtonIndex + 3) = State.GetHat(i).IsRight
'            ButtonIndex += 4
'        Next

'        ' Fill the rest of the array with false
'        For i = ButtonIndex To RxButtons.Count - 1
'            RxButtons(i) = False
'        Next


'        For i = 0 To 5 ' 0-5 | 6 Axis Support
'            If Not State.GetAxis(i) = 0 Then
'                RxAxis(RxAxis.Keys(i))(0) = Math.Round(State.GetAxis(i), 2)
'            End If
'        Next

'    End Sub

'    Public Sub InputRoll()

'        Thread.Sleep(1000)
'        While MainFormRef Is Nothing
'            While Not MainFormRef.FinishedLoading
'                Thread.Sleep(500)
'            End While
'            Thread.Sleep(500)
'        End While

'        Console.WriteLine("Started Input Rolling")
'        ' While loop is < 1ms
'        While True
'            Thread.Sleep(7)

'            If MainFormRef.ConfigFile.Status = "Spectator" Then
'                Try
'                    If MainFormRef.IsNullDCRunning Then
'                        If MainFormRef.NullDCLauncher.IsNullDCWindowSelected Then
'                            If GetAsyncKeyState(KeyBoardConfigs("right")(0)) <> 0 Then
'                                If SpectatorControls_FastForward_Pressed = False Then
'                                    SpectatorControls_FastForward_Pressed = True
'                                    FastforwardReplayToggle()
'                                End If
'                            Else
'                                SpectatorControls_FastForward_Pressed = False
'                            End If

'                        End If
'                    End If
'                Catch ex As Exception
'                End Try
'            End If
'            While Not TurnedOn
'                Thread.Sleep(1000)
'            End While

'            Dim xinputstate As XInputDotNetPure.GamePadState = XInputDotNetPure.GamePad.GetState(ControllerID)
'            Dim RawrInputState As JoystickState = Joystick.GetState(ControllerID)

'            If xinputstate.IsConnected Then
'                DoXInputRoll(xinputstate)
'            ElseIf RawrInputState.IsConnected Then
'                DoOpenTKInputRoll(RawrInputState)
'            Else
'                DoWinMMRoll()
'            End If

'            For i = 0 To RxButtons.Count - 1
'                If Not RxButtons(i) = LF_RxButtons(i) Then
'                    If RxButtons(i) Then
'                        RaiseEvent _KeyPressed(i)
'                    Else
'                        RaiseEvent _KeyReleased(i)
'                    End If
'                End If
'            Next

'            For Each key As String In RxAxis.Keys
'                If Not RxAxis(key)(4) = RxAxis(key)(0) Then
'                    If RxAxis(key)(0) > (RxAxis(key)(1) + (RxAxis(key)(3) * (DeadZone / 100))) Then
'                        RaiseEvent _KeyReleased(key & "-")
'                        RaiseEvent _KeyPressed(key & "+")

'                    ElseIf RxAxis(key)(0) < (RxAxis(key)(1) + (RxAxis(key)(2) * (DeadZone / 100))) Then
'                        RaiseEvent _KeyReleased(key & "+")
'                        RaiseEvent _KeyPressed(key & "-")

'                    Else
'                        RaiseEvent _KeyReleased(key & "-")
'                        RaiseEvent _KeyReleased(key & "+")
'                    End If
'                End If
'                RxAxis(key)(4) = RxAxis(key)(0)
'            Next

'            If NeedConfigReload Then
'                ReloadConfigs()
'            End If

'            For i = 0 To RxButtons.Count - 1
'                LF_RxButtons(i) = RxButtons(i)
'            Next

'        End While

'    End Sub

'    Public Sub KeyPressed(Button As String) Handles Me._KeyPressed
'        TranslateButtonToKey(Button, True)
'    End Sub

'    Public Sub KeyReleased(Button As String) Handles Me._KeyReleased
'        TranslateButtonToKey(Button, False)
'    End Sub

'    Private Sub FastforwardReplayToggle()
'        MainFormRef.NullDCLauncher.FastForward()
'    End Sub

'    Private Sub TranslateButtonToKey(Button As String, Down As Boolean)
'        ' If We're rebinding then don't fire anything, let it handle it
'        If MainFormRef.KeyMappingForm.Rebinding Then Exit Sub
'        Dim upordown = 2
'        If Down Then upordown = 0
'        If KeyCache.ContainsKey(Button) Then
'            If KeyCache(Button)(1) = Down Then Exit Sub

'            For Each Key As Int16 In KeyCache(Button)(0)
'                keybd_event(Key, MapVirtualKey(Key, 0), upordown, 0)
'            Next

'            KeyCache(Button)(1) = Down
'        End If


'    End Sub

'    Private Function GetKeyFromChar(c As Char) As Keys
'        Dim vkKeyCode As Short = VkKeyScanExW(c, InputLanguage.CurrentInputLanguage.Handle)
'        Return CType((((vkKeyCode And &HFF00) << 8) Or (vkKeyCode And &HFF)), Keys)
'    End Function

'    Public Sub UpdateAxisMap(NewMap As Dictionary(Of String, Array))
'        RxAxis = NewMap
'        WriteXMLConfigFile()
'        NeedConfigReload = True
'        Console.WriteLine("InputHandling:UpdateAxisMap")
'    End Sub

'End Class

'Public Class KeyBind

'    Public Name As String = ""
'    Public Button As String
'    Public Rebind As Array
'    Public Pressed As Boolean

'    Public Sub New(_Name As String, _Button As String, _Rebind As Array)
'        Name = _Name
'        Button = _Button
'        Rebind = _Rebind
'    End Sub

'End Class