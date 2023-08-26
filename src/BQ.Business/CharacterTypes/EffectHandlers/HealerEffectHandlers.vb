Friend Module HealerEffectHandlers
    Friend Sub DoHealerTalk(character As ICharacter, trigger As IEffect)
        Dim msg = character.World.CreateMessage.
                        AddLine(LightGray, "Welcome to the Nihilistic House of Healing.").
                        AddLine(LightGray, "If you go to the basin And wash,").
                        AddLine(LightGray, "you will be healed,").
                        AddLine(LightGray, "but it will cost you half of yer jools.").
                        AddLine(LightGray, "Not that I care or anything,").
                        AddLine(LightGray, "because I'm a nihilist.").
                        AddChoice(CoolStoryBro, EffectTypes.ExitDialog).
                        AddChoice("What's for sale?", EffectTypes.HealerPrices)
    End Sub

    Friend Sub DoNihilistPrices(character As ICharacter, trigger As IEffect)
        character.World.CreateMessage().
            AddLine(LightGray, "I don't sell anything.").
            AddLine(LightGray, "I'm a nihilist, remember?")
    End Sub
    Friend Sub NihilisticHealing(character As ICharacter, effect As IEffect)
        Dim maximumHealth = Math.Min(character.MaximumHealth, effect.GetStatistic(StatisticTypes.MaximumHealth))
        If character.Health >= maximumHealth Then
            character.World.CreateMessage().AddLine(LightGray, "Nothing happens!")
            Return
        End If
        character.SetHealth(maximumHealth)
        Dim msg =
            character.World.
                CreateMessage().
                AddLine(LightGray, $"{character.Name} is healed!").
                AddLine(LightGray, $"{character.Name} now has {character.Health} health.")
        Dim jools = character.Jools \ 2
        character.AddJools(-jools)
        If jools > 0 Then
            msg.AddLine(Red, $"{character.Name} loses {jools} jools!")
        End If
    End Sub

End Module
