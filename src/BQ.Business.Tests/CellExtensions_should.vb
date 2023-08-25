Imports System.Security.Cryptography.X509Certificates
Imports BQ.Persistence
Imports Moq
Imports Shouldly
Imports Xunit

Public Class CellExtensions_should
    <Theory>
    <InlineData(TerrainTypes.Empty, True)>
    <InlineData(TerrainTypes.CookingFire, False)>
    Public Sub indicate_when_cell_can_build_fire(givenTerrainType As String, expectedResult As Boolean)
        Dim subject = New Mock(Of ICell)
        subject.SetupGet(Function(x) x.TerrainType).Returns(givenTerrainType)
        Dim map = New Mock(Of IMap)
        map.SetupGet(Function(x) x.GetFlag(FlagTypes.AllowFireBuilding)).Returns(True)
        subject.SetupGet(Function(x) x.Map).Returns(map.Object)
        CellExtensions.CanBuildFire(subject.Object).ShouldBe(expectedResult)
        subject.VerifyGet(Function(x) x.TerrainType)
        map.VerifyGet(Function(x) x.GetFlag(FlagTypes.AllowFireBuilding))
        subject.VerifyNoOtherCalls()
        map.VerifyNoOtherCalls()
    End Sub
    <Theory>
    <InlineData(TerrainTypes.CookingFire, True)>
    <InlineData(TerrainTypes.Wall, False)>
    Public Sub indicate_when_cell_has_fire(givenTerrainType As String, expectedResult As Boolean)
        Dim subject = New Mock(Of ICell)
        subject.SetupGet(Function(x) x.TerrainType).Returns(givenTerrainType)
        CellExtensions.HasFire(subject.Object).ShouldBe(expectedResult)
        subject.VerifyGet(Function(x) x.TerrainType)
        subject.VerifyNoOtherCalls()
    End Sub
    <Theory>
    <InlineData(TerrainTypes.CookingFire, True)>
    <InlineData(TerrainTypes.Wall, False)>
    Public Sub indicate_when_cell_can_make_torch(givenTerrainType As String, expectedResult As Boolean)
        Dim subject = New Mock(Of ICell)
        subject.SetupGet(Function(x) x.TerrainType).Returns(givenTerrainType)
        CellExtensions.CanMakeTorch(subject.Object).ShouldBe(expectedResult)
        subject.VerifyGet(Function(x) x.TerrainType)
        subject.VerifyNoOtherCalls()
    End Sub
    <Theory>
    <InlineData(TerrainTypes.Tree, 0, ItemTypes.Stick)>
    <InlineData(TerrainTypes.Grass, 0, ItemTypes.PlantFiber)>
    Public Sub generate_forage_appropriate_to_terrain_type(givenTerrainType As String, givenSeed As Integer, expectedItemType As String)
        Dim random As New Random(givenSeed)
        Dim subject = New Mock(Of ICell)
        subject.SetupGet(Function(x) x.TerrainType).Returns(givenTerrainType)
        CellExtensions.GenerateForageItemType(subject.Object, random).ShouldBe(expectedItemType)
        subject.VerifyGet(Function(x) x.TerrainType)
        subject.VerifyNoOtherCalls()
    End Sub
    <Theory>
    <InlineData(TerrainTypes.Tree, 1, True, True)>
    <InlineData(TerrainTypes.Tree, 0, True, False)>
    <InlineData(TerrainTypes.Tree, 0, True, False)>
    <InlineData(TerrainTypes.Empty, 0, False, False)>
    Public Sub indicate_foraging_possible_on_terrain_type(givenTerrainType As String, foragingRemaining As Integer, expectStatisticCheck As Boolean, expectedResult As Boolean)
        Dim subject = New Mock(Of ICell)
        subject.SetupGet(Function(x) x.TerrainType).Returns(givenTerrainType)
        subject.Setup(Function(x) x.TryGetStatistic(StatisticTypes.ForageRemaining)).Returns(foragingRemaining)
        CellExtensions.CanForage(subject.Object).ShouldBe(expectedResult)
        subject.VerifyGet(Function(x) x.TerrainType)
        If expectStatisticCheck Then
            subject.Verify(Function(x) x.TryGetStatistic(StatisticTypes.ForageRemaining))
        End If
        subject.VerifyNoOtherCalls()
    End Sub
    <Theory>
    <InlineData(TerrainTypes.Tree, True)>
    <InlineData(TerrainTypes.Wall, False)>
    Public Sub indicate_tenability(givenTerrainType As String, expectedResult As Boolean)
        Dim subject = New Mock(Of ICell)
        subject.SetupGet(Function(x) x.TerrainType).Returns(givenTerrainType)
        CellExtensions.IsTenable(subject.Object).ShouldBe(expectedResult)
        subject.VerifyGet(Function(x) x.TerrainType)
        subject.VerifyNoOtherCalls()
    End Sub
    <Theory>
    <InlineData(TerrainTypes.Tree, 1)>
    <InlineData(TerrainTypes.Wall, 0)>
    Public Sub indicate_peril(givenTerrainType As String, expectedResult As Integer)
        Dim subject = New Mock(Of ICell)
        subject.SetupGet(Function(x) x.TerrainType).Returns(givenTerrainType)
        CellExtensions.Peril(subject.Object).ShouldBe(expectedResult)
        subject.VerifyGet(Function(x) x.TerrainType)
        subject.VerifyNoOtherCalls()
    End Sub
    <Theory>
    <InlineData(TerrainTypes.Tree, EffectTypes.PutOutFire, CharacterTypes.Loxy)>
    Public Sub do_nonexistant_effect_on_terrain(givenTerrainType As String, givenEffectType As String, givenCharacterType As String)
        Dim character As New Mock(Of ICharacter)
        Dim world As New Mock(Of IWorld)
        Dim message As New Mock(Of IMessage)
        message.Setup(Function(x) x.SetSfx(It.IsAny(Of String)())).Returns(message.Object)
        world.Setup(Function(x) x.CreateMessage()).Returns(message.Object)
        character.SetupGet(Function(x) x.CharacterType).Returns(givenCharacterType)
        character.SetupGet(Function(x) x.World).Returns(world.Object)
        Dim subject = New Mock(Of ICell)
        subject.SetupGet(Function(x) x.TerrainType).Returns(givenTerrainType)
        CellExtensions.DoEffect(subject.Object, givenEffectType, character.Object)
        subject.VerifyGet(Function(x) x.TerrainType)
        message.Verify(Function(x) x.AddLine(It.IsAny(Of Integer)(), It.IsAny(Of String)()))
        character.VerifyNoOtherCalls()
        world.VerifyNoOtherCalls()
        message.VerifyNoOtherCalls()
        subject.VerifyNoOtherCalls()
    End Sub
End Class
