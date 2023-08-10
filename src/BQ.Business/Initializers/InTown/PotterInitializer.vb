﻿Friend Module PotterInitializer
    Friend Const PotterColumns = 7
    Friend Const PotterRows = 7
    Friend Sub Initialize(map As IMap)
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
        trainerCell.TerrainType = TerrainTypes.Potter
        trainerCell.Effect =
            map.CreateEffect.
            SetEffectType(EffectTypes.PotterTalk)
    End Sub
End Module