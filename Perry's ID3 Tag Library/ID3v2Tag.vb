Public Class ID3v2Tag

    Public gTagReader As IO.FileStream
    Public gTagString As String

    ' TODO: 
    Class HeaderFlagsClass
        Public UnsynchronizationFlag As Boolean
        Public ExtendedHeaderFlag As Boolean
        Public ExperimentalIndicatorFlag As Boolean
    End Class

    Class FrameFlagsClass
        Public TagAlterPreservationFlag As Boolean
        Public FileAlterPreservationFlag As Boolean
        Public ReadOnlyFlag As Boolean
        Public CompressionFlag As Boolean
        Public EncryptionFlag As Boolean
        Public GroupingIdentityFlag As Boolean
    End Class

    Class FrameCollection
        Inherits Hashtable

        Sub AddFrame(ByVal argFrame As FrameBase)
            If Me.ContainsKey(argFrame.ID) = True Then
                Dim mFrameList As ArrayList
                mFrameList = Me.Values(argFrame.ID)
                mFrameList.Add(argFrame)
            Else
                Dim mFrameList As New ArrayList
                mFrameList.Add(argFrame)
                Me.Add(argFrame.ID, mFrameList)
            End If
        End Sub
    End Class

    Public Class FrameBase
        Public ID As String
        Public Size As Integer
        Public Flags As String
        Public ValueBytes() As Byte

        Public Function Value() As String
            Return BytesToText(ValueBytes, True)
        End Function

    End Class

    ' define tag header variables
    Public FileIdentifier As String
    Public TagVersion As String
    Public TagSize As String
    Public HeaderFlags As String

    Public Frames As New ArrayList
    Public RawAscii As String
    Public RawBytes As String

    'TODO: these functions don't work for ID3v2.2.0 because those frame id's are 3 characters instead of 4
    Public Function Artwork(ByVal argFrameSize As Integer) As Drawing.Image
        Dim img As Drawing.Image = Nothing
        For Each mFrame As FrameBase In Me.Frames
            'If mFrame.ID = "APIC" And mFrame.Size = argFrameSize Then
            If mFrame.ID = "APIC" Or mFrame.ID = "PIC" Then

                ' TODO:
                ' read the text encoding (1 byte)
                ' read the mime type (variable length, terminated with 0)
                ' read the picture type (1 byte)
                ' read the description (variable length, terminated with 0)
                ' remaining data is image data
                ' currently, a fixed APIC header size of 14 is always assumed

                Dim mApicTextEncoding As Byte
                Dim mApicMimeType As New ArrayList
                Dim mApicPictureType As Byte
                Dim mApicDescription As String = ""
                Dim mApicDataStart As Integer = 7 '14

                Dim c As String = ""
                Dim i As Integer = 0

                mApicTextEncoding = mFrame.ValueBytes(i)
                i += 1

                c = ""
                Do Until c = "0"
                    c = mFrame.ValueBytes(i).ToString
                    mApicMimeType.Add(c)
                    i = i + 1
                Loop

                mApicPictureType = mFrame.ValueBytes(i).ToString
                i += 1

                c = ""
                Do Until c = "0"
                    c = mFrame.ValueBytes(i).ToString
                    mApicDescription &= c.ToString
                    i = i + 1
                Loop

                mApicDataStart = i

                Dim ms As IO.MemoryStream
                ms = New IO.MemoryStream(mFrame.ValueBytes, mApicDataStart, mFrame.ValueBytes.Length - mApicDataStart)

                img = Drawing.Image.FromStream(ms)

            End If
        Next
        Return img
    End Function

    Public Function Title() As String
        Return GetFrameValue("TIT2")
    End Function

    Public Function Album() As String
        Dim val As String
        val = GetFrameValue("TALB")
        If val = "" Then val = GetFrameValue("TAL")
        Return val
    End Function

    Public Function Artist() As String
        Dim val As String
        val = GetFrameValue("TPE1")
        If val = "" Then val = GetFrameValue("TP1")
        Return val
    End Function

    Public Function Year() As String
        Dim val As String
        val = GetFrameValue("TDRC")
        If val = "" Then val = GetFrameValue("TRD")
        Return val
    End Function

    Public Function Comment() As String
        Dim val As String
        val = GetFrameValue("COMM")
        If val = "" Then val = GetFrameValue("COM")
        Return val
    End Function

    Public Function Genre() As String
        Dim val As String
        val = GetFrameValue("TCON")
        If val = "" Then val = GetFrameValue("TCO")
        Return val
    End Function

    Public Function TrackNumber() As String
        Dim val As String
        val = GetFrameValue("TRCK")
        If val = "" Then val = GetFrameValue("TRK")
        Return val
    End Function

    Public Function Length() As String
        Dim val As String
        val = GetFrameValue("TLEN")
        If val = "" Then val = GetFrameValue("TLE")
        Return val
    End Function

    Function GetFrameValue(ByVal argFrameName As String) As String
        For i As Integer = 0 To Me.Frames.Count - 1
            Dim mFrame As FrameBase
            mFrame = Me.Frames(i)
            If mFrame.ID = argFrameName Then Return mFrame.Value
        Next
        Return Nothing
    End Function

    Sub New(ByVal argFileInfo As IO.FileInfo)
        ParseTag(argFileInfo)
    End Sub

    'Sub New(ByVal argFileInfo As IO.FileInfo)

    '    ParseTag(argFileInfo)
    '    Exit Sub

    '    Dim mReader As IO.FileStream = argFileInfo.OpenRead

    '    Dim mFileIdentifier(2) As Byte
    '    Dim mTagVersion(1) As Byte
    '    Dim mTagSize(3) As Byte
    '    Dim mHeaderFlags(0) As Byte

    '    ' start at the beginning of the file
    '    mReader.Position = 0

    '    ' read the file identifier and make sure the value equals "ID3" which indicates an ID3v2 tag
    '    mReader.Read(mFileIdentifier, 0, 3)
    '    Me.FileIdentifier = BytesToText(mFileIdentifier, True)

    '    If Me.FileIdentifier = "ID3" Then

    '        ' 1) get the tag version from the tag header

    '        ' read the 2 bytes where the tag version is stored
    '        mReader.Read(mTagVersion, 0, 2)

    '        ' set the MP3 Tag Version
    '        Me.TagVersion = mTagVersion(0) & "." & mTagVersion(1)

    '        ' 2) get the header flags from the tag header
    '        'TODO: handle multiple versions here, e.g. v2.4.0 has 4 header flags where v.2.3.0 only has 3

    '        ' read the 1 byte where the header flags are stored
    '        mReader.Read(mHeaderFlags, 0, 1)

    '        ' set the MP3 Header Flags
    '        If BitManipulator.ExamineBit(mHeaderFlags(0), 8) = True Then Me.HeaderFlags = "Unsynchronization"
    '        If BitManipulator.ExamineBit(mHeaderFlags(0), 7) = True Then Me.HeaderFlags &= ", " & "Extended Header"
    '        If BitManipulator.ExamineBit(mHeaderFlags(0), 6) = True Then Me.HeaderFlags &= ", " & "Experimental Indicator"

    '        ' 3) get the tag size from the tag header

    '        ' read the 4 bytes where the tag size is stored
    '        mReader.Read(mTagSize, 0, 4)

    '        ' set the MP3 Tag Size
    '        Me.TagSize = GetID3EncodedSize(mTagSize)

    '        ' read the entire tag, minus the header

    '        Dim mEntireTag(Me.TagSize - 1) As Byte
    '        mReader.Read(mEntireTag, 0, Me.TagSize)

    '        ' increment by 1 char
    '        ' if char = 0 then end current find
    '        Dim val As String = ""
    '        For i As Integer = 0 To mEntireTag.Length - 1

    '            Dim mNextByte As Byte = mEntireTag(i)
    '            Dim mNextChar As String

    '            If mNextByte > 31 And mNextByte < 127 Then
    '                mNextChar = Chr(mNextByte)
    '                val &= mNextChar
    '            Else
    '                ' stop here and find the next valid char
    '            End If

    '        Next

    '        'Exit Sub

    '        mReader.Position = 10


    '        ' 4) get the frames and parse them

    '        Do Until mReader.Position >= Me.TagSize ' TODO change 800

    '            ' 1) get the next frame

    '            Dim mFrame As New FrameBase

    '            Dim mFrameIDBytes() As Byte
    '            Dim mFrameSizeBytes() As Byte
    '            Dim mFrameFlagsBytes(1) As Byte
    '            Dim mFrameValueBytes() As Byte

    '            ' TODO: Me.TagVersion contains the ID3v2 version, so 2.0 denotes ID3v2.2.0
    '            Select Case Me.TagVersion

    '                Case "2.0"

    '                    ReDim mFrameIDBytes(2)
    '                    ReDim mFrameSizeBytes(2)

    '                    mReader.Read(mFrameIDBytes, 0, 3)
    '                    mReader.Read(mFrameSizeBytes, 0, 3)

    '                    mFrame.ID = BytesToText(mFrameIDBytes, False)
    '                    mFrame.Size = GetID3EncodedSize(mFrameSizeBytes)

    '                Case Else

    '                    ReDim mFrameIDBytes(3)
    '                    ReDim mFrameSizeBytes(3)

    '                    ' read the frame header
    '                    mReader.Read(mFrameIDBytes, 0, 4)
    '                    mReader.Read(mFrameSizeBytes, 0, 4)
    '                    mReader.Read(mFrameFlagsBytes, 0, 2)

    '                    ' get the frame id, which indicates the type of frame
    '                    mFrame.ID = BytesToText(mFrameIDBytes, False)

    '                    ' get the frame size by reading the next four bytes and converting those bytes from HEX to DEC
    '                    'mFrame.Size = mFrameSizeBytes(0) + mFrameSizeBytes(1) + mFrameSizeBytes(2) + mFrameSizeBytes(3)
    '                    mFrame.Size = GetID3EncodedSize(mFrameSizeBytes) + 10

    '                    ' get the frame flags which will not be used
    '                    'mFrame.Flags = BytesToText(mFrameFlagsBytes, False)

    '                    If BitManipulator.ExamineBit(mFrameFlagsBytes(0), 8) = True Then mFrame.Flags &= "Tag Alter Preservation, "
    '                    If BitManipulator.ExamineBit(mFrameFlagsBytes(0), 7) = True Then mFrame.Flags &= "File Alter Preservation, "
    '                    If BitManipulator.ExamineBit(mFrameFlagsBytes(0), 6) = True Then mFrame.Flags &= "Read Only, "

    '                    If BitManipulator.ExamineBit(mFrameFlagsBytes(1), 8) = True Then mFrame.Flags &= "Compression, "
    '                    If BitManipulator.ExamineBit(mFrameFlagsBytes(1), 7) = True Then mFrame.Flags &= "Encryption, "
    '                    If BitManipulator.ExamineBit(mFrameFlagsBytes(1), 6) = True Then mFrame.Flags &= "Group Identity"

    '            End Select

    '            ' get the frame value
    '            ReDim mFrameValueBytes(mFrame.Size - 1)
    '            mReader.Read(mFrameValueBytes, 0, mFrame.Size)
    '            'mFrame.Value = BytesToText(mFrameValueBytes, True)
    '            mFrame.ValueBytes = mFrameValueBytes

    '            ' add the frame to the collection
    '            Me.Frames.Add(mFrame)

    '            '' determine what to do with the frame
    '            'Select Case mFrameID
    '            '    Case "TIT2" ' Title
    '            '        Me.Title = mFrame
    '            '    Case "TPE1" ' Artist
    '            '        Me.Artist = mFrame
    '            '    Case "TALB" ' Album
    '            '        Me.Album = mFrame
    '            '    Case "TRCK" ' Track number
    '            '        Me.TrackNumber = mFrame
    '            '    Case "TLEN" ' Length of song in milliseconds

    '            '    Case "COMM" ' Comments
    '            '        Me.Comments = mFrame
    '            '    Case "TCON" ' Content Type (aka Genre)
    '            '        Me.Genre = mFrame
    '            '    Case "TDAT", "TDRC" ' Date (aka Year)
    '            '        Me.Year = mFrame
    '            'End Select

    '        Loop

    '    Else

    '        MsgBox("Could not find an ID3v2 tag!")

    '    End If

    '    mReader.Close()

    '    ' get the raw info
    '    mReader = argFileInfo.OpenRead
    '    mReader.Position = 0
    '    Dim mRawInfo(4999) As Byte ' get the first megabyte of raw data as ASCII
    '    mReader.Position = 0
    '    mReader.Read(mRawInfo, 0, 5000)
    '    Me.RawAscii = BytesToText(mRawInfo, False)
    '    Me.RawBytes = BytesToAsciiCodes(mRawInfo)

    'End Sub

    'TODO: Descriptor 3bytes -> Tag Version 2bytes -> Header Flags 1byte -> Size 4bytes -> Frames Nbytes
    Sub ParseTag(ByVal argFileInfo As IO.FileInfo)

        ' create a file reader to read the tag data
        gTagReader = argFileInfo.OpenRead

        ' parse the tag version
        ParseTagVersion()

        ' parse the tag header and frames according to version
        Select Case Me.TagVersion
            Case "2.0"
                ParseHeaderV2()
                ParseFramesV2() ' FrameID 3bytes FrameSize 3bytes FrameData Nbytes
            Case "3.0"
                ParseHeaderV4()
                ParseFramesV3()
            Case "4.0"
                ParseHeaderV4()
                ParseFramesV4()
            Case "Unknown Descriptor"

        End Select

        ' close the file handle (remove the lock)
        gTagReader.Close()
        gTagReader.Dispose()

    End Sub

    Sub ParseTagVersion()

        Dim mTagDescriptor(2) As Byte
        Dim mTagVersionBytes(1) As Byte

        ' read the tag info
        gTagReader.Read(mTagDescriptor, 0, 3)
        gTagReader.Read(mTagVersionBytes, 0, 2)

        If BytesToText(mTagDescriptor, False) = "ID3" Then
            Me.FileIdentifier = "ID3"
            Me.TagVersion = mTagVersionBytes(0) & "." & mTagVersionBytes(1)
        Else
            Me.TagVersion = ""
        End If

    End Sub

    Sub ParseFramesV2()

        ' write the tag data to file for analysis
        'Dim mPos As Integer = gTagReader.Position
        'Dim b(Me.TagSize - mPos) As Byte
        'gTagReader.Read(b, 0, Me.TagSize - mPos)
        'Dim f As New IO.StreamWriter("C:\tag.txt")
        'For i As Integer = 0 To b.Length - 1
        '    f.Write(Chr(b(i)))
        'Next
        'f.WriteLine("-----")
        'For i As Integer = 0 To b.Length - 1
        '    f.WriteLine(b(i) & vbTab & vbTab & Chr(b(i)))
        'Next
        'f.Dispose()

        'gTagReader.Position = mPos

        gTagReader.Position = 10

        Do Until gTagReader.Position >= Me.TagSize

            Dim mFrame As New FrameBase
            Dim mFrameIdBytes(2) As Byte
            Dim mFrameSizeBytes(2) As Byte

            ' read the FrameID, FrameSize and FrameFlags
            gTagReader.Read(mFrameIdBytes, 0, 3)
            gTagReader.Read(mFrameSizeBytes, 0, 3)

            ' exit out if padding found (byte = 0)
            If mFrameIdBytes(0) = 0 Then
                Exit Do
            End If

            ' parse the Frame ID
            mFrame.ID = BytesToText(mFrameIdBytes, False)

            ' parse the Frame Size (4 bytes)
            ' TODO: GetID3EncodedSize does not seem to work for frame size, only for tag size, fix it
            'mFrame.Size = GetID3EncodedSize(mFrameSizeBytes) + 10
            'Return CByte(argBytes(0)) Or (CInt(argBytes(1)) << 8) Or (CInt(argBytes(2)) << 16) Or (CInt(argBytes(3)) << 24)
            'mFrame.Size = (65536 * (mFrameSizeBytes(0) * 256 + mFrameSizeBytes(1))) + (mFrameSizeBytes(2) * 256 + mFrameSizeBytes(3))
            'mFrame.Size = mFrameSizeBytes(0) + mFrameSizeBytes(1) + mFrameSizeBytes(2)

            Dim mSizeV2 As Integer = GetID3EncodedSizeV2(mFrameSizeBytes) '+ 10
            'Dim mSizeV3 As Integer = GetID3EncodedSizeV3(mFrameSizeBytes)
            'Dim mSizeV4 As Integer = GetID3EncodedSizeV4(mFrameSizeBytes) '+ 10

            mFrame.Size = mSizeV2

            ' read the Frame Data (mFrame.Size - 10 bytes)
            ReDim mFrame.ValueBytes(mFrame.Size)
            gTagReader.Read(mFrame.ValueBytes, 0, mFrame.Size)

            ' check the Frame Data
            Dim mVal As String = mFrame.Value

            ' add the frame to the collection
            Me.Frames.Add(mFrame)

        Loop

    End Sub

    Sub ParseFramesV3()

        '' write the tag data to file for analysis
        'Dim mPos As Integer = gTagReader.Position
        'Dim b(Me.TagSize - mPos) As Byte
        'gTagReader.Read(b, 0, Me.TagSize - mPos)
        'Dim f As New IO.StreamWriter("C:\tag.txt")
        'For i As Integer = 0 To b.Length - 1
        '    If b(i) > 32 And b(i) < 128 Then
        '        f.Write(Chr(b(i)))
        '    Else
        '        f.Write("?")
        '    End If
        'Next
        'f.WriteLine("-----")
        'For i As Integer = 0 To b.Length - 1
        '    f.WriteLine(b(i) & vbTab & vbTab & Chr(b(i)))
        'Next
        'f.Dispose()

        gTagReader.Position = 10

        Do Until gTagReader.Position >= Me.TagSize

            Dim mFrame As New FrameBase
            Dim mFrameIdBytes(3) As Byte
            Dim mFrameSizeBytes(3) As Byte
            Dim mFrameFlagsBytes(1) As Byte

            ' read the FrameID, FrameSize and FrameFlags
            gTagReader.Read(mFrameIdBytes, 0, 4)
            gTagReader.Read(mFrameSizeBytes, 0, 4)
            gTagReader.Read(mFrameFlagsBytes, 0, 2)

            ' exit out if padding (bytes = 0)
            If mFrameIdBytes(0) = 0 Then
                Exit Do
            End If

            ' parse the Frame ID (4 bytes)
            Dim mFrameID As String = BytesToText(mFrameIdBytes, False)
            mFrame.ID = mFrameID

            ' parse the Frame Size (4 bytes)
            ' TODO: GetID3EncodedSize does not seem to work for frame size, only for tag size, fix it
            'mFrame.Size = GetID3EncodedSize(mFrameSizeBytes) + 10
            'Return CByte(argBytes(0)) Or (CInt(argBytes(1)) << 8) Or (CInt(argBytes(2)) << 16) Or (CInt(argBytes(3)) << 24)
            'mFrame.Size = (65536 * (mFrameSizeBytes(0) * 256 + mFrameSizeBytes(1))) + (mFrameSizeBytes(2) * 256 + mFrameSizeBytes(3))

            'Dim mSizeV2 As Integer = GetID3EncodedSizeV2(mFrameSizeBytes) '+ 10
            Dim mSizeV3 As Integer = GetID3EncodedSizeV3(mFrameSizeBytes)
            'Dim mSizeV4 As Integer = GetID3EncodedSizeV4(mFrameSizeBytes) '+ 10

            mFrame.Size = mSizeV3

            ' preview the flags as a string for debug
            Dim flags As String = GetBitString(mFrameFlagsBytes)

            ' parse the Frame Flags (2 bytes)
            If BitManipulator.ExamineBit(mFrameFlagsBytes(1), 8) = True Then mFrame.Flags &= "Tag Alter Preservation, "
            If BitManipulator.ExamineBit(mFrameFlagsBytes(1), 7) = True Then mFrame.Flags &= "File Alter Preservation, "
            If BitManipulator.ExamineBit(mFrameFlagsBytes(1), 6) = True Then mFrame.Flags &= "Read Only, "

            If BitManipulator.ExamineBit(mFrameFlagsBytes(0), 8) = True Then mFrame.Flags &= "Compression, "
            If BitManipulator.ExamineBit(mFrameFlagsBytes(0), 7) = True Then mFrame.Flags &= "Encryption, "
            If BitManipulator.ExamineBit(mFrameFlagsBytes(0), 6) = True Then mFrame.Flags &= "Group Identity"

            ' read the Frame Data (mFrame.Size - 10 bytes)
            ReDim mFrame.ValueBytes(mFrame.Size)
            gTagReader.Read(mFrame.ValueBytes, 0, mFrame.Size)

            ' check the Frame Data
            Dim mVal As String = mFrame.Value

            ' add the frame to the collection
            Me.Frames.Add(mFrame)

        Loop

    End Sub

    Sub ParseFramesV4()

        '' write the tag data to file for analysis
        'Dim mPos As Integer = gTagReader.Position
        'Dim b(Me.TagSize - mPos) As Byte
        'gTagReader.Read(b, 0, Me.TagSize - mPos)
        'Dim f As New IO.StreamWriter("C:\tag.txt")
        'For i As Integer = 0 To b.Length - 1
        '    If b(i) > 32 And b(i) < 128 Then
        '        f.Write(Chr(b(i)))
        '    Else
        '        f.Write("?")
        '    End If
        'Next
        'f.WriteLine("-----")
        'For i As Integer = 0 To b.Length - 1
        '    f.WriteLine(b(i) & vbTab & vbTab & Chr(b(i)))
        'Next
        'f.Dispose()

        gTagReader.Position = 10

        Do Until gTagReader.Position >= Me.TagSize

            Dim mFrame As New FrameBase
            Dim mFrameIdBytes(3) As Byte
            Dim mFrameSizeBytes(3) As Byte
            Dim mFrameFlagsBytes(1) As Byte

            ' read the FrameID, FrameSize and FrameFlags
            gTagReader.Read(mFrameIdBytes, 0, 4)
            gTagReader.Read(mFrameSizeBytes, 0, 4)
            gTagReader.Read(mFrameFlagsBytes, 0, 2)

            ' exit out if padding (bytes = 0)
            If mFrameIdBytes(0) = 0 Then
                Exit Do
            End If

            ' parse the Frame ID (4 bytes)
            Dim mFrameID As String = BytesToText(mFrameIdBytes, False)
            mFrame.ID = mFrameID

            ' parse the Frame Size (4 bytes)
            ' TODO: GetID3EncodedSize does not seem to work for frame size, only for tag size, fix it
            'mFrame.Size = GetID3EncodedSize(mFrameSizeBytes) + 10
            'Return CByte(argBytes(0)) Or (CInt(argBytes(1)) << 8) Or (CInt(argBytes(2)) << 16) Or (CInt(argBytes(3)) << 24)
            'mFrame.Size = (65536 * (mFrameSizeBytes(0) * 256 + mFrameSizeBytes(1))) + (mFrameSizeBytes(2) * 256 + mFrameSizeBytes(3))

            Dim mSizeV2 As Integer = GetID3EncodedSizeV2(mFrameSizeBytes) '+ 10
            Dim mSizeV3 As Integer = GetID3EncodedSizeV3(mFrameSizeBytes)
            Dim mSizeV4 As Integer = GetID3EncodedSizeV4(mFrameSizeBytes) '+ 10

            mFrame.Size = mSizeV4

            ' preview the flags as a string for debug
            Dim flags As String = GetBitString(mFrameFlagsBytes)

            ' parse the Frame Flags (2 bytes)
            If BitManipulator.ExamineBit(mFrameFlagsBytes(1), 7) = True Then mFrame.Flags &= "Tag Alter Preservation, "
            If BitManipulator.ExamineBit(mFrameFlagsBytes(1), 6) = True Then mFrame.Flags &= "File Alter Preservation, "
            If BitManipulator.ExamineBit(mFrameFlagsBytes(1), 5) = True Then mFrame.Flags &= "Read Only, "

            If BitManipulator.ExamineBit(mFrameFlagsBytes(0), 7) = True Then mFrame.Flags &= "Group Identity, "
            If BitManipulator.ExamineBit(mFrameFlagsBytes(0), 4) = True Then mFrame.Flags &= "Compression, "
            If BitManipulator.ExamineBit(mFrameFlagsBytes(0), 3) = True Then mFrame.Flags &= "Encryption"
            If BitManipulator.ExamineBit(mFrameFlagsBytes(0), 2) = True Then mFrame.Flags &= "Unsynchronization"
            If BitManipulator.ExamineBit(mFrameFlagsBytes(0), 1) = True Then mFrame.Flags &= "Data Length Indicator"

            ' read the Frame Data (mFrame.Size - 10 bytes)
            ReDim mFrame.ValueBytes(mFrame.Size)
            gTagReader.Read(mFrame.ValueBytes, 0, mFrame.Size)

            ' check the Frame Data
            Dim mVal As String = mFrame.Value

            ' add the frame to the collection
            Me.Frames.Add(mFrame)

        Loop

    End Sub

    Function GetBitString(ByVal argBytes() As Byte) As String
        Dim s As String = ""
        For i As Integer = 0 To argBytes.Length - 1
            s &= BitManipulator.ExamineBit(argBytes(i), 0)
            s &= BitManipulator.ExamineBit(argBytes(i), 1)
            s &= BitManipulator.ExamineBit(argBytes(i), 2)
            s &= BitManipulator.ExamineBit(argBytes(i), 3)
            s &= BitManipulator.ExamineBit(argBytes(i), 4)
            s &= BitManipulator.ExamineBit(argBytes(i), 5)
            s &= BitManipulator.ExamineBit(argBytes(i), 6)
            s &= BitManipulator.ExamineBit(argBytes(i), 7)
            s &= BitManipulator.ExamineBit(argBytes(i), 8)
        Next
        Return s
    End Function

    Sub ParseHeaderV2()

        Dim mHeaderFlags(1) As Byte
        Dim mTagSize(3) As Byte

        ' read the 1 byte where the header flags are stored
        gTagReader.Read(mHeaderFlags, 0, 1)

        ' set the MP3 Header Flags
        If BitManipulator.ExamineBit(mHeaderFlags(0), 8) = True Then Me.HeaderFlags = "Unsynchronization"
        If BitManipulator.ExamineBit(mHeaderFlags(0), 7) = True Then Me.HeaderFlags &= ", " & "Extended Header"
        If BitManipulator.ExamineBit(mHeaderFlags(0), 6) = True Then Me.HeaderFlags &= ", " & "Experimental Indicator"

        ' 3) get the tag size from the tag header

        ' read the 4 bytes where the tag size is stored
        gTagReader.Read(mTagSize, 0, 4)

        ' set the MP3 Tag Size
        Me.TagSize = GetID3EncodedSizeV4(mTagSize)

        ' read the entire tag, minus the header
        'Dim mEntireTag(Me.TagSize - 1) As Byte
        'gTagReader.Read(mEntireTag, 0, Me.TagSize)

        '' increment by 1 char
        '' if char = 0 then end current find
        'Dim val As String = ""
        'For i As Integer = 0 To mEntireTag.Length - 1

        '    Dim mNextByte As Byte = mEntireTag(i)
        '    Dim mNextChar As String

        '    If mNextByte > 31 And mNextByte < 127 Then
        '        mNextChar = Chr(mNextByte)
        '        val &= mNextChar
        '    Else
        '        ' stop here and find the next valid char
        '    End If

        'Next

        'gTagString = val

    End Sub

    Sub ParseHeaderV4()

        Dim mHeaderFlags(1) As Byte
        Dim mTagSize(3) As Byte

        ' read the 1 byte where the header flags are stored
        gTagReader.Read(mHeaderFlags, 0, 1)

        ' set the MP3 Header Flags
        If BitManipulator.ExamineBit(mHeaderFlags(0), 8) = True Then Me.HeaderFlags = "Unsynchronization"
        If BitManipulator.ExamineBit(mHeaderFlags(0), 7) = True Then Me.HeaderFlags &= ", " & "Extended Header"
        If BitManipulator.ExamineBit(mHeaderFlags(0), 6) = True Then Me.HeaderFlags &= ", " & "Experimental Indicator"
        If BitManipulator.ExamineBit(mHeaderFlags(0), 5) = True Then Me.HeaderFlags &= ", " & "Footer"

        ' 3) get the tag size from the tag header

        ' read the 4 bytes where the tag size is stored
        gTagReader.Read(mTagSize, 0, 4)

        ' set the MP3 Tag Size
        Me.TagSize = GetID3EncodedSizeV4(mTagSize)

        '' read the entire tag, minus the header

        'Dim mEntireTag(Me.TagSize - 1) As Byte
        'gTagReader.Read(mEntireTag, 0, Me.TagSize)

        '' increment by 1 char
        '' if char = 0 then end current find
        'Dim val As String = ""
        'For i As Integer = 0 To mEntireTag.Length - 1

        '    Dim mNextByte As Byte = mEntireTag(i)
        '    Dim mNextChar As String

        '    If mNextByte > 31 And mNextByte < 127 Then
        '        mNextChar = Chr(mNextByte)
        '        val &= mNextChar
        '    Else
        '        ' stop here and find the next valid char
        '    End If

        'Next

        'gTagString = val

    End Sub

End Class