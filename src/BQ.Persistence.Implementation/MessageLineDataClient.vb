Imports BQ.Data

Friend Class MessageLineDataClient
    Inherits MessageDataClient
    Protected ReadOnly LineId As Integer
    Protected ReadOnly Property MessageLineData As MessageLineData
        Get
            Return MessageData.Lines(LineId)
        End Get
    End Property
    Public Sub New(worldData As Data.WorldData, messageId As Integer, lineId As Integer)
        MyBase.New(worldData, messageId)
        Me.LineId = lineId
    End Sub
End Class
