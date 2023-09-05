
        if not CharacterExtensions.ConsumeEnergy(character, 1, "build a furnace") then
            return
        end
        if not RecipeTypes.CanCraft("Furnace", character) then
            local msg = character.World:CreateMessage():
                AddLine(7, "To build a furnace,"):
                AddLine(7, CharacterExtensions.Name(character) .. " needs:")
            local recipeName = "Furnace"
            CharacterExtensions.ReportNeededRecipeInputs(character, msg, recipeName)
            return
        end
        RecipeTypes.Craft("Furnace", character)
        character.World:CreateMessage():
                AddLine(7, CharacterExtensions.Name(character) .. " builds a furnace.")
        character.Cell.TerrainType = "Furnace"