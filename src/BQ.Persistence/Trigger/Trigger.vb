Imports BQ.Data

Friend Class Trigger
    Inherits TriggerDataClient
    Implements ITrigger
    Public Sub New(worldData As WorldData, mapId As Integer, triggerId As Integer)
        MyBase.New(worldData, mapId, triggerId)
    End Sub
    Public ReadOnly Property Id As Integer Implements ITrigger.Id
        Get
            Return TriggerId
        End Get
    End Property

    Public ReadOnly Property TriggerType As String Implements ITrigger.TriggerType
        Get
            Return TriggerData.TriggerType
        End Get
    End Property

    Public ReadOnly Property Statistics(statisticType As String) As Integer Implements ITrigger.Statistics
        Get
            Return TriggerData.Statistics(statisticType)
        End Get
    End Property

    Public ReadOnly Property Metadata(identifier As String) As String Implements ITrigger.Metadata
        Get
            Return TriggerData.Metadata(identifier)
        End Get
    End Property

    Public Function SetTriggerType(triggerType As String) As ITrigger Implements ITrigger.SetTriggerType
        TriggerData.TriggerType = triggerType
        Return Me
    End Function

    Public Function SetStatistic(statisticType As String, value As Integer) As ITrigger Implements ITrigger.SetStatistic
        TriggerData.Statistics(statisticType) = value
        Return Me
    End Function

    Public Function SetMetadata(identifier As String, value As String) As ITrigger Implements ITrigger.SetMetadata
        TriggerData.Metadata(identifier) = value
        Return Me
    End Function
End Class
