Friend Module EffectTypes
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
