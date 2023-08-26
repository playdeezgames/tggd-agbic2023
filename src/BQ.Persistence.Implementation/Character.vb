Friend Class Character
    Inherits CharacterDataClient
    Implements ICharacter

    Public Sub New(worldData As Data.WorldData, characterId As Integer)
        MyBase.New(worldData, characterId)
    End Sub

    Public ReadOnly Property Id As Integer Implements ICharacter.Id
        Get
            Return CharacterId
        End Get
    End Property

    Public ReadOnly Property CharacterType As String Implements ICharacter.CharacterType
        Get
            Return CharacterData.CharacterType
        End Get
    End Property

    Public Property Cell As ICell Implements ICharacter.Cell
        Get
            Return New Cell(WorldData, CharacterData.MapId, CharacterData.CellIndex)
        End Get
        Set(value As ICell)
            CharacterData.MapId = value.Map.Id
            CharacterData.CellIndex = value.Id
        End Set
    End Property

    Public ReadOnly Property Map As IMap Implements ICharacter.Map
        Get
            Return Cell.Map
        End Get
    End Property

    Public ReadOnly Property World As IWorld Implements ICharacter.World
        Get
            Return New World(WorldData)
        End Get
    End Property

    Public ReadOnly Property Items As IEnumerable(Of IItem) Implements ICharacter.Items
        Get
            Return CharacterData.ItemIds.Select(Function(x) New Item(WorldData, x))
        End Get
    End Property

    Public ReadOnly Property HasItems As Boolean Implements ICharacter.HasItems
        Get
            Return CharacterData.ItemIds.Any
        End Get
    End Property

    ReadOnly Property IsAvatar As Boolean Implements ICharacter.IsAvatar
        Get
            Return If(WorldData.AvatarCharacterId, -1) = Id
        End Get
    End Property

    Public ReadOnly Property HasEquipment As Boolean Implements ICharacter.HasEquipment
        Get
            Return CharacterData.EquipSlots.Any
        End Get
    End Property

    Public ReadOnly Property Equipment As IReadOnlyDictionary(Of String, IItem) Implements ICharacter.Equipment
        Get
            Return CharacterData.EquipSlots.ToDictionary(Of String, IItem)(Function(x) x.Key, Function(x) New Item(WorldData, x.Value))
        End Get
    End Property

    Public ReadOnly Property EquippedItems As IReadOnlyList(Of IItem) Implements ICharacter.EquippedItems
        Get
            Return CharacterData.EquipSlots.Values.Distinct.Select(Of IItem)(Function(x) New Item(WorldData, x)).ToList
        End Get
    End Property

    Public ReadOnly Property ItemCount As Integer Implements ICharacter.ItemCount
        Get
            Return CharacterData.ItemIds.Count
        End Get
    End Property

    Public Sub Recycle() Implements ICharacter.Recycle
        If Not IsAvatar Then
            For Each equippedItem In EquippedItems
                UnequipItem(equippedItem)
            Next
            For Each item In Items
                RemoveItem(item)
                Cell.AddItem(item)
            Next
            Cell.RemoveCharacter(Me)
            CharacterData.Recycled = True
        End If
    End Sub

    Public Sub RemoveItem(item As IItem) Implements ICharacter.RemoveItem
        CharacterData.ItemIds.Remove(item.Id)
    End Sub

    Public Sub AddItem(item As IItem) Implements ICharacter.AddItem
        CharacterData.ItemIds.Add(item.Id)
    End Sub

    Public Sub Equip(equipSlotType As String, item As IItem) Implements ICharacter.Equip
        Unequip(equipSlotType)
        RemoveItem(item)
        CharacterData.EquipSlots(equipSlotType) = item.Id
    End Sub
    Public Sub Unequip(equipSlotType As String) Implements ICharacter.Unequip
        If CharacterData.EquipSlots.ContainsKey(equipSlotType) Then
            AddItem(New Item(WorldData, CharacterData.EquipSlots(equipSlotType)))
            CharacterData.EquipSlots.Remove(equipSlotType)
        End If
    End Sub

    Public Sub UnequipItem(item As IItem) Implements ICharacter.UnequipItem
        For Each equipSlot In CharacterData.EquipSlots.Where(Function(x) x.Value = item.Id).Select(Function(x) x.Key)
            Unequip(equipSlot)
        Next
    End Sub

    Public Sub RemoveStatistic(statisticType As String) Implements ICharacter.RemoveStatistic
        CharacterData.Statistics.Remove(statisticType)
    End Sub

    Public Sub RemoveMetadata(identifier As String) Implements ICharacter.RemoveMetadata
        CharacterData.Metadatas.Remove(identifier)
    End Sub

    Public Sub SetStatistic(statisticType As String, value As Integer) Implements IStatisticsHolder.SetStatistic
        CharacterData.Statistics(statisticType) = value
    End Sub

    Public Sub SetFlag(flagType As String, value As Boolean) Implements IFlagHolder.SetFlag
        If value Then
            CharacterData.Flags.Add(flagType)
        Else
            CharacterData.Flags.Remove(flagType)
        End If
    End Sub

    Public Sub SetMetadata(identifier As String, value As String) Implements IMetadataHolder.SetMetadata
        CharacterData.Metadatas(identifier) = value
    End Sub

    Public Function HasStatistic(statisticType As String) As Boolean Implements ICharacter.HasStatistic
        Return CharacterData.Statistics.ContainsKey(statisticType)
    End Function

    Public Function ItemTypeCount(itemType As String) As Integer Implements ICharacter.ItemTypeCount
        Return Items.Count(Function(x) x.ItemType = itemType)
    End Function

    Public Function HasMetadata(identifier As String) As Boolean Implements IMetadataHolder.HasMetadata
        Return CharacterData.Metadatas.ContainsKey(identifier)
    End Function

    Public Function GetStatistic(statisticType As String, Optional defaultValue As Integer = 0) As Integer Implements IStatisticsHolder.GetStatistic
        Return If(HasStatistic(statisticType), CharacterData.Statistics(statisticType), defaultValue)
    End Function

    Public Function AddStatistic(statisticType As String, delta As Integer) As Integer Implements IStatisticsHolder.AddStatistic
        SetStatistic(statisticType, GetStatistic(statisticType) + delta)
        Return GetStatistic(statisticType)
    End Function

    Public Function GetFlag(flagType As String) As Boolean Implements IFlagHolder.GetFlag
        Return CharacterData.Flags.Contains(flagType)
    End Function

    Public Function GetMetadata(identifier As String) As String Implements IMetadataHolder.GetMetadata
        Return CharacterData.Metadatas(identifier)
    End Function
End Class
