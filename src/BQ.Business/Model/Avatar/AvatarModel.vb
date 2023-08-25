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

    Public ReadOnly Property CanMakeTwine As Boolean Implements IAvatarModel.CanMakeTwine
        Get
            Return avatar.Flag(FlagTypes.KnowsTwineMaking)
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

    Public ReadOnly Property Equipment As IAvatarEquipmentModel Implements IAvatarModel.Equipment
        Get
            Return New AvatarEquipmentModel(avatar)
        End Get
    End Property

    Public ReadOnly Property Statistics As IAvatarStatisticsModel Implements IAvatarModel.Statistics
        Get
            Return New AvatarStatisticsModel(avatar)
        End Get
    End Property

    Public ReadOnly Property Crafting As IAvatarCraftingModel Implements IAvatarModel.Crafting
        Get
            Return New AvatarCraftingModel(avatar)
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
End Class
