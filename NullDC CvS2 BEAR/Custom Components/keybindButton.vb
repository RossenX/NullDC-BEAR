Imports System.Runtime.InteropServices

Public Class keybindButton
    Inherits Button


    Dim KC As String() = {"k0", "k0"}
    Public _configString As String = ""
    Public _emulator As String = ""
    Public _locked As Boolean = False
    Public _KeyDefaults As String() = {"k0,k0,k0", "k0,k0,k0"}

    Public Property KeyDefaults() As String()
        Get
            Return _KeyDefaults
        End Get

        Set(ByVal value As String())
            _KeyDefaults = value

        End Set

    End Property

    Public Property KeyLocked() As Boolean
        Get
            Return _locked
        End Get
        Set(ByVal value As Boolean)
            _locked = value
        End Set
    End Property

    Public Property KeyCode() As String()
        Get
            Return KC
        End Get
        Set(ByVal value As String())
            KC = value
        End Set
    End Property

    Public Property ConfigString() As String
        Get
            Return _configString
        End Get
        Set(ByVal value As String)
            _configString = value
        End Set

    End Property

    Public Property Emu() As String
        Get
            Return _emulator
        End Get
        Set(ByVal value As String)
            _emulator = value
        End Set

    End Property

    <DllImport("user32.dll", CharSet:=CharSet.Auto)>
    Public Shared Function MapVirtualKeyEx(ByVal uCode As Integer, ByVal nMapType As Integer, ByVal dwhkl As Integer) As Integer
    End Function

    <DllImport("user32.dll", CharSet:=CharSet.Auto, ExactSpelling:=True)>
    Public Shared Function GetKeyboardLayout(ByVal dwLayout As Integer) As Integer
    End Function

    Public Sub ResetToDefault(Optional ByVal _type As String = "", Optional ByVal _player As Int16 = -1)
        Dim p1Keycode = ""
        Dim p2Keycode = ""

        If Not KeyDefaults.Count = 2 Then
            p1Keycode = "k0"
            p2Keycode = "k0"
        Else
            If KeyDefaults(0).Split(",").Count < 3 Or KeyDefaults(1).Split(",").Count < 3 Then
                p1Keycode = "k0"
                p2Keycode = "k0"
            Else
                Select Case _type
                    Case "gamepad"
                        p1Keycode = KeyDefaults(0).Split(",")(0)
                        p2Keycode = KeyDefaults(1).Split(",")(0)

                    Case "fightstick"
                        p1Keycode = KeyDefaults(0).Split(",")(1)
                        p2Keycode = KeyDefaults(1).Split(",")(1)

                    Case "keyboard"
                        p1Keycode = KeyDefaults(0).Split(",")(2)
                        p2Keycode = KeyDefaults(1).Split(",")(2)
                    Case ""
                        p1Keycode = KeyDefaults(0).Split(",")(0)
                        p2Keycode = KeyDefaults(1).Split(",")(2)
                    Case Else
                        MsgBox("Reset to <" & _type & "> not handled")
                End Select
            End If
        End If



        Select Case _player
            Case -1
                KC(0) = p1Keycode
                KC(1) = p2Keycode
                UpdateTextFromPortID(0)

            Case 1
                KC(0) = p1Keycode
                UpdateTextFromPortID(_player - 1)

            Case 2
                KC(1) = p2Keycode
                UpdateTextFromPortID(_player - 1)

        End Select



    End Sub

    Public Function GetKeyCode(ByVal _port) As String
        If Not ConfigString.Contains("<port>") And Emu = "mednafen" And Not _port = 0 Then
            Return KC(0)
        End If

        Return KC(_port)

    End Function

    Public Sub SetKeyCode(ByVal _kc As String, ByVal _port As Int16)
        If Not ConfigString.Contains("<port>") And Emu = "mednafen" And Not _port = 0 Then
            KC(_port) = "k0"
            Exit Sub
        End If

        ' It's all numeric, so must be a normal keyboard code
        If _kc.StartsWith("k") Then
            If _kc = "k27" Then
                Text = "None"
                _kc = "k0"
            Else
                Text = KeyCodeToKeyboardButton(_kc)
            End If
        Else
            Text = _kc
        End If

        KC(_port) = _kc

    End Sub

    Public Sub UpdateTextFromPortID(ByVal current_port As Int16)

        If Not ConfigString.Contains("<port>") And Emu = "mednafen" And Not current_port = 0 Then
            current_port = 0
        End If

        If KC(current_port).StartsWith("k") Then
            If KC(current_port) = "k0" Then
                Text = "None"
            Else
                Text = KeyCodeToKeyboardButton(KC(current_port))
            End If

        Else
            Text = KC(current_port)
        End If

        'Console.WriteLine(KC(current_port))

    End Sub

    Private Function KeyCodeToKeyboardButton(ByVal _kc As String) As String
        If _kc Is Nothing Or _kc = "" Then Return "None"

        Dim _converted = Convert.ToChar(MapVirtualKeyEx(_kc.Substring(1, _kc.Length - 1), 2, GetKeyboardLayout(0))).ToString

        If _converted = vbNullChar Or _converted = vbCr Or _converted = vbTab Or _converted = " " Then
            _converted = CType(KeyCon.ConvertFromString(_kc.Substring(1, _kc.Length - 1)), Keys).ToString
        End If

        If _converted = vbBack Then
            Return "Backspace"
        End If

        Return _converted 'Convert.ToChar(MapVirtualKeyEx(_kc.Substring(1, _kc.Length - 1), 2, GetKeyboardLayout(0))).ToString

    End Function

End Class
