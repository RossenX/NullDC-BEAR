<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSecret
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
        Me.ColorDialog1 = New System.Windows.Forms.ColorDialog()
        Me.btnColor = New System.Windows.Forms.Button()
        Me.btnColor3 = New System.Windows.Forms.Button()
        Me.btnColor2 = New System.Windows.Forms.Button()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.SuspendLayout()
        '
        'btnColor
        '
        Me.btnColor.BackColor = System.Drawing.Color.Red
        Me.btnColor.BackgroundImage = Global.NullDC_CvS2_BEAR.My.Resources.Resources.Rupee
        Me.btnColor.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.btnColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnColor.Location = New System.Drawing.Point(194, 255)
        Me.btnColor.Name = "btnColor"
        Me.btnColor.Size = New System.Drawing.Size(48, 48)
        Me.btnColor.TabIndex = 0
        Me.btnColor.UseVisualStyleBackColor = False
        '
        'btnColor3
        '
        Me.btnColor3.BackColor = System.Drawing.Color.Red
        Me.btnColor3.BackgroundImage = Global.NullDC_CvS2_BEAR.My.Resources.Resources.Rupee
        Me.btnColor3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.btnColor3.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnColor3.Location = New System.Drawing.Point(461, 255)
        Me.btnColor3.Name = "btnColor3"
        Me.btnColor3.Size = New System.Drawing.Size(48, 48)
        Me.btnColor3.TabIndex = 1
        Me.btnColor3.UseVisualStyleBackColor = False
        '
        'btnColor2
        '
        Me.btnColor2.BackColor = System.Drawing.Color.Red
        Me.btnColor2.BackgroundImage = Global.NullDC_CvS2_BEAR.My.Resources.Resources.Rupee
        Me.btnColor2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.btnColor2.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnColor2.Location = New System.Drawing.Point(328, 255)
        Me.btnColor2.Name = "btnColor2"
        Me.btnColor2.Size = New System.Drawing.Size(48, 48)
        Me.btnColor2.TabIndex = 1
        Me.btnColor2.UseVisualStyleBackColor = False
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.BackColor = System.Drawing.Color.Transparent
        Me.TableLayoutPanel1.ColumnCount = 1
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(328, 178)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(48, 49)
        Me.TableLayoutPanel1.TabIndex = 3
        '
        'Button1
        '
        Me.Button1.BackColor = System.Drawing.Color.Transparent
        Me.Button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.Button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Button1.Location = New System.Drawing.Point(311, 396)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(83, 70)
        Me.Button1.TabIndex = 4
        Me.Button1.UseVisualStyleBackColor = False
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.BackColor = System.Drawing.Color.Transparent
        Me.TableLayoutPanel2.ColumnCount = 1
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(199, 180)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 1
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(42, 43)
        Me.TableLayoutPanel2.TabIndex = 4
        '
        'frmSecret
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackgroundImage = Global.NullDC_CvS2_BEAR.My.Resources.Resources.Secret_Moblin
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.ClientSize = New System.Drawing.Size(708, 465)
        Me.Controls.Add(Me.TableLayoutPanel2)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Controls.Add(Me.btnColor2)
        Me.Controls.Add(Me.btnColor3)
        Me.Controls.Add(Me.btnColor)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmSecret"
        Me.Text = "Shh...."
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents ColorDialog1 As ColorDialog
    Friend WithEvents btnColor As Button
    Friend WithEvents btnColor3 As Button
    Friend WithEvents btnColor2 As Button
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents Button1 As Button
    Friend WithEvents TableLayoutPanel2 As TableLayoutPanel
End Class
