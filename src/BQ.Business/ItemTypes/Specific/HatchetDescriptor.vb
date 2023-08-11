Friend Class HatchetDescriptor
    Inherits ItemTypeDescriptor

    Friend Sub New()
        MyBase.New(
            "Hatchet",
            ChrW(&H3D),
            DarkGray,
            equipSlotType:=EquipSlotTypes.Weapon,
            fullName:=Function(x) $"{x.Name}({x.Durability}/{x.MaximumDurability})",
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
