Public Interface ILegacyForagingModel
    ReadOnly Property GridSize As (columns As Integer, rows As Integer)
    Function LegacyForageItemType(itemType As String) As Boolean
    Function LegacyGenerateGrid() As (glyph As Char, hue As Integer, itemType As String)(,)
End Interface
