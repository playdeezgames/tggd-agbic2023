Public Interface IPixelSink
    Sub SetPixel(x As Integer, y As Integer, hue As Integer)
    Sub Stretch(
            source As IPixelSource,
            fromLocation As (Integer, Integer),
            toLocation As (Integer, Integer),
            size As (Integer, Integer),
            scale As (Integer, Integer),
            filter As Func(Of Integer, Boolean))
    Sub Colorize(
                source As IPixelSource,
                fromLocation As (Integer, Integer),
                toLocation As (Integer, Integer),
                size As (Integer, Integer),
                xform As Func(Of Integer, Integer?))
    Sub Fill(location As (Integer, Integer), size As (Integer, Integer), hue As Integer)
    Sub Fill(hue As Integer)
    Sub Frame(location As (Integer, Integer), size As (Integer, Integer), hue As Integer)
End Interface
