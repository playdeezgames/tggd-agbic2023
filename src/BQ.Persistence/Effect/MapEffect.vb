Imports BQ.Data

Friend Class MapEffect
    Inherits MapEffectDataClient
    Implements IEffect
    Public Sub New(worldData As WorldData, mapId As Integer, mapEffectId As Integer)
        MyBase.New(worldData, mapId, mapEffectId)
    End Sub
    Public ReadOnly Property Id As Integer Implements IEffect.Id
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

    Public Property Statistic(statisticType As String) As Integer Implements IEffect.Statistic
        Get
            Return EffectData.Statistics(statisticType)
        End Get
        Set(value As Integer)
            EffectData.Statistics(statisticType) = value
        End Set
    End Property

    Public Property Metadata(identifier As String) As String Implements IMetadataHolder.Metadata
        Get
            Return EffectData.Metadata(identifier)
        End Get
        Set(value As String)
            EffectData.Metadata(identifier) = value
        End Set
    End Property

    Public Property Flag(flagType As String) As Boolean Implements IFlagHolder.Flag
        Get
            Return EffectData.Flags.Contains(flagType)
        End Get
        Set(value As Boolean)
            If value Then
                EffectData.Flags.Add(flagType)
            Else
                EffectData.Flags.Remove(flagType)
            End If
        End Set
    End Property

    Public Sub RemoveStatistic(statisticType As String) Implements IStatisticsHolder.RemoveStatistic
        EffectData.Statistics.Remove(statisticType)
    End Sub

    Public Sub RemoveMetadata(identifier As String) Implements IMetadataHolder.RemoveMetadata
        EffectData.Metadata.Remove(identifier)
    End Sub

    Public Function HasStatistic(statisticType As String) As Boolean Implements IStatisticsHolder.HasStatistic
        Return EffectData.Statistics.ContainsKey(statisticType)
    End Function

    Public Function HasMetadata(identifier As String) As Boolean Implements IMetadataHolder.HasMetadata
        Return EffectData.Metadata.ContainsKey(identifier)
    End Function
End Class
