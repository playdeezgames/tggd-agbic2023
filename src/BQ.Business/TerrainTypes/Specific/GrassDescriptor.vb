Imports SPLORR.Game

Friend Class GrassDescriptor
    Inherits TerrainTypeDescriptor

    Public Sub New()
        MyBase.New(
                    "Grass",
                    ChrW(4),
                    Business.Hue.Green,
                    True,
                    effects:=New Dictionary(Of String, EffectData),
                    foragables:=New Dictionary(Of String, Integer) From
                    {
                        {String.Empty, 1},
                        {ItemTypes.PlantFiber, 1}
                    })
    End Sub
End Class
