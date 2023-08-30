Friend Module EffectTypes
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
