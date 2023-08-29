Friend Module RiverEffectHandlers
    Friend Sub DoFillClayPot(character As ICharacter, effect As IEffect)
        RecipeTypes.Craft("WaterPot", character)
        character.World.CreateMessage().AddLine(LightGray, $"{CharacterExtensions.Name(character)} fills a {ItemTypes.ClayPot.ToItemTypeDescriptor.Name} with water.")
    End Sub

    Friend Sub DoBumpRiver(character As ICharacter, effect As IEffect)
        Dim msg = character.World.CreateMessage().
            AddLine(LightGray, $"{CharacterExtensions.Name(character)} visits the river bank.").
            AddChoice(CoolStoryBro, EffectTypes.ExitDialog)
        If CharacterExtensions.HasItemTypeInInventory(character, ItemTypes.ClayPot) Then
            msg.AddChoice("Fill Clay Pot with Water", EffectTypes.FillClayPot)
        End If
    End Sub
End Module
