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
        townMap.GetCell(13, 13).Effect =
            townMap.CreateEffect()
        TriggerExtensions.SetEffectType(townMap.GetCell(13, 13).Effect, "Teleport")
        TriggerExtensions.SetDestination(townMap.GetCell(13, 13).Effect, potterMap.GetCell(PotterColumns \ 2, PotterRows - 2))
        potterMap.GetCell(PotterColumns \ 2, PotterRows - 1).Effect =
            potterMap.CreateEffect()
        TriggerExtensions.SetEffectType(potterMap.GetCell(PotterColumns \ 2, PotterRows - 1).Effect, "Teleport")
        TriggerExtensions.SetDestination(potterMap.GetCell(PotterColumns \ 2, PotterRows - 1).Effect, townMap.GetCell(13, 12))
    End Sub

    Private Sub StitchEnergyTrainerToTown(world As IWorld)
        Dim townMap = WorldExtensions.GetSingleMapByMapType(world, MapTypes.Town)
        Dim trainerMap = WorldExtensions.GetSingleMapByMapType(world, MapTypes.EnergyTrainer)
        townMap.GetCell(6, 6).Effect = townMap.CreateEffect()
        TriggerExtensions.SetEffectType(townMap.GetCell(6, 6).Effect, "Teleport")
        TriggerExtensions.SetDestination(townMap.GetCell(6, 6).Effect, trainerMap.GetCell(EnergyTrainerColumns \ 2, 1))
        trainerMap.GetCell(EnergyTrainerColumns \ 2, 0).Effect = trainerMap.CreateEffect()
        TriggerExtensions.SetEffectType(trainerMap.GetCell(EnergyTrainerColumns \ 2, 0).Effect, "Teleport")
        TriggerExtensions.SetDestination(trainerMap.GetCell(EnergyTrainerColumns \ 2, 0).Effect, townMap.GetCell(6, 5))
    End Sub

    Private Sub StitchInnToTown(world As IWorld)
        Dim townMap = WorldExtensions.GetSingleMapByMapType(world, MapTypes.Town)
        Dim innMap = WorldExtensions.GetSingleMapByMapType(world, MapTypes.Inn)
        Dim cellarMap = WorldExtensions.GetSingleMapByMapType(world, MapTypes.Cellar)
        townMap.GetCell(3, 3).Effect = townMap.CreateEffect()
        TriggerExtensions.SetEffectType(townMap.GetCell(3, 3).Effect, "Teleport")
        TriggerExtensions.SetDestination(townMap.GetCell(3, 3).Effect, innMap.GetCell(InnColumns \ 2, InnRows - 2))
        innMap.GetCell(InnColumns \ 2, InnRows - 1).Effect = innMap.CreateEffect()
        TriggerExtensions.SetEffectType(innMap.GetCell(InnColumns \ 2, InnRows - 1).Effect, "Teleport")
        TriggerExtensions.SetDestination(innMap.GetCell(InnColumns \ 2, InnRows - 1).Effect, townMap.GetCell(3, 2))
        Dim downStairs = innMap.Cells.Single(Function(x) x.TerrainType = "StairsDown")
        Dim upStairs = cellarMap.Cells.Single(Function(x) x.TerrainType = "StairsUp")

        TriggerExtensions.SetDestination(downStairs.Effect, upStairs)
        TriggerExtensions.SetDestination(upStairs.Effect, downStairs)
    End Sub

    Private Sub StitchDruidHouseToTown(world As IWorld)
        Dim townMap = WorldExtensions.GetSingleMapByMapType(world, MapTypes.Town)
        Dim druidMap = WorldExtensions.GetSingleMapByMapType(world, MapTypes.DruidHouse)
        townMap.GetCell(10, 10).Effect =
            townMap.CreateEffect()
        TriggerExtensions.SetEffectType(townMap.GetCell(10, 10).Effect, "Teleport")
        TriggerExtensions.SetDestination(townMap.GetCell(10, 10).Effect, druidMap.GetCell(1, DruidHouseRows \ 2))
        druidMap.GetCell(0, DruidHouseRows \ 2).Effect =
            druidMap.CreateEffect()
        TriggerExtensions.SetEffectType(druidMap.GetCell(0, DruidHouseRows \ 2).Effect, "Teleport")
        TriggerExtensions.SetDestination(druidMap.GetCell(0, DruidHouseRows \ 2).Effect, townMap.GetCell(9, 10))
    End Sub

    Private Sub StitchHealthTrainerToTown(world As IWorld)
        Dim townMap = WorldExtensions.GetSingleMapByMapType(world, MapTypes.Town)
        Dim trainerMap = WorldExtensions.GetSingleMapByMapType(world, MapTypes.HealthTrainer)
        townMap.GetCell(8, 4).Effect =
            townMap.CreateEffect()
        TriggerExtensions.SetEffectType(townMap.GetCell(8, 4).Effect, "Teleport")
        TriggerExtensions.SetDestination(townMap.GetCell(8, 4).Effect, trainerMap.GetCell(HealthTrainerColumns \ 2, HealthTrainerRows - 2))
        trainerMap.GetCell(HealthTrainerColumns \ 2, HealthTrainerRows - 1).Effect =
            trainerMap.CreateEffect()
        TriggerExtensions.SetEffectType(trainerMap.GetCell(HealthTrainerColumns \ 2, HealthTrainerRows - 1).Effect, "Teleport")
        TriggerExtensions.SetDestination(trainerMap.GetCell(HealthTrainerColumns \ 2, HealthTrainerRows - 1).Effect, townMap.GetCell(8, 3))
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
        townMap.GetCell(3, 13).Effect =
            townMap.CreateEffect()
        TriggerExtensions.SetEffectType(townMap.GetCell(3, 13).Effect, "Teleport")
        TriggerExtensions.SetDestination(townMap.GetCell(3, 13).Effect, healerMap.GetCell(HealerColumns \ 2, HealerRows - 2))
    End Sub

    Private Sub StitchTownToWilderness(world As IWorld)
        Dim townMap = WorldExtensions.GetSingleMapByMapType(world, MapTypes.Town)
        Dim wildernessMap = WorldExtensions.GetSingleMapByMapType(world, MapTypes.Wilderness)
        Dim townCell = wildernessMap.Cells.Single(Function(x) x.TerrainType = "Town")
        townMap.GetCell(townMap.Columns \ 2, townMap.Rows - 1).Effect =
            townMap.CreateEffect()
        TriggerExtensions.SetEffectType(townMap.GetCell(townMap.Columns \ 2, townMap.Rows - 1).Effect, "Teleport")
        TriggerExtensions.SetDestination(townMap.GetCell(townMap.Columns \ 2, townMap.Rows - 1).Effect, townCell)
        townMap.GetCell(townMap.Columns \ 2, 0).Effect =
            townMap.CreateEffect()
        TriggerExtensions.SetEffectType(townMap.GetCell(townMap.Columns \ 2, 0).Effect, "Teleport")
        TriggerExtensions.SetDestination(townMap.GetCell(townMap.Columns \ 2, 0).Effect, townCell)
        townMap.GetCell(0, townMap.Rows \ 2).Effect =
            townMap.CreateEffect()
        TriggerExtensions.SetEffectType(townMap.GetCell(0, townMap.Rows \ 2).Effect, "Teleport")
        TriggerExtensions.SetDestination(townMap.GetCell(0, townMap.Rows \ 2).Effect, townCell)
        townMap.GetCell(townMap.Columns - 1, townMap.Rows \ 2).Effect =
            townMap.CreateEffect()
        TriggerExtensions.SetEffectType(townMap.GetCell(townMap.Columns - 1, townMap.Rows \ 2).Effect, "Teleport")
        TriggerExtensions.SetDestination(townMap.GetCell(townMap.Columns - 1, townMap.Rows \ 2).Effect, townCell)
        townCell.Effect =
            wildernessMap.CreateEffect()
        TriggerExtensions.SetEffectType(townCell.Effect, "Teleport")
        TriggerExtensions.SetDestination(townCell.Effect, townMap.GetCell(townMap.Columns \ 2, townMap.Rows - 2))
    End Sub
End Module
