Friend Class WheatDescriptor
    Inherits ItemTypeDescriptor

    Friend Sub New()
        MyBase.New(
            "Wheat",
            ChrW(&H42),
            Yellow,
            effects:=New Dictionary(Of String, EffectData) From
            {
                {EffectTypes.MillWheat, New EffectData() With {.EffectType = EffectTypes.MillWheat}}
            })
    End Sub
End Class
