Friend Class EnergyHerbDescriptor
    Inherits ItemTypeDescriptor

    Friend Sub New()
        MyBase.New(
            "Energy Herb",
            ChrW(&H28),
            Cyan,
            effects:=New Dictionary(Of String, EffectData) From
            {
                {EffectTypes.UseEnergyHerb, New EffectData() With {.EffectType = EffectTypes.UseEnergyHerb}}
            })
    End Sub
End Class
