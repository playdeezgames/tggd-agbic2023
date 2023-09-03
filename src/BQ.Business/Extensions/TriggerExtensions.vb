Friend Module TriggerExtensions
    Friend Function SetDestination(Of THolder As IStatisticsHolder)(effect As THolder, cell As ICell) As THolder
        effect.SetStatistic("MapId", cell.Map.Id)
        effect.SetStatistic("CellColumn", cell.Column)
        effect.SetStatistic("CellRow", cell.Row)
        Return effect
    End Function
    Friend Function SetEffectType(Of THolder As IEffect)(effect As THolder, effectType As String) As THolder
        effect.EffectType = effectType
        Return effect
    End Function
End Module
