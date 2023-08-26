Friend Class BaseEffect
    Implements IEffect
    Private ReadOnly data As New BaseData
    Sub New(effectType As String, effectData As BaseData)
        Me.data.Flags = New HashSet(Of String)(effectData.Flags)
        Me.data.Metadatas = New Dictionary(Of String, String)(effectData.Metadatas)
        Me.data.Statistics = New Dictionary(Of String, Integer)(effectData.Statistics)
        Me.EffectType = effectType
    End Sub

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

    Public Sub SetFlag(flagType As String, value As Boolean) Implements IFlagHolder.SetFlag
        If value Then
            data.Flags.Add(flagType)
        Else
            data.Flags.Remove(flagType)
        End If
    End Sub

    Public Sub SetMetadata(identifier As String, value As String) Implements IMetadataHolder.SetMetadata
        data.Metadatas(identifier) = value
    End Sub

    Public Function HasStatistic(statisticType As String) As Boolean Implements IStatisticsHolder.HasStatistic
        Return data.Statistics.ContainsKey(statisticType)
    End Function

    Public Function HasMetadata(identifier As String) As Boolean Implements IMetadataHolder.HasMetadata
        Return data.Metadatas.ContainsKey(identifier)
    End Function

    Public Function GetStatistic(statisticType As String, Optional defaultValue As Integer = 0) As Integer Implements IStatisticsHolder.GetStatistic
        Return If(HasStatistic(statisticType), data.Statistics(statisticType), defaultValue)
    End Function

    Public Function AddStatistic(statisticType As String, delta As Integer) As Integer Implements IStatisticsHolder.AddStatistic
        SetStatistic(statisticType, GetStatistic(statisticType) + delta)
        Return GetStatistic(statisticType)
    End Function

    Public Function GetFlag(flagType As String) As Boolean Implements IFlagHolder.GetFlag
        Return data.Flags.Contains(flagType)
    End Function

    Public Function GetMetadata(identifier As String) As String Implements IMetadataHolder.GetMetadata
        Return data.Metadatas(identifier)
    End Function
End Class
