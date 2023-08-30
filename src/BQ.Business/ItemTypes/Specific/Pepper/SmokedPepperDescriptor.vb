Friend Class SmokedPepperDescriptor
    Inherits ItemTypeDescriptor

    Friend Sub New()
        MyBase.New(
            "Smoked Pepper",
            ChrW(&H39),
            Orange,
            effects:=New Dictionary(Of String, EffectData) From
            {
                {EffectTypes.MakePaprika, New EffectData() With {.EffectType = EffectTypes.MakePaprika}},
                {"EatSmokedPepper", New EffectData() With {.EffectType = "EatSmokedPepper"}}
            })
    End Sub
End Class
