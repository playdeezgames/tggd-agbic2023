Friend Class HatchetDescriptor
    Inherits ItemTypeDescriptor

    Friend Sub New()
        MyBase.New(
            "Hatchet",
            ChrW(&H3D),
            8,
            equipSlotType:="Weapon",
            fullName:=Function(x) $"{ItemExtensions.Name(x)}({ItemExtensions.Durability(x)}/{ItemExtensions.MaximumDurability(x)})",
            statistics:=New Dictionary(Of String, Integer) From
            {
                {"AttackDice", 3},
                {"MaximumAttack", 1},
                {"Durability", 20},
                {"MaximumDurability", 20}
            },
            flags:=New List(Of String) From
            {
                "IsWeapon"
            })
    End Sub
End Class
