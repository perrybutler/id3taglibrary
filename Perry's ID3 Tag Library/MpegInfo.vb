Public Class MpegInfo

    Public LocationIndex As Integer
    Public HeaderSize As Integer
    Public FileSize As Integer

    ' encoded values
    Public VersionEncoded As String
    Public LayerEncoded As String
    Public BitrateEncoded As String
    Public SamplingRateEncoded As String
    Public ChannelModeEncoded As String
    Public ModeExtensionEncoded As String
    Public EmphasisEncoded As String

    ' literal/computed values
    Public Bitrate As Integer
    Public Duration As String
    Public SamplingRate As String
    Public FrameSize As Integer = 1152 ' this is ALWAYS 1152 for MPEG-1 Layer 3 (mp3)
    Public FrameLength As Integer
    Public Layer As String
    Public Version As String
    Public Kbps As Integer
    Public ProtectionBit As Boolean
    Public PaddingBit As Boolean
    Public PrivateBit As Boolean
    Public ChannelMode As String
    Public Copyright As Boolean
    Public Original As Boolean
    Public Emphasis As String
    Public RawResults As String ' temporary - store raw parse results for diagnostic viewing in the Tag Viewer

    Sub New(ByVal argFileInfo As IO.FileInfo, ByVal argStartIndex As Integer)
        Dim mReader As IO.FileStream = argFileInfo.OpenRead
        Dim mFramesync(10) As Byte                  ' 11 bits for the Frame sync (all bits set)
        Dim mMpegAudioVersionID(1) As Byte          ' 2 bits for the MPEG Audio version ID
        Dim mLayerDescription(1) As Byte            ' 2 bits for the Layer description
        Dim mProtectionBit(0) As Byte               ' 1 bit for the Protection bit
        Dim mBitrateIndex(3) As Byte                ' 4 bits for the Bitrate index
        Dim mSamplingRateFrequencyIndex(1) As Byte  ' 2 bits for the Sampling rate frequency index
        Dim mPaddingBit(0) As Byte                  ' 1 bit for the Padding bit
        Dim mPrivateBit(0) As Byte                  ' 1 bit for the Private bit
        Dim mChannelMode(1) As Byte                 ' 2 bits for the Channel Mode
        Dim mModeExtension(1) As Byte               ' 2 bits for the Mode extension
        Dim mCopyright(0) As Byte                   ' 1 bit for the Copyright
        Dim mOriginal(0) As Byte                    ' 1 bit for the Original
        Dim mEmphasis(1) As Byte                    ' 2 bits for the Emphasis
        Me.FileSize = argFileInfo.Length
        ' start at the beginning of the file
        mReader.Position = 0
        ' set up the byte position and buffer size for reading in chunks of data
        Dim mStartIndex As Integer = argStartIndex
        Dim mEndIndex As Integer = argStartIndex + 20000
        Dim mBufferLength As Integer = 10000
        Dim mBufferOffset As Integer = 0
        Dim mSyncByte As Byte = 255
        Dim mTryHeaders As New ArrayList
        ' set the reader position just after the id3v2 tag
        mReader.Position = mStartIndex
        Do Until mReader.Position >= mEndIndex
            ' read in the next chunk
            Dim mBytes(mBufferLength - 1) As Byte
            mReader.Read(mBytes, 0, mBufferLength)
            Console.WriteLine("Reading next chunk of " & mBufferLength & " bytes...")
            Dim mTryIndex As Integer = 0
            Dim mTryIndexOffset As Integer = 0
            Do Until mTryIndexOffset >= mBufferLength
                ' locate the next sync byte for testing
                mTryIndex = Array.IndexOf(mBytes, mSyncByte, mTryIndexOffset + 1)
                If mTryIndex = -1 Then
                    mTryIndexOffset = mBufferLength
                Else
                    ' increment the offset position to the currently found sync byte
                    mTryIndexOffset = mTryIndex
                    Try
                        If mBytes(mTryIndexOffset + 1) >= 224 Then
                            ' in a try block because blockcopy will fail silently
                            Dim mTryHeader(3) As Byte
                            Buffer.BlockCopy(mBytes, mTryIndexOffset, mTryHeader, 0, 4)
                            If IsValidMpegHeaderBytes(mTryHeader) = "VALID" Then
                                ' TODO: create a strongly typed MpegFrame object here and store them in an array
                                '   so we have the entire mpeg structure
                                mTryHeaders.Add(mTryHeader)
                                ParseMpegHeader(mTryHeader)
                                Dim mFrameLength As Integer = Math.Ceiling((144 * (Me.Bitrate / Me.SamplingRate)) + Me.PaddingBit)
                                Dim mResults = "Detected MPEG header frame sync byte at byte index " & mStartIndex + mTryIndexOffset + mBufferOffset & " or array index " & mTryIndexOffset & " with a binary value of " & BytesToBinaryString(mTryHeader) & " or decoded/interpreted values of Bitrate: " & Me.Bitrate & " Sampling rate: " & Me.SamplingRate & " Frame length in bytes: " & mFrameLength & " Frame duration in milliseconds: " & (1152 / Me.SamplingRate) * 1000
                                Me.RawResults &= vbNewLine & mResults
                                'Console.WriteLine(mResults)
                            End If
                        End If
                    Catch ex As Exception
                        Console.WriteLine(ex.Message)
                    End Try
                End If
            Loop
            mBufferOffset += mBufferLength
        Loop
        '' visualize some data for debugging purposes
        'Dim b() As Byte
        'For Each b In mTryHeaders
        '   MsgBox(System.Text.Encoding.Default.GetString(b))
        'Next
        ' close the file stream
        mReader.Close()
        mReader.Dispose()
    End Sub

    Function IsValidMpegHeaderBytes(ByVal b() As Byte) As String
        Dim result As String = "VALID"
        ' convert the four bytes into a binary sequence, since we are reading data on a per-bit basis
        Dim s As String = BytesToBinaryString(b)
        ' validity the binary sequence 
        If s.Substring(11, 2) = "01" Then
            result = "INVALID A"
        End If
        If s.Substring(13, 2) = "00" Then
            result = "INVALID B"
        End If
        If s.Substring(16, 4) = "1111" Then
            result = "INVALID C"
        End If
        If s.Substring(20, 2) = "11" Then
            result = "INVALID D"
        End If
        If s.Substring(30, 2) = "10" Then
            result = "INVALID E"
        End If
        Return result
    End Function

    ' MPEG DURATION ALGORITHM:
    ' 1) convert bytes to kilobytes: 10971126 / 1024 = 10713.990 kilobytes
    ' 2) convert kilobytes to kilobits: 10713.990 * 8 = 85711.921 kilobits
    ' 3) divide kilobits by bitrate to get duration in seconds: / 320 = 267.849 seconds
    Sub ParseMpegHeader(ByVal argBytes() As Byte)
        ' convert the bytes into a binary sequence, since we are reading data on a per-bit basis
        Dim s As String = BytesToBinaryString(argBytes)
        ' get raw header info, some of these are encoded values and some are literal values
        Me.BitrateEncoded = s.Substring(16, 4)
        Me.SamplingRateEncoded = s.Substring(20, 2)
        Me.PaddingBit = s.Substring(22, 1)
        Me.PrivateBit = s.Substring(23, 1)
        Me.ChannelMode = s.Substring(24, 2)
        Me.ModeExtensionEncoded = s.Substring(26, 2)
        Me.Copyright = s.Substring(28, 1)
        Me.Original = s.Substring(29, 1)
        Me.EmphasisEncoded = s.Substring(30, 1)
        ' convert raw info
        ' Version
        Select Case Me.VersionEncoded
            Case "00"
                Me.Version = 2.5 ' MPEG Version 2.5
            Case "01"
                Me.Version = 0 ' (RESERVED - invalid mpeg header frame)
            Case "10"
                Me.Version = 2 ' MPEG Version 2 (ISO/IEC 13818-3)
            Case "11"
                Me.Version = 1 ' MPEG Version 1 (ISO/IEC 11172-3)
        End Select
        ' Layer
        Select Case Me.LayerEncoded
            Case "00"
                Me.Layer = 0 ' (RESERVED - invalid mpeg header frame)
            Case "01"
                Me.Layer = 3
            Case "10"
                Me.Layer = 2
            Case "11"
                Me.Layer = 1
        End Select
        ' Bitrate and Kbps
        Select Case Me.BitrateEncoded
            Case "0000"
                Me.Bitrate = 0 ' (FREE)
            Case "0001"
                Me.Bitrate = 32000
            Case "0010"
                Me.Bitrate = 40000
            Case "0011"
                Me.Bitrate = 48000
            Case "0100"
                Me.Bitrate = 56000
            Case "0101"
                Me.Bitrate = 64000
            Case "0110"
                Me.Bitrate = 80000
            Case "0111"
                Me.Bitrate = 96000
            Case "1000"
                Me.Bitrate = 112000
            Case "1001"
                Me.Bitrate = 128000
            Case "1010"
                Me.Bitrate = 160000
            Case "1011"
                Me.Bitrate = 192000
            Case "1100"
                Me.Bitrate = 224000
            Case "1101"
                Me.Bitrate = 256000
            Case "1110"
                Me.Bitrate = 320000
            Case "1111"
                Me.Bitrate = 0 ' (BAD - invalid mpeg header frame)
        End Select
        Me.Kbps = Me.Bitrate / 1000
        ' Sampling Rate
        Select Case Me.SamplingRateEncoded
            Case "00"
                Me.SamplingRate = 44100
            Case "01"
                Me.SamplingRate = 48000
            Case "10"
                Me.SamplingRate = 32000
            Case "11"
                Me.SamplingRate = 0 ' (RESERVED - invalid mpeg header frame)
        End Select
        ' Channel Mode
        Select Case Me.ChannelModeEncoded
            Case "00"
                Me.ChannelMode = "Stereo"
            Case "01"
                Me.ChannelMode = "Joint stereo (Stereo)"
            Case "10"
                Me.ChannelMode = "Dual channel (Stereo)"
            Case "11"
                Me.ChannelMode = "Single channel (Mono)"
        End Select
        ' Emphasis
        Select Case Me.EmphasisEncoded
            Case "00"
                Me.Emphasis = "none"
            Case "01"
                Me.Emphasis = "50/15 ms"
            Case "10"
                Me.Emphasis = 0 ' (RESERVED - invalid mpeg header frame)
            Case "11"
                Me.Emphasis = "CCIT J.17"
        End Select
        ' attempt to calculate the song duration using a variety of methods
        Me.Duration = ((Me.FileSize / 1024) * 8) / Me.Kbps
        Me.Duration = ((Me.FileSize / Me.Kbps) / 60)
        Me.Duration = Me.FileSize / (Me.Bitrate * 8)
    End Sub

End Class