﻿Friend Module ConsumableEffectHandlers

    Friend Sub DoEatCookedRat(character As ICharacter, effect As IEffect)
        Dim item As IItem = CharacterExtensions.ConsumedItem(character, effect)
        CharacterExtensions.DoHealing(character, item, 2)
    End Sub

    Friend Sub DoEatSeasonedRat(character As ICharacter, effect As IEffect)
        DoEatCookedRat(character, effect)
        CharacterExtensions.DetermineSpiciness(character, character.World.CreateMessage().AddLine(Orange, "That was a spicy one!"))
    End Sub
End Module
