Public Interface IEnemyModel
    ReadOnly Property Exists As Boolean
    ReadOnly Property Count As Integer
    ReadOnly Property Enemies As IEnumerable(Of (Glyph As Char, Hue As Integer, MaskGlyph As Char, MaskHue As Integer))
    ReadOnly Property Enemy(index As Integer) As (Name As String, Health As Integer, MaximumHealth As Integer)
End Interface
