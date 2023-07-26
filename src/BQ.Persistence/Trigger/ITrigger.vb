Public Interface ITrigger
    Inherits IStatisticsHolder
    Inherits IFlagHolder
    Inherits IMetadataHolder
    ReadOnly Property Id As Integer
    ReadOnly Property TriggerType As String
    Function SetTriggerType(triggerType As String) As ITrigger
End Interface
