Friend Module InnTriggerHandlers

    Friend Sub DoSleepAtInn(character As ICharacter, trigger As IEffect)
        If Not character.GetFlag(FlagTypes.PaidInnkeeper) Then
            character.World.CreateMessage.
                        AddLine(LightGray, $"{character.Name} needs to pay Gorachan first!")
            Return
        End If
        character.ChangeFlagTo(FlagTypes.PaidInnkeeper, False)
        character.AddEnergy(character.MaximumEnergy - character.Energy)
        character.World.CreateMessage.
                        AddLine(LightGray, $"{character.Name} rests and feels refreshed!").
                        AddLine(LightGray, $"{character.Name} has {character.Energy}/{character.MaximumEnergy} energy.")
    End Sub

    Friend Sub DoPayInnkeeper(character As ICharacter, trigger As IEffect)
        If character.GetFlag(FlagTypes.PaidInnkeeper) Then
            character.World.CreateMessage.
                        AddLine(LightGray, "You've already paid!")
            Return
        End If
        Const bedCost = 1
        If character.Jools < bedCost Then
            character.World.CreateMessage.
                        AddLine(LightGray, "Sorry! No jools, no bed!")
            Return
        End If
        character.AddJools(-bedCost)
        character.ChangeFlagTo(FlagTypes.PaidInnkeeper, True)
        character.World.CreateMessage.
                        AddLine(LightGray, "Thanks for yer business.").
                        AddLine(LightGray, "Choose any bed you like.")
    End Sub

    Friend Sub DoCompleteRatQuest(character As ICharacter, trigger As IEffect)
        Dim jools = 0
        For Each item In character.Items.Where(Function(x) x.ItemType = ItemTypes.RatTail)
            jools += 1
            character.RemoveItem(item)
            item.Recycle()
        Next
        character.AddJools(jools)
        character.World.CreateMessage().AddLine(LightGray, $"{character.Name} receives {jools} jools.")
    End Sub

    Friend Sub DoPerventInnkeeper(character As ICharacter, trigger As IEffect)
        Dim msg = character.World.CreateMessage.
                        AddLine(LightGray, "I'm not a pervert!").
                        AddLine(LightGray, "I'm just Australian!")
    End Sub

    Friend Sub DoGorachanTalk(character As ICharacter, trigger As IEffect)
        Dim msg = character.World.CreateMessage.
                        AddLine(LightGray, "Welcome to Jusdatip Inn!").
                        AddLine(LightGray, "I'm Gorachan.").
                        AddLine(LightGray, "You can rest in a bed for 1 jools.").
                        AddLine(LightGray, "I'd offer to join you,").
                        AddLine(LightGray, "but then you wouldn't get any rest!").
                        AddChoice(CoolStoryBro, EffectTypes.ExitDialog).
                        AddChoice("Yer a pervert!", EffectTypes.PervertInnkeeper).
                        AddChoice("I'll take a bed.", EffectTypes.PayInnkeeper)
        If character.GetFlag(FlagTypes.RatQuest) Then
            If character.HasItemTypeInInventory(ItemTypes.RatTail) Then
                msg.AddChoice("Here's some rat tails!", EffectTypes.CompleteRatQuest)
            End If
        Else
            msg.AddChoice("I need a job!", EffectTypes.StartRatQuest)
        End If
    End Sub

    Friend Sub DoStartRatQuest(character As ICharacter, trigger As IEffect)
        Dim msg = character.World.CreateMessage.
                        AddLine(LightGray, "Well, there are a bunch of rats in the cellar.").
                        AddLine(LightGray, "I'll pay you 1 jools for each rat tail.").
                        AddLine(LightGray, "I only accept the tails, no corpses.").
                        AddLine(LightGray, "So you'll need to cut them off first.").
                        AddChoice("I'm on it!", EffectTypes.AcceptRatQuest).
                        AddChoice("Mebbe later?", EffectTypes.ExitDialog)
    End Sub

    Friend Sub DoAcceptRatQuest(character As ICharacter, trigger As IEffect)
        character.ChangeFlagTo(FlagTypes.RatQuest, True)
    End Sub

    Friend Sub DoEnterCellar(character As ICharacter, trigger As IEffect)
        If Not character.GetFlag(FlagTypes.RatQuest) Then
            character.World.CreateMessage().AddLine(LightGray, $"{character.Name} has no business in the cellar.")
            Return
        End If
        DefaultTeleport(character, trigger)
    End Sub
End Module
