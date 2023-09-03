Friend Module HealerEffectHandlers

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
            msg.AddLine(4, $"{CharacterExtensions.Name(character)} loses {jools} jools!")
        End If
    End Sub

End Module
