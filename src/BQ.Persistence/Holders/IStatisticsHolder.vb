Public Interface IStatisticsHolder
    Property Statistic(statisticType As String) As Integer
    Sub RemoveStatistic(statisticType As String)
    Function HasStatistic(statisticType As String) As Boolean
End Interface
