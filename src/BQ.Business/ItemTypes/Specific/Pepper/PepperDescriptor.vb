Friend Class PepperDescriptor
    Inherits ItemTypeDescriptor

    Friend Sub New()
        MyBase.New(
            "Pepper",
            ChrW(&H39),
            Red,
            effects:=New Dictionary(Of String, EffectData) From
            {
                {"SmokePepper", New EffectData() With {.EffectType = "SmokePepper"}},
                {"EatPepper", New EffectData() With {.EffectType = "EatPepper"}}
            })
    End Sub
End Class
