Public Class BitManipulator

    ' The ClearBit Sub clears the 1 based, nth bit
    ' (MyBit) of an integer (MyByte).
    Shared Sub ClearBit(ByRef MyByte, ByVal MyBit)
        Dim BitMask As Int16
        ' Create a bitmask with the 2 to the nth power bit set:
        BitMask = 2 ^ (MyBit - 1)
        ' Clear the nth Bit:
        MyByte = MyByte And Not BitMask
    End Sub

    ' The ExamineBit function will return True or False
    ' depending on the value of the 1 based, nth bit (MyBit)
    ' of an integer (MyByte).
    Shared Function ExamineBit(ByVal MyByte, ByVal MyBit) As Boolean
        Dim BitMask As Int16
        BitMask = 2 ^ (MyBit - 1)
        ExamineBit = ((MyByte And BitMask) > 0)
    End Function

    ' The SetBit Sub will set the 1 based, nth bit
    ' (MyBit) of an integer (MyByte).
    Shared Sub SetBit(ByRef MyByte, ByVal MyBit)
        Dim BitMask As Int16
        BitMask = 2 ^ (MyBit - 1)
        MyByte = MyByte Or BitMask
    End Sub

    ' The ToggleBit Sub will change the state
    ' of the 1 based, nth bit (MyBit)
    ' of an integer (MyByte).
    Shared Sub ToggleBit(ByRef MyByte, ByVal MyBit)
        Dim BitMask As Int16
        BitMask = 2 ^ (MyBit - 1)
        MyByte = MyByte Xor BitMask
    End Sub

End Class
