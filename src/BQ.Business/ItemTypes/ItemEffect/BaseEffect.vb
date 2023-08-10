Friend Class BaseEffect
    Implements IEffect
    Private ReadOnly data As New BaseData
    Sub New(effectType As String, effectData As BaseData)
        Me.data.Flags = New HashSet(Of String)(effectData.Flags)
        Me.data.Metadatas = New Dictionary(Of String, String)(effectData.Metadatas)
        Me.data.Statistics = New Dictionary(Of String, Integer)(effectData.Statistics)
        Me.EffectType = effectType
    End Sub

    Public ReadOnly Property Statistic(statisticType As String) As Integer Implements IStatisticsHolder.Statistic
        Get
            Return data.Statistics(statisticType)
        End Get
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
            Return data.Metadatas(identifier)
        End Get
        Set(value As String)
            data.Metadatas(identifier) = identifier
        End Set
    End Property

    Public Property EffectType As String Implements IEffect.EffectType

    Public Sub RemoveStatistic(statisticType As String) Implements IStatisticsHolder.RemoveStatistic
        Data.Statistics.Remove(statisticType)
    End Sub

    Public Sub RemoveMetadata(identifier As String) Implements IMetadataHolder.RemoveMetadata
        data.Metadatas.Remove(identifier)
    End Sub

    Public Sub SetStatistic(statisticType As String, value As Integer) Implements IStatisticsHolder.SetStatistic
        data.Statistics(statisticType) = value
    End Sub

    Public Function HasStatistic(statisticType As String) As Boolean Implements IStatisticsHolder.HasStatistic
        Return data.Statistics.ContainsKey(statisticType)
    End Function

    Public Function HasMetadata(identifier As String) As Boolean Implements IMetadataHolder.HasMetadata
        Return data.Metadatas.ContainsKey(identifier)
    End Function

    Public Function TryGetStatistic(statisticType As String, Optional defaultValue As Integer = 0) As Integer Implements IStatisticsHolder.TryGetStatistic
        Return If(HasStatistic(statisticType), Statistic(statisticType), defaultValue)
    End Function

    Public Function AddStatistic(statisticType As String, delta As Integer) As Integer Implements IStatisticsHolder.AddStatistic
        SetStatistic(statisticType, Statistic(statisticType) + delta)
        Return Statistic(statisticType)
    End Function
End Class
