Imports BQ.Data

Friend Class MessageChoice
    Inherits MessageChoiceDataClient
    Implements IMessageChoice

    Public Sub New(worldData As Data.WorldData, messageId As Integer, choiceId As Integer)
        MyBase.New(worldData, messageId, choiceId)
    End Sub

    Public ReadOnly Property Text As String Implements IMessageChoice.Text
        Get
            Return MessageChoiceData.Text
        End Get
    End Property

    Public Property EffectType As String Implements IMessageChoice.EffectType
        Get
            Return MessageChoiceData.EffectType
        End Get
        Set(value As String)
            MessageChoiceData.EffectType = value
        End Set
    End Property

    Public Property Metadata(identifier As String) As String Implements IMetadataHolder.Metadata
        Get
            Return MessageChoiceData.Metadatas(identifier)
        End Get
        Set(value As String)
            MessageChoiceData.Metadatas(identifier) = value
        End Set
    End Property

    Public ReadOnly Property Statistic(statisticType As String) As Integer Implements IStatisticsHolder.Statistic
        Get
            Return MessageChoiceData.Statistics(statisticType)
        End Get
    End Property

    Public Property Flag(flagType As String) As Boolean Implements IFlagHolder.Flag
        Get
            Return MessageChoiceData.Flags.Contains(flagType)
        End Get
        Set(value As Boolean)
            If value Then
                MessageChoiceData.Flags.Add(flagType)
            Else
                MessageChoiceData.Flags.Remove(flagType)
            End If
        End Set
    End Property

    Public Sub RemoveStatistic(statisticType As String) Implements IStatisticsHolder.RemoveStatistic
        MessageChoiceData.Statistics.Remove(statisticType)
    End Sub

    Public Sub RemoveMetadata(identifier As String) Implements IMetadataHolder.RemoveMetadata
        MessageChoiceData.Metadatas.Remove(identifier)
    End Sub

    Public Sub SetStatistic(statisticType As String, value As Integer) Implements IStatisticsHolder.SetStatistic
        MessageChoiceData.Statistics(statisticType) = value
    End Sub

    Public Sub SetFlag(flagType As String, value As Boolean) Implements IFlagHolder.SetFlag
        If value Then
            MessageChoiceData.Flags.Add(flagType)
        Else
            MessageChoiceData.Flags.Remove(flagType)
        End If
    End Sub

    Public Function HasStatistic(statisticType As String) As Boolean Implements IStatisticsHolder.HasStatistic
        Return MessageChoiceData.Statistics.ContainsKey(statisticType)
    End Function

    Public Function HasMetadata(identifier As String) As Boolean Implements IMetadataHolder.HasMetadata
        Return MessageChoiceData.Metadatas.ContainsKey(identifier)
    End Function

    Public Function TryGetStatistic(statisticType As String, Optional defaultValue As Integer = 0) As Integer Implements IStatisticsHolder.TryGetStatistic
        Return If(HasStatistic(statisticType), Statistic(statisticType), defaultValue)
    End Function

    Public Function AddStatistic(statisticType As String, delta As Integer) As Integer Implements IStatisticsHolder.AddStatistic
        SetStatistic(statisticType, Statistic(statisticType) + delta)
        Return Statistic(statisticType)
    End Function

    Public Function GetFlag(flagType As String) As Boolean Implements IFlagHolder.GetFlag
        Return MessageChoiceData.Flags.Contains(flagType)
    End Function
End Class
