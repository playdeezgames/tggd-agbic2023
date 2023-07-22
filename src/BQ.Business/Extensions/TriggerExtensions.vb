Imports System.Runtime.CompilerServices

Friend Module TriggerExtensions
    <Extension>
    Friend Function SetDestination(trigger As ITrigger, cell As ICell) As ITrigger
        trigger.
            SetStatistic(StatisticTypes.MapId, cell.Map.Id).
            SetStatistic(StatisticTypes.CellColumn, cell.Column).
            SetStatistic(StatisticTypes.CellRow, cell.Row)
        Return trigger
    End Function
End Module
