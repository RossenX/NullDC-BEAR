Public Class frmMednafenOptions

    Dim MednafenSettings As New MednafenSettings

    Private Sub frmMednafenOptions_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

End Class

Public Class MednafenSettings
    Dim Setting As New Dictionary(Of String, String)

    Public Sub New()
        For Each _line In IO.File.ReadAllLines(MainformRef.NullDCPath & "\mednafen\mednafen.cfg")
            If _line.Trim.Length > 0 And Not _line.StartsWith(";") Then
                If Not _line.StartsWith("apple2") And
                    Not _line.StartsWith("demo.") And
                    Not _line.StartsWith("gg.") And
                    Not _line.StartsWith("lynx.") And
                    Not _line.StartsWith("pce.") And
                    Not _line.StartsWith("pcfx.") And
                    Not _line.StartsWith("player.") And
                    Not _line.StartsWith("sms.") And
                    Not _line.StartsWith("ssfplay.") And
                    Not _line.StartsWith("vb.") And
                    Not _line.StartsWith("pce_fast.") And
                    Not _line.StartsWith("cdplay.") And
                    Not _line.StartsWith("wswan.") Then

                    Dim a = _line.Split(".")
                    If a.Count > 2 Then
                        If Not a(1) = "input" Then
                            Console.WriteLine(_line)
                        End If
                    Else
                        Console.WriteLine(_line)
                    End If



                End If



            End If
        Next

    End Sub

End Class