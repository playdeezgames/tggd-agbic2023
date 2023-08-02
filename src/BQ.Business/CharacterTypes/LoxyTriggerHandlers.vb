Imports System.Data

Friend Module LoxyTriggerHandlers

    Friend All As IReadOnlyDictionary(Of String, Action(Of ICharacter, IEffect)) =
        New Dictionary(Of String, Action(Of ICharacter, IEffect)) From
                    {
                        {TriggerTypes.Teleport, AddressOf DefaultTeleport},
                        {TriggerTypes.Message, AddressOf DefaultMessage},
                        {TriggerTypes.Heal, AddressOf NihilisticHealing},
                        {TriggerTypes.ExitDialog, AddressOf DoExitDialog},
                        {TriggerTypes.HealerPrices, AddressOf DoNihilistPrices},
                        {TriggerTypes.TrainHealth, AddressOf DoTrainHealth},
                        {TriggerTypes.DruidAllergies, AddressOf DoDruidAllergies},
                        {TriggerTypes.DruidTeachMenu, AddressOf DoDruidTeachMenu},
                        {TriggerTypes.LearnForaging, AddressOf DoLearnForaging},
                        {TriggerTypes.LearnKnapping, AddressOf DoLearnKnapping},
                        {TriggerTypes.LearnTwineMaking, AddressOf DoLearnTwineMaking},
                        {TriggerTypes.DruidTalk, AddressOf DoDruidTalk},
                        {TriggerTypes.HealthTrainerTalk, AddressOf DoHealthTrainerTalk},
                        {TriggerTypes.HealerTalk, AddressOf DoHealerTalk},
                        {TriggerTypes.GorachanTalk, AddressOf DoGorachanTalk},
                        {TriggerTypes.PervertInnkeeper, AddressOf DoPerventInnkeeper},
                        {TriggerTypes.PayInnkeeper, AddressOf DoPayInnkeeper},
                        {TriggerTypes.SleepAtInn, AddressOf DoSleepAtInn},
                        {TriggerTypes.DruidPrices, AddressOf DoDruidPrices},
                        {TriggerTypes.Buy, AddressOf DoBuy},
                        {TriggerTypes.EnergyTrainerTalk, AddressOf DoEnergyTrainerTalk},
                        {TriggerTypes.TrainEnergy, AddressOf DoTrainEnergy},
                        {StartRatQuest, AddressOf DoStartRatQuest},
                        {AcceptRatQuest, AddressOf DoAcceptRatQuest},
                        {EnterCellar, AddressOf DoEnterCellar},
                        {CompleteRatQuest, AddressOf DoCompleteRatQuest}
                    }

    Private Sub DoTrainEnergy(character As ICharacter, trigger As IEffect)
        Dim msg = character.World.CreateMessage
        If character.AdvancementPoints < 1 Then
            msg.AddLine(LightGray, "You need at least 1 AP.")
            msg.AddLine(LightGray, "Come back when yer more experienced!")
            Return
        End If
        Const Multiplier = 2
        Dim TrainingCost = Multiplier * character.MaximumEnergy
        If character.Jools < TrainingCost Then
            msg.
                AddLine(LightGray, $"The price is {TrainingCost} jools.").
                AddLine(LightGray, "I have overhead, you know.")
            Return
        End If
        character.AddAdvancementPoints(-1)
        character.AddJools(-TrainingCost)
        character.SetMaximumEnergy(character.MaximumEnergy + 1)
        character.AddEnergy(1)
        msg.AddLine(Red, $"{character.Name} loses 1 AP")
        msg.AddLine(Red, $"{character.Name} loses {TrainingCost} jools")
        msg.AddLine(Green, $"{character.Name} adds 1 Maximum Energy")
        msg.AddLine(LightGray, $"Yer now at {character.MaximumEnergy} Maximum Energy.")
        msg.AddLine(LightGray, "Persistence is futile!")
    End Sub

    Private Sub DoEnergyTrainerTalk(character As ICharacter, trigger As IEffect)
        Dim trainCost = character.MaximumEnergy() * 2
        character.World.CreateMessage.
            AddLine(LightGray, "I am the endurance trainer.").
            AddLine(LightGray, "I can increase yer energy").
            AddLine(LightGray, $"for the cost of 1AP and {trainCost} jools.").
            AddChoice("Cool story, bro!", TriggerTypes.ExitDialog).
            AddChoice("Train Me!", TriggerTypes.TrainEnergy)
    End Sub

    Private Sub DoBuy(character As ICharacter, trigger As IEffect)
        Dim itemType = trigger.Metadata(Metadatas.ItemType)
        Dim price = trigger.Statistic(StatisticTypes.Price)
        If character.Jools < price Then
            character.World.
                CreateMessage().
                AddLine(LightGray, "You don't have enough!").
                AddChoice("Shucks!", trigger.Metadata(Metadatas.TriggerType))
            Return
        End If
        character.AddJools(-price)
        character.AddItem(ItemInitializer.CreateItem(character.World, itemType))
        character.World.
            CreateMessage().
            AddLine(LightGray, "Thank you for yer purchase!").
            AddChoice("No worries!", trigger.Metadata(Metadatas.TriggerType))
    End Sub
    Private Sub DoHealerTalk(character As ICharacter, trigger As IEffect)
        Dim msg = character.World.CreateMessage.
                        AddLine(LightGray, "Welcome to the Nihilistic House of Healing.").
                        AddLine(LightGray, "If you go to the basin And wash,").
                        AddLine(LightGray, "you will be healed,").
                        AddLine(LightGray, "but it will cost you half of yer jools.").
                        AddLine(LightGray, "Not that I care or anything,").
                        AddLine(LightGray, "because I'm a nihilist.").
                        AddChoice("Cool story, bro!", TriggerTypes.ExitDialog).
                        AddChoice("What's for sale?", TriggerTypes.HealerPrices)
    End Sub

    Private Sub DoHealthTrainerTalk(character As ICharacter, trigger As IEffect)
        Dim msg = character.World.CreateMessage.
                        AddLine(LightGray, "I am the health trainer!").
                        AddLine(LightGray, "I can help you increase yer health.").
                        AddLine(LightGray, $"The cost is {character.MaximumHealth * 5} AP.").
                        AddChoice("Cool story, bro!", TriggerTypes.ExitDialog).
                        AddChoice("Train Me!", TriggerTypes.TrainHealth)
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

    Private Sub DoNihilistPrices(character As ICharacter, trigger As IEffect)
        character.World.CreateMessage().
            AddLine(LightGray, "I don't sell anything.").
            AddLine(LightGray, "I'm a nihilist, remember?")
    End Sub

    Private Sub DoExitDialog(character As ICharacter, trigger As IEffect)
        'NOTHING!
    End Sub
    Private Sub NihilisticHealing(character As ICharacter, trigger As IEffect)
        Dim maximumHealth = Math.Min(character.MaximumHealth, trigger.Statistic(StatisticTypes.MaximumHealth))
        If character.Health >= maximumHealth Then
            character.World.CreateMessage().AddLine(LightGray, "Nothing happens!")
            Return
        End If
        character.SetHealth(maximumHealth)
        Dim msg =
            character.World.
                CreateMessage().
                AddLine(LightGray, $"{character.Name} is healed!").
                AddLine(LightGray, $"{character.Name} now has {character.Health} health.")
        Dim jools = character.Jools \ 2
        character.AddJools(-jools)
        If jools > 0 Then
            msg.AddLine(Red, $"{character.Name} loses {jools} jools!")
        End If
    End Sub

    Private Sub DoTrainHealth(character As ICharacter, trigger As IEffect)
        Dim msg = character.World.CreateMessage
        Const Multiplier = 5
        Dim TrainingCost = Multiplier * character.MaximumHealth
        If character.AdvancementPoints < TrainingCost Then
            msg.AddLine(LightGray, $"To go from {character.MaximumHealth} to {character.MaximumHealth + 1} Maximum Health,")
            msg.AddLine(LightGray, $"you need {TrainingCost} AP, but you have {character.AdvancementPoints}.")
            msg.AddLine(LightGray, "Come back when yer more experienced!")
            Return
        End If
        character.AddAdvancementPoints(-TrainingCost)
        character.SetMaximumHealth(character.MaximumHealth + 1)
        character.SetHealth(character.Health + 1)
        msg.AddLine(Red, $"{character.Name} loses {TrainingCost} AP")
        msg.AddLine(Green, $"{character.Name} adds 1 Maximum Health")
        msg.AddLine(LightGray, $"Yer now at {character.MaximumHealth} Maximum Health.")
        msg.AddLine(LightGray, "Remember! If you don't have yer health,")
        msg.AddLine(LightGray, "you don't really have anything!")
    End Sub
End Module
