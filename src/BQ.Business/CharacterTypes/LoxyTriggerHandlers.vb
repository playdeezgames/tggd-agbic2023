Imports System.Data

Friend Module LoxyTriggerHandlers

    Friend All As IReadOnlyDictionary(Of String, Action(Of ICharacter, ITrigger)) =
        New Dictionary(Of String, Action(Of ICharacter, ITrigger)) From
                    {
                        {TriggerTypes.Teleport, AddressOf DefaultTeleport},
                        {TriggerTypes.Message, AddressOf DefaultMessage},
                        {TriggerTypes.Heal, AddressOf NihilisticHealing},
                        {TriggerTypes.ExitDialog, AddressOf DoExitDialog},
                        {TriggerTypes.HealerPrices, AddressOf DoNihilistPrices},
                        {TriggerTypes.TrainHealth, AddressOf DoTrainHealth},
                        {TriggerTypes.DruidAllergies, AddressOf DoDruidAllergies},
                        {TriggerTypes.DruidTeachMenu, AddressOf DoDruidTeachMenu},
                        {TriggerTypes.LearnForaging, AddressOf LearnForaging},
                        {TriggerTypes.LearnTwineMaking, AddressOf LearnTwineMaking},
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
                        {EnterCellar, AddressOf DoEnterCellar}
                    }

    Private Sub DoTrainEnergy(character As ICharacter, trigger As ITrigger)
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

    Private Sub DoEnergyTrainerTalk(character As ICharacter, trigger As ITrigger)
        Dim trainCost = character.MaximumEnergy() * 2
        character.World.CreateMessage.
            AddLine(LightGray, "I am the endurance trainer.").
            AddLine(LightGray, "I can increase yer energy").
            AddLine(LightGray, $"for the cost of 1AP and {trainCost} jools.").
            AddChoice("Cool story, bro!", TriggerTypes.ExitDialog).
            AddChoice("Train Me!", TriggerTypes.TrainEnergy)
    End Sub

    Private Sub DoBuy(character As ICharacter, trigger As ITrigger)
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

    Private Sub DoDruidPrices(character As ICharacter, trigger As ITrigger)
        Dim msg = character.World.CreateMessage.
                        AddLine(LightGray, "I sell a variety of herbs.").
                        AddLine(LightGray, $"({character.Name} has {character.Jools} jools)").
                        AddChoice("Good to know!", TriggerTypes.ExitDialog).
                        AddChoice(
                            "Buy Energy Herb(5 jools)",
                            TriggerTypes.Buy,
                            Sub(c) c.
                                SetMetadata(Metadatas.ItemType, ItemTypes.EnergyHerb).
                                SetStatistic(StatisticTypes.Price, 5).
                                SetMetadata(Metadatas.TriggerType, TriggerTypes.DruidPrices))
    End Sub
    Private Sub DoHealerTalk(character As ICharacter, trigger As ITrigger)
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

    Private Sub DoHealthTrainerTalk(character As ICharacter, trigger As ITrigger)
        Dim msg = character.World.CreateMessage.
                        AddLine(LightGray, "I am the health trainer!").
                        AddLine(LightGray, "I can help you increase yer health.").
                        AddLine(LightGray, $"The cost is {character.MaximumHealth * 5} AP.").
                        AddChoice("Cool story, bro!", TriggerTypes.ExitDialog).
                        AddChoice("Train Me!", TriggerTypes.TrainHealth)
    End Sub

    Private Sub DoDruidTalk(character As ICharacter, trigger As ITrigger)
        Dim msg = character.World.CreateMessage.
                        AddLine(LightGray, "Greetings! I am a druid.").
                        AddLine(LightGray, "I can help you learn nature's way.").
                        AddChoice("Cool story, bro!", TriggerTypes.ExitDialog).
                        AddChoice("Don't druids live in the woods?", TriggerTypes.DruidAllergies).
                        AddChoice("Teach me!", TriggerTypes.DruidTeachMenu).
                        AddChoice("What's for sale?", TriggerTypes.DruidPrices)
    End Sub

    Private Sub DefaultMessage(character As ICharacter, trigger As ITrigger)
        trigger.Metadata(Metadatas.MessageType).ToMessageTypeDescriptor.CreateMessage(character.World)
    End Sub

    Friend Sub DefaultTeleport(character As ICharacter, trigger As ITrigger)
        Dim nextCell = character.World.
            Map(trigger.Statistic(StatisticTypes.MapId)).
            GetCell(trigger.Statistic(StatisticTypes.CellColumn), trigger.Statistic(StatisticTypes.CellRow))
        nextCell.AddCharacter(character)
        character.Cell.RemoveCharacter(character)
        character.Cell = nextCell
    End Sub

    Private Sub DoNihilistPrices(character As ICharacter, trigger As ITrigger)
        character.World.CreateMessage().
            AddLine(LightGray, "I don't sell anything.").
            AddLine(LightGray, "I'm a nihilist, remember?")
    End Sub

    Private Sub DoDruidAllergies(character As ICharacter, trigger As ITrigger)
        character.World.CreateMessage().
            AddLine(LightGray, "Alas, I have allergies.")
    End Sub

    Private Sub DoExitDialog(character As ICharacter, trigger As ITrigger)
        'NOTHING!
    End Sub
    Private Sub NihilisticHealing(character As ICharacter, trigger As ITrigger)
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

    Private Sub DoTrainHealth(character As ICharacter, trigger As ITrigger)
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

    Private Sub DoDruidTeachMenu(character As ICharacter, trigger As ITrigger)
        Dim canLearnForaging = Not character.Flag(FlagTypes.KnowsForaging)
        Dim canLearnTwineMaking = Not character.Flag(FlagTypes.KnowsTwineMaking)
        Dim canLearn = canLearnForaging OrElse canLearnTwineMaking
        Dim msg = character.World.CreateMessage()
        If Not canLearn Then
            msg.AddLine(LightGray, "You have learned all I have to teach you.")
            Return
        End If
        msg.AddLine(LightGray, "I can teach you these things:")
        If canLearnForaging Then
            msg.AddChoice(
                "Foraging(-1AP)",
                TriggerTypes.LearnForaging,
                Sub(choice)
                    choice.
                        SetStatistic(StatisticTypes.AdvancementPoints, 1).
                        SetMetadata(Metadatas.FlagType, FlagTypes.KnowsForaging)
                End Sub)
        End If
        If canLearnTwineMaking Then
            msg.AddChoice(
                "Twine Making(-1AP,-2 Plant Fiber)",
                TriggerTypes.LearnTwineMaking,
                Sub(choice)
                    choice.
                        SetStatistic(StatisticTypes.AdvancementPoints, 1).
                        SetMetadata(Metadatas.FlagType, FlagTypes.KnowsTwineMaking)
                End Sub)
        End If
    End Sub

    Private Sub LearnForaging(character As ICharacter, trigger As ITrigger)
        Dim msg = character.World.CreateMessage
        If character.Flag(trigger.Metadata(Metadatas.FlagType)) Then
            msg.AddLine(LightGray, $"{character.Name} already know how to forage!")
            Return
        End If
        Dim learnCost = trigger.Statistic(StatisticTypes.AdvancementPoints)
        If character.AdvancementPoints < learnCost Then
            msg.AddLine(LightGray, $"To learn foraging, {character.Name} needs {learnCost} AP, but has {character.AdvancementPoints}!")
            Return
        End If
        character.AddAdvancementPoints(-learnCost)
        character.Flag(trigger.Metadata(Metadatas.FlagType)) = True
        msg.
            AddLine(LightGray, $"{character.Name} now knows how to forage!").
            AddLine(LightGray, "To forage, simply select 'Forage...'").
            AddLine(LightGray, "from the Actions menu.")
    End Sub

    Private Sub LearnTwineMaking(character As ICharacter, trigger As ITrigger)
        Dim msg = character.World.CreateMessage
        If character.Flag(trigger.Metadata(Metadatas.FlagType)) Then
            msg.AddLine(LightGray, $"{character.Name} already knows how to make twine!")
            Return
        End If
        Dim learnCost = trigger.Statistic(StatisticTypes.AdvancementPoints)
        If character.AdvancementPoints < learnCost Then
            msg.
                AddLine(LightGray, $"To learn to make twine,").
                AddLine(LightGray, $"{character.Name} needs {learnCost} AP,").
                AddLine(LightGray, $"but has {character.AdvancementPoints}!")
            Return
        End If
        If character.ItemTypeCount(ItemTypes.PlantFiber) < 2 Then
            msg.
                AddLine(LightGray, $"To learn to make twine,").
                AddLine(LightGray, $"{character.Name} needs at least 2 plant fiber.")
            Return
        End If
        character.AddAdvancementPoints(-learnCost)
        character.Flag(trigger.Metadata(Metadatas.FlagType)) = True
        character.MakeTwine()
        msg.
            AddLine(LightGray, "You now know how to make twine!").
            AddLine(LightGray, "To do so, simply select 'Make Twine'").
            AddLine(LightGray, "from the Actions menu.")
    End Sub
End Module
