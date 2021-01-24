Imports System.Runtime.InteropServices

Public Class keybindButton
    Inherits Button


    Public KC As String() = {"k0", "k0"}
    Public _configString As String = ""
    Public _emulator As String = ""

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

    Public Sub New()
        MyBase.New
        InitializeComponent()

    End Sub

    Public Sub ChangeKeyCode(ByVal _kc As String, ByVal _port As Int16)

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
