
local item = CharacterExtensions.ConsumedItem(character, effect)
local energyBenefit = 10
CharacterExtensions.AddEnergy(character, energyBenefit)
character.World:
    CreateMessage():
    AddLine(7, CharacterExtensions.Name(character) .. " eats the " .. ItemExtensions.Name(item) .. "."):
    AddLine(7, CharacterExtensions.Name(character) .. " regains energy!"):
    AddLine(7, CharacterExtensions.Name(character) .. " now has " .. CharacterExtensions.Energy(character) .. "/" .. CharacterExtensions.MaximumEnergy(character) .. " energy.")