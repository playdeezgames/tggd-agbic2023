Public Interface IMap
    Inherits IStatisticsHolder
    Inherits IFlagHolder
    ReadOnly Property Columns As Integer
    ReadOnly Property Rows As Integer
    ReadOnly Property Cells As IEnumerable(Of ICell)
    ReadOnly Property World As IWorld
    ReadOnly Property Id As Integer
    Function GetCell(column As Integer, row As Integer) As ICell
    Function CreateTrigger() As ITrigger
    ReadOnly Property MapType As String
End Interface
