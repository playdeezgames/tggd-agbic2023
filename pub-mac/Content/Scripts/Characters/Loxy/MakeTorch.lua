
if not CharacterExtensions.ConsumeEnergy(character, 1, "make a torch") then
    return
end
if not RecipeTypes.CanCraft("Torch", character) then
    local msg = character.World:CreateMessage():
        AddLine(7, "To make a torch,"):
        AddLine(7, CharacterExtensions.Name(character) .. " needs:")
    local inputs = RecipeTypes.Inputs("Torch")
    for i = 0,inputs.Length-1 do
        msg:AddLine(7, ItemTypes.ToItemTypeDescriptor(inputs[i].ItemType).Name .. ": " .. character:ItemTypeCount(inputs[i].ItemType) .. "/" .. inputs[i].Count)
    end
    return
end
RecipeTypes.Craft("Torch", character)
character.World:CreateMessage():
        AddLine(7, CharacterExtensions.Name(character) .. " makes a torch.")