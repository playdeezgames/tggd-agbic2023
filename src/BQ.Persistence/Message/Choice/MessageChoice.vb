Friend Class MessageChoice
    Inherits MessageChoiceDataClient
    Implements IMessageChoice

    Public Sub New(worldData As Data.WorldData, messageId As Integer, choiceId As Integer)
        MyBase.New(worldData, messageId, choiceId)
    End Sub

    Public ReadOnly Property Text As String Implements IMessageChoice.Text
        Get
            Return MessageChoiceData.Text
        End Get
    End Property

    Public ReadOnly Property TriggerType As String Implements IMessageChoice.TriggerType
        Get
            Return MessageChoiceData.TriggerType
        End Get
    End Property
End Class
