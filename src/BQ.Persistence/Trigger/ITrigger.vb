Public Interface ITrigger
    Inherits IStatisticsHolder
    Inherits IFlagHolder
    Inherits IMetadataHolder
    ReadOnly Property Id As Integer
    Property TriggerType As String
End Interface
