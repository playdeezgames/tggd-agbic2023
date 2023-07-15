Public Class CellData
    Public Property TerrainType As String
    Public Property CharacterId As Integer?
    Public Property ItemIds As New HashSet(Of Integer)
    Public Property Statistics As New Dictionary(Of String, Integer)
End Class
