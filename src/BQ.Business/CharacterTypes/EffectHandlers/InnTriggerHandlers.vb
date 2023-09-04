Friend Module InnTriggerHandlers

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
End Module
