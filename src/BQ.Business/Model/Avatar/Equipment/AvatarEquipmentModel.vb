Friend Class AvatarEquipmentModel
    Implements IAvatarEquipmentModel

    Private ReadOnly avatar As ICharacter

    Public Sub New(avatar As ICharacter)
        Me.avatar = avatar
    End Sub

    Public ReadOnly Property Exists As Boolean Implements IAvatarEquipmentModel.Exists
        Get
            Return avatar.HasEquipment
        End Get
    End Property

    Public ReadOnly Property Display As IEnumerable(Of (String, String)) Implements IAvatarEquipmentModel.Display
        Get
            Return avatar.Equipment.Select(Function(x) ($"{x.Key.ToEquipSlotTypeDescriptor.Name}: {ItemExtensions.FullName(x.Value)}", x.Key))
        End Get
    End Property

    Public Sub Unequip(equipSlotType As String) Implements IAvatarEquipmentModel.Unequip
        avatar.Unequip(equipSlotType)
        avatar.
            World.
            CreateMessage().
            AddLine(7, $"{CharacterExtensions.Name(avatar)} unequips {equipSlotType.ToEquipSlotTypeDescriptor.Name}")
    End Sub
End Class
