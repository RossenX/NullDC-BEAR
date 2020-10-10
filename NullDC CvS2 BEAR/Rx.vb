﻿Imports System.IO
Imports System.Net.NetworkInformation
Imports System.Text
Imports System.Threading

Module Rx
    Public MainformRef As frmMain ' Mainly here to have a constatn reference to the main form even after minimzing to tray
    Public EEPROM As String ' the EEPROM we're using saved here for people that wanna join as spectator
    Public VMU As String ' the p1 VMU
    Public VMUPieces As New ArrayList

    Public Function GetEEPROM(ByVal _romfullpath As String) As String
        Dim EEPROMPath As String = _romfullpath & ".eeprom"
        Dim FileBytes As String

        If File.Exists(EEPROMPath) Then
            FileBytes = BitConverter.ToString(File.ReadAllBytes(EEPROMPath)).Replace("-", String.Empty)
            Console.WriteLine("Read EEPROM:" & FileBytes.ToString)
        Else
            Return ""
        End If

        Return FileBytes
    End Function

    ' Write the client EEPROM, only used to sync, has no actual use outside of sync
    Public Sub WriteEEPROM(ByVal EEPROMString As String, ByVal _romfullpath As String)
        Dim EEPROMPath As String = _romfullpath & ".eeprom"
        Dim EEPROMPath_backup As String = EEPROMPath & "_backup"

        If File.Exists(EEPROMPath) Then
            File.SetAttributes(EEPROMPath, FileAttributes.Normal)
            If File.Exists(EEPROMPath_backup) Then
                File.SetAttributes(EEPROMPath, FileAttributes.Normal)
                File.Delete(EEPROMPath)
            Else
                My.Computer.FileSystem.RenameFile(EEPROMPath, Path.GetFileName(EEPROMPath_backup))
            End If
        End If

        If Not EEPROMString = "" Then
            Dim nBytes = EEPROMString.Length \ 2
            Dim EEPROMasByte(nBytes - 1) As Byte
            For i = 0 To nBytes - 1
                EEPROMasByte(i) = Convert.ToByte(EEPROMString.Substring(i * 2, 2), 16)
            Next
            File.WriteAllBytes(EEPROMPath, EEPROMasByte)
        Else
            If File.Exists(EEPROMPath) Then
                File.SetAttributes(EEPROMPath, FileAttributes.Normal)
                File.Delete(EEPROMPath)
            End If
        End If

    End Sub

    Public Sub RestoreEEPROM(ByVal _romfullpath As String)
        Dim EEPROMPath As String = _romfullpath & ".eeprom"
        Dim EEPROMPath_backup As String = EEPROMPath & "_backup"

        If File.Exists(EEPROMPath_backup) Then
            File.SetAttributes(EEPROMPath_backup, FileAttributes.Normal)
            If File.Exists(EEPROMPath) Then
                File.SetAttributes(EEPROMPath, FileAttributes.Normal)
                File.Delete(EEPROMPath)
            End If
            My.Computer.FileSystem.RenameFile(EEPROMPath_backup, Path.GetFileName(EEPROMPath))
        End If
    End Sub

    ' No real use for these yet just trying shit out
    Public Function ReadVMU() As String

        Dim VMUFILE = MainformRef.NullDCPath & "\dc\vmu_data_host.bin"
        If Not File.Exists(VMUFILE) Then Return ""

        Dim bytes = File.ReadAllBytes(VMUFILE)

        If File.Exists(VMUFILE) Then
            Return Convert.ToBase64String(bytes)
        Else
            Return ""
        End If

    End Function

    Public Sub TestSend()
        Dim VMUPieceLength = Math.Floor(VMU.Length / 10)
        Console.WriteLine(VMUPieceLength)
        Console.WriteLine(VMU.Length)
        Dim VMUPiecesTosend As New ArrayList

        Dim CompleteSent As String = ""

        For i = 0 To 9
            If i = 9 Then
                ' This is we have odd number of splits, the last one will get any left over bytes
                VMUPiecesTosend.Add(VMU.Substring(i * VMUPieceLength, VMU.Length - (i * VMUPieceLength)))
            Else
                VMUPiecesTosend.Add(VMU.Substring(i * VMUPieceLength, VMUPieceLength))
            End If
        Next

        For i = 0 To 9
            CompleteSent += VMUPiecesTosend(i)
            Thread.Sleep(50)
        Next

        File.WriteAllBytes(MainformRef.NullDCPath + "\dc\vmu_data_client.bin", Convert.FromBase64String(CompleteSent))

    End Sub

    Public Sub SendVMU(ByVal _ip As String)

        Dim VMUSendingThread As Thread = New Thread(
            Sub()
                Dim VMUPieceLength = Math.Floor(VMU.Length / 10)
                Dim VMUPiecesTosend As New ArrayList

                For i = 0 To 9
                    If i = 9 Then
                        ' This is we have odd number of splits, the last one will get any left over bytes
                        VMUPiecesTosend.Add(VMU.Substring(i * VMUPieceLength, VMU.Length - (i * VMUPieceLength)))
                    Else
                        VMUPiecesTosend.Add(VMU.Substring(i * VMUPieceLength, VMUPieceLength))
                    End If
                Next

                For i = 0 To 9
                    MainformRef.NetworkHandler.SendMessage("#,9," & i & "," & VMUPiecesTosend(i), _ip)
                    Thread.Sleep(50)
                Next

                Console.WriteLine("Done Sending VMU to " & _ip)

            End Sub)

        VMUSendingThread.IsBackground = True
        VMUSendingThread.Start()

    End Sub

    Public Sub RecieveVMUPiece(ByVal _total As Int16, ByVal _piece As Int16, ByVal _data As String)
        'Console.WriteLine("Recivefd Piece: " & _data)

        If VMUPieces.Count < 10 Then
            VMUPieces.Clear()
            For i = 0 To 9
                VMUPieces.Add("")
            Next
        End If

        VMUPieces(_piece) = _data

        Dim RecivedAllVMUPieces = True

        For i = 0 To 9
            If VMUPieces(i) = "" Then
                RecivedAllVMUPieces = False
                Exit For
            End If
        Next

        If RecivedAllVMUPieces Then
            CombineVMUPieces()
        End If

    End Sub

    Public Sub CombineVMUPieces()
        Dim CombinedVMU As String = ""

        For i = 0 To 9
            CombinedVMU += VMUPieces(i)
        Next

        File.WriteAllBytes(MainformRef.NullDCPath + "\dc\vmu_data_client.bin", Convert.FromBase64String(CombinedVMU))
        Rx.VMU = CombinedVMU
        MainformRef.NetworkHandler.SendMessage("G", MainformRef.Challenger.ip)

        MainformRef.Invoke(
            Sub()
                If MainformRef.WaitingForm.Visible Then
                    MainformRef.WaitingForm.Label1.Text = "Waiting for Host"
                End If
            End Sub)

    End Sub

End Module
