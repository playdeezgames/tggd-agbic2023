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

    Public ReadOnly Property TriggerType As String Implements IMessageChoice.TriggerType
        Get
            Return MessageChoiceData.TriggerType
        End Get
    End Property

    Public ReadOnly Property Id As Integer Implements ITrigger.Id
        Get
            Return ChoiceId
        End Get
    End Property

    Public Property Metadata(identifier As String) As String Implements IMetadataHolder.Metadata
        Get
            Return MessageChoiceData.Metadatas(identifier)
        End Get
        Set(value As String)
            MessageChoiceData.Metadatas(identifier) = value
        End Set
    End Property

    Public Property Statistic(statisticType As String) As Integer Implements IStatisticsHolder.Statistic
        Get
            Return MessageChoiceData.Statistics(statisticType)
        End Get
        Set(value As Integer)
            MessageChoiceData.Statistics(statisticType) = value
        End Set
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
        MessageChoiceData.Metadata.Remove(identifier)
    End Sub

    Public Function SetTriggerType(triggerType As String) As ITrigger Implements ITrigger.SetTriggerType
        MessageChoiceData.TriggerType = triggerType
        Return Me
    End Function

    Public Function SetStatistic(statisticType As String, value As Integer) As ITrigger Implements ITrigger.SetStatistic
        MessageChoiceData.Statistics(statisticType) = value
        Return Me
    End Function

    Public Function SetMetadata(identifier As String, value As String) As ITrigger Implements ITrigger.SetMetadata
        MessageChoiceData.Metadatas(identifier) = value
        Return Me
    End Function

    Public Function HasStatistic(statisticType As String) As Boolean Implements IStatisticsHolder.HasStatistic
        Return MessageChoiceData.Statistics.ContainsKey(statisticType)
    End Function

    Public Function HasMetadata(identifier As String) As Boolean Implements IMetadataHolder.HasMetadata
        Return MessageChoiceData.Metadata.ContainsKey(identifier)
    End Function
End Class
