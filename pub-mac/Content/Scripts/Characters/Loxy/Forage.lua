
local cell = EffectExtensions.ToTerrainEffect(effect).Cell
if not CharacterExtensions.ConsumeEnergy(character, 1, "forage") then
    return
end
local itemType = CellExtensions.GenerateForageItemType(cell)
if System.String.IsNullOrEmpty(itemType) then
    character.World:CreateMessage():AddLine(7, CharacterExtensions.Name(character) .. " finds nothing.")
    return
end
local item = ItemInitializer.CreateItem(character.World, itemType)
character:AddItem(item)
character.World:CreateMessage():AddLine(7, CharacterExtensions.Name(character) .. " finds " .. ItemExtensions.Name(item))