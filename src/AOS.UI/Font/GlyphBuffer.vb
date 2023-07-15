Public Class GlyphBuffer
    Inherits OffscreenBuffer
    Public ReadOnly Property Width As Integer
    Public ReadOnly Property Height As Integer
    Public Sub New(font As FontData, glyph As Char)
        MyBase.New((font.Glyphs(glyph).Width, font.Height))
        Height = font.Height
        Width = font.Glyphs(glyph).Width
        For Each row In font.Glyphs(glyph).Lines
            For Each column In row.Value
                SetPixel(column, row.Key, 1)
            Next
        Next
    End Sub
    Const Zero = 0
    Public Sub CopyTo(
                                           sink As IPixelSink,
                                           position As (Integer, Integer),
                                           hue As Integer)
        sink.Colorize(Me, (Zero, Zero), position, (Width, Height), Function(x)
                                                                       If x <> 0 Then
                                                                           Return hue
                                                                       Else
                                                                           Return Nothing
                                                                       End If
                                                                   End Function)
    End Sub
End Class
