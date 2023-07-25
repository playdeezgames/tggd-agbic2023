Imports System.Runtime.CompilerServices
Imports SPLORR.Game

Friend Module CharacterTypes
    Friend Const Loxy = "Loxy"
    Friend Const OliveGlop = "OliveGlop"
    Friend Const CherryGlop = "CherryGlop"
    Private ReadOnly descriptors As IReadOnlyDictionary(Of String, CharacterTypeDescriptor) =
        New Dictionary(Of String, CharacterTypeDescriptor) From
        {
            {
                Loxy,
                New CharacterTypeDescriptor(
                    "Loxy",
                    ChrW(2),
                    Pink,
                    ChrW(1),
                    Black,
                    statistics:=New Dictionary(Of String, Integer) From
                    {
                        {StatisticTypes.Health, 2},
                        {StatisticTypes.MaximumHealth, 2},
                        {StatisticTypes.AttackDice, 1},
                        {StatisticTypes.MaximumAttack, 1},
                        {StatisticTypes.DefendDice, 4},
                        {StatisticTypes.MaximumDefend, 2},
                        {StatisticTypes.XP, 0},
                        {StatisticTypes.XPGoal, 10},
                        {StatisticTypes.XPLevel, 1},
                        {StatisticTypes.AdvancementPointsPerLevel, 10}
                    },
                    triggers:=New Dictionary(Of String, Action(Of ICharacter, ITrigger)) From
                    {
                        {TriggerTypes.Teleport, AddressOf DefaultTeleport},
                        {TriggerTypes.Message, AddressOf DefaultMessage},
                        {TriggerTypes.Heal, AddressOf NihilisticHealing},
                        {TriggerTypes.ExitDialog, AddressOf DoExitDialog},
                        {TriggerTypes.NihilistPrices, AddressOf DoNihilistPrices},
                        {TriggerTypes.TrainHealth, AddressOf DoTrainHealth},
                        {TriggerTypes.DruidAllergies, AddressOf DoDruidAllergies},
                        {TriggerTypes.DruidTeachMenu, AddressOf DoDruidTeachMenu},
                        {TriggerTypes.LearnForaging, AddressOf LoxyTriggerHandlers.LearnForaging},
                        {TriggerTypes.LearnTwineMaking, AddressOf LoxyTriggerHandlers.LearnTwineMaking}
                    })
            },
            {
                OliveGlop,
                New CharacterTypeDescriptor(
                    "Olive Glop",
                    ChrW(&H1B),
                    Hue.LightGreen,
                    ChrW(&H1A),
                    Hue.Black,
                    statistics:=New Dictionary(Of String, Integer) From
                    {
                        {StatisticTypes.Health, 1},
                        {StatisticTypes.MaximumHealth, 1},
                        {StatisticTypes.AttackDice, 2},
                        {StatisticTypes.MaximumAttack, 1},
                        {StatisticTypes.DefendDice, 1},
                        {StatisticTypes.MaximumDefend, 1},
                        {StatisticTypes.Peril, 5},
                        {StatisticTypes.XP, 1}
                    },
                    initializer:=AddressOf OliveGlopInitializer)
            },
            {
                CherryGlop,
                New CharacterTypeDescriptor(
                    "Cherry Glop",
                    ChrW(&H1B),
                    Hue.Red,
                    ChrW(&H1A),
                    Hue.Black,
                    statistics:=New Dictionary(Of String, Integer) From
                    {
                        {StatisticTypes.Health, 1},
                        {StatisticTypes.MaximumHealth, 1},
                        {StatisticTypes.AttackDice, 4},
                        {StatisticTypes.MaximumAttack, 2},
                        {StatisticTypes.DefendDice, 1},
                        {StatisticTypes.MaximumDefend, 1},
                        {StatisticTypes.Peril, 10},
                        {StatisticTypes.XP, 1}
                    },
                    initializer:=AddressOf CherryGlopInitializer)
            }
        }

    Private Sub CherryGlopInitializer(character As ICharacter)

    End Sub

    Private Sub OliveGlopInitializer(character As ICharacter)
        character.SetJools(RNG.RollDice("3d6/6"))
    End Sub


    <Extension>
    Friend Function ToCharacterTypeDescriptor(characterType As String) As CharacterTypeDescriptor
        Return Descriptors1(characterType)
    End Function
    Friend ReadOnly Property All As IEnumerable(Of String)
        Get
            Return Descriptors1.Keys
        End Get
    End Property

    Friend ReadOnly Property Descriptors1 As IReadOnlyDictionary(Of String, CharacterTypeDescriptor)
        Get
            Return descriptors
        End Get
    End Property
End Module
