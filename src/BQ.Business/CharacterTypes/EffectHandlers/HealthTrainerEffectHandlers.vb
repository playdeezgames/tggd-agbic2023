Friend Module HealthTrainerEffectHandlers

    Friend Sub DoHealthTrainerTalk(character As ICharacter, trigger As IEffect)
        Dim msg = character.World.CreateMessage.
                        AddLine(LightGray, "I am the health trainer!").
                        AddLine(LightGray, "I can help you increase yer health.").
                        AddLine(LightGray, $"The cost is {character.MaximumHealth * 5} AP.").
                        AddChoice(CoolStoryBro, EffectTypes.ExitDialog).
                        AddChoice(TrainMe, EffectTypes.TrainHealth)
    End Sub

    Friend Sub DoTrainHealth(character As ICharacter, trigger As IEffect)
        Dim msg = character.World.CreateMessage
        Const Multiplier = 5
        Dim TrainingCost = Multiplier * character.MaximumHealth
        If character.AdvancementPoints < TrainingCost Then
            msg.AddLine(LightGray, $"To go from {character.MaximumHealth} to {character.MaximumHealth + 1} Maximum Health,")
            msg.AddLine(LightGray, $"you need {TrainingCost} AP, but you have {character.AdvancementPoints}.")
            msg.AddLine(LightGray, "Come back when yer more experienced!")
            Return
        End If
        character.AddAdvancementPoints(-TrainingCost)
        character.SetMaximumHealth(character.MaximumHealth + 1)
        character.SetHealth(character.Health + 1)
        msg.AddLine(Red, $"{CharacterExtensions.Name(character)} loses {TrainingCost} AP")
        msg.AddLine(Green, $"{CharacterExtensions.Name(character)} adds 1 Maximum Health")
        msg.AddLine(LightGray, $"Yer now at {character.MaximumHealth} Maximum Health.")
        msg.AddLine(LightGray, "Remember! If you don't have yer health,")
        msg.AddLine(LightGray, "you don't really have anything!")
    End Sub

End Module
