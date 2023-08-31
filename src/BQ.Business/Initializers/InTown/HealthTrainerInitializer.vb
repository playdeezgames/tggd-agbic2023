Friend Module HealthTrainerInitializer
    Friend Const HealthTrainerColumns = 7
    Friend Const HealthTrainerRows = 7
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
        Dim trainerCell = map.GetCell(map.Columns \ 2, 1)
        trainerCell.TerrainType = "StrongMan"
        trainerCell.Effect =
            map.CreateEffect
        SetEffectType(trainerCell.Effect, "HealthTrainerTalk")
    End Sub

End Module
