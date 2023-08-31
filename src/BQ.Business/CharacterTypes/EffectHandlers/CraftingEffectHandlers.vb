Friend Module CraftingEffectHandlers
    Friend Sub DoCookBagel(character As ICharacter, effect As IEffect)
        Dim recipeType = "Bagel"
        Dim taskName = "cook a bagel"
        Dim resultName = "cooks a bagel"
        CharacterExtensions.CookFurnaceRecipe(character, recipeType, taskName, resultName)
    End Sub
    Friend Sub DoMakeTorch(character As ICharacter, effect As IEffect)
        If Not ConsumeEnergy(character, 1, "make a torch") Then
            Return
        End If
        If Not RecipeTypes.CanCraft("Torch", character) Then
            Dim msg = character.World.CreateMessage().
                AddLine(LightGray, $"To make a torch,").
                AddLine(LightGray, $"{CharacterExtensions.Name(character)} needs:")
            For Each input In RecipeTypes.Inputs("Torch")
                msg.AddLine(LightGray, $"{ToItemTypeDescriptor(input.itemType).Name}: {character.ItemTypeCount(input.itemType)}/{input.count}")
            Next
            Return
        End If
        RecipeTypes.Craft("Torch", character)
        character.World.CreateMessage().
                AddLine(LightGray, $"{CharacterExtensions.Name(character)} makes a torch.")
    End Sub
    Friend Sub DoPutOutFlames(character As ICharacter, effect As IEffect)
        If Not ConsumeEnergy(character, 1, "put out a fire") Then
            Return
        End If
        character.World.CreateMessage().
                AddLine(LightGray, $"{CharacterExtensions.Name(character)} extinguishes the fire.")
        If effect.HasMetadata(Metadatas.ItemType) Then
            character.Cell.AddItem(ItemInitializer.CreateItem(character.World, effect.GetMetadata(Metadatas.ItemType)))
        End If
        character.Cell.TerrainType = effect.GetMetadata(Metadatas.TerrainType)
    End Sub
    Friend Sub DoCraftFire(character As ICharacter, effect As IEffect)
        If Not ConsumeEnergy(character, 1, "build a fire") Then
            Return
        End If
        If Not RecipeTypes.CanCraft("CookingFire", character) Then
            Dim msg = character.World.CreateMessage().
                AddLine(LightGray, $"To build a fire,").
                AddLine(LightGray, $"{CharacterExtensions.Name(character)} needs:")
            For Each input In RecipeTypes.Inputs("CookingFire")
                msg.AddLine(LightGray, $"{ToItemTypeDescriptor(input.itemType).Name}: {character.ItemTypeCount(input.itemType)}/{input.count}")
            Next
            Return
        End If
        RecipeTypes.Craft("CookingFire", character)
        character.World.CreateMessage().
                AddLine(LightGray, $"{CharacterExtensions.Name(character)} builds a small fire.")
        character.Cell.TerrainType = TerrainTypes.CookingFire
    End Sub
    Friend Sub DoBuildFurnace(character As ICharacter, effect As IEffect)
        If Not ConsumeEnergy(character, 1, "build a furnace") Then
            Return
        End If
        If Not RecipeTypes.CanCraft("Furnace", character) Then
            Dim msg = character.World.CreateMessage().
                AddLine(LightGray, $"To build a furnace,").
                AddLine(LightGray, $"{CharacterExtensions.Name(character)} needs:")
            For Each input In RecipeTypes.Inputs("Furnace")
                msg.AddLine(LightGray, $"{ToItemTypeDescriptor(input.itemType).Name}: {character.ItemTypeCount(input.itemType)}/{input.count}")
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
        If Not ConsumeEnergy(character, 1, "make a hatchet") Then
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
