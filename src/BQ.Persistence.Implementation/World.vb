﻿Imports System.IO
Imports System.Text.Json
Imports BQ.Data

Public Class World
    Inherits WorldDataClient
    Implements IWorld
    Public Sub New(worldData As WorldData)
        MyBase.New(worldData)
    End Sub

    Public Property Avatar As ICharacter Implements IWorld.Avatar
        Get
            If WorldData.AvatarCharacterId.HasValue Then
                Return New Character(WorldData, WorldData.AvatarCharacterId.Value)
            End If
            Return Nothing
        End Get
        Set(value As ICharacter)
            WorldData.AvatarCharacterId = value?.Id
        End Set
    End Property

    Public ReadOnly Property Characters As IEnumerable(Of ICharacter) Implements IWorld.Characters
        Get
            Return Enumerable.Range(0, WorldData.Characters.Count).Select(Function(x) New Character(WorldData, x))
        End Get
    End Property

    Public ReadOnly Property HasMessages As Boolean Implements IWorld.HasMessages
        Get
            Return WorldData.Messages.Any
        End Get
    End Property

    Public ReadOnly Property CurrentMessage As IMessage Implements IWorld.CurrentMessage
        Get
            If HasMessages Then
                Return New Message(WorldData, 0)
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property Maps As IEnumerable(Of IMap) Implements IWorld.Maps
        Get
            Return Enumerable.Range(0, WorldData.Maps.Count).Select(Function(x) New Map(WorldData, x))
        End Get
    End Property

    Public ReadOnly Property SerializedData As String Implements IWorld.SerializedData
        Get
            Return JsonSerializer.Serialize(WorldData)
        End Get
    End Property

    Public Sub DismissMessage() Implements IWorld.DismissMessage
        If HasMessages Then
            WorldData.Messages.RemoveAt(0)
        End If
    End Sub

    Public Sub RemoveStatistic(statisticType As String) Implements IStatisticsHolder.RemoveStatistic
        WorldData.Statistics.Remove(statisticType)
    End Sub

    Public Sub RemoveMetadata(identifier As String) Implements IMetadataHolder.RemoveMetadata
        WorldData.Metadatas.Remove(identifier)
    End Sub

    Public Sub SetStatistic(statisticType As String, value As Integer) Implements IStatisticsHolder.SetStatistic
        WorldData.Statistics(statisticType) = value
    End Sub

    Public Sub SetFlag(flagType As String, value As Boolean) Implements IFlagHolder.SetFlag
        If value Then
            WorldData.Flags.Add(flagType)
        Else
            WorldData.Flags.Remove(flagType)
        End If
    End Sub

    Public Sub SetMetadata(identifier As String, value As String) Implements IMetadataHolder.SetMetadata
        WorldData.Metadatas(identifier) = value
    End Sub

    Public Shared Function Load(filename As String) As IWorld
        Try
            Return New World(JsonSerializer.Deserialize(Of WorldData)(File.ReadAllText(filename)))
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function CreateMap(mapType As String, size As (Integer, Integer), terrainType As String) As IMap Implements IWorld.CreateMap
        Dim mapId = WorldData.Maps.Count
        Dim mapData =
            New MapData With
            {
                .MapType = mapType,
                .Columns = size.Item1,
                .Rows = size.Item2
            }
        While mapData.Cells.Count < size.Item1 * size.Item2
            Dim cellData = New CellData With
                {
                    .TerrainType = terrainType
                }
            mapData.Cells.Add(cellData)
        End While
        WorldData.Maps.Add(mapData)
        Return New Map(WorldData, mapId)
    End Function

    Public Function CreateCharacter(characterType As String, cell As ICell) As ICharacter Implements IWorld.CreateCharacter
        Dim characterData As New CharacterData With
            {
                .Recycled = False,
                .CharacterType = characterType,
                .MapId = cell.Map.Id,
                .CellIndex = cell.Id
            }
        Dim index = WorldData.Characters.FindIndex(Function(x) x.Recycled)
        If index = -1 Then
            index = WorldData.Characters.Count
            WorldData.Characters.Add(characterData)
        Else
            WorldData.Characters(index) = characterData
        End If
        Return New Character(WorldData, index)
    End Function

    Public Function CreateMessage() As IMessage Implements IWorld.CreateMessage
        Dim messageId = WorldData.Messages.Count
        WorldData.Messages.Add(New MessageData)
        Return New Message(WorldData, messageId)
    End Function

    Public Function CreateItem(itemType As String) As IItem Implements IWorld.CreateItem
        Dim itemData As New ItemData With
            {
                .ItemType = itemType,
                .Recycled = False
            }
        Dim index = WorldData.Items.FindIndex(Function(x) x.Recycled)
        If index = -1 Then
            index = WorldData.Items.Count
            WorldData.Items.Add(itemData)
        Else
            WorldData.Items(index) = itemData
        End If
        Return New Item(WorldData, index)
    End Function

    Public Function HasStatistic(statisticType As String) As Boolean Implements IStatisticsHolder.HasStatistic
        Return WorldData.Statistics.ContainsKey(statisticType)
    End Function

    Public Function HasMetadata(identifier As String) As Boolean Implements IMetadataHolder.HasMetadata
        Return WorldData.Metadatas.ContainsKey(identifier)
    End Function

    Public Function GetStatistic(statisticType As String, Optional defaultValue As Integer = 0) As Integer Implements IStatisticsHolder.GetStatistic
        Return If(HasStatistic(statisticType), WorldData.Statistics(statisticType), defaultValue)
    End Function

    Public Function AddStatistic(statisticType As String, delta As Integer) As Integer Implements IStatisticsHolder.AddStatistic
        SetStatistic(statisticType, GetStatistic(statisticType) + delta)
        Return GetStatistic(statisticType)
    End Function

    Public Function GetFlag(flagType As String) As Boolean Implements IFlagHolder.GetFlag
        Return WorldData.Flags.Contains(flagType)
    End Function

    Public Function GetMetadata(identifier As String) As String Implements IMetadataHolder.GetMetadata
        Return WorldData.Metadatas(identifier)
    End Function

    Public Function GetMap(id As Integer) As IMap Implements IWorld.GetMap
        Return New Map(WorldData, id)
    End Function

    Public Function GetCharacter(id As Integer) As ICharacter Implements IWorld.GetCharacter
        Return New Character(WorldData, id)
    End Function

    Public Function GetItem(id As Integer) As IItem Implements IWorld.GetItem
        Return New Item(WorldData, id)
    End Function
End Class
