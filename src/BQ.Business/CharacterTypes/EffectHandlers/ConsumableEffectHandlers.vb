Friend Module ConsumableEffectHandlers
    Friend Sub DoEatRatCorpse(character As ICharacter, effect As IEffect)
        Dim item As IItem = ConsumedItem(character, effect)
        If RNG.GenerateBoolean(50, 50) Then
            DoHealing(character, item, 1)
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

    Private Function ConsumedItem(character As ICharacter, effect As IEffect) As IItem
        Dim item = CType(effect, IItemEffect).Item
        character.RemoveItem(item)
        item.Recycle()
        Return item
    End Function

    Private Sub DoHealing(character As ICharacter, item As IItem, amount As Integer)
        character.SetHealth(character.Health + amount)
        character.World.CreateMessage().
            AddLine(LightGray, $"{item.Name} restores {amount} health!").
            AddLine(LightGray, $"{character.Name} now has {character.Health}/{character.MaximumHealth} health")
    End Sub

    Friend Sub DoEatCookedRat(character As ICharacter, effect As IEffect)
        Dim item As IItem = ConsumedItem(character, effect)
        DoHealing(character, item, 2)
    End Sub

    Friend Sub DoUseEnergyHerb(character As ICharacter, effect As IEffect)
        Dim item As IItem = ConsumedItem(character, effect)
        Const energyBenefit = 10
        character.AddEnergy(energyBenefit)
        character.World.
            CreateMessage().
            AddLine(LightGray, $"{character.Name} eats the {item.Name}.").
            AddLine(LightGray, $"{character.Name} regains energy!").
        AddLine(LightGray, $"{character.Name} now has {character.Energy}/{character.MaximumEnergy} energy.")
    End Sub

End Module
