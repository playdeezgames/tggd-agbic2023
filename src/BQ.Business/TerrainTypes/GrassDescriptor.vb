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
                        {Business.VerbTypes.Forage, AddressOf DefaultForage}
                    },
                    foragables:=New Dictionary(Of String, Integer) From
                    {
                        {String.Empty, 1},
                        {ItemTypes.PlantFiber, 1}
                    })
    End Sub
    Private Shared Sub DefaultForage(character As ICharacter, cell As ICell)
        Dim descriptor = cell.TerrainType.ToTerrainTypeDescriptor
        Dim itemType = RNG.FromGenerator(descriptor.Foragables)
        If String.IsNullOrEmpty(itemType) Then
            character.World.CreateMessage().AddLine(LightGray, $"{character.Name} finds nothing.")
            Return
        End If
        Dim item = ItemInitializer.CreateItem(character.World, itemType)
        character.AddItem(item)
        character.World.CreateMessage().AddLine(LightGray, $"{character.Name} finds {item.Name}")
    End Sub
End Class
