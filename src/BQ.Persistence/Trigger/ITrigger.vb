Public Interface ITrigger
    ReadOnly Property Id As Integer
    ReadOnly Property TriggerType As String
    Function SetTriggerType(triggerType As String) As ITrigger
    ReadOnly Property Statistics(statisticType As String) As Integer
    Function SetStatistic(statisticType As String, value As Integer) As ITrigger
    ReadOnly Property Metadata(identifier As String) As String
    Function SetMetadata(identifier As String, value As String) As ITrigger
End Interface
