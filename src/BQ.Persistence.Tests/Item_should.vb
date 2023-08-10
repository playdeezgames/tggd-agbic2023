Public Class Item_should
    Const ItemType = "ItemType"
    Private Function CreateSubject() As IItem
        Dim world As IWorld = New World(New Data.WorldData)
        Return world.CreateItem(ItemType)
    End Function
    <Fact>
    Sub hold_statistics()
        Dim subject As IItem = CreateSubject()
        DoStatisticHolderTests(subject)
    End Sub
    <Fact>
    Sub hold_metadata()
        Dim subject As IItem = CreateSubject()
        DoMetadataHolderTests(subject)
    End Sub
    <Fact>
    Sub hold_flags()
        Dim subject As IItem = CreateSubject()
        DoFlagHolderTests(subject)
    End Sub
    <Fact>
    Sub has_properties()
        Dim subject As IItem = CreateSubject()
        subject.ItemType.ShouldBe(ItemType)
        subject.Id.ShouldBe(0)
    End Sub
    <Fact>
    Sub recycle()
        Dim subject As IItem = CreateSubject()
        subject.Recycle()
        subject = CreateSubject()
        subject.Id.ShouldBe(0)
    End Sub
End Class
