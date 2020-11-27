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
        Me.Label1 = New System.Windows.Forms.Label()
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar()
        Me.lnkRoms = New System.Windows.Forms.LinkLabel()
        Me.tc_games = New System.Windows.Forms.TabControl()
        Me.DLCContainer.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnDownload
        '
        Me.btnDownload.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.DLCContainer.SetColumnSpan(Me.btnDownload, 2)
        Me.btnDownload.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnDownload.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.btnDownload.Location = New System.Drawing.Point(10, 476)
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
        Me.DLCContainer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 73.21814!))
        Me.DLCContainer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 26.78186!))
        Me.DLCContainer.Controls.Add(Me.btnClose, 1, 5)
        Me.DLCContainer.Controls.Add(Me.btnRomsFolder, 0, 5)
        Me.DLCContainer.Controls.Add(Me.Label1, 0, 0)
        Me.DLCContainer.Controls.Add(Me.btnDownload, 0, 3)
        Me.DLCContainer.Controls.Add(Me.ProgressBar1, 0, 2)
        Me.DLCContainer.Controls.Add(Me.lnkRoms, 0, 4)
        Me.DLCContainer.Controls.Add(Me.tc_games, 0, 1)
        Me.DLCContainer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DLCContainer.Location = New System.Drawing.Point(0, 0)
        Me.DLCContainer.Name = "DLCContainer"
        Me.DLCContainer.Padding = New System.Windows.Forms.Padding(10)
        Me.DLCContainer.RowCount = 6
        Me.DLCContainer.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.DLCContainer.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.DLCContainer.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.DLCContainer.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.DLCContainer.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.DLCContainer.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.DLCContainer.Size = New System.Drawing.Size(658, 581)
        Me.DLCContainer.TabIndex = 2
        '
        'btnClose
        '
        Me.btnClose.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.btnClose.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.btnClose.Location = New System.Drawing.Point(477, 530)
        Me.btnClose.Margin = New System.Windows.Forms.Padding(0)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(171, 41)
        Me.btnClose.TabIndex = 8
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = False
        '
        'btnRomsFolder
        '
        Me.btnRomsFolder.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.btnRomsFolder.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnRomsFolder.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.btnRomsFolder.Location = New System.Drawing.Point(10, 530)
        Me.btnRomsFolder.Margin = New System.Windows.Forms.Padding(0)
        Me.btnRomsFolder.Name = "btnRomsFolder"
        Me.btnRomsFolder.Size = New System.Drawing.Size(467, 41)
        Me.btnRomsFolder.TabIndex = 6
        Me.btnRomsFolder.Text = "Open Naomi/Atomiswave Roms Folder"
        Me.btnRomsFolder.UseVisualStyleBackColor = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.DLCContainer.SetColumnSpan(Me.Label1, 2)
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold)
        Me.Label1.Location = New System.Drawing.Point(10, 10)
        Me.Label1.Margin = New System.Windows.Forms.Padding(0)
        Me.Label1.Name = "Label1"
        Me.Label1.Padding = New System.Windows.Forms.Padding(5)
        Me.Label1.Size = New System.Drawing.Size(638, 25)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Downloadable Games"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ProgressBar1
        '
        Me.DLCContainer.SetColumnSpan(Me.ProgressBar1, 2)
        Me.ProgressBar1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ProgressBar1.Location = New System.Drawing.Point(10, 453)
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
        Me.lnkRoms.Location = New System.Drawing.Point(13, 517)
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
        Me.tc_games.Location = New System.Drawing.Point(10, 35)
        Me.tc_games.Margin = New System.Windows.Forms.Padding(0)
        Me.tc_games.Multiline = True
        Me.tc_games.Name = "tc_games"
        Me.tc_games.SelectedIndex = 0
        Me.tc_games.Size = New System.Drawing.Size(638, 418)
        Me.tc_games.TabIndex = 7
        '
        'frmDLC
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(658, 581)
        Me.Controls.Add(Me.DLCContainer)
        Me.Name = "frmDLC"
        Me.Text = "Downloadable Content"
        Me.DLCContainer.ResumeLayout(False)
        Me.DLCContainer.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btnDownload As Button
    Friend WithEvents DLCContainer As TableLayoutPanel
    Friend WithEvents Label1 As Label
    Friend WithEvents ProgressBar1 As ProgressBar
    Friend WithEvents lnkRoms As LinkLabel
    Friend WithEvents btnRomsFolder As Button
    Friend WithEvents tc_games As TabControl
    Friend WithEvents btnClose As Button
End Class
