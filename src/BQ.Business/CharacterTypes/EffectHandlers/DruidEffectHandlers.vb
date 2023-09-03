Friend Module DruidEffectHandlers
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
            msg.AddLine(7, "You have learned all I have to teach you.")
            Return
        End If
        msg.AddLine(7, "I can teach you these things:")
        msg.AddChoice("Good to know!", "ExitDialog")
        If canLearnForaging Then
            msg.AddChoice(
                "Foraging(-1AP)",
                "LearnForaging")
            msg.LastChoice.SetStatistic("AdvancementPoints", 1)
            msg.LastChoice.SetMetadata("FlagType", "KnowsForaging")
            msg.LastChoice.SetMetadata("TaskName", "forage")
            msg.LastChoice.SetMetadata("ActionName", "Forage...")
            msg.LastChoice.SetMetadata("RecipeType", "Foraging")
        End If
        If canLearnTwineMaking Then
            msg.AddChoice(
                "Twine Making(-1AP,-2 Plant Fiber)",
                "LearnTwineMaking")
            msg.LastChoice.SetStatistic("AdvancementPoints", 1)
            msg.LastChoice.SetMetadata("FlagType", "KnowsTwineMaking")
            msg.LastChoice.SetMetadata("TaskName", "make twine")
            msg.LastChoice.SetMetadata("ActionName", "Make Twine")
            msg.LastChoice.SetMetadata("RecipeType", "Twine")
            msg.LastChoice.SetFlag("LearnByDoing", True)
        End If
        If canLearnKnapping Then
            msg.AddChoice(
                "Knapping(-1AP,-2 Rock)",
                "LearnKnapping")
            msg.LastChoice.SetStatistic("AdvancementPoints", 1)
            msg.LastChoice.SetMetadata("FlagType", "KnowsRockSharpening")
            msg.LastChoice.SetMetadata("TaskName", "knap")
            msg.LastChoice.SetMetadata("ActionName", "Knap")
            msg.LastChoice.SetMetadata("RecipeType", "SharpRock")
            msg.LastChoice.SetFlag("LearnByDoing", True)
        End If
        If canLearnFireMaking Then
            msg.AddChoice(
                "Fire Making(-1AP, -5 Rock, -5 Sticks)",
                "LearnFireMaking")
            msg.LastChoice.SetStatistic("AdvancementPoints", 1)
            msg.LastChoice.SetMetadata("FlagType", "KnowsFireMaking")
            msg.LastChoice.SetMetadata("TaskName", "make a fire")
            msg.LastChoice.SetMetadata("ActionName", "Build Fire")
            msg.LastChoice.SetMetadata("RecipeType", "CookingFire")
            msg.LastChoice.SetMetadata("Caveat", "(only works in clear areas in the wilderness)")
            msg.LastChoice.SetFlag("LearnByDoing", False)
        End If
        If canLearnTorchMaking Then
            msg.AddChoice(
                "Torch Making(-1AP, -1 Stick, -1 Charcoal)",
                "LearnTorchMaking")
            msg.LastChoice.SetStatistic("AdvancementPoints", 1)
            msg.LastChoice.SetMetadata("FlagType", "KnowsTorchMaking")
            msg.LastChoice.SetMetadata("TaskName", "make a torch")
            msg.LastChoice.SetMetadata("ActionName", "Make Torch")
            msg.LastChoice.SetMetadata("RecipeType", "Torch")
            msg.LastChoice.SetMetadata("Caveat", "(only works with a source of flames)")
            msg.LastChoice.SetFlag("LearnByDoing", False)
        End If
        If canLearnHatchetMaking Then
            msg.AddChoice(
                "Hatchet Making(-1AP,-1Stick,-1S.Rock,-1Twine)",
                "LearnHatchedMaking")
            msg.LastChoice.SetStatistic("AdvancementPoints", 1)
            msg.LastChoice.SetMetadata("FlagType", "KnowsHatchetMaking")
            msg.LastChoice.SetMetadata("TaskName", "make a hatchet")
            msg.LastChoice.SetMetadata("ActionName", "Make Hatchet")
            msg.LastChoice.SetMetadata("RecipeType", "Hatchet")
            msg.LastChoice.SetFlag("LearnByDoing", True)
        End If
    End Sub
End Module
