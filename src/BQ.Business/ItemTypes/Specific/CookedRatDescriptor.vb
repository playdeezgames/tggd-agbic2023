﻿Friend Class CookedRatDescriptor
    Inherits ItemTypeDescriptor

    Friend Sub New()
        MyBase.New(
            "Cooked Rat",
            ChrW(&H2F),
            Brown,
            effects:=New Dictionary(Of String, EffectData) From
            {
                {EffectTypes.EatCookedRat, New EffectData}
            })
    End Sub
End Class
