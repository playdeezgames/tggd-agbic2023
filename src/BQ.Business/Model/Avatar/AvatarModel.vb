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

    Public ReadOnly Property CanSleep As Boolean Implements IAvatarModel.CanSleep
        Get
            Return avatar.Energy < avatar.MaximumEnergy
        End Get
    End Property

    Public ReadOnly Property HasWon As Boolean Implements IAvatarModel.HasWon
        Get
            Return avatar.HasWon
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

    Public Sub MakeChoice(index As Integer) Implements IAvatarModel.MakeChoice
        Dim choice = avatar.World.CurrentMessage.Choice(index)
        avatar.
            CharacterType.
            ToCharacterTypeDescriptor.
            EffectHandlers(choice.EffectType).
            Invoke(avatar, choice)
        avatar.World.DismissMessage()
    End Sub

    Public Sub Sleep() Implements IAvatarModel.Sleep
        avatar.Sleep()
    End Sub
End Class
