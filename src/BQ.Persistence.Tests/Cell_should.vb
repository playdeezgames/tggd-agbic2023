Public Class Cell_should
    Private Function CreateSubject() As ICell
        Dim world As IWorld = New World(New Data.WorldData)
        Return world.CreateMap("MapType", (1, 1), "TerrainType").GetCell(0, 0)
    End Function
    Private Function CreateCharacter(cell As ICell) As ICharacter
        Return cell.Map.World.CreateCharacter("CharacterType", cell)
    End Function
    Private Function CreateItem(cell As ICell) As IItem
        Return cell.Map.World.CreateItem("ItemType")
    End Function
    <Fact>
    Sub hold_statistics()
        Dim subject As ICell = CreateSubject()
        DoStatisticHolderTests(subject)
    End Sub
    <Fact>
    Sub hold_metadata()
        Dim subject As ICell = CreateSubject()
        DoMetadataHolderTests(subject)
    End Sub
    <Fact>
    Sub hold_flags()
        Dim subject As ICell = CreateSubject()
        DoFlagHolderTests(subject)
    End Sub
    <Fact>
    Sub has_properties()
        Dim subject As ICell = CreateSubject()
        subject.HasCharacters.ShouldBeFalse()
        subject.Characters.ShouldBeEmpty()
        subject.Id.ShouldBe(0)
        subject.TopItem.ShouldBeNull()
        subject.HasItems.ShouldBeFalse()
        subject.Items.ShouldBeEmpty()
        subject.HasEffect.ShouldBeFalse()
        subject.Effect.ShouldBeNull()
    End Sub
    <Fact>
    Sub add_and_remove_character()
        Dim subject As ICell = CreateSubject()
        Dim character As ICharacter = CreateCharacter(subject)
        subject.HasCharacters.ShouldBeFalse
        subject.Characters.ShouldBeEmpty
        subject.HasCharacter(character).ShouldBeFalse
        subject.AddCharacter(character)
        subject.HasCharacters.ShouldBeTrue
        subject.Characters.ShouldHaveSingleItem
        subject.OtherCharacters(character).ShouldBeEmpty
        subject.HasCharacter(character).ShouldBeTrue
        subject.RemoveCharacter(character)
        subject.HasCharacters.ShouldBeFalse
        subject.Characters.ShouldBeEmpty
        subject.HasCharacter(character).ShouldBeFalse
    End Sub
    <Fact>
    Sub add_and_remove_item()
        Dim subject As ICell = CreateSubject()
        Dim item As IItem = CreateItem(subject)
        subject.HasItems.ShouldBeFalse
        subject.Items.ShouldBeEmpty
        subject.AddItem(item)
        subject.HasItems.ShouldBeTrue
        subject.Items.ShouldHaveSingleItem
        subject.RemoveItem(item)
        subject.HasItems.ShouldBeFalse
        subject.Items.ShouldBeEmpty
    End Sub
End Class
