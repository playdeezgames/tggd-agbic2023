Friend Module InnTriggerHandlers
    Friend Sub DoPayInnkeeper(character As ICharacter, effect As IEffect)
        If character.GetFlag("PaidInnkeeper") Then
            character.World.CreateMessage().
                        AddLine(LightGray, "You've already paid!")
            Return
        End If
        Const bedCost = 1
        If CharacterExtensions.Jools(character) < bedCost Then
            character.World.CreateMessage().
                        AddLine(LightGray, "Sorry! No jools, no bed!")
            Return
        End If
        CharacterExtensions.AddJools(character, -bedCost)
        character.SetFlag("PaidInnkeeper", True)
        character.World.CreateMessage().
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
        CharacterExtensions.AddJools(character, jools)
        character.World.CreateMessage().AddLine(LightGray, $"{CharacterExtensions.Name(character)} receives {jools} jools.")
    End Sub

    Friend Sub DoPerventInnkeeper(character As ICharacter, trigger As IEffect)
        character.World.CreateMessage().
            AddLine(LightGray, "I'm not a pervert!").
            AddLine(LightGray, "I'm just Australian!")
    End Sub

    Friend Sub DoGorachanTalk(character As ICharacter, trigger As IEffect)
        Dim msg = character.World.CreateMessage().
                        AddLine(LightGray, "Welcome to Jusdatip Inn!").
                        AddLine(LightGray, "I'm Gorachan.").
                        AddLine(LightGray, "You can rest in a bed for 1 jools.").
                        AddLine(LightGray, "I'd offer to join you,").
                        AddLine(LightGray, "but then you wouldn't get any rest!").
                        AddChoice("Cool story, bro!", EffectTypes.ExitDialog).
                        AddChoice("Yer a pervert!", EffectTypes.PervertInnkeeper).
                        AddChoice("I'll take a bed.", EffectTypes.PayInnkeeper)
        If character.GetFlag("RatQuest") Then
            If CharacterExtensions.HasItemTypeInInventory(character, ItemTypes.RatTail) Then
                msg.AddChoice("Here's some rat tails!", EffectTypes.CompleteRatQuest)
            End If
        Else
            msg.AddChoice("I need a job!", EffectTypes.StartRatQuest)
        End If
    End Sub

    Friend Sub DoStartRatQuest(character As ICharacter, trigger As IEffect)
        character.World.CreateMessage().
            AddLine(LightGray, "Well, there are a bunch of rats in the cellar.").
            AddLine(LightGray, "I'll pay you 1 jools for each rat tail.").
            AddLine(LightGray, "I only accept the tails, no corpses.").
            AddLine(LightGray, "So you'll need to cut them off first.").
            AddChoice("I'm on it!", EffectTypes.AcceptRatQuest).
            AddChoice("Mebbe later?", EffectTypes.ExitDialog)
    End Sub
End Module
