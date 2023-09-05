
if character:GetFlag("PaidInnkeeper") then
    character:SetFlag("PaidInnkeeper", false)
    CharacterExtensions.AddEnergy(character, CharacterExtensions.MaximumEnergy(character) - CharacterExtensions.Energy(character))
    character.World:CreateMessage():
                AddLine(7, CharacterExtensions.Name(character) .. " rests and feels refreshed!"):
                AddLine(7, CharacterExtensions.Name(character) .. " has " .. CharacterExtensions.Energy(character) .. "/" .. CharacterExtensions.MaximumEnergy(character) .. " energy.")
else
    character.World:CreateMessage():
                AddLine(7, CharacterExtensions.Name(character) .. " needs to pay Gorachan first!")
end