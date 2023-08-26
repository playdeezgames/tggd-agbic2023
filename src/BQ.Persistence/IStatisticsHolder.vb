Public Interface IStatisticsHolder
    Sub SetStatistic(statisticType As String, value As Integer)
    Function AddStatistic(statisticType As String, delta As Integer) As Integer
    Sub RemoveStatistic(statisticType As String)
    Function HasStatistic(statisticType As String) As Boolean
    Function GetStatistic(statisticType As String, Optional defaultValue As Integer = 0) As Integer
End Interface
