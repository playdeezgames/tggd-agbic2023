Public Interface IMapModel
    Function CellExists(cellXY As (column As Integer, row As Integer)) As Boolean
    ReadOnly Property Terrain(cellXY As (column As Integer, row As Integer)) As (Glyph As Char, Hue As Integer)
    ReadOnly Property Character(cellXY As (column As Integer, row As Integer)) As (Glyph As Char, Hue As Integer, MaskGlyph As Char, MaskHue As Integer)?
    ReadOnly Property Items(cellXY As (column As Integer, row As Integer)) As IEnumerable(Of (Glyph As Char, Hue As Integer))
    ReadOnly Property HasItems(cellXY As (column As Integer, row As Integer)) As Boolean
    ReadOnly Property GroundItems(cellXY As (column As Integer, row As Integer)) As List(Of (String, String))
    ReadOnly Property ItemCount(cellXY As (column As Integer, row As Integer), itemName As String) As Integer
    Function FormatItemCount(cellXY As (column As Integer, row As Integer), itemName As String) As String
End Interface
