Public Interface IAvatarEquipmentModel
    ReadOnly Property Exists As Boolean
    ReadOnly Property Display As IEnumerable(Of (String, String))
    Sub Unequip(equipSlotType As String)
End Interface
