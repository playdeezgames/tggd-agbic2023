Friend Class BaseEffect
    Implements IEffect
    Private ReadOnly data As New BaseData
    Sub New(effectType As String, effectData As BaseData)
        Me.data.Flags = New HashSet(Of String)(effectData.Flags)
        Me.data.Metadata = New Dictionary(Of String, String)(effectData.Metadata)
        Me.data.Statistics = New Dictionary(Of String, Integer)(effectData.Statistics)
        Me.EffectType = effectType
    End Sub

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
            Return Data.Flags.Contains(flagType)
        End Get
        Set(value As Boolean)
            If value Then
                Data.Flags.Add(flagType)
            Else
                Data.Flags.Remove(flagType)
            End If
        End Set
    End Property

    Public Property Metadata(identifier As String) As String Implements IMetadataHolder.Metadata
        Get
            Return Data.Metadata(identifier)
        End Get
        Set(value As String)
            Data.Metadata(identifier) = identifier
        End Set
    End Property

    Public Property EffectType As String Implements IEffect.EffectType

    Public Sub RemoveStatistic(statisticType As String) Implements IStatisticsHolder.RemoveStatistic
        Data.Statistics.Remove(statisticType)
    End Sub

    Public Sub RemoveMetadata(identifier As String) Implements IMetadataHolder.RemoveMetadata
        Data.Metadata.Remove(identifier)
    End Sub

    Public Function HasStatistic(statisticType As String) As Boolean Implements IStatisticsHolder.HasStatistic
        Return Data.Statistics.ContainsKey(statisticType)
    End Function

    Public Function HasMetadata(identifier As String) As Boolean Implements IMetadataHolder.HasMetadata
        Return Data.Metadata.ContainsKey(identifier)
    End Function

    Public Function TryGetStatistic(statisticType As String, Optional defaultValue As Integer = 0) As Integer Implements IStatisticsHolder.TryGetStatistic
        Return If(HasStatistic(statisticType), Statistic(statisticType), defaultValue)
    End Function
End Class
