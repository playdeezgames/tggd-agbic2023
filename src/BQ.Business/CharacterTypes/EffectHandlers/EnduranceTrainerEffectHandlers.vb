Friend Module EnduranceTrainerEffectHandlers

    Friend Sub DoTrainEnergy(character As ICharacter, trigger As IEffect)
        Dim msg = character.World.CreateMessage
        If CharacterExtensions.AdvancementPoints(character) < 1 Then
            msg.AddLine(7, "You need at least 1 AP.")
            msg.AddLine(7, "Come back when yer more experienced!")
            Return
        End If
        Const Multiplier = 2
        Dim TrainingCost = Multiplier * CharacterExtensions.MaximumEnergy(character)
        If CharacterExtensions.Jools(character) < TrainingCost Then
            msg.
                AddLine(7, $"The price is {TrainingCost} jools.").
                AddLine(7, "I have overhead, you know.")
            Return
        End If
        CharacterExtensions.AddAdvancementPoints(character, -1)
        CharacterExtensions.AddJools(character, -TrainingCost)
        CharacterExtensions.SetMaximumEnergy(character, CharacterExtensions.MaximumEnergy(character) + 1)
        CharacterExtensions.AddEnergy(character, 1)
        msg.AddLine(4, $"{CharacterExtensions.Name(character)} loses 1 AP")
        msg.AddLine(4, $"{CharacterExtensions.Name(character)} loses {TrainingCost} jools")
        msg.AddLine(2, $"{CharacterExtensions.Name(character)} adds 1 Maximum Energy")
        msg.AddLine(7, $"Yer now at {CharacterExtensions.MaximumEnergy(character)} Maximum Energy.")
        msg.AddLine(7, "Persistence is futile!")
    End Sub

    Friend Sub DoEnergyTrainerTalk(character As ICharacter, trigger As IEffect)
        Dim trainCost = CharacterExtensions.MaximumEnergy(character) * 2
        character.World.CreateMessage().
            AddLine(7, "I am the endurance trainer.").
            AddLine(7, "I can increase yer energy").
            AddLine(7, $"for the cost of 1AP and {trainCost} jools.").
            AddChoice("Cool story, bro!", "ExitDialog").
            AddChoice("Train me!", "TrainEnergy")
    End Sub

End Module
