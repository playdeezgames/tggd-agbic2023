Imports BQ.Persistence

Friend Module WorldInitializer
    Friend Sub Initialize(world As IWorld)
        MapInitializer.Initialize(world)
        StitchMaps(world)
        AvatarInitializer.Initialize(world)
    End Sub
    Private Sub StitchMaps(world As IWorld)
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
