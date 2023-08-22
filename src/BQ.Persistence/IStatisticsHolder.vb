Public Interface IStatisticsHolder
    ReadOnly Property Statistic(statisticType As String) As Integer
    Sub SetStatistic(statisticType As String, value As Integer)
    Function AddStatistic(statisticType As String, delta As Integer) As Integer
    Sub RemoveStatistic(statisticType As String)
    Function HasStatistic(statisticType As String) As Boolean
    Function TryGetStatistic(statisticType As String, Optional defaultValue As Integer = 0) As Integer
End Interface
