Friend Module DruidEffectHandlers

    Friend Sub DoDruidTalk(character As ICharacter, effect As IEffect)
        Dim msg = character.World.CreateMessage.
                        AddLine(LightGray, "Greetings! I am Marcus, the hippy druid.").
                        AddLine(LightGray, "I can help you learn nature's way.").
                        AddChoice(CoolStoryBro, EffectTypes.ExitDialog).
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
                        AddChoice(GoodToKnow, EffectTypes.ExitDialog).
                        AddChoice(
                            "Buy Energy Herb(5 jools)",
                            EffectTypes.Buy,
                            Sub(c) c.
                                SetMetadata(Metadatas.ItemType, ItemTypes.EnergyHerb).
                                ChangeStatisticTo(StatisticTypes.Price, 5).
                                SetMetadata(Metadatas.EffectType, EffectTypes.DruidPrices))
    End Sub

    Private Function AlreadyKnows(character As ICharacter, effect As IEffect, msg As IMessage, text As String) As Boolean
        If character.Flag(effect.Metadata(Metadatas.FlagType)) Then
            msg.AddLine(LightGray, $"{character.Name} already know how to {text}!")
            Return True
        End If
        Return False
    End Function

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
    Friend Sub DoLearnSkill(character As ICharacter, effect As IEffect)
        Dim msg = character.World.CreateMessage
        Dim taskName = effect.Metadata(Metadatas.TaskName)
        If AlreadyKnows(character, effect, msg, taskName) Then Return
        Dim recipeType = effect.Metadata(Metadatas.RecipeType)
        If Not RecipeTypes.CanCraft(recipeType, character) Then
            msg.
            AddLine(LightGray, $"To learn to {taskName},").
            AddLine(LightGray, $"{character.Name} needs:")
            For Each input In RecipeTypes.Inputs(recipeType)
                msg.AddLine(LightGray, $"{input.itemType.ToItemTypeDescriptor.Name}: {character.ItemTypeCount(input.itemType)}/{input.count}")
            Next
            Return
        End If
        If Not LearnSkill(character, effect, msg, taskName) Then Return
        If effect.Flag(FlagTypes.LearnByDoing) Then
            RecipeTypes.Craft(recipeType, character)
        End If
        msg.
            AddLine(LightGray, $"You now know how to {taskName}!").
            AddLine(LightGray, $"To do so, simply select '{effect.Metadata(Metadatas.ActionName)}'").
            AddLine(LightGray, "from the Actions menu.")
        If effect.HasMetadata(Metadatas.Caveat) Then
            msg.AddLine(LightGray, effect.Metadata(Metadatas.Caveat))
        End If
    End Sub

    Friend Sub DoDruidTeachMenu(character As ICharacter, effect As IEffect)
        Dim canLearnForaging = Not character.Flag(FlagTypes.KnowsForaging)
        Dim canLearnTwineMaking = Not character.Flag(FlagTypes.KnowsTwineMaking)
        Dim canLearnKnapping = Not character.Flag(FlagTypes.KnowsKnapping)
        Dim canLearnFireMaking = Not character.Flag(FlagTypes.KnowsFireMaking)
        Dim canLearnTorchMaking = Not character.Flag(FlagTypes.KnowsTorchMaking)
        Dim canLearnHatchetMaking = Not character.Flag(FlagTypes.KnowsHatchetMaking)
        Dim canLearn = canLearnForaging OrElse canLearnTwineMaking OrElse canLearnKnapping OrElse canLearnFireMaking OrElse canLearnTorchMaking OrElse canLearnHatchetMaking
        Dim msg = character.World.CreateMessage()
        If Not canLearn Then
            msg.AddLine(LightGray, "You have learned all I have to teach you.")
            Return
        End If
        msg.AddLine(LightGray, "I can teach you these things:")
        msg.AddChoice(GoodToKnow, EffectTypes.ExitDialog)
        If canLearnForaging Then
            msg.AddChoice(
                "Foraging(-1AP)",
                EffectTypes.LearnForaging,
                Sub(choice)
                    choice.
                        ChangeStatisticTo(StatisticTypes.AdvancementPoints, 1).
                        SetMetadata(Metadatas.FlagType, FlagTypes.KnowsForaging).
                        SetMetadata(Metadatas.TaskName, "forage").
                        SetMetadata(Metadatas.ActionName, "Forage...").
                        SetMetadata(Metadatas.RecipeType, RecipeTypes.Foraging)
                End Sub)
        End If
        If canLearnTwineMaking Then
            msg.AddChoice(
                "Twine Making(-1AP,-2 Plant Fiber)",
                EffectTypes.LearnTwineMaking,
                Sub(choice)
                    choice.
                        ChangeStatisticTo(StatisticTypes.AdvancementPoints, 1).
                        SetMetadata(Metadatas.FlagType, FlagTypes.KnowsTwineMaking).
                        SetMetadata(Metadatas.TaskName, "make twine").
                        SetMetadata(Metadatas.ActionName, "Make Twine").
                        SetMetadata(Metadatas.RecipeType, RecipeTypes.Twine).
                        ChangeFlagTo(FlagTypes.LearnByDoing, True)
                End Sub)
        End If
        If canLearnKnapping Then
            msg.AddChoice(
                "Knapping(-1AP,-2 Rock)",
                EffectTypes.LearnKnapping,
                Sub(choice)
                    choice.
                        ChangeStatisticTo(StatisticTypes.AdvancementPoints, 1).
                        SetMetadata(Metadatas.FlagType, FlagTypes.KnowsKnapping).
                        SetMetadata(Metadatas.TaskName, "knap").
                        SetMetadata(Metadatas.ActionName, "Knap").
                        SetMetadata(Metadatas.RecipeType, RecipeTypes.SharpRock).
                        ChangeFlagTo(FlagTypes.LearnByDoing, True)
                End Sub)
        End If
        If canLearnFireMaking Then
            msg.AddChoice(
                "Fire Making(-1AP, -5 Rock, -5 Sticks)",
                EffectTypes.LearnFireMaking,
                Sub(choice)
                    choice.
                        ChangeStatisticTo(StatisticTypes.AdvancementPoints, 1).
                        SetMetadata(Metadatas.FlagType, FlagTypes.KnowsFireMaking).
                        SetMetadata(Metadatas.TaskName, "make a fire").
                        SetMetadata(Metadatas.ActionName, "Build Fire").
                        SetMetadata(Metadatas.RecipeType, RecipeTypes.CookingFire).
                        SetMetadata(Metadatas.Caveat, "(only works in clear areas in the wilderness)").
                        ChangeFlagTo(FlagTypes.LearnByDoing, False)
                End Sub)
        End If
        If canLearnTorchMaking Then
            msg.AddChoice(
                "Torch Making(-1AP, -1 Stick, -1 Charcoal)",
                EffectTypes.LearnTorchMaking,
                Sub(choice)
                    choice.
                        ChangeStatisticTo(StatisticTypes.AdvancementPoints, 1).
                        SetMetadata(Metadatas.FlagType, FlagTypes.KnowsTorchMaking).
                        SetMetadata(Metadatas.TaskName, "make a torch").
                        SetMetadata(Metadatas.ActionName, "Make Torch").
                        SetMetadata(Metadatas.RecipeType, RecipeTypes.Torch).
                        SetMetadata(Metadatas.Caveat, "(only works with a source of flames)").
                        ChangeFlagTo(FlagTypes.LearnByDoing, False)
                End Sub)
        End If
        If canLearnHatchetMaking Then
            msg.AddChoice(
                "Hatchet Making(-1AP,-1Stick,-1S.Rock,-1Twine)",
                EffectTypes.LearnHatchedMaking,
                Sub(choice)
                    choice.
                        ChangeStatisticTo(StatisticTypes.AdvancementPoints, 1).
                        SetMetadata(Metadatas.FlagType, FlagTypes.KnowsHatchetMaking).
                        SetMetadata(Metadatas.TaskName, "make a hatchet").
                        SetMetadata(Metadatas.ActionName, "Make Hatchet").
                        SetMetadata(Metadatas.RecipeType, RecipeTypes.Hatchet).
                        ChangeFlagTo(FlagTypes.LearnByDoing, True)
                End Sub)
        End If
    End Sub
End Module
