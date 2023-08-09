Module PotterEffectHandlers
    Friend Sub DoPotterMakePot(character As ICharacter, effect As IEffect)
        Const price = 20
        Const RecipeType = RecipeTypes.UnfiredPot
        If character.Jools < price Then
            character.World.CreateMessage.
                        AddLine(LightGray, $"The price is {price} jools.").
                        AddLine(LightGray, $"({character.Name} has {character.Jools} jools)")
            Return
        End If
        If Not RecipeTypes.CanCraft(RecipeType, character) Then
            Dim msg = character.World.CreateMessage.AddLine(LightGray, "To make a pot, I need:")
            AddRecipeInputs(character, msg, RecipeType)
            Return
        End If
        character.AddJools(-price)
        RecipeTypes.Craft(RecipeType, character)
        RecipeTypes.Craft(RecipeTypes.ClayPot, character)
        character.World.CreateMessage.
            AddLine(Red, $"{character.Name} loses {price} jools").AddLine(LightGreen, $"{character.Name} gains 1 {ItemTypes.ClayPot.ToItemTypeDescriptor.Name}")
    End Sub

    Friend Sub DoPotterFlavorText(character As ICharacter, effect As IEffect)
        Dim msg = character.World.CreateMessage.
                        AddLine(LightGray, "Um. Thanks!").
                        AddLine(LightGray, "...").
                        AddLine(LightGray, "What's a 'Movie'?").
                        AddChoice("Nevermind!", EffectTypes.ExitDialog)
    End Sub

    Friend Sub DoPotterTalk(character As ICharacter, effect As IEffect)
        Dim msg = character.World.CreateMessage.
                        AddLine(LightGray, "Greetings! I am Harold, the Potter.").
                        AddLine(LightGray, "I make pots! For jools!").
                        AddChoice(CoolStoryBro, EffectTypes.PotterTalk).
                        AddChoice("I loved yer movies!", EffectTypes.PotterFlavorText).
                        AddChoice("Make me a pot!", EffectTypes.PotterMakePot)
    End Sub
End Module
