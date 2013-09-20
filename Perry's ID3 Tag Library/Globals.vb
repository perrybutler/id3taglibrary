Public Module Globals

    ''' <summary>
    ''' Gets the encoded size in bytes of an ID3v2.2 tag.
    ''' </summary>
    ''' <param name="argBytes"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetID3EncodedSizeV2(ByVal argBytes() As Byte) As Integer
        Dim encodedSize As Integer
        'encodedSize = argBytes(0) + argBytes(1) + argBytes(2)
        encodedSize = (argBytes(0) * 256 * 256) + (argBytes(1) * 256) + argBytes(2)
        Return encodedSize
    End Function

    ''' <summary>
    ''' Gets the encoded size in bytes of an ID3v2.3 tag.
    ''' </summary>
    ''' <param name="argBytes"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetID3EncodedSizeV3(ByVal argBytes() As Byte) As Integer
        Dim encodedSize As Integer
        'encodedSize = (65536 * (argBytes(0) * 256 + argBytes(1))) + (argBytes(2) * 256 + argBytes(3))
        'encodedSize = (argBytes(0) * 128 * 128 * 128) + (argBytes(1) * 128 * 128) + (argBytes(2) * 128) + argBytes(3)
        encodedSize = (argBytes(0) * 256 * 256 * 256) + (argBytes(1) * 256 * 256) + (argBytes(2) * 256) + argBytes(3)
        Return encodedSize
    End Function

    ''' <summary>
    ''' Gets the encoded size in bytes of the ID3v2.4 tag.
    ''' </summary>
    ''' <param name="argBytes"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetID3EncodedSizeV4(ByVal argBytes() As Byte) As String

        ' According to ID3v2 spec, the encoded size is contained by 4 bytes. Each
        ' byte has it's MSB set to zero. The NORMAL CHART and UNSYNCHED CHART 
        ' show this difference.

        ' NORMAL CHART (last 2 bytes)
        ' 16    15    14    13    12    11    10    9    |  8    7    6    5    4    3    2    1
        ' 32768 16384 8192  4096  2048  1024  512   256  |  128  64   32   16   8    4    2    1

        ' UNSYNCHED CHART (last 2 bytes)
        ' 16    15    14    13    12    11    10    9    |  8    7    6    5    4    3    2    1
        ' 0     8192  4096  2048  1024  512   256   128  |  0    64   32   16   8    4    2    1

        'Dim encodedSize As Integer

        '' check the value of each bit in the fourth byte and increment the encoded size
        'If BitManipulator.ExamineBit(argBytes(3), 1) = True Then encodedSize += 1
        'If BitManipulator.ExamineBit(argBytes(3), 2) = True Then encodedSize += 2
        'If BitManipulator.ExamineBit(argBytes(3), 3) = True Then encodedSize += 4
        'If BitManipulator.ExamineBit(argBytes(3), 4) = True Then encodedSize += 8
        'If BitManipulator.ExamineBit(argBytes(3), 5) = True Then encodedSize += 16
        'If BitManipulator.ExamineBit(argBytes(3), 6) = True Then encodedSize += 32
        'If BitManipulator.ExamineBit(argBytes(3), 7) = True Then encodedSize += 64

        '' check the value of each bit in the third byte and increment the encoded size
        'If BitManipulator.ExamineBit(argBytes(2), 1) = True Then encodedSize += 128
        'If BitManipulator.ExamineBit(argBytes(2), 2) = True Then encodedSize += 256
        'If BitManipulator.ExamineBit(argBytes(2), 3) = True Then encodedSize += 512
        'If BitManipulator.ExamineBit(argBytes(2), 4) = True Then encodedSize += 1024
        'If BitManipulator.ExamineBit(argBytes(2), 5) = True Then encodedSize += 2048
        'If BitManipulator.ExamineBit(argBytes(2), 6) = True Then encodedSize += 4096
        'If BitManipulator.ExamineBit(argBytes(2), 7) = True Then encodedSize += 8192

        '' check the value of each bit in the second byte and increment the encoded size
        'If BitManipulator.ExamineBit(argBytes(1), 1) = True Then encodedSize += 16384
        'If BitManipulator.ExamineBit(argBytes(1), 2) = True Then encodedSize += 32768
        'If BitManipulator.ExamineBit(argBytes(1), 3) = True Then encodedSize += 65536
        'If BitManipulator.ExamineBit(argBytes(1), 4) = True Then encodedSize += 131072
        'If BitManipulator.ExamineBit(argBytes(1), 5) = True Then encodedSize += 262144
        'If BitManipulator.ExamineBit(argBytes(1), 6) = True Then encodedSize += 524288
        'If BitManipulator.ExamineBit(argBytes(1), 7) = True Then encodedSize += 1048576

        '' check the value of each bit in the first byte and increment the encoded size
        'If BitManipulator.ExamineBit(argBytes(0), 1) = True Then encodedSize += 2097152
        'If BitManipulator.ExamineBit(argBytes(0), 2) = True Then encodedSize += 4194304
        'If BitManipulator.ExamineBit(argBytes(0), 3) = True Then encodedSize += 8388608
        'If BitManipulator.ExamineBit(argBytes(0), 4) = True Then encodedSize += 16777216
        'If BitManipulator.ExamineBit(argBytes(0), 5) = True Then encodedSize += 33554432
        'If BitManipulator.ExamineBit(argBytes(0), 6) = True Then encodedSize += 67108864
        'If BitManipulator.ExamineBit(argBytes(0), 7) = True Then encodedSize += 134217728

        '' subtract the tag header size (which is always 10 bytes according to ID3v2 spec) from the encodedSize to get the final tag size in bytes
        'Return encodedSize - 10 '50934

        Dim encodedSize As Integer
        'encodedSize = (argBytes(0) * 256 * 256 * 256) + (argBytes(1) * 256 * 256) + (argBytes(2) * 256) + argBytes(3)
        encodedSize = (argBytes(0) * 128 * 128 * 128) + (argBytes(1) * 128 * 128) + (argBytes(2) * 128) + argBytes(3)
        Return encodedSize

    End Function

    ''' <summary>
    ''' Converts an array of Bytes to a String, optionally removing non-alphanumeric characters.
    ''' </summary>
    ''' <param name="argBytes"></param>
    ''' <param name="argRemoveNonAlphanumeric"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function BytesToText(ByVal argBytes() As Byte, ByVal argRemoveNonAlphanumeric As Boolean) As String
        Dim mText As String = ""

        ' new method
        Dim lst As New ArrayList(argBytes)
        Dim x() As Byte
        x = Array.FindAll(argBytes, AddressOf isAlphaNumeric)
        mText = Text.Encoding.ASCII.GetString(x)

        ' old method
        'For Each b As Byte In argBytes
        '    Dim mCharacter As String
        '    If argRemoveNonAlphanumeric = True Then
        '        Select Case b
        '            Case Is < 32
        '            Case Else
        '                mCharacter = Convert.ToChar(b)
        '                mText &= mCharacter
        '        End Select
        '    Else
        '        mCharacter = Convert.ToChar(b)
        '        mText &= mCharacter
        '    End If
        'Next

        Return mText
    End Function

    ''' <summary>
    ''' Checks whether a byte is an alphanumeric character in the ASCII table.
    ''' </summary>
    ''' <param name="b"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function isAlphaNumeric(ByVal b As Byte) As Boolean
        If b > 31 And b < 128 Then
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' Converts a Byte array to a String of comma-delimited numeric ASCII codes.
    ''' </summary>
    ''' <param name="argBytes"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function BytesToAsciiCodes(ByVal argBytes() As Byte) As String
        Dim mText As String = ""
        For Each b As Byte In argBytes
            If mText = "" Then
                mText = b
            Else
                mText &= ", " & b
            End If
        Next
        Return mText
    End Function

    ''' <summary>
    ''' Removes one or more characters from a string.
    ''' </summary>
    ''' <param name="argString">The string that should have one or more characters removed.</param>
    ''' <param name="argRemove">An array of integer ASCII codes. Hint: New Integer() {1, 5, 23}</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function RemoveChars(ByVal argString As String, ByVal argRemove() As Integer)
        For i As Integer = 0 To argRemove.Length - 1
            argString = argString.Replace(Chr(argRemove(i)), "")
        Next
        Return argString
    End Function

    Function BytesToBinaryString(ByVal argBytes() As Byte) As String
        Dim s As String = ""
        For Each b As Byte In argBytes
            s &= Convert.ToString(Convert.ToInt32(b), 2).PadLeft(8, "0")
        Next
        Return s
    End Function

End Module

