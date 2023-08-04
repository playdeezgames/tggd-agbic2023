Imports System.Runtime.CompilerServices

Friend Module CellExtensions
    <Extension>
    Friend Function CanBuildFire(cell As ICell) As Boolean
        Return cell.Descriptor.HasEffect(EffectTypes.BuildFire)
    End Function
    <Extension>
    Friend Function CanForage(cell As ICell) As Boolean
        Return cell.Descriptor.HasEffect(EffectTypes.Forage)
    End Function
    <Extension>
    Friend Function Descriptor(cell As ICell) As TerrainTypeDescriptor
        Return cell.TerrainType.ToTerrainTypeDescriptor
    End Function
    <Extension>
    Friend Sub DoEffect(cell As ICell, effectType As String, character As ICharacter)
        Dim descriptor = cell.Descriptor
        If Not descriptor.HasEffect(effectType) Then
            MessageTypes.NothingHappens.ToMessageTypeDescriptor.CreateMessage(character.World)
            Return
        End If
        descriptor.DoEffect(character, effectType, cell)
    End Sub
    <Extension>
    Friend Function IsTenable(cell As ICell) As Boolean
        Return cell.TerrainTypeDescriptor.Tenable
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
    <Extension>
    Private Function TerrainTypeDescriptor(cell As ICell) As TerrainTypeDescriptor
        Return cell.Descriptor
    End Function
    <Extension>
    Private Function TryGetStatistic(cell As ICell, statisticType As String, Optional defaultValue As Integer = 0) As Integer
        Return If(cell.HasStatistic(statisticType), cell.Statistic(statisticType), defaultValue)
    End Function
    <Extension>
    Friend Function Peril(cell As ICell) As Integer
        Return cell.TryGetStatistic(StatisticTypes.Peril)
    End Function
End Module
