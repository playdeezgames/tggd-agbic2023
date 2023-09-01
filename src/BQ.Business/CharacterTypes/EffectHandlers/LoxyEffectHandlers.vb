Friend Module LoxyEffectHandlers
    Friend All As IReadOnlyDictionary(Of String, Action(Of ICharacter, IEffect)) =
        New Dictionary(Of String, Action(Of ICharacter, IEffect)) From
                    {
                        {"Message", AddressOf DefaultMessage},
                        {"Heal", AddressOf NihilisticHealing},
                        {"ExitDialog", AddressOf DoExitDialog},
                        {"NihilistPrices", AddressOf DoNihilistPrices},
                        {"TrainHealth", AddressOf DoTrainHealth},
                        {"DruidTeachMenu", AddressOf DoDruidTeachMenu},
                        {"HealthTrainerTalk", AddressOf DoHealthTrainerTalk},
                        {"HealerTalk", AddressOf DoHealerTalk},
                        {"GorachanTalk", AddressOf DoGorachanTalk},
                        {"PervertInnkeeper", AddressOf DoPerventInnkeeper},
                        {"PayInnkeeper", AddressOf DoPayInnkeeper},
                        {"DruidPrices", AddressOf DoDruidPrices},
                        {"Buy", AddressOf DoBuy},
                        {"EnergyTrainerTalk", AddressOf DoEnergyTrainerTalk},
                        {"TrainEnergy", AddressOf DoTrainEnergy},
                        {"StartRatQuest", AddressOf DoStartRatQuest},
                        {"CompleteRatQuest", AddressOf DoCompleteRatQuest},
                        {"CutOffTail", AddressOf DoCutOffTail},
                        {"Forage", AddressOf DoForage},
                        {"MakeHatchet", AddressOf CraftingEffectHandlers.DoMakeHatchet},
                        {"CookRatBody", AddressOf DoCookRatBody},
                        {"CookRatCorpse", AddressOf DoCookRatCorpse},
                        {"PotterMakePot", AddressOf DoPotterMakePot},
                        {"BumpRiver", AddressOf DoBumpRiver},
                        {"FillClayPot", AddressOf DoFillClayPot},
                        {"MillWheat", AddressOf DoMillWheat},
                        {"MakeDough", AddressOf DoMakeDough},
                        {"SmokePepper", AddressOf DoSmokePepper},
                        {"MakePaprika", AddressOf DoMakePaprika},
                        {"SeasonRat", AddressOf DoSeasonRat}
                    }
    Private Sub DoForage(character As ICharacter, effect As IEffect)
        Dim cell As ICell = CType(effect, ITerrainEffect).Cell
        If Not ConsumeEnergy(character, 1, "forage") Then
            Return
        End If
        Dim itemType = CellExtensions.GenerateForageItemType(cell)
        If String.IsNullOrEmpty(itemType) Then
            character.World.CreateMessage().AddLine(LightGray, $"{CharacterExtensions.Name(character)} finds nothing.")
            Return
        End If
        Dim item = ItemInitializer.CreateItem(character.World, itemType)
        character.AddItem(item)
        character.World.CreateMessage().AddLine(LightGray, $"{CharacterExtensions.Name(character)} finds {ItemExtensions.Name(item)}")
    End Sub

    Private Sub DoBuy(character As ICharacter, trigger As IEffect)
        Dim itemType = trigger.GetMetadata(Metadatas.ItemType)
        Dim price = trigger.GetStatistic(StatisticTypes.Price)
        If CharacterExtensions.Jools(character) < price Then
            character.World.
                CreateMessage().
                AddLine(LightGray, "You don't have enough!").
                AddChoice("Shucks!", trigger.GetMetadata(Metadatas.EffectType))
            Return
        End If
        CharacterExtensions.AddJools(character, -price)
        character.AddItem(ItemInitializer.CreateItem(character.World, itemType))
        character.World.
            CreateMessage().
            AddLine(LightGray, "Thank you for yer purchase!").
            AddChoice("No worries!", trigger.GetMetadata(Metadatas.EffectType))
    End Sub

    Private Sub DefaultMessage(character As ICharacter, trigger As IEffect)
        trigger.GetMetadata(Metadatas.MessageType).ToMessageTypeDescriptor.CreateMessage(character.World)
    End Sub

    Private Sub DoExitDialog(character As ICharacter, trigger As IEffect)
        'NOTHING!
    End Sub
End Module
