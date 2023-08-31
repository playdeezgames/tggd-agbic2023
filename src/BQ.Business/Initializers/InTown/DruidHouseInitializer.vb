Friend Module DruidHouseInitializer
    Friend Const DruidHouseColumns = 7
    Friend Const DruidHouseRows = 7
    Friend Sub Initialize(map As IMap)
        For Each column In Enumerable.Range(0, map.Columns)
            map.GetCell(column, 0).TerrainType = "Wall"
            map.GetCell(column, map.Rows - 1).TerrainType = "Wall"
        Next
        For Each row In Enumerable.Range(1, map.Rows - 2)
            map.GetCell(0, row).TerrainType = "Wall"
            map.GetCell(map.Columns - 1, row).TerrainType = "Wall"
        Next
        map.GetCell(0, map.Rows \ 2).TerrainType = "Door"
        Dim druidCell = map.GetCell(map.Columns - 2, map.Rows \ 2)
        druidCell.TerrainType = "Druid"
        druidCell.Effect =
            map.CreateEffect
        SetEffectType(druidCell.Effect, "DruidTalk")
    End Sub
End Module
