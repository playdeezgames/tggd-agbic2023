
local item = CharacterExtensions.ConsumedItem(character, effect)
if RNG.GenerateBoolean(50, 50) then
    CharacterExtensions.DoHealing(character, item, 1)
else
    CharacterExtensions.SetHealth(character, CharacterExtensions.Health(character) - 1)
    local msg = character.World:CreateMessage():
        AddLine(7, ItemExtensions.Name(item) .. " is tainted!"):
        AddLine(7, CharacterExtensions.Name(character) .. " loses 1 health!")
    if CharacterExtensions.IsDead(character) then
        msg:AddLine(4, CharacterExtensions.Name(character) .. " dies.")
    else
        msg:AddLine(7, CharacterExtensions.Name(character) .. " now has " .. CharacterExtensions.Health(character) .. "/" .. CharacterExtensions.MaximumHealth(character) .. " health")
    end
end