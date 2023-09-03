Friend Class StickDescriptor
    Inherits ItemTypeDescriptor

    Friend Sub New()
        MyBase.New(
            "Stick",
            ChrW(&H25),
            Brown,
            equipSlotType:="Weapon",
            fullName:=Function(x) $"{ItemExtensions.Name(x)}({ItemExtensions.Durability(x)}/{ItemExtensions.MaximumDurability(x)})",
            statistics:=New Dictionary(Of String, Integer) From
            {
                {"AttackDice", 2},
                {"Durability", 10},
                {"MaximumDurability", 10}
            },
            flags:=New List(Of String) From
            {
                "IsWeapon"
            })
    End Sub
End Class
