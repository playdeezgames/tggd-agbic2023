Imports System.Runtime.CompilerServices
Imports SPLORR.Game
Imports BQ.Persistence

Friend Module CharacterTypes
    Friend Const Tizzy = "Tizzy"
    Friend Const Goblin = "Goblin"
    Private ReadOnly descriptors As IReadOnlyDictionary(Of String, CharacterTypeDescriptor) =
        New Dictionary(Of String, CharacterTypeDescriptor) From
        {
            {
                Tizzy,
                New CharacterTypeDescriptor(
                    "Tizzy",
                    ChrW(1),
                    Hue.Brown,
                    statistics:=New Dictionary(Of String, Integer) From
                    {
                        {StatisticTypes.Health, 3},
                        {StatisticTypes.MaximumHealth, 3},
                        {StatisticTypes.AttackDice, 1},
                        {StatisticTypes.MaximumAttack, 1},
                        {StatisticTypes.DefendDice, 4},
                        {StatisticTypes.MaximumDefend, 2},
                        {StatisticTypes.MaximumEncumbrance, 10},
                        {StatisticTypes.Dignity, -10},
                        {StatisticTypes.DignityBuff, 0}
                    },
                    flags:=New List(Of String) From {FlagTypes.Avatar},
                    initializer:=AddressOf InitializeTizzy)
            },
            {
                Goblin,
                New CharacterTypeDescriptor(
                    "Goblin",
                    ChrW(5),
                    Hue.Green,
                    statistics:=New Dictionary(Of String, Integer) From
                    {
                        {StatisticTypes.Health, 1},
                        {StatisticTypes.MaximumHealth, 1},
                        {StatisticTypes.AttackDice, 4},
                        {StatisticTypes.MaximumAttack, 2},
                        {StatisticTypes.DefendDice, 1},
                        {StatisticTypes.MaximumDefend, 1},
                        {StatisticTypes.Dignity, 0},
                        {StatisticTypes.DignityBuff, 1}
                    },
                    verbs:=New Dictionary(Of String, Action(Of ICharacter, ICharacter)) From
                    {
                        {VerbTypes.Fight, AddressOf DefaultAttack},
                        {VerbTypes.Disengage, AddressOf DefaultDisengage}
                    },
                    flags:=New List(Of String) From {FlagTypes.Enemy},
                    initializer:=AddressOf InitializeGoblin)
            }
        }

    Private Sub InitializeTizzy(character As ICharacter)
        character.Metadata(Metadatas.Epitaph) = "Better luck next time!"
    End Sub

    Private Sub InitializeGoblin(character As ICharacter)
        character.AddItem(ItemInitializer.CreateItem(character.World, ItemTypes.GoblinCorpse))
        For Each dummy In Enumerable.Range(0, RNG.RollDice("2d2+-2d1"))
            Dim item As IItem
            If RNG.FromGenerator(RNG.MakeBooleanGenerator(4, 1)) Then
                item = ItemInitializer.CreateItem(character.World, ItemTypes.RottenMeat)
            Else
                item = ItemInitializer.CreateItem(character.World, ItemTypes.Meat)
            End If
            character.AddItem(item)
        Next
        If RNG.FromGenerator(RNG.MakeBooleanGenerator(2, 1)) Then
            character.Equip(EquipSlotTypes.Weapon, ItemInitializer.CreateItem(character.World, ItemTypes.Club))
        End If
        If RNG.FromGenerator(RNG.MakeBooleanGenerator(5, 1)) Then
            character.Equip(EquipSlotTypes.Shield, ItemInitializer.CreateItem(character.World, ItemTypes.WoodenShield))
        End If
    End Sub

    Private Sub DefaultAttack(source As ICharacter, target As ICharacter)
        Dim damageOccurred = False
        Do
            While source.World.HasMessages
                source.World.DismissMessage()
            End While
            damageOccurred = source.Attack(target) OrElse damageOccurred
            Dim enemies = source.AdjacentEnemies
            Dim index = 1
            For Each adjacentEnemy In enemies
                If source.IsBagelQuest Then
                    Exit For
                End If
                damageOccurred = adjacentEnemy.Attack(source, (LightGray, $"Counter Attack {index} of {enemies.Count}")) OrElse damageOccurred
                index += 1
            Next
        Loop Until damageOccurred
    End Sub

    Private Sub DefaultDisengage(source As ICharacter, target As ICharacter)
        source.SetTargetCharacter(Nothing)
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
