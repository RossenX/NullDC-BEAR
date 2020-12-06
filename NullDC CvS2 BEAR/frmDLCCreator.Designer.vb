<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmDLCCreator
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
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.lb_Platform = New System.Windows.Forms.Label()
        Me.cb_platform = New System.Windows.Forms.ComboBox()
        Me.tb_discription = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.tb_tabname = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.cb_extract = New System.Windows.Forms.ComboBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.RichTextBox1 = New System.Windows.Forms.RichTextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.btnSavePack = New System.Windows.Forms.Button()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TableLayoutPanel1.ColumnCount = 3
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.Controls.Add(Me.lb_Platform, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.cb_platform, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.tb_discription, 1, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.Label1, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.tb_tabname, 1, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.Label2, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.cb_extract, 1, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.Label3, 0, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.Label4, 0, 4)
        Me.TableLayoutPanel1.Controls.Add(Me.TextBox1, 1, 4)
        Me.TableLayoutPanel1.Controls.Add(Me.RichTextBox1, 1, 5)
        Me.TableLayoutPanel1.Controls.Add(Me.Label5, 0, 5)
        Me.TableLayoutPanel1.Controls.Add(Me.Button1, 2, 4)
        Me.TableLayoutPanel1.Controls.Add(Me.btnSavePack, 2, 6)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.Padding = New System.Windows.Forms.Padding(10)
        Me.TableLayoutPanel1.RowCount = 7
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(634, 533)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'lb_Platform
        '
        Me.lb_Platform.AutoSize = True
        Me.lb_Platform.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lb_Platform.Location = New System.Drawing.Point(13, 10)
        Me.lb_Platform.Name = "lb_Platform"
        Me.lb_Platform.Size = New System.Drawing.Size(70, 27)
        Me.lb_Platform.TabIndex = 0
        Me.lb_Platform.Text = "Platform"
        Me.lb_Platform.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cb_platform
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.cb_platform, 2)
        Me.cb_platform.Dock = System.Windows.Forms.DockStyle.Fill
        Me.cb_platform.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cb_platform.FormattingEnabled = True
        Me.cb_platform.Items.AddRange(New Object() {"NA", "DC", "PSX", "SS", "FDS", "NES", "SNES", "GBA", "GBC", "SG", "NGP"})
        Me.cb_platform.Location = New System.Drawing.Point(89, 13)
        Me.cb_platform.Name = "cb_platform"
        Me.cb_platform.Size = New System.Drawing.Size(532, 21)
        Me.cb_platform.TabIndex = 1
        '
        'tb_discription
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.tb_discription, 2)
        Me.tb_discription.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tb_discription.Location = New System.Drawing.Point(89, 40)
        Me.tb_discription.Name = "tb_discription"
        Me.tb_discription.Size = New System.Drawing.Size(532, 20)
        Me.tb_discription.TabIndex = 2
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label1.Location = New System.Drawing.Point(13, 37)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(70, 26)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Description"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'tb_tabname
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.tb_tabname, 2)
        Me.tb_tabname.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tb_tabname.Location = New System.Drawing.Point(89, 66)
        Me.tb_tabname.Name = "tb_tabname"
        Me.tb_tabname.Size = New System.Drawing.Size(532, 20)
        Me.tb_tabname.TabIndex = 4
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label2.Location = New System.Drawing.Point(13, 63)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(70, 26)
        Me.Label2.TabIndex = 5
        Me.Label2.Text = "Tab Name"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cb_extract
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.cb_extract, 2)
        Me.cb_extract.Dock = System.Windows.Forms.DockStyle.Fill
        Me.cb_extract.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cb_extract.FormattingEnabled = True
        Me.cb_extract.Items.AddRange(New Object() {"Do Not Unzip", "Unzip", "Unzip, Place In It's Own Folder", "Do not Unzip, Place in It's Own Folder"})
        Me.cb_extract.Location = New System.Drawing.Point(89, 92)
        Me.cb_extract.Name = "cb_extract"
        Me.cb_extract.Size = New System.Drawing.Size(532, 21)
        Me.cb_extract.TabIndex = 6
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label3.Location = New System.Drawing.Point(13, 89)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(70, 27)
        Me.Label3.TabIndex = 7
        Me.Label3.Text = "Install Type"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label4.Location = New System.Drawing.Point(13, 116)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(70, 29)
        Me.Label4.TabIndex = 8
        Me.Label4.Text = "External URL"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TextBox1
        '
        Me.TextBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TextBox1.Location = New System.Drawing.Point(89, 119)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(384, 20)
        Me.TextBox1.TabIndex = 9
        '
        'RichTextBox1
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.RichTextBox1, 2)
        Me.RichTextBox1.DetectUrls = False
        Me.RichTextBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RichTextBox1.Location = New System.Drawing.Point(89, 148)
        Me.RichTextBox1.Name = "RichTextBox1"
        Me.RichTextBox1.Size = New System.Drawing.Size(532, 343)
        Me.RichTextBox1.TabIndex = 10
        Me.RichTextBox1.Text = ""
        Me.RichTextBox1.WordWrap = False
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label5.Location = New System.Drawing.Point(13, 145)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(70, 349)
        Me.Label5.TabIndex = 12
        Me.Label5.Text = "Direct Links" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "1 Per Line"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Button1
        '
        Me.Button1.AutoSize = True
        Me.Button1.Location = New System.Drawing.Point(479, 119)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(142, 23)
        Me.Button1.TabIndex = 11
        Me.Button1.Text = "Import Archive.Org Repo"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'btnSavePack
        '
        Me.btnSavePack.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnSavePack.Location = New System.Drawing.Point(479, 497)
        Me.btnSavePack.Name = "btnSavePack"
        Me.btnSavePack.Size = New System.Drawing.Size(142, 23)
        Me.btnSavePack.TabIndex = 15
        Me.btnSavePack.Text = "Save Pack"
        Me.btnSavePack.UseVisualStyleBackColor = True
        '
        'frmDLCCreator
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(634, 533)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Name = "frmDLCCreator"
        Me.Text = "DLC Creator"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents lb_Platform As Label
    Friend WithEvents cb_platform As ComboBox
    Friend WithEvents tb_discription As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents tb_tabname As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents cb_extract As ComboBox
    Friend WithEvents Label3 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents TextBox1 As TextBox
    Friend WithEvents RichTextBox1 As RichTextBox
    Friend WithEvents Button1 As Button
    Friend WithEvents Label5 As Label
    Friend WithEvents btnSavePack As Button
End Class
