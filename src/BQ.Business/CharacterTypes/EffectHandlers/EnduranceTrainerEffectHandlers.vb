Friend Module EnduranceTrainerEffectHandlers

    Friend Sub DoTrainEnergy(character As ICharacter, trigger As IEffect)
        Dim msg = character.World.CreateMessage
        If CharacterExtensions.AdvancementPoints(character) < 1 Then
            msg.AddLine(LightGray, "You need at least 1 AP.")
            msg.AddLine(LightGray, "Come back when yer more experienced!")
            Return
        End If
        Const Multiplier = 2
        Dim TrainingCost = Multiplier * CharacterExtensions.MaximumEnergy(character)
        If CharacterExtensions.Jools(character) < TrainingCost Then
            msg.
                AddLine(LightGray, $"The price is {TrainingCost} jools.").
                AddLine(LightGray, "I have overhead, you know.")
            Return
        End If
        CharacterExtensions.AddAdvancementPoints(character, -1)
        CharacterExtensions.AddJools(character, -TrainingCost)
        CharacterExtensions.SetMaximumEnergy(character, CharacterExtensions.MaximumEnergy(character) + 1)
        CharacterExtensions.AddEnergy(character, 1)
        msg.AddLine(Red, $"{CharacterExtensions.Name(character)} loses 1 AP")
        msg.AddLine(Red, $"{CharacterExtensions.Name(character)} loses {TrainingCost} jools")
        msg.AddLine(Green, $"{CharacterExtensions.Name(character)} adds 1 Maximum Energy")
        msg.AddLine(LightGray, $"Yer now at {CharacterExtensions.MaximumEnergy(character)} Maximum Energy.")
        msg.AddLine(LightGray, "Persistence is futile!")
    End Sub

    Friend Sub DoEnergyTrainerTalk(character As ICharacter, trigger As IEffect)
        Dim trainCost = CharacterExtensions.MaximumEnergy(character) * 2
        character.World.CreateMessage().
            AddLine(LightGray, "I am the endurance trainer.").
            AddLine(LightGray, "I can increase yer energy").
            AddLine(LightGray, $"for the cost of 1AP and {trainCost} jools.").
            AddChoice(CoolStoryBro, EffectTypes.ExitDialog).
            AddChoice(TrainMe, EffectTypes.TrainEnergy)
    End Sub

End Module
