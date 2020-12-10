﻿Imports System.IO

Public Class frmMednafenOptions

    Public Sub New()
        Rx.MednafenConfigCache = File.ReadAllLines(MainformRef.NullDCPath & "\mednafen\mednafen.cfg")
        InitializeComponent()

    End Sub

    Private Sub frmMednafenOptions_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.NewNullDCBearIcon
        Me.CenterToParent()

        For i = 0 To tc_options.TabCount - 1
            For Each _child As Control In tc_options.TabPages(i).Controls
                ApplyThemeToControl(_child, 1)
                For Each _child2 As Control In _child.Controls
                    ApplyThemeToControl(_child2, 1)
                    For Each _child3 As Control In _child2.Controls
                        ApplyThemeToControl(_child3, 2)

                    Next : Next : Next : Next

        ApplyThemeToControl(Me, 1)
        ApplyThemeToControl(Panel1, 1)
        ApplyThemeToControl(TableLayoutPanel1, 1)
    End Sub

End Class