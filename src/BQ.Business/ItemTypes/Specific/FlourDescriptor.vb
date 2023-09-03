﻿Friend Class FlourDescriptor
    Inherits ItemTypeDescriptor

    Friend Sub New()
        MyBase.New(
            "Flour",
            ChrW(&H43),
            15,
            effects:=New Dictionary(Of String, EffectData) From
            {
                {"MakeDough", New EffectData() With {.EffectType = "MakeDough"}}
            })
    End Sub
End Class
