Public Interface IEnemyModel
    ReadOnly Property Exists As Boolean
    Sub Attack()
    ReadOnly Property Enemies As IEnumerable(Of (Glyph As Char, Hue As Integer, MaskGlyph As Char, MaskHue As Integer))
End Interface
