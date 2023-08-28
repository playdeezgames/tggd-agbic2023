Public Module ItemExtensions
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
    Public Function FullName(item As IItem) As String
        Return ItemExtensions.Descriptor(item).FullName(item)
    End Function
    <Extension>
    Public Function CanEquip(item As IItem) As Boolean
        Return ItemExtensions.Descriptor(item).CanEquip
    End Function
    <Extension>
    Public Function AttackDice(item As IItem) As Integer
        Return item.GetStatistic(StatisticTypes.AttackDice)
    End Function
    <Extension>
    Public Function MaximumAttack(item As IItem) As Integer
        Return item.GetStatistic(StatisticTypes.MaximumAttack)
    End Function
    <Extension>
    Public Function DefendDice(item As IItem) As Integer
        Return item.GetStatistic(StatisticTypes.DefendDice)
    End Function
    <Extension>
    Public Function MaximumDefend(item As IItem) As Integer
        Return item.GetStatistic(StatisticTypes.MaximumDefend)
    End Function
    <Extension>
    Public Function Durability(item As IItem) As Integer
        Return item.GetStatistic(StatisticTypes.Durability)
    End Function
    <Extension>
    Public Sub SetDurability(item As IItem, durability As Integer)
        item.SetStatistic(StatisticTypes.Durability, Math.Clamp(durability, 0, item.MaximumDurability))
    End Sub
    <Extension>
    Public Sub AddDurability(item As IItem, delta As Integer)
        item.SetDurability(item.Durability + delta)
    End Sub
    <Extension>
    Public Function MaximumDurability(item As IItem) As Integer
        Return item.GetStatistic(StatisticTypes.MaximumDurability)
    End Function
    <Extension>
    Public Function IsBroken(item As IItem) As Boolean
        Return item.Durability <= 0
    End Function
    <Extension>
    Public Function Glyph(item As IItem) As String
        Return ItemExtensions.Descriptor(item).Glyph
    End Function
    <Extension>
    Public Function Hue(item As IItem) As Integer
        Return ItemExtensions.Descriptor(item).Hue
    End Function
End Module
