Friend Class CookedRatDescriptor
    Inherits ItemTypeDescriptor

    Friend Sub New()
        MyBase.New(
            "Cooked Rat",
            ChrW(&H2F),
            6,
            effects:=New Dictionary(Of String, EffectData) From
            {
                {"EatCookedRat", New EffectData},
                {"SeasonRat", New EffectData}
            })
    End Sub
End Class
