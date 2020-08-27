Imports System.IO

Namespace My
    ' The following events are available for MyApplication:
    ' Startup: Raised when the application starts, before the startup form is created.
    ' Shutdown: Raised after all application forms are closed.  This event is not raised if the application terminates abnormally.
    ' UnhandledException: Raised if the application encounters an unhandled exception.
    ' StartupNextInstance: Raised when launching a single-instance application and the application is already active. 
    ' NetworkAvailabilityChanged: Raised when the network connection is connected or disconnected.
    Partial Friend Class MyApplication

        Private Sub MyApplication_UnhandledException(ByVal _
        sender As Object, ByVal e As _
        ApplicationServices.UnhandledExceptionEventArgs) _
        Handles Me.UnhandledException
            e.ExitApplication =
                MessageBox.Show("Yo Something went wrong send this to RossenX" & vbCrLf & e.Exception.Message & vbNewLine & vbNewLine & e.Exception.StackTrace, "Crashy McCrashface")
        End Sub

    End Class

End Namespace
