Public Interface ICharacter
    Inherits IStatisticsHolder
    Inherits IFlagHolder
    Inherits IMetadataHolder
    ReadOnly Property Id As Integer
    ReadOnly Property CharacterType As String
    Property Cell As ICell
    ReadOnly Property Map As IMap
    ReadOnly Property World As IWorld
    Sub Recycle()
    ReadOnly Property Items As IEnumerable(Of IItem)
    ReadOnly Property HasItems As Boolean
    ReadOnly Property ItemCount As Integer
    Sub RemoveItem(item As IItem)
    Sub AddItem(item As IItem)
    ReadOnly Property IsAvatar As Boolean
    ReadOnly Property HasEquipment As Boolean
    Sub Equip(equipSlotType As String, item As IItem)
    Sub Unequip(equipSlotType As String)
    Sub UnequipItem(item As IItem)
    Function ItemTypeCount(itemType As String) As Integer
    ReadOnly Property Equipment As IReadOnlyDictionary(Of String, IItem)
    ReadOnly Property EquippedItems As IReadOnlyList(Of IItem)
End Interface
