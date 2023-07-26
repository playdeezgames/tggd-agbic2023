Public Class CellData
    Inherits BaseData
    Public Property TerrainType As String
    Public Property ItemIds As New HashSet(Of Integer)
    Public Property TriggerId As Integer?
    Public Property CharacterIds As New HashSet(Of Integer)
End Class
