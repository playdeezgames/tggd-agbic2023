Public Class ItemData
    Public Property ItemType As String
    Public Property Recycled As Boolean
    Public Property Statistics As New Dictionary(Of String, Integer)
    Public Property Flags As New HashSet(Of String)
End Class
