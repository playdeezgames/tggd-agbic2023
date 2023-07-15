Public Class Sprite
    Inherits OffscreenBuffer
    Public ReadOnly Property Width As Integer
    Public ReadOnly Property Height As Integer
    Const Zero = 0
    Public Sub New(lines As IReadOnlyList(Of String), transform As Func(Of Char, Integer))
        MyBase.New((lines.First.Length, lines.Count))
        Width = lines.First.Length
        Height = lines.Count
        Dim y = Zero
        For Each line In lines
            Dim x = Zero
            For Each character In line
                SetPixel(x, y, transform(character))
                x += 1
            Next
            y += 1
        Next line
    End Sub

    Public Sub StretchTo(sink As IPixelSink, position As (Integer, Integer), scale As (Integer, Integer), filter As Func(Of Integer, Boolean))
        sink.Stretch(Me, (Zero, Zero), position, (Width, Height), scale, filter)
    End Sub
End Class
