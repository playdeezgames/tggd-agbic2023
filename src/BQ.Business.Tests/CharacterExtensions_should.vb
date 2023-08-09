Imports BQ.Persistence

Public Class CharacterExtensions_should
    <Fact>
    Public Sub check_for_item_type_in_inventory()
        Dim character As New Mock(Of ICharacter)
        CharacterExtensions.HasItemTypeInInventory(character.Object, ItemTypes.Pepper).ShouldBeFalse
        character.VerifyGet(Function(x) x.Items)
        character.VerifyNoOtherCalls()
    End Sub
    <Fact>
    Public Sub deny_sleep_to_nonavatars()
        Dim character As New Mock(Of ICharacter)
        CharacterExtensions.Sleep(character.Object)
        character.VerifyGet(Function(x) x.IsAvatar)
        character.VerifyNoOtherCalls()
    End Sub
    <Fact>
    Public Sub deny_sleep_for_avatars_in_nonwilderness()
        Dim character As New Mock(Of ICharacter)
        Dim map As New Mock(Of IMap)
        Dim world As New Mock(Of IWorld)
        Dim message As New Mock(Of IMessage)

        character.SetupGet(Function(x) x.CharacterType).Returns(CharacterTypes.Loxy)
        character.SetupGet(Function(x) x.IsAvatar).Returns(True)
        character.SetupGet(Function(x) x.Map).Returns(map.Object)
        character.SetupGet(Function(x) x.World).Returns(world.Object)
        world.Setup(Function(x) x.CreateMessage()).Returns(message.Object)

        CharacterExtensions.Sleep(character.Object)

        character.VerifyGet(Function(x) x.IsAvatar)
        character.VerifyGet(Function(x) x.CharacterType)
        map.VerifyGet(Function(x) x.Flag(FlagTypes.CampingAllowed))
        message.Verify(Function(x) x.AddLine(It.IsAny(Of Integer)(), It.IsAny(Of String)()))

        message.VerifyNoOtherCalls()
        world.VerifyNoOtherCalls()
        map.VerifyNoOtherCalls()
        character.VerifyNoOtherCalls()
    End Sub
    <Fact>
    Public Sub allows_sleep_for_avatars_in_wilderness()
        Dim character As New Mock(Of ICharacter)
        Dim map As New Mock(Of IMap)
        Dim world As New Mock(Of IWorld)
        Dim message As New Mock(Of IMessage)
        Dim cell As New Mock(Of ICell)

        character.SetupGet(Function(x) x.Cell).Returns(cell.Object)
        character.Setup(Function(x) x.TryGetStatistic(StatisticTypes.MaximumEnergy)).Returns(10)
        cell.SetupGet(Function(x) x.Map).Returns(map.Object)
        character.SetupGet(Function(x) x.CharacterType).Returns(CharacterTypes.Loxy)
        character.SetupGet(Function(x) x.IsAvatar).Returns(True)
        character.SetupGet(Function(x) x.Map).Returns(map.Object)
        character.SetupGet(Function(x) x.World).Returns(world.Object)
        world.Setup(Function(x) x.CreateMessage()).Returns(message.Object)
        map.SetupGet(Function(x) x.Flag(FlagTypes.CampingAllowed)).Returns(True)
        message.Setup(Function(x) x.AddLine(It.IsAny(Of Integer)(), It.IsAny(Of String)())).Returns(message.Object)

        CharacterExtensions.Sleep(character.Object)

        character.VerifyGet(Function(x) x.IsAvatar)
        character.VerifyGet(Function(x) x.CharacterType)
        character.Verify(Function(x) x.TryGetStatistic(StatisticTypes.MaximumEnergy))
        character.Verify(Function(x) x.TryGetStatistic(StatisticTypes.Energy))
        character.Verify(Function(x) x.TryGetStatistic(StatisticTypes.Peril))
        character.Verify(Sub(x) x.SetStatistic(StatisticTypes.Energy, 5))
        character.Verify(Sub(x) x.SetStatistic(StatisticTypes.Peril, 5))
        map.VerifyGet(Function(x) x.Flag(FlagTypes.CampingAllowed))
        message.Verify(Function(x) x.AddLine(It.IsAny(Of Integer)(), It.IsAny(Of String)()))
        map.Verify(Sub(x) x.GetCell(0, 0))
        cell.VerifyGet(Function(x) x.Column)
        cell.VerifyGet(Function(x) x.Row)
        cell.VerifyGet(Function(x) x.HasOtherCharacters(character.Object))

        message.VerifyNoOtherCalls()
        world.VerifyNoOtherCalls()
        map.VerifyNoOtherCalls()
        character.VerifyNoOtherCalls()
    End Sub
    <Fact>
    Public Sub attempts_to_build_fire()
        Dim character As New Mock(Of ICharacter)
        Dim stickItem As New Mock(Of IItem)
        Dim rockItem As New Mock(Of IItem)
        Dim message As New Mock(Of IMessage)

        stickItem.SetupGet(Function(x) x.ItemType).Returns(ItemTypes.Stick)
        rockItem.SetupGet(Function(x) x.ItemType).Returns(ItemTypes.Rock)
        character.Setup(Function(x) x.World.CreateMessage()).Returns(Message.Object)
        character.SetupGet(Function(x) x.CharacterType).Returns(CharacterTypes.Loxy)
        character.SetupGet(Function(x) x.Items).Returns({stickItem.Object, stickItem.Object, stickItem.Object, stickItem.Object, stickItem.Object, rockItem.Object, rockItem.Object, rockItem.Object, rockItem.Object, rockItem.Object})
        character.Setup(Function(x) x.TryGetStatistic(StatisticTypes.Energy)).Returns(1)
        character.SetupGet(Function(x) x.Cell.TerrainType).Returns(TerrainTypes.Empty)
        character.SetupSet(Sub(x) x.Cell.TerrainType = TerrainTypes.CookingFire).Verifiable()

        CharacterExtensions.DoBuildFire(character.Object)

        character.VerifyGet(Function(x) x.CharacterType)
        character.VerifyGet(Function(x) x.Items)
        character.Verify(Function(x) x.TryGetStatistic(StatisticTypes.Energy))
        character.Verify(Function(x) x.TryGetStatistic(StatisticTypes.MaximumEnergy))
        character.Verify(Sub(x) x.RemoveItem(stickItem.Object), Times.Exactly(5))
        character.Verify(Sub(x) x.RemoveItem(rockItem.Object), Times.Exactly(5))
        character.Verify(Sub(x) x.SetStatistic(StatisticTypes.Energy, 0))
        message.Verify(Function(x) x.AddLine(It.IsAny(Of Integer)(), It.IsAny(Of String)()))
        character.VerifyGet(Function(x) x.Cell.TerrainType)
        character.VerifySet(Sub(x) x.Cell.TerrainType = TerrainTypes.CookingFire)
        stickItem.VerifyGet(Function(x) x.ItemType, Times.Exactly(25))
        stickItem.Verify(Sub(x) x.Recycle(), Times.Exactly(5))
        rockItem.VerifyGet(Function(x) x.ItemType, Times.Exactly(20))
        rockItem.Verify(Sub(x) x.Recycle(), Times.Exactly(5))

        character.VerifyNoOtherCalls()
        rockItem.VerifyNoOtherCalls()
        stickItem.VerifyNoOtherCalls()
        message.VerifyNoOtherCalls()
    End Sub
End Class
