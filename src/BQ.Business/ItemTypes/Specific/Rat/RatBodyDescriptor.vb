Friend Class RatBodyDescriptor
    Inherits ItemTypeDescriptor

    Friend Sub New()
        MyBase.New(
            "Rat Body",
            ChrW(&H2F),
            DarkGray,
            effects:=New Dictionary(Of String, EffectData) From
            {
                {EffectTypes.EatRatCorpse, New EffectData() With {.EffectType = EffectTypes.EatRatCorpse}},
                {"CookRatBody", New EffectData() With {.EffectType = "CookRatBody"}}
            })
    End Sub
End Class
