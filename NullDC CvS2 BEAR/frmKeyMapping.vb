Imports System.IO
Imports System.Threading
Imports System.Xml

Public Class frmKeyMapping

    Dim CalibrationForm As frmCalibration
    Dim RebindAllThread As Thread

    Public Rebinding As Boolean = False
    Dim WaitingForRelease As Boolean = False
    Dim LastButtonBound As String = ""
    Dim KeyToRebind As String = ""
    Public LoadingSettings As Boolean = False

    Public Sub New(ByRef MainForm)
        InitializeComponent()
        CalibrationForm = New frmCalibration(MainForm)
        Me.Show()
        Me.Hide()
    End Sub

    Private Sub StartKeyBind(_KeyToBind As String)
        If Rebinding Then
            Me.Controls.Find("btn" & KeyToRebind, True)(0).BackColor = Color.White
        End If

        KeyToRebind = _KeyToBind
        Rebinding = True
        WaitingForRelease = False
        Me.Controls.Find("btn" & _KeyToBind, True)(0).BackColor = Color.Red

    End Sub

#Region "Buttons"

    Private Sub btnLP_Click(sender As Object, e As EventArgs) Handles btnlp.Click
        StartKeyBind("LP")
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles btnup.Click
        StartKeyBind("up")
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles btnright.Click
        StartKeyBind("right")
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles btndown.Click
        StartKeyBind("down")
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles btnleft.Click
        StartKeyBind("left")
    End Sub

    Private Sub btnMP_Click(sender As Object, e As EventArgs) Handles btnmp.Click
        StartKeyBind("MP")
    End Sub

    Private Sub btnHP_Click(sender As Object, e As EventArgs) Handles btnhp.Click
        StartKeyBind("HP")
    End Sub

    Private Sub btnLK_Click(sender As Object, e As EventArgs) Handles btnlk.Click
        StartKeyBind("LK")
    End Sub

    Private Sub btnMK_Click(sender As Object, e As EventArgs) Handles btnmk.Click
        StartKeyBind("MK")
    End Sub

    Private Sub btnHK_Click(sender As Object, e As EventArgs) Handles btnhk.Click
        StartKeyBind("HK")
    End Sub

    Private Sub Button25_Click(sender As Object, e As EventArgs) Handles btncoin.Click
        StartKeyBind("coin")
    End Sub

    Private Sub Button27_Click(sender As Object, e As EventArgs) Handles btnstart.Click
        StartKeyBind("start")
    End Sub

    Private Sub btnLPLK_Click(sender As Object, e As EventArgs) Handles btnLPLK.Click
        StartKeyBind("LPLK")
    End Sub

    Private Sub btnHPHK_Click(sender As Object, e As EventArgs) Handles btnHPHK.Click
        StartKeyBind("HPHK")
    End Sub

    Private Sub btnMPMK_Click(sender As Object, e As EventArgs) Handles btnMPMK.Click
        StartKeyBind("MPMK")
    End Sub

    Private Sub btnAP_Click(sender As Object, e As EventArgs) Handles btnAP.Click
        StartKeyBind("AP")
    End Sub

    Private Sub btnAK_Click(sender As Object, e As EventArgs) Handles btnAK.Click
        StartKeyBind("AK")
    End Sub

    Private Sub btnLPMP_Click(sender As Object, e As EventArgs) Handles btnLPMP.Click
        StartKeyBind("LPMP")
    End Sub

    Private Sub btnMPHP_Click(sender As Object, e As EventArgs) Handles btnMPHP.Click
        StartKeyBind("MPHP")
    End Sub

    Private Sub btnLKMK_Click(sender As Object, e As EventArgs) Handles btnLKMK.Click
        StartKeyBind("LKMK")
    End Sub

    Private Sub btnMKHK_Click(sender As Object, e As EventArgs) Handles btnMKHK.Click
        StartKeyBind("MKHK")
    End Sub

#End Region

    Private Sub frmKeyMapping_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.NewNullDCBearIcon
        AddHandler MainformRef.InputHandler._KeyPressed, AddressOf KeyPressed
        AddHandler MainformRef.InputHandler._KeyReleased, AddressOf KeyReleased
        ActiveControl = Nothing

    End Sub

    Public Sub KeyReleased(Button As String)
        If LastButtonBound = Button And WaitingForRelease Then
            Rebinding = False
            WaitingForRelease = False
            Me.Controls.Find("btn" & KeyToRebind, True)(0).BackColor = Color.White
        End If
    End Sub

    Public Sub KeyPressed(Button As String)

        'Check if rebinding
        If Rebinding And Not WaitingForRelease Then
            LastButtonBound = Button

            ' Edit the Config with new Key, ez pz
            Dim cfg As XDocument = XDocument.Load(MainformRef.NullDCPath & "\" & MainformRef.InputHandler.GetXMLFile)
            ' Check if button exists, if it does make it nothing
            For Each a In cfg.Descendants("button")
                If a.Value = Button Then a.Value = ""
            Next

            cfg.Descendants(KeyToRebind).<button>.Value = Button
            cfg.Save(MainformRef.NullDCPath & "\" & MainformRef.InputHandler.GetXMLFile)

            WaitingForRelease = True

            Me.Controls.Find("btn" & KeyToRebind, True)(0).BackColor = Color.Green

            MainformRef.InputHandler.NeedConfigReload = True

        End If

    End Sub

    Private Sub frmKeyMapping_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If Rebinding Then Exit Sub
        Dim kc As New KeysConverter

        Dim ButtonToChange As Button = Nothing
        For Each key As KeyBind In MainformRef.InputHandler.KeybindConfigs
            If key.Rebind.Length > 1 Then Continue For
            If key.Rebind(0) = e.KeyCode Then
                ButtonToChange = Me.Controls.Find("btn" & key.Name, True)(0)
            End If
        Next

        MsgBox(e.KeyValue)
        If Not ButtonToChange Is Nothing Then
            ButtonToChange.BackColor = Color.Green
        End If

    End Sub

    Private Sub frmKeyMapping_KeyUp(sender As Object, e As KeyEventArgs) Handles MyBase.KeyUp
        Dim kc As New KeysConverter

        Dim ButtonToChange As Button = Nothing
        For Each key As KeyBind In MainformRef.InputHandler.KeybindConfigs
            If key.Rebind.Length > 1 Then Continue For
            If key.Rebind(0) = e.KeyCode Then
                ButtonToChange = Me.Controls.Find("btn" & key.Name, True)(0)
            End If
        Next

        If Not ButtonToChange Is Nothing Then
            ButtonToChange.BackColor = Color.White
        End If

    End Sub

    Private Sub btnCalibrate_Click(sender As Object, e As EventArgs) Handles btnCalibrate.Click
        MsgBox("Make sure your stick is in neurtral before starting calibration")
        CalibrationForm.ShowDialog()
    End Sub

    Private Sub btnDone_Click(sender As Object, e As EventArgs) Handles btnDone.Click
        Me.Close()
        MainformRef.Focus()
    End Sub

    Private Sub btnOnOff_Click(sender As Object, e As EventArgs) Handles btnOnOff.Click
        If btnOnOff.Text = "On" Then
            MainformRef.InputHandler.TurnOnOff(False)
            btnOnOff.BackColor = Color.Red
            btnOnOff.Text = "Off"
            MainformRef.ConfigFile.UseRemap = False
        Else
            MainformRef.InputHandler.TurnOnOff(True)
            btnOnOff.BackColor = Color.Green
            btnOnOff.Text = "On"
            MainformRef.ConfigFile.UseRemap = True
        End If
        MainformRef.ConfigFile.SaveFile()
    End Sub

    Private Sub cbControllerID_SelectedIndexChanged(sender As ComboBox, e As EventArgs) Handles cbControllerID.SelectedIndexChanged
        If Not LoadingSettings Then
            Dim cfg As XDocument = XDocument.Load(MainformRef.NullDCPath & "\" & MainformRef.InputHandler.GetXMLFile)
            cfg.<Configs>.<ControllerID>.Value = cbControllerID.Text
            cfg.Save(MainformRef.NullDCPath & "\" & MainformRef.InputHandler.GetXMLFile)
            MainformRef.InputHandler.NeedConfigReload = True

        End If

    End Sub

    Private Sub frmKeyMapping_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        e.Cancel = True
        Me.Visible = False

    End Sub

    Private Sub btnUnbindAll_Click(sender As Object, e As EventArgs) Handles btnUnbindAll.Click
        Dim Result = MessageBox.Show("This will UNBIND all BEAR keys, so you can use an external program for mapping", "DECLAW", MessageBoxButtons.YesNo)
        If Result = DialogResult.Yes Then
            UnbindAll()
        End If
    End Sub

    Private Sub UnbindAll()

        Dim cfg As XDocument = XDocument.Load(MainformRef.NullDCPath & "\" & MainformRef.InputHandler.GetXMLFile)
        For Each a In cfg.Descendants("button")
            a.Value = ""
        Next
        cfg.Save(MainformRef.InputHandler.GetXMLFile(True))

        MainformRef.InputHandler.NeedConfigReload = True

    End Sub

    Private Sub frmKeyMapping_VisibleChanged(sender As Object, e As EventArgs) Handles MyBase.VisibleChanged
        If Me.Visible Then
            UpdateProfileList()

            If MainformRef.ConfigFile.UseRemap Then
                btnOnOff.BackColor = Color.Green
                btnOnOff.Text = "On"
            Else
                btnOnOff.BackColor = Color.Red
                btnOnOff.Text = "Off"
            End If
            Me.CenterToParent()

        End If
    End Sub

    Public Sub UpdateProfileList()
        LoadingSettings = True

        Dim files() As String = Directory.GetFiles(MainformRef.NullDCPath)
        Dim Profiles As New ArrayList

        For Each file In files
            Dim FileName As String = file.Split("\")(file.Split("\").Count - 1).Split(".")(0)
            If FileName.StartsWith("KeyMapReBinds_") Then Profiles.Add(FileName.Split("_")(1))
        Next

        cbProfile.Items.Clear()
        cbProfile.Items.Add("Default")
        Dim ProfileIndex = 1
        Dim SelectedProfile = 0
        For Each profile As String In Profiles
            cbProfile.Items.Add(profile)
            If profile = MainformRef.InputHandler.ProfileName Then SelectedProfile = ProfileIndex
            ProfileIndex += 1
        Next
        cbProfile.SelectedIndex = SelectedProfile
        cbControllerID.SelectedIndex = MainformRef.InputHandler.ControllerID

        LoadingSettings = False

    End Sub

    Private Sub btnQuickSetup_Click(sender As Object, e As EventArgs) Handles btnQuickSetup.Click
        If Not RebindAllThread Is Nothing Then
            If RebindAllThread.IsAlive Then
                RebindAllThread.Abort()
                While RebindAllThread.IsAlive
                    Thread.Sleep(50)
                End While
            End If
        End If
        RebindAllThread = New Thread(AddressOf StartQuickSetup)
        RebindAllThread.IsBackground = True
        RebindAllThread.Start()

    End Sub

    Private Sub StartQuickSetup()
        UnbindAll()
        CalibrationForm.ShowDialog()
        Dim ButtonsToRebind() As String = {"up", "down", "left", "right", "LP", "MP", "HP", "LK", "MK", "HK", "coin", "start", "dpadup", "dpaddown", "dpadleft", "dpadright"}
        For Each Key As String In ButtonsToRebind
            StartKeyBind(Key)

            Dim DoneBinding As Boolean = False
            While Not DoneBinding
                If Rebinding = False And WaitingForRelease = False Then DoneBinding = True
                Thread.Sleep(10)
            End While
        Next

    End Sub

    Private Sub cbProfile_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbProfile.SelectedIndexChanged
        If Not LoadingSettings Then ChangeProfile()

    End Sub

    Private Sub ChangeProfile()
        MainformRef.ConfigFile.KeyMapProfile = cbProfile.Text
        MainformRef.ConfigFile.SaveFile()
        MainformRef.InputHandler.NeedConfigReload = True

    End Sub

    Private Sub btnNewProfile_Click(sender As Object, e As EventArgs) Handles btnNewProfile.Click
        Dim NewProfileFrame = New frmNewProfile(MainformRef)
        NewProfileFrame.Show(Me)
    End Sub

    Private Sub btnDeleteProfile_Click(sender As Object, e As EventArgs) Handles btnDeleteProfile.Click
        If cbProfile.Text = "Default" Then
            MainformRef.NotificationForm.ShowMessage("Cannot Delete Default")
            Exit Sub
        End If

        File.SetAttributes(MainformRef.InputHandler.GetXMLFile(True), FileAttributes.Normal)
        File.Delete(MainformRef.InputHandler.GetXMLFile(True))
        cbProfile.Items.Remove(cbProfile.SelectedItem)
        cbProfile.SelectedIndex = 0
    End Sub

    Public Sub ClearAllButtons()

        For Each KeyBoardThing In MainformRef.InputHandler.KeyBoardConfigs
            Me.Controls.Find("btn" & KeyBoardThing.Key, True)(0).BackColor = Color.White
        Next

    End Sub

    Private Sub cbProfile_KeyPress(sender As Object, e As KeyPressEventArgs) Handles cbProfile.KeyPress
        e.Handled = True
    End Sub

    Private Sub btndup_Click(sender As Object, e As EventArgs) Handles btndpadup.Click
        StartKeyBind("dpadup")
    End Sub

    Private Sub btndpadright_Click(sender As Object, e As EventArgs) Handles btndpadright.Click
        StartKeyBind("dpadright")
    End Sub

    Private Sub btndpadleft_Click(sender As Object, e As EventArgs) Handles btndpadleft.Click
        StartKeyBind("dpadleft")
    End Sub

    Private Sub btndpaddown_Click(sender As Object, e As EventArgs) Handles btndpaddown.Click
        StartKeyBind("dpaddown")
    End Sub

End Class