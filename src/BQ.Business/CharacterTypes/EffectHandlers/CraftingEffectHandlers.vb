﻿Friend Module CraftingEffectHandlers
    Friend Sub DoBuildFurnace(character As ICharacter, effect As IEffect)
        If Not CharacterExtensions.ConsumeEnergy(character, 1, "build a furnace") Then
            Return
        End If
        If Not RecipeTypes.CanCraft("Furnace", character) Then
            Dim msg = character.World.CreateMessage().
                AddLine(LightGray, $"To build a furnace,").
                AddLine(LightGray, $"{CharacterExtensions.Name(character)} needs:")
            For Each input In RecipeTypes.Inputs("Furnace")
                msg.AddLine(LightGray, $"{ToItemTypeDescriptor(input.ItemType).Name}: {character.ItemTypeCount(input.ItemType)}/{input.Count}")
            Next
            Return
        End If
        RecipeTypes.Craft("Furnace", character)
        character.World.CreateMessage().
                AddLine(LightGray, $"{CharacterExtensions.Name(character)} builds a furnace.")
        character.Cell.TerrainType = TerrainTypes.Furnace
    End Sub
    Friend Sub DoCutOffTail(character As ICharacter, effect As IEffect)
        If Not CharacterExtensions.HasCuttingTool(character) Then
            character.World.CreateMessage().AddLine(LightGray, $"{CharacterExtensions.Name(character)} needs a cutting tool for that!")
            Return
        End If
        character.RemoveItem(CType(effect, IItemEffect).Item)
        character.AddItem(ItemInitializer.CreateItem(character.World, "RatBody"))
        character.AddItem(ItemInitializer.CreateItem(character.World, "RatTail"))
    End Sub

    Friend Sub DoCookRatBody(character As ICharacter, effect As IEffect)
        Dim recipeType = "CookedRatBody"
        Dim taskName = "cook a rat body"
        Dim resultName = "cooks a rat body"
        CharacterExtensions.CookRecipe(character, recipeType, taskName, resultName)
    End Sub

    Friend Sub DoCookRatCorpse(character As ICharacter, effect As IEffect)
        Dim recipeType = "CookedRatCorpse"
        Dim taskName = "cook a rat corpse"
        Dim resultName = "cooks a rat corpse"
        CharacterExtensions.CookRecipe(character, recipeType, taskName, resultName)
    End Sub

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

    Friend Sub DoMakeDough(character As ICharacter, effect As IEffect)
        CharacterExtensions.DoRecipe(character, 2, "Dough", "make dough", "makes dough")
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
