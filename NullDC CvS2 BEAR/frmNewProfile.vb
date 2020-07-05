Public Class frmNewProfile

    Public Sub New(ByRef _mf As frmMain)
        InitializeComponent()
    End Sub

    Private Sub frmNewProfile_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.NewNullDCBearIcon
        Me.CenterToParent()

    End Sub

    Private Sub btnNew_Click(sender As Object, e As EventArgs) Handles btnNew.Click
        CreateNewProfile()

    End Sub

    Private Sub CreateNewProfile()
        MainformRef.ConfigFile.KeyMapProfile = tbProfileName.Text.Trim
        MainformRef.ConfigFile.SaveFile()
        MainformRef.InputHandler.NeedConfigReload = True
        Me.Close()
    End Sub
    Private Sub tbProfileName_KeyPress(sender As Object, e As KeyPressEventArgs) Handles tbProfileName.KeyPress
        If e.KeyChar = ChrW(Keys.Return) Then
            CreateNewProfile()
            Exit Sub
        End If

        If Not (Asc(e.KeyChar) = 8) Then
            Dim allowedChars As String = "abcdefghijklmnopqrstuvwxyz1234567890 "
            If Not allowedChars.Contains(e.KeyChar.ToString.ToLower) Or (Asc(e.KeyChar) = 8) Then
                e.KeyChar = ChrW(0)
                e.Handled = True
            End If
        End If
    End Sub
End Class