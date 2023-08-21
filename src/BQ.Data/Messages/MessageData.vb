Public Class MessageData
    Inherits BaseData
    Public Property Sfx As String
    Public Property Lines As New List(Of MessageLineData)
    Public Property Choices As New List(Of MessageChoiceData)
    Public Property CancelChoice As Integer
End Class
