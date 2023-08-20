Public Class OffscreenBuffer
    Inherits BasePixelSink
    Implements IPixelSource
    Private ReadOnly _size As (Integer, Integer)
    Private ReadOnly _buffer As Integer()
    Sub New(size As (Integer, Integer))
        _size = size
        ReDim _buffer(_size.Item1 * _size.Item2 - 1)
    End Sub
    Public Overrides Sub SetPixel(x As Integer, y As Integer, hue As Integer)
        _buffer(x + y * _size.Item1) = hue
    End Sub
    Public Function GetPixel(x As Integer, y As Integer) As Integer Implements IPixelSource.GetPixel
        Return _buffer(x + y * _size.Item1)
    End Function
    Const Zero = 0
    Public Shared Function Create(transform As Func(Of Char, Integer), ParamArray lines As String()) As OffscreenBuffer
        Dim buffer As New OffscreenBuffer((lines.First.Length, lines.Length))
        For y = Zero To lines.Length - 1
            Dim line = lines(y)
            Dim x = Zero
            For Each character In line
                buffer.SetPixel(x, y, transform(character))
                x += 1
            Next
        Next
        Return buffer
    End Function

    Public Overrides Sub Fill(hue As Integer)
        Fill((0, 0), _size, hue)
    End Sub
End Class
