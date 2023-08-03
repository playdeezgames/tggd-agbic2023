Friend Class ItemEffect
    Implements IItemEffect
    Private ReadOnly effectData As New EffectData
    Sub New(effectData As EffectData, item As IItem)
        Me.Item = item
        Me.effectData.EffectType = effectData.EffectType
        Me.effectData.Flags = New HashSet(Of String)(effectData.Flags)
        Me.effectData.Metadata = New Dictionary(Of String, String)(effectData.Metadata)
        Me.effectData.Statistics = New Dictionary(Of String, Integer)(effectData.Statistics)
    End Sub

    Public ReadOnly Property Item As IItem Implements IItemEffect.Item

    Public Property EffectType As String Implements IEffect.EffectType
        Get
            Return effectData.EffectType
        End Get
        Set(value As String)
            effectData.EffectType = value
        End Set
    End Property

    Public Property Statistic(statisticType As String) As Integer Implements IStatisticsHolder.Statistic
        Get
            Return effectData.Statistics(statisticType)
        End Get
        Set(value As Integer)
            effectData.Statistics(statisticType) = value
        End Set
    End Property

    Public Property Flag(flagType As String) As Boolean Implements IFlagHolder.Flag
        Get
            Return effectData.Flags.Contains(flagType)
        End Get
        Set(value As Boolean)
            If value Then
                effectData.Flags.Add(flagType)
            Else
                effectData.Flags.Remove(flagType)
            End If
        End Set
    End Property

    Public Property Metadata(identifier As String) As String Implements IMetadataHolder.Metadata
        Get
            Return effectData.Metadata(identifier)
        End Get
        Set(value As String)
            effectData.Metadata(identifier) = identifier
        End Set
    End Property

    Public Sub RemoveStatistic(statisticType As String) Implements IStatisticsHolder.RemoveStatistic
        effectData.Statistics.Remove(statisticType)
    End Sub

    Public Sub RemoveMetadata(identifier As String) Implements IMetadataHolder.RemoveMetadata
        effectData.Metadata.Remove(identifier)
    End Sub

    Public Function HasStatistic(statisticType As String) As Boolean Implements IStatisticsHolder.HasStatistic
        Return effectData.Statistics.ContainsKey(statisticType)
    End Function

    Public Function HasMetadata(identifier As String) As Boolean Implements IMetadataHolder.HasMetadata
        Return effectData.Metadata.ContainsKey(identifier)
    End Function
End Class
