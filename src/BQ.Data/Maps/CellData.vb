﻿Public Class CellData
    Public Property TerrainType As String
    Public Property CharacterId As Integer?
    Public Property ItemIds As New HashSet(Of Integer)
    Public Property Statistics As New Dictionary(Of String, Integer)
    Public Property TriggerId As Integer?
    Public Property CharacterIds As New HashSet(Of Integer)
End Class
