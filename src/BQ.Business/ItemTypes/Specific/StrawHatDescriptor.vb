Friend Class StrawHatDescriptor
    Inherits ItemTypeDescriptor

    Friend Sub New()
        MyBase.New(
            "Straw Hat",
            ChrW(&H3B),
            Yellow,
            equipSlotType:="Head",
            fullName:=Function(x) $"{ItemExtensions.Name(x)}({ItemExtensions.Durability(x)}/{ItemExtensions.MaximumDurability(x)})",
            statistics:=New Dictionary(Of String, Integer) From
            {
                {"DefendDice", 1},
                {"MaximumDefend", 0},
                {StatisticTypes.Durability, 5},
                {StatisticTypes.MaximumDurability, 5}
            },
            flags:=New List(Of String) From
            {
                "IsArmor"
            })
    End Sub
End Class
