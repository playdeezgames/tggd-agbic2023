Imports BQ.Data

Friend Class Cell
    Inherits CellDataClient
    Implements ICell
    Public Sub New(worldData As Data.WorldData, mapId As Integer, cellIndex As Integer)
        MyBase.New(worldData, mapId, cellIndex)
    End Sub
    Public Property Character As ICharacter Implements ICell.Character
        Get
            If CellData.CharacterId.HasValue Then
                Return New Character(WorldData, CellData.CharacterId.Value)
            End If
            Return Nothing
        End Get
        Set(value As ICharacter)
            If value Is Nothing Then
                CellData.CharacterId = Nothing
                Return
            End If
            CellData.CharacterId = value.Id
        End Set
    End Property

    Public ReadOnly Property Id As Integer Implements ICell.Id
        Get
            Return CellIndex
        End Get
    End Property

    Public ReadOnly Property Map As IMap Implements ICell.Map
        Get
            Return New Map(WorldData, MapId)
        End Get
    End Property

    Public ReadOnly Property Column As Integer Implements ICell.Column
        Get
            Return CellIndex Mod Map.Columns
        End Get
    End Property

    Public ReadOnly Property Row As Integer Implements ICell.Row
        Get
            Return CellIndex \ Map.Rows
        End Get
    End Property

    Public Property TerrainType As String Implements ICell.TerrainType
        Get
            Return CellData.TerrainType
        End Get
        Set(value As String)
            CellData.TerrainType = value
        End Set
    End Property

    Public ReadOnly Property HasCharacter As Boolean Implements ICell.HasCharacter
        Get
            Return CellData.CharacterId.HasValue
        End Get
    End Property

    Public ReadOnly Property TopItem As IItem Implements ICell.TopItem
        Get
            If HasItems Then
                Return New Item(WorldData, CellData.ItemIds.First)
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property HasItems As Boolean Implements ICell.HasItems
        Get
            Return CellData.ItemIds.Any
        End Get
    End Property

    Public ReadOnly Property Items As IEnumerable(Of IItem) Implements ICell.Items
        Get
            Return CellData.ItemIds.Select(Function(x) New Item(WorldData, x))
        End Get
    End Property

    Public Property Statistic(statisticType As String) As Integer Implements ICell.Statistic
        Get
            Return CellData.Statistics(statisticType)
        End Get
        Set(value As Integer)
            CellData.Statistics(statisticType) = value
        End Set
    End Property

    Public ReadOnly Property HasStatistic(statisticType As String) As Boolean Implements ICell.HasStatistic
        Get
            Return CellData.Statistics.ContainsKey(statisticType)
        End Get
    End Property

    Public Sub AddItem(item As IItem) Implements ICell.AddItem
        CellData.ItemIds.Add(item.Id)
    End Sub

    Public Sub RemoveItem(item As IItem) Implements ICell.RemoveItem
        CellData.ItemIds.Remove(item.Id)
    End Sub
End Class
