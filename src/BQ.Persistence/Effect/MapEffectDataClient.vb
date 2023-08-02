Imports BQ.Data

Friend Class MapEffectDataClient
    Inherits MapDataClient
    Protected ReadOnly MapEffectId As Integer
    Protected ReadOnly Property EffectData As EffectData
        Get
            Return MapData.Effects(MapEffectId)
        End Get
    End Property
    Sub New(worldData As WorldData, mapId As Integer, mapEffectId As Integer)
        MyBase.New(worldData, mapId)
        Me.MapEffectId = mapEffectId
    End Sub
End Class
