Public Interface ICharacter
    ReadOnly Property Id As Integer
    ReadOnly Property CharacterType As String
    Property Cell As ICell
    ReadOnly Property Map As IMap
    Property Statistic(statisticType As String) As Integer
    Property Metadata(identifier As String) As String
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
    Sub RemoveStatistic(statisticType As String)
    Function HasStatistic(statisticType As String) As Boolean
    ReadOnly Property Equipment As IReadOnlyDictionary(Of String, IItem)
    ReadOnly Property EquippedItems As IReadOnlyList(Of IItem)
End Interface
