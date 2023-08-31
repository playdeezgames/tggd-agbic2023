Friend Module CellarInitializer
    Friend Const CellarColumns = 7
    Friend Const CellarRows = 7
    Friend Sub Initialize(map As IMap)
        For Each column In Enumerable.Range(0, map.Columns)
            map.GetCell(column, 0).TerrainType = "Wall"
            map.GetCell(column, map.Rows - 1).TerrainType = "Wall"
        Next
        For Each row In Enumerable.Range(1, map.Rows - 2)
            map.GetCell(0, row).TerrainType = "Wall"
            map.GetCell(map.Columns - 1, row).TerrainType = "Wall"
        Next
        Dim stairsCell = map.GetCell(1, map.Rows - 2)
        stairsCell.TerrainType = "StairsUp"
        stairsCell.Effect = map.CreateEffect()
        SetEffectType(stairsCell.Effect, "Teleport")
    End Sub
End Module
