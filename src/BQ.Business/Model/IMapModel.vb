Public Interface IMapModel
    Function CellExists(cellXY As (column As Integer, row As Integer)) As Boolean
    ReadOnly Property Terrain(cellXY As (column As Integer, row As Integer)) As (Glyph As Char, Hue As Integer)
    ReadOnly Property Character(cellXY As (column As Integer, row As Integer)) As (Glyph As Char, Hue As Integer, MaskGlyph As Char, MaskHue As Integer)?
End Interface
