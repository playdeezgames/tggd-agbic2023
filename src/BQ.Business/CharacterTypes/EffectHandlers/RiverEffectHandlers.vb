Friend Module RiverEffectHandlers
    Friend Sub DoFillClayPot(character As ICharacter, effect As IEffect)
        RecipeTypes.Craft("WaterPot", character)
        character.World.CreateMessage().AddLine(7, $"{CharacterExtensions.Name(character)} fills a {ToItemTypeDescriptor("ClayPot").Name} with water.")
    End Sub

    Friend Sub DoBumpRiver(character As ICharacter, effect As IEffect)
        Dim msg = character.World.CreateMessage().
            AddLine(7, $"{CharacterExtensions.Name(character)} visits the river bank.").
            AddChoice("Cool story, bro!", "ExitDialog")
        If CharacterExtensions.HasItemTypeInInventory(character, "ClayPot") Then
            msg.AddChoice("Fill Clay Pot with Water", "FillClayPot")
        End If
    End Sub
End Module
