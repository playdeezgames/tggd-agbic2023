Imports SPLORR.Game

Friend Module CommonTerrainVerbs
    Friend Sub DoForage(character As ICharacter, cell As ICell)
        Const EnergyCost = 1
        If character.Energy < EnergyCost Then
            character.World.CreateMessage().AddLine(LightGray, $"{character.Name} doesn't have the energy to forage.")
            Return
        End If
        character.AddEnergy(-EnergyCost)
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
End Module
