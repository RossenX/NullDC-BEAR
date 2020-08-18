<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSetup
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
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.tb_Volume = New System.Windows.Forms.TrackBar()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.cbAllowSpectators = New System.Windows.Forms.ComboBox()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.tbPort = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.btnSaveExit = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.tbPlayerName = New System.Windows.Forms.TextBox()
        Me.cbShowConsole = New System.Windows.Forms.CheckBox()
        Me.tb_eVolume = New System.Windows.Forms.TrackBar()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tb_Volume, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tb_eVolume, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = Global.NullDC_CvS2_BEAR.My.Resources.Resources.Clippy
        Me.PictureBox1.InitialImage = Global.NullDC_CvS2_BEAR.My.Resources.Resources.Clippy
        Me.PictureBox1.Location = New System.Drawing.Point(12, 12)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(257, 227)
        Me.PictureBox1.TabIndex = 1
        Me.PictureBox1.TabStop = False
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(275, 111)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(164, 23)
        Me.Button1.TabIndex = 3
        Me.Button1.TabStop = False
        Me.Button1.Text = "TELL ME ALL THE THINGS"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label3.Location = New System.Drawing.Point(57, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(45, 13)
        Me.Label3.TabIndex = 13
        Me.Label3.Text = "BEAR"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'tb_Volume
        '
        Me.tb_Volume.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tb_Volume.Location = New System.Drawing.Point(57, 16)
        Me.tb_Volume.Maximum = 100
        Me.tb_Volume.Name = "tb_Volume"
        Me.tb_Volume.Orientation = System.Windows.Forms.Orientation.Vertical
        Me.tb_Volume.Size = New System.Drawing.Size(45, 146)
        Me.tb_Volume.TabIndex = 12
        Me.tb_Volume.TabStop = False
        Me.tb_Volume.TickFrequency = 10
        Me.tb_Volume.TickStyle = System.Windows.Forms.TickStyle.Both
        '
        'Button3
        '
        Me.Button3.BackColor = System.Drawing.Color.Red
        Me.Button3.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.Button3.Location = New System.Drawing.Point(275, 140)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(306, 40)
        Me.Button3.TabIndex = 11
        Me.Button3.TabStop = False
        Me.Button3.Text = "Controls"
        Me.Button3.UseVisualStyleBackColor = False
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(484, 18)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(86, 13)
        Me.Label8.TabIndex = 10
        Me.Label8.Text = "Allow Spectators"
        '
        'cbAllowSpectators
        '
        Me.cbAllowSpectators.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbAllowSpectators.FormattingEnabled = True
        Me.cbAllowSpectators.Items.AddRange(New Object() {"Yes", "No"})
        Me.cbAllowSpectators.Location = New System.Drawing.Point(487, 34)
        Me.cbAllowSpectators.Name = "cbAllowSpectators"
        Me.cbAllowSpectators.Size = New System.Drawing.Size(83, 21)
        Me.cbAllowSpectators.TabIndex = 9
        Me.cbAllowSpectators.TabStop = False
        '
        'Button2
        '
        Me.Button2.BackColor = System.Drawing.Color.Lime
        Me.Button2.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.Button2.Location = New System.Drawing.Point(275, 186)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(306, 54)
        Me.Button2.TabIndex = 7
        Me.Button2.TabStop = False
        Me.Button2.Text = "Add Firewall Entry for BEAR and NullDC to Windows Firewall"
        Me.Button2.UseVisualStyleBackColor = False
        '
        'tbPort
        '
        Me.tbPort.Location = New System.Drawing.Point(381, 35)
        Me.tbPort.Name = "tbPort"
        Me.tbPort.Size = New System.Drawing.Size(100, 20)
        Me.tbPort.TabIndex = 6
        Me.tbPort.TabStop = False
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(381, 19)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(62, 13)
        Me.Label6.TabIndex = 5
        Me.Label6.Text = "NullDC Port"
        '
        'btnSaveExit
        '
        Me.btnSaveExit.Location = New System.Drawing.Point(587, 186)
        Me.btnSaveExit.Name = "btnSaveExit"
        Me.btnSaveExit.Size = New System.Drawing.Size(111, 54)
        Me.btnSaveExit.TabIndex = 4
        Me.btnSaveExit.TabStop = False
        Me.btnSaveExit.Text = "Save"
        Me.btnSaveExit.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(275, 19)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(98, 13)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "What's your name?"
        '
        'tbPlayerName
        '
        Me.tbPlayerName.Location = New System.Drawing.Point(275, 35)
        Me.tbPlayerName.Name = "tbPlayerName"
        Me.tbPlayerName.Size = New System.Drawing.Size(100, 20)
        Me.tbPlayerName.TabIndex = 0
        Me.tbPlayerName.TabStop = False
        '
        'cbShowConsole
        '
        Me.cbShowConsole.AutoSize = True
        Me.cbShowConsole.Location = New System.Drawing.Point(275, 61)
        Me.cbShowConsole.Name = "cbShowConsole"
        Me.cbShowConsole.Size = New System.Drawing.Size(130, 17)
        Me.cbShowConsole.TabIndex = 14
        Me.cbShowConsole.Text = "Show NullDC Console"
        Me.cbShowConsole.UseVisualStyleBackColor = True
        '
        'tb_eVolume
        '
        Me.tb_eVolume.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tb_eVolume.Location = New System.Drawing.Point(3, 16)
        Me.tb_eVolume.Maximum = 100
        Me.tb_eVolume.Name = "tb_eVolume"
        Me.tb_eVolume.Orientation = System.Windows.Forms.Orientation.Vertical
        Me.tb_eVolume.Size = New System.Drawing.Size(48, 146)
        Me.tb_eVolume.TabIndex = 15
        Me.tb_eVolume.TabStop = False
        Me.tb_eVolume.TickFrequency = 10
        Me.tb_eVolume.TickStyle = System.Windows.Forms.TickStyle.Both
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label1.Location = New System.Drawing.Point(3, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(48, 13)
        Me.Label1.TabIndex = 16
        Me.Label1.Text = "Emulator"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'GroupBox1
        '
        Me.GroupBox1.AutoSize = True
        Me.GroupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.GroupBox1.Controls.Add(Me.TableLayoutPanel1)
        Me.GroupBox1.Location = New System.Drawing.Point(587, 1)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(111, 184)
        Me.GroupBox1.TabIndex = 17
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Volume"
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.AutoSize = True
        Me.TableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.Controls.Add(Me.tb_Volume, 1, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.Label1, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Label3, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.tb_eVolume, 0, 1)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(3, 16)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 2
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(105, 165)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'frmSetup
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(706, 250)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.cbShowConsole)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.btnSaveExit)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.tbPort)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.Button3)
        Me.Controls.Add(Me.cbAllowSpectators)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.tbPlayerName)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "frmSetup"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "BEAR Setup"
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tb_Volume, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tb_eVolume, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents Label2 As Label
    Friend WithEvents tbPlayerName As TextBox
    Friend WithEvents btnSaveExit As Button
    Friend WithEvents tbPort As TextBox
    Friend WithEvents Label6 As Label
    Friend WithEvents Button1 As Button
    Friend WithEvents Button2 As Button
    Friend WithEvents Label8 As Label
    Friend WithEvents cbAllowSpectators As ComboBox
    Friend WithEvents Button3 As Button
    Friend WithEvents tb_Volume As TrackBar
    Friend WithEvents Label3 As Label
    Friend WithEvents cbShowConsole As CheckBox
    Friend WithEvents tb_eVolume As TrackBar
    Friend WithEvents Label1 As Label
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
End Class
