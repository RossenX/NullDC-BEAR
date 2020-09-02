﻿Imports System.IO
Imports System.Net.NetworkInformation
Imports System.Text
Imports System.Threading

Module Rx
    Public MainformRef As frmMain ' Mainly here to have a constatn reference to the main form even after minimzing to tray
    Public EEPROM As String

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

    Public Function ReadVMU(ByVal _player As Int16) As String
        Dim VMUFile As String = MainformRef.NullDCPath & "\dc\vmu_data_port01.bin"
        Dim FileBytes As String

        If _player = 2 Then VMUFile = MainformRef.NullDCPath & "\dc\vmu_data_port02.bin"

        If File.Exists(VMUFile) Then
            FileBytes = Convert.ToBase64String(File.ReadAllBytes(VMUFile))

            ' FileBytes = BitConverter.ToString(File.ReadAllBytes(VMUFile)).Replace("-", String.Empty)
            Console.WriteLine("Read VMU:" & FileBytes.ToString)
        Else
            Return ""
        End If

        ' Convert.ToBase64String(VMUFile)
        ' Convert.FromBase64String(FileBytes)

        Return FileBytes
    End Function

    Public Sub WriteVMU(ByVal _vmustring As String, ByVal _player As Int16)

    End Sub

End Module
