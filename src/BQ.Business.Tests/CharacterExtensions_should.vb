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
End Class
