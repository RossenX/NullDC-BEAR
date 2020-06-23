Imports System.IO

Module Module1

    Sub Main()
        If File.Exists(System.AppDomain.CurrentDomain.BaseDirectory & "\nulldc-1-0-4-en-win\NullDC.BEAR.exe") Then
            Process.Start(System.AppDomain.CurrentDomain.BaseDirectory & "\nulldc-1-0-4-en-win\NullDC.BEAR.exe")
        End If
        End
    End Sub

End Module
