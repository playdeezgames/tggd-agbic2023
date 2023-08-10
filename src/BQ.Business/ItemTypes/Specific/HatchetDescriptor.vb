﻿Friend Class HatchetDescriptor
    Inherits ItemTypeDescriptor

    Friend Sub New()
        MyBase.New(
            "Hatchet",
            ChrW(&H3D),
            DarkGray,
            equipSlotType:=EquipSlotTypes.Weapon,
            statistics:=New Dictionary(Of String, Integer) From
            {
                {StatisticTypes.AttackDice, 3},
                {StatisticTypes.MaximumAttack, 1}
            },
            flags:=New List(Of String) From
            {
                FlagTypes.IsWeapon
            })
    End Sub
End Class