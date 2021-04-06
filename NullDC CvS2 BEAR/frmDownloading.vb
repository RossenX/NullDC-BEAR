Imports System.Net

Public Class frmDownloading

    Public Sub AddDownload(ByVal _URL As String, ByVal _filename As String, ByVal _extract As String)
        _filename = MainformRef.NullDCPath & "\" & _filename
        Dim ExistingDownload As Boolean = False
        For Each _download As ccDownload In tlp_downloads.Controls
            If _download.URL_String = _URL Then
                ExistingDownload = True
                Exit For
            End If
        Next

        If Not ExistingDownload Then
            If tlp_downloads.Controls.Count >= 5 Then
                MainformRef.NotificationForm.ShowMessage("Slow Down there, max of 5 downloads at once.")
                Exit Sub
            End If

            tlp_downloads.RowStyles.Insert(0, New RowStyle(SizeType.AutoSize, 35.0F))

            Dim tmp As New ccDownload(_URL, WebUtility.UrlDecode(_filename), _extract)
            tmp.Dock = DockStyle.Top
            tlp_downloads.Controls.Add(tmp)

        End If

    End Sub

    Private Sub frmDownloading_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.fan_icon_text

    End Sub

    Private Sub frmDownloading_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        For Each _dl As ccDownload In tlp_downloads.Controls

        Next

    End Sub

End Class