﻿Imports SPLORR.Game

Friend Module CommonTerrainVerbs
    Friend Sub DoForage(character As ICharacter, cell As ICell)
        Const EnergyCost = 1
        If CharacterExtensions.Energy(character) < EnergyCost Then
            character.World.CreateMessage().AddLine(LightGray, $"{CharacterExtensions.Name(character)} doesn't have the energy to forage.")
            Return
        End If
        CharacterExtensions.AddEnergy(character, -EnergyCost)
        Dim descriptor = cell.Descriptor
        Dim itemType = RNG.FromGenerator(descriptor.Foragables)
        If String.IsNullOrEmpty(itemType) Then
            character.World.CreateMessage().AddLine(LightGray, $"{CharacterExtensions.Name(character)} finds nothing.")
            Return
        End If
        Dim item = ItemInitializer.CreateItem(character.World, itemType)
        character.AddItem(item)
        character.World.CreateMessage().AddLine(LightGray, $"{CharacterExtensions.Name(character)} finds {item.Name}")
    End Sub
End Module
