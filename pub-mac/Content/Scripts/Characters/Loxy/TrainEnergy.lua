
local msg = character.World:CreateMessage()
if CharacterExtensions.AdvancementPoints(character) < 1 then
    msg:AddLine(7, "You need at least 1 AP.")
    msg:AddLine(7, "Come back when yer more experienced!")
    return
end
local Multiplier = 2
local TrainingCost = Multiplier * CharacterExtensions.MaximumEnergy(character)
if CharacterExtensions.Jools(character) < TrainingCost then
    msg:
        AddLine(7, "The price is " .. TrainingCost .. " jools."):
        AddLine(7, "I have overhead, you know.")
    return
end
CharacterExtensions.AddAdvancementPoints(character, -1)
CharacterExtensions.AddJools(character, -TrainingCost)
CharacterExtensions.SetMaximumEnergy(character, CharacterExtensions.MaximumEnergy(character) + 1)
CharacterExtensions.AddEnergy(character, 1)
msg:AddLine(4, CharacterExtensions.Name(character) .. " loses 1 AP")
msg:AddLine(4, CharacterExtensions.Name(character) .. " loses " .. TrainingCost .. " jools")
msg:AddLine(2, CharacterExtensions.Name(character) .. " adds 1 Maximum Energy")
msg:AddLine(7, "Yer now at " .. CharacterExtensions.MaximumEnergy(character) .. " Maximum Energy.")
msg:AddLine(7, "Persistence is futile!")