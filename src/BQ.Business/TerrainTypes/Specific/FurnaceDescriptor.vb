Friend Class FurnaceDescriptor
    Inherits TerrainTypeDescriptor

    Public Sub New()
        MyBase.New("Furnace",
            ChrW(&H45),
            Business.Hue.LightGray,
            True,
            effects:=New Dictionary(Of String, EffectData) From
            {
                {EffectTypes.CookBagel, New EffectData}
            },
            isFurnace:=True)
    End Sub
End Class
