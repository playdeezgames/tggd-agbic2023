Friend Module RiverEffectHandlers
    Friend Sub DoFillClayPot(character As ICharacter, effect As IEffect)
        RecipeTypes.Craft(RecipeTypes.WaterPot, character)
        character.World.CreateMessage().AddLine(LightGray, $"{character.Name} fills a {ItemTypes.ClayPot.ToItemTypeDescriptor.Name} with water.")
    End Sub

    Friend Sub DoBumpRiver(character As ICharacter, effect As IEffect)
        Dim msg = character.World.CreateMessage.
            AddLine(LightGray, $"{character.Name} visits the river bank.").
            AddChoice(CoolStoryBro, EffectTypes.ExitDialog)
        If character.HasItemTypeInInventory(ItemTypes.ClayPot) Then
            msg.AddChoice("Fill Clay Pot with Water", EffectTypes.FillClayPot)
        End If
    End Sub
End Module
