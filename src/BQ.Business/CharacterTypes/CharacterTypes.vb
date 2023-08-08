Imports System.Runtime.CompilerServices
Imports SPLORR.Game

Friend Module CharacterTypes
    Friend Const Loxy = "Loxy"
    Friend Const OliveGlop = "OliveGlop"
    Friend Const CherryGlop = "CherryGlop"
    Friend Const Rat = "Rat"
    Friend Const Scarecrow = "Scarecrow"
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
                        {StatisticTypes.AttackDice, 2},
                        {StatisticTypes.MaximumAttack, 1},
                        {StatisticTypes.DefendDice, 4},
                        {StatisticTypes.MaximumDefend, 2},
                        {StatisticTypes.XP, 0},
                        {StatisticTypes.XPGoal, 10},
                        {StatisticTypes.XPLevel, 1},
                        {StatisticTypes.AdvancementPointsPerLevel, 10},
                        {StatisticTypes.AdvancementPoints, 0},'TODO: zero me
                        {StatisticTypes.Energy, 10},
                        {StatisticTypes.MaximumEnergy, 10}
                    },
                    effectHandlers:=LoxyEffectHandlers.All)
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
                Rat,
                New CharacterTypeDescriptor(
                    "Rat",
                    ChrW(&H2A),
                    Hue.Brown,
                    ChrW(&H32),
                    Hue.Black,
                    statistics:=New Dictionary(Of String, Integer) From
                    {
                        {StatisticTypes.Health, 1},
                        {StatisticTypes.MaximumHealth, 1},
                        {StatisticTypes.AttackDice, 1},
                        {StatisticTypes.MaximumAttack, 1},
                        {StatisticTypes.DefendDice, 1},
                        {StatisticTypes.MaximumDefend, 1},
                        {StatisticTypes.Peril, 3},
                        {StatisticTypes.XP, 0}
                    },
                    initializer:=AddressOf RatInitializer)
            },
            {
                Scarecrow,
                New CharacterTypeDescriptor(
                    "Scarecrow",
                    ChrW(&H3A),
                    Hue.Yellow,
                    ChrW(&H3C),
                    Hue.Black,
                    statistics:=New Dictionary(Of String, Integer) From
                    {
                        {StatisticTypes.Health, 3},
                        {StatisticTypes.MaximumHealth, 3},
                        {StatisticTypes.AttackDice, 4},
                        {StatisticTypes.MaximumAttack, 2},
                        {StatisticTypes.DefendDice, 3},
                        {StatisticTypes.MaximumDefend, 3},
                        {StatisticTypes.Peril, 7},
                        {StatisticTypes.XP, 3}
                    },
                    initializer:=AddressOf ScarecrowInitializer)
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
                        {StatisticTypes.Health, 2},
                        {StatisticTypes.MaximumHealth, 2},
                        {StatisticTypes.AttackDice, 4},
                        {StatisticTypes.MaximumAttack, 2},
                        {StatisticTypes.DefendDice, 1},
                        {StatisticTypes.MaximumDefend, 1},
                        {StatisticTypes.Peril, 10},
                        {StatisticTypes.XP, 2}
                    },
                    initializer:=AddressOf CherryGlopInitializer)
            }
        }

    Private Sub ScarecrowInitializer(character As ICharacter)
        If RNG.RollDice("1d5/5") > 0 Then
            character.AddItem(ItemInitializer.CreateItem(character.World, ItemTypes.StrawHat))
        End If
    End Sub

    Private Sub RatInitializer(character As ICharacter)
        character.AddItem(ItemInitializer.CreateItem(character.World, ItemTypes.RatCorpse))
    End Sub

    Private Sub CherryGlopInitializer(character As ICharacter)
        character.SetJools(RNG.RollDice("6d6/6"))
    End Sub

    Private Sub OliveGlopInitializer(character As ICharacter)
        character.SetJools(RNG.RollDice("3d6/6"))
    End Sub


    <Extension>
    Friend Function ToCharacterTypeDescriptor(characterType As String) As CharacterTypeDescriptor
        Return descriptors(characterType)
    End Function
    Friend ReadOnly Property All As IEnumerable(Of String)
        Get
            Return descriptors.Keys
        End Get
    End Property
End Module
