Friend Class WheatDescriptor
    Inherits ItemTypeDescriptor

    Friend Sub New()
        MyBase.New(
            "Wheat",
            ChrW(&H42),
            14,
            effects:=New Dictionary(Of String, EffectData) From
            {
                {"MillWheat", New EffectData() With {.EffectType = "MillWheat"}}
            })
    End Sub
End Class
