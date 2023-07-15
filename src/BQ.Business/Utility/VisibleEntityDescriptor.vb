Friend MustInherit Class VisibleEntityDescriptor
    ReadOnly Property Glyph As Char
    ReadOnly Property Hue As Integer
    ReadOnly Property Name As String
    Sub New(name As String, glyph As Char, hue As Integer)
        Me.Glyph = glyph
        Me.Hue = hue
        Me.Name = name
    End Sub
End Class
