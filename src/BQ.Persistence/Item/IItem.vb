Public Interface IItem
    Inherits IStatisticsHolder
    ReadOnly Property Id As Integer
    ReadOnly Property ItemType As String
    Sub Recycle()
End Interface
