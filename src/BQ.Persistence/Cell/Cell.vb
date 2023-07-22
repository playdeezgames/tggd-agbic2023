Imports BQ.Data

Friend Class Cell
    Inherits CellDataClient
    Implements ICell
    Public Sub New(worldData As Data.WorldData, mapId As Integer, cellIndex As Integer)
        MyBase.New(worldData, mapId, cellIndex)
    End Sub
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

    Public ReadOnly Property HasCharacters As Boolean Implements ICell.HasCharacters
        Get
            Return CellData.CharacterIds.Any
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

    Public ReadOnly Property HasTrigger As Boolean Implements ICell.HasTrigger
        Get
            Return CellData.TriggerId.HasValue
        End Get
    End Property

    Public Property Trigger As ITrigger Implements ICell.Trigger
        Get
            If Not HasTrigger Then
                Return Nothing
            End If
            Return New Trigger(WorldData, Map.Id, CellData.TriggerId.Value)
        End Get
        Set(value As ITrigger)
            If value Is Nothing Then
                CellData.TriggerId = Nothing
                Return
            End If
            CellData.TriggerId = value.Id
        End Set
    End Property

    Public ReadOnly Property HasCharacter(character As ICharacter) As Boolean Implements ICell.HasCharacter
        Get
            Return CellData.CharacterIds.Contains(character.Id)
        End Get
    End Property

    Public ReadOnly Property Characters As IEnumerable(Of ICharacter) Implements ICell.Characters
        Get
            Return CellData.CharacterIds.Select(Function(x) New Character(WorldData, x))
        End Get
    End Property

    Public ReadOnly Property HasOtherCharacters(character As ICharacter) As Boolean Implements ICell.HasOtherCharacters
        Get
            Return CellData.CharacterIds.Any(Function(x) x <> character.Id)
        End Get
    End Property

    Public ReadOnly Property OtherCharacters(character As ICharacter) As IEnumerable(Of ICharacter) Implements ICell.OtherCharacters
        Get
            Return CellData.CharacterIds.Where(Function(x) x <> character.Id).Select(Function(x) New Character(WorldData, x))
        End Get
    End Property

    Public Sub AddItem(item As IItem) Implements ICell.AddItem
        CellData.ItemIds.Add(item.Id)
    End Sub

    Public Sub RemoveItem(item As IItem) Implements ICell.RemoveItem
        CellData.ItemIds.Remove(item.Id)
    End Sub

    Public Sub AddCharacter(character As ICharacter) Implements ICell.AddCharacter
        CellData.CharacterIds.Add(character.Id)
    End Sub

    Public Sub RemoveCharacter(character As ICharacter) Implements ICell.RemoveCharacter
        CellData.CharacterIds.Remove(character.Id)
    End Sub
End Class
