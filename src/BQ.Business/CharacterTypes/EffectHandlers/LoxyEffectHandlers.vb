Friend Module LoxyEffectHandlers
    Friend All As IReadOnlyDictionary(Of String, Action(Of ICharacter, IEffect)) =
        New Dictionary(Of String, Action(Of ICharacter, IEffect)) From
                    {
                        {"Heal", AddressOf NihilisticHealing},
                        {"NihilistPrices", AddressOf DoNihilistPrices},
                        {"TrainHealth", AddressOf DoTrainHealth},
                        {"HealthTrainerTalk", AddressOf DoHealthTrainerTalk},
                        {"GorachanTalk", AddressOf DoGorachanTalk},
                        {"PervertInnkeeper", AddressOf DoPerventInnkeeper},
                        {"PayInnkeeper", AddressOf DoPayInnkeeper},
                        {"Buy", AddressOf DoBuy},
                        {"EnergyTrainerTalk", AddressOf DoEnergyTrainerTalk},
                        {"TrainEnergy", AddressOf DoTrainEnergy},
                        {"StartRatQuest", AddressOf DoStartRatQuest},
                        {"CompleteRatQuest", AddressOf DoCompleteRatQuest},
                        {"Forage", AddressOf DoForage},
                        {"PotterMakePot", AddressOf DoPotterMakePot},
                        {"BumpRiver", AddressOf DoBumpRiver},
                        {"FillClayPot", AddressOf DoFillClayPot}
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

    Private Sub DoBuy(character As ICharacter, trigger As IEffect)
        Dim itemType = trigger.GetMetadata("ItemType")
        Dim price = trigger.GetStatistic("Price")
        If CharacterExtensions.Jools(character) < price Then
            character.World.
                CreateMessage().
                AddLine(7, "You don't have enough!").
                AddChoice("Shucks!", trigger.GetMetadata("EffectType"))
            Return
        End If
        CharacterExtensions.AddJools(character, -price)
        character.AddItem(ItemInitializer.CreateItem(character.World, itemType))
        character.World.
            CreateMessage().
            AddLine(7, "Thank you for yer purchase!").
            AddChoice("No worries!", trigger.GetMetadata("EffectType"))
    End Sub
End Module
