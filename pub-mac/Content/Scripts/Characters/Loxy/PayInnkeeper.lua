
if character:GetFlag("PaidInnkeeper") then
    character.World:CreateMessage():
                AddLine(7, "You've already paid!")
    return
end
local bedCost = 1
if CharacterExtensions.Jools(character) < bedCost then
    character.World:CreateMessage():
                AddLine(7, "Sorry! No jools, no bed!")
    return
end
CharacterExtensions.AddJools(character, -bedCost)
character:SetFlag("PaidInnkeeper", true)
character.World:CreateMessage():
                AddLine(7, "Thanks for yer business."):
                AddLine(7, "Choose any bed you like.")