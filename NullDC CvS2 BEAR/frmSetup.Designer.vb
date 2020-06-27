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
        Me.tcSetup = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.btnT1B1 = New System.Windows.Forms.Button()
        Me.btnT1B2 = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.tbPort = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.btnT2B1 = New System.Windows.Forms.Button()
        Me.cbNetworks = New System.Windows.Forms.ComboBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.tbPlayerName = New System.Windows.Forms.TextBox()
        Me.TabPage3 = New System.Windows.Forms.TabPage()
        Me.btnT3B2 = New System.Windows.Forms.Button()
        Me.btnT3B3 = New System.Windows.Forms.Button()
        Me.btnT3B1 = New System.Windows.Forms.Button()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Panel1 = New System.Windows.Forms.Panel()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tcSetup.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.TabPage3.SuspendLayout()
        Me.Panel1.SuspendLayout()
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
        'tcSetup
        '
        Me.tcSetup.Controls.Add(Me.TabPage1)
        Me.tcSetup.Controls.Add(Me.TabPage2)
        Me.tcSetup.Controls.Add(Me.TabPage3)
        Me.tcSetup.Location = New System.Drawing.Point(-4, -23)
        Me.tcSetup.Margin = New System.Windows.Forms.Padding(0)
        Me.tcSetup.Name = "tcSetup"
        Me.tcSetup.SelectedIndex = 0
        Me.tcSetup.Size = New System.Drawing.Size(508, 254)
        Me.tcSetup.TabIndex = 2
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.Button1)
        Me.TabPage1.Controls.Add(Me.btnT1B1)
        Me.TabPage1.Controls.Add(Me.btnT1B2)
        Me.TabPage1.Controls.Add(Me.Label1)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(500, 228)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "TabPage1"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(164, 191)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(164, 23)
        Me.Button1.TabIndex = 3
        Me.Button1.Text = "TELL ME ALL THE THINGS"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'btnT1B1
        '
        Me.btnT1B1.Location = New System.Drawing.Point(9, 191)
        Me.btnT1B1.Name = "btnT1B1"
        Me.btnT1B1.Size = New System.Drawing.Size(149, 23)
        Me.btnT1B1.TabIndex = 2
        Me.btnT1B1.Text = "Hell no get me out of here"
        Me.btnT1B1.UseVisualStyleBackColor = True
        '
        'btnT1B2
        '
        Me.btnT1B2.Location = New System.Drawing.Point(416, 191)
        Me.btnT1B2.Name = "btnT1B2"
        Me.btnT1B2.Size = New System.Drawing.Size(75, 23)
        Me.btnT1B2.TabIndex = 1
        Me.btnT1B2.Text = "Lets go"
        Me.btnT1B2.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(6, 3)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(441, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Hey there! Looks like this is the first time you ran this program, lets do a quic" &
    "k setup shall we."
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.Label7)
        Me.TabPage2.Controls.Add(Me.Button2)
        Me.TabPage2.Controls.Add(Me.tbPort)
        Me.TabPage2.Controls.Add(Me.Label6)
        Me.TabPage2.Controls.Add(Me.btnT2B1)
        Me.TabPage2.Controls.Add(Me.cbNetworks)
        Me.TabPage2.Controls.Add(Me.Label3)
        Me.TabPage2.Controls.Add(Me.Label2)
        Me.TabPage2.Controls.Add(Me.tbPlayerName)
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(500, 228)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "TabPage2"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(6, 152)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(390, 13)
        Me.Label7.TabIndex = 8
        Me.Label7.Text = "If you're having trouble connecting or hosting, CLICK THE BIG GREEN BUTTON"
        '
        'Button2
        '
        Me.Button2.BackColor = System.Drawing.Color.Lime
        Me.Button2.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.Button2.Location = New System.Drawing.Point(9, 168)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(380, 54)
        Me.Button2.TabIndex = 7
        Me.Button2.Text = "Add Firewall Entry for BEAR and NullDC to Windows Firewall"
        Me.Button2.UseVisualStyleBackColor = False
        '
        'tbPort
        '
        Me.tbPort.Location = New System.Drawing.Point(9, 98)
        Me.tbPort.Name = "tbPort"
        Me.tbPort.Size = New System.Drawing.Size(100, 20)
        Me.tbPort.TabIndex = 6
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(6, 82)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(323, 13)
        Me.Label6.TabIndex = 5
        Me.Label6.Text = "Which port do you want to use? (Usually don't want to change this)"
        '
        'btnT2B1
        '
        Me.btnT2B1.Location = New System.Drawing.Point(419, 199)
        Me.btnT2B1.Name = "btnT2B1"
        Me.btnT2B1.Size = New System.Drawing.Size(75, 23)
        Me.btnT2B1.TabIndex = 4
        Me.btnT2B1.Text = "Next"
        Me.btnT2B1.UseVisualStyleBackColor = True
        '
        'cbNetworks
        '
        Me.cbNetworks.FormattingEnabled = True
        Me.cbNetworks.Location = New System.Drawing.Point(9, 58)
        Me.cbNetworks.Name = "cbNetworks"
        Me.cbNetworks.Size = New System.Drawing.Size(212, 21)
        Me.cbNetworks.TabIndex = 3
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(6, 42)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(215, 13)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "Which network do you want to kick ass on?"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(6, 3)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(98, 13)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "What's your name?"
        '
        'tbPlayerName
        '
        Me.tbPlayerName.Location = New System.Drawing.Point(9, 19)
        Me.tbPlayerName.Name = "tbPlayerName"
        Me.tbPlayerName.Size = New System.Drawing.Size(100, 20)
        Me.tbPlayerName.TabIndex = 0
        '
        'TabPage3
        '
        Me.TabPage3.Controls.Add(Me.btnT3B2)
        Me.TabPage3.Controls.Add(Me.btnT3B3)
        Me.TabPage3.Controls.Add(Me.btnT3B1)
        Me.TabPage3.Controls.Add(Me.Label5)
        Me.TabPage3.Controls.Add(Me.Label4)
        Me.TabPage3.Location = New System.Drawing.Point(4, 22)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage3.Size = New System.Drawing.Size(500, 228)
        Me.TabPage3.TabIndex = 2
        Me.TabPage3.Text = "`"
        Me.TabPage3.UseVisualStyleBackColor = True
        '
        'btnT3B2
        '
        Me.btnT3B2.Location = New System.Drawing.Point(3, 200)
        Me.btnT3B2.Name = "btnT3B2"
        Me.btnT3B2.Size = New System.Drawing.Size(72, 23)
        Me.btnT3B2.TabIndex = 4
        Me.btnT3B2.Text = "Back"
        Me.btnT3B2.UseVisualStyleBackColor = True
        '
        'btnT3B3
        '
        Me.btnT3B3.Location = New System.Drawing.Point(388, 200)
        Me.btnT3B3.Name = "btnT3B3"
        Me.btnT3B3.Size = New System.Drawing.Size(109, 23)
        Me.btnT3B3.TabIndex = 3
        Me.btnT3B3.Text = "Done and Done"
        Me.btnT3B3.UseVisualStyleBackColor = True
        '
        'btnT3B1
        '
        Me.btnT3B1.BackColor = System.Drawing.Color.Red
        Me.btnT3B1.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.btnT3B1.Location = New System.Drawing.Point(115, 81)
        Me.btnT3B1.Name = "btnT3B1"
        Me.btnT3B1.Size = New System.Drawing.Size(278, 40)
        Me.btnT3B1.TabIndex = 2
        Me.btnT3B1.Text = "Controls"
        Me.btnT3B1.UseVisualStyleBackColor = False
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(3, 19)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(169, 13)
        Me.Label5.TabIndex = 1
        Me.Label5.Text = "Now lets take care of the controls."
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(3, 3)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(441, 13)
        Me.Label4.TabIndex = 0
        Me.Label4.Text = "Heh, can't say that's what i would've picked for you, but if that's your name; th" &
    "at's your name"
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.tcSetup)
        Me.Panel1.Location = New System.Drawing.Point(275, 12)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(500, 227)
        Me.Panel1.TabIndex = 3
        '
        'frmSetup
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(787, 248)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.PictureBox1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "frmSetup"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "BEAR Setup"
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tcSetup.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage1.PerformLayout()
        Me.TabPage2.ResumeLayout(False)
        Me.TabPage2.PerformLayout()
        Me.TabPage3.ResumeLayout(False)
        Me.TabPage3.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents tcSetup As TabControl
    Friend WithEvents TabPage1 As TabPage
    Friend WithEvents TabPage2 As TabPage
    Friend WithEvents TabPage3 As TabPage
    Friend WithEvents Panel1 As Panel
    Friend WithEvents btnT1B2 As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents btnT1B1 As Button
    Friend WithEvents Label2 As Label
    Friend WithEvents tbPlayerName As TextBox
    Friend WithEvents cbNetworks As ComboBox
    Friend WithEvents Label3 As Label
    Friend WithEvents btnT2B1 As Button
    Friend WithEvents Label5 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents btnT3B1 As Button
    Friend WithEvents tbPort As TextBox
    Friend WithEvents Label6 As Label
    Friend WithEvents btnT3B2 As Button
    Friend WithEvents btnT3B3 As Button
    Friend WithEvents Button1 As Button
    Friend WithEvents Button2 As Button
    Friend WithEvents Label7 As Label
End Class
