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
                {"BuildFire", New EffectData},
                {"BuildFurnace", New EffectData}
            },
            canBuildFurnace:=True)
    End Sub
End Class
