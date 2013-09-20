Public Class ID3v1Tag

    Public FileIdentifier As String
    Public Title As String
    Public Artist As String
    Public Album As String
    Public Year As String
    Public Comment As String
    Public Genre As String
    Public TagVersion As String

    Sub New(ByVal argFileInfo As IO.FileInfo)
        Dim mReader As IO.FileStream = argFileInfo.OpenRead
        Dim mFileIdentifier(2) As Byte
        Dim mTitle(29) As Byte
        Dim mArtist(29) As Byte
        Dim mAlbum(29) As Byte
        Dim mYear(3) As Byte
        Dim mComment(29) As Byte
        Dim mGenre(0) As Byte
        ' start at the last 128 bytes of the file, which is where an ID3v1 tag is supposed to be
        mReader.Position = mReader.Length - 128
        ' read the file identifier and make sure the value equals "TAG" which indicates an ID3v1 tag
        mReader.Read(mFileIdentifier, 0, 3)
        Me.FileIdentifier = BytesToText(mFileIdentifier, True).Trim
        ' read the rest of the tag if it is an ID3v1 tag
        If Me.FileIdentifier = "TAG" Then
            Me.TagVersion = "1.0"
            ' read the data
            mReader.Read(mTitle, 0, 30)
            mReader.Read(mArtist, 0, 30)
            mReader.Read(mAlbum, 0, 30)
            mReader.Read(mYear, 0, 4)
            mReader.Read(mComment, 0, 30)
            mReader.Read(mGenre, 0, 1)
            ' decode and store the data
            Me.Title = BytesToText(mTitle, True).Trim
            Me.Artist = BytesToText(mArtist, True).Trim
            Me.Album = BytesToText(mAlbum, True).Trim
            Me.Year = BytesToText(mYear, True).Trim
            Me.Comment = BytesToText(mComment, True).Trim
            'Me.Comment = RemoveChars(Me.Comment, New Integer() {0, 1, 32})
            Me.Genre = mGenre(0)
        Else
            'MsgBox("Could not find an ID3v1 tag!")
        End If
        ' close the file handle (remove the lock)
        mReader.Close()
        mReader.Dispose()
    End Sub

End Class