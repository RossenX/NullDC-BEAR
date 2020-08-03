Imports System.IO

Module Rx
    Public MainformRef As frmMain
    Public PreferedStatus As String
    ' yes this is all that there is in this module. It's litterally so i can keep a reference to the main form after i minimize to tray, 
    ' because APPERANLY minimizing something To tray And removing it from the taskbar makes it lose it's reference in the openforms param.

    Public Function GetEEPROM(ByVal _romfullpath As String) As Byte()
        Dim EEPROM As Byte() = {}
        Dim EEPROMPath As String = _romfullpath & ".eeprom"
        Dim FileBytes As Byte()

        If File.Exists(EEPROMPath) Then
            FileBytes = File.ReadAllBytes(_romfullpath & ".eeprom")
            Console.WriteLine("Read EEPROM:" & FileBytes.ToString)
        Else
            Return {}
        End If

        Return FileBytes
    End Function

    Public Sub WriteEEPROM(ByVal _romfullpath As String, ByVal _eeprom As Byte())
        Dim EEPROMPath As String = MainformRef.NullDCPath & _romfullpath & ".eeprom"
        Console.WriteLine("Write EEPROM:" & _eeprom.ToString)
        If File.Exists(EEPROMPath) Then
            File.SetAttributes(EEPROMPath, FileAttributes.Normal)
            File.Delete(EEPROMPath)
        End If

        File.WriteAllBytes(EEPROMPath, _eeprom)
    End Sub

End Module
