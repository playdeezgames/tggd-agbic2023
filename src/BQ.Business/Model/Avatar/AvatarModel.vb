Friend Class AvatarModel
    Implements IAvatarModel

    Private ReadOnly avatar As ICharacter

    Public Sub New(avatar As ICharacter)
        Me.avatar = avatar
    End Sub

    Public ReadOnly Property IsDead As Boolean Implements IAvatarModel.IsDead
        Get
            Return avatar.IsDead
        End Get
    End Property

    Public ReadOnly Property Character As (Glyph As Char, Hue As Integer, MaskGlyph As Char, MaskHue As Integer) Implements IAvatarModel.Character
        Get
            Dim descriptor = avatar.Descriptor
            Return (descriptor.Glyph, descriptor.Hue, descriptor.MaskGlyph, descriptor.MaskHue)
        End Get
    End Property

    Public ReadOnly Property Name As String Implements IAvatarModel.Name
        Get
            Return avatar.Name
        End Get
    End Property

    Public ReadOnly Property XP As Integer Implements IAvatarModel.XP
        Get
            Return avatar.XP
        End Get
    End Property

    Public ReadOnly Property XPGoal As Integer Implements IAvatarModel.XPGoal
        Get
            Return avatar.XPGoal
        End Get
    End Property

    Public ReadOnly Property XPLevel As Integer Implements IAvatarModel.XPLevel
        Get
            Return avatar.XPLevel
        End Get
    End Property

    Public ReadOnly Property HasItems As Boolean Implements IAvatarModel.HasItems
        Get
            Return avatar.HasItems
        End Get
    End Property

    Public ReadOnly Property LegacyItems As IEnumerable(Of (String, String)) Implements IAvatarModel.LegacyItems
        Get
            Return avatar.Items.GroupBy(Function(x) x.Name).Select(Function(x) ($"{x.Key}(x{x.Count})", x.Key))
        End Get
    End Property

    Public ReadOnly Property LegacyItemCount(itemName As String) As Integer Implements IAvatarModel.LegacyItemCount
        Get
            Return avatar.Items.Count(Function(x) x.Name = itemName)
        End Get
    End Property

    Public ReadOnly Property Jools As Integer Implements IAvatarModel.Jools
        Get
            Return avatar.TryGetStatistic(StatisticTypes.Jools)
        End Get
    End Property

    Public ReadOnly Property CanForage As Boolean Implements IAvatarModel.CanForage
        Get
            Return avatar.Flag(FlagTypes.KnowsForaging) AndAlso avatar.Cell.CanForage
        End Get
    End Property

    Public ReadOnly Property CanMakeTwine As Boolean Implements IAvatarModel.CanMakeTwine
        Get
            Return avatar.Flag(FlagTypes.KnowsTwineMaking)
        End Get
    End Property

    Public ReadOnly Property AdvancementPoints As Integer Implements IAvatarModel.AdvancementPoints
        Get
            Return avatar.AdvancementPoints
        End Get
    End Property

    Public ReadOnly Property HasEquipment As Boolean Implements IAvatarModel.HasEquipment
        Get
            Return avatar.HasEquipment
        End Get
    End Property

    Public ReadOnly Property Equipment As IEnumerable(Of (String, String)) Implements IAvatarModel.Equipment
        Get
            Return avatar.Equipment.Select(Function(x) ($"{x.Key.ToEquipSlotTypeDescriptor.Name}: {x.Value.FullName}", x.Key))
        End Get
    End Property

    Public ReadOnly Property Attack As (average As Double, maximum As Integer) Implements IAvatarModel.Attack
        Get
            Return (avatar.AttackDice / 6, avatar.MaximumAttack)
        End Get
    End Property

    Public ReadOnly Property Defend As (average As Double, maximum As Integer) Implements IAvatarModel.Defend
        Get
            Return (avatar.DefendDice / 6, avatar.MaximumDefend)
        End Get
    End Property

    Public ReadOnly Property CanSleep As Boolean Implements IAvatarModel.CanSleep
        Get
            Return avatar.Energy < avatar.MaximumEnergy
        End Get
    End Property

    Public ReadOnly Property CanBuildFire As Boolean Implements IAvatarModel.CanBuildFire
        Get
            Return avatar.Flag(FlagTypes.KnowsFireMaking) AndAlso avatar.Cell.CanBuildFire
        End Get
    End Property

    Public ReadOnly Property CanPutOutFire As Boolean Implements IAvatarModel.CanPutOutFire
        Get
            Return avatar.Cell.HasFire
        End Get
    End Property

    Public ReadOnly Property CanMakeTorch As Boolean Implements IAvatarModel.CanMakeTorch
        Get
            Return avatar.Flag(FlagTypes.KnowsTorchMaking) AndAlso avatar.Cell.CanMakeTorch
        End Get
    End Property

    Public ReadOnly Property CanMakeHatchet As Boolean Implements IAvatarModel.CanMakeHatchet
        Get
            Return avatar.Flag(FlagTypes.KnowsHatchetMaking)
        End Get
    End Property

    Public ReadOnly Property CanKnap As Boolean Implements IAvatarModel.CanKnap
        Get
            Return avatar.Flag(FlagTypes.KnowsKnapping)
        End Get
    End Property

    Public ReadOnly Property CanBuildFurnace As Boolean Implements IAvatarModel.CanBuildFurnace
        Get
            Return avatar.CanBuildFurnace AndAlso avatar.Cell.CanBuildFurnace
        End Get
    End Property

    Public ReadOnly Property CanCookBagel As Boolean Implements IAvatarModel.CanCookBagel
        Get
            Return avatar.CanCookBagel AndAlso avatar.Cell.CanCookBagel
        End Get
    End Property

    Public ReadOnly Property HasWon As Boolean Implements IAvatarModel.HasWon
        Get
            Return avatar.HasWon
        End Get
    End Property

    Public ReadOnly Property HealthDisplay As String Implements IAvatarModel.HealthDisplay
        Get
            Return avatar.HealthDisplay
        End Get
    End Property

    Public ReadOnly Property EnergyDisplay As String Implements IAvatarModel.EnergyDisplay
        Get
            Return avatar.EnergyDisplay
        End Get
    End Property

    Public ReadOnly Property XPLevelDisplay As String Implements IAvatarModel.XPLevelDisplay
        Get
            Return avatar.XPLevelDisplay
        End Get
    End Property

    Public ReadOnly Property XPDisplay As String Implements IAvatarModel.XPDisplay
        Get
            Return avatar.XPDisplay
        End Get
    End Property

    Public ReadOnly Property JoolsDisplay As String Implements IAvatarModel.JoolsDisplay
        Get
            Return avatar.JoolsDisplay
        End Get
    End Property

    Public ReadOnly Property CanCraft As Boolean Implements IAvatarModel.CanCraft
        Get
            Return CanBuildFire OrElse
                CanBuildFurnace OrElse
                CanCookBagel OrElse
                CanKnap OrElse
                CanMakeHatchet OrElse
                CanMakeTorch OrElse
                CanMakeTwine
        End Get
    End Property

    Public ReadOnly Property Inventory As IAvatarInventoryModel Implements IAvatarModel.Inventory
        Get
            Return New AvatarInventoryModel(avatar)
        End Get
    End Property

    Public Sub Move(delta As (x As Integer, y As Integer)) Implements IAvatarModel.Move
        avatar.Move(delta)
    End Sub

    Public Sub DoChoiceTrigger(index As Integer) Implements IAvatarModel.DoChoiceTrigger
        Dim choice = avatar.World.CurrentMessage.Choice(index)
        avatar.
            CharacterType.
            ToCharacterTypeDescriptor.
            EffectHandlers(choice.EffectType).
            Invoke(avatar, choice)
        avatar.World.DismissMessage()
    End Sub

    Public Sub MakeTwine() Implements IAvatarModel.MakeTwine
        avatar.DoMakeTwine()
    End Sub

    Public Sub Unequip(equipSlotType As String) Implements IAvatarModel.Unequip
        avatar.Unequip(equipSlotType)
        avatar.
            World.
            CreateMessage().
            AddLine(LightGray, $"{avatar.Name} unequips {equipSlotType.ToEquipSlotTypeDescriptor.Name}")
    End Sub

    Public Sub Sleep() Implements IAvatarModel.Sleep
        avatar.Sleep()
    End Sub

    Public Sub BuildFire() Implements IAvatarModel.BuildFire
        avatar.DoBuildFire()
    End Sub

    Public Sub PutOutFire() Implements IAvatarModel.PutOutFire
        avatar.DoPutOutFire()
    End Sub

    Public Sub MakeTorch() Implements IAvatarModel.MakeTorch
        avatar.DoMakeTorch()
    End Sub

    Public Sub MakeHatchet() Implements IAvatarModel.MakeHatchet
        avatar.DoMakeHatchet()
    End Sub

    Public Sub BuildFurnace() Implements IAvatarModel.BuildFurnace
        avatar.DoBuildFurnace()
    End Sub

    Public Sub Knap() Implements IAvatarModel.Knap
        avatar.DoKnap()
    End Sub

    Public Sub CookBagel() Implements IAvatarModel.CookBagel
        avatar.DoCookBagel()
    End Sub

    Public Function LegacyFormatItemCount(itemName As String) As String Implements IAvatarModel.LegacyFormatItemCount
        Return $"{itemName}(x{LegacyItemCount(itemName)})"
    End Function
End Class
