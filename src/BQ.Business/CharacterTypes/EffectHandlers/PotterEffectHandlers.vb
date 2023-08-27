Module PotterEffectHandlers
    Friend Sub DoPotterMakePot(character As ICharacter, effect As IEffect)
        Const price = 20
        Const RecipeType = RecipeTypes.UnfiredPot
        If CharacterExtensions.Jools(character) < price Then
            character.World.CreateMessage.
                        AddLine(LightGray, $"The price is {price} jools.").
                        AddLine(LightGray, $"({CharacterExtensions.Name(character)} has {CharacterExtensions.Jools(character)} jools)")
            Return
        End If
        If Not RecipeTypes.CanCraft(RecipeType, character) Then
            Dim msg = character.World.CreateMessage.AddLine(LightGray, "To make a pot, I need:")
            AddRecipeInputs(character, msg, RecipeType)
            Return
        End If
        CharacterExtensions.AddJools(character, -price)
        RecipeTypes.Craft(RecipeType, character)
        RecipeTypes.Craft(RecipeTypes.ClayPot, character)
        character.World.CreateMessage.
            AddLine(Red, $"{CharacterExtensions.Name(character)} loses {price} jools").AddLine(LightGreen, $"{CharacterExtensions.Name(character)} gains 1 {ItemTypes.ClayPot.ToItemTypeDescriptor.Name}")
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
                        AddChoice(CoolStoryBro, EffectTypes.ExitDialog).
                        AddChoice("I loved yer movies!", EffectTypes.PotterFlavorText).
                        AddChoice("Make me a pot!", EffectTypes.PotterMakePot)
    End Sub
End Module
