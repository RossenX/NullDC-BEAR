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
        Me.btnDLC = New System.Windows.Forms.Button()
        Me.cbRegion = New System.Windows.Forms.ComboBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'cbGameList
        '
        Me.cbGameList.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(200, Byte), Integer), CType(CType(0, Byte), Integer))
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
        'btnDLC
        '
        Me.btnDLC.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.btnDLC.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.btnDLC.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnDLC.Location = New System.Drawing.Point(70, 235)
        Me.btnDLC.Name = "btnDLC"
        Me.btnDLC.Size = New System.Drawing.Size(142, 28)
        Me.btnDLC.TabIndex = 22
        Me.btnDLC.Text = "Get More Games"
        Me.btnDLC.UseVisualStyleBackColor = False
        '
        'cbRegion
        '
        Me.cbRegion.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(200, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.cbRegion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbRegion.FormattingEnabled = True
        Me.cbRegion.Items.AddRange(New Object() {"JPN", "USA", "EUR"})
        Me.cbRegion.Location = New System.Drawing.Point(334, 235)
        Me.cbRegion.Name = "cbRegion"
        Me.cbRegion.Size = New System.Drawing.Size(52, 21)
        Me.cbRegion.TabIndex = 23
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.BackColor = System.Drawing.Color.Transparent
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.ForeColor = System.Drawing.Color.FromArgb(CType(CType(5, Byte), Integer), CType(CType(5, Byte), Integer), CType(CType(5, Byte), Integer))
        Me.Label4.Location = New System.Drawing.Point(274, 238)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(58, 16)
        Me.Label4.TabIndex = 24
        Me.Label4.Text = "Region"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'frmChallengeGameSelect
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.BackColor = System.Drawing.Color.DimGray
        Me.BackgroundImage = Global.NullDC_CvS2_BEAR.My.Resources.Resources.SingleSquare
        Me.ClientSize = New System.Drawing.Size(450, 450)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.cbRegion)
        Me.Controls.Add(Me.btnDLC)
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
    Friend WithEvents btnDLC As Button
    Friend WithEvents cbRegion As ComboBox
    Friend WithEvents Label4 As Label
End Class
