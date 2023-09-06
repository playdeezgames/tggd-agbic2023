Imports System.ComponentModel
Imports SPLORR.Game

Module WildernessInitializer
    Private Const WildernessMazeColumns = 5
    Private Const WildernessCellColumns = 15
    Private Const WildernessMazerows = 5
    Private Const WildernessCellRows = 15
    Friend Const WildernessColumns = WildernessMazeColumns * WildernessCellColumns
    Friend Const WildernessRows = WildernessMazerows * WildernessCellRows
    Private ReadOnly walker As IReadOnlyDictionary(Of Direction, MazeDirection(Of Direction)) =
        New Dictionary(Of Direction, MazeDirection(Of Direction)) From
        {
            {Direction.North, New MazeDirection(Of Direction)(Direction.South, 0, -1)},
            {Direction.East, New MazeDirection(Of Direction)(Direction.West, 1, 0)},
            {Direction.South, New MazeDirection(Of Direction)(Direction.North, 0, 1)},
            {Direction.West, New MazeDirection(Of Direction)(Direction.East, -1, 0)}
        }
    Private ReadOnly terrainTable As IReadOnlyDictionary(Of String, Integer) =
        New Dictionary(Of String, Integer) From
        {
            {"Empty", 100},
            {"Grass", 50},
            {"Tree", 150},
            {"Farm", 10},
            {"ClayPit", 1},
            {"RockQuarry", 5}
        }
    Friend Sub Initialize(map As IMap)
        map.SetFlag("CampingAllowed", True)
        map.SetFlag("AllowFireBuilding", True)
        PaintTerrain(map)
        DrawRivers(map)
        PlaceTown(map)
    End Sub

    Private Sub PlaceTown(map As IMap)
        Dim x As Integer
        Dim y As Integer
        Do
            x = RNG.FromRange(0, WildernessColumns - 1)
            y = RNG.FromRange(0, WildernessRows - 1)
        Loop Until CellExtensions.IsTenable(map.GetCell(x, y))
        map.GetCell(x, y).TerrainType = "Town"
    End Sub

    Private Sub PaintTerrain(map As IMap)
        For Each x In Enumerable.Range(0, WildernessColumns)
            For Each y In Enumerable.Range(0, WildernessRows)
                map.GetCell(x, y).TerrainType = RNG.FromGenerator(terrainTable)
            Next
        Next
    End Sub

    Private Sub DrawRivers(map As IMap)
        Dim maze = New SPLORR.Game.Maze(Of Direction)(WildernessMazeColumns, WildernessMazerows, walker)
        maze.Generate()
        For Each mazeColumn In Enumerable.Range(0, CInt(maze.Columns))
            For Each mazeRow In Enumerable.Range(0, CInt(maze.Rows))
                InitializeCell(map, mazeColumn * WildernessCellColumns, mazeRow * WildernessCellRows, maze.GetCell(mazeColumn, mazeRow))
            Next
        Next
        Dim riverEffect = map.CreateEffect
        SetEffectType(riverEffect, "BumpRiver")
        For Each cell In map.Cells.Where(Function(x) TerrainTypes.Descriptor(x).GetFlag("IsWaterSource"))
            cell.Effect = riverEffect
        Next
    End Sub

    Private ReadOnly riverTable As IReadOnlyDictionary(Of Integer, String) =
        New Dictionary(Of Integer, String) From
        {
            {1, "RiverN"},
            {2, "RiverE"},
            {3, "RiverNE"},
            {4, "RiverS"},
            {5, "RiverNS"},
            {6, "RiverSE"},
            {7, "RiverNES"},
            {8, "RiverW"},
            {9, "RiverNW"},
            {10, "RiverEW"},
            {11, "RiverWNE"},
            {12, "RiverSW"},
            {13, "RiverSWN"},
            {14, "RiverESW"},
            {15, "RiverNESW"}
        }
    Private Sub InitializeCell(map As IMap, offsetX As Integer, offsetY As Integer, mazeCell As MazeCell(Of Direction))
        Dim flags = 0
        If If(mazeCell.GetDoor(Direction.North)?.Open, False) Then
            For Each y In Enumerable.Range(offsetY, WildernessCellRows \ 2)
                map.GetCell(offsetX + WildernessCellColumns \ 2, y).TerrainType = "RiverNS"
            Next
            flags += 1
        End If
        If If(mazeCell.GetDoor(Direction.South)?.Open, False) Then
            For Each y In Enumerable.Range(offsetY + WildernessCellRows \ 2 + 1, WildernessCellRows \ 2)
                map.GetCell(offsetX + WildernessCellColumns \ 2, y).TerrainType = "RiverNS"
            Next
            flags += 4
        End If
        If If(mazeCell.GetDoor(Direction.West)?.Open, False) Then
            For Each x In Enumerable.Range(offsetX, WildernessCellColumns \ 2)
                map.GetCell(x, offsetY + WildernessCellRows \ 2).TerrainType = "RiverEW"
            Next
            flags += 8
        End If
        If If(mazeCell.GetDoor(Direction.East)?.Open, False) Then
            For Each x In Enumerable.Range(offsetX + WildernessCellColumns \ 2 + 1, WildernessCellColumns \ 2)
                map.GetCell(x, offsetY + WildernessCellRows \ 2).TerrainType = "RiverEW"
            Next
            flags += 2
        End If
        map.GetCell(offsetX + WildernessCellColumns \ 2, offsetY + WildernessCellRows \ 2).TerrainType = riverTable(flags)
    End Sub
End Module
