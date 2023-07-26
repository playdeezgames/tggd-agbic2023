Imports System.Runtime.CompilerServices

Friend Module TriggerExtensions
    <Extension>
    Friend Function SetDestination(Of THolder As IStatisticsHolder)(trigger As THolder, cell As ICell) As THolder
        trigger.
            SetStatistic(StatisticTypes.MapId, cell.Map.Id).
            SetStatistic(StatisticTypes.CellColumn, cell.Column).
            SetStatistic(StatisticTypes.CellRow, cell.Row)
        Return trigger
    End Function
    <Extension>
    Friend Function SetTriggerType(Of THolder As ITrigger)(trigger As THolder, triggerType As String) As THolder
        trigger.TriggerType = triggerType
        Return trigger
    End Function
End Module
