﻿Public Module CharacterExtensions
    Public Function HasWon(character As ICharacter) As Boolean
        Return character.IsAvatar AndAlso character.ItemTypeCount(ItemTypes.Bagel) > 0
    End Function
    Public Function CanCookBagel(character As ICharacter) As Boolean
        Return RecipeTypes.CanCraft(RecipeTypes.Bagel, character)
    End Function
    Public Function HealthDisplay(character As ICharacter) As String
        Return $"HP {CharacterExtensions.Health(character)}/{character.MaximumHealth}"
    End Function
    Public Function EnergyDisplay(character As ICharacter) As String
        Return $"EN {CharacterExtensions.Energy(character)}/{CharacterExtensions.MaximumEnergy(character)}"
    End Function
    Public Function XPLevelDisplay(character As ICharacter) As String
        Return $"LV {character.XPLevel}"
    End Function
    Public Function XPDisplay(character As ICharacter) As String
        Return $"XP {character.XP}/{character.XPGoal}"
    End Function
    Public Function JoolsDisplay(character As ICharacter) As String
        Return $"$ {character.Jools}"
    End Function
    Public Function APDisplay(character As ICharacter) As String
        Return $"AP {character.AdvancementPoints}"
    End Function
    Public Function ATKDisplay(character As ICharacter) As String
        Return $"ATK: Max {character.MaximumAttack} avg {character.AttackDice / 6:f2}"
    End Function
    Public Function DEFDisplay(character As ICharacter) As String
        Return $"DEF: Max {character.MaximumDefend} avg {character.DefendDice / 6:f2}"
    End Function
    Public Function CanBuildFurnace(character As ICharacter) As Boolean
        Return RecipeTypes.CanCraft(RecipeTypes.Furnace, character)
    End Function
    Public Function HasItemTypeInInventory(character As ICharacter, itemType As String) As Boolean
        Return character.Items.Any(Function(x) x.ItemType = itemType)
    End Function
    Public Sub Sleep(character As ICharacter)
        If Not character.IsAvatar Then
            Return
        End If
        If Not character.Map.CampingAllowed OrElse Not CellExtensions.CanSleep(character.Cell) Then
            character.World.CreateMessage().AddLine(LightGray, $"{CharacterExtensions.Name(character)} cannot sleep here!")
            Return
        End If
        CharacterExtensions.AddEnergy(character, CharacterExtensions.MaximumEnergy(character) \ 2)
        Dim msg = character.World.CreateMessage().
            AddLine(LightGray, $"{CharacterExtensions.Name(character)} sleeps.").
            AddLine(LightGray, $"{CharacterExtensions.Name(character)} now has {CharacterExtensions.Energy(character)}/{CharacterExtensions.MaximumEnergy(character)} energy.")
        CharacterExtensions.AddPeril(character, CharacterExtensions.MaximumEnergy(character) \ 2)
        CharacterExtensions.Move(character, (0, 0))
        If character.Cell.HasOtherCharacters(character) Then
            msg.AddLine(Red, $"{CharacterExtensions.Name(character)} awakens to a surprise attack!")
        End If
    End Sub
    Public Sub DoBuildFire(character As ICharacter)
        character.Cell.Descriptor.DoEffect(character, EffectTypes.BuildFire, character.Cell)
    End Sub
    Public Sub DoBuildFurnace(character As ICharacter)
        character.Cell.Descriptor.DoEffect(character, EffectTypes.BuildFurnace, character.Cell)
    End Sub
    Public Sub DoCookBagel(character As ICharacter)
        character.Cell.Descriptor.DoEffect(character, EffectTypes.CookBagel, character.Cell)
    End Sub
    Public Sub DoMakeTorch(character As ICharacter)
        character.Cell.Descriptor.DoEffect(character, EffectTypes.MakeTorch, character.Cell)
    End Sub
    Public Sub DoMakeHatchet(character As ICharacter)
        DoMakeItem(character, RecipeTypes.Hatchet, "a hatchet", AddressOf MakeHatchet)
    End Sub
    Private Sub DoMakeItem(character As ICharacter, recipeType As String, noun As String, makeAction As Action(Of ICharacter))
        If Not RecipeTypes.CanCraft(recipeType, character) Then
            Dim msg = character.World.CreateMessage.AddLine(LightGray, $"To make {noun},").AddLine(LightGray, $"{CharacterExtensions.Name(character)} needs:")
            CraftingEffectHandlers.AddRecipeInputs(character, msg, recipeType)
            Return
        End If
        makeAction.Invoke(character)
        character.World.CreateMessage.AddLine(LightGray, $"{CharacterExtensions.Name(character)} makes {noun}.")
    End Sub
    Public Sub DoPutOutFire(character As ICharacter)
        character.Cell.Descriptor.DoEffect(character, EffectTypes.PutOutFire, character.Cell)
    End Sub
    Public Sub DoKnap(character As ICharacter)
        DoMakeItem(character, RecipeTypes.SharpRock, "a sharp rock", AddressOf Knap)
    End Sub
    Public Sub DoMakeTwine(character As ICharacter)
        DoMakeItem(character, RecipeTypes.Twine, "twine", AddressOf MakeTwine)
    End Sub
    Public Sub MakeTwine(character As ICharacter)
        RecipeTypes.Craft(RecipeTypes.Twine, character)
    End Sub
    Public Sub MakeHatchet(character As ICharacter)
        RecipeTypes.Craft(RecipeTypes.Hatchet, character)
    End Sub
    Public Sub Knap(character As ICharacter)
        RecipeTypes.Craft(RecipeTypes.SharpRock, character)
    End Sub
    Public Function Name(character As ICharacter) As String
        Return CharacterExtensions.Descriptor(character).Name
    End Function
    Private Function Weapons(character As ICharacter) As IEnumerable(Of IItem)
        Return character.EquippedItems.Where(Function(x) x.IsWeapon)
    End Function
    Private Function Armors(character As ICharacter) As IEnumerable(Of IItem)
        Return character.EquippedItems.Where(Function(x) x.IsArmor)
    End Function
    Public Sub TakeItem(character As ICharacter, item As IItem)
        character.Cell.RemoveItem(item)
        character.AddItem(item)
    End Sub
    Public Sub DropItem(character As ICharacter, item As IItem)
        character.Cell.AddItem(item)
        character.RemoveItem(item)
    End Sub
    Private Function CanEnter(character As ICharacter, cell As ICell) As Boolean
        Return cell IsNot Nothing AndAlso CellExtensions.IsTenable(cell)
    End Function
    Public Sub DoMapEffect(character As ICharacter, cell As ICell)
        If cell IsNot Nothing AndAlso cell.HasEffect Then
            Dim effect = cell.Effect
            CharacterExtensions.Descriptor(character).RunEffectScript(WorldModel.LuaState, effect.EffectType, character, effect)
        End If
    End Sub
    Public Function Descriptor(character As ICharacter) As CharacterTypeDescriptor
        Return character.CharacterType.ToCharacterTypeDescriptor
    End Function
    Public Sub DoItemEffect(character As ICharacter, effectType As String, item As IItem)
        Dim effect = item.ItemType.ToItemTypeDescriptor.ToItemEffect(effectType, item)
        CharacterExtensions.Descriptor(character).RunEffectScript(WorldModel.LuaState, effect.EffectType, character, effect)
    End Sub
    Public Function Move(character As ICharacter, delta As (x As Integer, y As Integer)) As Boolean
        Dim cell = character.Cell
        Dim nextCell = cell.Map.GetCell(cell.Column + delta.x, cell.Row + delta.y)
        If Not CharacterExtensions.CanEnter(character, nextCell) Then
            CharacterExtensions.DoMapEffect(character, nextCell)
            Return False
        End If

        If delta.x <> 0 OrElse delta.y <> 0 Then
            nextCell.AddCharacter(character)
            character.Cell.RemoveCharacter(character)
            character.Cell = nextCell
        End If

        CharacterExtensions.DoMapEffect(character, nextCell)
        CharacterExtensions.EnterCell(character)
        Return True
    End Function
    Private Sub AddPeril(character As ICharacter, delta As Integer)
        CharacterExtensions.SetPeril(character, CharacterExtensions.Peril(character) + delta)
    End Sub
    Private Sub EnterCell(character As ICharacter)
        If CellExtensions.Peril(character.Cell) > 0 Then
            CharacterExtensions.AddPeril(character, CellExtensions.Peril(character.Cell))
            If CharacterExtensions.Peril(character) > 0 Then
                Dim roll = RNG.RollDice("1d20")
                If roll <= CharacterExtensions.Peril(character) Then
                    Dim enemyType = character.Cell.Descriptor.GenerateCreatureType()
                    Dim enemy = CreateCharacter(enemyType, character.Cell)
                    CharacterExtensions.SetPeril(character, CharacterExtensions.Peril(character) - CharacterExtensions.Peril(enemy))
                    character.Cell.AddCharacter(enemy)
                End If
            End If
        End If
    End Sub
    Private Sub SetPeril(character As ICharacter, peril As Integer)
        character.SetStatistic(StatisticTypes.Peril, Math.Max(0, peril))
    End Sub
    Private Function Peril(character As ICharacter) As Integer
        Return character.GetStatistic(StatisticTypes.Peril)
    End Function
    Public Function Energy(character As ICharacter) As Integer
        Return character.GetStatistic(StatisticTypes.Energy)
    End Function
    Public Sub AddEnergy(character As ICharacter, delta As Integer)
        character.SetStatistic(StatisticTypes.Energy, Math.Clamp(CharacterExtensions.Energy(character) + delta, 0, CharacterExtensions.MaximumEnergy(character)))
    End Sub
    Public Function MaximumEnergy(character As ICharacter) As Integer
        Return character.GetStatistic(StatisticTypes.MaximumEnergy)
    End Function
    Public Sub SetMaximumEnergy(character As ICharacter, maximumEnergy As Integer)
        character.SetStatistic(StatisticTypes.MaximumEnergy, maximumEnergy)
    End Sub
    Public Function Health(character As ICharacter) As Integer
        Return character.GetStatistic(StatisticTypes.Health)
    End Function
    <Extension>
    Public Function MaximumHealth(character As ICharacter) As Integer
        Return character.GetStatistic(StatisticTypes.MaximumHealth)
    End Function
    <Extension>
    Public Function AttackDice(character As ICharacter) As Integer
        Return character.GetStatistic(StatisticTypes.AttackDice) + character.EquippedItems.Sum(Function(x) x.AttackDice)
    End Function
    <Extension>
    Public Function MaximumAttack(character As ICharacter) As Integer
        Return character.GetStatistic(StatisticTypes.MaximumAttack) + character.EquippedItems.Sum(Function(x) x.MaximumAttack)
    End Function
    <Extension>
    Public Function DefendDice(character As ICharacter) As Integer
        Return character.GetStatistic(StatisticTypes.DefendDice) + character.EquippedItems.Sum(Function(x) x.DefendDice)
    End Function
    <Extension>
    Public Function MaximumDefend(character As ICharacter) As Integer
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
    Public Sub SetHealth(character As ICharacter, health As Integer)
        character.SetStatistic(StatisticTypes.Health, Math.Clamp(health, 0, character.MaximumHealth))
    End Sub
    <Extension>
    Public Sub SetMaximumHealth(character As ICharacter, maximumHealth As Integer)
        character.SetStatistic(StatisticTypes.MaximumHealth, Math.Max(1, maximumHealth))
        character.SetHealth(CharacterExtensions.Health(character))
    End Sub
    <Extension>
    Public Function IsDead(character As ICharacter) As Boolean
        Return CharacterExtensions.Health(character) <= 0
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
    Public Sub AddAdvancementPoints(character As ICharacter, advancementPoints As Integer)
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
    Public Sub AddJools(character As ICharacter, jools As Integer)
        character.SetJools(character.Jools + jools)
    End Sub
    <Extension>
    Public Function Jools(character As ICharacter) As Integer
        Return character.GetStatistic(StatisticTypes.Jools)
    End Function
    <Extension>
    Public Sub SetJools(character As ICharacter, jools As Integer)
        character.SetStatistic(StatisticTypes.Jools, Math.Max(0, jools))
    End Sub
    <Extension>
    Private Sub AwardJools(character As ICharacter, msg As IMessage, jools As Integer)
        If Not character.IsAvatar Then
            Return
        End If
        If jools > 0 Then
            msg.AddLine(LightGray, $"{CharacterExtensions.Name(character)} gets {jools} jools!")
            character.AddJools(jools)
        End If
    End Sub
    <Extension>
    Public Function AdvancementPoints(character As ICharacter) As Integer
        Return character.GetStatistic(StatisticTypes.AdvancementPoints)
    End Function
    <Extension>
    Public Sub AwardXP(character As ICharacter, msg As IMessage, xp As Integer)
        If Not character.IsAvatar OrElse xp = 0 Then
            Return
        End If
        msg.AddLine(LightGray, $"{CharacterExtensions.Name(character)} gains {xp} XP!")
        If character.AddXP(xp) Then
            msg.AddLine(LightGreen, $"{CharacterExtensions.Name(character)} is now level {character.XPLevel}!")
            msg.AddLine(LightGray, $"{CharacterExtensions.Name(character)} now has {character.AdvancementPoints} AP!")
        Else
            msg.AddLine(LightGray, $"{CharacterExtensions.Name(character)} needs {character.XPGoal - character.XP} for the next level.")
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
                msg.AddLine(Red, $"{CharacterExtensions.Name(character)}' {item.Name} breaks!")
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
                msg.AddLine(Red, $"{CharacterExtensions.Name(character)}' {item.Name} breaks!")
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
    Public Function Attack(attacker As ICharacter, defender As ICharacter, Optional message As String = Nothing) As Boolean
        If defender.IsDead Then
            Return False
        End If
        Dim result = False
        Dim msg = attacker.World.CreateMessage
        If Not String.IsNullOrEmpty(message) Then
            msg.AddLine(LightGray, message)
        End If
        msg.AddLine(LightGray, $"{CharacterExtensions.Name(attacker)} attacks {CharacterExtensions.Name(defender)}")
        Dim attackRoll = attacker.RollAttack()
        msg.AddLine(LightGray, $"{CharacterExtensions.Name(attacker)} rolls an attack of {attackRoll}")
        result = attacker.ScuffWeapons(attackRoll, msg) OrElse result
        Dim defendRoll = defender.RollDefend()
        msg.AddLine(LightGray, $"{CharacterExtensions.Name(defender)} rolls a defend of {defendRoll}")
        Dim damage = Math.Max(0, attackRoll - defendRoll)
        result = defender.ScuffArmors(Math.Max(defendRoll, damage), msg) OrElse result
        If damage <= 0 Then
            msg.AddLine(LightGray, $"{CharacterExtensions.Name(attacker)} misses.")
            msg.SetSfx(If(attacker.IsAvatar, Sfx.PlayerMiss, Sfx.EnemyMiss))
            Return result
        End If
        result = True
        msg.AddLine(LightGray, $"{CharacterExtensions.Name(defender)} takes {damage} damage")
        defender.SetHealth(CharacterExtensions.Health(defender) - damage)
        If defender.IsDead Then
            msg.SetSfx(If(defender.IsAvatar, Sfx.PlayerDeath, Sfx.EnemyDeath))
            msg.AddLine(LightGray, $"{CharacterExtensions.Name(attacker)} kills {CharacterExtensions.Name(defender)}")
            attacker.AwardJools(msg, defender.Jools)
            attacker.AwardXP(msg, defender.XP)
            defender.Die()
            Return result
        End If
        msg.SetSfx(If(defender.IsAvatar, Sfx.PlayerHit, Sfx.EnemyHit))
        msg.AddLine(LightGray, $"{CharacterExtensions.Name(defender)} has {CharacterExtensions.Health(defender)}/{defender.MaximumHealth} health.")
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
