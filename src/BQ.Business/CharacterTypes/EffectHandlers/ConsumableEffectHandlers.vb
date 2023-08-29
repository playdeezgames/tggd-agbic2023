Friend Module ConsumableEffectHandlers
    Friend Sub DoEatPepper(character As ICharacter, effect As IEffect)
        Dim item As IItem = CharacterExtensions.ConsumedItem(character, effect)
        Dim msg = character.World.CreateMessage().AddLine(LightGray, $"{CharacterExtensions.Name(character)} eats the pepper.")
        CharacterExtensions.DetermineSpiciness(character, msg)
    End Sub

    Friend Sub DoEatSmokedPepper(character As ICharacter, effect As IEffect)
        Dim item As IItem = CharacterExtensions.ConsumedItem(character, effect)
        Dim msg = character.World.CreateMessage().AddLine(LightGray, $"{CharacterExtensions.Name(character)} eats the smoked pepper.")
        CharacterExtensions.DetermineSpiciness(character, msg)
    End Sub

    Friend Sub DoEatCookedRat(character As ICharacter, effect As IEffect)
        Dim item As IItem = CharacterExtensions.ConsumedItem(character, effect)
        CharacterExtensions.DoHealing(character, item, 2)
    End Sub

    Friend Sub DoEatSeasonedRat(character As ICharacter, effect As IEffect)
        DoEatCookedRat(character, effect)
        CharacterExtensions.DetermineSpiciness(character, character.World.CreateMessage().AddLine(Orange, "That was a spicy one!"))
    End Sub
End Module
