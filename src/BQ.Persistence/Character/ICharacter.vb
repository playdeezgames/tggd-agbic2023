Public Interface ICharacter
    Inherits IStatisticsHolder
    ReadOnly Property Id As Integer
    ReadOnly Property CharacterType As String
    Property Cell As ICell
    ReadOnly Property Map As IMap
    Property Metadata(identifier As String) As String
    Sub RemoveMetadata(identifier As String)
    ReadOnly Property World As IWorld
    Sub Recycle()
    ReadOnly Property Items As IEnumerable(Of IItem)
    ReadOnly Property HasItems As Boolean
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
    Property Flag(flagType As String) As Boolean
End Interface
