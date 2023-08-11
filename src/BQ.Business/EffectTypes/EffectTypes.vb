﻿Imports System.Runtime.CompilerServices

Friend Module EffectTypes
    Friend Const AcceptRatQuest = "AcceptRatQuest"
    Friend Const BuildFire = "BuildFire"
    Friend Const BumpRiver = "BumpRiver"
    Friend Const Buy = "Buy"
    Friend Const CompleteRatQuest = "CompleteRatQuest"
    Friend Const CookRatBody = "CookRatBody"
    Friend Const CookRatCorpse = "CookRatCorpse"
    Friend Const CutOffTail = "CutOffTail"
    Friend Const DruidAllergies = "DruidAllergies"
    Friend Const DruidPrices = "DruidPrices"
    Friend Const DruidTalk = "DruidTalk"
    Friend Const DruidTeachMenu = "DruidTeachMenu"
    Friend Const EatCookedRat = "EatCookedRat"
    Friend Const EatPepper = "EatPepper"
    Friend Const EatRatCorpse = "EatRatCorpse"
    Friend Const EatSeasonedRat = "EatSeasonedRat"
    Friend Const EatSmokedPepper = "EatSmokedPepper"
    Friend Const EnergyTrainerTalk = "EnergyTrainerTalk"
    Friend Const EnterCellar = "EnterCellar"
    Friend Const ExitDialog = "ExitDialog"
    Friend Const FillClayPot = "FillClayPot"
    Friend Const GorachanTalk = "GorachanTalk"
    Friend Const Heal = "Heal"
    Friend Const HealerPrices = "NihilistPrices"
    Friend Const HealerTalk = "HealerTalk"
    Friend Const HealthTrainerTalk = "HealthTrainerTalk"
    Friend Const LearnFireMaking = "LearnFireMaking"
    Friend Const LearnForaging = "LearnForaging"
    Friend Const LearnHatchedMaking = "LearnHatchedMaking"
    Friend Const LearnKnapping = "LearnKnapping"
    Friend Const LearnTorchMaking = "LearnTorchMaking"
    Friend Const LearnTwineMaking = "LearnTwineMaking"
    Friend Const MakeDough = "MakeDough"
    Friend Const MakeHatchet = "MakeHatchet"
    Friend Const MakePaprika = "MakePaprika"
    Friend Const MakeTorch = "MakeTorch"
    Friend Const Message = "Message"
    Friend Const MillWheat = "MillWheat"
    Friend Const PayInnkeeper = "PayInnkeeper"
    Friend Const PervertInnkeeper = "PervertInnkeeper"
    Friend Const PotterFlavorText = "PotterFlavorText"
    Friend Const PotterMakePot = "PotterMakePot"
    Friend Const PotterTalk = "PotterTalk"
    Friend Const PutOutFire = "PutOutFire"
    Friend Const SeasonRat = "SeasonRat"
    Friend Const SleepAtInn = "SleepAtInn"
    Friend Const SmokePepper = "SmokePepper"
    Friend Const Teleport = "Teleport"
    Friend Const TrainEnergy = "TrainEnergy"
    Friend Const TrainHealth = "TrainHealth"
    Friend Const StartRatQuest = "StartRatQuest"
    Friend Const Forage = "Forage"
    Friend Const UseEnergyHerb = "UseEnergyHerb"
    Private ReadOnly descriptors As IReadOnlyDictionary(Of String, EffectTypeDescriptor) =
        New Dictionary(Of String, EffectTypeDescriptor) From
        {
            {Buy, New EffectTypeDescriptor("Buy")},
            {CompleteRatQuest, New EffectTypeDescriptor("Complete Rat Quest")},
            {DruidAllergies, New EffectTypeDescriptor("Druid Allergies")},
            {DruidPrices, New EffectTypeDescriptor("Druid Prices")},
            {DruidTalk, New EffectTypeDescriptor("Druid Talk")},
            {DruidTeachMenu, New EffectTypeDescriptor("Druid Teach Menu")},
            {EnergyTrainerTalk, New EffectTypeDescriptor("Energy Trainer Talk")},
            {ExitDialog, New EffectTypeDescriptor("Exit Dialog")},
            {GorachanTalk, New EffectTypeDescriptor("Gorachan Talk")},
            {Heal, New EffectTypeDescriptor("Heal")},
            {HealerPrices, New EffectTypeDescriptor("Healer Prices")},
            {HealerTalk, New EffectTypeDescriptor("Healer Talk")},
            {HealthTrainerTalk, New EffectTypeDescriptor("Health Trainer Talk")},
            {LearnForaging, New EffectTypeDescriptor("Learn Foraging")},
            {LearnKnapping, New EffectTypeDescriptor("Learn Knapping")},
            {LearnTwineMaking, New EffectTypeDescriptor("Learn Twine Making")},
            {MakeDough, New EffectTypeDescriptor("Make Dough")},
            {MakePaprika, New EffectTypeDescriptor("Make Paprika")},
            {MillWheat, New EffectTypeDescriptor("Mill")},
            {Message, New EffectTypeDescriptor("Message")},
            {PayInnkeeper, New EffectTypeDescriptor("Pay Innkeeper")},
            {PervertInnkeeper, New EffectTypeDescriptor("Pervert Innkeeper")},
            {SleepAtInn, New EffectTypeDescriptor("Sleep At Inn")},
            {Teleport, New EffectTypeDescriptor("Teleport")},
            {TrainEnergy, New EffectTypeDescriptor("Train Energy")},
            {TrainHealth, New EffectTypeDescriptor("Train Health")},
            {StartRatQuest, New EffectTypeDescriptor("Start Rat Quest")},
            {EffectTypes.SeasonRat, New EffectTypeDescriptor("Season")},
            {AcceptRatQuest, New EffectTypeDescriptor("Accept Rat Quest")},
            {EnterCellar, New EffectTypeDescriptor("Enter Cellar")},
            {Forage, New EffectTypeDescriptor("Forage")},
            {UseEnergyHerb, New EffectTypeDescriptor("Use")},
            {CutOffTail, New EffectTypeDescriptor("Cut Off Tail")},
            {EatRatCorpse, New EffectTypeDescriptor("Eat")},
            {EatSeasonedRat, New EffectTypeDescriptor("Eat")},
            {EatCookedRat, New EffectTypeDescriptor("Eat")},
            {BuildFire, New EffectTypeDescriptor("Build Fire")},
            {MakeTorch, New EffectTypeDescriptor("Make Torch")},
            {PutOutFire, New EffectTypeDescriptor("Put Out Fire")},
            {CookRatBody, New EffectTypeDescriptor("Cook")},
            {CookRatCorpse, New EffectTypeDescriptor("Cook")},
            {EffectTypes.SmokePepper, New EffectTypeDescriptor("Smoke")},
            {EffectTypes.EatPepper, New EffectTypeDescriptor("Eat")},
            {EffectTypes.EatSmokedPepper, New EffectTypeDescriptor("Eat")}
        }
    <Extension>
    Friend Function ToEffectTypeDescriptor(effectType As String) As EffectTypeDescriptor
        Return descriptors(effectType)
    End Function
    Friend ReadOnly Property All As IEnumerable(Of String)
        Get
            Return descriptors.Keys
        End Get
    End Property
End Module
