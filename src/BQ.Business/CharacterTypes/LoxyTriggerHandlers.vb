Friend Module LoxyTriggerHandlers

    Friend Sub DefaultMessage(character As ICharacter, trigger As ITrigger)
        Dim descriptor = trigger.Metadata(Metadatas.MessageType).ToMessageTypeDescriptor
        descriptor.CreateMessage(character.World)
    End Sub

    Friend Sub DefaultTeleport(character As ICharacter, trigger As ITrigger)
        Dim nextCell = character.World.Map(trigger.Statistics(StatisticTypes.MapId)).GetCell(trigger.Statistics(StatisticTypes.CellColumn), trigger.Statistics(StatisticTypes.CellRow))
        nextCell.AddCharacter(character)
        character.Cell.RemoveCharacter(character)
        character.Cell = nextCell
    End Sub

    Friend Sub DoNihilistPrices(character As ICharacter, trigger As ITrigger)
        character.World.CreateMessage().
            AddLine(LightGray, "I don't sell anything.").
            AddLine(LightGray, "I'm a nihilist, remember?")
    End Sub

    Friend Sub DoDruidAllergies(character As ICharacter, trigger As ITrigger)
        character.World.CreateMessage().
            AddLine(LightGray, "Alas, I have allergies.")
    End Sub

    Friend Sub DoExitDialog(character As ICharacter, trigger As ITrigger)
        'NOTHING!
    End Sub
    Friend Sub NihilisticHealing(character As ICharacter, trigger As ITrigger)
        Dim maximumHealth = Math.Min(character.MaximumHealth, trigger.Statistics(StatisticTypes.MaximumHealth))

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

    Friend Sub DoTrainHealth(character As ICharacter, trigger As ITrigger)
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
        msg.AddLine(Red, $"{character.Name} loses {TrainingCost} AP")
        msg.AddLine(Green, $"{character.Name} adds 1 Maximum Health")
        msg.AddLine(LightGray, $"Yer now at {character.MaximumHealth} Maximum Health.")
        msg.AddLine(LightGray, "Remember! If you don't have yer health,")
        msg.AddLine(LightGray, "you don't really have anything!")
    End Sub

    Friend Sub DoDruidTeachMenu(character As ICharacter, trigger As ITrigger)
        Dim canLearnForaging = Not character.Flag(FlagTypes.KnowsForaging)
        Dim canLearnTwineMaking = Not character.Flag(FlagTypes.KnowsTwineMaking)
        Dim canLearn = canLearnForaging OrElse canLearnTwineMaking
        Dim msg = character.World.CreateMessage()
        If Not canLearn Then
            msg.AddLine(LightGray, "You have learned all I have to teach you.")
            Return
        End If
        msg.AddLine(LightGray, "I can teach you these things:")
        If canLearnForaging Then
            msg.AddChoice(
                "Foraging(-1AP)",
                TriggerTypes.LearnForaging,
                Sub(choice)
                    choice.
                        SetStatistic(StatisticTypes.AdvancementPoints, 1).
                        SetMetadata(Metadatas.FlagType, FlagTypes.KnowsForaging)
                End Sub)
        End If
        If canLearnTwineMaking Then
            msg.AddChoice(
                "Twine Making(-1AP,-2 Plant Fiber)",
                TriggerTypes.LearnTwineMaking,
                Sub(choice)
                    choice.
                        SetStatistic(StatisticTypes.AdvancementPoints, 1).
                        SetMetadata(Metadatas.FlagType, FlagTypes.KnowsTwineMaking)
                End Sub)
        End If
    End Sub

    Friend Sub LearnForaging(character As ICharacter, trigger As ITrigger)
        Dim msg = character.World.CreateMessage
        If character.Flag(trigger.Metadata(Metadatas.FlagType)) Then
            msg.AddLine(LightGray, "You already know how to forage!")
            Return
        End If
        Dim learnCost = trigger.Statistics(StatisticTypes.AdvancementPoints)
        If character.AdvancementPoints < learnCost Then
            msg.AddLine(LightGray, $"To learn foraging, you need {learnCost} AP, but have {character.AdvancementPoints}!")
            Return
        End If
        character.AddAdvancementPoints(-learnCost)
        character.Flag(trigger.Metadata(Metadatas.FlagType)) = True
        msg.
            AddLine(LightGray, "You now know how to forage!").
            AddLine(LightGray, "To forage, simply select 'Forage...'").
            AddLine(LightGray, "from the Actions menu.")
    End Sub

    Friend Sub LearnTwineMaking(character As ICharacter, trigger As ITrigger)
        Dim msg = character.World.CreateMessage
        msg.AddLine(LightGray, "TODO: Learn Twine Making")
    End Sub
End Module
