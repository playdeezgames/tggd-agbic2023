Public Interface IAvatarModel
    Sub Move(delta As (x As Integer, y As Integer))
    ReadOnly Property IsDead As Boolean
    ReadOnly Property Character As (Glyph As Char, Hue As Integer, MaskGlyph As Char, MaskHue As Integer)
    ReadOnly Property Name As String
    ReadOnly Property Health As Integer
    ReadOnly Property MaximumHealth As Integer
End Interface
