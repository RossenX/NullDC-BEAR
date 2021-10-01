Imports System.IO
Imports SDL2.SDL

Public Class frmMappingString

    Private Sub frmMappingString_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.fan_icon_text
        CenterToParent()
        Dim _currentJoyStickIndex As Int16 = frmKeyMapperSDL.ControllerCB.SelectedValue

        RichTextBox1.Text = SDL_GameControllerMappingForGUID(SDL_JoystickGetDeviceGUID(_currentJoyStickIndex))

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim _SDL = RichTextBox1.Text.Trim

        Dim _FoundExisting = False
        Dim _FoundIndex = 0
        Dim _File As String() = {}

        ' Naomi
        If File.Exists(MainformRef.NullDCPath & "\bearcontrollerdb.txt") Then
            _File = File.ReadAllLines(MainformRef.NullDCPath & "\bearcontrollerdb.txt")
            For Each _line In _File
                If _line.StartsWith(_SDL.Split(",")(0)) Then
                    _FoundExisting = True
                    _File(_FoundIndex) = _SDL
                    Exit For
                End If
                _FoundIndex += 1
            Next

            If _FoundExisting Then
                File.WriteAllLines(MainformRef.NullDCPath & "\bearcontrollerdb.txt", _File)
            Else
                File.AppendAllLines(MainformRef.NullDCPath & "\bearcontrollerdb.txt", {_SDL})
            End If

        Else
            File.WriteAllLines(MainformRef.NullDCPath & "\bearcontrollerdb.txt", {_SDL})

        End If

        ' Dreamcast
        _FoundIndex = 0
        If File.Exists(MainformRef.NullDCPath & "\dc\bearcontrollerdb.txt") Then
            _File = File.ReadAllLines(MainformRef.NullDCPath & "\dc\bearcontrollerdb.txt")
            For Each _line In _File
                If _line.StartsWith(_SDL.Split(",")(0)) Then
                    _FoundExisting = True
                    _File(_FoundIndex) = _SDL
                    Exit For
                End If
                _FoundIndex += 1
            Next

            If _FoundExisting Then
                File.WriteAllLines(MainformRef.NullDCPath & "\dc\bearcontrollerdb.txt", _File)
            Else
                File.AppendAllLines(MainformRef.NullDCPath & "\dc\bearcontrollerdb.txt", {_SDL})
            End If

        Else
            File.WriteAllLines(MainformRef.NullDCPath & "\dc\bearcontrollerdb.txt", {_SDL})

        End If

        SDL2.SDL.SDL_GameControllerAddMapping(_SDL)
        frmKeyMapperSDL.NaomiChanged = True
        frmKeyMapperSDL.DreamcastChanged = True
        frmKeyMapperSDL.MednafenChanged = True
        Me.Close()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.Close()
    End Sub

End Class