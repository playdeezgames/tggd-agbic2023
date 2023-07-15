Public Class FontData
    Public Property Height As Integer
    Public Property Glyphs As Dictionary(Of Char, GlyphData)
    Public Function ToOffscreenBuffer(glyph As Char) As OffscreenBuffer
        Dim buffer As New OffscreenBuffer((Glyphs(glyph).Width, Height))
        For Each row In Glyphs(glyph).Lines
            For Each column In row.Value
                buffer.SetPixel(column, row.Key, 1)
            Next
        Next
        Return buffer
    End Function
End Class
