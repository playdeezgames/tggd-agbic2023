﻿Imports BQ.Data

Friend Class MapEffect
    Inherits MapEffectDataClient
    Implements IMapEffect
    Public Sub New(worldData As WorldData, mapId As Integer, mapEffectId As Integer)
        MyBase.New(worldData, mapId, mapEffectId)
    End Sub
    Public ReadOnly Property Id As Integer Implements IMapEffect.Id
        Get
            Return MapEffectId
        End Get
    End Property

    Public Property EffectType As String Implements IEffect.EffectType
        Get
            Return EffectData.EffectType
        End Get
        Set(value As String)
            EffectData.EffectType = value
        End Set
    End Property

    Public Sub RemoveStatistic(statisticType As String) Implements IStatisticsHolder.RemoveStatistic
        EffectData.Statistics.Remove(statisticType)
    End Sub

    Public Sub RemoveMetadata(identifier As String) Implements IMetadataHolder.RemoveMetadata
        EffectData.Metadatas.Remove(identifier)
    End Sub

    Public Sub SetStatistic(statisticType As String, value As Integer) Implements IStatisticsHolder.SetStatistic
        EffectData.Statistics(statisticType) = value
    End Sub

    Public Sub SetFlag(flagType As String, value As Boolean) Implements IFlagHolder.SetFlag
        If value Then
            EffectData.Flags.Add(flagType)
        Else
            EffectData.Flags.Remove(flagType)
        End If
    End Sub

    Public Sub SetMetadata(identifier As String, value As String) Implements IMetadataHolder.SetMetadata
        EffectData.Metadatas(identifier) = value
    End Sub

    Public Function HasStatistic(statisticType As String) As Boolean Implements IStatisticsHolder.HasStatistic
        Return EffectData.Statistics.ContainsKey(statisticType)
    End Function

    Public Function HasMetadata(identifier As String) As Boolean Implements IMetadataHolder.HasMetadata
        Return EffectData.Metadatas.ContainsKey(identifier)
    End Function

    Public Function GetStatistic(statisticType As String, Optional defaultValue As Integer = 0) As Integer Implements IStatisticsHolder.GetStatistic
        Return If(HasStatistic(statisticType), EffectData.Statistics(statisticType), defaultValue)
    End Function

    Public Function AddStatistic(statisticType As String, delta As Integer) As Integer Implements IStatisticsHolder.AddStatistic
        SetStatistic(statisticType, GetStatistic(statisticType) + delta)
        Return GetStatistic(statisticType)
    End Function

    Public Function GetFlag(flagType As String) As Boolean Implements IFlagHolder.GetFlag
        Return EffectData.Flags.Contains(flagType)
    End Function

    Public Function GetMetadata(identifier As String) As String Implements IMetadataHolder.GetMetadata
        Return EffectData.Metadatas(identifier)
    End Function
End Class
