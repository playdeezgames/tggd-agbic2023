Imports System.Runtime.CompilerServices
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
    Friend Sub DoVerb(item As IItem, verbType As String, character As ICharacter)
        item.Descriptor.VerbTypes(verbType).Invoke(character, item)
    End Sub
    <Extension>
    Friend Function IsWeapon(item As IItem) As Boolean
        Return item.Descriptor.IsWeapon
    End Function
    <Extension>
    Friend Function IsArmor(item As IItem) As Boolean
        Return item.Descriptor.IsArmor
    End Function
    <Extension>
    Friend Function FullName(item As IItem) As String
        Return item.Descriptor.FullName(item)
    End Function
End Module
