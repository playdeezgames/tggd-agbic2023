
require('Content.Scripts.teleport')
if character:GetFlag("RatQuest") then
    doTeleport(character,effect)
else
    character.World:CreateMessage():AddLine(7, CharacterExtensions.Name(character) .. " has no business in the cellar.")
end