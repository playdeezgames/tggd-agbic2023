Imports BQ.Data

Friend Class MessageChoiceDataClient
    Inherits MessageDataClient
    Protected ReadOnly ChoiceId As Integer
    Protected ReadOnly Property MessageChoiceData As MessageChoiceData
        Get
            Return MessageData.Choices(ChoiceId)
        End Get
    End Property
    Public Sub New(worldData As Data.WorldData, messageId As Integer, choiceId As Integer)
        MyBase.New(worldData, messageId)
        Me.ChoiceId = choiceId
    End Sub
End Class
