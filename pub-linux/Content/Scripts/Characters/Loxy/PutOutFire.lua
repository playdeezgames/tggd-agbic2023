
if not CharacterExtensions.ConsumeEnergy(character, 1, "put out a fire") then
    return
end
character.World:CreateMessage():
        AddLine(7, CharacterExtensions.Name(character) .. " extinguishes the fire.")
if effect:HasMetadata("ItemType") then
    character.Cell:AddItem(ItemInitializer.CreateItem(character.World, effect:GetMetadata("ItemType")))
end
character.Cell.TerrainType = effect:GetMetadata("TerrainType")