﻿Friend Module EnergyTrainerInitializer
    Friend Const EnergyTrainerColumns = 7
    Friend Const EnergyTrainerRows = 7
    Friend Sub Initialize(map As IMap)
        For Each column In Enumerable.Range(0, map.Columns)
            map.GetCell(column, 0).TerrainType = TerrainTypes.Wall
            map.GetCell(column, map.Rows - 1).TerrainType = TerrainTypes.Wall
        Next
        For Each row In Enumerable.Range(1, map.Rows - 2)
            map.GetCell(0, row).TerrainType = TerrainTypes.Wall
            map.GetCell(map.Columns - 1, row).TerrainType = TerrainTypes.Wall
        Next
        map.GetCell(map.Columns \ 2, 0).TerrainType = TerrainTypes.Door
        Dim trainerCell = map.GetCell(map.Columns \ 2, map.Rows - 2)
        trainerCell.TerrainType = TerrainTypes.EnergyTrainer
        trainerCell.Effect =
            map.CreateEffect.
            SetEffectType(EffectTypes.EnergyTrainerTalk)
    End Sub

End Module