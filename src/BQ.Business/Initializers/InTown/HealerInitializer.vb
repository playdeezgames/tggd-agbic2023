Friend Module HealerInitializer
    Friend Const HealerColumns = 7
    Friend Const HealerRows = 7
    Sub Initialize(map As IMap)
        For Each column In Enumerable.Range(0, map.Columns)
            map.GetCell(column, 0).TerrainType = TerrainTypes.Wall
            map.GetCell(column, map.Rows - 1).TerrainType = TerrainTypes.Wall
        Next
        For Each row In Enumerable.Range(1, map.Rows - 2)
            map.GetCell(0, row).TerrainType = TerrainTypes.Wall
            map.GetCell(map.Columns - 1, row).TerrainType = TerrainTypes.Wall
        Next
        map.GetCell(map.Columns \ 2, map.Rows - 1).TerrainType = TerrainTypes.Door
        Dim basinCell = map.GetCell(map.Columns \ 2, 1)
        basinCell.TerrainType = TerrainTypes.Basin
        basinCell.Effect =
            map.CreateEffect
        SetEffectType(basinCell.Effect, "Heal")
        basinCell.Effect.SetStatistic(StatisticTypes.MaximumHealth, 5)
        Dim healerCell = map.GetCell(map.Columns - 2, map.Rows - 2)
        healerCell.TerrainType = TerrainTypes.OldMan
        healerCell.Effect =
            map.CreateEffect
        SetEffectType(healerCell.Effect, EffectTypes.HealerTalk)
    End Sub
End Module
