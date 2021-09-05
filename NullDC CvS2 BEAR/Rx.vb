Imports System.Drawing.Drawing2D
Imports System.IO
Imports System.Text
Imports System.Threading
Imports SDL2.SDL

Module Rx

    Public MainformRef As frmMain ' Mainly here to have a constatn reference to the main form even after minimzing to tray
    Public EEPROM As String ' the EEPROM we're using saved here for people that wanna join as spectator
    Public VMU As String ' the p1 VMU
    Public VMUPieces As New ArrayList From {"", "", "", "", "", "", "", "", "", ""}
    Public platform As String = ""
    Public MultiTap As Int16 = 0
    Public GaggedUsers As New Dictionary(Of String, String)
    Public MednafenConfigCache As String()
    Public MupenConfigCache As String()
    Public SecretSettings As String = ""
    Public KeyCon As New KeysConverter

    Public Sub SafeDeleteFile(ByVal _file As String)
        If File.Exists(_file) Then
            File.SetAttributes(_file, FileAttributes.Normal)
            File.Delete(_file)
        End If
    End Sub

    Public Function GetControlsFilePath(Optional ByVal _profile As String = Nothing) As String
        Dim KeyProfileFile As String = ""
        Dim ProfileName = ""

        If _profile Is Nothing Then
            ProfileName = MainformRef.ConfigFile.KeyMapProfile
        Else
            ProfileName = _profile
        End If

        If ProfileName = "Default" Then
            KeyProfileFile = MainformRef.NullDCPath & "\Controls.bear"
        Else
            If File.Exists(MainformRef.NullDCPath & "\Controls_" & ProfileName & ".bear") Then
                KeyProfileFile = MainformRef.NullDCPath & "\Controls_" & ProfileName & ".bear"
            Else
                KeyProfileFile = MainformRef.NullDCPath & "\Controls.bear"
            End If

        End If

            Return KeyProfileFile
    End Function

    Public Sub LoadGaggedUsers()
        If File.Exists(MainformRef.NullDCPath & "\gagged.users") Then
            For Each _g As String In File.ReadAllLines(MainformRef.NullDCPath & "\gagged.users")
                If Not _g.Trim.Length > 0 Then Continue For

                Dim _ip As String = _g.Split("|")(0)
                Dim _name As String = _g.Split("|")(1)
                GaggedUsers.Add(_ip, _name)
            Next

        End If

    End Sub

    Public Sub SaveGaggedUsers()
        Dim GaggedUserList As String = ""
        For Each _key In GaggedUsers.Keys
            GaggedUserList += _key & "|" & GaggedUsers(_key) & vbNewLine
        Next

        File.WriteAllText(MainformRef.NullDCPath & "\gagged.users", GaggedUserList)
    End Sub

    Public Sub GagUser(ByVal _ip As String, ByVal _name As String)
        GaggedUsers.Add(_ip, _name)
        MainformRef.RemovePlayerFromList(_ip)
        MainformRef.NetworkHandler.SendMessage("&", _ip)

    End Sub

    Public Sub UnGagUser(ByVal _ip As String)
        GaggedUsers.Remove(_ip)
        MainformRef.ConfigFile.SaveFile()

    End Sub

    Public Function IsUserGagged(ByVal _ip As String) As Boolean

        If GaggedUsers.ContainsKey(_ip) Then
            Return True
        End If

        Return False

    End Function

    Public Function GenerateGameKey() As String
        If MainformRef.ConfigFile.NoKey = -1 Then
            Return "NoKey"
        End If

        Dim r As New Random
        Dim s As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"
        Dim sb As New StringBuilder
        For i As Integer = 1 To 20
            Dim idx As Integer = r.Next(0, s.Length)
            sb.Append(s.Substring(idx, 1))
        Next

        Return sb.ToString()
    End Function


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
        Dim EEPROMPath As String = _romfullpath & ".eeprom_client"

        If Not EEPROMString = "" Then
            Dim nBytes = EEPROMString.Length \ 2
            Dim EEPROMasByte(nBytes - 1) As Byte
            For i = 0 To nBytes - 1
                EEPROMasByte(i) = Convert.ToByte(EEPROMString.Substring(i * 2, 2), 16)
            Next
            File.WriteAllBytes(EEPROMPath, EEPROMasByte)
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
                    Thread.Sleep(100)
                Next

                Console.WriteLine("Done Sending VMU to " & _ip)

            End Sub)

        VMUSendingThread.IsBackground = True
        VMUSendingThread.Start()

    End Sub

    Public Sub RecieveVMUPiece(ByVal _total As Int16, ByVal _piece As Int16, ByVal _data As String)
        'Console.WriteLine("Recivefd Piece: " & _data)
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
                    MainformRef.WaitingForm.lbWaitingForHost.Text = "Waiting for Host"
                    MainformRef.WaitingForm.btnRetryVMU.Visible = False
                    MainformRef.WaitingForm.VMUTimer.Stop()
                End If
            End Sub)

    End Sub

    Public Function RemoveAnnoyingRomNumbersFromString(ByVal RomName As String) As String
        Dim _romname = RomName
        If RomName.Length > 7 Then
            If IsNumeric(_romname.Split(" ")(0)) And _romname.Substring(4, 3) = " - " Then
                _romname = _romname.Substring(7, _romname.Length - 7)
            End If
        End If

        Return _romname
    End Function

    Public Function GetCurrentPeripherals() As String
        Dim cfgEntry = ""
        Dim PeripheralList = ""

        Select Case MainformRef.GamesList(MainformRef.ConfigFile.Game)(2)
            Case "dc", "na"

            Case "sg"
                cfgEntry = "md"
            Case "ss"
                cfgEntry = "ss"
            Case "nes"
                cfgEntry = "nes"
            Case "ngp"
                cfgEntry = "ngp"
            Case "snes"
                cfgEntry = "snes"
            Case "psx"
                cfgEntry = "psx"
            Case "gba"
                cfgEntry = "gba"
            Case "gbc"
                cfgEntry = "gb"
            Case "fds"
                cfgEntry = "nes"
            Case "sms"
                cfgEntry = "sms"
            Case Else
                MsgBox("Missing emulator type: " & MainformRef.GamesList(MainformRef.ConfigFile.Game)(2))
        End Select
        cfgEntry += ".input.port"
        For Each _line As String In File.ReadAllLines(MainformRef.NullDCPath & "\mednafen\mednafen.cfg")
            For i = 1 To 12
                If _line.StartsWith(cfgEntry & i & " ") Or
                    (MainformRef.GamesList(MainformRef.ConfigFile.Game)(2) = "nes" And _line.StartsWith("nes.input.fcexp ")) Then
                    If PeripheralList.Count > 0 Then PeripheralList += "|"
                    PeripheralList += _line.Replace(cfgEntry & i & " ", "").Trim

                End If
            Next

        Next

        Return PeripheralList & "|" & MultiTap.ToString
    End Function

    Public Sub SetCurrentPeripheralsFromString(ByVal _string As String, _game As String)
        Dim _peri = _string.Split("|")

        Dim cfgEntry = ""

        Select Case MainformRef.GamesList(_game)(2)
            Case "dc", "na"
            Case "sg"
                cfgEntry = "md"
            Case "ss"
                cfgEntry = "ss"
            Case "nes"
                cfgEntry = "nes"
            Case "ngp"
                cfgEntry = "ngp"
            Case "snes"
                cfgEntry = "snes"
            Case "psx"
                cfgEntry = "psx"
            Case "gba"
                cfgEntry = "gba"
            Case "gbc"
                cfgEntry = "gb"
            Case "fds"
                cfgEntry = "nes"
            Case "sms"
                cfgEntry = "sms"
            Case Else
                MsgBox("Missing emulator type: " & MainformRef.GamesList(MainformRef.ConfigFile.Game)(2))
        End Select

        cfgEntry += ".input.port"
        Dim EntryCount As Int16 = 1

        File.SetAttributes(MainformRef.NullDCPath & "\mednafen\mednafen.cfg", FileAttributes.Normal)
        Dim _Lines = File.ReadAllLines(MainformRef.NullDCPath & "\mednafen\mednafen.cfg")

        For i = 0 To _Lines.Count - 1
            If _Lines(i).StartsWith(cfgEntry) Then ' This is the start of a port
                For l = 1 To 12 ' check thorugh all 12 ports
                    If _Lines(i).StartsWith(cfgEntry & l & " ") Then
                        _Lines(i) = cfgEntry & l & " " & _peri(EntryCount - 1)
                        EntryCount += 1
                        Exit For

                    ElseIf MainformRef.GamesList(_game)(2) = "nes" And _Lines(i).StartsWith("nes.input.fcexp ") Then
                        _Lines(i) = "nes.input.fcexp " & _peri(EntryCount - 1)
                        EntryCount += 1
                        Exit For

                    End If

                Next
            End If
        Next

        Rx.MultiTap = CInt(_peri(_peri.Count - 1))
        File.WriteAllLines(MainformRef.NullDCPath & "\mednafen\mednafen.cfg", _Lines)

    End Sub

    Public Function FindMessangerWindowFromIP(ByVal _ip As String) As frmDM
        Dim FoundWindow As frmDM = Nothing
        For Each _form In Application.OpenForms()
            If _form.GetType = GetType(frmDM) Then
                If DirectCast(_form, frmDM).UserIP = _ip Then
                    FoundWindow = _form
                    Exit For
                End If
            End If
        Next

        Return FoundWindow

    End Function


    ' List Of KeyCode Names To ScanCode Values
    Public Enum KCtSC
        ' Right here we go then MUST BE LOWER CASE BECAUSE FUCK DEALING WITH CASE SENSATIVE STUFF
        ' CTRL, ALT, SHIFT are not mappable becuase they are used as modifiers
        ' Basic Alphabet
        a = SDL_Scancode.SDL_SCANCODE_A
        b = SDL_Scancode.SDL_SCANCODE_B
        c = SDL_Scancode.SDL_SCANCODE_C
        d = SDL_Scancode.SDL_SCANCODE_D
        e = SDL_Scancode.SDL_SCANCODE_E
        f = SDL_Scancode.SDL_SCANCODE_F
        g = SDL_Scancode.SDL_SCANCODE_G
        h = SDL_Scancode.SDL_SCANCODE_H
        i = SDL_Scancode.SDL_SCANCODE_I
        j = SDL_Scancode.SDL_SCANCODE_J
        k = SDL_Scancode.SDL_SCANCODE_K
        l = SDL_Scancode.SDL_SCANCODE_L
        m = SDL_Scancode.SDL_SCANCODE_M
        n = SDL_Scancode.SDL_SCANCODE_N
        o = SDL_Scancode.SDL_SCANCODE_O
        p = SDL_Scancode.SDL_SCANCODE_P
        q = SDL_Scancode.SDL_SCANCODE_Q
        r = SDL_Scancode.SDL_SCANCODE_R
        s = SDL_Scancode.SDL_SCANCODE_S
        t = SDL_Scancode.SDL_SCANCODE_T
        u = SDL_Scancode.SDL_SCANCODE_U
        v = SDL_Scancode.SDL_SCANCODE_V
        w = SDL_Scancode.SDL_SCANCODE_W
        x = SDL_Scancode.SDL_SCANCODE_X
        y = SDL_Scancode.SDL_SCANCODE_Y
        z = SDL_Scancode.SDL_SCANCODE_Z

        ' Number row from Tilde to Backspace
        oemtilde = SDL_Scancode.SDL_SCANCODE_GRAVE
        number0 = SDL_Scancode.SDL_SCANCODE_0
        number1 = SDL_Scancode.SDL_SCANCODE_1
        number2 = SDL_Scancode.SDL_SCANCODE_2
        number3 = SDL_Scancode.SDL_SCANCODE_3
        number4 = SDL_Scancode.SDL_SCANCODE_4
        number5 = SDL_Scancode.SDL_SCANCODE_5
        number6 = SDL_Scancode.SDL_SCANCODE_6
        number7 = SDL_Scancode.SDL_SCANCODE_7
        number8 = SDL_Scancode.SDL_SCANCODE_8
        number9 = SDL_Scancode.SDL_SCANCODE_9
        oemminus = SDL_Scancode.SDL_SCANCODE_MINUS
        oemplus = SDL_Scancode.SDL_SCANCODE_EQUALS
        back = SDL_Scancode.SDL_SCANCODE_BACKSPACE

        ' misc symbols shit on the right of the letters
        oemopenbrackets = SDL_Scancode.SDL_SCANCODE_LEFTBRACKET
        oem6 = SDL_Scancode.SDL_SCANCODE_RIGHTBRACKET
        oem5 = SDL_Scancode.SDL_SCANCODE_BACKSLASH
        oem1 = SDL_Scancode.SDL_SCANCODE_SEMICOLON
        oem7 = SDL_Scancode.SDL_SCANCODE_APOSTROPHE
        oemcomma = SDL_Scancode.SDL_SCANCODE_COMMA
        oemperiod = SDL_Scancode.SDL_SCANCODE_PERIOD
        oemquestion = SDL_Scancode.SDL_SCANCODE_SLASH
        enter = SDL_Scancode.SDL_SCANCODE_RETURN

        ' numpad
        numpad0 = SDL_Scancode.SDL_SCANCODE_KP_0
        numpad1 = SDL_Scancode.SDL_SCANCODE_KP_1
        numpad2 = SDL_Scancode.SDL_SCANCODE_KP_2
        numpad3 = SDL_Scancode.SDL_SCANCODE_KP_3
        numpad4 = SDL_Scancode.SDL_SCANCODE_KP_4
        numpad5 = SDL_Scancode.SDL_SCANCODE_KP_5
        numpad6 = SDL_Scancode.SDL_SCANCODE_KP_6
        numpad7 = SDL_Scancode.SDL_SCANCODE_KP_7
        numpad8 = SDL_Scancode.SDL_SCANCODE_KP_8
        numpad9 = SDL_Scancode.SDL_SCANCODE_KP_9
        kpdecimal = SDL_Scancode.SDL_SCANCODE_KP_PERIOD
        divide = SDL_Scancode.SDL_SCANCODE_KP_DIVIDE
        multiply = SDL_Scancode.SDL_SCANCODE_KP_MULTIPLY
        subtract = SDL_Scancode.SDL_SCANCODE_KP_MINUS
        add = SDL_Scancode.SDL_SCANCODE_KP_PLUS

        ' Arrow Keys
        up = SDL_Scancode.SDL_SCANCODE_UP
        down = SDL_Scancode.SDL_SCANCODE_DOWN
        left = SDL_Scancode.SDL_SCANCODE_LEFT
        right = SDL_Scancode.SDL_SCANCODE_RIGHT

        ' Home and Whatnot above the Arrow Keys
        pgup = SDL_Scancode.SDL_SCANCODE_PAGEUP
        home = SDL_Scancode.SDL_SCANCODE_HOME
        ins = SDL_Scancode.SDL_SCANCODE_INSERT
        del = SDL_Scancode.SDL_SCANCODE_DELETE
        kend = SDL_Scancode.SDL_SCANCODE_END
        pgdn = SDL_Scancode.SDL_SCANCODE_PAGEDOWN

        ' Function Keys
        f1 = SDL_Scancode.SDL_SCANCODE_F1
        f2 = SDL_Scancode.SDL_SCANCODE_F2
        f3 = SDL_Scancode.SDL_SCANCODE_F3
        f4 = SDL_Scancode.SDL_SCANCODE_F4
        f5 = SDL_Scancode.SDL_SCANCODE_F5
        f6 = SDL_Scancode.SDL_SCANCODE_F6
        f7 = SDL_Scancode.SDL_SCANCODE_F7
        f8 = SDL_Scancode.SDL_SCANCODE_F8
        f9 = SDL_Scancode.SDL_SCANCODE_F9
        f10 = SDL_Scancode.SDL_SCANCODE_F10
        f11 = SDL_Scancode.SDL_SCANCODE_F11
        f12 = SDL_Scancode.SDL_SCANCODE_F12

        ' Up to 24, cuz why not
        f13 = SDL_Scancode.SDL_SCANCODE_F13
        f14 = SDL_Scancode.SDL_SCANCODE_F14
        f15 = SDL_Scancode.SDL_SCANCODE_F15
        f16 = SDL_Scancode.SDL_SCANCODE_F16
        f17 = SDL_Scancode.SDL_SCANCODE_F17
        f18 = SDL_Scancode.SDL_SCANCODE_F18
        f19 = SDL_Scancode.SDL_SCANCODE_F19
        f20 = SDL_Scancode.SDL_SCANCODE_F20
        f21 = SDL_Scancode.SDL_SCANCODE_F21
        f22 = SDL_Scancode.SDL_SCANCODE_F22
        f23 = SDL_Scancode.SDL_SCANCODE_F23
        f24 = SDL_Scancode.SDL_SCANCODE_F24

        space = SDL_Scancode.SDL_SCANCODE_SPACE
    End Enum

    ' List Of KeyCode Names To ScanCode Values
    Public Enum KCtSDLKC
        ' Right here we go then MUST BE LOWER CASE BECAUSE FUCK DEALING WITH CASE SENSATIVE STUFF
        ' CTRL, ALT, SHIFT are not mappable becuase they are used as modifiers
        ' Basic Alphabet
        a = SDL_MupenKeySym.SDLK_a
        b = SDL_MupenKeySym.SDLK_b
        c = SDL_MupenKeySym.SDLK_c
        d = SDL_MupenKeySym.SDLK_d
        e = SDL_MupenKeySym.SDLK_e
        f = SDL_MupenKeySym.SDLK_f
        g = SDL_MupenKeySym.SDLK_g
        h = SDL_MupenKeySym.SDLK_h
        i = SDL_MupenKeySym.SDLK_i
        j = SDL_MupenKeySym.SDLK_j
        k = SDL_MupenKeySym.SDLK_k
        l = SDL_MupenKeySym.SDLK_l
        m = SDL_MupenKeySym.SDLK_m
        n = SDL_MupenKeySym.SDLK_n
        o = SDL_MupenKeySym.SDLK_o
        p = SDL_MupenKeySym.SDLK_p
        q = SDL_MupenKeySym.SDLK_q
        r = SDL_MupenKeySym.SDLK_r
        s = SDL_MupenKeySym.SDLK_s
        t = SDL_MupenKeySym.SDLK_t
        u = SDL_MupenKeySym.SDLK_u
        v = SDL_MupenKeySym.SDLK_v
        w = SDL_MupenKeySym.SDLK_w
        x = SDL_MupenKeySym.SDLK_x
        y = SDL_MupenKeySym.SDLK_y
        z = SDL_MupenKeySym.SDLK_z

        ' Number row from Tilde to Backspace
        oemtilde = SDL_MupenKeySym.SDLK_BACKQUOTE
        number0 = SDL_MupenKeySym.SDLK_0
        number1 = SDL_MupenKeySym.SDLK_1
        number2 = SDL_MupenKeySym.SDLK_2
        number3 = SDL_MupenKeySym.SDLK_3
        number4 = SDL_MupenKeySym.SDLK_4
        number5 = SDL_MupenKeySym.SDLK_5
        number6 = SDL_MupenKeySym.SDLK_6
        number7 = SDL_MupenKeySym.SDLK_7
        number8 = SDL_MupenKeySym.SDLK_8
        number9 = SDL_MupenKeySym.SDLK_9
        oemminus = SDL_MupenKeySym.SDLK_MINUS
        oemplus = SDL_MupenKeySym.SDLK_EQUALS
        back = SDL_MupenKeySym.SDLK_BACKSPACE

        ' misc symbols shit on the right of the letters
        oemopenbrackets = SDL_MupenKeySym.SDLK_LEFTBRACKET
        oem6 = SDL_MupenKeySym.SDLK_RIGHTBRACKET
        oem5 = SDL_MupenKeySym.SDLK_BACKSLASH
        oem1 = SDL_MupenKeySym.SDLK_SEMICOLON
        oem7 = SDL_MupenKeySym.SDLK_QUOTE
        oemcomma = SDL_MupenKeySym.SDLK_COMMA
        oemperiod = SDL_MupenKeySym.SDLK_PERIOD
        oemquestion = SDL_MupenKeySym.SDLK_SLASH
        enter = SDL_MupenKeySym.SDLK_RETURN

        ' numpad
        numpad0 = SDL_MupenKeySym.SDLK_KP0
        numpad1 = SDL_MupenKeySym.SDLK_KP1
        numpad2 = SDL_MupenKeySym.SDLK_KP2
        numpad3 = SDL_MupenKeySym.SDLK_KP3
        numpad4 = SDL_MupenKeySym.SDLK_KP4
        numpad5 = SDL_MupenKeySym.SDLK_KP5
        numpad6 = SDL_MupenKeySym.SDLK_KP6
        numpad7 = SDL_MupenKeySym.SDLK_KP7
        numpad8 = SDL_MupenKeySym.SDLK_KP8
        numpad9 = SDL_MupenKeySym.SDLK_KP9
        kpdecimal = SDL_MupenKeySym.SDLK_KP_PERIOD
        divide = SDL_MupenKeySym.SDLK_KP_DIVIDE
        multiply = SDL_MupenKeySym.SDLK_KP_MULTIPLY
        subtract = SDL_MupenKeySym.SDLK_KP_MINUS
        add = SDL_MupenKeySym.SDLK_KP_PLUS

        ' Arrow Keys
        up = SDL_MupenKeySym.SDLK_UP
        down = SDL_MupenKeySym.SDLK_DOWN
        left = SDL_MupenKeySym.SDLK_LEFT
        right = SDL_MupenKeySym.SDLK_RIGHT

        ' Home and Whatnot above the Arrow Keys
        pgup = SDL_MupenKeySym.SDLK_PAGEUP
        home = SDL_MupenKeySym.SDLK_HOME
        ins = SDL_MupenKeySym.SDLK_INSERT
        del = SDL_MupenKeySym.SDLK_DELETE
        kend = SDL_MupenKeySym.SDLK_END
        pgdn = SDL_MupenKeySym.SDLK_PAGEDOWN

        ' Function Keys
        f1 = SDL_MupenKeySym.SDLK_F1
        f2 = SDL_MupenKeySym.SDLK_F2
        f3 = SDL_MupenKeySym.SDLK_F3
        f4 = SDL_MupenKeySym.SDLK_F4
        f5 = SDL_MupenKeySym.SDLK_F5
        f6 = SDL_MupenKeySym.SDLK_F6
        f7 = SDL_MupenKeySym.SDLK_F7
        f8 = SDL_MupenKeySym.SDLK_F8
        f9 = SDL_MupenKeySym.SDLK_F9
        f10 = SDL_MupenKeySym.SDLK_F10
        f11 = SDL_MupenKeySym.SDLK_F11
        f12 = SDL_MupenKeySym.SDLK_F12

        ' Up to 24, cuz why not
        f13 = SDL_MupenKeySym.SDLK_F13
        f14 = SDL_MupenKeySym.SDLK_F14
        f15 = SDL_MupenKeySym.SDLK_F15

        space = SDL_MupenKeySym.SDLK_SPACE

    End Enum

    'Muper Key Symbols, had to use my own because the one in the current SDL did not seem to all be exactly what mupen expected
    Public Enum SDL_MupenKeySym
        SDLK_UNKNOWN = 0
        SDLK_FIRST = 0
        SDLK_BACKSPACE = 8
        SDLK_TAB = 9
        SDLK_CLEAR = 12
        SDLK_RETURN = 13
        SDLK_PAUSE = 19
        SDLK_ESCAPE = 27
        SDLK_SPACE = 32
        SDLK_EXCLAIM = 33
        SDLK_QUOTEDBL = 34
        SDLK_HASH = 35
        SDLK_DOLLAR = 36
        SDLK_AMPERSAND = 38
        SDLK_QUOTE = 39
        SDLK_LEFTPAREN = 40
        SDLK_RIGHTPAREN = 41
        SDLK_ASTERISK = 42
        SDLK_PLUS = 43
        SDLK_COMMA = 44
        SDLK_MINUS = 45
        SDLK_PERIOD = 46
        SDLK_SLASH = 47
        SDLK_0 = 48
        SDLK_1 = 49
        SDLK_2 = 50
        SDLK_3 = 51
        SDLK_4 = 52
        SDLK_5 = 53
        SDLK_6 = 54
        SDLK_7 = 55
        SDLK_8 = 56
        SDLK_9 = 57
        SDLK_COLON = 58
        SDLK_SEMICOLON = 59
        SDLK_LESS = 60
        SDLK_EQUALS = 61
        SDLK_GREATER = 62
        SDLK_QUESTION = 63
        SDLK_AT = 64

        SDLK_LEFTBRACKET = 91
        SDLK_BACKSLASH = 92
        SDLK_RIGHTBRACKET = 93
        SDLK_CARET = 94
        SDLK_UNDERSCORE = 95
        SDLK_BACKQUOTE = 96
        SDLK_a = 97
        SDLK_b = 98
        SDLK_c = 99
        SDLK_d = 100
        SDLK_e = 101
        SDLK_f = 102
        SDLK_g = 103
        SDLK_h = 104
        SDLK_i = 105
        SDLK_j = 106
        SDLK_k = 107
        SDLK_l = 108
        SDLK_m = 109
        SDLK_n = 110
        SDLK_o = 111
        SDLK_p = 112
        SDLK_q = 113
        SDLK_r = 114
        SDLK_s = 115
        SDLK_t = 116
        SDLK_u = 117
        SDLK_v = 118
        SDLK_w = 119
        SDLK_x = 120
        SDLK_y = 121
        SDLK_z = 122
        SDLK_DELETE = 127

        SDLK_WORLD_0 = 160
        SDLK_WORLD_1 = 161
        SDLK_WORLD_2 = 162
        SDLK_WORLD_3 = 163
        SDLK_WORLD_4 = 164
        SDLK_WORLD_5 = 165
        SDLK_WORLD_6 = 166
        SDLK_WORLD_7 = 167
        SDLK_WORLD_8 = 168
        SDLK_WORLD_9 = 169
        SDLK_WORLD_10 = 170
        SDLK_WORLD_11 = 171
        SDLK_WORLD_12 = 172
        SDLK_WORLD_13 = 173
        SDLK_WORLD_14 = 174
        SDLK_WORLD_15 = 175
        SDLK_WORLD_16 = 176
        SDLK_WORLD_17 = 177
        SDLK_WORLD_18 = 178
        SDLK_WORLD_19 = 179
        SDLK_WORLD_20 = 180
        SDLK_WORLD_21 = 181
        SDLK_WORLD_22 = 182
        SDLK_WORLD_23 = 183
        SDLK_WORLD_24 = 184
        SDLK_WORLD_25 = 185
        SDLK_WORLD_26 = 186
        SDLK_WORLD_27 = 187
        SDLK_WORLD_28 = 188
        SDLK_WORLD_29 = 189
        SDLK_WORLD_30 = 190
        SDLK_WORLD_31 = 191
        SDLK_WORLD_32 = 192
        SDLK_WORLD_33 = 193
        SDLK_WORLD_34 = 194
        SDLK_WORLD_35 = 195
        SDLK_WORLD_36 = 196
        SDLK_WORLD_37 = 197
        SDLK_WORLD_38 = 198
        SDLK_WORLD_39 = 199
        SDLK_WORLD_40 = 200
        SDLK_WORLD_41 = 201
        SDLK_WORLD_42 = 202
        SDLK_WORLD_43 = 203
        SDLK_WORLD_44 = 204
        SDLK_WORLD_45 = 205
        SDLK_WORLD_46 = 206
        SDLK_WORLD_47 = 207
        SDLK_WORLD_48 = 208
        SDLK_WORLD_49 = 209
        SDLK_WORLD_50 = 210
        SDLK_WORLD_51 = 211
        SDLK_WORLD_52 = 212
        SDLK_WORLD_53 = 213
        SDLK_WORLD_54 = 214
        SDLK_WORLD_55 = 215
        SDLK_WORLD_56 = 216
        SDLK_WORLD_57 = 217
        SDLK_WORLD_58 = 218
        SDLK_WORLD_59 = 219
        SDLK_WORLD_60 = 220
        SDLK_WORLD_61 = 221
        SDLK_WORLD_62 = 222
        SDLK_WORLD_63 = 223
        SDLK_WORLD_64 = 224
        SDLK_WORLD_65 = 225
        SDLK_WORLD_66 = 226
        SDLK_WORLD_67 = 227
        SDLK_WORLD_68 = 228
        SDLK_WORLD_69 = 229
        SDLK_WORLD_70 = 230
        SDLK_WORLD_71 = 231
        SDLK_WORLD_72 = 232
        SDLK_WORLD_73 = 233
        SDLK_WORLD_74 = 234
        SDLK_WORLD_75 = 235
        SDLK_WORLD_76 = 236
        SDLK_WORLD_77 = 237
        SDLK_WORLD_78 = 238
        SDLK_WORLD_79 = 239
        SDLK_WORLD_80 = 240
        SDLK_WORLD_81 = 241
        SDLK_WORLD_82 = 242
        SDLK_WORLD_83 = 243
        SDLK_WORLD_84 = 244
        SDLK_WORLD_85 = 245
        SDLK_WORLD_86 = 246
        SDLK_WORLD_87 = 247
        SDLK_WORLD_88 = 248
        SDLK_WORLD_89 = 249
        SDLK_WORLD_90 = 250
        SDLK_WORLD_91 = 251
        SDLK_WORLD_92 = 252
        SDLK_WORLD_93 = 253
        SDLK_WORLD_94 = 254
        SDLK_WORLD_95 = 255

        SDLK_KP0 = 256
        SDLK_KP1 = 257
        SDLK_KP2 = 258
        SDLK_KP3 = 259
        SDLK_KP4 = 260
        SDLK_KP5 = 261
        SDLK_KP6 = 262
        SDLK_KP7 = 263
        SDLK_KP8 = 264
        SDLK_KP9 = 265
        SDLK_KP_PERIOD = 266
        SDLK_KP_DIVIDE = 267
        SDLK_KP_MULTIPLY = 268
        SDLK_KP_MINUS = 269
        SDLK_KP_PLUS = 270
        SDLK_KP_ENTER = 271
        SDLK_KP_EQUALS = 272

        SDLK_UP = 273
        SDLK_DOWN = 274
        SDLK_RIGHT = 275
        SDLK_LEFT = 276
        SDLK_INSERT = 277
        SDLK_HOME = 278
        SDLK_END = 279
        SDLK_PAGEUP = 280
        SDLK_PAGEDOWN = 281

        SDLK_F1 = 282
        SDLK_F2 = 283
        SDLK_F3 = 284
        SDLK_F4 = 285
        SDLK_F5 = 286
        SDLK_F6 = 287
        SDLK_F7 = 288
        SDLK_F8 = 289
        SDLK_F9 = 290
        SDLK_F10 = 291
        SDLK_F11 = 292
        SDLK_F12 = 293
        SDLK_F13 = 294
        SDLK_F14		= 295
        SDLK_F15 = 296
        SDLK_NUMLOCK		= 300
        SDLK_CAPSLOCK = 301
        SDLK_SCROLLOCK = 302
        SDLK_RSHIFT = 303
        SDLK_LSHIFT = 304
        SDLK_RCTRL = 305
        SDLK_LCTRL = 306
        SDLK_RALT = 307
        SDLK_LALT = 308
        SDLK_RMETA = 309
        SDLK_LMETA = 310
        SDLK_LSUPER = 311
        SDLK_RSUPER = 312
        SDLK_MODE = 313
        SDLK_COMPOSE = 314
        SDLK_HELP = 315
        SDLK_PRINT = 316
        SDLK_SYSREQ = 317
        SDLK_BREAK = 318
        SDLK_MENU = 319
        SDLK_POWER = 320
        SDLK_EURO = 321
        SDLK_UNDO = 322

    End Enum

    Public Function KeyCodeToSDLScanCode(ByVal _keycode As Keys) As String

        Dim a As String = KeyCon.ConvertToString(_keycode)
        If IsNumeric(a) Then a = "Number" & a
        If a = "Decimal" Then a = "kpDecimal"
        If a = "End" Then a = "kEnd"
        If a = "None" Then Return "0"

        If [Enum].IsDefined(GetType(KCtSC), a.ToLower) Then Return DirectCast([Enum].Parse(GetType(KCtSC), a.ToLower, True), KCtSC)

        Console.WriteLine("Undefined KeyName: " & a)

        Return "0"

    End Function

    Public Function KeyCodeToSDLKeyCode(ByVal _keycode As Keys) As String

        Dim a As String = KeyCon.ConvertToString(_keycode)
        If IsNumeric(a) Then a = "Number" & a
        If a = "Decimal" Then a = "kpDecimal"
        If a = "End" Then a = "kEnd"
        If a = "None" Then Return "0"

        If [Enum].IsDefined(GetType(KCtSDLKC), a.ToLower) Then Return DirectCast([Enum].Parse(GetType(KCtSDLKC), a.ToLower, True), KCtSDLKC)

        Console.WriteLine("Undefined KeyName: " & a)

        Return "0"

    End Function

    Dim ButtonNames As String() = {"b0", "b1", "b2", "b3", "b4",
        "b6", "b7", "b8", ' b5 is Guide, we don't use guide
        "b9", "b10",
        "b11", "b12", "b13", "b14",
        "a0+", "a0-", "a1+", "a1-",
        "a2+", "a2-", "a3+", "a3-",
        "a4+", "a5+"}

    Dim ButtonMappedName As String() = {"a", "b", "x", "y", "back",
        "start", "leftstick", "rightstick",
        "leftshoulder", "rightshoulder",
        "dpup", "dpdown", "dpleft", "dpright",
        "leftx", "leftx", "lefty", "lefty",
        "rightx", "rightx", "righty", "righty",
        "lefttrigger", "righttrigger"}

    ' a0+ = leftx
    ' a1+ = lefty
    ' a2+ = rightx
    ' a3+ = righty

    ' 03000000790000000600000000000000,Generic USB Joystick,a:b2,b:b1,x:b3,y:b0,leftshoulder:b4,rightshoulder:b5,lefttrigger:b6,righttrigger:b7,leftstick:b10,rightstick:b11,dpup:h0.1,dpdown:h0.4,dpleft:h0.8,dpright:h0.2,back:b8,start:b9,leftx:a0~,lefty:a1,rightx:a2~,righty:a4,platform:Windows,

    ' Translate BEAR Button (SDL GameController Mapped Button) to Mednafen (Joystick old style mapping)
    Public Function BEARButtonToMednafenButton(ByVal _configString As String, ByVal _numAxes As String) As Dictionary(Of String, String)
        Dim _TranslatedControls As New Dictionary(Of String, String)

        ' New method
        Dim _buttonIndex = 0
        For Each _buttonToMap In ButtonNames
            Dim MednafenControlLine = ""
            Dim SplitConfigLine = _configString.Split(",")

            For Each _ButtonConfig In SplitConfigLine
                If _ButtonConfig.Split(":")(0) = ButtonMappedName(_buttonIndex) Then
                    Dim _ButtonButton = _ButtonConfig.Split(":")(1)

                    If _ButtonButton.Contains("b") Then
                        MednafenControlLine = _ButtonButton.Replace("b", "button_")

                    ElseIf _ButtonButton.Contains("a") Then

                        Dim Negative As Boolean = _ButtonButton.Contains("-")
                        Dim Reverse As Boolean = _ButtonButton.Contains("~")
                        Dim Axis As String = _ButtonButton.Replace("+", "").Replace("-", "").Replace("a", "").Replace("~", "")

                        MednafenControlLine = "abs_" & Axis

                        If Not ButtonNames(_buttonIndex).Contains("-") Then ' This is an axis that wants to be possitive
                            If Negative Then ' Possitive axis map to negative joy direction
                                MednafenControlLine += "-"
                            Else
                                MednafenControlLine += "+" ' Possive axis to possitive joy direction
                            End If
                        Else ' This is his bother, he's pretty negative
                            If Negative Then ' negative axis map to negative joy direction
                                MednafenControlLine += "+"
                            Else
                                MednafenControlLine += "-" ' negative axis to possitive joy direction
                            End If
                        End If

                        If Reverse Then
                            If MednafenControlLine.EndsWith("+") Then
                                MednafenControlLine += "-"
                            Else
                                MednafenControlLine += "+"
                            End If
                        End If

                    ElseIf _ButtonButton.Contains("h") Then
                        Dim _hatNumber As Int16 = CInt(_ButtonButton.Chars(1).ToString)

                        Select Case _ButtonButton.Split(".")(1)
                            Case "1" ' UP
                                MednafenControlLine = "abs_" & _numAxes + _hatNumber + 1 & "-"
                            Case "4" ' DOWN
                                MednafenControlLine = "abs_" & _numAxes + _hatNumber + 1 & "+"
                            Case "8" ' LEFT
                                MednafenControlLine = "abs_" & _numAxes + _hatNumber & "-"
                            Case "2" ' RIGHT
                                MednafenControlLine = "abs_" & _numAxes + _hatNumber & "+"
                        End Select

                    End If
                End If
            Next

            If Not MednafenControlLine = "" Then
                'Console.WriteLine("Translated: " & ButtonNames(_buttonIndex) & "=" & MednafenControlLine)
                _TranslatedControls.Add(ButtonNames(_buttonIndex), MednafenControlLine)
            End If

            _buttonIndex += 1
        Next

        Return _TranslatedControls
    End Function

    Public Function GetMednafenControllerIDs() As String()
        Try

            Dim MedProc As Process = New Process()
            MedProc.StartInfo.FileName = MainformRef.NullDCPath & "\mednafen\mednafen.exe"
            MedProc.StartInfo.EnvironmentVariables.Add("MEDNAFEN_NOPOPUPS", "1")
            MedProc.StartInfo.CreateNoWindow = True
            MedProc.StartInfo.UseShellExecute = False
            MedProc.StartInfo.Arguments = "Hi"
            MedProc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden

            MedProc.Start()
            MedProc.WaitForExit()

            Dim MednafenControllerID(128) As String

            Dim _xinput As New ArrayList
            Dim _dinput As New ArrayList

            For Each line As String In File.ReadAllLines(MainformRef.NullDCPath & "\mednafen\stdout.txt")
                If line.StartsWith("  ID: ") Then

                    If line.ToLower.Contains("xinput") Then
                        _xinput.Add("xinput_" & line.Trim.Split(" ")(1))
                    Else
                        _dinput.Add(line.Trim.Split(" ")(1))
                    End If

                    Console.WriteLine("Found Medanfen Controller ID: " & line.Trim)
                End If
            Next

            Dim ReOrderedIDS As New ArrayList

            For Each _id In _xinput
                ReOrderedIDS.Add(_id)
            Next

            _dinput.Reverse()

            For Each _id In _dinput
                ReOrderedIDS.Add(_id)
            Next

            Dim ControllerIndex = 0
            For Each _id In ReOrderedIDS
                MednafenControllerID(ControllerIndex) = _id
                ControllerIndex += 1
            Next

            Console.WriteLine("Ran Mednafen once to get the controls output")

            Return MednafenControllerID

        Catch ex As Exception
            MsgBox("Unable to get Mednafen Controller IDs: " & ex.Message)
            Return {"0x0", "0x0"}

        End Try

    End Function

    Public Function GetSDLControllerButtonName(ByVal _Button As String) As String
        Dim SDLButtonName As String = ""

        For i = 0 To ButtonNames.Count - 1
            If ButtonNames(i) = _Button Then
                SDLButtonName = ButtonMappedName(i)
                Exit For
            End If
        Next

        Return SDLButtonName
    End Function

    Public Function BEARButtonToMupenButton(ByVal _mappingString As String, ByVal _Button As String) As String

        ' If this is jut a key just return the keycode
        If _Button.StartsWith("k") Then
            If KeyCodeToSDLKeyCode(_Button.Substring(1)) = "0" Then Return ""

            Dim tmpKey = "key(" & KeyCodeToSDLKeyCode(_Button.Substring(1)) & ")"
            Return "key(" & KeyCodeToSDLKeyCode(_Button.Substring(1)) & ")"

        End If

        Dim SDLMappingString As String() = _mappingString.Split(",")
        Dim SDLControllerButtonName As String = GetSDLControllerButtonName(_Button)
        ' Button wasn't found might as well just stop here
        If SDLControllerButtonName = "" Then Return ""

        Dim MupenButton = ""
        For i = 2 To SDLMappingString.Count
            If SDLMappingString(i).Split(":")(0) = SDLControllerButtonName Then

                Dim _key = SDLMappingString(i).Split(":")(1)

                If _key.StartsWith("b") Then
                    MupenButton = _key.Replace("b", "button(") & ")"

                ElseIf _key.StartsWith("h") Then
                    Dim HatNumber = _key.Replace("h", "").Split(".")(0)
                    Dim HatDirection As String = ""

                    Select Case _key.Replace("h", "").Split(".")(1)
                        Case "1" ' Up
                            HatDirection = "Up"
                        Case "2" ' Right
                            HatDirection = "Right"
                        Case "4" ' Down
                            HatDirection = "Down"
                        Case "8" ' Left
                            HatDirection = "Left"
                        Case Else
                            MupenButton = ""
                    End Select

                    MupenButton = "hat(" & HatNumber & " " & HatDirection & ")"

                ElseIf _key.StartsWith("a") Then
                    Dim PlusOrMinus = "-"
                    If _Button.Contains("+") Then
                        PlusOrMinus = "+"
                    End If
                    MupenButton = _key.Replace("a", "axis(") & PlusOrMinus & ")"

                End If

                Exit For
            End If
        Next

        Return MupenButton
    End Function

    Public Function GetFullMappingStringforIndex(ByVal _index) As String

        Dim Fullstring = ""

        Dim DeviceGUIDasString(40) As Byte
        SDL_JoystickGetGUIDString(SDL_JoystickGetDeviceGUID(_index), DeviceGUIDasString, 40)
        Dim GUIDSTRING As String = Encoding.ASCII.GetString(DeviceGUIDasString).ToString.Replace(vbNullChar, "").Trim

        Dim _SDLMapping = SDL_GameControllerMappingForGUID(SDL_JoystickGetDeviceGUID(_index))
        Dim _MednafenMapping As String = ""

        Dim MednafenControllerConfigLines As String()

        If File.Exists(MainformRef.NullDCPath & "\mednafenmapping.txt") Then
            MednafenControllerConfigLines = File.ReadAllLines(MainformRef.NullDCPath & "\mednafenmapping.txt")
        Else
            MednafenControllerConfigLines = {""}
        End If

        Dim MednafenMappingFound = False
        For Each _line In MednafenControllerConfigLines
            If _line.StartsWith(GUIDSTRING) Then
                MednafenMappingFound = True
                _MednafenMapping = _line.Trim
                Exit For
            End If
        Next

        If Not MednafenMappingFound Then
            Dim Joy = SDL_JoystickOpen(_index)
            Dim _numAxis = SDL_JoystickNumAxes(Joy)
            ' DO EXTRA CHECK HERE FOR XINPUT AND SET NUM AXIS TO 6 REGARDLESS

            ' If for w.e reason it could not get the mapping or one was never created for this device, then take a shot at using the xinput defaults
            If _SDLMapping Is Nothing Then
                _SDLMapping = "00000000000000000000000000000000,XInput Controller,a:b0,b:b1,back:b6,dpdown:h0.4,dpleft:h0.8,dpright:h0.2,dpup:h0.1,guide:b10,leftshoulder:b4,leftstick:b8,lefttrigger:a2,leftx:a0,lefty:a1,rightshoulder:b5,rightstick:b9,righttrigger:a5,rightx:a3,righty:a4,start:b7,x:b2,y:b3,platform:Windows,"
            ElseIf _SDLMapping = "" Then
                _SDLMapping = "00000000000000000000000000000000,XInput Controller,a:b0,b:b1,back:b6,dpdown:h0.4,dpleft:h0.8,dpright:h0.2,dpup:h0.1,guide:b10,leftshoulder:b4,leftstick:b8,lefttrigger:a2,leftx:a0,lefty:a1,rightshoulder:b5,rightstick:b9,righttrigger:a5,rightx:a3,righty:a4,start:b7,x:b2,y:b3,platform:Windows,"
            End If

            Dim MednafenTranslated = BEARButtonToMednafenButton(_SDLMapping, _numAxis)
            If _MednafenMapping = "" Then
                _MednafenMapping = GUIDSTRING
                For i = 0 To MednafenTranslated.Count - 1
                    _MednafenMapping += "," & MednafenTranslated.Keys(i) & ":" & MednafenTranslated.Values(i)
                Next
            End If

            SDL_JoystickClose(Joy)
        End If

        If Not _SDLMapping.Trim.EndsWith("platform:Windows,") Then
            If Not _SDLMapping.Trim.EndsWith(",") Then _SDLMapping += ","
            _SDLMapping += "platform:Windows,"

        End If

        Return _SDLMapping.Trim & "|" & _MednafenMapping.Trim

    End Function

    Public Function MednafenXinputButtonToSDLxInputButton(ByVal _medbutton As String) As String
        Dim Converted As String = _medbutton

        Dim Reverse As Boolean = False
        Select Case Converted
            Case "button_0" ' A
                Converted = "button_12"
            Case "button_1" ' B
                Converted = "button_13"
            Case "button_2" ' X
                Converted = "button_14"
            Case "button_3" ' Y
                Converted = "button_15"
            Case "button_4" ' LS
                Converted = "button_8"
            Case "button_5" ' RS
                Converted = "button_9"
            Case "button_6" ' Select
                Converted = "button_5"
            Case "button_7" ' Start
                Converted = "button_4"
            Case "button_8" ' L3
                Converted = "button_6"
            Case "button_9" ' R3
                Converted = "button_7"
        End Select

        'Dim AxisCount = SDL_JoystickNumAxes()

        ' Convert Axis
        If Converted.Contains("abs_0") Then
        ElseIf Converted.Contains("abs_1") Then
            Reverse = True
        ElseIf Converted.Contains("abs_2") Then
        ElseIf Converted.Contains("abs_3") Then
            Reverse = True
        ElseIf Converted.Contains("abs_4") Then
        ElseIf Converted.Contains("abs_5") Then
        ElseIf Converted.Contains("abs_6") Then
            If Converted.Contains("-") Then
                Converted = "button_2" ' DPAD
            Else
                Converted = "button_3" ' DPAD
            End If
        ElseIf Converted.Contains("abs_7") Then
            If Converted.Contains("-") Then
                Converted = "button_0" ' DPAD
            Else
                Converted = "button_1" ' DPAD
            End If
        End If

        If Reverse Then
            If Converted.Contains("-+") Then
                Converted = Converted.Replace("-+", "+-")
            ElseIf Converted.Contains("+-") Then
                Converted = Converted.Replace("+-", "-+")
            ElseIf Converted.Contains("+") Then
                Converted = Converted.Replace("+", "-")
            ElseIf Converted.Contains("-") Then
                Converted = Converted.Replace("-", "+")
            End If
        End If

        'Console.WriteLine("Xinput SDL to Mednafen Button: " & _medbutton & "|" & Converted)
        Return Converted
    End Function

End Module

Module BEARTheme

    Enum ThemeKeys
        PrimaryColor
        PrimaryFontColor
        PrimaryFontSize

        SecondaryColor
        SecondaryFontColor
        SecondaryFontSize

        TertiaryColor
        TertiaryFontColor
        TertiaryFontSize

        ButtonColor
        ButtonOverColor
        ButtonBorderColor
        ButtonFontColor
        ButtonFontSize

        DropdownColor
        DropdownFontColor
        DropdownFontSize

        MenuStripColor
        MenuStripHighlightColor
        MenuStripFontColor
        MenuStripFontSize

        LogoImage
        LogoLink

        MainMenuBackground
        WaitBackground
        WaitHostBackground
        ChallengeBackground
        NotificationBackground
        HostBackground

        WaitHostAnimation
        HostAnimation

        OverlayTop
        OverlayBottom

        PlayerListColor
        PlayerListFontColor
        PlayerListFontSize

        MatchListColor
        MatchListFontColor
        MatchListFontSize
    End Enum

    Public Theme As New Dictionary(Of String, String)

    Public Sub ApplyThemeToControl(ByRef _control As Control, Optional _which As Single = 1, Optional _type As Type = Nothing)

        Select Case _control.GetType()
            Case GetType(Button)
                ApplyButtonTheme(_control)
            Case GetType(CheckBox)
                ApplyCheckBoxTheme(_control, _which)
            Case GetType(ComboBox)
                ApplyComboBoxTheme(_control)
            Case GetType(Label)
                ApplyLabelTheme(_control, _which)
            Case GetType(GroupBox)
                ApplyGroupBoxTheme(_control, _which)
            Case GetType(MenuStrip)
                ApplyMenuStripTheme(_control)
            Case GetType(TableLayoutPanel)
                ApplyTableLayoutPanelTheme(_control, _which)
            Case GetType(ListView)
                ApplyListViewTheme(_control, _which)
            Case GetType(RichTextBox)
                ApplyRichTextBoxTheme(_control, _which)
            Case GetType(TextBox)
                ApplyTextBoxTheme(_control, _which)
            Case GetType(Form)
                ApplyFormTheme(_control, _which)
            Case GetType(MednafenSetting)
                ApplyMDFTheme(_control, _which)
            Case GetType(Panel)
                ApplyPanelTheme(_control, _which)
            Case GetType(FlowLayoutPanel)
                ApplyFlowPanelTheme(_control, _which)
            Case GetType(keybindButton)
                ApplyKeybindButtonTheme(_control)
            Case GetType(TabPage)
                ApplyTabPageTheme(_control, _which)
        End Select

    End Sub

    Private Sub ApplyTabPageTheme(ByRef _Control As TabPage, ByVal _which As Single)
        Select Case _which
            Case 1
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.PrimaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.PrimaryFontColor)
            Case 2
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.SecondaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.SecondaryFontColor)
            Case 3
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.TertiaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.TertiaryFontColor)
            Case 4
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.PlayerListColor)
                _Control.ForeColor = LoadColor(ThemeKeys.PlayerListFontColor)
            Case 5
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.MatchListColor)
                _Control.ForeColor = LoadColor(ThemeKeys.MatchListFontColor)
        End Select

    End Sub

    Private Sub ApplyFlowPanelTheme(ByRef _Control As FlowLayoutPanel, ByVal _which As Single)
        Select Case _which
            Case 1
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.PrimaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.PrimaryFontColor)
            Case 2
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.SecondaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.SecondaryFontColor)
            Case 3
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.TertiaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.TertiaryFontColor)
            Case 4
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.PlayerListColor)
                _Control.ForeColor = LoadColor(ThemeKeys.PlayerListFontColor)
            Case 5
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.MatchListColor)
                _Control.ForeColor = LoadColor(ThemeKeys.MatchListFontColor)
        End Select

    End Sub

    Private Sub ApplyPanelTheme(ByRef _Control As Panel, ByVal _which As Single)
        Select Case _which
            Case 1
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.PrimaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.PrimaryFontColor)
            Case 2
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.SecondaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.SecondaryFontColor)
            Case 3
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.TertiaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.TertiaryFontColor)
            Case 4
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.PlayerListColor)
                _Control.ForeColor = LoadColor(ThemeKeys.PlayerListFontColor)
            Case 5
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.MatchListColor)
                _Control.ForeColor = LoadColor(ThemeKeys.MatchListFontColor)
        End Select

    End Sub

    Private Sub ApplyMDFTheme(ByRef _Control As MednafenSetting, ByVal _which As Single)
        Select Case _which
            Case 1
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.PrimaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.PrimaryFontColor)
            Case 2
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.SecondaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.SecondaryFontColor)
            Case 3
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.TertiaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.TertiaryFontColor)
            Case 4
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.PlayerListColor)
                _Control.ForeColor = LoadColor(ThemeKeys.PlayerListFontColor)
            Case 5
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.MatchListColor)
                _Control.ForeColor = LoadColor(ThemeKeys.MatchListFontColor)
        End Select

    End Sub

    Private Sub ApplyFormTheme(ByRef _Control As TextBox, ByVal _which As Single)
        Select Case _which
            Case 1
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.PrimaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.PrimaryFontColor)
            Case 2
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.SecondaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.SecondaryFontColor)
            Case 3
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.TertiaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.TertiaryFontColor)
        End Select
    End Sub

    Private Sub ApplyButtonTheme(ByRef _button As Button)
        _button.FlatStyle = FlatStyle.Flat
        _button.BackColor = LoadColor(ThemeKeys.ButtonColor)
        _button.ForeColor = LoadColor(ThemeKeys.ButtonFontColor)
        _button.FlatAppearance.BorderColor = LoadColor(ThemeKeys.ButtonBorderColor)
        _button.FlatAppearance.MouseOverBackColor = LoadColor(ThemeKeys.ButtonOverColor)
        '_button.Font = New Font(_button.Font.FontFamily, LoadSize(ThemeKeys.ButtonFontSize), _button.Font.Style)

    End Sub

    Private Sub ApplyKeybindButtonTheme(ByRef _button As keybindButton)
        _button.FlatStyle = FlatStyle.Flat
        _button.BackColor = Color.White
        _button.ForeColor = Color.Black
        _button.FlatAppearance.BorderColor = Color.Black
        _button.FlatAppearance.MouseOverBackColor = Color.LightYellow
        '_button.Font = New Font(_button.Font.FontFamily, LoadSize(ThemeKeys.ButtonFontSize), _button.Font.Style)

    End Sub

    Private Sub ApplyTextBoxTheme(ByRef _Control As TextBox, ByVal _which As Single)
        Select Case _which
            Case 1
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.PrimaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.PrimaryFontColor)
                '_Control.Font = New Font(_Control.Font.FontFamily, LoadSize(ThemeKeys.PrimaryFontSize), _Control.Font.Style)
            Case 2
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.SecondaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.SecondaryFontColor)
                '_Control.Font = New Font(_Control.Font.FontFamily, LoadSize(ThemeKeys.SecondaryFontSize), _Control.Font.Style)
            Case 3
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.TertiaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.TertiaryFontColor)
                '_Control.Font = New Font(_Control.Font.FontFamily, LoadSize(ThemeKeys.TertiaryFontSize), _Control.Font.Style)
        End Select
    End Sub

    Private Sub ApplyRichTextBoxTheme(ByRef _Control As RichTextBox, ByVal _which As Single)
        Select Case _which
            Case 1
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.PrimaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.PrimaryFontColor)
                '_Control.Font = New Font(_Control.Font.FontFamily, LoadSize(ThemeKeys.PrimaryFontSize), _Control.Font.Style)
            Case 2
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.SecondaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.SecondaryFontColor)
                '_Control.Font = New Font(_Control.Font.FontFamily, LoadSize(ThemeKeys.SecondaryFontSize), _Control.Font.Style)
            Case 3
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.TertiaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.TertiaryFontColor)
                '_Control.Font = New Font(_Control.Font.FontFamily, LoadSize(ThemeKeys.TertiaryFontSize), _Control.Font.Style)
        End Select
    End Sub

    Private Sub ApplyListViewTheme(ByRef _Control As ListView, ByVal _which As Single)
        Select Case _which
            Case 1
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.PrimaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.PrimaryFontColor)
                '_Control.Font = New Font(_Control.Font.FontFamily, LoadSize(ThemeKeys.PrimaryFontSize), _Control.Font.Style)
            Case 2
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.SecondaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.SecondaryFontColor)
                '_Control.Font = New Font(_Control.Font.FontFamily, LoadSize(ThemeKeys.SecondaryFontSize), _Control.Font.Style)
            Case 3
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.TertiaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.TertiaryFontColor)
                '_Control.Font = New Font(_Control.Font.FontFamily, LoadSize(ThemeKeys.TertiaryFontSize), _Control.Font.Style)
            Case 4
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.PlayerListColor)
                _Control.ForeColor = LoadColor(ThemeKeys.PlayerListFontColor)
                '_Control.Font = New Font(_Control.Font.FontFamily, LoadSize(ThemeKeys.PlayerListFontSize), _Control.Font.Style)
            Case 5
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.MatchListColor)
                _Control.ForeColor = LoadColor(ThemeKeys.MatchListFontColor)
                '_Control.Font = New Font(_Control.Font.FontFamily, LoadSize(ThemeKeys.MatchListFontSize), _Control.Font.Style)
        End Select
    End Sub

    Private Sub ApplyTableLayoutPanelTheme(ByRef _Control As TableLayoutPanel, ByVal _which As Single)
        Select Case _which
            Case 1
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.PrimaryColor)
            Case 2
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.SecondaryColor)
            Case 3
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.TertiaryColor)
        End Select
    End Sub


    Private Sub ApplyMenuStripTheme(ByRef _Control As MenuStrip)
        _Control.BackColor = LoadColor(ThemeKeys.MenuStripColor)
        _Control.ForeColor = LoadColor(ThemeKeys.MenuStripFontColor)

        '_Control.Font = New Font(_Control.Font.FontFamily, LoadSize(ThemeKeys.MenuStripFontSize), _Control.Font.Style)
        _Control.Renderer = New MenuStripRenderer

    End Sub

    Private Sub ApplyGroupBoxTheme(ByRef _Control As GroupBox, ByVal _which As Single)
        Select Case _which
            Case 1
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.PrimaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.PrimaryFontColor)
                '_Control.Font = New Font(_Control.Font.FontFamily, LoadSize(ThemeKeys.PrimaryFontSize), _Control.Font.Style)
            Case 2
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.SecondaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.SecondaryFontColor)
                '_Control.Font = New Font(_Control.Font.FontFamily, LoadSize(ThemeKeys.SecondaryFontSize), _Control.Font.Style)
            Case 3
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.TertiaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.TertiaryFontColor)
                '_Control.Font = New Font(_Control.Font.FontFamily, LoadSize(ThemeKeys.TertiaryFontSize), _Control.Font.Style)
        End Select
    End Sub

    Private Sub ApplyCheckBoxTheme(ByRef _Control As CheckBox, ByVal _which As Single)
        Select Case _which
            Case 1
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.PrimaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.PrimaryFontColor)
                '_Control.Font = New Font(_Control.Font.FontFamily, LoadSize(ThemeKeys.PrimaryFontSize), _Control.Font.Style)
            Case 2
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.SecondaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.SecondaryFontColor)
                '_Control.Font = New Font(_Control.Font.FontFamily, LoadSize(ThemeKeys.SecondaryFontSize), _Control.Font.Style)
            Case 3
                If Not _Control.BackColor = Color.Transparent Then _Control.BackColor = LoadColor(ThemeKeys.TertiaryColor)
                _Control.ForeColor = LoadColor(ThemeKeys.TertiaryFontColor)
                '_Control.Font = New Font(_Control.Font.FontFamily, LoadSize(ThemeKeys.TertiaryFontSize), _Control.Font.Style)
        End Select

    End Sub

    Private Sub ApplyLabelTheme(ByRef _control As Label, ByVal _which As Single)
        Select Case _which
            Case 1
                If Not _control.BackColor = Color.Transparent Then _control.BackColor = LoadColor(ThemeKeys.PrimaryColor)
                _control.ForeColor = LoadColor(ThemeKeys.PrimaryFontColor)
                '_control.Font = New Font(_control.Font.FontFamily, LoadSize(ThemeKeys.PrimaryFontSize), _control.Font.Style)
            Case 2
                If Not _control.BackColor = Color.Transparent Then _control.BackColor = LoadColor(ThemeKeys.SecondaryColor)
                _control.ForeColor = LoadColor(ThemeKeys.SecondaryFontColor)
                '_control.Font = New Font(_control.Font.FontFamily, LoadSize(ThemeKeys.SecondaryFontSize), _control.Font.Style)
            Case 3
                If Not _control.BackColor = Color.Transparent Then _control.BackColor = LoadColor(ThemeKeys.TertiaryColor)
                _control.ForeColor = LoadColor(ThemeKeys.TertiaryFontColor)
                '_control.Font = New Font(_control.Font.FontFamily, LoadSize(ThemeKeys.TertiaryFontSize), _control.Font.Style)
        End Select

    End Sub

    Private Sub ApplyComboBoxTheme(ByRef _control As ComboBox)
        _control.BackColor = LoadColor(ThemeKeys.DropdownColor)
        _control.ForeColor = LoadColor(ThemeKeys.DropdownFontColor)
        '_control.Font = New Font(_control.Font.FontFamily, LoadSize(ThemeKeys.DropdownFontSize), _control.Font.Style)

    End Sub

    Public Function LoadSize(ByVal _font As ThemeKeys) As Single
        If Theme.ContainsKey(_font.ToString().ToLower) Then
            If Not Theme(_font.ToString().ToLower) = "" Then
                Dim a As Single
                If Single.TryParse(Theme(_font.ToString().ToLower), a) Then
                    Return a
                Else
                    Return 8.2
                End If
            Else
                Return 8.2
            End If
        Else
            Return 8.2
        End If

    End Function

    Public Function LoadLink(ByVal _url As ThemeKeys)
        Return Theme(_url.ToString().ToLower)

    End Function

    Public Function LoadColor(ByVal _color As ThemeKeys) As Color
        If Theme.ContainsKey(_color.ToString().ToLower) Then
            If Not Theme(_color.ToString().ToLower) = "" Then
                Return ColorTranslator.FromHtml(Theme(_color.ToString().ToLower))
            Else
                Return Color.FromArgb(255, 0, 255)
            End If
        Else
            Return Color.FromArgb(255, 0, 255)
        End If

    End Function

    Public Function LoadImage(ByVal _image As ThemeKeys) As Image
        If Theme.ContainsKey(_image.ToString.ToLower) Then
            Return Bitmap.FromFile(MainformRef.NullDCPath & "\themes\" & MainformRef.ConfigFile.Theme & "\images\" & Theme(_image.ToString.ToLower))
        Else
            Return Nothing
        End If

    End Function

    Public Sub LoadNewTheme()

        MainformRef.LoadThemeSettings()

        For Each _form As Form In Application.OpenForms

            Select Case _form.GetType
                Case GetType(frmChallenge)
                    DirectCast(_form, frmChallenge).ReloadTheme()
                Case GetType(frmChallengeGameSelect)
                    DirectCast(_form, frmChallengeGameSelect).ReloadTheme()
                Case GetType(frmChallengeSent)
                    DirectCast(_form, frmChallengeSent).ReloadTheme()
                Case GetType(frmDelayHelp)
                    DirectCast(_form, frmDelayHelp).ReloadTheme()
                Case GetType(frmDLC)
                    DirectCast(_form, frmDLC).ReloadTheme()
                Case GetType(frmHostPanel)
                    DirectCast(_form, frmHostPanel).ReloadTheme()
                Case GetType(frmLoLNerd)
                    DirectCast(_form, frmLoLNerd).ReloadTheme()
                Case GetType(frmMain)
                    DirectCast(_form, frmMain).ReloadTheme()
                Case GetType(frmNotification)
                    DirectCast(_form, frmNotification).ReloadTheme()
                Case GetType(frmReplays)
                    DirectCast(_form, frmReplays).ReloadTheme()
                Case GetType(frmSetup)
                    DirectCast(_form, frmSetup).ReloadTheme()
                Case GetType(frmWaitingForHost)
                    DirectCast(_form, frmWaitingForHost).ReloadTheme()
                Case GetType(frmMednafenOptions)
                    DirectCast(_form, frmMednafenOptions).ReloadTheme()
                Case GetType(frmDM)
                    DirectCast(_form, frmDM).ReloadTheme()
                Case GetType(frmKeyMapperSDL)
                    DirectCast(_form, frmKeyMapperSDL).ReloadTheme()
            End Select
        Next

    End Sub

End Module

Class ListViewItemComparer
    Implements IComparer
    Private col As Integer
    Private order As SortOrder

    Public Sub New()
        col = 0
        order = SortOrder.Ascending
    End Sub

    Public Sub New(column As Integer, order As SortOrder)
        col = column
        Me.order = order
    End Sub

    Public Function Compare(x As Object, y As Object) As Integer Implements IComparer.Compare

        Dim returnVal As Integer = -1
        returnVal = [String].Compare(CType(x, ListViewItem).SubItems(col).Text, CType(y, ListViewItem).SubItems(col).Text)
        If order = SortOrder.Descending Then returnVal *= -1
        Return returnVal

    End Function

End Class

Public Class MenuStripRenderer : Inherits ToolStripProfessionalRenderer

    Protected Overrides Sub OnRenderItemText(e As ToolStripItemTextRenderEventArgs)
        Dim tclr As Color = BEARTheme.LoadColor(ThemeKeys.MenuStripFontColor)
        TextRenderer.DrawText(e.Graphics, e.Text, e.TextFont, e.TextRectangle, BEARTheme.LoadColor(ThemeKeys.MenuStripFontColor), TextFormatFlags.VerticalCenter Or TextFormatFlags.Left Or TextFormatFlags.SingleLine)

    End Sub


    Protected Overrides Sub OnRenderMenuItemBackground(ByVal e As System.Windows.Forms.ToolStripItemRenderEventArgs)
        If e.Item.Selected Then
            Dim rc As New Rectangle(Point.Empty, e.Item.Size)
            Dim Pen = New Pen(BEARTheme.LoadColor(ThemeKeys.MenuStripFontColor), 2)
            Pen.Alignment = PenAlignment.Inset

            'Set the highlight color
            e.Graphics.FillRectangle(New SolidBrush(BEARTheme.LoadColor(ThemeKeys.MenuStripHighlightColor)), rc)
            'e.Graphics.DrawRectangle(Pens.Beige, 1, 0, rc.Width - 2, rc.Height - 1)
        Else
            Dim rc As New Rectangle(Point.Empty, e.Item.Size)
            Dim Pen = New Pen(BEARTheme.LoadColor(ThemeKeys.MenuStripFontColor), 2)
            Pen.Alignment = PenAlignment.Inset

            'Set the default color
            e.Graphics.FillRectangle(New SolidBrush(BEARTheme.LoadColor(ThemeKeys.MenuStripColor)), rc)
            'e.Graphics.DrawRectangle(Pens.Gray, 1, 0, rc.Width - 2, rc.Height - 1)

        End If
    End Sub

End Class

