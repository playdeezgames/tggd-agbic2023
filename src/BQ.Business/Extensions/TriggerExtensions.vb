Imports System.Runtime.CompilerServices

Friend Module TriggerExtensions
    <Extension>
    Friend Function SetDestination(Of THolder As IStatisticsHolder)(effect As THolder, cell As ICell) As THolder
        effect.
            SetStatisticTo(StatisticTypes.MapId, cell.Map.Id).
            SetStatisticTo(StatisticTypes.CellColumn, cell.Column).
            SetStatisticTo(StatisticTypes.CellRow, cell.Row)
        Return effect
    End Function
    <Extension>
    Friend Function SetEffectType(Of THolder As IEffect)(effect As THolder, effectType As String) As THolder
        effect.EffectType = effectType
        Return effect
    End Function
End Module
