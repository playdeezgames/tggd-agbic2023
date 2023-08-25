Public Module ItemInitializer
    Public Function CreateItem(world As IWorld, itemType As String) As IItem
        Dim item = world.CreateItem(itemType)
        For Each entry In itemType.ToItemTypeDescriptor.Statistics
            item.SetStatistic(entry.Key, entry.Value)
        Next
        Return item
    End Function
End Module
