Imports System.IO

Public Class frmNewProfile
    Private Sub frmNewProfile_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.CenterToParent()
        TextBox1.Clear()
        TextBox1.Focus()

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If TextBox1.Text.ToLower.Trim = "default" Then
            MsgBox("Can't name the profile Default, that's just confusing dood.")
            Exit Sub
        End If

        Try
            MainformRef.ConfigFile.KeyMapProfile = TextBox1.Text.Trim()
            MainformRef.ConfigFile.SaveFile(False)
            File.WriteAllLines(MainformRef.NullDCPath & "\Controls_" & TextBox1.Text.Trim() & ".bear", {})
            frmKeyMapperSDL.LoadSettings()
        Catch ex As Exception
            MsgBox("Unable to create profile: " & ex.InnerException.Message)

        End Try

        Me.Close()

    End Sub

    Private Sub TextBox1_KeyPress(sender As TextBox, e As KeyPressEventArgs) Handles TextBox1.KeyPress
        If sender.Text.Length < 40 Then
            If Not (Asc(e.KeyChar) = 8) Then
                Dim allowedChars As String = "abcdefghijklmnopqrstuvwxyz1234567890_ "
                If Not allowedChars.Contains(e.KeyChar.ToString.ToLower) Or (Asc(e.KeyChar) = 8) Then
                    e.KeyChar = ChrW(0)
                    e.Handled = True

                End If
            End If
        Else
            e.KeyChar = ChrW(0)
            e.Handled = True
        End If



    End Sub

    Private Sub frmNewProfile_VisibleChanged(sender As Object, e As EventArgs) Handles MyBase.VisibleChanged
        Me.CenterToParent()
        TextBox1.Clear()
        TextBox1.Focus()

    End Sub

    Private Sub TextBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox1.KeyDown
        If e.KeyCode = Keys.Enter Then
            Button1_Click(sender, e)

        End If
    End Sub


End Class