Public Interface IItem
    Inherits IStatisticsHolder
    Inherits IFlagHolder
    ReadOnly Property Id As Integer
    ReadOnly Property ItemType As String
    Sub Recycle()
End Interface
