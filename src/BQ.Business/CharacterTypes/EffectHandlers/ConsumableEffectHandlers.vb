﻿Friend Module ConsumableEffectHandlers
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

    Friend Sub DoEatRatCorpse(character As ICharacter, effect As IEffect)
        Dim item As IItem = CharacterExtensions.ConsumedItem(character, effect)
        If RNG.GenerateBoolean(50, 50) Then
            CharacterExtensions.DoHealing(character, item, 1)
        Else
            CharacterExtensions.SetHealth(character, CharacterExtensions.Health(character) - 1)
            Dim msg = character.World.CreateMessage().
                AddLine(LightGray, $"{ItemExtensions.Name(item)} is tainted!").
                AddLine(LightGray, $"{CharacterExtensions.Name(character)} loses 1 health!")
            If CharacterExtensions.IsDead(character) Then
                msg.AddLine(Red, $"{CharacterExtensions.Name(character)} dies.")
            Else
                msg.AddLine(LightGray, $"{CharacterExtensions.Name(character)} now has {CharacterExtensions.Health(character)}/{CharacterExtensions.MaximumHealth(character)} health")
            End If
        End If
    End Sub

    Friend Sub DoEatCookedRat(character As ICharacter, effect As IEffect)
        Dim item As IItem = CharacterExtensions.ConsumedItem(character, effect)
        CharacterExtensions.DoHealing(character, item, 2)
    End Sub

    Friend Sub DoEatSeasonedRat(character As ICharacter, effect As IEffect)
        DoEatCookedRat(character, effect)
        CharacterExtensions.DetermineSpiciness(character, character.World.CreateMessage().AddLine(Orange, "That was a spicy one!"))
    End Sub

    Friend Sub DoUseEnergyHerb(character As ICharacter, effect As IEffect)
        Dim item As IItem = CharacterExtensions.ConsumedItem(character, effect)
        Const energyBenefit = 10
        CharacterExtensions.AddEnergy(character, energyBenefit)
        character.World.
            CreateMessage().
            AddLine(LightGray, $"{CharacterExtensions.Name(character)} eats the {ItemExtensions.Name(item)}.").
            AddLine(LightGray, $"{CharacterExtensions.Name(character)} regains energy!").
        AddLine(LightGray, $"{CharacterExtensions.Name(character)} now has {CharacterExtensions.Energy(character)}/{CharacterExtensions.MaximumEnergy(character)} energy.")
    End Sub

End Module
