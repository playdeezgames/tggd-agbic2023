Friend Class MessageLine
    Inherits MessageLineDataClient
    Implements IMessageLine

    Public Sub New(worldData As Data.WorldData, messageId As Integer, lineId As Integer)
        MyBase.New(worldData, messageId, lineId)
    End Sub

    Public ReadOnly Property Text As String Implements IMessageLine.Text
        Get
            Return MessageLineData.Text
        End Get
    End Property

    Public ReadOnly Property Hue As Integer Implements IMessageLine.Hue
        Get
            Return MessageLineData.Hue
        End Get
    End Property
End Class
