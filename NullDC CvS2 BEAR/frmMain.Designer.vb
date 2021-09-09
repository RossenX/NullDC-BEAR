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
        Dim ListViewGroup1 As System.Windows.Forms.ListViewGroup = New System.Windows.Forms.ListViewGroup("Idle", System.Windows.Forms.HorizontalAlignment.Center)
        Dim ListViewGroup2 As System.Windows.Forms.ListViewGroup = New System.Windows.Forms.ListViewGroup("DND", System.Windows.Forms.HorizontalAlignment.Center)
        Me.Matchlist = New System.Windows.Forms.ListView()
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader3 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader4 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader5 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.UserNameRadminIPToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ChallengeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DMToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BlockToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SystemIcons = New System.Windows.Forms.ImageList(Me.components)
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
        Me.MainMenuContainer = New System.Windows.Forms.TableLayoutPanel()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lbVer = New System.Windows.Forms.Label()
        Me._MainMenuStrip = New System.Windows.Forms.MenuStrip()
        Me.ToolStripLogo = New System.Windows.Forms.ToolStripMenuItem()
        Me.ReplaysToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ControlsToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.OptionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GeneralToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MednafenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GaggedUsersToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FreeDLCToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DiscordToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PatreonO3oToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExtrasToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MVC2TierListToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenFlycastDataFolderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ForceOpenPanelToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.HostToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.WaitingToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.imgBeta = New System.Windows.Forms.PictureBox()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.MainMenuContainer.SuspendLayout()
        Me._MainMenuStrip.SuspendLayout()
        CType(Me.imgBeta, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Matchlist
        '
        Me.Matchlist.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(100, Byte), Integer))
        Me.Matchlist.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.Matchlist.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1, Me.ColumnHeader2, Me.ColumnHeader3, Me.ColumnHeader4, Me.ColumnHeader5})
        Me.MainMenuContainer.SetColumnSpan(Me.Matchlist, 4)
        Me.Matchlist.ContextMenuStrip = Me.ContextMenuStrip1
        Me.Matchlist.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Matchlist.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Matchlist.ForeColor = System.Drawing.Color.Red
        Me.Matchlist.FullRowSelect = True
        Me.Matchlist.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
        Me.Matchlist.HideSelection = False
        Me.Matchlist.LargeImageList = Me.SystemIcons
        Me.Matchlist.Location = New System.Drawing.Point(261, 37)
        Me.Matchlist.Margin = New System.Windows.Forms.Padding(2, 2, 0, 0)
        Me.Matchlist.MultiSelect = False
        Me.Matchlist.Name = "Matchlist"
        Me.Matchlist.ShowItemToolTips = True
        Me.Matchlist.Size = New System.Drawing.Size(701, 406)
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
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.UserNameRadminIPToolStripMenuItem, Me.ChallengeToolStripMenuItem, Me.DMToolStripMenuItem, Me.BlockToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.ShowImageMargin = False
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(168, 92)
        '
        'UserNameRadminIPToolStripMenuItem
        '
        Me.UserNameRadminIPToolStripMenuItem.Name = "UserNameRadminIPToolStripMenuItem"
        Me.UserNameRadminIPToolStripMenuItem.Size = New System.Drawing.Size(167, 22)
        Me.UserNameRadminIPToolStripMenuItem.Text = "UserName | Radmin IP"
        '
        'ChallengeToolStripMenuItem
        '
        Me.ChallengeToolStripMenuItem.Name = "ChallengeToolStripMenuItem"
        Me.ChallengeToolStripMenuItem.Size = New System.Drawing.Size(167, 22)
        Me.ChallengeToolStripMenuItem.Text = "Challenge"
        '
        'DMToolStripMenuItem
        '
        Me.DMToolStripMenuItem.Name = "DMToolStripMenuItem"
        Me.DMToolStripMenuItem.Size = New System.Drawing.Size(167, 22)
        Me.DMToolStripMenuItem.Text = "DM"
        '
        'BlockToolStripMenuItem
        '
        Me.BlockToolStripMenuItem.Name = "BlockToolStripMenuItem"
        Me.BlockToolStripMenuItem.Size = New System.Drawing.Size(167, 22)
        Me.BlockToolStripMenuItem.Text = "Gag(Block)"
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
        Me.SystemIcons.Images.SetKeyName(11, "Icon_SMS.png")
        Me.SystemIcons.Images.SetKeyName(12, "icon_supergrafx.png")
        Me.SystemIcons.Images.SetKeyName(13, "icon_N64.png")
        Me.SystemIcons.Images.SetKeyName(14, "icon_Flycast.png")
        '
        'btnSearch
        '
        Me.btnSearch.AutoSize = True
        Me.btnSearch.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.btnSearch.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.btnSearch.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSearch.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(1, Byte), Integer))
        Me.btnSearch.Location = New System.Drawing.Point(261, 443)
        Me.btnSearch.Margin = New System.Windows.Forms.Padding(2, 0, 0, 0)
        Me.btnSearch.Name = "btnSearch"
        Me.btnSearch.Size = New System.Drawing.Size(163, 36)
        Me.btnSearch.TabIndex = 4
        Me.btnSearch.TabStop = False
        Me.btnSearch.Text = "Refresh ↺"
        Me.btnSearch.UseVisualStyleBackColor = False
        '
        'BtnJoin
        '
        Me.BtnJoin.AutoSize = True
        Me.BtnJoin.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.BtnJoin.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BtnJoin.Enabled = False
        Me.BtnJoin.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.BtnJoin.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnJoin.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(1, Byte), Integer))
        Me.BtnJoin.Location = New System.Drawing.Point(424, 443)
        Me.BtnJoin.Margin = New System.Windows.Forms.Padding(0)
        Me.BtnJoin.Name = "BtnJoin"
        Me.BtnJoin.Size = New System.Drawing.Size(161, 36)
        Me.BtnJoin.TabIndex = 5
        Me.BtnJoin.TabStop = False
        Me.BtnJoin.Text = "Challenge"
        Me.BtnJoin.UseVisualStyleBackColor = False
        '
        'btnOffline
        '
        Me.btnOffline.AutoSize = True
        Me.btnOffline.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.btnOffline.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnOffline.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.btnOffline.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnOffline.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(1, Byte), Integer))
        Me.btnOffline.Location = New System.Drawing.Point(772, 443)
        Me.btnOffline.Margin = New System.Windows.Forms.Padding(0)
        Me.btnOffline.Name = "btnOffline"
        Me.btnOffline.Size = New System.Drawing.Size(190, 36)
        Me.btnOffline.TabIndex = 7
        Me.btnOffline.TabStop = False
        Me.btnOffline.Text = "Play"
        Me.btnOffline.UseVisualStyleBackColor = False
        '
        'niBEAR
        '
        Me.niBEAR.Text = "BEAR"
        '
        'cbStatus
        '
        Me.cbStatus.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(200, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.cbStatus.Dock = System.Windows.Forms.DockStyle.Fill
        Me.cbStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbStatus.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cbStatus.FormattingEnabled = True
        Me.cbStatus.Items.AddRange(New Object() {"Idle", "DND", "Hidden"})
        Me.cbStatus.Location = New System.Drawing.Point(10, 443)
        Me.cbStatus.Margin = New System.Windows.Forms.Padding(0)
        Me.cbStatus.Name = "cbStatus"
        Me.cbStatus.Size = New System.Drawing.Size(75, 21)
        Me.cbStatus.TabIndex = 15
        Me.cbStatus.TabStop = False
        '
        'PlayerList
        '
        Me.PlayerList.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(200, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.PlayerList.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.PlayerList.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader6, Me.ColumnHeader7, Me.ColumnHeader8, Me.ColumnHeader9, Me.ColumnHeader10})
        Me.MainMenuContainer.SetColumnSpan(Me.PlayerList, 2)
        Me.PlayerList.ContextMenuStrip = Me.ContextMenuStrip1
        Me.PlayerList.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PlayerList.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.PlayerList.ForeColor = System.Drawing.Color.Black
        Me.PlayerList.FullRowSelect = True
        Me.PlayerList.GridLines = True
        ListViewGroup1.Header = "Idle"
        ListViewGroup1.HeaderAlignment = System.Windows.Forms.HorizontalAlignment.Center
        ListViewGroup1.Name = "Idle"
        ListViewGroup2.Header = "DND"
        ListViewGroup2.HeaderAlignment = System.Windows.Forms.HorizontalAlignment.Center
        ListViewGroup2.Name = "DND"
        Me.PlayerList.Groups.AddRange(New System.Windows.Forms.ListViewGroup() {ListViewGroup1, ListViewGroup2})
        Me.PlayerList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
        Me.PlayerList.HideSelection = False
        Me.PlayerList.Location = New System.Drawing.Point(10, 37)
        Me.PlayerList.Margin = New System.Windows.Forms.Padding(0, 2, 2, 0)
        Me.PlayerList.MultiSelect = False
        Me.PlayerList.Name = "PlayerList"
        Me.PlayerList.ShowItemToolTips = True
        Me.PlayerList.Size = New System.Drawing.Size(247, 406)
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
        'MainMenuContainer
        '
        Me.MainMenuContainer.ColumnCount = 6
        Me.MainMenuContainer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75.0!))
        Me.MainMenuContainer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.84036!))
        Me.MainMenuContainer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18.81414!))
        Me.MainMenuContainer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18.35804!))
        Me.MainMenuContainer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 21.39038!))
        Me.MainMenuContainer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 21.39038!))
        Me.MainMenuContainer.Controls.Add(Me.btnOffline, 5, 2)
        Me.MainMenuContainer.Controls.Add(Me.btnSearch, 2, 2)
        Me.MainMenuContainer.Controls.Add(Me.BtnJoin, 3, 2)
        Me.MainMenuContainer.Controls.Add(Me.cbStatus, 0, 2)
        Me.MainMenuContainer.Controls.Add(Me.Label2, 2, 0)
        Me.MainMenuContainer.Controls.Add(Me.Label1, 0, 0)
        Me.MainMenuContainer.Controls.Add(Me.Matchlist, 2, 1)
        Me.MainMenuContainer.Controls.Add(Me.PlayerList, 0, 1)
        Me.MainMenuContainer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MainMenuContainer.Location = New System.Drawing.Point(0, 29)
        Me.MainMenuContainer.Margin = New System.Windows.Forms.Padding(0)
        Me.MainMenuContainer.Name = "MainMenuContainer"
        Me.MainMenuContainer.Padding = New System.Windows.Forms.Padding(10, 0, 10, 10)
        Me.MainMenuContainer.RowCount = 3
        Me.MainMenuContainer.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35.0!))
        Me.MainMenuContainer.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.MainMenuContainer.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36.0!))
        Me.MainMenuContainer.Size = New System.Drawing.Size(972, 489)
        Me.MainMenuContainer.TabIndex = 21
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.FromArgb(CType(CType(250, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.MainMenuContainer.SetColumnSpan(Me.Label2, 4)
        Me.Label2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.Black
        Me.Label2.Location = New System.Drawing.Point(259, 0)
        Me.Label2.Margin = New System.Windows.Forms.Padding(0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(703, 35)
        Me.Label2.TabIndex = 0
        Me.Label2.Text = "Current Network Games"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.FromArgb(CType(CType(250, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.MainMenuContainer.SetColumnSpan(Me.Label1, 2)
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(10, 0)
        Me.Label1.Margin = New System.Windows.Forms.Padding(0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(249, 35)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Network Players"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lbVer
        '
        Me.lbVer.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lbVer.AutoSize = True
        Me.lbVer.BackColor = System.Drawing.Color.LightGray
        Me.lbVer.Location = New System.Drawing.Point(936, 3)
        Me.lbVer.Name = "lbVer"
        Me.lbVer.Size = New System.Drawing.Size(34, 13)
        Me.lbVer.TabIndex = 10
        Me.lbVer.Text = "0.00a"
        Me.lbVer.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        '_MainMenuStrip
        '
        Me._MainMenuStrip.BackColor = System.Drawing.Color.Transparent
        Me._MainMenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripLogo, Me.ReplaysToolStripMenuItem, Me.ControlsToolStripMenuItem1, Me.OptionsToolStripMenuItem, Me.FreeDLCToolStripMenuItem, Me.DiscordToolStripMenuItem, Me.PatreonO3oToolStripMenuItem, Me.ExtrasToolStripMenuItem, Me.ForceOpenPanelToolStripMenuItem})
        Me._MainMenuStrip.Location = New System.Drawing.Point(0, 0)
        Me._MainMenuStrip.Name = "_MainMenuStrip"
        Me._MainMenuStrip.Padding = New System.Windows.Forms.Padding(0)
        Me._MainMenuStrip.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me._MainMenuStrip.Size = New System.Drawing.Size(972, 29)
        Me._MainMenuStrip.TabIndex = 22
        Me._MainMenuStrip.Text = "MainMenuStrip"
        '
        'ToolStripLogo
        '
        Me.ToolStripLogo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripLogo.Image = CType(resources.GetObject("ToolStripLogo.Image"), System.Drawing.Image)
        Me.ToolStripLogo.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.ToolStripLogo.ImageTransparentColor = System.Drawing.Color.Transparent
        Me.ToolStripLogo.Name = "ToolStripLogo"
        Me.ToolStripLogo.Padding = New System.Windows.Forms.Padding(0)
        Me.ToolStripLogo.Size = New System.Drawing.Size(138, 29)
        '
        'ReplaysToolStripMenuItem
        '
        Me.ReplaysToolStripMenuItem.BackColor = System.Drawing.Color.Transparent
        Me.ReplaysToolStripMenuItem.Name = "ReplaysToolStripMenuItem"
        Me.ReplaysToolStripMenuItem.Size = New System.Drawing.Size(98, 29)
        Me.ReplaysToolStripMenuItem.Text = "Naomi Replays"
        '
        'ControlsToolStripMenuItem1
        '
        Me.ControlsToolStripMenuItem1.BackColor = System.Drawing.Color.Transparent
        Me.ControlsToolStripMenuItem1.Name = "ControlsToolStripMenuItem1"
        Me.ControlsToolStripMenuItem1.Size = New System.Drawing.Size(64, 29)
        Me.ControlsToolStripMenuItem1.Text = "Controls"
        '
        'OptionsToolStripMenuItem
        '
        Me.OptionsToolStripMenuItem.BackColor = System.Drawing.Color.Transparent
        Me.OptionsToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.OptionsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.GeneralToolStripMenuItem, Me.MednafenToolStripMenuItem, Me.GaggedUsersToolStripMenuItem})
        Me.OptionsToolStripMenuItem.Name = "OptionsToolStripMenuItem"
        Me.OptionsToolStripMenuItem.ShowShortcutKeys = False
        Me.OptionsToolStripMenuItem.Size = New System.Drawing.Size(61, 29)
        Me.OptionsToolStripMenuItem.Text = "Options"
        '
        'GeneralToolStripMenuItem
        '
        Me.GeneralToolStripMenuItem.Name = "GeneralToolStripMenuItem"
        Me.GeneralToolStripMenuItem.Size = New System.Drawing.Size(146, 22)
        Me.GeneralToolStripMenuItem.Text = "General"
        '
        'MednafenToolStripMenuItem
        '
        Me.MednafenToolStripMenuItem.Name = "MednafenToolStripMenuItem"
        Me.MednafenToolStripMenuItem.Size = New System.Drawing.Size(146, 22)
        Me.MednafenToolStripMenuItem.Text = "Emulators"
        '
        'GaggedUsersToolStripMenuItem
        '
        Me.GaggedUsersToolStripMenuItem.Name = "GaggedUsersToolStripMenuItem"
        Me.GaggedUsersToolStripMenuItem.Size = New System.Drawing.Size(146, 22)
        Me.GaggedUsersToolStripMenuItem.Text = "Gagged Users"
        '
        'FreeDLCToolStripMenuItem
        '
        Me.FreeDLCToolStripMenuItem.Name = "FreeDLCToolStripMenuItem"
        Me.FreeDLCToolStripMenuItem.Size = New System.Drawing.Size(66, 29)
        Me.FreeDLCToolStripMenuItem.Text = "Free DLC"
        '
        'DiscordToolStripMenuItem
        '
        Me.DiscordToolStripMenuItem.BackColor = System.Drawing.Color.Transparent
        Me.DiscordToolStripMenuItem.Name = "DiscordToolStripMenuItem"
        Me.DiscordToolStripMenuItem.Size = New System.Drawing.Size(59, 29)
        Me.DiscordToolStripMenuItem.Text = "Discord"
        '
        'PatreonO3oToolStripMenuItem
        '
        Me.PatreonO3oToolStripMenuItem.Name = "PatreonO3oToolStripMenuItem"
        Me.PatreonO3oToolStripMenuItem.Size = New System.Drawing.Size(83, 29)
        Me.PatreonO3oToolStripMenuItem.Text = "Patreon o3o"
        '
        'ExtrasToolStripMenuItem
        '
        Me.ExtrasToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MVC2TierListToolStripMenuItem, Me.OpenFlycastDataFolderToolStripMenuItem})
        Me.ExtrasToolStripMenuItem.Name = "ExtrasToolStripMenuItem"
        Me.ExtrasToolStripMenuItem.Size = New System.Drawing.Size(50, 29)
        Me.ExtrasToolStripMenuItem.Text = "Extras"
        '
        'MVC2TierListToolStripMenuItem
        '
        Me.MVC2TierListToolStripMenuItem.Name = "MVC2TierListToolStripMenuItem"
        Me.MVC2TierListToolStripMenuItem.Size = New System.Drawing.Size(205, 22)
        Me.MVC2TierListToolStripMenuItem.Text = "MVC2 Ratio List"
        '
        'OpenFlycastDataFolderToolStripMenuItem
        '
        Me.OpenFlycastDataFolderToolStripMenuItem.Name = "OpenFlycastDataFolderToolStripMenuItem"
        Me.OpenFlycastDataFolderToolStripMenuItem.Size = New System.Drawing.Size(205, 22)
        Me.OpenFlycastDataFolderToolStripMenuItem.Text = "Open Flycast Data Folder"
        '
        'ForceOpenPanelToolStripMenuItem
        '
        Me.ForceOpenPanelToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.HostToolStripMenuItem, Me.WaitingToolStripMenuItem})
        Me.ForceOpenPanelToolStripMenuItem.Name = "ForceOpenPanelToolStripMenuItem"
        Me.ForceOpenPanelToolStripMenuItem.Size = New System.Drawing.Size(80, 29)
        Me.ForceOpenPanelToolStripMenuItem.Text = "Open Panel"
        '
        'HostToolStripMenuItem
        '
        Me.HostToolStripMenuItem.Name = "HostToolStripMenuItem"
        Me.HostToolStripMenuItem.Size = New System.Drawing.Size(115, 22)
        Me.HostToolStripMenuItem.Text = "Host"
        '
        'WaitingToolStripMenuItem
        '
        Me.WaitingToolStripMenuItem.Name = "WaitingToolStripMenuItem"
        Me.WaitingToolStripMenuItem.Size = New System.Drawing.Size(115, 22)
        Me.WaitingToolStripMenuItem.Text = "Waiting"
        '
        'imgBeta
        '
        Me.imgBeta.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.imgBeta.BackColor = System.Drawing.Color.Transparent
        Me.imgBeta.BackgroundImage = Global.NullDC_CvS2_BEAR.My.Resources.Resources.Beta
        Me.imgBeta.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.imgBeta.Location = New System.Drawing.Point(878, 1)
        Me.imgBeta.Margin = New System.Windows.Forms.Padding(0)
        Me.imgBeta.Name = "imgBeta"
        Me.imgBeta.Size = New System.Drawing.Size(48, 18)
        Me.imgBeta.TabIndex = 11
        Me.imgBeta.TabStop = False
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(250, Byte), Integer), CType(CType(200, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(972, 518)
        Me.Controls.Add(Me.imgBeta)
        Me.Controls.Add(Me.MainMenuContainer)
        Me.Controls.Add(Me.lbVer)
        Me.Controls.Add(Me._MainMenuStrip)
        Me.KeyPreview = True
        Me.Name = "frmMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "NullDC BEAR"
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.MainMenuContainer.ResumeLayout(False)
        Me.MainMenuContainer.PerformLayout()
        Me._MainMenuStrip.ResumeLayout(False)
        Me._MainMenuStrip.PerformLayout()
        CType(Me.imgBeta, System.ComponentModel.ISupportInitialize).EndInit()
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
    Friend WithEvents cbStatus As ComboBox
    Friend WithEvents PlayerList As ListView
    Friend WithEvents ColumnHeader6 As ColumnHeader
    Friend WithEvents ColumnHeader7 As ColumnHeader
    Friend WithEvents ColumnHeader8 As ColumnHeader
    Friend WithEvents ColumnHeader9 As ColumnHeader
    Friend WithEvents ColumnHeader10 As ColumnHeader
    Friend WithEvents MainMenuContainer As TableLayoutPanel
    Friend WithEvents lbVer As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents _MainMenuStrip As MenuStrip
    Friend WithEvents OptionsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ReplaysToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ControlsToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents DiscordToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents PatreonO3oToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents imgBeta As PictureBox
    Friend WithEvents FreeDLCToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents Label1 As Label
    Friend WithEvents ToolStripLogo As ToolStripMenuItem
    Friend WithEvents GeneralToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents MednafenToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents ChallengeToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents DMToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents BlockToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents UserNameRadminIPToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents GaggedUsersToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ForceOpenPanelToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents HostToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SystemIcons As ImageList
    Friend WithEvents WaitingToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ExtrasToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents MVC2TierListToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OpenFlycastDataFolderToolStripMenuItem As ToolStripMenuItem
End Class
