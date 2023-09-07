Friend Module WorldInitializer
    Friend Sub Initialize(world As IWorld)
        MapInitializer.Initialize(world)
        StitchMaps(world)
        AvatarInitializer.Initialize(world)
    End Sub
    Private Sub StitchMaps(world As IWorld)
        StitchTownToWilderness(world)
        StitchHealerToTown(world)
        StitchHealthTrainerToTown(world)
        StitchDruidHouseToTown(world)
        StitchInnToTown(world)
        StitchEnergyTrainerToTown(world)
        StitchPotterToTown(world)
    End Sub

    Private Sub StitchPotterToTown(world As IWorld)
        Dim townMap = WorldExtensions.GetSingleMapByMapType(world, MapTypes.Town)
        Dim potterMap = WorldExtensions.GetSingleMapByMapType(world, MapTypes.Potter)
        FromTownToPotter(townMap, potterMap)
        FromPotterToTown(townMap, potterMap)
    End Sub

    Private Sub FromPotterToTown(townMap As IMap, potterMap As IMap)
        Dim cell As ICell = potterMap.GetCell(PotterColumns \ 2, PotterRows - 1)
        cell.Effect = potterMap.CreateEffect()
        TriggerExtensions.SetEffectType(cell.Effect, "Teleport")
        TriggerExtensions.SetDestination(cell.Effect, townMap.GetCell(13, 12))
    End Sub

    Private Sub FromTownToPotter(townMap As IMap, potterMap As IMap)
        townMap.GetCell(13, 13).Effect =
            townMap.CreateEffect()
        TriggerExtensions.SetEffectType(townMap.GetCell(13, 13).Effect, "Teleport")
        TriggerExtensions.SetDestination(townMap.GetCell(13, 13).Effect, potterMap.GetCell(PotterColumns \ 2, PotterRows - 2))
    End Sub

    Private Sub StitchEnergyTrainerToTown(world As IWorld)
        Dim townMap = WorldExtensions.GetSingleMapByMapType(world, MapTypes.Town)
        Dim trainerMap = WorldExtensions.GetSingleMapByMapType(world, MapTypes.EnergyTrainer)
        FromTownToEnergyTrainer(townMap, trainerMap)
        FromEnergyTrainerToTown(townMap, trainerMap)
    End Sub

    Private Sub FromEnergyTrainerToTown(townMap As IMap, trainerMap As IMap)
        Dim cell As ICell = trainerMap.GetCell(EnergyTrainerColumns \ 2, 0)
        cell.Effect = trainerMap.CreateEffect()
        TriggerExtensions.SetEffectType(cell.Effect, "Teleport")
        TriggerExtensions.SetDestination(cell.Effect, townMap.GetCell(6, 5))
    End Sub

    Private Sub FromTownToEnergyTrainer(townMap As IMap, trainerMap As IMap)
        Dim cell As ICell = townMap.GetCell(6, 6)
        cell.Effect = townMap.CreateEffect()
        TriggerExtensions.SetEffectType(cell.Effect, "Teleport")
        TriggerExtensions.SetDestination(cell.Effect, trainerMap.GetCell(EnergyTrainerColumns \ 2, 1))
    End Sub

    Private Sub StitchInnToTown(world As IWorld)
        Dim townMap = WorldExtensions.GetSingleMapByMapType(world, MapTypes.Town)
        Dim innMap = WorldExtensions.GetSingleMapByMapType(world, MapTypes.Inn)
        Dim cellarMap = WorldExtensions.GetSingleMapByMapType(world, MapTypes.Cellar)
        FromTownToInn(townMap, innMap)
        FromInnToTown(townMap, innMap)
        TransitionsBetweenInnAndCellar(innMap, cellarMap)
    End Sub

    Private Sub TransitionsBetweenInnAndCellar(innMap As IMap, cellarMap As IMap)
        Dim downStairs = innMap.Cells.Single(Function(x) x.TerrainType = "StairsDown") 'TODO: 
        Dim upStairs = cellarMap.Cells.Single(Function(x) x.TerrainType = "StairsUp") 'TODO: 

        TriggerExtensions.SetDestination(downStairs.Effect, upStairs)
        TriggerExtensions.SetDestination(upStairs.Effect, downStairs)
    End Sub

    Private Sub FromInnToTown(townMap As IMap, innMap As IMap)
        Dim cell As ICell = innMap.GetCell(InnColumns \ 2, InnRows - 1)
        cell.Effect = innMap.CreateEffect()
        TriggerExtensions.SetEffectType(cell.Effect, "Teleport")
        TriggerExtensions.SetDestination(cell.Effect, townMap.GetCell(3, 2))
    End Sub

    Private Sub FromTownToInn(townMap As IMap, innMap As IMap)
        Dim cell As ICell = townMap.GetCell(3, 3)
        cell.Effect = townMap.CreateEffect()
        TriggerExtensions.SetEffectType(cell.Effect, "Teleport")
        TriggerExtensions.SetDestination(cell.Effect, innMap.GetCell(InnColumns \ 2, InnRows - 2))
    End Sub

    Private Sub StitchDruidHouseToTown(world As IWorld)
        Dim townMap = WorldExtensions.GetSingleMapByMapType(world, MapTypes.Town)
        Dim druidMap = WorldExtensions.GetSingleMapByMapType(world, MapTypes.DruidHouse)
        FromTownToDruid(townMap, druidMap)
        FromDruidToTown(townMap, druidMap)
    End Sub

    Private Sub FromDruidToTown(townMap As IMap, druidMap As IMap)
        Dim cell As ICell = druidMap.GetCell(0, DruidHouseRows \ 2)
        cell.Effect =
                    druidMap.CreateEffect()
        TriggerExtensions.SetEffectType(cell.Effect, "Teleport")
        TriggerExtensions.SetDestination(cell.Effect, townMap.GetCell(9, 10))
    End Sub

    Private Sub FromTownToDruid(townMap As IMap, druidMap As IMap)
        Dim cell As ICell = townMap.GetCell(10, 10)
        cell.Effect =
            townMap.CreateEffect()
        TriggerExtensions.SetEffectType(cell.Effect, "Teleport")
        TriggerExtensions.SetDestination(cell.Effect, druidMap.GetCell(1, DruidHouseRows \ 2))
    End Sub

    Private Sub StitchHealthTrainerToTown(world As IWorld)
        Dim townMap = WorldExtensions.GetSingleMapByMapType(world, MapTypes.Town)
        Dim trainerMap = WorldExtensions.GetSingleMapByMapType(world, MapTypes.HealthTrainer)
        FromTownToHealthTrainer(townMap, trainerMap)
        FromHealthTrainerToTown(townMap, trainerMap)
    End Sub

    Private Sub FromHealthTrainerToTown(townMap As IMap, trainerMap As IMap)
        Dim cell As ICell = trainerMap.GetCell(HealthTrainerColumns \ 2, HealthTrainerRows - 1)
        cell.Effect =
                    trainerMap.CreateEffect()
        TriggerExtensions.SetEffectType(cell.Effect, "Teleport")
        TriggerExtensions.SetDestination(cell.Effect, townMap.GetCell(8, 3))
    End Sub

    Private Sub FromTownToHealthTrainer(townMap As IMap, trainerMap As IMap)
        Dim cell As ICell = townMap.GetCell(8, 4)
        cell.Effect =
            townMap.CreateEffect()
        TriggerExtensions.SetEffectType(cell.Effect, "Teleport")
        TriggerExtensions.SetDestination(cell.Effect, trainerMap.GetCell(HealthTrainerColumns \ 2, HealthTrainerRows - 2))
    End Sub

    Private Sub StitchHealerToTown(world As IWorld)
        Dim townMap = WorldExtensions.GetSingleMapByMapType(world, MapTypes.Town)
        Dim healerMap = WorldExtensions.GetSingleMapByMapType(world, MapTypes.Healer)
        FromTownToHealerHouse(townMap, healerMap)
        FromHealerHouseToTown(townMap, healerMap)
    End Sub

    Private Sub FromHealerHouseToTown(townMap As IMap, healerMap As IMap)
        Dim fromCell = healerMap.GetCell(HealerColumns \ 2, HealerRows - 1)
        Dim toCell = townMap.GetCell(4, 13)
        fromCell.Effect =
                    healerMap.CreateEffect()
        TriggerExtensions.SetEffectType(fromCell.Effect, "Teleport")
        TriggerExtensions.SetDestination(fromCell.Effect, toCell)
    End Sub

    Private Sub FromTownToHealerHouse(townMap As IMap, healerMap As IMap)
        Dim fromCell As ICell = townMap.GetCell(3, 13)
        fromCell.Effect =
            townMap.CreateEffect()
        TriggerExtensions.SetEffectType(fromCell.Effect, "Teleport")
        Dim toCell As ICell = healerMap.GetCell(HealerColumns \ 2, HealerRows - 2)
        TriggerExtensions.SetDestination(fromCell.Effect, toCell)
    End Sub

    Private Sub StitchTownToWilderness(world As IWorld)
        Dim townMap = WorldExtensions.GetSingleMapByMapType(world, MapTypes.Town)
        Dim wildernessMap = WorldExtensions.GetSingleMapByMapType(world, MapTypes.Wilderness)
        Dim townCell As ICell = FromWildernessToTown(townMap, wildernessMap)
        FromTownToWildernessSouth(townMap, townCell)
        FromTownToWildernessNorth(townMap, townCell)
        FromTownToWildernessWest(townMap, townCell)
        FromTownToWildernessEast(townMap, townCell)
    End Sub

    Private Function FromWildernessToTown(townMap As IMap, wildernessMap As IMap) As ICell
        Dim fromCell = wildernessMap.Cells.Single(Function(x) x.TerrainType = "Town")
        fromCell.Effect =
                    wildernessMap.CreateEffect()
        TriggerExtensions.SetEffectType(fromCell.Effect, "Teleport")
        Dim toCell As ICell = townMap.GetCell(townMap.Columns \ 2, townMap.Rows - 2)
        TriggerExtensions.SetDestination(fromCell.Effect, toCell)
        Return fromCell
    End Function

    Private Sub FromTownToWildernessEast(townMap As IMap, townCell As ICell)
        townMap.GetCell(townMap.Columns - 1, townMap.Rows \ 2).Effect =
                    townMap.CreateEffect()
        TriggerExtensions.SetEffectType(townMap.GetCell(townMap.Columns - 1, townMap.Rows \ 2).Effect, "Teleport")
        TriggerExtensions.SetDestination(townMap.GetCell(townMap.Columns - 1, townMap.Rows \ 2).Effect, townCell)
    End Sub

    Private Sub FromTownToWildernessWest(townMap As IMap, townCell As ICell)
        townMap.GetCell(0, townMap.Rows \ 2).Effect =
                    townMap.CreateEffect()
        TriggerExtensions.SetEffectType(townMap.GetCell(0, townMap.Rows \ 2).Effect, "Teleport")
        TriggerExtensions.SetDestination(townMap.GetCell(0, townMap.Rows \ 2).Effect, townCell)
    End Sub

    Private Sub FromTownToWildernessNorth(townMap As IMap, townCell As ICell)
        townMap.GetCell(townMap.Columns \ 2, 0).Effect =
                    townMap.CreateEffect()
        TriggerExtensions.SetEffectType(townMap.GetCell(townMap.Columns \ 2, 0).Effect, "Teleport")
        TriggerExtensions.SetDestination(townMap.GetCell(townMap.Columns \ 2, 0).Effect, townCell)
    End Sub

    Private Sub FromTownToWildernessSouth(townMap As IMap, townCell As ICell)
        townMap.GetCell(townMap.Columns \ 2, townMap.Rows - 1).Effect =
            townMap.CreateEffect()
        TriggerExtensions.SetEffectType(townMap.GetCell(townMap.Columns \ 2, townMap.Rows - 1).Effect, "Teleport")
        TriggerExtensions.SetDestination(townMap.GetCell(townMap.Columns \ 2, townMap.Rows - 1).Effect, townCell)
    End Sub
End Module
