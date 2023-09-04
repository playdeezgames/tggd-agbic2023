Friend Module InnTriggerHandlers
    Friend Sub DoPayInnkeeper(character As ICharacter, effect As IEffect)
        If character.GetFlag("PaidInnkeeper") Then
            character.World.CreateMessage().
                        AddLine(7, "You've already paid!")
            Return
        End If
        Const bedCost = 1
        If CharacterExtensions.Jools(character) < bedCost Then
            character.World.CreateMessage().
                        AddLine(7, "Sorry! No jools, no bed!")
            Return
        End If
        CharacterExtensions.AddJools(character, -bedCost)
        character.SetFlag("PaidInnkeeper", True)
        character.World.CreateMessage().
                        AddLine(7, "Thanks for yer business.").
                        AddLine(7, "Choose any bed you like.")
    End Sub

    Friend Sub DoCompleteRatQuest(character As ICharacter, trigger As IEffect)
        Dim jools = 0
        For Each item In character.Items.Where(Function(x) x.ItemType = "RatTail")
            jools += 1
            character.RemoveItem(item)
            item.Recycle()
        Next
        CharacterExtensions.AddJools(character, jools)
        character.World.CreateMessage().AddLine(7, $"{CharacterExtensions.Name(character)} receives {jools} jools.")
    End Sub

    Friend Sub DoPerventInnkeeper(character As ICharacter, trigger As IEffect)
        character.World.CreateMessage().
            AddLine(7, "I'm not a pervert!").
            AddLine(7, "I'm just Australian!")
    End Sub
End Module
