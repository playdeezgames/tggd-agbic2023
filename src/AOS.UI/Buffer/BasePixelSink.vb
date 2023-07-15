Public MustInherit Class BasePixelSink
    Implements IPixelSink
    Public MustOverride Sub SetPixel(x As Integer, y As Integer, hue As Integer) Implements IPixelSink.SetPixel
    Const Zero = 0
    Public Sub Fill(location As (Integer, Integer), size As (Integer, Integer), hue As Integer) Implements IPixelSink.Fill
        For x = Zero To size.Item1 - 1
            For y = Zero To size.Item2 - 1
                SetPixel(location.Item1 + x, location.Item2 + y, hue)
            Next
        Next
    End Sub

    Public Sub Colorize(source As IPixelSource, fromLocation As (Integer, Integer), toLocation As (Integer, Integer), size As (Integer, Integer), xform As Func(Of Integer, Integer?)) Implements IPixelSink.Colorize
        For x = Zero To size.Item1 - 1
            For y = Zero To size.Item2 - 1
                Dim sourceHue = source.GetPixel(x + fromLocation.Item1, y + fromLocation.Item2)
                Dim destinationHue As Integer? = xform(sourceHue)
                If destinationHue IsNot Nothing Then
                    SetPixel(x + toLocation.Item1, y + toLocation.Item2, destinationHue.Value)
                End If
            Next
        Next
    End Sub

    Public Sub Stretch(source As IPixelSource, fromLocation As (Integer, Integer), toLocation As (Integer, Integer), size As (Integer, Integer), scale As (Integer, Integer), filter As Func(Of Integer, Boolean)) Implements IPixelSink.Stretch
        For x = Zero To size.Item1 - 1
            For y = Zero To size.Item2 - 1
                Dim hue = source.GetPixel(x + fromLocation.Item1, y + fromLocation.Item2)
                If filter(hue) Then
                    Fill((x * scale.Item1 + toLocation.Item1, y * scale.Item2 + toLocation.Item2), scale, hue)
                End If
            Next
        Next
    End Sub

    Public Sub Frame(location As (Integer, Integer), size As (Integer, Integer), hue As Integer) Implements IPixelSink.Frame
        For x = location.Item1 To location.Item1 + size.Item1 - 1
            SetPixel(x, location.Item2, hue)
            SetPixel(x, location.Item2 + size.Item2 - 1, hue)
        Next
        For y = location.Item2 + 1 To location.Item2 + size.Item2 - 2
            SetPixel(location.Item1, y, hue)
            SetPixel(location.Item1 + size.Item1 - 1, y, hue)
        Next
    End Sub
End Class
