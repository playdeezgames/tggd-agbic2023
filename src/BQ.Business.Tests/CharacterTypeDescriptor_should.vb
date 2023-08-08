Public Class CharacterTypeDescriptor_should
    <Fact>
    Sub store_name_glyph_hue()
        Const name = "name"
        Const glyph = " "c
        Const hue = 1
        Dim subject As CharacterTypeDescriptor = New CharacterTypeDescriptor(name, glyph, hue)
        subject.Name.ShouldBe(name)
        subject.Glyph.ShouldBe(glyph)
        subject.Hue.ShouldBe(hue)
        subject.MaskGlyph.ShouldBe(ChrW(0))
        subject.MaskHue.ShouldBe(0)
    End Sub
End Class

