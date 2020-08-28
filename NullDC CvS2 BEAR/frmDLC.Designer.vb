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
        Me.lvGamesList_naomi = New System.Windows.Forms.ListView()
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.btnDownload = New System.Windows.Forms.Button()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.btnRomsFolder = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar()
        Me.lnkRoms = New System.Windows.Forms.LinkLabel()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.tc_games = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.lvGamelist_Atomiswave = New System.Windows.Forms.ListView()
        Me.ColumnHeader3 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader4 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.TabPage3 = New System.Windows.Forms.TabPage()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.LinkLabel1 = New System.Windows.Forms.LinkLabel()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.tc_games.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.TabPage3.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'lvGamesList_naomi
        '
        Me.lvGamesList_naomi.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(200, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.lvGamesList_naomi.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lvGamesList_naomi.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1, Me.ColumnHeader2})
        Me.lvGamesList_naomi.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lvGamesList_naomi.FullRowSelect = True
        Me.lvGamesList_naomi.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
        Me.lvGamesList_naomi.HideSelection = False
        Me.lvGamesList_naomi.Location = New System.Drawing.Point(0, 0)
        Me.lvGamesList_naomi.MultiSelect = False
        Me.lvGamesList_naomi.Name = "lvGamesList_naomi"
        Me.lvGamesList_naomi.Size = New System.Drawing.Size(420, 327)
        Me.lvGamesList_naomi.TabIndex = 0
        Me.lvGamesList_naomi.UseCompatibleStateImageBehavior = False
        Me.lvGamesList_naomi.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Width = 380
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Width = 0
        '
        'btnDownload
        '
        Me.btnDownload.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.TableLayoutPanel1.SetColumnSpan(Me.btnDownload, 2)
        Me.btnDownload.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnDownload.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.btnDownload.Location = New System.Drawing.Point(0, 401)
        Me.btnDownload.Margin = New System.Windows.Forms.Padding(0)
        Me.btnDownload.Name = "btnDownload"
        Me.btnDownload.Size = New System.Drawing.Size(428, 41)
        Me.btnDownload.TabIndex = 1
        Me.btnDownload.Text = "Download"
        Me.btnDownload.UseVisualStyleBackColor = False
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 73.21814!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 26.78186!))
        Me.TableLayoutPanel1.Controls.Add(Me.btnRomsFolder, 0, 5)
        Me.TableLayoutPanel1.Controls.Add(Me.Label1, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.btnDownload, 0, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.ProgressBar1, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.lnkRoms, 0, 4)
        Me.TableLayoutPanel1.Controls.Add(Me.Label2, 0, 5)
        Me.TableLayoutPanel1.Controls.Add(Me.tc_games, 0, 1)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 6
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(428, 496)
        Me.TableLayoutPanel1.TabIndex = 2
        '
        'btnRomsFolder
        '
        Me.btnRomsFolder.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.btnRomsFolder.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnRomsFolder.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.btnRomsFolder.Location = New System.Drawing.Point(0, 455)
        Me.btnRomsFolder.Margin = New System.Windows.Forms.Padding(0)
        Me.btnRomsFolder.Name = "btnRomsFolder"
        Me.btnRomsFolder.Size = New System.Drawing.Size(313, 41)
        Me.btnRomsFolder.TabIndex = 6
        Me.btnRomsFolder.Text = "Open Naomi/Atomiswave Roms Folder"
        Me.btnRomsFolder.UseVisualStyleBackColor = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.TableLayoutPanel1.SetColumnSpan(Me.Label1, 2)
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold)
        Me.Label1.Location = New System.Drawing.Point(0, 0)
        Me.Label1.Margin = New System.Windows.Forms.Padding(0)
        Me.Label1.Name = "Label1"
        Me.Label1.Padding = New System.Windows.Forms.Padding(5)
        Me.Label1.Size = New System.Drawing.Size(428, 25)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Downloadable Games"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ProgressBar1
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.ProgressBar1, 2)
        Me.ProgressBar1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ProgressBar1.Location = New System.Drawing.Point(0, 378)
        Me.ProgressBar1.Margin = New System.Windows.Forms.Padding(0)
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(428, 23)
        Me.ProgressBar1.TabIndex = 3
        Me.ProgressBar1.Visible = False
        '
        'lnkRoms
        '
        Me.lnkRoms.AutoSize = True
        Me.TableLayoutPanel1.SetColumnSpan(Me.lnkRoms, 2)
        Me.lnkRoms.Location = New System.Drawing.Point(3, 442)
        Me.lnkRoms.Name = "lnkRoms"
        Me.lnkRoms.Size = New System.Drawing.Size(244, 13)
        Me.lnkRoms.TabIndex = 4
        Me.lnkRoms.TabStop = True
        Me.lnkRoms.Text = "Can't download? Click here to manually get games"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(316, 455)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(91, 39)
        Me.Label2.TabIndex = 5
        Me.Label2.Text = "Disclaimer: Some games might not work Online. "
        '
        'tc_games
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.tc_games, 2)
        Me.tc_games.Controls.Add(Me.TabPage1)
        Me.tc_games.Controls.Add(Me.TabPage2)
        Me.tc_games.Controls.Add(Me.TabPage3)
        Me.tc_games.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tc_games.Location = New System.Drawing.Point(0, 25)
        Me.tc_games.Margin = New System.Windows.Forms.Padding(0)
        Me.tc_games.Name = "tc_games"
        Me.tc_games.SelectedIndex = 0
        Me.tc_games.Size = New System.Drawing.Size(428, 353)
        Me.tc_games.TabIndex = 7
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.lvGamesList_naomi)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Margin = New System.Windows.Forms.Padding(0)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Size = New System.Drawing.Size(420, 327)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Naomi"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.lvGamelist_Atomiswave)
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Margin = New System.Windows.Forms.Padding(0)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Size = New System.Drawing.Size(420, 327)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "Atomiswave"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'lvGamelist_Atomiswave
        '
        Me.lvGamelist_Atomiswave.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(200, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.lvGamelist_Atomiswave.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lvGamelist_Atomiswave.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader3, Me.ColumnHeader4})
        Me.lvGamelist_Atomiswave.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lvGamelist_Atomiswave.FullRowSelect = True
        Me.lvGamelist_Atomiswave.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
        Me.lvGamelist_Atomiswave.HideSelection = False
        Me.lvGamelist_Atomiswave.Location = New System.Drawing.Point(0, 0)
        Me.lvGamelist_Atomiswave.MultiSelect = False
        Me.lvGamelist_Atomiswave.Name = "lvGamelist_Atomiswave"
        Me.lvGamelist_Atomiswave.Size = New System.Drawing.Size(420, 327)
        Me.lvGamelist_Atomiswave.TabIndex = 1
        Me.lvGamelist_Atomiswave.UseCompatibleStateImageBehavior = False
        Me.lvGamelist_Atomiswave.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader3
        '
        Me.ColumnHeader3.Width = 380
        '
        'ColumnHeader4
        '
        Me.ColumnHeader4.Width = 0
        '
        'TabPage3
        '
        Me.TabPage3.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(200, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.TabPage3.Controls.Add(Me.TableLayoutPanel2)
        Me.TabPage3.Location = New System.Drawing.Point(4, 22)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage3.Size = New System.Drawing.Size(420, 327)
        Me.TabPage3.TabIndex = 2
        Me.TabPage3.Text = "Dreamcast"
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.ColumnCount = 1
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel2.Controls.Add(Me.LinkLabel1, 0, 0)
        Me.TableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(3, 3)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 1
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(414, 321)
        Me.TableLayoutPanel2.TabIndex = 0
        '
        'LinkLabel1
        '
        Me.LinkLabel1.AutoSize = True
        Me.LinkLabel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LinkLabel1.Location = New System.Drawing.Point(3, 0)
        Me.LinkLabel1.Name = "LinkLabel1"
        Me.LinkLabel1.Size = New System.Drawing.Size(408, 321)
        Me.LinkLabel1.TabIndex = 5
        Me.LinkLabel1.TabStop = True
        Me.LinkLabel1.Text = "Click here to get games" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "NullDC REQUIRES .CDI FILES, PLEASE ONLY GET THE CDI FILE" &
    "S." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        Me.LinkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'frmDLC
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(200, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(428, 496)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "frmDLC"
        Me.Text = "Downloadable Content"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.tc_games.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage2.ResumeLayout(False)
        Me.TabPage3.ResumeLayout(False)
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.TableLayoutPanel2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents lvGamesList_naomi As ListView
    Friend WithEvents btnDownload As Button
    Friend WithEvents ColumnHeader1 As ColumnHeader
    Friend WithEvents ColumnHeader2 As ColumnHeader
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents Label1 As Label
    Friend WithEvents ProgressBar1 As ProgressBar
    Friend WithEvents lnkRoms As LinkLabel
    Friend WithEvents Label2 As Label
    Friend WithEvents btnRomsFolder As Button
    Friend WithEvents tc_games As TabControl
    Friend WithEvents TabPage1 As TabPage
    Friend WithEvents TabPage2 As TabPage
    Friend WithEvents lvGamelist_Atomiswave As ListView
    Friend WithEvents ColumnHeader3 As ColumnHeader
    Friend WithEvents ColumnHeader4 As ColumnHeader
    Friend WithEvents TabPage3 As TabPage
    Friend WithEvents TableLayoutPanel2 As TableLayoutPanel
    Friend WithEvents LinkLabel1 As LinkLabel
End Class
