Public Class Drawer
    Private X As Integer
    Private Y As Integer
    Private Hue As Integer
    Private sink As IPixelSink
    Sub New(
           sink As IPixelSink,
           Optional position As (Integer, Integer) = Nothing,
           Optional hue As Integer = 0)
        Me.sink = sink
        X = position.Item1
        Y = position.Item2
        Me.Hue = hue
    End Sub
    Const Zero = 0
    Public Function Repeat(iterationCount As Integer, operation As Func(Of Drawer, Drawer)) As Drawer
        Dim result As Drawer = Me
        While iterationCount > Zero
            result = operation(result)
            iterationCount -= 1
        End While
        Return result
    End Function

    Public Function DownRight(stepCount As Integer) As Drawer
        While stepCount > Zero
            sink.SetPixel(X, Y, Hue)
            Y += 1
            X += 1
            stepCount -= 1
        End While
        Return Me
    End Function

    Public Function UpRight(stepCount As Integer) As Drawer
        While stepCount > Zero
            sink.SetPixel(X, Y, Hue)
            Y -= 1
            X += 1
            stepCount -= 1
        End While
        Return Me
    End Function
    Public Function DownLeft(stepCount As Integer) As Drawer
        While stepCount > Zero
            sink.SetPixel(X, Y, Hue)
            Y += 1
            X -= 1
            stepCount -= 1
        End While
        Return Me
    End Function

    Public Function UpLeft(stepCount As Integer) As Drawer
        While stepCount > Zero
            sink.SetPixel(X, Y, Hue)
            Y -= 1
            X -= 1
            stepCount -= 1
        End While
        Return Me
    End Function

    Public Function MoveTo(x As Integer, y As Integer) As Drawer
        Me.X = x
        Me.Y = y
        Return Me
    End Function

    Public Function Color(hue As Integer) As Drawer
        Me.Hue = hue
        Return Me
    End Function

    Public Function Up(stepCount As Integer) As Drawer
        While stepCount > Zero
            sink.SetPixel(X, Y, Hue)
            Y -= 1
            stepCount -= 1
        End While
        Return Me
    End Function

    Public Function Down(stepCount As Integer) As Drawer
        While stepCount > Zero
            sink.SetPixel(X, Y, Hue)
            Y += 1
            stepCount -= 1
        End While
        Return Me
    End Function

    Public Function Left(stepCount As Integer) As Drawer
        While stepCount > Zero
            sink.SetPixel(X, Y, Hue)
            X -= 1
            stepCount -= 1
        End While
        Return Me
    End Function

    Public Function Right(stepCount As Integer) As Drawer
        While stepCount > Zero
            sink.SetPixel(X, Y, Hue)
            X += 1
            stepCount -= 1
        End While
        Return Me
    End Function
End Class
