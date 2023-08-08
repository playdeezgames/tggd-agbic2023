Imports BQ.Persistence

Public Class ConsumableEffectHandlers_should
    <Fact>
    Public Sub allow_eating_rat_corpses()
        Dim character As New Mock(Of ICharacter)
        character.SetupGet(Function(x) x.CharacterType).Returns(CharacterTypes.Loxy)
        character.SetupGet(Function(x) x.Statistic(StatisticTypes.Health)).Returns(1)
        character.SetupGet(Function(x) x.Statistic(StatisticTypes.MaximumHealth)).Returns(2)
        Dim effect As New Mock(Of IItemEffect)
        Dim item As New Mock(Of IItem)
        item.SetupGet(Function(x) x.ItemType).Returns(ItemTypes.RatCorpse)
        Dim world As New Mock(Of IWorld)
        Dim message As New Mock(Of IMessage)
        message.Setup(Function(x) x.AddLine(It.IsAny(Of Integer), It.IsAny(Of String))).Returns(message.Object)
        world.Setup(Function(x) x.CreateMessage()).Returns(Message.Object)
        character.SetupGet(Function(x) x.World).Returns(world.Object)
        effect.SetupGet(Function(x) x.Item).Returns(item.Object)

        ConsumableEffectHandlers.DoEatRatCorpse(character.Object, effect.Object)

        item.Verify(Sub(x) x.Recycle())
        item.VerifyGet(Function(x) x.ItemType)
        character.Verify(Sub(x) x.RemoveItem(It.IsAny(Of IItem)))
        character.Verify(Function(x) x.TryGetStatistic(StatisticTypes.Health, 0))
        character.Verify(Function(x) x.TryGetStatistic(StatisticTypes.MaximumHealth, 0))
        character.VerifyGet(Function(x) x.CharacterType)
        character.VerifyGet(Function(x) x.World)
        character.Verify(Sub(x) x.SetStatistic(StatisticTypes.Health, It.IsAny(Of Integer)))

        message.VerifyNoOtherCalls()
        item.VerifyNoOtherCalls()
        world.VerifyNoOtherCalls()
        effect.VerifyNoOtherCalls()
        character.VerifyNoOtherCalls()
    End Sub
End Class
