<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMain
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Dim ListViewGroup3 As System.Windows.Forms.ListViewGroup = New System.Windows.Forms.ListViewGroup("Idle", System.Windows.Forms.HorizontalAlignment.Center)
        Dim ListViewGroup4 As System.Windows.Forms.ListViewGroup = New System.Windows.Forms.ListViewGroup("DND", System.Windows.Forms.HorizontalAlignment.Center)
        Me.Matchlist = New System.Windows.Forms.ListView()
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader3 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader4 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader5 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.OptionsToolStrip = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ChallengeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DownloadRomToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PingToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SystemIcons = New System.Windows.Forms.ImageList(Me.components)
        Me.btnDLC = New System.Windows.Forms.Button()
        Me.btnSearch = New System.Windows.Forms.Button()
        Me.BtnJoin = New System.Windows.Forms.Button()
        Me.btnOffline = New System.Windows.Forms.Button()
        Me.niBEAR = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.cbStatus = New System.Windows.Forms.ComboBox()
        Me.PlayerList = New System.Windows.Forms.ListView()
        Me.ColumnHeader6 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader7 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader8 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader9 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader10 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.TableLayoutPanel4 = New System.Windows.Forms.TableLayoutPanel()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.TableLayoutPanel5 = New System.Windows.Forms.TableLayoutPanel()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.TableLayoutPanel6 = New System.Windows.Forms.TableLayoutPanel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.lbVer = New System.Windows.Forms.Label()
        Me.MainMenuStrip = New System.Windows.Forms.MenuStrip()
        Me.ReplaysToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ControlsToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.OptionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DiscordToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PatreonO3oToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.imgBeta = New System.Windows.Forms.PictureBox()
        Me.sus_i = New System.Windows.Forms.PictureBox()
        Me.BEARTitle = New System.Windows.Forms.PictureBox()
        Me.OptionsToolStrip.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.TableLayoutPanel4.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.TableLayoutPanel5.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.TableLayoutPanel6.SuspendLayout()
        Me.MainMenuStrip.SuspendLayout()
        CType(Me.imgBeta, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.sus_i, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BEARTitle, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Matchlist
        '
        Me.Matchlist.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(100, Byte), Integer))
        Me.Matchlist.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.Matchlist.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1, Me.ColumnHeader2, Me.ColumnHeader3, Me.ColumnHeader4, Me.ColumnHeader5})
        Me.Matchlist.ContextMenuStrip = Me.OptionsToolStrip
        Me.Matchlist.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Matchlist.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Matchlist.ForeColor = System.Drawing.Color.Red
        Me.Matchlist.FullRowSelect = True
        Me.Matchlist.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
        Me.Matchlist.HideSelection = False
        Me.Matchlist.Location = New System.Drawing.Point(251, 40)
        Me.Matchlist.Margin = New System.Windows.Forms.Padding(0)
        Me.Matchlist.MultiSelect = False
        Me.Matchlist.Name = "Matchlist"
        Me.Matchlist.ShowItemToolTips = True
        Me.Matchlist.Size = New System.Drawing.Size(617, 416)
        Me.Matchlist.SmallImageList = Me.SystemIcons
        Me.Matchlist.Sorting = System.Windows.Forms.SortOrder.Ascending
        Me.Matchlist.TabIndex = 3
        Me.Matchlist.TileSize = New System.Drawing.Size(32, 32)
        Me.Matchlist.UseCompatibleStateImageBehavior = False
        Me.Matchlist.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.DisplayIndex = 1
        Me.ColumnHeader1.Text = "Player"
        Me.ColumnHeader1.Width = 168
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.DisplayIndex = 2
        Me.ColumnHeader2.Text = "IP"
        Me.ColumnHeader2.Width = 0
        '
        'ColumnHeader3
        '
        Me.ColumnHeader3.DisplayIndex = 0
        Me.ColumnHeader3.Text = "Ping"
        '
        'ColumnHeader4
        '
        Me.ColumnHeader4.Text = "Game"
        Me.ColumnHeader4.Width = 204
        '
        'ColumnHeader5
        '
        Me.ColumnHeader5.Text = "Status"
        Me.ColumnHeader5.Width = 70
        '
        'OptionsToolStrip
        '
        Me.OptionsToolStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ChallengeToolStripMenuItem, Me.DownloadRomToolStripMenuItem, Me.PingToolStripMenuItem})
        Me.OptionsToolStrip.Name = "OptionsToolStrip"
        Me.OptionsToolStrip.Size = New System.Drawing.Size(178, 70)
        '
        'ChallengeToolStripMenuItem
        '
        Me.ChallengeToolStripMenuItem.Name = "ChallengeToolStripMenuItem"
        Me.ChallengeToolStripMenuItem.Size = New System.Drawing.Size(177, 22)
        Me.ChallengeToolStripMenuItem.Text = "Challenge/Spectate"
        '
        'DownloadRomToolStripMenuItem
        '
        Me.DownloadRomToolStripMenuItem.Name = "DownloadRomToolStripMenuItem"
        Me.DownloadRomToolStripMenuItem.Size = New System.Drawing.Size(177, 22)
        Me.DownloadRomToolStripMenuItem.Text = "Download Rom"
        '
        'PingToolStripMenuItem
        '
        Me.PingToolStripMenuItem.Name = "PingToolStripMenuItem"
        Me.PingToolStripMenuItem.Size = New System.Drawing.Size(177, 22)
        Me.PingToolStripMenuItem.Text = "Ping"
        '
        'SystemIcons
        '
        Me.SystemIcons.ImageStream = CType(resources.GetObject("SystemIcons.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.SystemIcons.TransparentColor = System.Drawing.Color.Transparent
        Me.SystemIcons.Images.SetKeyName(0, "icon_Naomi.png")
        Me.SystemIcons.Images.SetKeyName(1, "icon_dreamcast.png")
        Me.SystemIcons.Images.SetKeyName(2, "Icon_SegaSaturn.png")
        Me.SystemIcons.Images.SetKeyName(3, "icon_Genesis.png")
        Me.SystemIcons.Images.SetKeyName(4, "icon_PSX.png")
        Me.SystemIcons.Images.SetKeyName(5, "icon_NES.png")
        Me.SystemIcons.Images.SetKeyName(6, "icon_SNES.png")
        Me.SystemIcons.Images.SetKeyName(7, "icon_fds.png")
        Me.SystemIcons.Images.SetKeyName(8, "icon_NGP.png")
        Me.SystemIcons.Images.SetKeyName(9, "icon_GBA.png")
        Me.SystemIcons.Images.SetKeyName(10, "icon_GBC.png")
        '
        'btnDLC
        '
        Me.btnDLC.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.btnDLC.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnDLC.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.btnDLC.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnDLC.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(1, Byte), Integer))
        Me.btnDLC.Location = New System.Drawing.Point(610, 0)
        Me.btnDLC.Margin = New System.Windows.Forms.Padding(0)
        Me.btnDLC.Name = "btnDLC"
        Me.btnDLC.Size = New System.Drawing.Size(123, 27)
        Me.btnDLC.TabIndex = 13
        Me.btnDLC.TabStop = False
        Me.btnDLC.Text = "Free DLC"
        Me.btnDLC.UseVisualStyleBackColor = False
        '
        'btnSearch
        '
        Me.btnSearch.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.btnSearch.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.btnSearch.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSearch.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(1, Byte), Integer))
        Me.btnSearch.Location = New System.Drawing.Point(241, 0)
        Me.btnSearch.Margin = New System.Windows.Forms.Padding(0)
        Me.btnSearch.Name = "btnSearch"
        Me.btnSearch.Size = New System.Drawing.Size(123, 27)
        Me.btnSearch.TabIndex = 4
        Me.btnSearch.TabStop = False
        Me.btnSearch.Text = "Refresh ↺"
        Me.btnSearch.UseVisualStyleBackColor = False
        '
        'BtnJoin
        '
        Me.BtnJoin.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.BtnJoin.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BtnJoin.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.BtnJoin.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnJoin.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(1, Byte), Integer))
        Me.BtnJoin.Location = New System.Drawing.Point(364, 0)
        Me.BtnJoin.Margin = New System.Windows.Forms.Padding(0)
        Me.BtnJoin.Name = "BtnJoin"
        Me.BtnJoin.Size = New System.Drawing.Size(123, 27)
        Me.BtnJoin.TabIndex = 5
        Me.BtnJoin.TabStop = False
        Me.BtnJoin.Text = "Challenge"
        Me.BtnJoin.UseVisualStyleBackColor = False
        '
        'btnOffline
        '
        Me.btnOffline.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.btnOffline.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnOffline.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.btnOffline.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnOffline.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(1, Byte), Integer))
        Me.btnOffline.Location = New System.Drawing.Point(733, 0)
        Me.btnOffline.Margin = New System.Windows.Forms.Padding(0)
        Me.btnOffline.Name = "btnOffline"
        Me.btnOffline.Size = New System.Drawing.Size(125, 27)
        Me.btnOffline.TabIndex = 7
        Me.btnOffline.TabStop = False
        Me.btnOffline.Text = "Play Offline"
        Me.btnOffline.UseVisualStyleBackColor = False
        '
        'niBEAR
        '
        Me.niBEAR.Text = "BEAR"
        '
        'cbStatus
        '
        Me.cbStatus.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(200, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.cbStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cbStatus.FormattingEnabled = True
        Me.cbStatus.Items.AddRange(New Object() {"Idle", "DND", "Hidden"})
        Me.cbStatus.Location = New System.Drawing.Point(3, 3)
        Me.cbStatus.Name = "cbStatus"
        Me.cbStatus.Size = New System.Drawing.Size(59, 21)
        Me.cbStatus.TabIndex = 15
        Me.cbStatus.TabStop = False
        '
        'PlayerList
        '
        Me.PlayerList.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(200, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.PlayerList.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.PlayerList.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader6, Me.ColumnHeader7, Me.ColumnHeader8, Me.ColumnHeader9, Me.ColumnHeader10})
        Me.PlayerList.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PlayerList.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.PlayerList.ForeColor = System.Drawing.Color.Black
        Me.PlayerList.FullRowSelect = True
        Me.PlayerList.GridLines = True
        ListViewGroup3.Header = "Idle"
        ListViewGroup3.HeaderAlignment = System.Windows.Forms.HorizontalAlignment.Center
        ListViewGroup3.Name = "Idle"
        ListViewGroup4.Header = "DND"
        ListViewGroup4.HeaderAlignment = System.Windows.Forms.HorizontalAlignment.Center
        ListViewGroup4.Name = "DND"
        Me.PlayerList.Groups.AddRange(New System.Windows.Forms.ListViewGroup() {ListViewGroup3, ListViewGroup4})
        Me.PlayerList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
        Me.PlayerList.HideSelection = False
        Me.PlayerList.Location = New System.Drawing.Point(10, 40)
        Me.PlayerList.Margin = New System.Windows.Forms.Padding(0)
        Me.PlayerList.MultiSelect = False
        Me.PlayerList.Name = "PlayerList"
        Me.PlayerList.ShowItemToolTips = True
        Me.PlayerList.Size = New System.Drawing.Size(241, 416)
        Me.PlayerList.Sorting = System.Windows.Forms.SortOrder.Ascending
        Me.PlayerList.TabIndex = 20
        Me.PlayerList.UseCompatibleStateImageBehavior = False
        Me.PlayerList.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader6
        '
        Me.ColumnHeader6.DisplayIndex = 1
        Me.ColumnHeader6.Text = "Player"
        Me.ColumnHeader6.Width = 160
        '
        'ColumnHeader7
        '
        Me.ColumnHeader7.DisplayIndex = 2
        Me.ColumnHeader7.Text = "IP"
        Me.ColumnHeader7.Width = 0
        '
        'ColumnHeader8
        '
        Me.ColumnHeader8.DisplayIndex = 0
        Me.ColumnHeader8.Text = "Ping"
        '
        'ColumnHeader9
        '
        Me.ColumnHeader9.Text = "Game"
        Me.ColumnHeader9.Width = 0
        '
        'ColumnHeader10
        '
        Me.ColumnHeader10.Text = "Hosting"
        Me.ColumnHeader10.Width = 0
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.ColumnCount = 2
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 241.0!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.Controls.Add(Me.TableLayoutPanel4, 1, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.Matchlist, 1, 1)
        Me.TableLayoutPanel2.Controls.Add(Me.PlayerList, 0, 1)
        Me.TableLayoutPanel2.Controls.Add(Me.Panel1, 0, 2)
        Me.TableLayoutPanel2.Controls.Add(Me.TableLayoutPanel1, 0, 0)
        Me.TableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(0, 24)
        Me.TableLayoutPanel2.Margin = New System.Windows.Forms.Padding(0)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.Padding = New System.Windows.Forms.Padding(10)
        Me.TableLayoutPanel2.RowCount = 3
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(878, 493)
        Me.TableLayoutPanel2.TabIndex = 21
        '
        'TableLayoutPanel4
        '
        Me.TableLayoutPanel4.ColumnCount = 1
        Me.TableLayoutPanel4.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel4.Controls.Add(Me.Label2, 0, 0)
        Me.TableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel4.Location = New System.Drawing.Point(251, 10)
        Me.TableLayoutPanel4.Margin = New System.Windows.Forms.Padding(0)
        Me.TableLayoutPanel4.Name = "TableLayoutPanel4"
        Me.TableLayoutPanel4.RowCount = 1
        Me.TableLayoutPanel4.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel4.Size = New System.Drawing.Size(617, 30)
        Me.TableLayoutPanel4.TabIndex = 24
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.FromArgb(CType(CType(250, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.Label2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.Black
        Me.Label2.Location = New System.Drawing.Point(0, 0)
        Me.Label2.Margin = New System.Windows.Forms.Padding(0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(617, 30)
        Me.Label2.TabIndex = 0
        Me.Label2.Text = "Current Online Games"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Panel1
        '
        Me.TableLayoutPanel2.SetColumnSpan(Me.Panel1, 2)
        Me.Panel1.Controls.Add(Me.TableLayoutPanel5)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(10, 456)
        Me.Panel1.Margin = New System.Windows.Forms.Padding(0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(858, 27)
        Me.Panel1.TabIndex = 22
        '
        'TableLayoutPanel5
        '
        Me.TableLayoutPanel5.ColumnCount = 6
        Me.TableLayoutPanel5.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 241.0!))
        Me.TableLayoutPanel5.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.0!))
        Me.TableLayoutPanel5.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.0!))
        Me.TableLayoutPanel5.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.0!))
        Me.TableLayoutPanel5.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.0!))
        Me.TableLayoutPanel5.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.0!))
        Me.TableLayoutPanel5.Controls.Add(Me.cbStatus, 0, 0)
        Me.TableLayoutPanel5.Controls.Add(Me.btnSearch, 1, 0)
        Me.TableLayoutPanel5.Controls.Add(Me.btnDLC, 4, 0)
        Me.TableLayoutPanel5.Controls.Add(Me.btnOffline, 5, 0)
        Me.TableLayoutPanel5.Controls.Add(Me.BtnJoin, 2, 0)
        Me.TableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel5.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel5.Name = "TableLayoutPanel5"
        Me.TableLayoutPanel5.RowCount = 1
        Me.TableLayoutPanel5.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel5.Size = New System.Drawing.Size(858, 27)
        Me.TableLayoutPanel5.TabIndex = 20
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 1
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.TableLayoutPanel6, 0, 0)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(10, 10)
        Me.TableLayoutPanel1.Margin = New System.Windows.Forms.Padding(0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(241, 30)
        Me.TableLayoutPanel1.TabIndex = 23
        '
        'TableLayoutPanel6
        '
        Me.TableLayoutPanel6.ColumnCount = 2
        Me.TableLayoutPanel6.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.TableLayoutPanel6.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75.0!))
        Me.TableLayoutPanel6.Controls.Add(Me.Label1, 0, 0)
        Me.TableLayoutPanel6.Controls.Add(Me.Label4, 1, 0)
        Me.TableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel6.ForeColor = System.Drawing.Color.Black
        Me.TableLayoutPanel6.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel6.Margin = New System.Windows.Forms.Padding(0)
        Me.TableLayoutPanel6.Name = "TableLayoutPanel6"
        Me.TableLayoutPanel6.RowCount = 1
        Me.TableLayoutPanel6.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel6.Size = New System.Drawing.Size(241, 30)
        Me.TableLayoutPanel6.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.FromArgb(CType(CType(250, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(0, 0)
        Me.Label1.Margin = New System.Windows.Forms.Padding(0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(60, 30)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Ping"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.BackColor = System.Drawing.Color.FromArgb(CType(CType(250, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.Label4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.ForeColor = System.Drawing.Color.Black
        Me.Label4.Location = New System.Drawing.Point(60, 0)
        Me.Label4.Margin = New System.Windows.Forms.Padding(0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(181, 30)
        Me.Label4.TabIndex = 1
        Me.Label4.Text = "Player"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lbVer
        '
        Me.lbVer.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lbVer.AutoSize = True
        Me.lbVer.BackColor = System.Drawing.Color.Transparent
        Me.lbVer.Location = New System.Drawing.Point(844, 0)
        Me.lbVer.Name = "lbVer"
        Me.lbVer.Size = New System.Drawing.Size(34, 13)
        Me.lbVer.TabIndex = 10
        Me.lbVer.Text = "0.00a"
        Me.lbVer.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'MainMenuStrip
        '
        Me.MainMenuStrip.BackColor = System.Drawing.Color.Transparent
        Me.MainMenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ReplaysToolStripMenuItem, Me.ControlsToolStripMenuItem1, Me.OptionsToolStripMenuItem, Me.DiscordToolStripMenuItem, Me.PatreonO3oToolStripMenuItem})
        Me.MainMenuStrip.Location = New System.Drawing.Point(0, 0)
        Me.MainMenuStrip.Name = "MainMenuStrip"
        Me.MainMenuStrip.Padding = New System.Windows.Forms.Padding(0)
        Me.MainMenuStrip.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.MainMenuStrip.Size = New System.Drawing.Size(878, 24)
        Me.MainMenuStrip.TabIndex = 22
        Me.MainMenuStrip.Text = "MainMenuStrip"
        '
        'ReplaysToolStripMenuItem
        '
        Me.ReplaysToolStripMenuItem.BackColor = System.Drawing.Color.Transparent
        Me.ReplaysToolStripMenuItem.Name = "ReplaysToolStripMenuItem"
        Me.ReplaysToolStripMenuItem.Size = New System.Drawing.Size(59, 24)
        Me.ReplaysToolStripMenuItem.Text = "Replays"
        '
        'ControlsToolStripMenuItem1
        '
        Me.ControlsToolStripMenuItem1.BackColor = System.Drawing.Color.Transparent
        Me.ControlsToolStripMenuItem1.Name = "ControlsToolStripMenuItem1"
        Me.ControlsToolStripMenuItem1.Size = New System.Drawing.Size(64, 24)
        Me.ControlsToolStripMenuItem1.Text = "Controls"
        '
        'OptionsToolStripMenuItem
        '
        Me.OptionsToolStripMenuItem.BackColor = System.Drawing.Color.Transparent
        Me.OptionsToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.OptionsToolStripMenuItem.ForeColor = System.Drawing.Color.Black
        Me.OptionsToolStripMenuItem.Name = "OptionsToolStripMenuItem"
        Me.OptionsToolStripMenuItem.Size = New System.Drawing.Size(61, 24)
        Me.OptionsToolStripMenuItem.Text = "Options"
        '
        'DiscordToolStripMenuItem
        '
        Me.DiscordToolStripMenuItem.BackColor = System.Drawing.Color.Transparent
        Me.DiscordToolStripMenuItem.Name = "DiscordToolStripMenuItem"
        Me.DiscordToolStripMenuItem.Size = New System.Drawing.Size(59, 24)
        Me.DiscordToolStripMenuItem.Text = "Discord"
        '
        'PatreonO3oToolStripMenuItem
        '
        Me.PatreonO3oToolStripMenuItem.BackColor = System.Drawing.Color.Transparent
        Me.PatreonO3oToolStripMenuItem.Name = "PatreonO3oToolStripMenuItem"
        Me.PatreonO3oToolStripMenuItem.Size = New System.Drawing.Size(83, 24)
        Me.PatreonO3oToolStripMenuItem.Text = "Patreon o3o"
        '
        'imgBeta
        '
        Me.imgBeta.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.imgBeta.BackColor = System.Drawing.Color.Transparent
        Me.imgBeta.BackgroundImage = Global.NullDC_CvS2_BEAR.My.Resources.Resources.Beta
        Me.imgBeta.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.imgBeta.Location = New System.Drawing.Point(775, -8)
        Me.imgBeta.Margin = New System.Windows.Forms.Padding(0)
        Me.imgBeta.Name = "imgBeta"
        Me.imgBeta.Size = New System.Drawing.Size(67, 34)
        Me.imgBeta.TabIndex = 11
        Me.imgBeta.TabStop = False
        '
        'sus_i
        '
        Me.sus_i.BackColor = System.Drawing.Color.Transparent
        Me.sus_i.BackgroundImage = Global.NullDC_CvS2_BEAR.My.Resources.Resources.sus
        Me.sus_i.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.sus_i.Location = New System.Drawing.Point(743, -1)
        Me.sus_i.Margin = New System.Windows.Forms.Padding(0)
        Me.sus_i.Name = "sus_i"
        Me.sus_i.Size = New System.Drawing.Size(32, 29)
        Me.sus_i.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.sus_i.TabIndex = 18
        Me.sus_i.TabStop = False
        '
        'BEARTitle
        '
        Me.BEARTitle.BackColor = System.Drawing.Color.Transparent
        Me.BEARTitle.BackgroundImage = Global.NullDC_CvS2_BEAR.My.Resources.Resources.NullDCBEAR_Title
        Me.BEARTitle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.BEARTitle.Location = New System.Drawing.Point(493, 0)
        Me.BEARTitle.Name = "BEARTitle"
        Me.BEARTitle.Size = New System.Drawing.Size(58, 26)
        Me.BEARTitle.TabIndex = 17
        Me.BEARTitle.TabStop = False
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(250, Byte), Integer), CType(CType(200, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(878, 517)
        Me.Controls.Add(Me.sus_i)
        Me.Controls.Add(Me.imgBeta)
        Me.Controls.Add(Me.BEARTitle)
        Me.Controls.Add(Me.TableLayoutPanel2)
        Me.Controls.Add(Me.lbVer)
        Me.Controls.Add(Me.MainMenuStrip)
        Me.DoubleBuffered = True
        Me.Name = "frmMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "NullDC BEAR"
        Me.OptionsToolStrip.ResumeLayout(False)
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.TableLayoutPanel4.ResumeLayout(False)
        Me.TableLayoutPanel4.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.TableLayoutPanel5.ResumeLayout(False)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel6.ResumeLayout(False)
        Me.TableLayoutPanel6.PerformLayout()
        Me.MainMenuStrip.ResumeLayout(False)
        Me.MainMenuStrip.PerformLayout()
        CType(Me.imgBeta, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.sus_i, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BEARTitle, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Matchlist As ListView
    Friend WithEvents btnSearch As Button
    Friend WithEvents ColumnHeader1 As ColumnHeader
    Friend WithEvents ColumnHeader2 As ColumnHeader
    Friend WithEvents ColumnHeader3 As ColumnHeader
    Friend WithEvents ColumnHeader4 As ColumnHeader
    Friend WithEvents BtnJoin As Button
    Friend WithEvents ColumnHeader5 As ColumnHeader
    Friend WithEvents btnOffline As Button
    Friend WithEvents niBEAR As NotifyIcon
    Friend WithEvents btnDLC As Button
    Friend WithEvents cbStatus As ComboBox
    Friend WithEvents PlayerList As ListView
    Friend WithEvents ColumnHeader6 As ColumnHeader
    Friend WithEvents ColumnHeader7 As ColumnHeader
    Friend WithEvents ColumnHeader8 As ColumnHeader
    Friend WithEvents ColumnHeader9 As ColumnHeader
    Friend WithEvents ColumnHeader10 As ColumnHeader
    Friend WithEvents TableLayoutPanel2 As TableLayoutPanel
    Friend WithEvents Panel1 As Panel
    Friend WithEvents lbVer As Label
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents TableLayoutPanel4 As TableLayoutPanel
    Friend WithEvents Label2 As Label
    Friend WithEvents TableLayoutPanel5 As TableLayoutPanel
    Friend WithEvents SystemIcons As ImageList
    Friend WithEvents MainMenuStrip As MenuStrip
    Friend WithEvents OptionsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ReplaysToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents TableLayoutPanel6 As TableLayoutPanel
    Friend WithEvents Label1 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents ControlsToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents DiscordToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents PatreonO3oToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OptionsToolStrip As ContextMenuStrip
    Friend WithEvents PingToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ChallengeToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents DownloadRomToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents imgBeta As PictureBox
    Friend WithEvents sus_i As PictureBox
    Friend WithEvents BEARTitle As PictureBox
End Class
