Public MustInherit Class VisibleEntityDescriptor
    Property Glyph As Char
    Property Hue As Integer
    Property Name As String
    Sub New()

    End Sub
    Sub New(name As String, glyph As Char, hue As Integer)
        Me.Glyph = glyph
        Me.Hue = hue
        Me.Name = name
    End Sub
End Class
