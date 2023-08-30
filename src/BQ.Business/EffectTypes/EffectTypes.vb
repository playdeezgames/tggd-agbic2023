Friend Module EffectTypes
    Friend Const BuildFurnace = "BuildFurnace"
    Friend Const BumpRiver = "BumpRiver"
    Friend Const Buy = "Buy"
    Friend Const CompleteRatQuest = "CompleteRatQuest"
    Friend Const CookBagel = "CookBagel"
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
    Private descriptors As IReadOnlyDictionary(Of String, EffectTypeDescriptor)
    <Extension>
    Friend Function ToEffectTypeDescriptor(effectType As String) As EffectTypeDescriptor
        Return descriptors(effectType)
    End Function

    Friend Sub Load(filename As String)
        descriptors = JsonSerializer.Deserialize(Of Dictionary(Of String, EffectTypeDescriptor))(File.ReadAllText(filename))
    End Sub

    Friend ReadOnly Property All As IEnumerable(Of String)
        Get
            Return descriptors.Keys
        End Get
    End Property
End Module
