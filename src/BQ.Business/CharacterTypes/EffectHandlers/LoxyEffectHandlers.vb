Imports SPLORR.Game

Friend Module LoxyEffectHandlers

    Friend All As IReadOnlyDictionary(Of String, Action(Of ICharacter, IEffect)) =
        New Dictionary(Of String, Action(Of ICharacter, IEffect)) From
                    {
                        {EffectTypes.Teleport, AddressOf DefaultTeleport},
                        {EffectTypes.PutOutFire, AddressOf DoPutOutFlames},
                        {EffectTypes.Message, AddressOf DefaultMessage},
                        {EffectTypes.Heal, AddressOf NihilisticHealing},
                        {EffectTypes.ExitDialog, AddressOf DoExitDialog},
                        {EffectTypes.HealerPrices, AddressOf DoNihilistPrices},
                        {EffectTypes.TrainHealth, AddressOf DoTrainHealth},
                        {EffectTypes.DruidAllergies, AddressOf DoDruidAllergies},
                        {EffectTypes.DruidTeachMenu, AddressOf DoDruidTeachMenu},
                        {EffectTypes.LearnForaging, AddressOf DoLearnSkill},
                        {EffectTypes.LearnKnapping, AddressOf DoLearnSkill},
                        {EffectTypes.LearnTwineMaking, AddressOf DoLearnSkill},
                        {EffectTypes.DruidTalk, AddressOf DoDruidTalk},
                        {EffectTypes.HealthTrainerTalk, AddressOf DoHealthTrainerTalk},
                        {EffectTypes.HealerTalk, AddressOf DoHealerTalk},
                        {EffectTypes.GorachanTalk, AddressOf DoGorachanTalk},
                        {EffectTypes.PervertInnkeeper, AddressOf DoPerventInnkeeper},
                        {EffectTypes.PayInnkeeper, AddressOf DoPayInnkeeper},
                        {EffectTypes.SleepAtInn, AddressOf DoSleepAtInn},
                        {EffectTypes.DruidPrices, AddressOf DoDruidPrices},
                        {EffectTypes.Buy, AddressOf DoBuy},
                        {EffectTypes.EnergyTrainerTalk, AddressOf DoEnergyTrainerTalk},
                        {EffectTypes.TrainEnergy, AddressOf DoTrainEnergy},
                        {EffectTypes.StartRatQuest, AddressOf DoStartRatQuest},
                        {EffectTypes.AcceptRatQuest, AddressOf DoAcceptRatQuest},
                        {EffectTypes.EnterCellar, AddressOf DoEnterCellar},
                        {EffectTypes.CompleteRatQuest, AddressOf DoCompleteRatQuest},
                        {EffectTypes.CutOffTail, AddressOf DoCutOffTail},
                        {EffectTypes.UseEnergyHerb, AddressOf DoUseEnergyHerb},
                        {EffectTypes.EatRatCorpse, AddressOf DoEatRatCorpse},
                        {EffectTypes.Forage, AddressOf DoForage},
                        {EffectTypes.BuildFire, AddressOf DoCraftFire},
                        {EffectTypes.LearnFireMaking, AddressOf DoLearnSkill},
                        {EffectTypes.LearnTorchMaking, AddressOf DoLearnSkill},
                        {EffectTypes.LearnHatchedMaking, AddressOf DoLearnSkill},
                        {EffectTypes.MakeTorch, AddressOf CraftingEffectHandlers.DoMakeTorch},
                        {EffectTypes.MakeHatchet, AddressOf CraftingEffectHandlers.DoMakeHatchet},
                        {EffectTypes.CookRatBody, AddressOf DoCookRatBody},
                        {EffectTypes.CookRatCorpse, AddressOf DoCookRatCorpse},
                        {EffectTypes.EatCookedRat, AddressOf DoEatCookedRat},
                        {EffectTypes.PotterTalk, AddressOf DoPotterTalk},
                        {EffectTypes.PotterFlavorText, AddressOf DoPotterFlavorText},
                        {EffectTypes.PotterMakePot, AddressOf DoPotterMakePot},
                        {EffectTypes.BumpRiver, AddressOf DoBumpRiver},
                        {EffectTypes.FillClayPot, AddressOf DoFillClayPot},
                        {EffectTypes.MillWheat, AddressOf DoMillWheat},
                        {EffectTypes.MakeDough, AddressOf DoMakeDough},
                        {EffectTypes.SmokePepper, AddressOf DoSmokePepper},
                        {EffectTypes.MakePaprika, AddressOf DoMakePaprika},
                        {EffectTypes.SeasonRat, AddressOf DoSeasonRat}
                    }


    Friend Function ConsumeEnergy(character As ICharacter, energyCost As Integer, actionName As String) As Boolean
        If character.Energy < energyCost Then
            character.World.CreateMessage().AddLine(LightGray, $"{character.Name} doesn't have the energy to {actionName}.")
            Return False
        End If
        character.AddEnergy(-energyCost)
        Return True
    End Function
    Private Sub DoForage(character As ICharacter, effect As IEffect)
        Dim cell As ICell = CType(effect, ITerrainEffect).Cell
        If Not ConsumeEnergy(character, 1, "forage") Then
            Return
        End If
        Dim itemType = cell.GenerateForageItemType()
        If String.IsNullOrEmpty(itemType) Then
            character.World.CreateMessage().AddLine(LightGray, $"{character.Name} finds nothing.")
            Return
        End If
        Dim item = ItemInitializer.CreateItem(character.World, itemType)
        character.AddItem(item)
        character.World.CreateMessage().AddLine(LightGray, $"{character.Name} finds {item.Name}")
    End Sub

    Private Sub DoBuy(character As ICharacter, trigger As IEffect)
        Dim itemType = trigger.Metadata(Metadatas.ItemType)
        Dim price = trigger.Statistic(StatisticTypes.Price)
        If character.Jools < price Then
            character.World.
                CreateMessage().
                AddLine(LightGray, "You don't have enough!").
                AddChoice("Shucks!", trigger.Metadata(Metadatas.EffectType))
            Return
        End If
        character.AddJools(-price)
        character.AddItem(ItemInitializer.CreateItem(character.World, itemType))
        character.World.
            CreateMessage().
            AddLine(LightGray, "Thank you for yer purchase!").
            AddChoice("No worries!", trigger.Metadata(Metadatas.EffectType))
    End Sub

    Private Sub DefaultMessage(character As ICharacter, trigger As IEffect)
        trigger.Metadata(Metadatas.MessageType).ToMessageTypeDescriptor.CreateMessage(character.World)
    End Sub

    Friend Sub DefaultTeleport(character As ICharacter, trigger As IEffect)
        Dim nextCell = character.World.
            Map(trigger.Statistic(StatisticTypes.MapId)).
            GetCell(trigger.Statistic(StatisticTypes.CellColumn), trigger.Statistic(StatisticTypes.CellRow))
        nextCell.AddCharacter(character)
        character.Cell.RemoveCharacter(character)
        character.Cell = nextCell
    End Sub

    Private Sub DoExitDialog(character As ICharacter, trigger As IEffect)
        'NOTHING!
    End Sub
End Module
