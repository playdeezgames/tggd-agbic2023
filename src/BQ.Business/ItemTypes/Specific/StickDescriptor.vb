Friend Class StickDescriptor
    Inherits ItemTypeDescriptor

    Friend Sub New()
        MyBase.New(
            "Stick",
            ChrW(&H25),
            Brown,
            equipSlotType:=EquipSlotTypes.Weapon,
            fullName:=Function(x) $"{ItemExtensions.Name(x)}({ItemExtensions.Durability(x)}/{ItemExtensions.MaximumDurability(x)})",
            statistics:=New Dictionary(Of String, Integer) From
            {
                {StatisticTypes.AttackDice, 2},
                {StatisticTypes.Durability, 10},
                {StatisticTypes.MaximumDurability, 10}
            },
            flags:=New List(Of String) From
            {
                FlagTypes.IsWeapon
            })
    End Sub
End Class
