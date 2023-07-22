Public Class TriggerData
    Public Property TriggerType As String
    Public Property MessageLines As New List(Of MessageLineData)
    Public Property Destination As TriggerDestinationData
    Public Property Statistics As New Dictionary(Of String, Integer)
    Public Property VerbType As String
End Class
