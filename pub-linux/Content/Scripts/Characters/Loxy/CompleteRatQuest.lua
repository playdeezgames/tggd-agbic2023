
local jools = 0
local items = CharacterExtensions.ItemsOfItemType(character, "RatTail")
for i = 0,items.Length-1 do
    local item = items[i]
    jools = jools + 1
    character:RemoveItem(item)
    item:Recycle()
end
CharacterExtensions.AddJools(character, jools)
character.World:CreateMessage():AddLine(7, CharacterExtensions.Name(character) .. " receives " .. jools .. " jools.")