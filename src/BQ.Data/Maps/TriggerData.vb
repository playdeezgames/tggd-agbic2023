Public Class TriggerData
    Public Property TriggerType As String
    Public Property Statistics As New Dictionary(Of String, Integer)
    Public Property Metadata As New Dictionary(Of String, String)
    Public Property Flags As New HashSet(Of String)
End Class
