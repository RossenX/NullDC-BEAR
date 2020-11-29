Public Class FrmPaidDLC
    Dim NumberOfClicks As Int16 = 0

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Select Case NumberOfClicks
            Case 1
                Process.Start("https://www.youtube.com/watch?v=dQw4w9WgXcQ")
            Case 2
                Process.Start("https://www.youtube.com/watch?v=caCGpPFrGBo")
            Case 3
                Process.Start("https://www.youtube.com/watch?v=G9Z-q5atjiA")
            Case 4
                Process.Start("https://www.youtube.com/watch?v=J48orv6InGI")
            Case 5
                Process.Start("https://www.youtube.com/watch?v=czcqx17yQJI")
            Case 6
                Process.Start("https://www.youtube.com/watch?v=OudCsiYXDnU")
            Case Else
                Process.Start("https://www.youtube.com/watch?v=AGrIR_jlLno")
        End Select

        If NumberOfClicks < 7 Then NumberOfClicks += 1

        Button2.Text = (9 + NumberOfClicks).ToString & ".99$ + Free " & ((NumberOfClicks + 1) * 12).ToString & " Months FREE"

    End Sub

    Private Sub FrmPaidDLC_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.BackColor = BEARTheme.LoadColor(ThemeKeys.PrimaryColor)
        ApplyThemeToControl(Label1)
        ApplyThemeToControl(Label2)
        ApplyThemeToControl(Label3)
        ApplyThemeToControl(Label4)

        Me.CenterToParent()

        Dim ButtonFlashingThread As Threading.Thread = New Threading.Thread(Sub()
                                                                                While True
                                                                                    Dim m_Rnd As New Random
                                                                                    Button2.BackColor = Color.FromArgb(255, m_Rnd.Next(0, 255), m_Rnd.Next(0, 255), m_Rnd.Next(0, 255))
                                                                                    Threading.Thread.Sleep(250)
                                                                                End While

                                                                            End Sub)
        ButtonFlashingThread.Start()
        ButtonFlashingThread.IsBackground = True
    End Sub

End Class