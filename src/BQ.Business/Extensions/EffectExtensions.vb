Public Module EffectExtensions
    Public Function ToItemEffect(effect As IEffect) As IItemEffect
        Return CType(effect, IItemEffect)
    End Function
    Public Function ToTerrainEffect(effect As IEffect) As ITerrainEffect
        Return CType(effect, ITerrainEffect)
    End Function
End Module
