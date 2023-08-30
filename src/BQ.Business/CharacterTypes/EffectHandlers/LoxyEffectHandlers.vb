Friend Module LoxyEffectHandlers
    Friend All As IReadOnlyDictionary(Of String, Action(Of ICharacter, IEffect)) =
        New Dictionary(Of String, Action(Of ICharacter, IEffect)) From
                    {
                        {EffectTypes.PutOutFire, AddressOf DoPutOutFlames},
                        {EffectTypes.Message, AddressOf DefaultMessage},
                        {EffectTypes.Heal, AddressOf NihilisticHealing},
                        {EffectTypes.ExitDialog, AddressOf DoExitDialog},
                        {EffectTypes.HealerPrices, AddressOf DoNihilistPrices},
                        {EffectTypes.TrainHealth, AddressOf DoTrainHealth},
                        {"DruidAllergies", AddressOf DoDruidAllergies},
                        {EffectTypes.DruidTeachMenu, AddressOf DoDruidTeachMenu},
                        {EffectTypes.LearnForaging, AddressOf DoLearnSkill},
                        {EffectTypes.LearnKnapping, AddressOf DoLearnSkill},
                        {EffectTypes.LearnTwineMaking, AddressOf DoLearnSkill},
                        {"DruidTalk", AddressOf DoDruidTalk},
                        {EffectTypes.HealthTrainerTalk, AddressOf DoHealthTrainerTalk},
                        {EffectTypes.HealerTalk, AddressOf DoHealerTalk},
                        {EffectTypes.GorachanTalk, AddressOf DoGorachanTalk},
                        {EffectTypes.PervertInnkeeper, AddressOf DoPerventInnkeeper},
                        {EffectTypes.PayInnkeeper, AddressOf DoPayInnkeeper},
                        {"DruidPrices", AddressOf DoDruidPrices},
                        {"Buy", AddressOf DoBuy},
                        {EffectTypes.EnergyTrainerTalk, AddressOf DoEnergyTrainerTalk},
                        {EffectTypes.TrainEnergy, AddressOf DoTrainEnergy},
                        {EffectTypes.StartRatQuest, AddressOf DoStartRatQuest},
                        {"CompleteRatQuest", AddressOf DoCompleteRatQuest},
                        {"CutOffTail", AddressOf DoCutOffTail},
                        {EffectTypes.Forage, AddressOf DoForage},
                        {"BuildFire", AddressOf DoCraftFire},
                        {"BuildFurnace", AddressOf CraftingEffectHandlers.DoBuildFurnace},
                        {EffectTypes.LearnFireMaking, AddressOf DoLearnSkill},
                        {EffectTypes.LearnTorchMaking, AddressOf DoLearnSkill},
                        {EffectTypes.LearnHatchedMaking, AddressOf DoLearnSkill},
                        {EffectTypes.MakeTorch, AddressOf CraftingEffectHandlers.DoMakeTorch},
                        {EffectTypes.MakeHatchet, AddressOf CraftingEffectHandlers.DoMakeHatchet},
                        {"CookRatBody", AddressOf DoCookRatBody},
                        {"CookRatCorpse", AddressOf DoCookRatCorpse},
                        {EffectTypes.PotterMakePot, AddressOf DoPotterMakePot},
                        {"BumpRiver", AddressOf DoBumpRiver},
                        {EffectTypes.FillClayPot, AddressOf DoFillClayPot},
                        {EffectTypes.MillWheat, AddressOf DoMillWheat},
                        {EffectTypes.MakeDough, AddressOf DoMakeDough},
                        {EffectTypes.SmokePepper, AddressOf DoSmokePepper},
                        {EffectTypes.MakePaprika, AddressOf DoMakePaprika},
                        {EffectTypes.SeasonRat, AddressOf DoSeasonRat},
                        {"CookBagel", AddressOf CraftingEffectHandlers.DoCookBagel}
                    }

    Friend Function ConsumeEnergy(character As ICharacter, energyCost As Integer, actionName As String) As Boolean
        If CharacterExtensions.Energy(character) < energyCost Then
            character.World.CreateMessage().AddLine(LightGray, $"{CharacterExtensions.Name(character)} doesn't have the energy to {actionName}.")
            Return False
        End If
        CharacterExtensions.AddEnergy(character, -energyCost)
        Return True
    End Function
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
