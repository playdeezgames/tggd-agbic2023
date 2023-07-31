Imports BQ.Persistence

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
    End Sub

    Private Sub StitchEnergyTrainerToTown(world As IWorld)
        Dim townMap = world.Maps.Single(Function(x) x.MapType = MapTypes.Town)
        Dim trainerMap = world.Maps.Single(Function(x) x.MapType = MapTypes.EnergyTrainer)
        townMap.GetCell(6, 6).Trigger =
            townMap.CreateTrigger().
            SetTriggerType(Teleport).
            SetDestination(trainerMap.GetCell(EnergyTrainerColumns \ 2, 1))
        trainerMap.GetCell(EnergyTrainerColumns \ 2, 0).Trigger =
            trainerMap.CreateTrigger().
            SetTriggerType(Teleport).
            SetDestination(townMap.GetCell(6, 5))
    End Sub

    Private Sub StitchInnToTown(world As IWorld)
        Dim townMap = world.Maps.Single(Function(x) x.MapType = MapTypes.Town)
        Dim innMap = world.Maps.Single(Function(x) x.MapType = MapTypes.Inn)
        Dim cellarMap = world.Maps.Single(Function(x) x.MapType = MapTypes.Cellar)
        townMap.GetCell(3, 3).Trigger =
            townMap.CreateTrigger().
            SetTriggerType(Teleport).
            SetDestination(innMap.GetCell(InnColumns \ 2, InnRows - 2))
        innMap.GetCell(InnColumns \ 2, InnRows - 1).Trigger =
            innMap.CreateTrigger().
            SetTriggerType(Teleport).
            SetDestination(townMap.GetCell(3, 2))
        Dim downStairs = innMap.Cells.Single(Function(x) x.TerrainType = TerrainTypes.StairsDown)
        Dim upStairs = cellarMap.Cells.Single(Function(x) x.TerrainType = TerrainTypes.StairsUp)
        downStairs.Trigger.
            SetDestination(upStairs)
        upStairs.Trigger.SetDestination(downStairs)
    End Sub

    Private Sub StitchDruidHouseToTown(world As IWorld)
        Dim townMap = world.Maps.Single(Function(x) x.MapType = MapTypes.Town)
        Dim druidMap = world.Maps.Single(Function(x) x.MapType = MapTypes.DruidHouse)
        townMap.GetCell(10, 10).Trigger =
            townMap.CreateTrigger().
            SetTriggerType(Teleport).
            SetDestination(druidMap.GetCell(1, DruidHouseRows \ 2))
        druidMap.GetCell(0, DruidHouseRows \ 2).Trigger =
            druidMap.CreateTrigger().
            SetTriggerType(Teleport).
            SetDestination(townMap.GetCell(9, 10))
    End Sub

    Private Sub StitchHealthTrainerToTown(world As IWorld)
        Dim townMap = world.Maps.Single(Function(x) x.MapType = MapTypes.Town)
        Dim trainerMap = world.Maps.Single(Function(x) x.MapType = MapTypes.HealthTrainer)
        townMap.GetCell(8, 4).Trigger =
            townMap.CreateTrigger().
            SetTriggerType(Teleport).
            SetDestination(trainerMap.GetCell(HealthTrainerColumns \ 2, HealthTrainerRows - 2))
        trainerMap.GetCell(HealthTrainerColumns \ 2, HealthTrainerRows - 1).Trigger =
            trainerMap.CreateTrigger().
            SetTriggerType(Teleport).
            SetDestination(townMap.GetCell(8, 3))
    End Sub

    Private Sub StitchHealerToTown(world As IWorld)
        Dim townMap = world.Maps.Single(Function(x) x.MapType = MapTypes.Town)
        Dim healerMap = world.Maps.Single(Function(x) x.MapType = MapTypes.Healer)
        townMap.GetCell(3, 13).Trigger =
            townMap.CreateTrigger().
            SetTriggerType(Teleport).
            SetDestination(healerMap.GetCell(HealerColumns \ 2, HealerRows - 2))
        healerMap.GetCell(HealerColumns \ 2, HealerRows - 1).Trigger =
            healerMap.CreateTrigger().
            SetTriggerType(Teleport).
            SetDestination(townMap.GetCell(4, 13))
    End Sub

    Private Sub StitchTownToWilderness(world As IWorld)
        Dim townMap = world.Maps.Single(Function(x) x.MapType = MapTypes.Town)
        Dim wildernessMap = world.Maps.Single(Function(x) x.MapType = MapTypes.Wilderness)
        Dim townCell = wildernessMap.Cells.Single(Function(x) x.TerrainType = TerrainTypes.Town)
        townMap.GetCell(townMap.Columns \ 2, townMap.Rows - 1).Trigger =
            townMap.CreateTrigger().
            SetTriggerType(TriggerTypes.Teleport).
            SetDestination(townCell)
        townMap.GetCell(townMap.Columns \ 2, 0).Trigger =
            townMap.CreateTrigger().
            SetTriggerType(TriggerTypes.Teleport).
            SetDestination(townCell)
        townMap.GetCell(0, townMap.Rows \ 2).Trigger =
            townMap.CreateTrigger().
            SetTriggerType(TriggerTypes.Teleport).
            SetDestination(townCell)
        townMap.GetCell(townMap.Columns - 1, townMap.Rows \ 2).Trigger =
            townMap.CreateTrigger().
            SetTriggerType(TriggerTypes.Teleport).
            SetDestination(townCell)
        townCell.Trigger =
            wildernessMap.CreateTrigger().
            SetTriggerType(TriggerTypes.Teleport).
            SetDestination(townMap.GetCell(townMap.Columns \ 2, townMap.Rows - 2))
    End Sub
End Module
