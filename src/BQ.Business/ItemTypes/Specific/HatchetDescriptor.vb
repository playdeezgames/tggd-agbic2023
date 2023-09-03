Friend Class HatchetDescriptor
    Inherits ItemTypeDescriptor

    Friend Sub New()
        MyBase.New(
            "Hatchet",
            ChrW(&H3D),
            DarkGray,
            equipSlotType:="Weapon",
            fullName:=Function(x) $"{ItemExtensions.Name(x)}({ItemExtensions.Durability(x)}/{ItemExtensions.MaximumDurability(x)})",
            statistics:=New Dictionary(Of String, Integer) From
            {
                {"AttackDice", 3},
                {"MaximumAttack", 1},
                {StatisticTypes.Durability, 20},
                {StatisticTypes.MaximumDurability, 20}
            },
            flags:=New List(Of String) From
            {
                "IsWeapon"
            })
    End Sub
End Class
