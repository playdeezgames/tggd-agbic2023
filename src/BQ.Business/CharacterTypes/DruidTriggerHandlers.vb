Friend Module DruidTriggerHandlers

    Friend Sub DoDruidTalk(character As ICharacter, trigger As ITrigger)
        Dim msg = character.World.CreateMessage.
                        AddLine(LightGray, "Greetings! I am Marcus, the hippy druid.").
                        AddLine(LightGray, "I can help you learn nature's way.").
                        AddChoice("Cool story, bro!", TriggerTypes.ExitDialog).
                        AddChoice("Don't druids live in the woods?", TriggerTypes.DruidAllergies).
                        AddChoice("Teach me!", TriggerTypes.DruidTeachMenu).
                        AddChoice("What's for sale?", TriggerTypes.DruidPrices)
    End Sub

    Friend Sub DoDruidAllergies(character As ICharacter, trigger As ITrigger)
        character.World.CreateMessage().
            AddLine(LightGray, "Alas, I have allergies.")
    End Sub

    Friend Sub DoDruidPrices(character As ICharacter, trigger As ITrigger)
        Dim msg = character.World.CreateMessage.
                        AddLine(LightGray, "I sell a variety of herbs.").
                        AddLine(LightGray, $"({character.Name} has {character.Jools} jools)").
                        AddChoice("Good to know!", TriggerTypes.ExitDialog).
                        AddChoice(
                            "Buy Energy Herb(5 jools)",
                            TriggerTypes.Buy,
                            Sub(c) c.
                                SetMetadata(Metadatas.ItemType, ItemTypes.EnergyHerb).
                                SetStatistic(StatisticTypes.Price, 5).
                                SetMetadata(Metadatas.TriggerType, TriggerTypes.DruidPrices))
    End Sub

    Private Function AlreadyKnows(character As ICharacter, trigger As ITrigger, msg As IMessage, text As String) As Boolean
        If character.Flag(trigger.Metadata(Metadatas.FlagType)) Then
            msg.AddLine(LightGray, $"{character.Name} already know how to {text}!")
            Return True
        End If
        Return False
    End Function

    Friend Sub DoLearnForaging(character As ICharacter, trigger As ITrigger)
        Dim msg = character.World.CreateMessage
        If AlreadyKnows(character, trigger, msg, "forage") Then Return
        If Not LearnSkill(character, trigger, msg, "forage") Then Return
        msg.
            AddLine(LightGray, $"{character.Name} now knows how to forage!").
            AddLine(LightGray, "To forage, simply select 'Forage...'").
            AddLine(LightGray, "from the Actions menu.")
    End Sub

    Friend Sub DoLearnKnapping(character As ICharacter, trigger As ITrigger)
        Dim msg = character.World.CreateMessage
        If AlreadyKnows(character, trigger, msg, "knap") Then Return
        If character.ItemTypeCount(ItemTypes.Rock) < 2 Then
            msg.
                AddLine(LightGray, $"To learn to knap,").
                AddLine(LightGray, $"{character.Name} needs at least 2 rocks.")
            Return
        End If
        If Not LearnSkill(character, trigger, msg, "knap") Then Return
        character.Knap()
        msg.
            AddLine(LightGray, $"{character.Name} now knows how to knap rocks!").
            AddLine(LightGray, "To knap rocks, simply select 'Knap'").
            AddLine(LightGray, "from the Actions menu.")
    End Sub

    Private Function LearnSkill(character As ICharacter, trigger As ITrigger, msg As IMessage, text As String) As Boolean
        Dim learnCost = trigger.Statistic(StatisticTypes.AdvancementPoints)
        If character.AdvancementPoints < learnCost Then
            msg.
                AddLine(LightGray, $"To learn to {text},").
                AddLine(LightGray, $"{character.Name} needs {learnCost} AP,").
                AddLine(LightGray, $"but has {character.AdvancementPoints}!")
            Return False
        End If
        character.AddAdvancementPoints(-learnCost)
        character.Flag(trigger.Metadata(Metadatas.FlagType)) = True
        Return True
    End Function

    Friend Sub DoLearnTwineMaking(character As ICharacter, trigger As ITrigger)
        Dim msg = character.World.CreateMessage
        If AlreadyKnows(character, trigger, msg, "make twine") Then Return
        If character.ItemTypeCount(ItemTypes.PlantFiber) < 2 Then
            msg.
            AddLine(LightGray, $"To learn to make twine,").
            AddLine(LightGray, $"{character.Name} needs at least 2 plant fiber.")
            Return
        End If

        If Not LearnSkill(character, trigger, msg, "make twine") Then Return
        character.MakeTwine()
        msg.
            AddLine(LightGray, "You now know how to make twine!").
            AddLine(LightGray, "To do so, simply select 'Make Twine'").
            AddLine(LightGray, "from the Actions menu.")
    End Sub

    Friend Sub DoDruidTeachMenu(character As ICharacter, trigger As ITrigger)
        Dim canLearnForaging = Not character.Flag(FlagTypes.KnowsForaging)
        Dim canLearnTwineMaking = Not character.Flag(FlagTypes.KnowsTwineMaking)
        Dim canLearnKnapping = Not character.Flag(FlagTypes.KnowsKnapping)
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
        If canLearnKnapping Then
            msg.AddChoice(
                "Knapping(-1AP,-2 Rock)",
                TriggerTypes.LearnKnapping,
                Sub(choice)
                    choice.
                        SetStatistic(StatisticTypes.AdvancementPoints, 1).
                        SetMetadata(Metadatas.FlagType, FlagTypes.KnowsKnapping)
                End Sub)
        End If
    End Sub

End Module
