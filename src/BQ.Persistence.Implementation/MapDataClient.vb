Imports BQ.Data

Friend Class MapDataClient
    Inherits WorldDataClient
    Protected ReadOnly MapId As Integer
    Protected ReadOnly Property MapData As MapData
        Get
            Return WorldData.Maps(MapId)
        End Get
    End Property
    Public Sub New(worldData As Data.WorldData, mapId As Integer)
        MyBase.New(worldData)
        Me.MapId = mapId
    End Sub
End Class
