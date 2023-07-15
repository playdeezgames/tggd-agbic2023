Public Interface IMapModel
    Function GetOffsetX(viewWidth As Integer, cellWidth As Integer) As Integer
    Function GetOffsetY(viewHeight As Integer, cellHeight As Integer) As Integer
    Function TerrainGlyphAndColor(location As (Integer, Integer)) As (Char, Integer)
    Function HasCharacter(location As (Integer, Integer)) As Boolean
    Function HasEnemy(location As (Integer, Integer)) As Boolean
    Function IsAdjacent(location As (Integer, Integer)) As Boolean
    Function CharacterGlyphAndColor(location As (Integer, Integer)) As (Char, Integer)
    Function HasItem(location As (Integer, Integer)) As Boolean
    Function ItemGlyphAndColor(location As (Integer, Integer)) As (Char, Integer)
    ReadOnly Property Columns As Integer
    ReadOnly Property Rows As Integer
End Interface
