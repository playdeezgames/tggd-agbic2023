if not CharacterExtensions.ConsumeEnergy(character, 1, "build a fire") then
    return
end
if not RecipeTypes.CanCraft("CookingFire", character) then
    local msg = character.World:CreateMessage():
        AddLine(7, "To build a fire,"):
        AddLine(7, CharacterExtensions.Name(character) .. " needs:")
    local inputs = RecipeTypes.Inputs("CookingFire")
    for i = 0, inputs.Length-1 do
        local input = inputs[i]
        msg:AddLine(7, ItemTypes.ToItemTypeDescriptor(input.ItemType).Name .. ": " .. character:ItemTypeCount(input.ItemType) .. "/" .. input.Count)
    end
    return
end
RecipeTypes.Craft("CookingFire", character)
character.World:CreateMessage():
        AddLine(7, CharacterExtensions.Name(character) .. " builds a small fire.")
character.Cell.TerrainType = "CookingFire"