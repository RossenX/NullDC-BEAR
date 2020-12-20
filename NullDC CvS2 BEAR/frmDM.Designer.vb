<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmDM
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
        Me.tlpContainer = New System.Windows.Forms.TableLayoutPanel()
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.TLP_Messages = New System.Windows.Forms.TableLayoutPanel()
        Me.tlpInputBottom = New System.Windows.Forms.TableLayoutPanel()
        Me.InputBox = New System.Windows.Forms.RichTextBox()
        Me.btnSend = New System.Windows.Forms.Button()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.GagToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GagToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.tlpContainer.SuspendLayout()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.tlpInputBottom.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'tlpContainer
        '
        Me.tlpContainer.ColumnCount = 1
        Me.tlpContainer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpContainer.Controls.Add(Me.FlowLayoutPanel1, 0, 0)
        Me.tlpContainer.Controls.Add(Me.tlpInputBottom, 0, 1)
        Me.tlpContainer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpContainer.Location = New System.Drawing.Point(0, 0)
        Me.tlpContainer.Margin = New System.Windows.Forms.Padding(0)
        Me.tlpContainer.Name = "tlpContainer"
        Me.tlpContainer.Padding = New System.Windows.Forms.Padding(10)
        Me.tlpContainer.RowCount = 2
        Me.tlpContainer.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpContainer.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.tlpContainer.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.tlpContainer.Size = New System.Drawing.Size(462, 339)
        Me.tlpContainer.TabIndex = 0
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.AutoScroll = True
        Me.FlowLayoutPanel1.AutoSize = True
        Me.FlowLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.FlowLayoutPanel1.Controls.Add(Me.TLP_Messages)
        Me.FlowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FlowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.BottomUp
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(13, 13)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Padding = New System.Windows.Forms.Padding(0, 0, 10, 0)
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(436, 288)
        Me.FlowLayoutPanel1.TabIndex = 2
        '
        'TLP_Messages
        '
        Me.TLP_Messages.AutoSize = True
        Me.TLP_Messages.BackColor = System.Drawing.Color.Transparent
        Me.TLP_Messages.ColumnCount = 1
        Me.TLP_Messages.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TLP_Messages.Location = New System.Drawing.Point(3, 285)
        Me.TLP_Messages.Name = "TLP_Messages"
        Me.TLP_Messages.RowCount = 1
        Me.TLP_Messages.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TLP_Messages.Size = New System.Drawing.Size(0, 0)
        Me.TLP_Messages.TabIndex = 0
        '
        'tlpInputBottom
        '
        Me.tlpInputBottom.AutoSize = True
        Me.tlpInputBottom.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.tlpInputBottom.ColumnCount = 2
        Me.tlpInputBottom.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpInputBottom.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.tlpInputBottom.Controls.Add(Me.InputBox, 0, 0)
        Me.tlpInputBottom.Controls.Add(Me.btnSend, 1, 0)
        Me.tlpInputBottom.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpInputBottom.Location = New System.Drawing.Point(10, 304)
        Me.tlpInputBottom.Margin = New System.Windows.Forms.Padding(0)
        Me.tlpInputBottom.Name = "tlpInputBottom"
        Me.tlpInputBottom.RowCount = 1
        Me.tlpInputBottom.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpInputBottom.Size = New System.Drawing.Size(442, 25)
        Me.tlpInputBottom.TabIndex = 1
        '
        'InputBox
        '
        Me.InputBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.InputBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.InputBox.Location = New System.Drawing.Point(0, 0)
        Me.InputBox.Margin = New System.Windows.Forms.Padding(0)
        Me.InputBox.MaxLength = 256
        Me.InputBox.Name = "InputBox"
        Me.InputBox.Size = New System.Drawing.Size(350, 25)
        Me.InputBox.TabIndex = 0
        Me.InputBox.Text = ""
        '
        'btnSend
        '
        Me.btnSend.AutoSize = True
        Me.btnSend.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.btnSend.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnSend.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnSend.Location = New System.Drawing.Point(350, 0)
        Me.btnSend.Margin = New System.Windows.Forms.Padding(0)
        Me.btnSend.Name = "btnSend"
        Me.btnSend.Size = New System.Drawing.Size(92, 25)
        Me.btnSend.TabIndex = 1
        Me.btnSend.Text = "        Send        "
        Me.btnSend.UseVisualStyleBackColor = True
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.GagToolStripMenuItem})
        Me.MenuStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 339)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(462, 24)
        Me.MenuStrip1.TabIndex = 1
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'GagToolStripMenuItem
        '
        Me.GagToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.GagToolStripMenuItem1})
        Me.GagToolStripMenuItem.Name = "GagToolStripMenuItem"
        Me.GagToolStripMenuItem.ShowShortcutKeys = False
        Me.GagToolStripMenuItem.Size = New System.Drawing.Size(61, 20)
        Me.GagToolStripMenuItem.Text = "Options"
        '
        'GagToolStripMenuItem1
        '
        Me.GagToolStripMenuItem1.Name = "GagToolStripMenuItem1"
        Me.GagToolStripMenuItem1.Size = New System.Drawing.Size(95, 22)
        Me.GagToolStripMenuItem1.Text = "Gag"
        '
        'frmDM
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(462, 363)
        Me.Controls.Add(Me.tlpContainer)
        Me.Controls.Add(Me.MenuStrip1)
        Me.KeyPreview = True
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "frmDM"
        Me.Text = " <Name Of Player>"
        Me.tlpContainer.ResumeLayout(False)
        Me.tlpContainer.PerformLayout()
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.FlowLayoutPanel1.PerformLayout()
        Me.tlpInputBottom.ResumeLayout(False)
        Me.tlpInputBottom.PerformLayout()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents tlpContainer As TableLayoutPanel
    Friend WithEvents tlpInputBottom As TableLayoutPanel
    Friend WithEvents InputBox As RichTextBox
    Friend WithEvents btnSend As Button
    Friend WithEvents FlowLayoutPanel1 As FlowLayoutPanel
    Friend WithEvents TLP_Messages As TableLayoutPanel
    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents GagToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents GagToolStripMenuItem1 As ToolStripMenuItem
End Class
