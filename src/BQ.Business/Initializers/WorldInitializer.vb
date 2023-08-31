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
        Dim townMap = world.Maps.Single(Function(x) x.MapType = MapTypes.Town)
        Dim potterMap = world.Maps.Single(Function(x) x.MapType = MapTypes.Potter)
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
        Dim townMap = world.Maps.Single(Function(x) x.MapType = MapTypes.Town)
        Dim trainerMap = world.Maps.Single(Function(x) x.MapType = MapTypes.EnergyTrainer)
        townMap.GetCell(6, 6).Effect = townMap.CreateEffect()
        TriggerExtensions.SetEffectType(townMap.GetCell(6, 6).Effect, "Teleport")
        TriggerExtensions.SetDestination(townMap.GetCell(6, 6).Effect, trainerMap.GetCell(EnergyTrainerColumns \ 2, 1))
        trainerMap.GetCell(EnergyTrainerColumns \ 2, 0).Effect = trainerMap.CreateEffect()
        TriggerExtensions.SetEffectType(trainerMap.GetCell(EnergyTrainerColumns \ 2, 0).Effect, "Teleport")
        TriggerExtensions.SetDestination(trainerMap.GetCell(EnergyTrainerColumns \ 2, 0).Effect, townMap.GetCell(6, 5))
    End Sub

    Private Sub StitchInnToTown(world As IWorld)
        Dim townMap = world.Maps.Single(Function(x) x.MapType = MapTypes.Town)
        Dim innMap = world.Maps.Single(Function(x) x.MapType = MapTypes.Inn)
        Dim cellarMap = world.Maps.Single(Function(x) x.MapType = MapTypes.Cellar)
        townMap.GetCell(3, 3).Effect = townMap.CreateEffect()
        TriggerExtensions.SetEffectType(townMap.GetCell(3, 3).Effect, "Teleport")
        TriggerExtensions.SetDestination(townMap.GetCell(3, 3).Effect, innMap.GetCell(InnColumns \ 2, InnRows - 2))
        innMap.GetCell(InnColumns \ 2, InnRows - 1).Effect = innMap.CreateEffect()
        TriggerExtensions.SetEffectType(innMap.GetCell(InnColumns \ 2, InnRows - 1).Effect, "Teleport")
        TriggerExtensions.SetDestination(innMap.GetCell(InnColumns \ 2, InnRows - 1).Effect, townMap.GetCell(3, 2))
        Dim downStairs = innMap.Cells.Single(Function(x) x.TerrainType = TerrainTypes.StairsDown)
        Dim upStairs = cellarMap.Cells.Single(Function(x) x.TerrainType = "StairsUp")

        TriggerExtensions.SetDestination(downStairs.Effect, upStairs)
        TriggerExtensions.SetDestination(upStairs.Effect, downStairs)
    End Sub

    Private Sub StitchDruidHouseToTown(world As IWorld)
        Dim townMap = world.Maps.Single(Function(x) x.MapType = MapTypes.Town)
        Dim druidMap = world.Maps.Single(Function(x) x.MapType = MapTypes.DruidHouse)
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
        Dim townMap = world.Maps.Single(Function(x) x.MapType = MapTypes.Town)
        Dim trainerMap = world.Maps.Single(Function(x) x.MapType = MapTypes.HealthTrainer)
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
        Dim townMap = world.Maps.Single(Function(x) x.MapType = MapTypes.Town)
        Dim healerMap = world.Maps.Single(Function(x) x.MapType = MapTypes.Healer)
        townMap.GetCell(3, 13).Effect =
            townMap.CreateEffect()
        TriggerExtensions.SetEffectType(townMap.GetCell(3, 13).Effect, "Teleport")
        TriggerExtensions.SetDestination(townMap.GetCell(3, 13).Effect, healerMap.GetCell(HealerColumns \ 2, HealerRows - 2))
        healerMap.GetCell(HealerColumns \ 2, HealerRows - 1).Effect =
            healerMap.CreateEffect()
        TriggerExtensions.SetEffectType(townMap.GetCell(3, 13).Effect, "Teleport")
        TriggerExtensions.SetDestination(townMap.GetCell(3, 13).Effect, townMap.GetCell(4, 13))
    End Sub

    Private Sub StitchTownToWilderness(world As IWorld)
        Dim townMap = world.Maps.Single(Function(x) x.MapType = MapTypes.Town)
        Dim wildernessMap = world.Maps.Single(Function(x) x.MapType = MapTypes.Wilderness)
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
