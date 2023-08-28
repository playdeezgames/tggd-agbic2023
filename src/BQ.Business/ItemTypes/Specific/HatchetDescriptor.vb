Friend Class HatchetDescriptor
    Inherits ItemTypeDescriptor

    Friend Sub New()
        MyBase.New(
            "Hatchet",
            ChrW(&H3D),
            DarkGray,
            equipSlotType:=EquipSlotTypes.Weapon,
            fullName:=Function(x) $"{ItemExtensions.Name(x)}({ItemExtensions.Durability(x)}/{ItemExtensions.MaximumDurability(x)})",
            statistics:=New Dictionary(Of String, Integer) From
            {
                {StatisticTypes.AttackDice, 3},
                {StatisticTypes.MaximumAttack, 1},
                {StatisticTypes.Durability, 20},
                {StatisticTypes.MaximumDurability, 20}
            },
            flags:=New List(Of String) From
            {
                FlagTypes.IsWeapon
            })
    End Sub
End Class
