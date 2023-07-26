Imports SPLORR.Game

Friend Class GrassDescriptor
    Inherits TerrainTypeDescriptor

    Public Sub New()
        MyBase.New(
                    "Grass",
                    ChrW(4),
                    Business.Hue.Green,
                    True,
                    verbTypes:=New Dictionary(Of String, Action(Of ICharacter, ICell)) From
                    {
                        {Business.VerbTypes.Forage, AddressOf DoForage}
                    },
                    foragables:=New Dictionary(Of String, Integer) From
                    {
                        {String.Empty, 1},
                        {ItemTypes.PlantFiber, 1}
                    })
    End Sub
End Class
