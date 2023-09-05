
local msg = character.World:CreateMessage()
local Multiplier = 5
local TrainingCost = Multiplier * CharacterExtensions.MaximumHealth(character)
if CharacterExtensions.AdvancementPoints(character) < TrainingCost then
    msg:AddLine(7, "To go from " .. CharacterExtensions.MaximumHealth(character) .. " to " .. CharacterExtensions.MaximumHealth(character) + 1 .. " Maximum Health,")
    msg:AddLine(7, "you need " .. TrainingCost .. " AP, but you have " .. CharacterExtensions.AdvancementPoints(character) .. ".")
    msg:AddLine(7, "Come back when yer more experienced!")
    return
end
CharacterExtensions.AddAdvancementPoints(character, -TrainingCost)
CharacterExtensions.SetMaximumHealth(character, CharacterExtensions.MaximumHealth(character) + 1)
CharacterExtensions.SetHealth(character, CharacterExtensions.Health(character) + 1)
msg:AddLine(4, CharacterExtensions.Name(character) .. " loses " .. TrainingCost .. " AP")
msg:AddLine(2, CharacterExtensions.Name(character) .. " adds 1 Maximum Health")
msg:AddLine(7, "Yer now at " .. CharacterExtensions.MaximumHealth(character) .. " Maximum Health.")
msg:AddLine(7, "Remember! if you don't have yer health,")
msg:AddLine(7, "you don't really have anything!")