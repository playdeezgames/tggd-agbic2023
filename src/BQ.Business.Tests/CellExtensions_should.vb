﻿Imports System.Security.Cryptography.X509Certificates
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
        map.SetupGet(Function(x) x.Flag(FlagTypes.AllowFireBuilding)).Returns(True)
        subject.SetupGet(Function(x) x.Map).Returns(map.Object)
        CellExtensions.CanBuildFire(subject.Object).ShouldBe(expectedResult)
        subject.VerifyGet(Function(x) x.TerrainType)
        map.VerifyGet(Function(x) x.Flag(FlagTypes.AllowFireBuilding))
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
    <InlineData(TerrainTypes.Tree, True, 1, True, True)>
    <InlineData(TerrainTypes.Tree, True, 0, True, False)>
    <InlineData(TerrainTypes.Tree, False, 0, True, False)>
    <InlineData(TerrainTypes.Empty, False, 0, False, False)>
    Public Sub indicate_foraging_possible_on_terrain_type(givenTerrainType As String, hasForagingRemaining As Boolean, foragingRemaining As Integer, expectStatisticCheck As Boolean, expectedResult As Boolean)
        Dim subject = New Mock(Of ICell)
        subject.SetupGet(Function(x) x.TerrainType).Returns(givenTerrainType)
        subject.Setup(Function(x) x.HasStatistic(It.IsAny(Of String)())).Returns(hasForagingRemaining)
        subject.SetupGet(Function(x) x.Statistic(StatisticTypes.ForageRemaining)).Returns(foragingRemaining)
        CellExtensions.CanForage(subject.Object).ShouldBe(expectedResult)
        subject.VerifyGet(Function(x) x.TerrainType)
        If expectStatisticCheck Then
            subject.Verify(Function(x) x.HasStatistic(StatisticTypes.ForageRemaining))
            If hasForagingRemaining Then
                subject.VerifyGet(Function(x) x.Statistic(StatisticTypes.ForageRemaining))
            End If
        End If
        subject.VerifyNoOtherCalls()
    End Sub
End Class
