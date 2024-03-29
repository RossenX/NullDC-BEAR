﻿Public Class frmNotification

    Dim HideTimer As Timer = New Timer
    Public Sub New(ByRef _mf As frmMain)
        InitializeComponent()
        HideTimer.Interval = 5000
        AddHandler HideTimer.Tick, AddressOf HideTimer_tick


    End Sub

    Private Sub HideTimer_tick(sender As Object, e As EventArgs)
        Visible = False
        HideTimer.Stop()
    End Sub

    Public Sub ShowMessage(ByVal msg As String)
        If MainformRef.InvokeRequired Then
            Me.Invoke(Sub()
                          Me.Hide()
                          Me.Show(MainformRef)
                          Label1.Text = msg
                      End Sub)
        Else
            Me.Hide()
            Me.Show(MainformRef)
            Label1.Text = msg
        End If


    End Sub

    Public Sub ShowMessageDialog(ByVal msg As String)
        If MainformRef.InvokeRequired Then
            Me.Invoke(Sub()
                          Me.Hide()
                          Label1.Text = msg
                          Me.ShowDialog(MainformRef)
                      End Sub)
        Else
            Me.Hide()
            Label1.Text = msg
            Me.ShowDialog(MainformRef)
        End If


    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        HideTimer.Stop()
        If MainformRef.Visible Then
            MainformRef.Focus()
        End If
        Me.Visible = False
    End Sub

    Private Sub frmChallengeSent_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        e.Cancel = True
        Me.Visible = False
    End Sub

    Private Sub frmNotification_VisibleChanged(sender As Object, e As EventArgs) Handles MyBase.VisibleChanged
        If Visible Then
            HideTimer.Start()
            Me.CenterToParent()
        Else
            HideTimer.Stop()
        End If
    End Sub

    Private Sub frmNotification_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.fan_icon_text
        ReloadTheme()


    End Sub

    Public Sub ReloadTheme()
        Me.BackgroundImage = BEARTheme.LoadImage(ThemeKeys.NotificationBackground)
        ApplyThemeToControl(Button1)
        ApplyThemeToControl(Label1, 3)

    End Sub

End Class