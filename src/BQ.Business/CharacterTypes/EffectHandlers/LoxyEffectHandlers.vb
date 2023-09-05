Friend Module LoxyEffectHandlers
    Friend All As IReadOnlyDictionary(Of String, Action(Of ICharacter, IEffect)) =
        New Dictionary(Of String, Action(Of ICharacter, IEffect)) From
                    {
                        {"Buy", AddressOf DoBuy},
                        {"Forage", AddressOf DoForage}
                    }
    Private Sub DoForage(character As ICharacter, effect As IEffect)
        Dim cell As ICell = CType(effect, ITerrainEffect).Cell
        If Not ConsumeEnergy(character, 1, "forage") Then
            Return
        End If
        Dim itemType = CellExtensions.GenerateForageItemType(cell)
        If String.IsNullOrEmpty(itemType) Then
            character.World.CreateMessage().AddLine(7, $"{CharacterExtensions.Name(character)} finds nothing.")
            Return
        End If
        Dim item = ItemInitializer.CreateItem(character.World, itemType)
        character.AddItem(item)
        character.World.CreateMessage().AddLine(7, $"{CharacterExtensions.Name(character)} finds {ItemExtensions.Name(item)}")
    End Sub

    Private Sub DoBuy(character As ICharacter, effect As IEffect)
        Dim itemType = effect.GetMetadata("ItemType")
        Dim price = effect.GetStatistic("Price")
        If CharacterExtensions.Jools(character) < price Then
            character.World.
                CreateMessage().
                AddLine(7, "You don't have enough!").
                AddChoice("Shucks!", effect.GetMetadata("EffectType"))
            Return
        End If
        CharacterExtensions.AddJools(character, -price)
        character.AddItem(ItemInitializer.CreateItem(character.World, itemType))
        character.World.
            CreateMessage().
            AddLine(7, "Thank you for yer purchase!").
            AddChoice("No worries!", effect.GetMetadata("EffectType"))
    End Sub
End Module
