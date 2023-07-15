Public Interface IAvatarEquipmentModel
    ReadOnly Property Exists As Boolean
    ReadOnly Property EquippedSlots As IEnumerable(Of (String, String))
    Property SelectedSlot As String
    ReadOnly Property SelectedEquippedItemFullName As String
    Sub UnequipSelected()
End Interface
