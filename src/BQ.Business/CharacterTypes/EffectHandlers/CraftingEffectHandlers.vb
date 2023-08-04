Friend Module CraftingEffectHandlers
    Friend Sub DoCraftFire(character As ICharacter, effect As IEffect)
        If Not ConsumeEnergy(character, 1, "build a fire") Then
            Return
        End If
        Const SticksRequired = 5
        Const RocksRequired = 5
        Dim stickCount = character.ItemTypeCount(ItemTypes.Stick)
        Dim rockCount = character.ItemTypeCount(ItemTypes.Rock)
        If stickCount < SticksRequired OrElse rockCount < RocksRequired Then
            character.World.CreateMessage().
                AddLine(LightGray, $"To build a fire,").
                AddLine(LightGray, $"{character.Name} needs:").
                AddLine(LightGray, $"{SticksRequired} sticks (has {stickCount})").
                AddLine(LightGray, $"{RocksRequired} rocks (has {rockCount})")
            Return
        End If
        character.World.CreateMessage().
                AddLine(LightGray, $"{character.Name} builds a small fire.")
        character.Cell.TerrainType = TerrainTypes.CookingFire
    End Sub
    Friend Sub DoCutOffTail(character As ICharacter, effect As IEffect)
        If Not character.HasCuttingTool Then
            character.World.CreateMessage().AddLine(LightGray, $"{character.Name} needs a cutting tool for that!")
            Return
        End If
        character.RemoveItem(CType(effect, IItemEffect).Item)
        character.AddItem(ItemInitializer.CreateItem(character.World, ItemTypes.RatBody))
        character.AddItem(ItemInitializer.CreateItem(character.World, ItemTypes.RatTail))
    End Sub
End Module
