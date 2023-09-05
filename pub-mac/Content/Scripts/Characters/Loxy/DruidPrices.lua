
local msg = character.World:CreateMessage():
                AddLine(7, "I sell a variety of herbs."):
                AddLine(7, "(" .. CharacterExtensions.Name(character) .. " has " .. CharacterExtensions.Jools(character) .. " jools)"):
                AddChoice("Good to know!", "ExitDialog"):
                AddChoice("Buy Energy Herb(5 jools)","Buy")
msg.LastChoice:SetMetadata("ItemType", "EnergyHerb")
msg.LastChoice:SetStatistic("Price", 5)
msg.LastChoice:SetMetadata("EffectType", "DruidPrices")