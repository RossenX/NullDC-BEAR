'Imports System.Runtime.InteropServices
'Imports System.Threading
'Imports System.Timers


'Public Class frmCalibration

'    Dim CalibrationThread As Threading.Thread
'    ' Current, Idle, Min, Max, Last Frame
'    Dim AxixData As New Dictionary(Of String, Array)
'    Dim PoVRestInit As Decimal = 0

'    Public Sub New(ByRef _mainwindow As frmMain)
'        InitializeComponent()

'        AxixData.Add("x", {0, 0, 0, 0, 0})
'        AxixData.Add("y", {0, 0, 0, 0, 0})
'        AxixData.Add("z", {0, 0, 0, 0, 0})
'        AxixData.Add("t", {0, 0, 0, 0, 0})
'        AxixData.Add("u", {0, 0, 0, 0, 0})
'        AxixData.Add("v", {0, 0, 0, 0, 0})
'    End Sub

'    Private Sub frmCalibration_Load(sender As Object, e As EventArgs) Handles MyBase.Load
'        Me.Icon = My.Resources.NewNullDCBearIcon
'    End Sub

'    Public Sub CalibrationLoop()
'        While Me.Visible
'            Dim tempAxis = MainformRef.InputHandler.RxAxis
'            For Each Key In tempAxis.Keys
'                If tempAxis(Key)(0) > AxixData(Key)(3) Then AxixData(Key)(3) = tempAxis(Key)(0) ' If it's more than 'max' then set it as new max
'                If tempAxis(Key)(0) < AxixData(Key)(2) Then AxixData(Key)(2) = tempAxis(Key)(0) ' If it's more than 'min' then set it as new max
'            Next

'            UpdateSliders()
'            Thread.Sleep(8)
'        End While
'    End Sub

'    Private Function Normalize(val As Decimal, min As Decimal, max As Decimal) As Int32
'        If val = 0 And max = 0 And min = 0 Then Return 0
'        Return (val - min) / (max - min) * 100
'    End Function

'    Private Sub UpdateSliders()
'        Try
'            For Each key In AxixData.Keys
'                Dim tempAxis = MainformRef.InputHandler.RxAxis
'                Dim tmpCTRL As SimpleProgressBar = Me.Controls.Find("Bar" & key.ToUpper, True)(0)
'                Dim INVOKATION As SimpleProgressBar.setValue_Delegate = AddressOf tmpCTRL.SetValue
'                Try
'                    If tempAxis(key)(0) > (AxixData(key)(1) + (AxixData(key)(3) * (MainformRef.InputHandler.DeadZone / 100))) Or
'                        tempAxis(key)(0) < (AxixData(key)(1) + (AxixData(key)(2) * (MainformRef.InputHandler.DeadZone / 100))) Then

'                        Dim BarValue = Normalize(tempAxis(key)(0), AxixData(key)(2), AxixData(key)(3))
'                        tmpCTRL.Invoke(INVOKATION, BarValue)

'                    Else
'                        Dim BarValue = Normalize(AxixData(key)(1), AxixData(key)(2), AxixData(key)(3))
'                        tmpCTRL.Invoke(INVOKATION, BarValue)
'                    End If
'                Catch ex As Exception
'                    tmpCTRL.Invoke(INVOKATION, 0)
'                End Try
'            Next
'        Catch ex As Exception
'        End Try
'    End Sub

'    Private Sub btnDone_Click(sender As Object, e As EventArgs) Handles btnDone.Click
'        Me.Close()
'    End Sub

'    Private Sub frmCalibration_VisibleChanged(sender As Object, e As EventArgs) Handles MyBase.VisibleChanged
'        If Me.Visible Then
'            Console.WriteLine("Calibrator Visible. Deadzone: {0}", MainformRef.InputHandler.DeadZone)
'            PoVRestInit = MainformRef.InputHandler.PoV
'            Dim tempAxis = MainformRef.InputHandler.RxAxis
'            trbDeadZone.Value = MainformRef.InputHandler.DeadZone
'            lbDeadZone.Text = trbDeadZone.Value

'            AxixData("x") = {0, tempAxis("x")(0), 0, 0, 0}
'            AxixData("y") = {0, tempAxis("y")(0), 0, 0, 0}
'            AxixData("z") = {0, tempAxis("z")(0), 0, 0, 0}
'            AxixData("t") = {0, tempAxis("t")(0), 0, 0, 0}
'            AxixData("u") = {0, tempAxis("u")(0), 0, 0, 0}
'            AxixData("v") = {0, tempAxis("v")(0), 0, 0, 0}

'            CalibrationThread = New Thread(AddressOf CalibrationLoop)
'            CalibrationThread.IsBackground = True
'            CalibrationThread.Start()
'        Else
'            MainformRef.InputHandler.PoVRest = PoVRestInit
'            MainformRef.InputHandler.DeadZone = trbDeadZone.Value
'            MainformRef.InputHandler.UpdateAxisMap(AxixData)
'            MainformRef.InputHandler.NeedConfigReload = True
'        End If
'    End Sub

'    Private Sub frmCalibration_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
'        e.Cancel = True
'        Me.Visible = False
'    End Sub

'    Private Sub TrackBar1_Scroll(sender As Object, e As EventArgs) Handles trbDeadZone.Scroll
'        Console.WriteLine("TrackBarChange: {0}", trbDeadZone.Value)
'        lbDeadZone.Text = trbDeadZone.Value
'        MainformRef.InputHandler.DeadZone = trbDeadZone.Value
'    End Sub
'End Class