Friend Module HealerInitializer
    Friend Const HealerColumns = 7
    Friend Const HealerRows = 7
    Sub Initialize(map As IMap)
        For Each column In Enumerable.Range(0, map.Columns)
            map.GetCell(column, 0).TerrainType = "Wall"
            map.GetCell(column, map.Rows - 1).TerrainType = "Wall"
        Next
        For Each row In Enumerable.Range(1, map.Rows - 2)
            map.GetCell(0, row).TerrainType = "Wall"
            map.GetCell(map.Columns - 1, row).TerrainType = "Wall"
        Next
        map.GetCell(map.Columns \ 2, map.Rows - 1).TerrainType = "Door"
        Dim basinCell = map.GetCell(map.Columns \ 2, 1)
        basinCell.TerrainType = "Basin"
        basinCell.Effect =
            map.CreateEffect
        SetEffectType(basinCell.Effect, "Heal")
        basinCell.Effect.SetStatistic("MaximumHealth", 5)
        Dim healerCell = map.GetCell(map.Columns - 2, map.Rows - 2)
        healerCell.TerrainType = "OldMan"
        healerCell.Effect =
            map.CreateEffect
        SetEffectType(healerCell.Effect, "HealerTalk")
    End Sub
End Module
