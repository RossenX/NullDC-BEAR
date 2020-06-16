Imports System.Threading
Imports System.Xml

Public Class frmKeyMapping


    Dim KeyToBind As String = ""
    Dim WaitingForInput As Boolean = False
    Public Rebinding As Boolean = False
    Dim KeyToRebind As String = ""
    Public KeybindConfigs As ArrayList = New ArrayList
    Dim CalibrationForm As frmCalibration
    Dim MainformRef As frmMain
    Dim ControllerID As Int32 = 0

    Dim InitSetupDone As Boolean = False

    Public Sub New(ByRef MainForm)
        InitializeComponent()
        MainformRef = MainForm
        CalibrationForm = New frmCalibration(MainForm)
    End Sub

    Private Sub StartKeyBind(_KeyToBind As String)
        KeyToRebind = _KeyToBind
        Rebinding = True
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

#End Region

    Private Sub frmKeyMapping_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.NewNullDCBearIcon
        AddHandler MainformRef.InputHandler._KeyPressed, AddressOf KeyPressed
        GetAllKeybinds()
        cbControllerID.SelectedIndex = ControllerID
        InitSetupDone = True
    End Sub

    Public Sub KeyPressed(Button As String)
        'Check if rebinding
        If Rebinding Then
            Console.WriteLine("Rebind Key Pressed")
            'Read configs
            GetAllKeybinds()
            Dim i As Int16
            For Each KB As KeyBind In KeybindConfigs
                If KB.Name = KeyToRebind Then
                    If Not KB.Button = Button Then
                        KeybindConfigs(i).Button = Button
                    End If
                ElseIf KB.Button = Button Then
                    KeybindConfigs(i).Button = ""
                End If

                i += 1
            Next
            WriteXMLConfigFile()
            Rebinding = False
            MainformRef.InputHandler.NeedConfigReload = True
        End If

    End Sub

    Private Sub GetAllKeybinds()

        Dim cfg As XDocument = XDocument.Load(MainformRef.NullDCPath & "\KeyMapReBinds.xml")
        KeybindConfigs.Clear()
        For Each node As XElement In cfg.<Configs>.<KeyMap>.Nodes
            KeybindConfigs.Add(New KeyBind(node.Name.ToString, node.<button>.Value, node.<rebind>.Value))
        Next
        ControllerID = cfg.<Configs>.<ControllerID>.Value

    End Sub

    Public Sub WriteXMLConfigFile()
        Dim writer As New XmlTextWriter(MainformRef.NullDCPath & "\KeyMapReBinds.xml", System.Text.Encoding.UTF8)
        writer.WriteStartDocument(True)
        writer.Formatting = Formatting.Indented
        writer.Indentation = 2
        writer.WriteStartElement("Configs")                 ' Config Start
        writer.WriteStartElement("ControllerID")                'ControllerID
        writer.WriteString(ControllerID)                            ' Set
        writer.WriteEndElement()                                'End

        writer.WriteStartElement("KeyMap")                      ' Keymap
        For Each Rebind As KeyBind In KeybindConfigs                ' Set
            WriteKeyCodeXML(Rebind, writer)
        Next
        writer.WriteEndElement()                                'End

        writer.WriteStartElement("AxisMap")                     ' Axis Map
        For Each key As String In MainformRef.InputHandler.RxAxis.Keys
            CreateAxisXMLEntry(key, MainformRef.InputHandler.RxAxis(key)(2), MainformRef.InputHandler.RxAxis(key)(3), MainformRef.InputHandler.RxAxis(key)(1), writer)
        Next
        writer.WriteEndElement()                                ' End

        writer.WriteStartElement("PoV")
        writer.WriteString(MainformRef.InputHandler.PoVRest)
        writer.WriteEndElement()

        writer.WriteStartElement("DeadZone")
        writer.WriteString(MainformRef.InputHandler.DeadZone)
        writer.WriteEndElement()

        writer.WriteEndElement()                            ' Config End
        writer.WriteEndDocument()
        writer.Close()

    End Sub

    Private Sub WriteKeyCodeXML(key As KeyBind, ByRef writer As XmlTextWriter)
        writer.WriteStartElement(key.Name)

        writer.WriteStartElement("button")
        writer.WriteString(key.Button)
        writer.WriteEndElement()

        writer.WriteStartElement("rebind")
        writer.WriteString(key.Rebind)
        writer.WriteEndElement()
        writer.WriteEndElement()
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
    Private Sub frmKeyMapping_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If Rebinding Then Exit Sub
        Dim kc As New KeysConverter

        Dim ButtonToChange As Button = Nothing
        For Each key As KeyBind In MainformRef.InputHandler.KeybindConfigs
            If key.Rebind.ToLower = kc.ConvertToString(e.KeyCode).ToLower Then
                ButtonToChange = Me.Controls.Find("btn" & key.Name, True)(0)
            End If
        Next



        If Not ButtonToChange Is Nothing Then
            ButtonToChange.BackColor = Color.Green
        End If

    End Sub

    Private Sub frmKeyMapping_KeyUp(sender As Object, e As KeyEventArgs) Handles MyBase.KeyUp
        Dim kc As New KeysConverter

        Dim ButtonToChange As Button = Nothing
        For Each key As KeyBind In MainformRef.InputHandler.KeybindConfigs
            If key.Rebind.ToLower = kc.ConvertToString(e.KeyCode).ToLower Then
                ButtonToChange = Me.Controls.Find("btn" & key.Name, True)(0)
            End If
        Next

        If Not ButtonToChange Is Nothing Then
            ButtonToChange.BackColor = Color.White
        End If

    End Sub

    Private Sub btnCalibrate_Click(sender As Object, e As EventArgs) Handles btnCalibrate.Click
        MsgBox("Make sure your stick is in neurtral before starting calibration")
        CalibrationForm.Show()
    End Sub

    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles Button1.Click
        MsgBox("1. Click Calibrate." & vbCrLf & "2. Click a button in this window, click a button on your controller/stick." & vbCrLf & "3. Do that for all the controls." & vbCrLf & "4. Move/Click your controller, conresponding buttons will glow green, " & vbCrLf & "just make sure to bind them all first. Double inputs will not be recognized, like if you put X as both LK and HK, it won't click em both.")
    End Sub

    Private Sub Button2_Click_1(sender As Object, e As EventArgs) Handles Button2.Click
        MsgBox("NullDC has terrible input plugins, so bassicly this will map your controller to a keyboard key. WASD for movement and uio(punches) jkl(kicks) 1(start) 3(coins). If you don't have a controller you can use those keys to play")
    End Sub

    Private Sub Button3_Click_1(sender As Object, e As EventArgs) Handles Button3.Click
        MsgBox("Because Controllers")
    End Sub

    Private Sub btnDone_Click(sender As Object, e As EventArgs) Handles btnDone.Click
        Me.Close()
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

    Private Sub Panel1_Paint(sender As Object, e As PaintEventArgs) Handles Panel1.Paint
        If MainformRef.ConfigFile.UseRemap Then
            btnOnOff.BackColor = Color.Green
            btnOnOff.Text = "On"
        Else
            btnOnOff.BackColor = Color.Red
            btnOnOff.Text = "Off"
        End If
    End Sub

    Private Sub cbControllerID_SelectedIndexChanged(sender As ComboBox, e As EventArgs) Handles cbControllerID.SelectedIndexChanged
        If InitSetupDone = True Then
            GetAllKeybinds()
            ControllerID = cbControllerID.Text
            WriteXMLConfigFile()
            MainformRef.InputHandler.NeedConfigReload = True
        End If

    End Sub

    Private Sub frmKeyMapping_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        e.Cancel = True
        Me.Visible = False

    End Sub

    Private Sub Button4_Click_1(sender As Object, e As EventArgs) Handles Button4.Click
        frm360ce.Show()
    End Sub

    Private Sub btnUnbindAll_Click(sender As Object, e As EventArgs) Handles btnUnbindAll.Click
        Dim Result = MessageBox.Show("This will UNBIND all BEAR keys, so you can use an external program for mapping", "DECLAW", MessageBoxButtons.YesNo)
        If Result = DialogResult.Yes Then
            For Each i As KeyBind In KeybindConfigs
                i.Button = ""
            Next
            WriteXMLConfigFile()
            MainformRef.InputHandler.NeedConfigReload = True
        End If
    End Sub

End Class