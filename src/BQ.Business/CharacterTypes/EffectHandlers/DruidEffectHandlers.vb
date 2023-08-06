Friend Module DruidEffectHandlers

    Friend Sub DoDruidTalk(character As ICharacter, effect As IEffect)
        Dim msg = character.World.CreateMessage.
                        AddLine(LightGray, "Greetings! I am Marcus, the hippy druid.").
                        AddLine(LightGray, "I can help you learn nature's way.").
                        AddChoice("Cool story, bro!", EffectTypes.ExitDialog).
                        AddChoice("Don't druids live in the woods?", EffectTypes.DruidAllergies).
                        AddChoice("Teach me!", EffectTypes.DruidTeachMenu).
                        AddChoice("What's for sale?", EffectTypes.DruidPrices)
    End Sub

    Friend Sub DoDruidAllergies(character As ICharacter, effect As IEffect)
        character.World.CreateMessage().
            AddLine(LightGray, "Alas, I have allergies.")
    End Sub

    Friend Sub DoDruidPrices(character As ICharacter, effect As IEffect)
        Dim msg = character.World.CreateMessage.
                        AddLine(LightGray, "I sell a variety of herbs.").
                        AddLine(LightGray, $"({character.Name} has {character.Jools} jools)").
                        AddChoice("Good to know!", EffectTypes.ExitDialog).
                        AddChoice(
                            "Buy Energy Herb(5 jools)",
                            EffectTypes.Buy,
                            Sub(c) c.
                                SetMetadata(Metadatas.ItemType, ItemTypes.EnergyHerb).
                                SetStatistic(StatisticTypes.Price, 5).
                                SetMetadata(Metadatas.EffectType, EffectTypes.DruidPrices))
    End Sub

    Private Function AlreadyKnows(character As ICharacter, effect As IEffect, msg As IMessage, text As String) As Boolean
        If character.Flag(effect.Metadata(Metadatas.FlagType)) Then
            msg.AddLine(LightGray, $"{character.Name} already know how to {text}!")
            Return True
        End If
        Return False
    End Function

    Friend Sub DoLearnForaging(character As ICharacter, effect As IEffect)
        Dim msg = character.World.CreateMessage
        If AlreadyKnows(character, effect, msg, "forage") Then Return
        If Not LearnSkill(character, effect, msg, "forage") Then Return
        msg.
            AddLine(LightGray, $"{character.Name} now knows how to forage!").
            AddLine(LightGray, "To forage, simply select 'Forage...'").
            AddLine(LightGray, "from the Actions menu.")
    End Sub

    Friend Sub DoLearnKnapping(character As ICharacter, effect As IEffect)
        Dim msg = character.World.CreateMessage
        If AlreadyKnows(character, effect, msg, "knap") Then Return
        If character.ItemTypeCount(ItemTypes.Rock) < 2 Then
            msg.
                AddLine(LightGray, $"To learn to knap,").
                AddLine(LightGray, $"{character.Name} needs at least 2 rocks.")
            Return
        End If
        If Not LearnSkill(character, effect, msg, "knap") Then Return
        character.Knap()
        msg.
            AddLine(LightGray, $"{character.Name} now knows how to knap rocks!").
            AddLine(LightGray, "To knap rocks, simply select 'Knap'").
            AddLine(LightGray, "from the Actions menu.")
    End Sub

    Private Function LearnSkill(character As ICharacter, effect As IEffect, msg As IMessage, text As String) As Boolean
        Dim learnCost = effect.Statistic(StatisticTypes.AdvancementPoints)
        If character.AdvancementPoints < learnCost Then
            msg.
                AddLine(LightGray, $"To learn to {text},").
                AddLine(LightGray, $"{character.Name} needs {learnCost} AP,").
                AddLine(LightGray, $"but has {character.AdvancementPoints}!")
            Return False
        End If
        character.AddAdvancementPoints(-learnCost)
        character.Flag(effect.Metadata(Metadatas.FlagType)) = True
        Return True
    End Function

    Friend Sub DoLearnTwineMaking(character As ICharacter, effect As IEffect)
        Dim msg = character.World.CreateMessage
        If AlreadyKnows(character, effect, msg, "make twine") Then Return
        If character.ItemTypeCount(ItemTypes.PlantFiber) < 2 Then
            msg.
            AddLine(LightGray, $"To learn to make twine,").
            AddLine(LightGray, $"{character.Name} needs at least 2 plant fiber.")
            Return
        End If

        If Not LearnSkill(character, effect, msg, "make twine") Then Return
        character.MakeTwine()
        msg.
            AddLine(LightGray, "You now know how to make twine!").
            AddLine(LightGray, "To do so, simply select 'Make Twine'").
            AddLine(LightGray, "from the Actions menu.")
    End Sub
    Friend Sub DoLearnFireMaking(character As ICharacter, effect As IEffect)
        Dim msg = character.World.CreateMessage
        If AlreadyKnows(character, effect, msg, "make a fire") Then Return
        If Not RecipeTypes.CanCraft(RecipeTypes.CookingFire, character) Then
            msg.
            AddLine(LightGray, $"To learn to make a fire,").
            AddLine(LightGray, $"{character.Name} needs:")
            For Each input In RecipeTypes.Inputs(RecipeTypes.CookingFire)
                msg.AddLine(LightGray, $"{input.itemType.ToItemTypeDescriptor.Name}: {character.ItemTypeCount(input.itemType)}/{input.count}")
            Next
            Return
        End If
        If Not LearnSkill(character, effect, msg, "make a fire") Then Return
        msg.
            AddLine(LightGray, "You now know how to make a fire!").
            AddLine(LightGray, "To do so, simply select 'Build Fire'").
            AddLine(LightGray, "from the Actions menu.").
            AddLine(LightGray, "(only works in clear areas in the wilderness)")
    End Sub

    Friend Sub DoDruidTeachMenu(character As ICharacter, effect As IEffect)
        Dim canLearnForaging = Not character.Flag(FlagTypes.KnowsForaging)
        Dim canLearnTwineMaking = Not character.Flag(FlagTypes.KnowsTwineMaking)
        Dim canLearnKnapping = Not character.Flag(FlagTypes.KnowsKnapping)
        Dim canLearnFireMaking = Not character.Flag(FlagTypes.KnowsFireMaking)
        Dim canLearnTorchMaking = Not character.Flag(FlagTypes.KnowsTorchMaking)
        Dim canLearn = canLearnForaging OrElse canLearnTwineMaking OrElse canLearnKnapping OrElse canLearnFireMaking OrElse canLearnTorchMaking
        Dim msg = character.World.CreateMessage()
        If Not canLearn Then
            msg.AddLine(LightGray, "You have learned all I have to teach you.")
            Return
        End If
        msg.AddLine(LightGray, "I can teach you these things:")
        msg.AddChoice("Good to know!", EffectTypes.ExitDialog)
        If canLearnForaging Then
            msg.AddChoice(
                "Foraging(-1AP)",
                EffectTypes.LearnForaging,
                Sub(choice)
                    choice.
                        SetStatistic(StatisticTypes.AdvancementPoints, 1).
                        SetMetadata(Metadatas.FlagType, FlagTypes.KnowsForaging)
                End Sub)
        End If
        If canLearnTwineMaking Then
            msg.AddChoice(
                "Twine Making(-1AP,-2 Plant Fiber)",
                EffectTypes.LearnTwineMaking,
                Sub(choice)
                    choice.
                        SetStatistic(StatisticTypes.AdvancementPoints, 1).
                        SetMetadata(Metadatas.FlagType, FlagTypes.KnowsTwineMaking)
                End Sub)
        End If
        If canLearnKnapping Then
            msg.AddChoice(
                "Knapping(-1AP,-2 Rock)",
                EffectTypes.LearnKnapping,
                Sub(choice)
                    choice.
                        SetStatistic(StatisticTypes.AdvancementPoints, 1).
                        SetMetadata(Metadatas.FlagType, FlagTypes.KnowsKnapping)
                End Sub)
        End If
        If canLearnFireMaking Then
            msg.AddChoice(
                "Fire Making(-1AP,-5 Rock, -5 Sticks)",
                EffectTypes.LearnFireMaking,
                Sub(choice)
                    choice.
                        SetStatistic(StatisticTypes.AdvancementPoints, 1).
                        SetMetadata(Metadatas.FlagType, FlagTypes.KnowsFireMaking)
                End Sub)
        End If
        If canLearnTorchMaking Then
            msg.AddChoice(
                "Torch Making(-1AP, -1 Stick, -1 Charcoal)",
                EffectTypes.LearnTorchMaking,
                Sub(choice)
                    choice.
                        SetStatistic(StatisticTypes.AdvancementPoints, 1).
                        SetMetadata(Metadatas.FlagType, FlagTypes.KnowsTorchMaking)
                End Sub)
        End If
    End Sub
    Friend Sub DoLearnTorchMaking(character As ICharacter, effect As IEffect)
        Dim msg = character.World.CreateMessage
        If AlreadyKnows(character, effect, msg, "make a torch") Then Return
        If Not RecipeTypes.CanCraft(RecipeTypes.Torch, character) Then
            msg.
            AddLine(LightGray, $"To learn to make a torch,").
            AddLine(LightGray, $"{character.Name} needs:")
            For Each input In RecipeTypes.Inputs(RecipeTypes.Torch)
                msg.AddLine(LightGray, $"{input.itemType.ToItemTypeDescriptor.Name}: {character.ItemTypeCount(input.itemType)}/{input.count}")
            Next
            Return
        End If
        If Not LearnSkill(character, effect, msg, "make a torch") Then Return
        msg.
            AddLine(LightGray, "You now know how to make a torch!").
            AddLine(LightGray, "To do so, simply select 'Make Torch'").
            AddLine(LightGray, "from the Actions menu.").
            AddLine(LightGray, "(only works with a source of flames)")
    End Sub
End Module
