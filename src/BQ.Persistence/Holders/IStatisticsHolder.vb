Public Interface IStatisticsHolder
    Property Statistic(statisticType As String) As Integer
    Sub RemoveStatistic(statisticType As String)
    Function HasStatistic(statisticType As String) As Boolean
    Function TryGetStatistic(statisticType As String, Optional defaultValue As Integer = 0) As Integer
End Interface
