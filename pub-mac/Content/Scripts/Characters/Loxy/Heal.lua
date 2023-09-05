
local maximumHealth = math.min(CharacterExtensions.MaximumHealth(character), effect:GetStatistic("MaximumHealth"))
if CharacterExtensions.Health(character) >= maximumHealth then
    character.World:CreateMessage():AddLine(7, "Nothing happens!")
    return
end
CharacterExtensions.SetHealth(character, maximumHealth)
local msg =
    character.World:
        CreateMessage():
        AddLine(7, CharacterExtensions.Name(character) .. " is healed!"):
        AddLine(7, CharacterExtensions.Name(character) .. " now has " .. CharacterExtensions.Health(character) .. " health.")
local jools = math.floor(CharacterExtensions.Jools(character)/2)
CharacterExtensions.AddJools(character, -jools)
if jools > 0 then
    msg:AddLine(4, CharacterExtensions.Name(character) .. " loses {jools} jools!")
end