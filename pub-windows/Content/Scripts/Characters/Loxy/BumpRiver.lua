
        local msg = character.World:CreateMessage():
            AddLine(7, CharacterExtensions.Name(character) .. " visits the river bank."):
            AddChoice("Cool story, bro!", "ExitDialog")
        if CharacterExtensions.HasItemTypeInInventory(character, "ClayPot") then
            msg:AddChoice("Fill Clay Pot with Water", "FillClayPot")
        end