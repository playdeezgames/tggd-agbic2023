Imports SPLORR.Game

Friend Module ConsumableEffectHandlers
    Friend Sub DoEatRatCorpse(character As ICharacter, effect As IEffect)
        Dim item = CType(effect, IItemEffect).Item
        character.RemoveItem(item)
        item.Recycle()
        If RNG.GenerateBoolean(50, 50) Then
            character.SetHealth(character.Health + 1)
            character.World.CreateMessage().
                AddLine(LightGray, $"{item.Name} restores 1 health!").
                AddLine(LightGray, $"{character.Name} now has {character.Health}/{character.MaximumHealth} health")
        Else
            character.SetHealth(character.Health - 1)
            Dim msg = character.World.CreateMessage().
                AddLine(LightGray, $"{item.Name} is tainted!").
                AddLine(LightGray, $"{character.Name} loses 1 health!")
            If character.IsDead Then
                msg.AddLine(Red, $"{character.Name} dies.")
            Else
                msg.AddLine(LightGray, $"{character.Name} now has {character.Health}/{character.MaximumHealth} health")
            End If
        End If
    End Sub

    Friend Sub DoUseEnergyHerb(character As ICharacter, effect As IEffect)
        Dim item = CType(effect, IItemEffect).Item
        Const energyBenefit = 10
        character.AddEnergy(energyBenefit)
        character.World.
            CreateMessage().
            AddLine(LightGray, $"{character.Name} eats the {item.Name}.").
            AddLine(LightGray, $"{character.Name} regains energy!").
            AddLine(LightGray, $"{character.Name} now has {character.Energy}/{character.MaximumEnergy} energy.")
        character.RemoveItem(item)
        item.Recycle()
    End Sub

End Module
