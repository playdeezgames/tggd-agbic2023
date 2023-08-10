Imports System
Imports Shouldly
Imports Xunit

Public Class World_should
    <Fact>
    Sub hold_statistics()
        Const statisticType = "StatisticType"
        Const statisticValue = 10
        Dim subject As IWorld = New World(New Data.WorldData)
        subject.HasStatistic(statisticType).ShouldBeFalse
        subject.TryGetStatistic(statisticType).ShouldBe(0)

        subject.SetStatistic(statisticType, statisticValue)
        subject.HasStatistic(statisticType).ShouldBeTrue
        subject.Statistic(statisticType).ShouldBe(statisticValue)
        subject.TryGetStatistic(statisticType).ShouldBe(statisticValue)

        subject.RemoveStatistic(statisticType)
        subject.HasStatistic(statisticType).ShouldBeFalse
        subject.TryGetStatistic(statisticType).ShouldBe(0)
    End Sub
    <Fact>
    Sub hold_metadata()
        Const metadataIdentifier = "MetadataIdentifier"
        Const metadataValue = "MetadataValue"
        Dim subject As IWorld = New World(New Data.WorldData)
        subject.HasMetadata(metadataIdentifier).ShouldBeFalse
        subject.SetMetadata(metadataIdentifier, metadataValue)
        subject.HasMetadata(metadataIdentifier).ShouldBeTrue
        subject.Metadata(metadataIdentifier).ShouldBe(metadataValue)
        subject.RemoveMetadata(metadataIdentifier)
        subject.HasMetadata(metadataIdentifier).ShouldBeFalse
    End Sub
    <Fact>
    Sub hold_flags()
        Const flagType = "FlagType"
        Dim subject As IWorld = New World(New Data.WorldData)
        subject.Flag(flagType).ShouldBeFalse
        subject.Flag(flagType) = True
        subject.Flag(flagType).ShouldBeTrue
        subject.Flag(flagType) = False
        subject.Flag(flagType).ShouldBeFalse
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
        subject.Map(0).Id.ShouldBe(actual.Id)
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
        subject.Character(0).Id.ShouldBe(actual.Id)
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
        subject.Item(0).Id.ShouldBe(actual.Id)
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
End Class


