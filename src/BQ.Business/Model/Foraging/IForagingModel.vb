Public Interface IForagingModel
    ReadOnly Property GridSize As (columns As Integer, rows As Integer)
    Function ForageItemType(itemType As String) As Boolean
    Function GenerateGrid() As (glyph As Char, hue As Integer, itemType As String)(,)
End Interface
