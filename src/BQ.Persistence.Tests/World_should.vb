Public Class World_should
    <Fact>
    Sub hold_statistics()
        Dim subject As IWorld = New World(New Data.WorldData)
        DoStatisticHolderTests(subject)
    End Sub

    <Fact>
    Sub hold_metadata()
        Dim subject As IWorld = New World(New Data.WorldData)
        DoMetadataHolderTests(subject)
    End Sub

    <Fact>
    Sub hold_flags()
        Dim subject As IWorld = New World(New Data.WorldData)
        DoFlagHolderTests(subject)
    End Sub

    <Fact>
    Sub create_map()
        Const mapType = "MapType"
        Const columns = 3
        Const rows = 2
        Const terrainType = "TerrainType"
        Dim subject As IWorld = New World(New Data.WorldData)
        Dim actual = subject.CreateMap(mapType, (columns, rows), terrainType)
        actual.ShouldNotBeNull
        actual.Columns.ShouldBe(columns)
        actual.Rows.ShouldBe(rows)
        actual.Cells.Count.ShouldBe(columns * rows)
        actual.Cells.ShouldAllBe(Function(x) x.TerrainType = terrainType)
        subject.Maps.ShouldHaveSingleItem
        subject.GetMap(0).Id.ShouldBe(actual.Id)
    End Sub
    <Fact>
    Sub create_character()
        Dim subject As IWorld = New World(New Data.WorldData)
        Dim map = subject.CreateMap("MapType", (3, 2), "TerrainType")
        Dim actual = subject.CreateCharacter("CharacterType", map.GetCell(2, 1))
        actual.ShouldNotBeNull
        actual.Cell.Column.ShouldBe(2)
        actual.Cell.Row.ShouldBe(1)
        subject.Characters.ShouldHaveSingleItem
        subject.GetCharacter(0).Id.ShouldBe(actual.Id)
    End Sub
    <Fact>
    Sub begin_blank()
        Dim subject As IWorld = New World(New Data.WorldData)
        subject.Avatar.ShouldBeNull
        subject.Maps.ShouldBeEmpty
        subject.Characters.ShouldBeEmpty
        subject.CurrentMessage.ShouldBeNull
        subject.HasMessages.ShouldBeFalse
    End Sub
    <Fact>
    Sub create_message()
        Dim subject As IWorld = New World(New Data.WorldData)
        Dim actual = subject.CreateMessage
        actual.ShouldNotBeNull
        subject.CurrentMessage.ShouldNotBeNull
        subject.HasMessages.ShouldBeTrue
    End Sub
    <Fact>
    Sub create_item()
        Dim subject As IWorld = New World(New Data.WorldData)
        Dim actual = subject.CreateItem("ItemType")
        actual.ShouldNotBeNull
        subject.GetItem(0).Id.ShouldBe(actual.Id)
    End Sub
    <Fact>
    Sub dismiss_message()
        Dim subject As IWorld = New World(New Data.WorldData)
        subject.CreateMessage()
        subject.CurrentMessage.ShouldNotBeNull
        subject.HasMessages.ShouldBeTrue
        subject.DismissMessage()
        subject.CurrentMessage.ShouldBeNull
        subject.HasMessages.ShouldBeFalse
    End Sub
    <Fact>
    Sub serialize_data()
        Dim subject As IWorld = New World(New Data.WorldData)
        Dim actual = subject.SerializedData
        actual.ShouldBe("{""Maps"":[],""Characters"":[],""AvatarCharacterId"":null,""Messages"":[],""Items"":[],""Statistics"":{},""Flags"":[],""Metadatas"":{}}")
    End Sub
End Class


