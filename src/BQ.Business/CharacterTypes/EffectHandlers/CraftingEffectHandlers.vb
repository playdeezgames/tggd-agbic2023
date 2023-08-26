Friend Module CraftingEffectHandlers
    Friend Sub DoCookBagel(character As ICharacter, effect As IEffect)
        Dim recipeType = RecipeTypes.Bagel
        Dim taskName = "cook a bagel"
        Dim resultName = "cooks a bagel"
        CookFurnaceRecipe(character, recipeType, taskName, resultName)
    End Sub
    Friend Sub DoMakeTorch(character As ICharacter, effect As IEffect)
        If Not ConsumeEnergy(character, 1, "make a torch") Then
            Return
        End If
        If Not RecipeTypes.CanCraft(RecipeTypes.Torch, character) Then
            Dim msg = character.World.CreateMessage().
                AddLine(LightGray, $"To make a torch,").
                AddLine(LightGray, $"{character.Name} needs:")
            For Each input In RecipeTypes.Inputs(RecipeTypes.Torch)
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
            character.Cell.AddItem(ItemInitializer.CreateItem(character.World, effect.GetMetadata(Metadatas.ItemType)))
        End If
        character.Cell.TerrainType = effect.GetMetadata(Metadatas.TerrainType)
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
    Friend Sub DoBuildFurnace(character As ICharacter, effect As IEffect)
        If Not ConsumeEnergy(character, 1, "build a furnace") Then
            Return
        End If
        If Not RecipeTypes.CanCraft(RecipeTypes.Furnace, character) Then
            Dim msg = character.World.CreateMessage().
                AddLine(LightGray, $"To build a furnace,").
                AddLine(LightGray, $"{character.Name} needs:")
            For Each input In RecipeTypes.Inputs(RecipeTypes.Furnace)
                msg.AddLine(LightGray, $"{input.itemType.ToItemTypeDescriptor.Name}: {character.ItemTypeCount(input.itemType)}/{input.count}")
            Next
            Return
        End If
        RecipeTypes.Craft(RecipeTypes.Furnace, character)
        character.World.CreateMessage().
                AddLine(LightGray, $"{character.Name} builds a furnace.")
        character.Cell.TerrainType = TerrainTypes.Furnace
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

    Private Function CheckForFurnace(character As ICharacter, taskName As String) As Boolean
        If Not character.Cell.Descriptor.IsFurnace Then
            character.World.CreateMessage().
                AddLine(LightGray, $"{character.Name} needs a furnace to {taskName}.")
            Return False
        End If
        Return True
    End Function

    Private Function CheckForFire(character As ICharacter, taskName As String) As Boolean
        If Not character.Cell.Descriptor.HasFire Then
            character.World.CreateMessage().
                AddLine(LightGray, $"{character.Name} needs a fire to {taskName}.")
            Return False
        End If
        Return True
    End Function

    Friend Sub DoCookRatBody(character As ICharacter, effect As IEffect)
        Dim recipeType = RecipeTypes.CookedRatBody
        Dim taskName = "cook a rat body"
        Dim resultName = "cooks a rat body"
        CookRecipe(character, recipeType, taskName, resultName)
    End Sub

    Friend Sub DoCookRatCorpse(character As ICharacter, effect As IEffect)
        Dim recipeType = RecipeTypes.CookedRatCorpse
        Dim taskName = "cook a rat corpse"
        Dim resultName = "cooks a rat corpse"
        CookRecipe(character, recipeType, taskName, resultName)
    End Sub

    Private Sub CookRecipe(character As ICharacter, recipeType As String, taskName As String, resultName As String)
        If CheckForFire(character, taskName) Then
            DoRecipe(character, 0, recipeType, taskName, resultName)
        End If
    End Sub

    Private Sub CookFurnaceRecipe(character As ICharacter, recipeType As String, taskName As String, resultName As String)
        If CheckForFurnace(character, taskName) Then
            DoRecipe(character, 0, recipeType, taskName, resultName)
        End If
    End Sub

    Friend Sub DoMakeHatchet(character As ICharacter, effect As IEffect)
        If Not ConsumeEnergy(character, 1, "make a hatchet") Then
            Return
        End If
        If Not RecipeTypes.CanCraft(RecipeTypes.Hatchet, character) Then
            Dim msg = character.World.CreateMessage().
                AddLine(LightGray, $"To make a hatchet,").
                AddLine(LightGray, $"{character.Name} needs:")
            AddRecipeInputs(character, msg, RecipeTypes.Hatchet)
            Return
        End If
        RecipeTypes.Craft(RecipeTypes.Hatchet, character)
        character.World.CreateMessage().
                AddLine(LightGray, $"{character.Name} makes a hatchet.")
    End Sub

    Friend Sub AddRecipeInputs(character As ICharacter, msg As IMessage, recipeType As String)
        For Each input In RecipeTypes.Inputs(recipeType)
            msg.AddLine(LightGray, $"{input.itemType.ToItemTypeDescriptor.Name}: {character.ItemTypeCount(input.itemType)}/{input.count}")
        Next
    End Sub

    Private Function CanDoRecipe(character As ICharacter, recipeType As String, taskName As String) As Boolean
        If Not RecipeTypes.CanCraft(recipeType, character) Then
            Dim msg = character.World.CreateMessage().
                AddLine(LightGray, $"To {taskName},").
                AddLine(LightGray, $"{character.Name} needs:")
            For Each input In RecipeTypes.Inputs(recipeType)
                msg.AddLine(LightGray, $"{input.itemType.ToItemTypeDescriptor.Name}: {character.ItemTypeCount(input.itemType)}/{input.count}")
            Next
            Return False
        End If
        Return True
    End Function

    Private Sub DoRecipe(character As ICharacter, energyCost As Integer, recipeType As String, taskName As String, resultName As String)
        If CanDoRecipe(character, recipeType, taskName) Then
            If Not ConsumeEnergy(character, energyCost, taskName) Then
                Return
            End If
            RecipeTypes.Craft(recipeType, character)
            character.World.CreateMessage().
                AddLine(LightGray, $"{character.Name} {resultName}.")
        End If
    End Sub

    Friend Sub DoMillWheat(character As ICharacter, effect As IEffect)
        DoRecipe(character, 1, RecipeTypes.Flour, "make flour", "makes flour")
    End Sub

    Friend Sub DoMakeDough(character As ICharacter, effect As IEffect)
        DoRecipe(character, 2, RecipeTypes.Dough, "make dough", "makes dough")
    End Sub

    Friend Sub DoSmokePepper(character As ICharacter, effect As IEffect)
        CookRecipe(character, RecipeTypes.SmokedPepper, "smoke a pepper", "smokes a pepper")
    End Sub

    Friend Sub DoMakePaprika(character As ICharacter, effect As IEffect)
        DoRecipe(character, 1, RecipeTypes.Paprika, "make paprika", "makes paprika")
    End Sub
    Friend Sub DoSeasonRat(character As ICharacter, effect As IEffect)
        DoRecipe(character, 0, RecipeTypes.SeasonedRat, "season a rat", "seasons a rat")
    End Sub
End Module
