
local price = 20
local RecipeType = "UnfiredPot"
if CharacterExtensions.Jools(character) < price then
    character.World:CreateMessage():
                AddLine(7, "The price is " .. price .. " jools."):
                AddLine(7, CharacterExtensions.Name(character) .. " has " .. CharacterExtensions.Jools(character) .. " jools)")
    return
end
if not RecipeTypes.CanCraft(RecipeType, character) then
    local msg = character.World:CreateMessage():AddLine(7, "To make a pot, I need:")
    CharacterExtensions.AddRecipeInputs(character, msg, RecipeType)
    return
end
CharacterExtensions.AddJools(character, -price)
RecipeTypes.Craft(RecipeType, character)
RecipeTypes.Craft("ClayPot",character)
character.World:CreateMessage():
    AddLine(4, CharacterExtensions.Name(character) .. " loses " .. price .. " jools"):AddLine(10, CharacterExtensions.Name(character) .. " gains 1 " .. ItemTypes.ToItemTypeDescriptor("ClayPot").Name)