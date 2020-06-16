<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmChallengeGameSelect
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
        Me.cbGameList = New System.Windows.Forms.ComboBox()
        Me.btnLetsGo = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.tbFPS = New System.Windows.Forms.TextBox()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.cbHostType = New System.Windows.Forms.ComboBox()
        Me.SuspendLayout()
        '
        'cbGameList
        '
        Me.cbGameList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbGameList.FormattingEnabled = True
        Me.cbGameList.Location = New System.Drawing.Point(70, 208)
        Me.cbGameList.Name = "cbGameList"
        Me.cbGameList.Size = New System.Drawing.Size(316, 21)
        Me.cbGameList.TabIndex = 0
        '
        'btnLetsGo
        '
        Me.btnLetsGo.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.btnLetsGo.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.btnLetsGo.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnLetsGo.Location = New System.Drawing.Point(172, 277)
        Me.btnLetsGo.Name = "btnLetsGo"
        Me.btnLetsGo.Size = New System.Drawing.Size(102, 88)
        Me.btnLetsGo.TabIndex = 1
        Me.btnLetsGo.Text = "LETS GO!"
        Me.btnLetsGo.UseVisualStyleBackColor = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.Black
        Me.Label1.Location = New System.Drawing.Point(168, 185)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(113, 20)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Game Select"
        '
        'Button1
        '
        Me.Button1.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.Button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.Button1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button1.Location = New System.Drawing.Point(172, 89)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(102, 28)
        Me.Button1.TabIndex = 3
        Me.Button1.Text = "Cancel"
        Me.Button1.UseVisualStyleBackColor = False
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.BackColor = System.Drawing.Color.Transparent
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.Color.FromArgb(CType(CType(5, Byte), Integer), CType(CType(5, Byte), Integer), CType(CType(5, Byte), Integer))
        Me.Label3.Location = New System.Drawing.Point(191, 162)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(40, 16)
        Me.Label3.TabIndex = 21
        Me.Label3.Text = "Limit"
        '
        'tbFPS
        '
        Me.tbFPS.Location = New System.Drawing.Point(230, 159)
        Me.tbFPS.Name = "tbFPS"
        Me.tbFPS.Size = New System.Drawing.Size(35, 20)
        Me.tbFPS.TabIndex = 20
        '
        'Button2
        '
        Me.Button2.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.Button2.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.Button2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button2.ForeColor = System.Drawing.Color.FromArgb(CType(CType(5, Byte), Integer), CType(CType(5, Byte), Integer), CType(CType(5, Byte), Integer))
        Me.Button2.Location = New System.Drawing.Point(161, 136)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(25, 25)
        Me.Button2.TabIndex = 19
        Me.Button2.Text = "?"
        Me.Button2.UseVisualStyleBackColor = False
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.FromArgb(CType(CType(5, Byte), Integer), CType(CType(5, Byte), Integer), CType(CType(5, Byte), Integer))
        Me.Label2.Location = New System.Drawing.Point(199, 120)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(54, 16)
        Me.Label2.TabIndex = 18
        Me.Label2.Text = "Limiter"
        '
        'cbHostType
        '
        Me.cbHostType.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.cbHostType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbHostType.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.cbHostType.FormattingEnabled = True
        Me.cbHostType.Items.AddRange(New Object() {"Audio Sync", "FPS Limit"})
        Me.cbHostType.Location = New System.Drawing.Point(188, 138)
        Me.cbHostType.Name = "cbHostType"
        Me.cbHostType.Size = New System.Drawing.Size(77, 21)
        Me.cbHostType.TabIndex = 17
        '
        'frmChallengeGameSelect
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.DimGray
        Me.BackgroundImage = Global.NullDC_CvS2_BEAR.My.Resources.Resources.SingleSquare
        Me.ClientSize = New System.Drawing.Size(450, 450)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.tbFPS)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.cbHostType)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.btnLetsGo)
        Me.Controls.Add(Me.cbGameList)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmChallengeGameSelect"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Game Select"
        Me.TransparencyKey = System.Drawing.Color.DimGray
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents cbGameList As ComboBox
    Friend WithEvents btnLetsGo As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents Button1 As Button
    Friend WithEvents Label3 As Label
    Friend WithEvents tbFPS As TextBox
    Friend WithEvents Button2 As Button
    Friend WithEvents Label2 As Label
    Friend WithEvents cbHostType As ComboBox
End Class
