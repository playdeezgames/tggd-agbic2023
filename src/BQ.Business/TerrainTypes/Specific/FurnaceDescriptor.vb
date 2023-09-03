Friend Class FurnaceDescriptor
    Inherits TerrainTypeDescriptor

    Public Sub New()
        MyBase.New("Furnace",
            ChrW(&H45),
            7,
            True,
            effects:=New Dictionary(Of String, EffectData) From
            {
                {"CookBagel", New EffectData}
            },
            isFurnace:=True)
    End Sub
End Class
