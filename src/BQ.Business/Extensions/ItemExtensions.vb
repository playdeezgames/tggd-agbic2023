﻿Public Module ItemExtensions
    Public Function Descriptor(item As IItem) As ItemTypeDescriptor
        Return item.ItemType.ToItemTypeDescriptor
    End Function
    Public Function Name(item As IItem) As String
        Return ItemExtensions.Descriptor(item).Name
    End Function
    Public Function IsWeapon(item As IItem) As Boolean
        Return ItemExtensions.Descriptor(item).Flags.Contains(FlagTypes.IsWeapon)
    End Function
    Public Function IsArmor(item As IItem) As Boolean
        Return ItemExtensions.Descriptor(item).Flags.Contains(FlagTypes.IsArmor)
    End Function
    Public Function FullName(luaState As Lua, item As IItem) As String
        Return ItemExtensions.Descriptor(item).FullName(luaState, item)
    End Function
    Public Function CanEquip(item As IItem) As Boolean
        Return ItemExtensions.Descriptor(item).CanEquip
    End Function
    Public Function AttackDice(item As IItem) As Integer
        Return item.GetStatistic(StatisticTypes.AttackDice)
    End Function
    Public Function MaximumAttack(item As IItem) As Integer
        Return item.GetStatistic(StatisticTypes.MaximumAttack)
    End Function
    Public Function DefendDice(item As IItem) As Integer
        Return item.GetStatistic(StatisticTypes.DefendDice)
    End Function
    Public Function MaximumDefend(item As IItem) As Integer
        Return item.GetStatistic(StatisticTypes.MaximumDefend)
    End Function
    Public Function Durability(item As IItem) As Integer
        Return item.GetStatistic(StatisticTypes.Durability)
    End Function
    Public Sub SetDurability(item As IItem, durability As Integer)
        item.SetStatistic(StatisticTypes.Durability, Math.Clamp(durability, 0, ItemExtensions.MaximumDurability(item)))
    End Sub
    Public Sub AddDurability(item As IItem, delta As Integer)
        ItemExtensions.SetDurability(item, ItemExtensions.Durability(item) + delta)
    End Sub
    Public Function MaximumDurability(item As IItem) As Integer
        Return item.GetStatistic(StatisticTypes.MaximumDurability)
    End Function
    Public Function IsBroken(item As IItem) As Boolean
        Return ItemExtensions.Durability(item) <= 0
    End Function
    Public Function Glyph(item As IItem) As String
        Return ItemExtensions.Descriptor(item).Glyph
    End Function
    Public Function Hue(item As IItem) As Integer
        Return ItemExtensions.Descriptor(item).Hue
    End Function
End Module
