Imports System.Runtime.CompilerServices
Imports BQ.Persistence

Friend Module CellExtensions
    <Extension>
    Friend Function IsTenable(cell As ICell) As Boolean
        Return cell.TerrainType.ToTerrainTypeDescriptor.Tenable
    End Function
    Private ReadOnly deltas As IReadOnlyList(Of (Integer, Integer)) =
        New List(Of (Integer, Integer)) From
        {
            (0, -1),
            (0, 1),
            (-1, 0),
            (1, 0)
        }
    <Extension>
    Friend Function Neighbors(cell As ICell) As IEnumerable(Of ICell)
        Return deltas.Select(Function(xy) cell.Map.GetCell(xy.Item1 + cell.Column, xy.Item2 + cell.Row)).Where(Function(x) x IsNot Nothing)
    End Function
End Module
