﻿Imports System.Runtime.CompilerServices
Imports BQ.Persistence

Friend Module ItemExtensions
    <Extension>
    Friend Function Descriptor(item As IItem) As ItemTypeDescriptor
        Return item.ItemType.ToItemTypeDescriptor
    End Function
    <Extension>
    Friend Function Name(item As IItem) As String
        Return item.Descriptor.Name
    End Function
    <Extension>
    Friend Function TryGetStatistic(item As IItem, statisticType As String, Optional defaultValue As Integer = 0) As Integer
        Return If(item.HasStatistic(statisticType), item.Statistic(statisticType), defaultValue)
    End Function
    <Extension>
    Friend Sub DoEffect(item As IItem, effectType As String, character As ICharacter)
        Dim effect = item.Descriptor.ToItemEffect(effectType, item)
        character.Descriptor.EffectHandlers(effectType).Invoke(character, effect)
    End Sub
    <Extension>
    Friend Function IsWeapon(item As IItem) As Boolean
        Return item.Descriptor.Flags.Contains(FlagTypes.IsWeapon)
    End Function
    <Extension>
    Friend Function IsArmor(item As IItem) As Boolean
        Return item.Descriptor.Flags.Contains(FlagTypes.IsArmor)
    End Function
    <Extension>
    Friend Function FullName(item As IItem) As String
        Return item.Descriptor.FullName(item)
    End Function
    <Extension>
    Friend Function CanEquip(item As IItem) As Boolean
        Return item.Descriptor.CanEquip
    End Function
    <Extension>
    Friend Function AttackDice(item As IItem) As Integer
        Return item.TryGetStatistic(StatisticTypes.AttackDice)
    End Function
    <Extension>
    Friend Function MaximumAttack(item As IItem) As Integer
        Return item.TryGetStatistic(StatisticTypes.MaximumAttack)
    End Function
End Module
