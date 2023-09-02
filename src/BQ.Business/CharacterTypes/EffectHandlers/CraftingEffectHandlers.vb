Friend Module CraftingEffectHandlers

    Friend Sub DoMakeHatchet(character As ICharacter, effect As IEffect)
        If Not CharacterExtensions.ConsumeEnergy(character, 1, "make a hatchet") Then
            Return
        End If
        If Not RecipeTypes.CanCraft("Hatchet", character) Then
            Dim msg = character.World.CreateMessage().
                AddLine(LightGray, $"To make a hatchet,").
                AddLine(LightGray, $"{CharacterExtensions.Name(character)} needs:")
            AddRecipeInputs(character, msg, "Hatchet")
            Return
        End If
        RecipeTypes.Craft("Hatchet", character)
        character.World.CreateMessage().
                AddLine(LightGray, $"{CharacterExtensions.Name(character)} makes a hatchet.")
    End Sub

    Friend Sub AddRecipeInputs(character As ICharacter, msg As IMessage, recipeType As String)
        For Each input In RecipeTypes.Inputs(recipeType)
            msg.AddLine(LightGray, $"{ToItemTypeDescriptor(input.itemType).Name}: {character.ItemTypeCount(input.itemType)}/{input.count}")
        Next
    End Sub

    Friend Sub DoMillWheat(character As ICharacter, effect As IEffect)
        CharacterExtensions.DoRecipe(character, 1, "Flour", "make flour", "makes flour")
    End Sub

    Friend Sub DoSmokePepper(character As ICharacter, effect As IEffect)
        CharacterExtensions.CookRecipe(character, "SmokedPepper", "smoke a pepper", "smokes a pepper")
    End Sub

    Friend Sub DoMakePaprika(character As ICharacter, effect As IEffect)
        CharacterExtensions.DoRecipe(character, 1, "Paprika", "make paprika", "makes paprika")
    End Sub
    Friend Sub DoSeasonRat(character As ICharacter, effect As IEffect)
        CharacterExtensions.DoRecipe(character, 0, "SeasonedRat", "season a rat", "seasons a rat")
    End Sub
End Module
