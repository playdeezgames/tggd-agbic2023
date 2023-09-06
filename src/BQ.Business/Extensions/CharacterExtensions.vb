Public Module CharacterExtensions
    Public Sub AddRecipeInputs(character As ICharacter, msg As IMessage, recipeType As String)
        For Each input In RecipeTypes.Inputs(recipeType)
            msg.AddLine(7, $"{ToItemTypeDescriptor(input.ItemType).Name}: {character.ItemTypeCount(input.ItemType)}/{input.Count}")
        Next
    End Sub

    Public Sub ReportNeededRecipeInputs(character As ICharacter, msg As IMessage, recipeName As String)
        For Each input In RecipeTypes.Inputs(recipeName)
            msg.AddLine(7, $"{ToItemTypeDescriptor(input.ItemType).Name}: {character.ItemTypeCount(input.ItemType)}/{input.Count}")
        Next
    End Sub

    Public Sub DoRecipe(character As ICharacter, energyCost As Integer, recipeType As String, taskName As String, resultName As String)
        If CanDoRecipe(character, recipeType, taskName) Then
            If Not ConsumeEnergy(character, energyCost, taskName) Then
                Return
            End If
            RecipeTypes.Craft(recipeType, character)
            character.World.CreateMessage().
                AddLine(7, $"{CharacterExtensions.Name(character)} {resultName}.")
        End If
    End Sub

    Public Function ConsumeEnergy(character As ICharacter, energyCost As Integer, actionName As String) As Boolean
        If CharacterExtensions.Energy(character) < energyCost Then
            character.World.CreateMessage().AddLine(7, $"{CharacterExtensions.Name(character)} doesn't have the energy to {actionName}.")
            Return False
        End If
        CharacterExtensions.AddEnergy(character, -energyCost)
        Return True
    End Function

    Private Function CanDoRecipe(character As ICharacter, recipeType As String, taskName As String) As Boolean
        If Not RecipeTypes.CanCraft(recipeType, character) Then
            Dim msg = character.World.CreateMessage().
                AddLine(7, $"To {taskName},").
                AddLine(7, $"{CharacterExtensions.Name(character)} needs:")
            For Each input In RecipeTypes.Inputs(recipeType)
                msg.AddLine(7, $"{ToItemTypeDescriptor(input.ItemType).Name}: {character.ItemTypeCount(input.ItemType)}/{input.Count}")
            Next
            Return False
        End If
        Return True
    End Function


    Public Sub CookRecipe(character As ICharacter, recipeType As String, taskName As String, resultName As String)
        If CheckForFire(character, taskName) Then
            DoRecipe(character, 0, recipeType, taskName, resultName)
        End If
    End Sub

    Private Function CheckForFurnace(character As ICharacter, taskName As String) As Boolean
        If Not TerrainTypes.Descriptor(character.Cell).GetFlag("IsFurnace") Then
            character.World.CreateMessage().
                AddLine(7, $"{CharacterExtensions.Name(character)} needs a furnace to {taskName}.")
            Return False
        End If
        Return True
    End Function

    Private Function CheckForFire(character As ICharacter, taskName As String) As Boolean
        If Not TerrainTypes.Descriptor(character.Cell).GetFlag("HasFire") Then
            character.World.CreateMessage().
                AddLine(7, $"{CharacterExtensions.Name(character)} needs a fire to {taskName}.")
            Return False
        End If
        Return True
    End Function

    Public Sub CookFurnaceRecipe(character As ICharacter, recipeType As String, taskName As String, resultName As String)
        If CheckForFurnace(character, taskName) Then
            DoRecipe(character, 0, recipeType, taskName, resultName)
        End If
    End Sub
    Public Sub DoLearnSkill(character As ICharacter, effect As IEffect)
        Dim msg = character.World.CreateMessage
        Dim taskName = effect.GetMetadata("TaskName")
        If CharacterExtensions.AlreadyKnows(character, effect, msg, taskName) Then Return
        Dim recipeType = effect.GetMetadata("RecipeType")
        If Not RecipeTypes.CanCraft(recipeType, character) Then
            msg.
            AddLine(7, $"To learn to {taskName},").
            AddLine(7, $"{CharacterExtensions.Name(character)} needs:")
            For Each input In RecipeTypes.Inputs(recipeType)
                msg.AddLine(7, $"{ItemTypes.ToItemTypeDescriptor(input.ItemType).Name}: {character.ItemTypeCount(input.ItemType)}/{input.Count}")
            Next
            Return
        End If
        If Not LearnSkill(character, effect, msg, taskName) Then Return
        If effect.GetFlag("LearnByDoing") Then
            RecipeTypes.Craft(recipeType, character)
        End If
        msg.
            AddLine(7, $"You now know how to {taskName}!").
            AddLine(7, $"To do so, simply select '{effect.GetMetadata("ActionName")}'").
            AddLine(7, "from the Actions menu.")
        If effect.HasMetadata("Caveat") Then
            msg.AddLine(7, effect.GetMetadata("Caveat"))
        End If
    End Sub

    Private Function LearnSkill(character As ICharacter, effect As IEffect, msg As IMessage, text As String) As Boolean
        Dim learnCost = effect.GetStatistic("AdvancementPoints")
        If CharacterExtensions.AdvancementPoints(character) < learnCost Then
            msg.
                AddLine(7, $"To learn to {text},").
                AddLine(7, $"{CharacterExtensions.Name(character)} needs {learnCost} AP,").
                AddLine(7, $"but has {CharacterExtensions.AdvancementPoints(character)}!")
            Return False
        End If
        CharacterExtensions.AddAdvancementPoints(character, -learnCost)
        character.SetFlag(effect.GetMetadata("FlagType"), True)
        Return True
    End Function
    Public Function AlreadyKnows(character As ICharacter, effect As IEffect, msg As IMessage, text As String) As Boolean
        If character.GetFlag(effect.GetMetadata("FlagType")) Then
            msg.AddLine(7, $"{CharacterExtensions.Name(character)} already know how to {text}!")
            Return True
        End If
        Return False
    End Function


    Public Sub DoHealing(character As ICharacter, item As IItem, amount As Integer)
        CharacterExtensions.SetHealth(character, CharacterExtensions.Health(character) + amount)
        character.World.CreateMessage().
            AddLine(7, $"{ItemExtensions.Name(item)} restores {amount} health!").
            AddLine(7, $"{CharacterExtensions.Name(character)} now has {CharacterExtensions.Health(character)}/{CharacterExtensions.MaximumHealth(character)} health")
    End Sub
    Public Function ConsumedItem(character As ICharacter, effect As IEffect) As IItem
        Dim item = EffectExtensions.ToItemEffect(effect).Item
        character.RemoveItem(item)
        item.Recycle()
        Return item
    End Function

    Public Sub DetermineSpiciness(character As ICharacter, msg As IMessage)
        If RNG.GenerateBoolean(5, 5) Then
            CharacterExtensions.AwardXP(character, msg.AddLine(11, "That was a spicy one!"), 1)
        End If
    End Sub

    Public Function HasWon(character As ICharacter) As Boolean
        Return character.IsAvatar AndAlso character.ItemTypeCount("Bagel") > 0
    End Function
    Public Function CanCookBagel(character As ICharacter) As Boolean
        Return RecipeTypes.CanCraft("Bagel", character)
    End Function
    Public Function HealthDisplay(character As ICharacter) As String
        Return $"HP {CharacterExtensions.Health(character)}/{CharacterExtensions.MaximumHealth(character)}"
    End Function
    Public Function EnergyDisplay(character As ICharacter) As String
        Return $"EN {CharacterExtensions.Energy(character)}/{CharacterExtensions.MaximumEnergy(character)}"
    End Function
    Public Function XPLevelDisplay(character As ICharacter) As String
        Return $"LV {CharacterExtensions.XPLevel(character)}"
    End Function
    Public Function XPDisplay(character As ICharacter) As String
        Return $"XP {CharacterExtensions.XP(character)}/{CharacterExtensions.XPGoal(character)}"
    End Function
    Public Function JoolsDisplay(character As ICharacter) As String
        Return $"$ {CharacterExtensions.Jools(character)}"
    End Function
    Public Function APDisplay(character As ICharacter) As String
        Return $"AP {CharacterExtensions.AdvancementPoints(character)}"
    End Function
    Public Function ATKDisplay(character As ICharacter) As String
        Return $"ATK: Max {CharacterExtensions.MaximumAttack(character)} avg {CharacterExtensions.AttackDice(character) / 6:f2}"
    End Function
    Public Function DEFDisplay(character As ICharacter) As String
        Return $"DEF: Max {CharacterExtensions.MaximumDefend(character)} avg {CharacterExtensions.DefendDice(character) / 6:f2}"
    End Function
    Public Function CanBuildFurnace(character As ICharacter) As Boolean
        Return RecipeTypes.CanCraft("Furnace", character)
    End Function
    Public Function HasItemTypeInInventory(character As ICharacter, itemType As String) As Boolean
        Return character.Items.Any(Function(x) x.ItemType = itemType)
    End Function
    Public Sub Sleep(character As ICharacter)
        If Not character.IsAvatar Then
            Return
        End If
        If Not character.Map.GetFlag("CampingAllowed") OrElse Not CellExtensions.CanSleep(character.Cell) Then
            character.World.CreateMessage().AddLine(7, $"{CharacterExtensions.Name(character)} cannot sleep here!")
            Return
        End If
        CharacterExtensions.AddEnergy(character, CharacterExtensions.MaximumEnergy(character) \ 2)
        Dim msg = character.World.CreateMessage().
            AddLine(7, $"{CharacterExtensions.Name(character)} sleeps.").
            AddLine(7, $"{CharacterExtensions.Name(character)} now has {CharacterExtensions.Energy(character)}/{CharacterExtensions.MaximumEnergy(character)} energy.")
        CharacterExtensions.AddPeril(character, CharacterExtensions.MaximumEnergy(character) \ 2)
        CharacterExtensions.Move(character, (0, 0))
        If character.Cell.HasOtherCharacters(character) Then
            msg.AddLine(4, $"{CharacterExtensions.Name(character)} awakens to a surprise attack!")
        End If
    End Sub
    Public Sub DoBuildFire(character As ICharacter)
        TerrainTypes.Descriptor(character.Cell).DoEffect(character, "BuildFire", character.Cell)
    End Sub
    Public Sub DoBuildFurnace(character As ICharacter)
        TerrainTypes.Descriptor(character.Cell).DoEffect(character, "BuildFurnace", character.Cell)
    End Sub
    Public Sub DoCookBagel(character As ICharacter)
        TerrainTypes.Descriptor(character.Cell).DoEffect(character, "CookBagel", character.Cell)
    End Sub
    Public Sub DoMakeTorch(character As ICharacter)
        TerrainTypes.Descriptor(character.Cell).DoEffect(character, "MakeTorch", character.Cell)
    End Sub
    Public Sub DoMakeHatchet(character As ICharacter)
        DoMakeItem(character, "Hatchet", "a hatchet", AddressOf MakeHatchet)
    End Sub
    Public Sub DoMakeItem(character As ICharacter, recipeType As String, noun As String, makeAction As Action(Of ICharacter))
        If Not RecipeTypes.CanCraft(recipeType, character) Then
            Dim msg = character.World.CreateMessage().AddLine(7, $"To make {noun},").AddLine(7, $"{CharacterExtensions.Name(character)} needs:")
            AddRecipeInputs(character, msg, recipeType)
            Return
        End If
        makeAction.Invoke(character)
        character.World.CreateMessage().AddLine(7, $"{CharacterExtensions.Name(character)} makes {noun}.")
    End Sub
    Public Sub DoPutOutFire(character As ICharacter)
        TerrainTypes.Descriptor(character.Cell).DoEffect(character, "PutOutFire", character.Cell)
    End Sub
    Public Sub DoKnap(character As ICharacter)
        DoMakeItem(character, "SharpRock", "a sharp rock", AddressOf Knap)
    End Sub
    Public Sub DoMakeTwine(character As ICharacter)
        DoMakeItem(character, "Twine", "twine", AddressOf MakeTwine)
    End Sub
    Public Sub MakeTwine(character As ICharacter)
        RecipeTypes.Craft("Twine", character)
    End Sub
    Public Sub MakeHatchet(character As ICharacter)
        RecipeTypes.Craft("Hatchet", character)
    End Sub
    Public Sub Knap(character As ICharacter)
        RecipeTypes.Craft("SharpRock", character)
    End Sub
    Public Function Name(character As ICharacter) As String
        Return CharacterExtensions.Descriptor(character).Name
    End Function
    Public Function Weapons(character As ICharacter) As IEnumerable(Of IItem)
        Return character.EquippedItems.Where(Function(x) ItemExtensions.IsWeapon(x))
    End Function
    Public Function Armors(character As ICharacter) As IEnumerable(Of IItem)
        Return character.EquippedItems.Where(Function(x) ItemExtensions.IsArmor(x))
    End Function
    Public Sub TakeItem(character As ICharacter, item As IItem)
        character.Cell.RemoveItem(item)
        character.AddItem(item)
    End Sub
    Public Sub DropItem(character As ICharacter, item As IItem)
        character.Cell.AddItem(item)
        character.RemoveItem(item)
    End Sub
    Public Function CanEnter(character As ICharacter, cell As ICell) As Boolean
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
        Dim effect = ToItemTypeDescriptor(item.ItemType).ToItemEffect(effectType, item)
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
    Public Sub AddPeril(character As ICharacter, delta As Integer)
        CharacterExtensions.SetPeril(character, CharacterExtensions.Peril(character) + delta)
    End Sub
    Public Sub EnterCell(character As ICharacter)
        If CellExtensions.Peril(character.Cell) > 0 Then
            CharacterExtensions.AddPeril(character, CellExtensions.Peril(character.Cell))
            If CharacterExtensions.Peril(character) > 0 Then
                Dim roll = RNG.RollDice("1d20")
                If roll <= CharacterExtensions.Peril(character) Then
                    Dim enemyType = TerrainTypes.Descriptor(character.Cell).GenerateCreatureType()
                    Dim enemy = CreateCharacter(enemyType, character.Cell)
                    CharacterExtensions.SetPeril(character, CharacterExtensions.Peril(character) - CharacterExtensions.Peril(enemy))
                    character.Cell.AddCharacter(enemy)
                End If
            End If
        End If
    End Sub
    Public Sub SetPeril(character As ICharacter, peril As Integer)
        character.SetStatistic("Peril", Math.Max(0, peril))
    End Sub
    Public Function Peril(character As ICharacter) As Integer
        Return character.GetStatistic("Peril")
    End Function
    Public Function Energy(character As ICharacter) As Integer
        Return character.GetStatistic("Energy")
    End Function
    Public Sub AddEnergy(character As ICharacter, delta As Integer)
        character.SetStatistic("Energy", Math.Clamp(CharacterExtensions.Energy(character) + delta, 0, CharacterExtensions.MaximumEnergy(character)))
    End Sub
    Public Function MaximumEnergy(character As ICharacter) As Integer
        Return character.GetStatistic("MaximumEnergy")
    End Function
    Public Sub SetMaximumEnergy(character As ICharacter, maximumEnergy As Integer)
        character.SetStatistic("MaximumEnergy", maximumEnergy)
    End Sub
    Public Function Health(character As ICharacter) As Integer
        Return character.GetStatistic("Health")
    End Function
    Public Function MaximumHealth(character As ICharacter) As Integer
        Return character.GetStatistic("MaximumHealth")
    End Function
    Public Function AttackDice(character As ICharacter) As Integer
        Return character.GetStatistic("AttackDice") + character.EquippedItems.Sum(Function(x) ItemExtensions.AttackDice(x))
    End Function
    Public Function MaximumAttack(character As ICharacter) As Integer
        Return character.GetStatistic("MaximumAttack") + character.EquippedItems.Sum(Function(x) ItemExtensions.MaximumAttack(x))
    End Function
    Public Function DefendDice(character As ICharacter) As Integer
        Return character.GetStatistic("DefendDice") + character.EquippedItems.Sum(Function(x) ItemExtensions.DefendDice(x))
    End Function
    Public Function MaximumDefend(character As ICharacter) As Integer
        Return character.GetStatistic("MaximumDefend") + character.EquippedItems.Sum(Function(x) ItemExtensions.MaximumDefend(x))
    End Function
    Public Function RollAttack(character As ICharacter) As Integer
        Return Math.Min(CharacterExtensions.MaximumAttack(character), Enumerable.Range(0, CharacterExtensions.AttackDice(character)).Sum(Function(x) RNG.RollDice("1d6/6")))
    End Function
    Public Function RollDefend(character As ICharacter) As Integer
        Return Math.Min(CharacterExtensions.MaximumDefend(character), Enumerable.Range(0, CharacterExtensions.DefendDice(character)).Sum(Function(x) RNG.RollDice("1d6/6")))
    End Function
    Public Sub SetHealth(character As ICharacter, health As Integer)
        character.SetStatistic("Health", Math.Clamp(health, 0, CharacterExtensions.MaximumHealth(character)))
    End Sub
    Public Sub SetMaximumHealth(character As ICharacter, maximumHealth As Integer)
        character.SetStatistic("MaximumHealth", Math.Max(1, maximumHealth))
        CharacterExtensions.SetHealth(character, CharacterExtensions.Health(character))
    End Sub
    Public Function IsDead(character As ICharacter) As Boolean
        Return CharacterExtensions.Health(character) <= 0
    End Function
    Public Sub Die(character As ICharacter)
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
    Public Function AdvancementPointsPerLevel(character As ICharacter) As Integer
        Return character.GetStatistic("AdvancementPointsPerLevel")
    End Function
    Public Sub AddAdvancementPoints(character As ICharacter, advancementPoints As Integer)
        character.SetStatistic("AdvancementPoints", Math.Max(0, character.GetStatistic("AdvancementPoints") + advancementPoints))
    End Sub
    Public Function AddXP(character As ICharacter, xp As Integer) As Boolean
        character.AddStatistic("XP", xp)
        If CharacterExtensions.XP(character) >= CharacterExtensions.XPGoal(character) Then
            CharacterExtensions.AddAdvancementPoints(character, CharacterExtensions.AdvancementPointsPerLevel(character))
            character.AddStatistic("XPLevel", 1)
            Dim currentGoal = CharacterExtensions.XPGoal(character)
            character.AddStatistic("XPGoal", character.GetStatistic("XPGoal"))
            CharacterExtensions.AddXP(character, -currentGoal)
            Return True
        End If
        Return False
    End Function
    Public Sub AddJools(character As ICharacter, jools As Integer)
        CharacterExtensions.SetJools(character, CharacterExtensions.Jools(character) + jools)
    End Sub
    Public Function Jools(character As ICharacter) As Integer
        Return character.GetStatistic("Jools")
    End Function
    Public Sub SetJools(character As ICharacter, jools As Integer)
        character.SetStatistic("Jools", Math.Max(0, jools))
    End Sub
    Public Sub AwardJools(character As ICharacter, msg As IMessage, jools As Integer)
        If Not character.IsAvatar Then
            Return
        End If
        If jools > 0 Then
            msg.AddLine(7, $"{CharacterExtensions.Name(character)} gets {jools} jools!")
            CharacterExtensions.AddJools(character, jools)
        End If
    End Sub
    Public Function AdvancementPoints(character As ICharacter) As Integer
        Return character.GetStatistic("AdvancementPoints")
    End Function
    Public Sub AwardXP(character As ICharacter, msg As IMessage, xp As Integer)
        If Not character.IsAvatar OrElse xp = 0 Then
            Return
        End If
        msg.AddLine(7, $"{CharacterExtensions.Name(character)} gains {xp} XP!")
        If CharacterExtensions.AddXP(character, xp) Then
            msg.AddLine(10, $"{CharacterExtensions.Name(character)} is now level {CharacterExtensions.XPLevel(character)}!")
            msg.AddLine(7, $"{CharacterExtensions.Name(character)} now has {CharacterExtensions.AdvancementPoints(character)} AP!")
        Else
            msg.AddLine(7, $"{CharacterExtensions.Name(character)} needs {CharacterExtensions.XPGoal(character) - CharacterExtensions.XP(character)} for the next level.")
        End If
    End Sub
    Public Function ScuffWeapons(character As ICharacter, scuffAmount As Integer, msg As IMessage) As Boolean
        Dim items = character.EquippedItems.Where(Function(x) ItemExtensions.IsWeapon(x) AndAlso ItemExtensions.Durability(x) > 0)
        Dim result = False
        While scuffAmount > 0 AndAlso items.Any
            Dim item = RNG.FromEnumerable(items)
            ItemExtensions.AddDurability(item, -1)
            If ItemExtensions.IsBroken(item) Then
                msg.AddLine(4, $"{CharacterExtensions.Name(character)}' {ItemExtensions.Name(item)} breaks!")
                character.UnequipItem(item)
                character.RemoveItem(item)
                item.Recycle()
                result = True
            End If
            scuffAmount -= 1
            items = character.EquippedItems.Where(Function(x) ItemExtensions.IsWeapon(x) AndAlso ItemExtensions.Durability(x) > 0)
        End While
        Return result
    End Function
    Public Function ScuffArmors(character As ICharacter, scuffAmount As Integer, msg As IMessage) As Boolean
        Dim items = character.EquippedItems.Where(Function(x) ItemExtensions.IsArmor(x) AndAlso ItemExtensions.Durability(x) > 0)
        Dim result = False
        While scuffAmount > 0 AndAlso items.Any
            Dim item = RNG.FromEnumerable(items)
            ItemExtensions.AddDurability(item, -1)
            If ItemExtensions.IsBroken(item) Then
                msg.AddLine(4, $"{CharacterExtensions.Name(character)}' {ItemExtensions.Name(item)} breaks!")
                character.UnequipItem(item)
                character.RemoveItem(item)
                item.Recycle()
                result = True
            End If
            scuffAmount -= 1
            items = character.EquippedItems.Where(Function(x) ItemExtensions.IsArmor(x) AndAlso ItemExtensions.Durability(x) > 0)
        End While
        Return result
    End Function
    Public Function Attack(attacker As ICharacter, defender As ICharacter, Optional message As String = Nothing) As Boolean
        If CharacterExtensions.IsDead(defender) Then
            Return False
        End If
        Dim result = False
        Dim msg = attacker.World.CreateMessage
        If Not String.IsNullOrEmpty(message) Then
            msg.AddLine(7, message)
        End If
        msg.AddLine(7, $"{CharacterExtensions.Name(attacker)} attacks {CharacterExtensions.Name(defender)}")
        Dim attackRoll = CharacterExtensions.RollAttack(attacker)
        msg.AddLine(7, $"{CharacterExtensions.Name(attacker)} rolls an attack of {attackRoll}")
        result = CharacterExtensions.ScuffWeapons(attacker, attackRoll, msg) OrElse result
        Dim defendRoll = CharacterExtensions.RollDefend(defender)
        msg.AddLine(7, $"{CharacterExtensions.Name(defender)} rolls a defend of {defendRoll}")
        Dim damage = Math.Max(0, attackRoll - defendRoll)
        result = CharacterExtensions.ScuffArmors(defender, Math.Max(defendRoll, damage), msg) OrElse result
        If damage <= 0 Then
            msg.AddLine(7, $"{CharacterExtensions.Name(attacker)} misses.")
            msg.SetSfx(If(attacker.IsAvatar, Sfx.PlayerMiss, Sfx.EnemyMiss))
            Return result
        End If
        result = True
        msg.AddLine(7, $"{CharacterExtensions.Name(defender)} takes {damage} damage")
        CharacterExtensions.SetHealth(defender, CharacterExtensions.Health(defender) - damage)
        If CharacterExtensions.IsDead(defender) Then
            msg.SetSfx(If(defender.IsAvatar, Sfx.PlayerDeath, Sfx.EnemyDeath))
            msg.AddLine(7, $"{CharacterExtensions.Name(attacker)} kills {CharacterExtensions.Name(defender)}")
            CharacterExtensions.AwardJools(attacker, msg, CharacterExtensions.Jools(defender))
            CharacterExtensions.AwardXP(attacker, msg, CharacterExtensions.XP(defender))
            CharacterExtensions.Die(defender)
            Return result
        End If
        msg.SetSfx(If(defender.IsAvatar, Sfx.PlayerHit, Sfx.EnemyHit))
        msg.AddLine(7, $"{CharacterExtensions.Name(defender)} has {CharacterExtensions.Health(defender)}/{CharacterExtensions.MaximumHealth(defender)} health.")
        Return result
    End Function
    Function XP(character As ICharacter) As Integer
        Return character.GetStatistic("XP")
    End Function
    Function XPGoal(character As ICharacter) As Integer
        Return character.GetStatistic("XPGoal")
    End Function
    Function XPLevel(character As ICharacter) As Integer
        Return character.GetStatistic("XPLevel")
    End Function
    Sub EquipItem(character As ICharacter, item As IItem)
        Dim equipSlotType = ToItemTypeDescriptor(item.ItemType).EquipSlotType
        character.Equip(equipSlotType, item)
    End Sub
    Function HasCuttingTool(character As ICharacter) As Boolean
        Return character.Items.Any(Function(x) ToItemTypeDescriptor(x.ItemType).IsCuttingTool) OrElse character.EquippedItems.Any(Function(x) ToItemTypeDescriptor(x.ItemType).IsCuttingTool)
    End Function
    Function ItemCountsByName(character As ICharacter) As IReadOnlyDictionary(Of String, IEnumerable(Of IItem))
        Return character.Items.GroupBy(Function(x) ItemExtensions.Name(x)).ToDictionary(Function(x) x.Key, Function(x) x.AsEnumerable)
    End Function

    Public Function ItemsOfItemType(character As ICharacter, itemType As String) As IItem()
        Dim result = character.Items.Where(Function(x) x.ItemType = itemType).ToArray
        Return result
    End Function
End Module
