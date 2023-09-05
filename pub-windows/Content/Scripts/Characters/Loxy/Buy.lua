
        local itemType = effect:GetMetadata("ItemType")
        local price = effect:GetStatistic("Price")
        if CharacterExtensions.Jools(character) < price then
            character.World:
                CreateMessage():
                AddLine(7, "You don't have enough!"):
                AddChoice("Shucks!", effect:GetMetadata("EffectType"))
            return
        end
        CharacterExtensions.AddJools(character, -price)
        character:AddItem(ItemInitializer.CreateItem(character.World, itemType))
        character.World:
            CreateMessage():
            AddLine(7, "Thank you for yer purchase!"):
            AddChoice("No worries!", effect:GetMetadata("EffectType"))