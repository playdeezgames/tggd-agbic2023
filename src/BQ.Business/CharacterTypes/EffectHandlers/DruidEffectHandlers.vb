Friend Module DruidEffectHandlers
    Friend Sub DoDruidPrices(character As ICharacter, effect As IEffect)
        Dim msg = character.World.CreateMessage().
                        AddLine(LightGray, "I sell a variety of herbs.").
                        AddLine(LightGray, $"({CharacterExtensions.Name(character)} has {CharacterExtensions.Jools(character)} jools)").
                        AddChoice("Good to know!", "ExitDialog").
                        AddChoice(
                            "Buy Energy Herb(5 jools)",
                            "Buy")
        msg.LastChoice.SetMetadata(Metadatas.ItemType, ItemTypes.EnergyHerb)
        msg.LastChoice.SetStatistic(StatisticTypes.Price, 5)
        msg.LastChoice.SetMetadata(Metadatas.EffectType, "DruidPrices")
    End Sub

    Friend Sub DoDruidTeachMenu(character As ICharacter, effect As IEffect)
        Dim canLearnForaging = Not character.GetFlag("KnowsForaging")
        Dim canLearnTwineMaking = Not character.GetFlag("KnowsTwineMaking")
        Dim canLearnKnapping = Not character.GetFlag("KnowsRockSharpening")
        Dim canLearnFireMaking = Not character.GetFlag("KnowsFireMaking")
        Dim canLearnTorchMaking = Not character.GetFlag("KnowsTorchMaking")
        Dim canLearnHatchetMaking = Not character.GetFlag("KnowsHatchetMaking")
        Dim canLearn = canLearnForaging OrElse canLearnTwineMaking OrElse canLearnKnapping OrElse canLearnFireMaking OrElse canLearnTorchMaking OrElse canLearnHatchetMaking
        Dim msg = character.World.CreateMessage()
        If Not canLearn Then
            msg.AddLine(LightGray, "You have learned all I have to teach you.")
            Return
        End If
        msg.AddLine(LightGray, "I can teach you these things:")
        msg.AddChoice("Good to know!", "ExitDialog")
        If canLearnForaging Then
            msg.AddChoice(
                "Foraging(-1AP)",
                "LearnForaging")
            msg.LastChoice.SetStatistic(StatisticTypes.AdvancementPoints, 1)
            msg.LastChoice.SetMetadata(Metadatas.FlagType, "KnowsForaging")
            msg.LastChoice.SetMetadata(Metadatas.TaskName, "forage")
            msg.LastChoice.SetMetadata(Metadatas.ActionName, "Forage...")
            msg.LastChoice.SetMetadata(Metadatas.RecipeType, "Foraging")
        End If
        If canLearnTwineMaking Then
            msg.AddChoice(
                "Twine Making(-1AP,-2 Plant Fiber)",
                EffectTypes.LearnTwineMaking)
            msg.LastChoice.SetStatistic(StatisticTypes.AdvancementPoints, 1)
            msg.LastChoice.SetMetadata(Metadatas.FlagType, "KnowsTwineMaking")
            msg.LastChoice.SetMetadata(Metadatas.TaskName, "make twine")
            msg.LastChoice.SetMetadata(Metadatas.ActionName, "Make Twine")
            msg.LastChoice.SetMetadata(Metadatas.RecipeType, "Twine")
            msg.LastChoice.SetFlag("LearnByDoing", True)
        End If
        If canLearnKnapping Then
            msg.AddChoice(
                "Knapping(-1AP,-2 Rock)",
                "LearnKnapping")
            msg.LastChoice.SetStatistic(StatisticTypes.AdvancementPoints, 1)
            msg.LastChoice.SetMetadata(Metadatas.FlagType, "KnowsRockSharpening")
            msg.LastChoice.SetMetadata(Metadatas.TaskName, "knap")
            msg.LastChoice.SetMetadata(Metadatas.ActionName, "Knap")
            msg.LastChoice.SetMetadata(Metadatas.RecipeType, "SharpRock")
            msg.LastChoice.SetFlag("LearnByDoing", True)
        End If
        If canLearnFireMaking Then
            msg.AddChoice(
                "Fire Making(-1AP, -5 Rock, -5 Sticks)",
                "LearnFireMaking")
            msg.LastChoice.SetStatistic(StatisticTypes.AdvancementPoints, 1)
            msg.LastChoice.SetMetadata(Metadatas.FlagType, "KnowsFireMaking")
            msg.LastChoice.SetMetadata(Metadatas.TaskName, "make a fire")
            msg.LastChoice.SetMetadata(Metadatas.ActionName, "Build Fire")
            msg.LastChoice.SetMetadata(Metadatas.RecipeType, "CookingFire")
            msg.LastChoice.SetMetadata(Metadatas.Caveat, "(only works in clear areas in the wilderness)")
            msg.LastChoice.SetFlag("LearnByDoing", False)
        End If
        If canLearnTorchMaking Then
            msg.AddChoice(
                "Torch Making(-1AP, -1 Stick, -1 Charcoal)",
                EffectTypes.LearnTorchMaking)
            msg.LastChoice.SetStatistic(StatisticTypes.AdvancementPoints, 1)
            msg.LastChoice.SetMetadata(Metadatas.FlagType, "KnowsTorchMaking")
            msg.LastChoice.SetMetadata(Metadatas.TaskName, "make a torch")
            msg.LastChoice.SetMetadata(Metadatas.ActionName, "Make Torch")
            msg.LastChoice.SetMetadata(Metadatas.RecipeType, "Torch")
            msg.LastChoice.SetMetadata(Metadatas.Caveat, "(only works with a source of flames)")
            msg.LastChoice.SetFlag("LearnByDoing", False)
        End If
        If canLearnHatchetMaking Then
            msg.AddChoice(
                "Hatchet Making(-1AP,-1Stick,-1S.Rock,-1Twine)",
                "LearnHatchedMaking")
            msg.LastChoice.SetStatistic(StatisticTypes.AdvancementPoints, 1)
            msg.LastChoice.SetMetadata(Metadatas.FlagType, "KnowsHatchetMaking")
            msg.LastChoice.SetMetadata(Metadatas.TaskName, "make a hatchet")
            msg.LastChoice.SetMetadata(Metadatas.ActionName, "Make Hatchet")
            msg.LastChoice.SetMetadata(Metadatas.RecipeType, "Hatchet")
            msg.LastChoice.SetFlag("LearnByDoing", True)
        End If
    End Sub
End Module
