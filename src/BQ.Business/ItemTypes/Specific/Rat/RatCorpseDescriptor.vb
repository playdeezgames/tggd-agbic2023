Friend Class RatCorpseDescriptor
    Inherits ItemTypeDescriptor

    Friend Sub New()
        MyBase.New(
            "Rat Corpse",
            ChrW(&H2D),
            DarkGray,
            effects:=New Dictionary(Of String, EffectData) From
            {
                {"CutOffTail", New EffectData() With {.EffectType = "CutOffTail"}},
                {"EatRatCorpse", New EffectData() With {.EffectType = "EatRatCorpse"}},
                {"CookRatCorpse", New EffectData() With {.EffectType = "CookRatCorpse"}}
            },
            fullNameScript:="return ""test""")
    End Sub
End Class
