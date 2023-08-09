Module PotterEffectHandlers
    Friend Sub DoPotterTalk(character As ICharacter, effect As IEffect)
        Dim msg = character.World.CreateMessage.
                        AddLine(LightGray, "Greetings! I am Harold, the Potter.").
                        AddChoice(CoolStoryBro, EffectTypes.ExitDialog)
    End Sub
End Module
