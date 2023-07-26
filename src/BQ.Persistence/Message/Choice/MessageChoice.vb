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

    Public ReadOnly Property Metadata(identifier As String) As String Implements ITrigger.Metadata
        Get
            Return MessageChoiceData.Metadatas(identifier)
        End Get
    End Property

    Public Property Statistic(statisticType As String) As Integer Implements IStatisticsHolder.Statistic
        Get
            Return MessageChoiceData.Statistics(statisticType)
        End Get
        Set(value As Integer)
            MessageChoiceData.Statistics(statisticType) = value
        End Set
    End Property

    Public Sub RemoveStatistic(statisticType As String) Implements IStatisticsHolder.RemoveStatistic
        MessageChoiceData.Statistics.Remove(statisticType)
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
End Class
