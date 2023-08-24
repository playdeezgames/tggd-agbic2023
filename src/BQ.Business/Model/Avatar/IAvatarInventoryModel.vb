Public Interface IAvatarInventoryModel
    ReadOnly Property GridSize As (columns As Integer, rows As Integer)
    ReadOnly Property Items As IEnumerable(Of (glyph As Char, hue As Integer, name As String, count As Integer))
    ReadOnly Property ItemCount(name As String) As Integer
End Interface
