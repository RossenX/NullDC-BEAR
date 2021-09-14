Namespace My
    Partial Friend Class MyApplication

        Private Delegate Sub SafeApplicationThreadException(ByVal sender As Object, ByVal e As Threading.ThreadExceptionEventArgs)

        Private Sub ShowDebugOutput(ByVal ex As Exception)

            Dim st As StackTrace = New StackTrace(ex, True)
            FrmCrash.ShowDialog2("Yo something went wrong! Send this to RossenX: " & ex.Message & vbNewLine & vbNewLine & ex.ToString)
            'MsgBox("Yo something went wrong! Send this to RossenX: " & vbNewLine & ex.Message & vbNewLine & ">" & FileName & "(" & st.GetFrame(0).GetFileLineNumber & ")" & vbNewLine & Stacktrace & vbNewLine & ex.ToString,, "Crashy McCrashface                                                                                                              ")

            Environment.Exit(0)

        End Sub

        Private Sub MyApplication_Startup(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.ApplicationServices.StartupEventArgs) Handles Me.Startup

            AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf AppDomain_UnhandledException
            AddHandler System.Windows.Forms.Application.ThreadException, AddressOf app_ThreadException

        End Sub

        Private Sub app_ThreadException(ByVal sender As Object, ByVal e As Threading.ThreadExceptionEventArgs)

            'This is not thread safe, so make it thread safe.
            If MainForm.InvokeRequired Then
                ' Invoke back to the main thread
                MainForm.Invoke(New SafeApplicationThreadException(AddressOf app_ThreadException), New Object() {sender, e})
            Else
                ShowDebugOutput(e.Exception)
            End If

        End Sub

        Private Sub AppDomain_UnhandledException(ByVal sender As Object, ByVal e As UnhandledExceptionEventArgs)

            ShowDebugOutput(DirectCast(e.ExceptionObject, Exception))

        End Sub

        Private Sub MyApplication_UnhandledException(sender As Object, e As Microsoft.VisualBasic.ApplicationServices.UnhandledExceptionEventArgs) Handles Me.UnhandledException

            ShowDebugOutput(e.Exception)

        End Sub
    End Class


End Namespace

