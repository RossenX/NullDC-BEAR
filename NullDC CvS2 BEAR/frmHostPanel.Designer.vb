<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmHostPanel
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.btnStartHosting = New System.Windows.Forms.Button()
        Me.cbDelay = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.lbInfo = New System.Windows.Forms.Label()
        Me.lbPing = New System.Windows.Forms.Label()
        Me.btnExit = New System.Windows.Forms.Button()
        Me.btnSuggestDelay = New System.Windows.Forms.Button()
        Me.cbGameList = New System.Windows.Forms.ComboBox()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.cbHostType = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.tbFPS = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.cbRegion = New System.Windows.Forms.ComboBox()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnStartHosting
        '
        Me.btnStartHosting.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.btnStartHosting.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.btnStartHosting.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnStartHosting.ForeColor = System.Drawing.Color.FromArgb(CType(CType(5, Byte), Integer), CType(CType(5, Byte), Integer), CType(CType(5, Byte), Integer))
        Me.btnStartHosting.Location = New System.Drawing.Point(187, 342)
        Me.btnStartHosting.Name = "btnStartHosting"
        Me.btnStartHosting.Size = New System.Drawing.Size(75, 31)
        Me.btnStartHosting.TabIndex = 3
        Me.btnStartHosting.Text = "Host"
        Me.btnStartHosting.UseVisualStyleBackColor = False
        '
        'cbDelay
        '
        Me.cbDelay.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.cbDelay.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cbDelay.ForeColor = System.Drawing.Color.FromArgb(CType(CType(5, Byte), Integer), CType(CType(5, Byte), Integer), CType(CType(5, Byte), Integer))
        Me.cbDelay.FormattingEnabled = True
        Me.cbDelay.Items.AddRange(New Object() {"1", "2", "3", "4", "5", "6", "7"})
        Me.cbDelay.Location = New System.Drawing.Point(244, 278)
        Me.cbDelay.Name = "cbDelay"
        Me.cbDelay.Size = New System.Drawing.Size(61, 21)
        Me.cbDelay.TabIndex = 4
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(5, Byte), Integer), CType(CType(5, Byte), Integer), CType(CType(5, Byte), Integer))
        Me.Label1.Location = New System.Drawing.Point(249, 260)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(49, 16)
        Me.Label1.TabIndex = 5
        Me.Label1.Text = "Delay"
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = Global.NullDC_CvS2_BEAR.My.Resources.Resources.ConnectingNetplay
        Me.PictureBox1.Location = New System.Drawing.Point(75, 32)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(300, 149)
        Me.PictureBox1.TabIndex = 6
        Me.PictureBox1.TabStop = False
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(200, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.TableLayoutPanel1.ColumnCount = 1
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.lbInfo, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.lbPing, 0, 1)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(107, 187)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 2
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(236, 37)
        Me.TableLayoutPanel1.TabIndex = 7
        '
        'lbInfo
        '
        Me.lbInfo.AutoSize = True
        Me.lbInfo.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lbInfo.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbInfo.ForeColor = System.Drawing.Color.FromArgb(CType(CType(5, Byte), Integer), CType(CType(5, Byte), Integer), CType(CType(5, Byte), Integer))
        Me.lbInfo.Location = New System.Drawing.Point(3, 0)
        Me.lbInfo.Name = "lbInfo"
        Me.lbInfo.Size = New System.Drawing.Size(230, 17)
        Me.lbInfo.TabIndex = 0
        Me.lbInfo.Text = "Hosting Solo"
        Me.lbInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lbPing
        '
        Me.lbPing.AutoSize = True
        Me.lbPing.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lbPing.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbPing.ForeColor = System.Drawing.Color.FromArgb(CType(CType(5, Byte), Integer), CType(CType(5, Byte), Integer), CType(CType(5, Byte), Integer))
        Me.lbPing.Location = New System.Drawing.Point(3, 17)
        Me.lbPing.Name = "lbPing"
        Me.lbPing.Size = New System.Drawing.Size(230, 20)
        Me.lbPing.TabIndex = 1
        Me.lbPing.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnExit
        '
        Me.btnExit.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.btnExit.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnExit.ForeColor = System.Drawing.Color.FromArgb(CType(CType(5, Byte), Integer), CType(CType(5, Byte), Integer), CType(CType(5, Byte), Integer))
        Me.btnExit.Location = New System.Drawing.Point(187, 1)
        Me.btnExit.Name = "btnExit"
        Me.btnExit.Size = New System.Drawing.Size(75, 31)
        Me.btnExit.TabIndex = 8
        Me.btnExit.Text = "Exit"
        Me.btnExit.UseVisualStyleBackColor = False
        '
        'btnSuggestDelay
        '
        Me.btnSuggestDelay.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.btnSuggestDelay.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.btnSuggestDelay.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSuggestDelay.ForeColor = System.Drawing.Color.FromArgb(CType(CType(5, Byte), Integer), CType(CType(5, Byte), Integer), CType(CType(5, Byte), Integer))
        Me.btnSuggestDelay.Location = New System.Drawing.Point(244, 301)
        Me.btnSuggestDelay.Name = "btnSuggestDelay"
        Me.btnSuggestDelay.Size = New System.Drawing.Size(61, 22)
        Me.btnSuggestDelay.TabIndex = 9
        Me.btnSuggestDelay.Text = "Suggest"
        Me.btnSuggestDelay.UseVisualStyleBackColor = False
        '
        'cbGameList
        '
        Me.cbGameList.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(200, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.cbGameList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbGameList.ForeColor = System.Drawing.Color.FromArgb(CType(CType(5, Byte), Integer), CType(CType(5, Byte), Integer), CType(CType(5, Byte), Integer))
        Me.cbGameList.FormattingEnabled = True
        Me.cbGameList.Location = New System.Drawing.Point(65, 230)
        Me.cbGameList.Name = "cbGameList"
        Me.cbGameList.Size = New System.Drawing.Size(258, 21)
        Me.cbGameList.TabIndex = 10
        '
        'Button1
        '
        Me.Button1.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.Button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.Button1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(5, Byte), Integer), CType(CType(5, Byte), Integer), CType(CType(5, Byte), Integer))
        Me.Button1.Location = New System.Drawing.Point(308, 277)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(25, 25)
        Me.Button1.TabIndex = 11
        Me.Button1.Text = "?"
        Me.Button1.UseVisualStyleBackColor = False
        '
        'cbHostType
        '
        Me.cbHostType.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.cbHostType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbHostType.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.cbHostType.FormattingEnabled = True
        Me.cbHostType.Items.AddRange(New Object() {"Audio Sync"})
        Me.cbHostType.Location = New System.Drawing.Point(136, 278)
        Me.cbHostType.Name = "cbHostType"
        Me.cbHostType.Size = New System.Drawing.Size(77, 21)
        Me.cbHostType.TabIndex = 12
        Me.cbHostType.Visible = False
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.FromArgb(CType(CType(5, Byte), Integer), CType(CType(5, Byte), Integer), CType(CType(5, Byte), Integer))
        Me.Label2.Location = New System.Drawing.Point(147, 260)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(54, 16)
        Me.Label2.TabIndex = 13
        Me.Label2.Text = "Limiter"
        Me.Label2.Visible = False
        '
        'Button2
        '
        Me.Button2.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.Button2.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.Button2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button2.ForeColor = System.Drawing.Color.FromArgb(CType(CType(5, Byte), Integer), CType(CType(5, Byte), Integer), CType(CType(5, Byte), Integer))
        Me.Button2.Location = New System.Drawing.Point(109, 276)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(25, 25)
        Me.Button2.TabIndex = 14
        Me.Button2.Text = "?"
        Me.Button2.UseVisualStyleBackColor = False
        Me.Button2.Visible = False
        '
        'tbFPS
        '
        Me.tbFPS.Location = New System.Drawing.Point(178, 299)
        Me.tbFPS.Name = "tbFPS"
        Me.tbFPS.Size = New System.Drawing.Size(35, 20)
        Me.tbFPS.TabIndex = 15
        Me.tbFPS.Visible = False
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.BackColor = System.Drawing.Color.Transparent
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.Color.FromArgb(CType(CType(5, Byte), Integer), CType(CType(5, Byte), Integer), CType(CType(5, Byte), Integer))
        Me.Label3.Location = New System.Drawing.Point(139, 302)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(40, 16)
        Me.Label3.TabIndex = 16
        Me.Label3.Text = "Limit"
        Me.Label3.Visible = False
        '
        'cbRegion
        '
        Me.cbRegion.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(200, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.cbRegion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbRegion.FormattingEnabled = True
        Me.cbRegion.Items.AddRange(New Object() {"JPN", "USA", "EUR"})
        Me.cbRegion.Location = New System.Drawing.Point(329, 230)
        Me.cbRegion.Name = "cbRegion"
        Me.cbRegion.Size = New System.Drawing.Size(52, 21)
        Me.cbRegion.TabIndex = 25
        '
        'frmHostPanel
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.BackColor = System.Drawing.Color.DimGray
        Me.BackgroundImage = Global.NullDC_CvS2_BEAR.My.Resources.Resources.SingleSquare
        Me.ClientSize = New System.Drawing.Size(450, 450)
        Me.Controls.Add(Me.cbRegion)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.tbFPS)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.cbHostType)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.cbGameList)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.btnSuggestDelay)
        Me.Controls.Add(Me.btnExit)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.cbDelay)
        Me.Controls.Add(Me.btnStartHosting)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.MaximumSize = New System.Drawing.Size(450, 450)
        Me.MinimumSize = New System.Drawing.Size(450, 450)
        Me.Name = "frmHostPanel"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Host"
        Me.TransparencyKey = System.Drawing.Color.DimGray
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnStartHosting As Button
    Friend WithEvents cbDelay As ComboBox
    Friend WithEvents Label1 As Label
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents lbInfo As Label
    Friend WithEvents btnExit As Button
    Friend WithEvents lbPing As Label
    Friend WithEvents btnSuggestDelay As Button
    Friend WithEvents cbGameList As ComboBox
    Friend WithEvents Button1 As Button
    Friend WithEvents cbHostType As ComboBox
    Friend WithEvents Label2 As Label
    Friend WithEvents Button2 As Button
    Friend WithEvents tbFPS As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents cbRegion As ComboBox
End Class
