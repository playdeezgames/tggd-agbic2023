Imports BQ.Data

Friend Class TriggerDataClient
    Inherits MapDataClient
    Protected ReadOnly TriggerId As Integer
    Protected ReadOnly Property TriggerData As TriggerData
        Get
            Return MapData.Triggers(TriggerId)
        End Get
    End Property
    Sub New(worldData As WorldData, mapId As Integer, triggerId As Integer)
        MyBase.New(worldData, mapId)
        Me.TriggerId = triggerId
    End Sub
End Class
