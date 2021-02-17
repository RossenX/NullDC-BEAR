Imports System.Net

Public Class frmDownloading

    Public Sub AddDownload(ByVal _URL As String, ByVal _filename As String, ByVal _extract As String)
        tlp_downloads.RowStyles.Insert(0, New RowStyle(SizeType.AutoSize, 35.0F))

        Dim tmp As New ccDownload(_URL, WebUtility.UrlDecode(_filename), _extract)
        tmp.Dock = DockStyle.Top
        tlp_downloads.Controls.Add(tmp)

    End Sub

    Private Sub frmDownloading_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub frmDownloading_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        For Each _dl As ccDownload In tlp_downloads.Controls

        Next

    End Sub

End Class