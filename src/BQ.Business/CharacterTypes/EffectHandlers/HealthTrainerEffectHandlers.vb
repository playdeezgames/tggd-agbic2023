Friend Module HealthTrainerEffectHandlers

    Friend Sub DoHealthTrainerTalk(character As ICharacter, trigger As IEffect)
        Dim msg = character.World.CreateMessage().
                        AddLine(LightGray, "I am the health trainer!").
                        AddLine(LightGray, "I can help you increase yer health.").
                        AddLine(LightGray, $"The cost is {CharacterExtensions.MaximumHealth(character) * 5} AP.").
                        AddChoice(CoolStoryBro, EffectTypes.ExitDialog).
                        AddChoice(TrainMe, EffectTypes.TrainHealth)
    End Sub

    Friend Sub DoTrainHealth(character As ICharacter, trigger As IEffect)
        Dim msg = character.World.CreateMessage
        Const Multiplier = 5
        Dim TrainingCost = Multiplier * CharacterExtensions.MaximumHealth(character)
        If CharacterExtensions.AdvancementPoints(character) < TrainingCost Then
            msg.AddLine(LightGray, $"To go from {CharacterExtensions.MaximumHealth(character)} to {CharacterExtensions.MaximumHealth(character) + 1} Maximum Health,")
            msg.AddLine(LightGray, $"you need {TrainingCost} AP, but you have {CharacterExtensions.AdvancementPoints(character)}.")
            msg.AddLine(LightGray, "Come back when yer more experienced!")
            Return
        End If
        CharacterExtensions.AddAdvancementPoints(character, -TrainingCost)
        CharacterExtensions.SetMaximumHealth(character, CharacterExtensions.MaximumHealth(character) + 1)
        CharacterExtensions.SetHealth(character, CharacterExtensions.Health(character) + 1)
        msg.AddLine(Red, $"{CharacterExtensions.Name(character)} loses {TrainingCost} AP")
        msg.AddLine(Green, $"{CharacterExtensions.Name(character)} adds 1 Maximum Health")
        msg.AddLine(LightGray, $"Yer now at {CharacterExtensions.MaximumHealth(character)} Maximum Health.")
        msg.AddLine(LightGray, "Remember! If you don't have yer health,")
        msg.AddLine(LightGray, "you don't really have anything!")
    End Sub

End Module
