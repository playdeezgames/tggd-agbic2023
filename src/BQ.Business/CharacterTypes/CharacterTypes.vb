Imports System.Runtime.CompilerServices
Imports SPLORR.Game

Friend Module CharacterTypes
    Friend Const Schmeara = "Schmeara"
    Friend Const OliveGlop = "OliveGlop"
    Friend Const CherryGlop = "CherryGlop"
    Private ReadOnly descriptors As IReadOnlyDictionary(Of String, CharacterTypeDescriptor) =
        New Dictionary(Of String, CharacterTypeDescriptor) From
        {
            {
                Schmeara,
                New CharacterTypeDescriptor(
                    "Schmeara",
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
                        {StatisticTypes.XPLevel, 1}
                    }, triggers:=New Dictionary(Of String, Action(Of ICharacter, ITrigger)) From
                    {
                        {TriggerTypes.Teleport, AddressOf DefaultTeleport},
                        {TriggerTypes.Message, AddressOf DefaultMessage},
                        {TriggerTypes.Heal, AddressOf DefaultHeal}
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
                    })
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
                    })
            }
        }

    Private Sub DefaultHeal(character As ICharacter, trigger As ITrigger)
        Dim maximumHealth = Math.Min(character.MaximumHealth, trigger.Statistics(StatisticTypes.MaximumHealth))
        If character.Health >= maximumHealth Then
            character.World.CreateMessage().AddLine(LightGray, "Nothing happens!")
            Return
        End If
        character.SetHealth(maximumHealth)
        character.World.
            CreateMessage().
            AddLine(LightGray, $"{character.Name} is healed!").
            AddLine(LightGray, $"{character.Name} now has {character.Health} health.")
    End Sub

    Private Sub DefaultMessage(character As ICharacter, trigger As ITrigger)
        Dim descriptor = trigger.Metadata(Metadatas.MessageType).ToMessageTypeDescriptor
        Dim msg = character.World.CreateMessage().SetSfx(descriptor.Sfx)
        For Each line In descriptor.Lines
            msg.AddLine(line.hue, line.text)
        Next
    End Sub

    Private Sub DefaultTeleport(character As ICharacter, trigger As ITrigger)
        Dim nextCell = character.World.Map(trigger.Statistics(StatisticTypes.MapId)).GetCell(trigger.Statistics(StatisticTypes.CellColumn), trigger.Statistics(StatisticTypes.CellRow))
        nextCell.AddCharacter(character)
        character.Cell.RemoveCharacter(character)
        character.Cell = nextCell
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
