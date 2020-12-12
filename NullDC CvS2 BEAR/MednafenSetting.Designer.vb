<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MednafenSetting
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Me.SettingContainer = New System.Windows.Forms.TableLayoutPanel()
        Me.Setting_Label = New System.Windows.Forms.Label()
        Me.SettingContainer.SuspendLayout()
        Me.SuspendLayout()
        '
        'SettingContainer
        '
        Me.SettingContainer.AutoSize = True
        Me.SettingContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.SettingContainer.ColumnCount = 2
        Me.SettingContainer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.SettingContainer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.SettingContainer.Controls.Add(Me.Setting_Label, 0, 1)
        Me.SettingContainer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SettingContainer.Location = New System.Drawing.Point(0, 0)
        Me.SettingContainer.Margin = New System.Windows.Forms.Padding(0)
        Me.SettingContainer.Name = "SettingContainer"
        Me.SettingContainer.RowCount = 3
        Me.SettingContainer.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.SettingContainer.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.SettingContainer.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.SettingContainer.Size = New System.Drawing.Size(106, 16)
        Me.SettingContainer.TabIndex = 0
        '
        'Setting_Label
        '
        Me.Setting_Label.AutoSize = True
        Me.Setting_Label.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Setting_Label.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Setting_Label.Location = New System.Drawing.Point(3, 0)
        Me.Setting_Label.Name = "Setting_Label"
        Me.Setting_Label.Size = New System.Drawing.Size(47, 16)
        Me.Setting_Label.TabIndex = 0
        Me.Setting_Label.Text = "Label"
        '
        'MednafenSetting
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BackColor = System.Drawing.Color.DarkGray
        Me.Controls.Add(Me.SettingContainer)
        Me.Margin = New System.Windows.Forms.Padding(0)
        Me.Name = "MednafenSetting"
        Me.Size = New System.Drawing.Size(106, 16)
        Me.SettingContainer.ResumeLayout(False)
        Me.SettingContainer.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents SettingContainer As TableLayoutPanel
    Friend WithEvents Setting_Label As Label
End Class
