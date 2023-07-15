Public Interface IItem
    ReadOnly Property Id As Integer
    ReadOnly Property ItemType As String
    Sub Recycle()
    Property Statistic(statisticType As String) As Integer
    ReadOnly Property HasStatistic(statisticType As String) As Boolean
End Interface
