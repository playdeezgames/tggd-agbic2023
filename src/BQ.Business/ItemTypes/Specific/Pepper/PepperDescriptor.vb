﻿Friend Class PepperDescriptor
    Inherits ItemTypeDescriptor

    Friend Sub New()
        MyBase.New(
            "Pepper",
            ChrW(&H39),
            Red,
            effects:=New Dictionary(Of String, EffectData) From
            {
                {EffectTypes.SmokePepper, New EffectData() With {.EffectType = EffectTypes.SmokePepper}},
                {EffectTypes.EatPepper, New EffectData() With {.EffectType = EffectTypes.EatPepper}}
            })
    End Sub
End Class
