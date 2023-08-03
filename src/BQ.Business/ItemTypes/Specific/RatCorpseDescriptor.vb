﻿Friend Class RatCorpseDescriptor
    Inherits ItemTypeDescriptor

    Friend Sub New()
        MyBase.New(
            "Rat Corpse",
            ChrW(&H2D),
            DarkGray,
            effects:=New Dictionary(Of String, EffectData) From
            {
                {EffectTypes.CutOffTail, New EffectData() With {.EffectType = EffectTypes.CutOffTail}},
                {EffectTypes.EatRatCorpse, New EffectData() With {.EffectType = EffectTypes.EatRatCorpse}}
            })
    End Sub
End Class
