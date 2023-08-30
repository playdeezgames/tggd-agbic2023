Friend Module DruidHouseInitializer
    Friend Const DruidHouseColumns = 7
    Friend Const DruidHouseRows = 7
    Friend Sub Initialize(map As IMap)
        For Each column In Enumerable.Range(0, map.Columns)
            map.GetCell(column, 0).TerrainType = TerrainTypes.Wall
            map.GetCell(column, map.Rows - 1).TerrainType = TerrainTypes.Wall
        Next
        For Each row In Enumerable.Range(1, map.Rows - 2)
            map.GetCell(0, row).TerrainType = TerrainTypes.Wall
            map.GetCell(map.Columns - 1, row).TerrainType = TerrainTypes.Wall
        Next
        map.GetCell(0, map.Rows \ 2).TerrainType = TerrainTypes.Door
        Dim druidCell = map.GetCell(map.Columns - 2, map.Rows \ 2)
        druidCell.TerrainType = TerrainTypes.Druid
        druidCell.Effect =
            map.CreateEffect
        SetEffectType(druidCell.Effect, "DruidTalk")
    End Sub
End Module
