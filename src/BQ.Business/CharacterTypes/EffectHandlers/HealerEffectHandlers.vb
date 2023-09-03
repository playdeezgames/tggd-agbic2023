Friend Module HealerEffectHandlers
    Friend Sub DoHealerTalk(character As ICharacter, trigger As IEffect)
        Dim msg = character.World.CreateMessage().
                        AddLine(7, "Welcome to the Nihilistic House of Healing.").
                        AddLine(7, "If you go to the basin And wash,").
                        AddLine(7, "you will be healed,").
                        AddLine(7, "but it will cost you half of yer jools.").
                        AddLine(7, "Not that I care or anything,").
                        AddLine(7, "because I'm a nihilist.").
                        AddChoice("Cool story, bro!", "ExitDialog").
                        AddChoice("What's for sale?", "NihilistPrices")
    End Sub

    Friend Sub DoNihilistPrices(character As ICharacter, trigger As IEffect)
        character.World.CreateMessage().
            AddLine(7, "I don't sell anything.").
            AddLine(7, "I'm a nihilist, remember?")
    End Sub
    Friend Sub NihilisticHealing(character As ICharacter, effect As IEffect)
        Dim maximumHealth = Math.Min(CharacterExtensions.MaximumHealth(character), effect.GetStatistic("MaximumHealth"))
        If CharacterExtensions.Health(character) >= maximumHealth Then
            character.World.CreateMessage().AddLine(7, "Nothing happens!")
            Return
        End If
        CharacterExtensions.SetHealth(character, maximumHealth)
        Dim msg =
            character.World.
                CreateMessage().
                AddLine(7, $"{CharacterExtensions.Name(character)} is healed!").
                AddLine(7, $"{CharacterExtensions.Name(character)} now has {CharacterExtensions.Health(character)} health.")
        Dim jools = CharacterExtensions.Jools(character) \ 2
        CharacterExtensions.AddJools(character, -jools)
        If jools > 0 Then
            msg.AddLine(Red, $"{CharacterExtensions.Name(character)} loses {jools} jools!")
        End If
    End Sub

End Module
