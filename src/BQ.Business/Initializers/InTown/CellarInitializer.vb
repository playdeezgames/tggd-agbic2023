Friend Module CellarInitializer
    Friend Const CellarColumns = 7
    Friend Const CellarRows = 7
    Friend Sub Initialize(map As IMap)
        For Each column In Enumerable.Range(0, map.Columns)
            map.GetCell(column, 0).TerrainType = TerrainTypes.Wall
            map.GetCell(column, map.Rows - 1).TerrainType = TerrainTypes.Wall
        Next
        For Each row In Enumerable.Range(1, map.Rows - 2)
            map.GetCell(0, row).TerrainType = TerrainTypes.Wall
            map.GetCell(map.Columns - 1, row).TerrainType = TerrainTypes.Wall
        Next
        Dim stairsCell = map.GetCell(1, map.Rows - 2)
        stairsCell.TerrainType = TerrainTypes.StairsUp
        stairsCell.Trigger = map.CreateTrigger().SetTriggerType(TriggerTypes.Teleport)
        For Each cell In map.Cells.Where(Function(x) x.TerrainType = TerrainTypes.Empty)
            cell.Statistic(StatisticTypes.Peril) = 1
        Next
    End Sub
End Module
