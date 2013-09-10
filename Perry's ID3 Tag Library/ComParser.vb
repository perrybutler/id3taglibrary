<ComClass(ComParser.ClassId, ComParser.InterfaceId, ComParser.EventsId)> _
Public Class ComParser

#Region "COM GUIDs"
    ' These  GUIDs provide the COM identity for this class 
    ' and its COM interfaces. If you change them, existing 
    ' clients will no longer be able to access the class.
    Public Const ClassId As String = "2a314524-a053-4dc4-87dc-ffe55d8efdc8"
    Public Const InterfaceId As String = "70c601e7-841d-4a50-b7e7-4e69665a2783"
    Public Const EventsId As String = "d969b307-6f11-47f1-97b2-9d540b069f4a"
#End Region

    ' A creatable COM class must have a Public Sub New() 
    ' with no parameters, otherwise, the class will not be 
    ' registered in the COM registry and cannot be created 
    ' via CreateObject.
    Public Sub New()
        MyBase.New()
    End Sub

    ''' <summary>
    ''' Parses an MP3 file whose result is accessible through COM.
    ''' </summary>
    ''' <param name="argFilePath"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ParseMP3(ByVal argFilePath As String) As MP3File
        Return New ComMP3File(argFilePath)
    End Function

    Public Class ComMP3File
        Inherits ID3TagLibrary.MP3File
        Sub New(ByVal argFilePath As String)
            MyBase.new(argFilePath)
        End Sub
    End Class

End Class


