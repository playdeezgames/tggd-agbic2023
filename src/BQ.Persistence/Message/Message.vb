Friend Class Message
    Inherits MessageDataClient
    Implements IMessage

    Public Sub New(worldData As Data.WorldData, messageId As Integer)
        MyBase.New(worldData, messageId)
    End Sub

    Public ReadOnly Property LineCount As Integer Implements IMessage.LineCount
        Get
            Return MessageData.Lines.Count
        End Get
    End Property

    Public ReadOnly Property Lines As IEnumerable(Of IMessageLine) Implements IMessage.Lines
        Get
            Return Enumerable.Range(0, LineCount).Select(Function(x) New MessageLine(WorldData, MessageId, x))
        End Get
    End Property

    Public Property Sfx As String Implements IMessage.Sfx
        Get
            Return MessageData.Sfx
        End Get
        Set(value As String)
            MessageData.Sfx = value
        End Set
    End Property

    Public Function AddLine(hue As Integer, text As String) As IMessage Implements IMessage.AddLine
        MessageData.Lines.Add(New Data.MessageLineData With
                              {
                                .Text = text,
                                .Hue = hue
                              })
        Return Me
    End Function

    Public Function SetSfx(sfx As String) As IMessage Implements IMessage.SetSfx
        Me.Sfx = sfx
        Return Me
    End Function
End Class
