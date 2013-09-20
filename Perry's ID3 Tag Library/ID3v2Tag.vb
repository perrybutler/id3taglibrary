Public Class ID3v2Tag

    Public gTagReader As IO.FileStream
    Public gTagString As String

    ' TODO: implement this class, we do parse the values but we don't currently store them in this structure...
    Class HeaderFlagsClass
        Public UnsynchronizationFlag As Boolean
        Public ExtendedHeaderFlag As Boolean
        Public ExperimentalIndicatorFlag As Boolean
    End Class

    ' TODO: implement this class, we do parse the values but we don't currently store them in this structure...
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

    ' TODO: use public properties instead of public variables in case we need
    '   some pre-processing down the road...
    ' define public interfaces for tag header data
    Public FileIdentifier As String
    Public TagVersion As String
    Public TagSize As String
    Public HeaderFlags As String
    Public Frames As New ArrayList
    Public RawAscii As String
    Public RawBytes As String

    Public Function GetFrame(ByVal argFrameIndex As Integer) As FrameBase
        Return Me.Frames(argFrameIndex)
    End Function

    ''' <summary>
    ''' Returns the first frame found in the tag based on the given argFrameType (e.g. APIC or PIC)
    ''' </summary>
    ''' <param name="argFrameType"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetFrame(ByVal argFrameType As String) As FrameBase
        For Each mFrame As FrameBase In Me.Frames
            If argFrameType.Contains(mFrame.ID) Then
                Return mFrame
            End If
        Next
        Return Nothing
    End Function

    Public Function GetFrames(ByVal argFrameTypes As String) As ArrayList
        Dim mFrames As New ArrayList
        Dim mTypes() As String
        mTypes = argFrameTypes.Split(",")
        For Each mFrame As FrameBase In Me.Frames
            If mTypes.Contains(mFrame.ID) Then
                mFrames.Add(mFrame)
            End If
        Next
        Return mFrames
    End Function


    ' TODO: couldn't we define an object structure and then somehow agnostically de-serialize 
    '   the APIC data into this structure?
    ' TODO:
    '   -read/store the text encoding (1 byte)
    '   -read/store the mime type (variable length, terminated with 0)
    '   -read/store the picture type (1 byte)
    '   -read/store the description (variable length, terminated with 0)
    '   -remaining data is image data
    '   -currently, a fixed APIC header size of 14 is always assumed, this may have problems 
    '   for ID3v2.2 tags which use different header sizes and 3-character frame types (PIC)
    ''' <summary>
    ''' Returns a System.Drawing.Image by decoding the given argFrame's value.
    ''' </summary>
    ''' <param name="argFrame"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetArtworkImage(ByVal argFrame) As Drawing.Image

        Dim img As System.Drawing.Image = Nothing

        Dim mApicTextEncoding As Byte
        Dim mApicMimeType As New ArrayList
        Dim mApicPictureType As Byte
        Dim mApicDescription As String = ""
        Dim mApicDataStart As Integer = 7 '14

        Dim c As String = ""
        Dim i As Integer = 0

        mApicTextEncoding = argFrame.ValueBytes(i)
        i += 1

        c = ""
        Do Until c = "0"
            c = argFrame.ValueBytes(i).ToString
            mApicMimeType.Add(c)
            i = i + 1
        Loop

        mApicPictureType = argFrame.ValueBytes(i).ToString
        i += 1

        c = ""
        Do Until c = "0"
            c = argFrame.ValueBytes(i).ToString
            mApicDescription &= c.ToString
            i = i + 1
        Loop

        mApicDataStart = i

        Dim ms As IO.MemoryStream
        ms = New IO.MemoryStream(argFrame.ValueBytes, mApicDataStart, argFrame.ValueBytes.Length - mApicDataStart)

        img = Drawing.Image.FromStream(ms)
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
            Dim mSizeV2 As Integer = GetID3EncodedSizeV2(mFrameSizeBytes)
            mFrame.Size = mSizeV2
            ' read the Frame Data (mFrame.Size - 10 bytes)
            ReDim mFrame.ValueBytes(mFrame.Size)
            gTagReader.Read(mFrame.ValueBytes, 0, mFrame.Size)
            ' visualize the value for debugging purposes
            Dim mVal As String = mFrame.Value
            ' add the frame to the collection
            Me.Frames.Add(mFrame)
        Loop

    End Sub

    Sub ParseFramesV3()
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
            ' parse the frame size
            Dim mSizeV3 As Integer = GetID3EncodedSizeV3(mFrameSizeBytes)
            mFrame.Size = mSizeV3
            ' visualize the flags for debugging purposes
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
            ' parse the frame size
            Dim mSizeV2 As Integer = GetID3EncodedSizeV2(mFrameSizeBytes) '+ 10
            Dim mSizeV3 As Integer = GetID3EncodedSizeV3(mFrameSizeBytes)
            Dim mSizeV4 As Integer = GetID3EncodedSizeV4(mFrameSizeBytes) '+ 10
            mFrame.Size = mSizeV4
            ' visualize the flags for debugging purposes
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
        ' read the 4 bytes where the tag size is stored
        gTagReader.Read(mTagSize, 0, 4)
        ' set the MP3 Tag Size
        Me.TagSize = GetID3EncodedSizeV4(mTagSize)
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
        ' read the 4 bytes where the tag size is stored
        gTagReader.Read(mTagSize, 0, 4)
        ' set the MP3 Tag Size
        Me.TagSize = GetID3EncodedSizeV4(mTagSize)
    End Sub

End Class