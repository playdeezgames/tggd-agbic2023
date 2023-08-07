Friend Module CraftingEffectHandlers
    Friend Sub DoMakeTorch(character As ICharacter, effect As IEffect)
        If Not ConsumeEnergy(character, 1, "make a torch") Then
            Return
        End If
        If Not RecipeTypes.CanCraft(RecipeTypes.Torch, character) Then
            Dim msg = character.World.CreateMessage().
                AddLine(LightGray, $"To make a torch,").
                AddLine(LightGray, $"{character.Name} needs:")
            For Each input In RecipeTypes.Inputs(RecipeTypes.CookingFire)
                msg.AddLine(LightGray, $"{input.itemType.ToItemTypeDescriptor.Name}: {character.ItemTypeCount(input.itemType)}/{input.count}")
            Next
            Return
        End If
        RecipeTypes.Craft(RecipeTypes.Torch, character)
        character.World.CreateMessage().
                AddLine(LightGray, $"{character.Name} makes a torch.")
    End Sub
    Friend Sub DoPutOutFlames(character As ICharacter, effect As IEffect)
        If Not ConsumeEnergy(character, 1, "put out a fire") Then
            Return
        End If
        character.World.CreateMessage().
                AddLine(LightGray, $"{character.Name} extinguishes the fire.")
        If effect.HasMetadata(Metadatas.ItemType) Then
            character.Cell.AddItem(ItemInitializer.CreateItem(character.World, effect.Metadata(Metadatas.ItemType)))
        End If
        character.Cell.TerrainType = effect.Metadata(Metadatas.TerrainType)
    End Sub
    Friend Sub DoCraftFire(character As ICharacter, effect As IEffect)
        If Not ConsumeEnergy(character, 1, "build a fire") Then
            Return
        End If
        If Not RecipeTypes.CanCraft(RecipeTypes.CookingFire, character) Then
            Dim msg = character.World.CreateMessage().
                AddLine(LightGray, $"To build a fire,").
                AddLine(LightGray, $"{character.Name} needs:")
            For Each input In RecipeTypes.Inputs(RecipeTypes.CookingFire)
                msg.AddLine(LightGray, $"{input.itemType.ToItemTypeDescriptor.Name}: {character.ItemTypeCount(input.itemType)}/{input.count}")
            Next
            Return
        End If
        RecipeTypes.Craft(RecipeTypes.CookingFire, character)
        character.World.CreateMessage().
                AddLine(LightGray, $"{character.Name} builds a small fire.")
        character.Cell.TerrainType = TerrainTypes.CookingFire
    End Sub
    Friend Sub DoCutOffTail(character As ICharacter, effect As IEffect)
        If Not character.HasCuttingTool Then
            character.World.CreateMessage().AddLine(LightGray, $"{character.Name} needs a cutting tool for that!")
            Return
        End If
        character.RemoveItem(CType(effect, IItemEffect).Item)
        character.AddItem(ItemInitializer.CreateItem(character.World, ItemTypes.RatBody))
        character.AddItem(ItemInitializer.CreateItem(character.World, ItemTypes.RatTail))
    End Sub

    Friend Sub DoCookRatBody(character As ICharacter, effect As IEffect)
        Dim recipeType = RecipeTypes.CookedRatBody
        Dim taskName = "cook a rat body"
        Dim resultName = "cooks a rat body"
        If Not character.Cell.Descriptor.HasFire Then
            character.World.CreateMessage().
                AddLine(LightGray, $"{character.Name} needs a fire to {taskName}.")
            Return
        End If
        DoRecipe(character, recipeType, taskName, resultName)
    End Sub

    Friend Sub DoCookRatCorpse(character As ICharacter, effect As IEffect)
        Dim recipeType = RecipeTypes.CookedRatCorpse
        Dim taskName = "cook a rat corpse"
        Dim resultName = "cooks a rat corpse"
        If Not character.Cell.Descriptor.HasFire Then
            character.World.CreateMessage().
                AddLine(LightGray, $"{character.Name} needs a fire to {taskName}.")
            Return
        End If
        DoRecipe(character, recipeType, taskName, resultName)
    End Sub

    Private Sub DoRecipe(character As ICharacter, recipeType As String, taskName As String, resultName As String)
        If Not RecipeTypes.CanCraft(recipeType, character) Then
            Dim msg = character.World.CreateMessage().
                AddLine(LightGray, $"To {taskName},").
                AddLine(LightGray, $"{character.Name} needs:")
            For Each input In RecipeTypes.Inputs(recipeType)
                msg.AddLine(LightGray, $"{input.itemType.ToItemTypeDescriptor.Name}: {character.ItemTypeCount(input.itemType)}/{input.count}")
            Next
            Return
        End If
        RecipeTypes.Craft(recipeType, character)
        character.World.CreateMessage().
                AddLine(LightGray, $"{character.Name} {resultName}.")
    End Sub
End Module
