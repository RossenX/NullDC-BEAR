<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmCalibration
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.btnDone = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.trbDeadZone = New System.Windows.Forms.TrackBar()
        Me.lbDeadZone = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.BarX = New NullDC_CvS2_BEAR.SimpleProgressBar()
        Me.BarY = New NullDC_CvS2_BEAR.SimpleProgressBar()
        Me.BarZ = New NullDC_CvS2_BEAR.SimpleProgressBar()
        Me.BarT = New NullDC_CvS2_BEAR.SimpleProgressBar()
        Me.BarU = New NullDC_CvS2_BEAR.SimpleProgressBar()
        Me.BarV = New NullDC_CvS2_BEAR.SimpleProgressBar()
        Me.TableLayoutPanel1.SuspendLayout()
        CType(Me.trbDeadZone, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 3
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 196.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 91.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.btnDone, 0, 6)
        Me.TableLayoutPanel1.Controls.Add(Me.BarX, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.BarY, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.BarZ, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.BarT, 0, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.BarU, 0, 4)
        Me.TableLayoutPanel1.Controls.Add(Me.BarV, 0, 5)
        Me.TableLayoutPanel1.Controls.Add(Me.Label2, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.trbDeadZone, 1, 6)
        Me.TableLayoutPanel1.Controls.Add(Me.lbDeadZone, 2, 6)
        Me.TableLayoutPanel1.Controls.Add(Me.Label6, 1, 5)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 7
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(620, 228)
        Me.TableLayoutPanel1.TabIndex = 6
        '
        'btnDone
        '
        Me.btnDone.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.btnDone.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnDone.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.btnDone.Location = New System.Drawing.Point(0, 192)
        Me.btnDone.Margin = New System.Windows.Forms.Padding(0)
        Me.btnDone.Name = "btnDone"
        Me.btnDone.Size = New System.Drawing.Size(333, 36)
        Me.btnDone.TabIndex = 6
        Me.btnDone.Text = "Done"
        Me.btnDone.UseVisualStyleBackColor = False
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.TableLayoutPanel1.SetColumnSpan(Me.Label2, 2)
        Me.Label2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label2.Location = New System.Drawing.Point(336, 0)
        Me.Label2.Name = "Label2"
        Me.TableLayoutPanel1.SetRowSpan(Me.Label2, 3)
        Me.Label2.Size = New System.Drawing.Size(281, 96)
        Me.Label2.TabIndex = 8
        Me.Label2.Text = "Rotate your Sticks" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Pull your triggers" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Bars should be around the center of the" &
    " bars when idle." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "If your controller has some idle jitter, move the deadzone sli" &
    "der up untill the bars stop to jitter."
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'trbDeadZone
        '
        Me.trbDeadZone.Dock = System.Windows.Forms.DockStyle.Fill
        Me.trbDeadZone.Location = New System.Drawing.Point(336, 195)
        Me.trbDeadZone.Maximum = 100
        Me.trbDeadZone.Name = "trbDeadZone"
        Me.trbDeadZone.Size = New System.Drawing.Size(190, 30)
        Me.trbDeadZone.TabIndex = 18
        '
        'lbDeadZone
        '
        Me.lbDeadZone.AutoSize = True
        Me.lbDeadZone.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lbDeadZone.Location = New System.Drawing.Point(532, 192)
        Me.lbDeadZone.Name = "lbDeadZone"
        Me.lbDeadZone.Size = New System.Drawing.Size(85, 36)
        Me.lbDeadZone.TabIndex = 19
        Me.lbDeadZone.Text = "DEADZONE"
        Me.lbDeadZone.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label6.Location = New System.Drawing.Point(336, 160)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(190, 32)
        Me.Label6.TabIndex = 20
        Me.Label6.Text = "DeadZone"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        '
        'Timer1
        '
        Me.Timer1.Enabled = True
        Me.Timer1.Interval = 8
        '
        'BarX
        '
        Me.BarX.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BarX.Location = New System.Drawing.Point(3, 3)
        Me.BarX.Name = "BarX"
        Me.BarX.Size = New System.Drawing.Size(327, 26)
        Me.BarX.TabIndex = 9
        '
        'BarY
        '
        Me.BarY.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BarY.Location = New System.Drawing.Point(3, 35)
        Me.BarY.Name = "BarY"
        Me.BarY.Size = New System.Drawing.Size(327, 26)
        Me.BarY.TabIndex = 10
        '
        'BarZ
        '
        Me.BarZ.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BarZ.Location = New System.Drawing.Point(3, 67)
        Me.BarZ.Name = "BarZ"
        Me.BarZ.Size = New System.Drawing.Size(327, 26)
        Me.BarZ.TabIndex = 11
        '
        'BarT
        '
        Me.BarT.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BarT.Location = New System.Drawing.Point(3, 99)
        Me.BarT.Name = "BarT"
        Me.BarT.Size = New System.Drawing.Size(327, 26)
        Me.BarT.TabIndex = 12
        '
        'BarU
        '
        Me.BarU.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BarU.Location = New System.Drawing.Point(3, 131)
        Me.BarU.Name = "BarU"
        Me.BarU.Size = New System.Drawing.Size(327, 26)
        Me.BarU.TabIndex = 13
        '
        'BarV
        '
        Me.BarV.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BarV.Location = New System.Drawing.Point(3, 163)
        Me.BarV.Name = "BarV"
        Me.BarV.Size = New System.Drawing.Size(327, 26)
        Me.BarV.TabIndex = 14
        '
        'frmCalibration
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(200, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(620, 228)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "frmCalibration"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Calibration"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        CType(Me.trbDeadZone, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents btnDone As Button
    Friend WithEvents Label2 As Label
    Friend WithEvents Timer1 As Timer
    Friend WithEvents BarX As SimpleProgressBar
    Friend WithEvents BarY As SimpleProgressBar
    Friend WithEvents BarZ As SimpleProgressBar
    Friend WithEvents BarT As SimpleProgressBar
    Friend WithEvents BarU As SimpleProgressBar
    Friend WithEvents BarV As SimpleProgressBar
    Friend WithEvents trbDeadZone As TrackBar
    Friend WithEvents lbDeadZone As Label
    Friend WithEvents Label6 As Label
End Class
