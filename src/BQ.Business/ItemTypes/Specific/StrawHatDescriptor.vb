﻿Friend Class StrawHatDescriptor
    Inherits ItemTypeDescriptor

    Friend Sub New()
        MyBase.New(
            "Straw Hat",
            ChrW(&H3B),
            Yellow,
            equipSlotType:=EquipSlotTypes.Head,
            statistics:=New Dictionary(Of String, Integer) From
            {
                {StatisticTypes.DefendDice, 1},
                {StatisticTypes.MaximumDefend, 0},
                {StatisticTypes.Durability, 5},
                {StatisticTypes.MaximumDurability, 5}
            },
            flags:=New List(Of String) From
            {
                FlagTypes.IsArmor
            })
    End Sub
End Class