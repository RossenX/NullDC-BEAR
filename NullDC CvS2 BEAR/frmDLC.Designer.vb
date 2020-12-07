<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmDLC
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
        Me.btnDownload = New System.Windows.Forms.Button()
        Me.DLCContainer = New System.Windows.Forms.TableLayoutPanel()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.btnRomsFolder = New System.Windows.Forms.Button()
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar()
        Me.lnkRoms = New System.Windows.Forms.LinkLabel()
        Me.tc_games = New System.Windows.Forms.TabControl()
        Me.lbDisclaimer = New System.Windows.Forms.Label()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.DLCCreatorToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MultidiskPlaylistCreatorToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DLCContainer.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnDownload
        '
        Me.btnDownload.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.DLCContainer.SetColumnSpan(Me.btnDownload, 2)
        Me.btnDownload.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnDownload.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.btnDownload.Location = New System.Drawing.Point(10, 439)
        Me.btnDownload.Margin = New System.Windows.Forms.Padding(0)
        Me.btnDownload.Name = "btnDownload"
        Me.btnDownload.Size = New System.Drawing.Size(638, 41)
        Me.btnDownload.TabIndex = 1
        Me.btnDownload.Text = "Download"
        Me.btnDownload.UseVisualStyleBackColor = False
        '
        'DLCContainer
        '
        Me.DLCContainer.ColumnCount = 2
        Me.DLCContainer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 21.78683!))
        Me.DLCContainer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 78.21317!))
        Me.DLCContainer.Controls.Add(Me.btnClose, 0, 4)
        Me.DLCContainer.Controls.Add(Me.btnRomsFolder, 1, 4)
        Me.DLCContainer.Controls.Add(Me.btnDownload, 0, 2)
        Me.DLCContainer.Controls.Add(Me.ProgressBar1, 0, 1)
        Me.DLCContainer.Controls.Add(Me.lnkRoms, 0, 3)
        Me.DLCContainer.Controls.Add(Me.tc_games, 0, 0)
        Me.DLCContainer.Controls.Add(Me.lbDisclaimer, 0, 5)
        Me.DLCContainer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DLCContainer.Location = New System.Drawing.Point(0, 24)
        Me.DLCContainer.Name = "DLCContainer"
        Me.DLCContainer.Padding = New System.Windows.Forms.Padding(10)
        Me.DLCContainer.RowCount = 6
        Me.DLCContainer.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.DLCContainer.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.DLCContainer.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.DLCContainer.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.DLCContainer.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.DLCContainer.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.DLCContainer.Size = New System.Drawing.Size(658, 557)
        Me.DLCContainer.TabIndex = 2
        '
        'btnClose
        '
        Me.btnClose.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.btnClose.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.btnClose.Location = New System.Drawing.Point(10, 493)
        Me.btnClose.Margin = New System.Windows.Forms.Padding(0)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(138, 41)
        Me.btnClose.TabIndex = 8
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = False
        '
        'btnRomsFolder
        '
        Me.btnRomsFolder.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.btnRomsFolder.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnRomsFolder.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.btnRomsFolder.Location = New System.Drawing.Point(148, 493)
        Me.btnRomsFolder.Margin = New System.Windows.Forms.Padding(0)
        Me.btnRomsFolder.Name = "btnRomsFolder"
        Me.btnRomsFolder.Size = New System.Drawing.Size(500, 41)
        Me.btnRomsFolder.TabIndex = 6
        Me.btnRomsFolder.Text = "Open Naomi/Atomiswave Roms Folder"
        Me.btnRomsFolder.UseVisualStyleBackColor = False
        '
        'ProgressBar1
        '
        Me.DLCContainer.SetColumnSpan(Me.ProgressBar1, 2)
        Me.ProgressBar1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ProgressBar1.Location = New System.Drawing.Point(10, 416)
        Me.ProgressBar1.Margin = New System.Windows.Forms.Padding(0)
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(638, 23)
        Me.ProgressBar1.TabIndex = 3
        Me.ProgressBar1.Visible = False
        '
        'lnkRoms
        '
        Me.lnkRoms.AutoSize = True
        Me.DLCContainer.SetColumnSpan(Me.lnkRoms, 2)
        Me.lnkRoms.Location = New System.Drawing.Point(13, 480)
        Me.lnkRoms.Name = "lnkRoms"
        Me.lnkRoms.Size = New System.Drawing.Size(244, 13)
        Me.lnkRoms.TabIndex = 4
        Me.lnkRoms.TabStop = True
        Me.lnkRoms.Text = "Can't download? Click here to manually get games"
        '
        'tc_games
        '
        Me.DLCContainer.SetColumnSpan(Me.tc_games, 2)
        Me.tc_games.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tc_games.Location = New System.Drawing.Point(10, 10)
        Me.tc_games.Margin = New System.Windows.Forms.Padding(0)
        Me.tc_games.Multiline = True
        Me.tc_games.Name = "tc_games"
        Me.tc_games.SelectedIndex = 0
        Me.tc_games.Size = New System.Drawing.Size(638, 406)
        Me.tc_games.TabIndex = 7
        '
        'lbDisclaimer
        '
        Me.lbDisclaimer.AutoSize = True
        Me.DLCContainer.SetColumnSpan(Me.lbDisclaimer, 2)
        Me.lbDisclaimer.Location = New System.Drawing.Point(13, 534)
        Me.lbDisclaimer.Name = "lbDisclaimer"
        Me.lbDisclaimer.Size = New System.Drawing.Size(194, 13)
        Me.lbDisclaimer.TabIndex = 9
        Me.lbDisclaimer.Text = "Disclaimer: Some games might not work" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DLCCreatorToolStripMenuItem, Me.MultidiskPlaylistCreatorToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(658, 24)
        Me.MenuStrip1.TabIndex = 3
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'DLCCreatorToolStripMenuItem
        '
        Me.DLCCreatorToolStripMenuItem.Name = "DLCCreatorToolStripMenuItem"
        Me.DLCCreatorToolStripMenuItem.Size = New System.Drawing.Size(83, 20)
        Me.DLCCreatorToolStripMenuItem.Text = "DLC Creator"
        '
        'MultidiskPlaylistCreatorToolStripMenuItem
        '
        Me.MultidiskPlaylistCreatorToolStripMenuItem.Name = "MultidiskPlaylistCreatorToolStripMenuItem"
        Me.MultidiskPlaylistCreatorToolStripMenuItem.Size = New System.Drawing.Size(150, 20)
        Me.MultidiskPlaylistCreatorToolStripMenuItem.Text = "Multidisk Playlist Creator"
        '
        'frmDLC
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(658, 581)
        Me.Controls.Add(Me.DLCContainer)
        Me.Controls.Add(Me.MenuStrip1)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "frmDLC"
        Me.Text = "Downloadable Content"
        Me.DLCContainer.ResumeLayout(False)
        Me.DLCContainer.PerformLayout()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnDownload As Button
    Friend WithEvents DLCContainer As TableLayoutPanel
    Friend WithEvents ProgressBar1 As ProgressBar
    Friend WithEvents lnkRoms As LinkLabel
    Friend WithEvents btnRomsFolder As Button
    Friend WithEvents tc_games As TabControl
    Friend WithEvents btnClose As Button
    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents lbDisclaimer As Label
    Friend WithEvents DLCCreatorToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents MultidiskPlaylistCreatorToolStripMenuItem As ToolStripMenuItem
End Class
