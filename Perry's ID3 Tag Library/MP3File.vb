Public Class MP3File

    Public FileInfo As IO.FileInfo
    Public MpegInfo As MpegInfo
    Public Tag1 As ID3v1Tag
    Public Tag2 As ID3v2Tag

    ''' <summary>
    ''' Retrieves the ID3 tag version.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property TagVersion() As String
        Get
            Dim val As String = ""
            If Me.Tag1.TagVersion <> "" Then val = Me.Tag1.TagVersion
            If Me.Tag2.TagVersion <> "" Then val = Me.Tag2.TagVersion
            Return val
        End Get
        Set(ByVal value As String)

        End Set
    End Property

    ''' <summary>
    ''' Retrieves the most accurate Artist associated with this MP3 file (preferring ID3v2 info over ID3v1 info).
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property Artist() As String
        Get
            Dim val As String = ""
            If Me.Tag1.Artist <> "" Then val = Me.Tag1.Artist
            If Me.Tag2.Artist <> "" Then val = Me.Tag2.Artist
            Return val
        End Get
        Set(ByVal value As String)

        End Set
    End Property

    ''' <summary>
    ''' Retrieves the most accurate Album title associated with this MP3 file (preferring ID3v2 info over ID3v1 info).
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property Album() As String
        Get
            Dim val As String = ""
            If Me.Tag1.Album <> "" Then val = Me.Tag1.Album
            If Me.Tag2.Album <> "" Then val = Me.Tag2.Album
            Return val
        End Get
        Set(ByVal value As String)

        End Set
    End Property

    ''' <summary>
    ''' Retrieves the most accurate track Title associated with this MP3 file (preferring ID3v2 info over ID3v1 info).
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property Title() As String
        Get
            Dim val As String = ""
            If Me.Tag1.Title <> "" Then val = Me.Tag1.Title
            If Me.Tag2.Title <> "" Then val = Me.Tag2.Title
            Return val
        End Get
        Set(ByVal value As String)

        End Set
    End Property

    ''' <summary>
    ''' Retrieves the most accurate Year associated with this MP3 file (preferring ID3v2 info over ID3v1 info).
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property Year() As String
        Get
            Dim val As String = ""
            If Me.Tag1.Year <> "" Then val = Me.Tag1.Year
            If Me.Tag2.Year <> "" Then val = Me.Tag2.Year
            Return val
        End Get
        Set(ByVal value As String)

        End Set
    End Property

    ''' <summary>
    ''' Retrieves the most accurate Comment associated with this MP3 file (preferring ID3v2 info over ID3v1 info).
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property Comment() As String
        Get
            Dim val As String = ""
            If Me.Tag1.Comment <> "" Then val = Me.Tag1.Comment
            If Me.Tag2.Comment <> "" Then val = Me.Tag2.Comment
            Return val
        End Get
        Set(ByVal value As String)

        End Set
    End Property

    ''' <summary>
    ''' Retrieves the most accurate Genre associated with this MP3 file (preferring ID3v2 info over ID3v1 info).
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property Genre() As String
        Get
            Dim val As String = ""
            If Me.Tag1.Genre <> "" Then val = Me.Tag1.Genre
            If Me.Tag2.Genre <> "" Then val = Me.Tag2.Genre
            Return val
        End Get
        Set(ByVal value As String)

        End Set
    End Property

    ''' <summary>
    ''' When instantiating a new instance of this class, the MP3 file is parsed immediately and the Tag1/Tag2 properties are populated.
    ''' </summary>
    ''' <param name="argFilePath">Path to the MP3 file we wish to parse.</param>
    ''' <remarks></remarks>
    Sub New(ByVal argFilePath As String)
        Me.FileInfo = New IO.FileInfo(argFilePath)
        Me.Tag1 = New ID3v1Tag(Me.FileInfo)
        Me.Tag2 = New ID3v2Tag(Me.FileInfo)
        Me.MpegInfo = New MpegInfo(Me.FileInfo, Me.Tag2.TagSize)
    End Sub

End Class