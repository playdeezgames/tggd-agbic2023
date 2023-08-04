Friend Module EnduranceTrainerEffectHandlers

    Friend Sub DoTrainEnergy(character As ICharacter, trigger As IEffect)
        Dim msg = character.World.CreateMessage
        If character.AdvancementPoints < 1 Then
            msg.AddLine(LightGray, "You need at least 1 AP.")
            msg.AddLine(LightGray, "Come back when yer more experienced!")
            Return
        End If
        Const Multiplier = 2
        Dim TrainingCost = Multiplier * character.MaximumEnergy
        If character.Jools < TrainingCost Then
            msg.
                AddLine(LightGray, $"The price is {TrainingCost} jools.").
                AddLine(LightGray, "I have overhead, you know.")
            Return
        End If
        character.AddAdvancementPoints(-1)
        character.AddJools(-TrainingCost)
        character.SetMaximumEnergy(character.MaximumEnergy + 1)
        character.AddEnergy(1)
        msg.AddLine(Red, $"{character.Name} loses 1 AP")
        msg.AddLine(Red, $"{character.Name} loses {TrainingCost} jools")
        msg.AddLine(Green, $"{character.Name} adds 1 Maximum Energy")
        msg.AddLine(LightGray, $"Yer now at {character.MaximumEnergy} Maximum Energy.")
        msg.AddLine(LightGray, "Persistence is futile!")
    End Sub

    Friend Sub DoEnergyTrainerTalk(character As ICharacter, trigger As IEffect)
        Dim trainCost = character.MaximumEnergy() * 2
        character.World.CreateMessage.
            AddLine(LightGray, "I am the endurance trainer.").
            AddLine(LightGray, "I can increase yer energy").
            AddLine(LightGray, $"for the cost of 1AP and {trainCost} jools.").
            AddChoice("Cool story, bro!", EffectTypes.ExitDialog).
            AddChoice("Train Me!", EffectTypes.TrainEnergy)
    End Sub

End Module
