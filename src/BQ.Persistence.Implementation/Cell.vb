﻿Imports BQ.Data

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
            Return CellIndex \ Map.Columns
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

    Public ReadOnly Property HasEffect As Boolean Implements ICell.HasEffect
        Get
            Return CellData.TriggerId.HasValue
        End Get
    End Property

    Public Property Effect As IMapEffect Implements ICell.Effect
        Get
            If Not HasEffect Then
                Return Nothing
            End If
            Return New MapEffect(WorldData, Map.Id, CellData.TriggerId.Value)
        End Get
        Set(value As IMapEffect)
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

    Public Sub RemoveStatistic(statisticType As String) Implements IStatisticsHolder.RemoveStatistic
        CellData.Statistics.Remove(statisticType)
    End Sub

    Public Sub RemoveMetadata(identifier As String) Implements IMetadataHolder.RemoveMetadata
        CellData.Metadatas.Remove(identifier)
    End Sub

    Public Sub SetStatistic(statisticType As String, value As Integer) Implements IStatisticsHolder.SetStatistic
        CellData.Statistics(statisticType) = value
    End Sub

    Public Sub SetFlag(flagType As String, value As Boolean) Implements IFlagHolder.SetFlag
        If value Then
            CellData.Flags.Add(flagType)
        Else
            CellData.Flags.Remove(flagType)
        End If
    End Sub

    Public Sub SetMetadata(identifier As String, value As String) Implements IMetadataHolder.SetMetadata
        CellData.Metadatas(identifier) = value
    End Sub

    Public Function HasMetadata(identifier As String) As Boolean Implements IMetadataHolder.HasMetadata
        Return CellData.Metadatas.ContainsKey(identifier)
    End Function

    Public Function GetStatistic(statisticType As String, Optional defaultValue As Integer = 0) As Integer Implements IStatisticsHolder.GetStatistic
        Return If(HasStatistic(statisticType), CellData.Statistics(statisticType), defaultValue)
    End Function

    Public Function AddStatistic(statisticType As String, delta As Integer) As Integer Implements IStatisticsHolder.AddStatistic
        SetStatistic(statisticType, GetStatistic(statisticType) + delta)
        Return GetStatistic(statisticType)
    End Function

    Public Function GetFlag(flagType As String) As Boolean Implements IFlagHolder.GetFlag
        Return CellData.Flags.Contains(flagType)
    End Function

    Public Function GetMetadata(identifier As String) As String Implements IMetadataHolder.GetMetadata
        Return CellData.Metadatas(identifier)
    End Function

    Private Function HasStatistic(statisticType As String) As Boolean Implements IStatisticsHolder.HasStatistic
        Return CellData.Statistics.ContainsKey(statisticType)
    End Function
End Class
