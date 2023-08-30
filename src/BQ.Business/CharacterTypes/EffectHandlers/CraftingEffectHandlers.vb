Friend Module CraftingEffectHandlers
    Friend Sub DoCookBagel(character As ICharacter, effect As IEffect)
        Dim recipeType = "Bagel"
        Dim taskName = "cook a bagel"
        Dim resultName = "cooks a bagel"
        CookFurnaceRecipe(character, recipeType, taskName, resultName)
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
                msg.AddLine(LightGray, $"{input.itemType.ToItemTypeDescriptor.Name}: {character.ItemTypeCount(input.itemType)}/{input.count}")
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
                msg.AddLine(LightGray, $"{input.itemType.ToItemTypeDescriptor.Name}: {character.ItemTypeCount(input.itemType)}/{input.count}")
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
                msg.AddLine(LightGray, $"{input.itemType.ToItemTypeDescriptor.Name}: {character.ItemTypeCount(input.itemType)}/{input.count}")
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
        character.AddItem(ItemInitializer.CreateItem(character.World, ItemTypes.RatTail))
    End Sub

    Private Function CheckForFurnace(character As ICharacter, taskName As String) As Boolean
        If Not character.Cell.Descriptor.IsFurnace Then
            character.World.CreateMessage().
                AddLine(LightGray, $"{CharacterExtensions.Name(character)} needs a furnace to {taskName}.")
            Return False
        End If
        Return True
    End Function

    Private Function CheckForFire(character As ICharacter, taskName As String) As Boolean
        If Not character.Cell.Descriptor.HasFire Then
            character.World.CreateMessage().
                AddLine(LightGray, $"{CharacterExtensions.Name(character)} needs a fire to {taskName}.")
            Return False
        End If
        Return True
    End Function

    Friend Sub DoCookRatBody(character As ICharacter, effect As IEffect)
        Dim recipeType = "CookedRatBody"
        Dim taskName = "cook a rat body"
        Dim resultName = "cooks a rat body"
        CookRecipe(character, recipeType, taskName, resultName)
    End Sub

    Friend Sub DoCookRatCorpse(character As ICharacter, effect As IEffect)
        Dim recipeType = "CookedRatCorpse"
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
            msg.AddLine(LightGray, $"{input.itemType.ToItemTypeDescriptor.Name}: {character.ItemTypeCount(input.itemType)}/{input.count}")
        Next
    End Sub

    Private Function CanDoRecipe(character As ICharacter, recipeType As String, taskName As String) As Boolean
        If Not RecipeTypes.CanCraft(recipeType, character) Then
            Dim msg = character.World.CreateMessage().
                AddLine(LightGray, $"To {taskName},").
                AddLine(LightGray, $"{CharacterExtensions.Name(character)} needs:")
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
                AddLine(LightGray, $"{CharacterExtensions.Name(character)} {resultName}.")
        End If
    End Sub

    Friend Sub DoMillWheat(character As ICharacter, effect As IEffect)
        DoRecipe(character, 1, "Flour", "make flour", "makes flour")
    End Sub

    Friend Sub DoMakeDough(character As ICharacter, effect As IEffect)
        DoRecipe(character, 2, "Dough", "make dough", "makes dough")
    End Sub

    Friend Sub DoSmokePepper(character As ICharacter, effect As IEffect)
        CookRecipe(character, "SmokedPepper", "smoke a pepper", "smokes a pepper")
    End Sub

    Friend Sub DoMakePaprika(character As ICharacter, effect As IEffect)
        DoRecipe(character, 1, "Paprika", "make paprika", "makes paprika")
    End Sub
    Friend Sub DoSeasonRat(character As ICharacter, effect As IEffect)
        DoRecipe(character, 0, "SeasonedRat", "season a rat", "seasons a rat")
    End Sub
End Module
