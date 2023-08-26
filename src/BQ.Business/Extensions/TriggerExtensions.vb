Friend Module TriggerExtensions
    <Extension>
    Friend Function SetDestination(Of THolder As IStatisticsHolder)(effect As THolder, cell As ICell) As THolder
        effect.SetStatistic(StatisticTypes.MapId, cell.Map.Id)
        effect.SetStatistic(StatisticTypes.CellColumn, cell.Column)
        effect.SetStatistic(StatisticTypes.CellRow, cell.Row)
        Return effect
    End Function
    <Extension>
    Friend Function SetEffectType(Of THolder As IEffect)(effect As THolder, effectType As String) As THolder
        effect.EffectType = effectType
        Return effect
    End Function
End Module
