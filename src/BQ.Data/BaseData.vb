Public MustInherit Class BaseData
    Public Property Statistics As New Dictionary(Of String, Integer)
    Public Property Flags As New HashSet(Of String)
    Public Property Metadata As New Dictionary(Of String, String)
End Class
