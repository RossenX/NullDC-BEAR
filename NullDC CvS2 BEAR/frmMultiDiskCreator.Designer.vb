<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMultiDiskCreator
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
        Me.RichTextBox1 = New System.Windows.Forms.RichTextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.cb_Platform = New System.Windows.Forms.ComboBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.btnHelp = New System.Windows.Forms.Button()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 3
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.Controls.Add(Me.RichTextBox1, 1, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.Label1, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.Button2, 2, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.Button1, 2, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.cb_Platform, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Label3, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.btnHelp, 2, 0)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.Padding = New System.Windows.Forms.Padding(10)
        Me.TableLayoutPanel1.RowCount = 3
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(652, 327)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'RichTextBox1
        '
        Me.RichTextBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RichTextBox1.Location = New System.Drawing.Point(76, 42)
        Me.RichTextBox1.Name = "RichTextBox1"
        Me.RichTextBox1.ReadOnly = True
        Me.RichTextBox1.Size = New System.Drawing.Size(466, 231)
        Me.RichTextBox1.TabIndex = 0
        Me.RichTextBox1.Text = ""
        Me.RichTextBox1.WordWrap = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label1.Location = New System.Drawing.Point(13, 39)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(57, 237)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Disc Files" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "1 per line" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "in ORDER"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(548, 279)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(91, 35)
        Me.Button2.TabIndex = 3
        Me.Button2.Text = "Save"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'Button1
        '
        Me.Button1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Button1.Location = New System.Drawing.Point(548, 42)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(91, 231)
        Me.Button1.TabIndex = 1
        Me.Button1.Text = "Browse"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'cb_Platform
        '
        Me.cb_Platform.Dock = System.Windows.Forms.DockStyle.Fill
        Me.cb_Platform.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cb_Platform.FormattingEnabled = True
        Me.cb_Platform.Items.AddRange(New Object() {"PSX", "SS"})
        Me.cb_Platform.Location = New System.Drawing.Point(76, 13)
        Me.cb_Platform.Name = "cb_Platform"
        Me.cb_Platform.Size = New System.Drawing.Size(466, 21)
        Me.cb_Platform.TabIndex = 6
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label3.Location = New System.Drawing.Point(13, 10)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(57, 29)
        Me.Label3.TabIndex = 7
        Me.Label3.Text = "Platform"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'btnHelp
        '
        Me.btnHelp.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnHelp.Location = New System.Drawing.Point(548, 13)
        Me.btnHelp.Name = "btnHelp"
        Me.btnHelp.Size = New System.Drawing.Size(91, 23)
        Me.btnHelp.TabIndex = 8
        Me.btnHelp.Text = "What?"
        Me.btnHelp.UseVisualStyleBackColor = True
        '
        'frmMultiDiskCreator
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(652, 327)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Name = "frmMultiDiskCreator"
        Me.Text = "Mednafen MultiDisk Playlist Creator"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents Button1 As Button
    Friend WithEvents RichTextBox1 As RichTextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Button2 As Button
    Friend WithEvents cb_Platform As ComboBox
    Friend WithEvents Label3 As Label
    Friend WithEvents btnHelp As Button
End Class
