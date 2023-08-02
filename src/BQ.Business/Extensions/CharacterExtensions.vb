﻿Imports System.Runtime.CompilerServices
Imports SPLORR.Game

Friend Module CharacterExtensions
    <Extension>
    Friend Function HasItemTypeInInventory(character As ICharacter, itemType As String) As Boolean
        Return character.Items.Any(Function(x) x.ItemType = itemType)
    End Function
    <Extension>
    Friend Sub Sleep(character As ICharacter)
        If Not character.IsAvatar Then
            Return
        End If
        If Not character.Map.CampingAllowed Then
            character.World.CreateMessage().AddLine(LightGray, $"{character.Name} cannot sleep here!")
            Return
        End If
        character.AddEnergy(character.MaximumEnergy \ 2)
        Dim msg = character.World.CreateMessage().
            AddLine(LightGray, $"{character.Name} sleeps.").
            AddLine(LightGray, $"{character.Name} now has {character.Energy}/{character.MaximumEnergy} energy.")
        character.SetPeril(character.MaximumEnergy \ 2)
        character.Move((0, 0))
        If character.Cell.HasOtherCharacters(character) Then
            msg.AddLine(Red, $"{character.Name} awakens to a surprise attack!")
        End If
    End Sub
    <Extension>
    Friend Sub DoMakeTwine(character As ICharacter)
        Dim plantFibers = character.Items.Where(Function(x) x.ItemType = ItemTypes.PlantFiber)
        If plantFibers.Count < 2 Then
            character.World.CreateMessage.AddLine(LightGray, $"{character.Name} needs 2 plant fiber, but has {plantFibers.Count}!")
            Return
        End If
        character.MakeTwine()
        character.World.CreateMessage.AddLine(LightGray, $"{character.Name} makes twine.")
    End Sub
    <Extension>
    Friend Sub MakeTwine(character As ICharacter)
        Dim inputs = character.Items.Where(Function(x) x.ItemType = ItemTypes.PlantFiber).Take(2)
        For Each input In inputs
            character.RemoveItem(input)
            input.Recycle()
        Next
        character.AddItem(ItemInitializer.CreateItem(character.World, ItemTypes.Twine))
    End Sub
    <Extension>
    Friend Sub Knap(character As ICharacter)
        Dim inputs = character.Items.Where(Function(x) x.ItemType = ItemTypes.Rock).Take(2)
        For Each input In inputs
            character.RemoveItem(input)
            input.Recycle()
        Next
        character.AddItem(ItemInitializer.CreateItem(character.World, ItemTypes.SharpRock))
        character.AddItem(ItemInitializer.CreateItem(character.World, ItemTypes.Rock))
    End Sub
    <Extension>
    Friend Function Name(character As ICharacter) As String
        Return character.CharacterType.ToCharacterTypeDescriptor.Name
    End Function
    <Extension>
    Friend Sub DoVerb(
                     target As ICharacter,
                     verbType As String,
                     item As IItem)
        item.Descriptor.VerbTypes(verbType).Invoke(target, item)
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
            Dim trigger = cell.Trigger
            character.CharacterType.ToCharacterTypeDescriptor.EffectHandlers(trigger.EffectType).Invoke(character, trigger)
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

        If delta.x <> 0 OrElse delta.y <> 0 Then
            nextCell.AddCharacter(character)
            character.Cell.RemoveCharacter(character)
            character.Cell = nextCell
        End If

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
    Friend Function TryGetStatistic(character As ICharacter, statisticType As String, Optional defaultValue As Integer = 0) As Integer
        Return If(character.HasStatistic(statisticType), character.Statistic(statisticType), defaultValue)
    End Function
    <Extension>
    Private Function Peril(character As ICharacter) As Integer
        Return character.TryGetStatistic(StatisticTypes.Peril)
    End Function
    <Extension>
    Friend Function Energy(character As ICharacter) As Integer
        Return character.TryGetStatistic(StatisticTypes.Energy)
    End Function
    <Extension>
    Friend Sub AddEnergy(character As ICharacter, delta As Integer)
        character.Statistic(StatisticTypes.Energy) = Math.Clamp(character.Energy + delta, 0, character.MaximumEnergy)
    End Sub
    <Extension>
    Friend Function MaximumEnergy(character As ICharacter) As Integer
        Return character.TryGetStatistic(StatisticTypes.MaximumEnergy)
    End Function
    <Extension>
    Friend Sub SetMaximumEnergy(character As ICharacter, maximumEnergy As Integer)
        character.Statistic(StatisticTypes.MaximumEnergy) = maximumEnergy
    End Sub
    <Extension>
    Friend Function Health(character As ICharacter) As Integer
        Return character.TryGetStatistic(StatisticTypes.Health)
    End Function
    <Extension>
    Friend Function MaximumHealth(character As ICharacter) As Integer
        Return character.TryGetStatistic(StatisticTypes.MaximumHealth)
    End Function
    <Extension>
    Friend Function AttackDice(character As ICharacter) As Integer
        Return character.TryGetStatistic(StatisticTypes.AttackDice) + character.EquippedItems.Sum(Function(x) x.AttackDice)
    End Function
    <Extension>
    Friend Function MaximumAttack(character As ICharacter) As Integer
        Return character.TryGetStatistic(StatisticTypes.MaximumAttack) + character.EquippedItems.Sum(Function(x) x.MaximumAttack)
    End Function
    <Extension>
    Friend Function DefendDice(character As ICharacter) As Integer
        Return character.TryGetStatistic(StatisticTypes.DefendDice)
    End Function
    <Extension>
    Friend Function MaximumDefend(character As ICharacter) As Integer
        Return character.TryGetStatistic(StatisticTypes.MaximumDefend)
    End Function
    <Extension>
    Private Function RollAttack(character As ICharacter) As Integer
        Return Math.Min(character.MaximumAttack, Enumerable.Range(0, character.AttackDice).Sum(Function(x) RNG.RollDice("1d6/6")))
    End Function
    <Extension>
    Private Function RollDefend(character As ICharacter) As Integer
        Return Math.Min(character.MaximumDefend, Enumerable.Range(0, character.DefendDice).Sum(Function(x) RNG.RollDice("1d6/6")))
    End Function
    <Extension>
    Friend Sub SetHealth(character As ICharacter, health As Integer)
        character.Statistic(StatisticTypes.Health) = Math.Clamp(health, 0, character.MaximumHealth)
    End Sub
    <Extension>
    Friend Sub SetMaximumHealth(character As ICharacter, maximumHealth As Integer)
        character.Statistic(StatisticTypes.MaximumHealth) = Math.Max(1, maximumHealth)
        character.SetHealth(character.Health)
    End Sub
    <Extension>
    Friend Function IsDead(character As ICharacter) As Boolean
        Return character.Health <= 0
    End Function
    <Extension>
    Private Sub Die(character As ICharacter)
        If Not character.IsAvatar Then
            Dim cell = character.Cell
            cell.RemoveCharacter(character)
            For Each item In character.EquippedItems
                character.UnequipItem(item)
                cell.AddItem(item)
            Next
            For Each item In character.Items
                character.RemoveItem(item)
                cell.AddItem(item)
            Next
            character.Recycle()
        End If
    End Sub
    <Extension>
    Private Function AdvancementPointsPerLevel(character As ICharacter) As Integer
        Return character.TryGetStatistic(StatisticTypes.AdvancementPointsPerLevel)
    End Function
    <Extension>
    Friend Sub AddAdvancementPoints(character As ICharacter, advancementPoints As Integer)
        character.Statistic(StatisticTypes.AdvancementPoints) = Math.Max(0, character.TryGetStatistic(StatisticTypes.AdvancementPoints) + advancementPoints)
    End Sub
    <Extension>
    Private Function AddXP(character As ICharacter, xp As Integer) As Boolean
        character.Statistic(StatisticTypes.XP) += xp
        If character.XP >= character.XPGoal Then
            character.AddAdvancementPoints(character.AdvancementPointsPerLevel)
            character.Statistic(StatisticTypes.XPLevel) += 1
            Dim currentGoal = character.XPGoal
            character.Statistic(StatisticTypes.XPGoal) *= 2
            character.AddXP(-currentGoal)
            Return True
        End If
        Return False
    End Function
    <Extension>
    Friend Sub AddJools(character As ICharacter, jools As Integer)
        character.SetJools(character.Jools + jools)
    End Sub
    <Extension>
    Friend Function Jools(character As ICharacter) As Integer
        Return character.TryGetStatistic(StatisticTypes.Jools)
    End Function
    <Extension>
    Friend Sub SetJools(character As ICharacter, jools As Integer)
        character.Statistic(StatisticTypes.Jools) = Math.Max(0, jools)
    End Sub
    <Extension>
    Private Sub AwardJools(toCharacter As ICharacter, msg As IMessage, jools As Integer)
        If Not toCharacter.IsAvatar Then
            Return
        End If
        If jools > 0 Then
            msg.AddLine(LightGray, $"{toCharacter.Name} gets {jools} jools!")
            toCharacter.AddJools(jools)
        End If
    End Sub
    <Extension>
    Friend Function AdvancementPoints(character As ICharacter) As Integer
        Return character.TryGetStatistic(StatisticTypes.AdvancementPoints)
    End Function
    <Extension>
    Private Sub AwardXP(character As ICharacter, msg As IMessage, xp As Integer)
        If Not character.IsAvatar Then
            Return
        End If
        msg.AddLine(LightGray, $"{character.Name} gains {xp} XP!")
        If character.AddXP(xp) Then
            msg.AddLine(LightGreen, $"{character.Name} is now level {character.XPLevel}!")
            msg.AddLine(LightGray, $"{character.Name} now has {character.AdvancementPoints} AP!")
        Else
            msg.AddLine(LightGray, $"{character.Name} needs {character.XPGoal - character.XP} for the next level.")
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
            attacker.AwardJools(msg, defender.Jools)
            attacker.AwardXP(msg, defender.XP)
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
    <Extension>
    Sub EquipItem(character As ICharacter, item As IItem)
        Dim equipSlotType = item.ItemType.ToItemTypeDescriptor.EquipSlotType
        character.Equip(equipSlotType, item)
    End Sub
    <Extension>
    Function HasCuttingTool(character As ICharacter) As Boolean
        Return character.Items.Any(Function(x) x.ItemType.ToItemTypeDescriptor.IsCuttingTool) OrElse character.EquippedItems.Any(Function(x) x.ItemType.ToItemTypeDescriptor.IsCuttingTool)
    End Function
End Module
