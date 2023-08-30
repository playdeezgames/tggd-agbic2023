Friend Class SmokedPepperDescriptor
    Inherits ItemTypeDescriptor

    Friend Sub New()
        MyBase.New(
            "Smoked Pepper",
            ChrW(&H39),
            Orange,
            effects:=New Dictionary(Of String, EffectData) From
            {
                {"MakePaprika", New EffectData() With {.EffectType = "MakePaprika"}},
                {"EatSmokedPepper", New EffectData() With {.EffectType = "EatSmokedPepper"}}
            })
    End Sub
End Class
