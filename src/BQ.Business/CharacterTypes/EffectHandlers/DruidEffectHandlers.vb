Friend Module DruidEffectHandlers
    Friend Sub DoDruidPrices(character As ICharacter, effect As IEffect)
        Dim msg = character.World.CreateMessage().
                        AddLine(LightGray, "I sell a variety of herbs.").
                        AddLine(LightGray, $"({CharacterExtensions.Name(character)} has {CharacterExtensions.Jools(character)} jools)").
                        AddChoice("Good to know!", "ExitDialog").
                        AddChoice(
                            "Buy Energy Herb(5 jools)",
                            "Buy",
                            Sub(c)
                                c.SetMetadata(Metadatas.ItemType, ItemTypes.EnergyHerb)
                                c.SetStatistic(StatisticTypes.Price, 5)
                                c.SetMetadata(Metadatas.EffectType, "DruidPrices")
                            End Sub)
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
                EffectTypes.LearnForaging,
                Sub(choice)
                    choice.SetStatistic(StatisticTypes.AdvancementPoints, 1)
                    choice.SetMetadata(Metadatas.FlagType, "KnowsForaging")
                    choice.SetMetadata(Metadatas.TaskName, "forage")
                    choice.SetMetadata(Metadatas.ActionName, "Forage...")
                    choice.SetMetadata(Metadatas.RecipeType, "Foraging")
                End Sub)
        End If
        If canLearnTwineMaking Then
            msg.AddChoice(
                "Twine Making(-1AP,-2 Plant Fiber)",
                EffectTypes.LearnTwineMaking,
                Sub(choice)
                    choice.SetStatistic(StatisticTypes.AdvancementPoints, 1)
                    choice.SetMetadata(Metadatas.FlagType, "KnowsTwineMaking")
                    choice.SetMetadata(Metadatas.TaskName, "make twine")
                    choice.SetMetadata(Metadatas.ActionName, "Make Twine")
                    choice.SetMetadata(Metadatas.RecipeType, "Twine")
                    choice.SetFlag("LearnByDoing", True)
                End Sub)
        End If
        If canLearnKnapping Then
            msg.AddChoice(
                "Knapping(-1AP,-2 Rock)",
                EffectTypes.LearnKnapping,
                Sub(choice)
                    choice.
                        SetStatistic(StatisticTypes.AdvancementPoints, 1)

                    choice.SetMetadata(Metadatas.FlagType, "KnowsRockSharpening")
                    choice.SetMetadata(Metadatas.TaskName, "knap")
                    choice.SetMetadata(Metadatas.ActionName, "Knap")
                    choice.SetMetadata(Metadatas.RecipeType, "SharpRock")
                    choice.SetFlag("LearnByDoing", True)
                End Sub)
        End If
        If canLearnFireMaking Then
            msg.AddChoice(
                "Fire Making(-1AP, -5 Rock, -5 Sticks)",
                EffectTypes.LearnFireMaking,
                Sub(choice)
                    choice.
                        SetStatistic(StatisticTypes.AdvancementPoints, 1)
                    choice.SetMetadata(Metadatas.FlagType, "KnowsFireMaking")
                    choice.SetMetadata(Metadatas.TaskName, "make a fire")
                    choice.SetMetadata(Metadatas.ActionName, "Build Fire")
                    choice.SetMetadata(Metadatas.RecipeType, "CookingFire")
                    choice.SetMetadata(Metadatas.Caveat, "(only works in clear areas in the wilderness)")
                    choice.SetFlag("LearnByDoing", False)
                End Sub)
        End If
        If canLearnTorchMaking Then
            msg.AddChoice(
                "Torch Making(-1AP, -1 Stick, -1 Charcoal)",
                EffectTypes.LearnTorchMaking,
                Sub(choice)
                    choice.
                        SetStatistic(StatisticTypes.AdvancementPoints, 1)
                    choice.SetMetadata(Metadatas.FlagType, "KnowsTorchMaking")
                    choice.SetMetadata(Metadatas.TaskName, "make a torch")
                    choice.SetMetadata(Metadatas.ActionName, "Make Torch")
                    choice.SetMetadata(Metadatas.RecipeType, "Torch")
                    choice.SetMetadata(Metadatas.Caveat, "(only works with a source of flames)")
                    choice.SetFlag("LearnByDoing", False)
                End Sub)
        End If
        If canLearnHatchetMaking Then
            msg.AddChoice(
                "Hatchet Making(-1AP,-1Stick,-1S.Rock,-1Twine)",
                EffectTypes.LearnHatchedMaking,
                Sub(choice)
                    choice.
                        SetStatistic(StatisticTypes.AdvancementPoints, 1)

                    choice.SetMetadata(Metadatas.FlagType, "KnowsHatchetMaking")
                    choice.SetMetadata(Metadatas.TaskName, "make a hatchet")
                    choice.SetMetadata(Metadatas.ActionName, "Make Hatchet")
                    choice.SetMetadata(Metadatas.RecipeType, "Hatchet")
                    choice.SetFlag("LearnByDoing", True)
                End Sub)
        End If
    End Sub
End Module
