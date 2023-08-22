Public Interface IForagingModel
    ReadOnly Property GridSize As (columns As Integer, rows As Integer)
    ReadOnly Property Remaining As Integer
    ReadOnly Property CanForage As Boolean
    Function Forage() As IItem
    Sub FinalReport(items As IEnumerable(Of IItem))
End Interface
