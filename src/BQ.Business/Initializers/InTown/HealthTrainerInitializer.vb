﻿Friend Module HealthTrainerInitializer
    Friend Const HealthTrainerColumns = 7
    Friend Const HealthTrainerRows = 7
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
        Dim trainerCell = map.GetCell(map.Columns \ 2, 1)
        trainerCell.TerrainType = TerrainTypes.StrongMan
        trainerCell.Effect =
            map.CreateEffect.
            SetEffectType(EffectTypes.HealthTrainerTalk)
    End Sub

End Module