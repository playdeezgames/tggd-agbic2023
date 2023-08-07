Friend Class EmptyTerrainDescriptor
    Inherits TerrainTypeDescriptor

    Public Sub New()
        MyBase.New(
            "Empty",
            ChrW(0),
            Black,
            True,
            effects:=New Dictionary(Of String, EffectData) From
            {
                {EffectTypes.BuildFire, New EffectData}
            })
    End Sub
End Class
