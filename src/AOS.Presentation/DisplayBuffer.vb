Friend Class DisplayBuffer
    Inherits BasePixelSink
    Implements IDisplayBuffer
    Private ReadOnly _texture As Texture2D
    Private ReadOnly _hueTable As IReadOnlyDictionary(Of Integer, Color)
    Protected _buffer As Color()
    Sub New(texture As Texture2D, hueTable As IReadOnlyDictionary(Of Integer, Color))
        _texture = texture
        _hueTable = hueTable
        ReDim _buffer(_texture.Width * _texture.Height - 1)
    End Sub
    Public Sub Commit() Implements IDisplayBuffer.Commit
        _texture.SetData(_buffer)
    End Sub
    Const Zero = 0
    Public Overrides Sub SetPixel(x As Integer, y As Integer, hue As Integer) Implements IDisplayBuffer.SetPixel
        If x < Zero OrElse y < Zero OrElse x >= _texture.Width OrElse y >= _texture.Height Then
            Return
        End If
        _buffer(x + y * _texture.Width) = _hueTable(hue)
    End Sub

    Public Overrides Sub Fill(hue As Integer)
        Fill((0, 0), (_texture.Width, _texture.Height), hue)
    End Sub
End Class
