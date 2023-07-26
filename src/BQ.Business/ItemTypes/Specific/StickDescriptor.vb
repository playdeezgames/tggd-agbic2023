Friend Class StickDescriptor
    Inherits ItemTypeDescriptor

    Friend Sub New()
        MyBase.New(
            "Stick",
            ChrW(&H25),
            Brown,
            equipSlotType:=EquipSlotTypes.Weapon,
            statistics:=New Dictionary(Of String, Integer) From
            {
                {StatisticTypes.AttackDice, 2}
            },
            flags:=New List(Of String) From
            {
                FlagTypes.IsWeapon
            })
    End Sub
End Class
