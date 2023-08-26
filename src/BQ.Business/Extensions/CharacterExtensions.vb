﻿Public Module CharacterExtensions
    <Extension>
    Friend Function HasWon(character As ICharacter) As Boolean
        Return character.IsAvatar AndAlso character.ItemTypeCount(ItemTypes.Bagel) > 0
    End Function
    <Extension>
    Friend Function CanCookBagel(character As ICharacter) As Boolean
        Return RecipeTypes.CanCraft(RecipeTypes.Bagel, character)
    End Function
    <Extension>
    Friend Function HealthDisplay(character As ICharacter) As String
        Return $"HP {character.Health}/{character.MaximumHealth}"
    End Function
    <Extension>
    Friend Function EnergyDisplay(character As ICharacter) As String
        Return $"EN {character.Energy}/{character.MaximumEnergy}"
    End Function
    <Extension>
    Friend Function XPLevelDisplay(character As ICharacter) As String
        Return $"LV {character.XPLevel}"
    End Function
    <Extension>
    Friend Function XPDisplay(character As ICharacter) As String
        Return $"XP {character.XP}/{character.XPGoal}"
    End Function
    <Extension>
    Friend Function JoolsDisplay(character As ICharacter) As String
        Return $"$ {character.Jools}"
    End Function
    <Extension>
    Friend Function APDisplay(character As ICharacter) As String
        Return $"AP {character.AdvancementPoints}"
    End Function
    <Extension>
    Friend Function ATKDisplay(character As ICharacter) As String
        Return $"ATK: Max {character.MaximumAttack} avg {character.AttackDice / 6:f2}"
    End Function
    <Extension>
    Friend Function DEFDisplay(character As ICharacter) As String
        Return $"DEF: Max {character.MaximumDefend} avg {character.DefendDice / 6:f2}"
    End Function
    <Extension>
    Friend Function CanBuildFurnace(character As ICharacter) As Boolean
        Return RecipeTypes.CanCraft(RecipeTypes.Furnace, character)
    End Function
    <Extension>
    Friend Function HasItemTypeInInventory(character As ICharacter, itemType As String) As Boolean
        Return character.Items.Any(Function(x) x.ItemType = itemType)
    End Function
    <Extension>
    Friend Sub Sleep(character As ICharacter)
        If Not character.IsAvatar Then
            Return
        End If
        If Not character.Map.CampingAllowed OrElse Not CellExtensions.CanSleep(character.Cell) Then
            character.World.CreateMessage().AddLine(LightGray, $"{character.Name} cannot sleep here!")
            Return
        End If
        character.AddEnergy(character.MaximumEnergy \ 2)
        Dim msg = character.World.CreateMessage().
            AddLine(LightGray, $"{character.Name} sleeps.").
            AddLine(LightGray, $"{character.Name} now has {character.Energy}/{character.MaximumEnergy} energy.")
        character.AddPeril(character.MaximumEnergy \ 2)
        character.Move((0, 0))
        If character.Cell.HasOtherCharacters(character) Then
            msg.AddLine(Red, $"{character.Name} awakens to a surprise attack!")
        End If
    End Sub
    <Extension>
    Friend Sub DoBuildFire(character As ICharacter)
        character.Cell.Descriptor.DoEffect(character, EffectTypes.BuildFire, character.Cell)
    End Sub
    <Extension>
    Friend Sub DoBuildFurnace(character As ICharacter)
        character.Cell.Descriptor.DoEffect(character, EffectTypes.BuildFurnace, character.Cell)
    End Sub
    <Extension>
    Friend Sub DoCookBagel(character As ICharacter)
        character.Cell.Descriptor.DoEffect(character, EffectTypes.CookBagel, character.Cell)
    End Sub
    <Extension>
    Friend Sub DoMakeTorch(character As ICharacter)
        character.Cell.Descriptor.DoEffect(character, EffectTypes.MakeTorch, character.Cell)
    End Sub
    <Extension>
    Friend Sub DoMakeHatchet(character As ICharacter)
        DoMakeItem(character, RecipeTypes.Hatchet, "a hatchet", AddressOf MakeHatchet)
    End Sub

    Private Sub DoMakeItem(character As ICharacter, recipeType As String, noun As String, makeAction As Action(Of ICharacter))
        If Not RecipeTypes.CanCraft(recipeType, character) Then
            Dim msg = character.World.CreateMessage.AddLine(LightGray, $"To make {noun},").AddLine(LightGray, $"{character.Name} needs:")
            CraftingEffectHandlers.AddRecipeInputs(character, msg, recipeType)
            Return
        End If
        makeAction.Invoke(character)
        character.World.CreateMessage.AddLine(LightGray, $"{character.Name} makes {noun}.")
    End Sub

    <Extension>
    Friend Sub DoPutOutFire(character As ICharacter)
        character.Cell.Descriptor.DoEffect(character, EffectTypes.PutOutFire, character.Cell)
    End Sub
    <Extension>
    Friend Sub DoKnap(character As ICharacter)
        DoMakeItem(character, RecipeTypes.SharpRock, "a sharp rock", AddressOf Knap)
    End Sub
    <Extension>
    Friend Sub DoMakeTwine(character As ICharacter)
        DoMakeItem(character, RecipeTypes.Twine, "twine", AddressOf MakeTwine)
    End Sub
    <Extension>
    Friend Sub MakeTwine(character As ICharacter)
        RecipeTypes.Craft(RecipeTypes.Twine, character)
    End Sub
    <Extension>
    Friend Sub MakeHatchet(character As ICharacter)
        RecipeTypes.Craft(RecipeTypes.Hatchet, character)
    End Sub
    <Extension>
    Friend Sub Knap(character As ICharacter)
        RecipeTypes.Craft(RecipeTypes.SharpRock, character)
    End Sub
    <Extension>
    Public Function Name(character As ICharacter) As String
        Return character.Descriptor.Name
    End Function
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
        Return cell IsNot Nothing AndAlso CellExtensions.IsTenable(cell)
    End Function
    <Extension>
    Friend Sub DoMapEffect(character As ICharacter, cell As ICell)
        If cell IsNot Nothing AndAlso cell.HasEffect Then
            Dim effect = cell.Effect
            character.Descriptor.RunEffectScript(WorldModel.LuaState, effect.EffectType, character, effect)
        End If
    End Sub
    <Extension>
    Friend Function Descriptor(character As ICharacter) As CharacterTypeDescriptor
        Return character.CharacterType.ToCharacterTypeDescriptor
    End Function
    <Extension>
    Friend Sub DoItemEffect(character As ICharacter, effectType As String, item As IItem)
        Dim effect = item.ItemType.ToItemTypeDescriptor.ToItemEffect(effectType, item)
        character.Descriptor.RunEffectScript(WorldModel.LuaState, effect.EffectType, character, effect)
    End Sub
    <Extension>
    Friend Function Move(character As ICharacter, delta As (x As Integer, y As Integer)) As Boolean
        Dim cell = character.Cell
        Dim nextCell = cell.Map.GetCell(cell.Column + delta.x, cell.Row + delta.y)
        If Not character.CanEnter(nextCell) Then
            character.DoMapEffect(nextCell)
            Return False
        End If

        If delta.x <> 0 OrElse delta.y <> 0 Then
            nextCell.AddCharacter(character)
            character.Cell.RemoveCharacter(character)
            character.Cell = nextCell
        End If

        character.DoMapEffect(nextCell)
        character.EnterCell()
        Return True
    End Function
    <Extension>
    Private Sub AddPeril(character As ICharacter, delta As Integer)
        character.SetPeril(character.Peril + delta)
    End Sub
    <Extension>
    Private Sub EnterCell(character As ICharacter)
        If CellExtensions.Peril(character.Cell) > 0 Then
            character.AddPeril(CellExtensions.Peril(character.Cell))
            If character.Peril > 0 Then
                Dim roll = RNG.RollDice("1d20")
                If roll <= character.Peril Then
                    Dim enemyType = character.Cell.Descriptor.GenerateCreatureType()
                    Dim enemy = CreateCharacter(enemyType, character.Cell)
                    character.SetPeril(character.Peril - enemy.Peril)
                    character.Cell.AddCharacter(enemy)
                End If
            End If
        End If
    End Sub
    <Extension>
    Private Sub SetPeril(character As ICharacter, peril As Integer)
        character.SetStatistic(StatisticTypes.Peril, Math.Max(0, peril))
    End Sub
    <Extension>
    Private Function Peril(character As ICharacter) As Integer
        Return character.GetStatistic(StatisticTypes.Peril)
    End Function
    <Extension>
    Public Function Energy(character As ICharacter) As Integer
        Return character.GetStatistic(StatisticTypes.Energy)
    End Function
    <Extension>
    Public Sub AddEnergy(character As ICharacter, delta As Integer)
        character.SetStatistic(StatisticTypes.Energy, Math.Clamp(character.Energy + delta, 0, character.MaximumEnergy))
    End Sub
    <Extension>
    Public Function MaximumEnergy(character As ICharacter) As Integer
        Return character.GetStatistic(StatisticTypes.MaximumEnergy)
    End Function
    <Extension>
    Friend Sub SetMaximumEnergy(character As ICharacter, maximumEnergy As Integer)
        character.SetStatistic(StatisticTypes.MaximumEnergy, maximumEnergy)
    End Sub
    <Extension>
    Friend Function Health(character As ICharacter) As Integer
        Return character.GetStatistic(StatisticTypes.Health)
    End Function
    <Extension>
    Friend Function MaximumHealth(character As ICharacter) As Integer
        Return character.GetStatistic(StatisticTypes.MaximumHealth)
    End Function
    <Extension>
    Friend Function AttackDice(character As ICharacter) As Integer
        Return character.GetStatistic(StatisticTypes.AttackDice) + character.EquippedItems.Sum(Function(x) x.AttackDice)
    End Function
    <Extension>
    Friend Function MaximumAttack(character As ICharacter) As Integer
        Return character.GetStatistic(StatisticTypes.MaximumAttack) + character.EquippedItems.Sum(Function(x) x.MaximumAttack)
    End Function
    <Extension>
    Friend Function DefendDice(character As ICharacter) As Integer
        Return character.GetStatistic(StatisticTypes.DefendDice) + character.EquippedItems.Sum(Function(x) x.DefendDice)
    End Function
    <Extension>
    Friend Function MaximumDefend(character As ICharacter) As Integer
        Return character.GetStatistic(StatisticTypes.MaximumDefend) + character.EquippedItems.Sum(Function(x) x.MaximumDefend)
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
        character.SetStatistic(StatisticTypes.Health, Math.Clamp(health, 0, character.MaximumHealth))
    End Sub
    <Extension>
    Friend Sub SetMaximumHealth(character As ICharacter, maximumHealth As Integer)
        character.SetStatistic(StatisticTypes.MaximumHealth, Math.Max(1, maximumHealth))
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
        Return character.GetStatistic(StatisticTypes.AdvancementPointsPerLevel)
    End Function
    <Extension>
    Friend Sub AddAdvancementPoints(character As ICharacter, advancementPoints As Integer)
        character.SetStatistic(StatisticTypes.AdvancementPoints, Math.Max(0, character.GetStatistic(StatisticTypes.AdvancementPoints) + advancementPoints))
    End Sub
    <Extension>
    Private Function AddXP(character As ICharacter, xp As Integer) As Boolean
        character.AddStatistic(StatisticTypes.XP, xp)
        If character.XP >= character.XPGoal Then
            character.AddAdvancementPoints(character.AdvancementPointsPerLevel)
            character.AddStatistic(StatisticTypes.XPLevel, 1)
            Dim currentGoal = character.XPGoal
            character.AddStatistic(StatisticTypes.XPGoal, character.GetStatistic(StatisticTypes.XPGoal))
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
        Return character.GetStatistic(StatisticTypes.Jools)
    End Function
    <Extension>
    Friend Sub SetJools(character As ICharacter, jools As Integer)
        character.SetStatistic(StatisticTypes.Jools, Math.Max(0, jools))
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
        Return character.GetStatistic(StatisticTypes.AdvancementPoints)
    End Function
    <Extension>
    Friend Sub AwardXP(character As ICharacter, msg As IMessage, xp As Integer)
        If Not character.IsAvatar OrElse xp = 0 Then
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
    Private Function ScuffWeapons(character As ICharacter, scuffAmount As Integer, msg As IMessage) As Boolean
        Dim items = character.EquippedItems.Where(Function(x) x.IsWeapon AndAlso x.Durability > 0)
        Dim result = False
        While scuffAmount > 0 AndAlso items.Any
            Dim item = RNG.FromEnumerable(items)
            item.AddDurability(-1)
            If item.IsBroken Then
                msg.AddLine(Red, $"{character.Name}' {item.Name} breaks!")
                character.UnequipItem(item)
                character.RemoveItem(item)
                item.Recycle()
                result = True
            End If
            scuffAmount -= 1
            items = character.EquippedItems.Where(Function(x) x.IsWeapon AndAlso x.Durability > 0)
        End While
        Return result
    End Function
    <Extension>
    Private Function ScuffArmors(character As ICharacter, scuffAmount As Integer, msg As IMessage) As Boolean
        Dim items = character.EquippedItems.Where(Function(x) x.IsArmor AndAlso x.Durability > 0)
        Dim result = False
        While scuffAmount > 0 AndAlso items.Any
            Dim item = RNG.FromEnumerable(items)
            item.AddDurability(-1)
            If item.IsBroken Then
                msg.AddLine(Red, $"{character.Name}' {item.Name} breaks!")
                character.UnequipItem(item)
                character.RemoveItem(item)
                item.Recycle()
                result = True
            End If
            scuffAmount -= 1
            items = character.EquippedItems.Where(Function(x) x.IsArmor AndAlso x.Durability > 0)
        End While
        Return result
    End Function
    <Extension>
    Friend Function Attack(attacker As ICharacter, defender As ICharacter, Optional message As String = Nothing) As Boolean
        If defender.IsDead Then
            Return False
        End If
        Dim result = False
        Dim msg = attacker.World.CreateMessage
        If Not String.IsNullOrEmpty(message) Then
            msg.AddLine(LightGray, message)
        End If
        msg.AddLine(LightGray, $"{attacker.Name} attacks {defender.Name}")
        Dim attackRoll = attacker.RollAttack()
        msg.AddLine(LightGray, $"{attacker.Name} rolls an attack of {attackRoll}")
        result = attacker.ScuffWeapons(attackRoll, msg) OrElse result
        Dim defendRoll = defender.RollDefend()
        msg.AddLine(LightGray, $"{defender.Name} rolls a defend of {defendRoll}")
        Dim damage = Math.Max(0, attackRoll - defendRoll)
        result = defender.ScuffArmors(Math.Max(defendRoll, damage), msg) OrElse result
        If damage <= 0 Then
            msg.AddLine(LightGray, $"{attacker.Name} misses.")
            msg.SetSfx(If(attacker.IsAvatar, Sfx.PlayerMiss, Sfx.EnemyMiss))
            Return result
        End If
        result = True
        msg.AddLine(LightGray, $"{defender.Name} takes {damage} damage")
        defender.SetHealth(defender.Health - damage)
        If defender.IsDead Then
            msg.SetSfx(If(defender.IsAvatar, Sfx.PlayerDeath, Sfx.EnemyDeath))
            msg.AddLine(LightGray, $"{attacker.Name} kills {defender.Name}")
            attacker.AwardJools(msg, defender.Jools)
            attacker.AwardXP(msg, defender.XP)
            defender.Die()
            Return result
        End If
        msg.SetSfx(If(defender.IsAvatar, Sfx.PlayerHit, Sfx.EnemyHit))
        msg.AddLine(LightGray, $"{defender.Name} has {defender.Health}/{defender.MaximumHealth} health.")
        Return result
    End Function
    <Extension>
    Function XP(character As ICharacter) As Integer
        Return character.GetStatistic(StatisticTypes.XP)
    End Function
    <Extension>
    Function XPGoal(character As ICharacter) As Integer
        Return character.GetStatistic(StatisticTypes.XPGoal)
    End Function
    <Extension>
    Function XPLevel(character As ICharacter) As Integer
        Return character.GetStatistic(StatisticTypes.XPLevel)
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
    <Extension>
    Function ItemCountsByName(character As ICharacter) As IReadOnlyDictionary(Of String, IEnumerable(Of IItem))
        Return character.Items.GroupBy(Function(x) x.Name).ToDictionary(Function(x) x.Key, Function(x) x.AsEnumerable)
    End Function
End Module
