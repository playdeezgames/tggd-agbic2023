Public Module EffectExtensions
    Public Function ToItemEffect(effect As IEffect) As IItemEffect
        Return CType(effect, IItemEffect)
    End Function
End Module
