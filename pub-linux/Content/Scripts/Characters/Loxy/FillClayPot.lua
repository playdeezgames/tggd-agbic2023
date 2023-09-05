
RecipeTypes.Craft("WaterPot", character)
character.World:CreateMessage():AddLine(7, CharacterExtensions.Name(character) .. " fills a " .. ItemTypes.ToItemTypeDescriptor("ClayPot").Name .. " with water.")