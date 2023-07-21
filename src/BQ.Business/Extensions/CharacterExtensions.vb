Imports System.Runtime.CompilerServices
Imports SPLORR.Game

Friend Module CharacterExtensions
    <Extension>
    Friend Function Name(character As ICharacter) As String
        Return character.CharacterType.ToCharacterTypeDescriptor.Name
    End Function
    <Extension>
    Friend Function Verbs(character As ICharacter) As IEnumerable(Of String)
        Return character.CharacterType.ToCharacterTypeDescriptor.AvailableVerbs
    End Function
    <Extension>
    Friend Sub DoVerb(target As ICharacter, verbType As String, source As ICharacter)
        target.CharacterType.ToCharacterTypeDescriptor.Verbs(verbType).Invoke(source, target)
    End Sub
    <Extension>
    Private Function Weapons(character As ICharacter) As IEnumerable(Of IItem)
        Return character.EquippedItems.Where(Function(x) x.IsWeapon)
    End Function
    <Extension>
    Private Function Armors(character As ICharacter) As IEnumerable(Of IItem)
        Return character.EquippedItems.Where(Function(x) x.IsArmor)
    End Function
    <Extension>
    Friend Sub TakeItem(character As ICharacter, item As IItem)
        character.Cell.RemoveItem(item)
        character.AddItem(item)
    End Sub
    <Extension>
    Friend Sub DropItem(character As ICharacter, item As IItem)
        character.Cell.AddItem(item)
        character.RemoveItem(item)
    End Sub
    <Extension>
    Private Function CanEnter(character As ICharacter, cell As ICell) As Boolean
        Return cell IsNot Nothing AndAlso cell.IsTenable
    End Function
    <Extension>
    Private Sub DoTrigger(character As ICharacter, cell As ICell)
        If cell IsNot Nothing AndAlso cell.HasTrigger Then
            cell.DoTrigger(character)
        End If
    End Sub
    <Extension>
    Friend Function Move(character As ICharacter, delta As (x As Integer, y As Integer)) As Boolean
        Dim cell = character.Cell
        Dim nextCell = cell.Map.GetCell(cell.Column + delta.x, cell.Row + delta.y)
        If Not character.CanEnter(nextCell) Then
            character.DoTrigger(nextCell)
            Return False
        End If

        nextCell.AddCharacter(character)
        character.Cell.RemoveCharacter(character)
        character.Cell = nextCell

        character.DoTrigger(nextCell)
        character.EnterCell()
        Return True
    End Function
    <Extension>
    Private Sub EnterCell(character As ICharacter)
        If character.Cell.Peril > 0 Then
            character.SetPeril(character.Peril + character.Cell.Peril)
            If character.Peril > 0 Then
                Dim roll = RNG.RollDice("1d20")
                If roll <= character.Peril Then
                    character.SetPeril(character.Peril - roll)
                    Dim enemyType = RNG.FromGenerator(character.Map.MapType.ToMapTypeDescriptor.EncounterGenerator)
                    Dim enemy = CreateCharacter(enemyType, character.Cell)
                    character.Cell.AddCharacter(enemy)
                End If
            End If
        End If
    End Sub
    <Extension>
    Private Sub SetPeril(character As ICharacter, peril As Integer)
        character.Statistic(StatisticTypes.Peril) = Math.Max(0, peril)
    End Sub
    <Extension>
    Private Function TryGetStatistic(character As ICharacter, statisticType As String, Optional defaultValue As Integer = 0) As Integer
        Return If(character.HasStatistic(statisticType), character.Statistic(statisticType), defaultValue)
    End Function
    <Extension>
    Private Function Peril(character As ICharacter) As Integer
        Return character.TryGetStatistic(StatisticTypes.Peril)
    End Function
    <Extension>
    Friend Function Health(character As ICharacter) As Integer
        Return character.TryGetStatistic(StatisticTypes.Health)
    End Function
    <Extension>
    Friend Function MaximumHealth(character As ICharacter) As Integer
        Return character.TryGetStatistic(StatisticTypes.MaximumHealth)
    End Function
End Module
