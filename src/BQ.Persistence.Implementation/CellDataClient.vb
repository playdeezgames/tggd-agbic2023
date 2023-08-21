Imports BQ.Data

Friend Class CellDataClient
    Inherits MapDataClient
    Protected ReadOnly CellIndex As Integer
    Protected ReadOnly Property CellData As CellData
        Get
            Return MapData.Cells(CellIndex)
        End Get
    End Property
    Public Sub New(worldData As Data.WorldData, mapId As Integer, cellIndex As Integer)
        MyBase.New(worldData, mapId)
        Me.CellIndex = cellIndex
    End Sub
End Class
