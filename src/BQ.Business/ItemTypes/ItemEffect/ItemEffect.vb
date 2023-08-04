Friend Class ItemEffect
    Implements IItemEffect
    Private ReadOnly data As New BaseData
    Sub New(effectType As String, effectData As BaseData, item As IItem)
        Me.Item = item
        Me.data.Flags = New HashSet(Of String)(effectData.Flags)
        Me.data.Metadata = New Dictionary(Of String, String)(effectData.Metadata)
        Me.data.Statistics = New Dictionary(Of String, Integer)(effectData.Statistics)
        Me.EffectType = effectType
    End Sub

    Public ReadOnly Property Item As IItem Implements IItemEffect.Item

    Public Property Statistic(statisticType As String) As Integer Implements IStatisticsHolder.Statistic
        Get
            Return data.Statistics(statisticType)
        End Get
        Set(value As Integer)
            data.Statistics(statisticType) = value
        End Set
    End Property

    Public Property Flag(flagType As String) As Boolean Implements IFlagHolder.Flag
        Get
            Return data.Flags.Contains(flagType)
        End Get
        Set(value As Boolean)
            If value Then
                data.Flags.Add(flagType)
            Else
                data.Flags.Remove(flagType)
            End If
        End Set
    End Property

    Public Property Metadata(identifier As String) As String Implements IMetadataHolder.Metadata
        Get
            Return data.Metadata(identifier)
        End Get
        Set(value As String)
            data.Metadata(identifier) = identifier
        End Set
    End Property

    Public Property EffectType As String Implements IEffect.EffectType

    Public Sub RemoveStatistic(statisticType As String) Implements IStatisticsHolder.RemoveStatistic
        data.Statistics.Remove(statisticType)
    End Sub

    Public Sub RemoveMetadata(identifier As String) Implements IMetadataHolder.RemoveMetadata
        data.Metadata.Remove(identifier)
    End Sub

    Public Function HasStatistic(statisticType As String) As Boolean Implements IStatisticsHolder.HasStatistic
        Return data.Statistics.ContainsKey(statisticType)
    End Function

    Public Function HasMetadata(identifier As String) As Boolean Implements IMetadataHolder.HasMetadata
        Return data.Metadata.ContainsKey(identifier)
    End Function
End Class
