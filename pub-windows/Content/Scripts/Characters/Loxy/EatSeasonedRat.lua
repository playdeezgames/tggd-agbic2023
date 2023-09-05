
local item = CharacterExtensions.ConsumedItem(character, effect)
CharacterExtensions.DoHealing(character, item, 2)
CharacterExtensions.DetermineSpiciness(character, character.World:CreateMessage():AddLine(11, "That was a spicy one!"))