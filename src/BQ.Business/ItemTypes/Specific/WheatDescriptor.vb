Friend Class WheatDescriptor
    Inherits ItemTypeDescriptor

    Friend Sub New()
        MyBase.New(
            "Wheat",
            ChrW(&H42),
            Yellow,
            effects:=New Dictionary(Of String, EffectData) From
            {
                {"MillWheat", New EffectData() With {.EffectType = "MillWheat"}}
            })
    End Sub
End Class
