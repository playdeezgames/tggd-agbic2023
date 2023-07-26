﻿Imports BQ.Data

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

    Public Property Statistic(statisticType As String) As Integer Implements ITrigger.Statistic
        Get
            Return TriggerData.Statistics(statisticType)
        End Get
        Set(value As Integer)
            TriggerData.Statistics(statisticType) = value
        End Set
    End Property

    Public ReadOnly Property Metadata(identifier As String) As String Implements ITrigger.Metadata
        Get
            Return TriggerData.Metadata(identifier)
        End Get
    End Property

    Public Sub RemoveStatistic(statisticType As String) Implements IStatisticsHolder.RemoveStatistic
        TriggerData.Statistics.Remove(statisticType)
    End Sub

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

    Public Function HasStatistic(statisticType As String) As Boolean Implements IStatisticsHolder.HasStatistic
        Return TriggerData.Statistics.ContainsKey(statisticType)
    End Function
End Class
