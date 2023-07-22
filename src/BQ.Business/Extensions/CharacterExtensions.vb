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
                    Dim enemyType = RNG.FromGenerator(character.Map.MapType.ToMapTypeDescriptor.EncounterGenerator)
                    Dim enemy = CreateCharacter(enemyType, character.Cell)
                    character.SetPeril(character.Peril - enemy.Peril)
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
    <Extension>
    Private Function Attack(character As ICharacter) As Integer
        Return character.TryGetStatistic(StatisticTypes.AttackDice)
    End Function
    <Extension>
    Private Function MaximumAttack(character As ICharacter) As Integer
        Return character.TryGetStatistic(StatisticTypes.MaximumAttack)
    End Function
    <Extension>
    Private Function Defend(character As ICharacter) As Integer
        Return character.TryGetStatistic(StatisticTypes.DefendDice)
    End Function
    <Extension>
    Private Function MaximumDefend(character As ICharacter) As Integer
        Return character.TryGetStatistic(StatisticTypes.MaximumDefend)
    End Function
    <Extension>
    Private Function RollAttack(character As ICharacter) As Integer
        Return Math.Min(character.MaximumAttack, Enumerable.Range(0, character.Attack).Sum(Function(x) RNG.RollDice("1d6/6")))
    End Function
    <Extension>
    Private Function RollDefend(character As ICharacter) As Integer
        Return Math.Min(character.MaximumDefend, Enumerable.Range(0, character.Defend).Sum(Function(x) RNG.RollDice("1d6/6")))
    End Function
    <Extension>
    Private Sub SetHealth(character As ICharacter, health As Integer)
        character.Statistic(StatisticTypes.Health) = Math.Clamp(health, 0, character.MaximumHealth)
    End Sub
    <Extension>
    Friend Function IsDead(character As ICharacter) As Boolean
        Return character.Health <= 0
    End Function
    <Extension>
    Private Sub Die(character As ICharacter)
        If Not character.IsAvatar Then
            character.Cell.RemoveCharacter(character)
            'TODO: drop stuff
            character.Recycle()
        End If
    End Sub
    <Extension>
    Private Function AddXP(character As ICharacter, xp As Integer) As Boolean
        character.Statistic(StatisticTypes.XP) += xp
        If character.XP >= character.XPGoal Then
            character.Statistic(StatisticTypes.XPLevel) += 1
            Dim currentGoal = character.XPGoal
            character.Statistic(StatisticTypes.XPGoal) *= 2
            character.AddXP(-currentGoal)
            Return True
        End If
        Return False
    End Function
    <Extension>
    Private Sub AwardXP(character As ICharacter, xp As Integer)
        If Not character.IsAvatar Then
            Return
        End If
        Dim msg = character.World.CreateMessage().AddLine(LightGray, $"{character.Name} gains {xp} XP!")
        If character.AddXP(xp) Then
            msg.AddLine(LightGreen, $"{character.Name} is now level {character.XPLevel}!")
        End If
    End Sub
    <Extension>
    Friend Function Attack(attacker As ICharacter, defender As ICharacter, Optional message As String = Nothing) As Boolean
        If defender.IsDead Then
            Return False
        End If
        Dim msg = attacker.World.CreateMessage
        If Not String.IsNullOrEmpty(message) Then
            msg.AddLine(LightGray, message)
        End If
        msg.AddLine(LightGray, $"{attacker.Name} attacks {defender.Name}")
        Dim attackRoll = attacker.RollAttack()
        msg.AddLine(LightGray, $"{attacker.Name} rolls an attack of {attackRoll}")
        Dim defendRoll = defender.RollDefend()
        msg.AddLine(LightGray, $"{defender.Name} rolls a defend of {defendRoll}")
        Dim damage = Math.Max(0, attackRoll - defendRoll)
        If damage <= 0 Then
            msg.AddLine(LightGray, $"{attacker.Name} misses.")
            msg.SetSfx(If(attacker.IsAvatar, Sfx.PlayerMiss, Sfx.EnemyMiss))
            Return False
        End If
        msg.AddLine(LightGray, $"{defender.Name} takes {damage} damage")
        defender.SetHealth(defender.Health - damage)
        If defender.IsDead Then
            msg.SetSfx(If(defender.IsAvatar, Sfx.PlayerDeath, Sfx.EnemyDeath))
            msg.AddLine(LightGray, $"{attacker.Name} kills {defender.Name}")
            attacker.AwardXP(defender.XP)
            defender.Die()
            Return True
        End If
        msg.SetSfx(If(defender.IsAvatar, Sfx.PlayerHit, Sfx.EnemyHit))
        msg.AddLine(LightGray, $"{defender.Name} has {defender.Health}/{defender.MaximumHealth} health.")
        Return True
    End Function
    <Extension>
    Function XP(character As ICharacter) As Integer
        Return character.TryGetStatistic(StatisticTypes.XP)
    End Function
    <Extension>
    Function XPGoal(character As ICharacter) As Integer
        Return character.TryGetStatistic(StatisticTypes.XPGoal)
    End Function
    <Extension>
    Function XPLevel(character As ICharacter) As Integer
        Return character.TryGetStatistic(StatisticTypes.XPLevel)
    End Function
End Module
