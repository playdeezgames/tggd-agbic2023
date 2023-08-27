Friend Class AvatarModel
    Implements IAvatarModel

    Private ReadOnly avatar As ICharacter

    Public Sub New(avatar As ICharacter)
        Me.avatar = avatar
    End Sub

    Public ReadOnly Property IsDead As Boolean Implements IAvatarModel.IsDead
        Get
            Return CharacterExtensions.IsDead(avatar)
        End Get
    End Property

    Public ReadOnly Property Character As (Glyph As Char, Hue As Integer, MaskGlyph As Char, MaskHue As Integer) Implements IAvatarModel.Character
        Get
            Dim descriptor = CharacterExtensions.Descriptor(avatar)
            Return (descriptor.Glyph, descriptor.Hue, descriptor.MaskGlyph, descriptor.MaskHue)
        End Get
    End Property

    Public ReadOnly Property Name As String Implements IAvatarModel.Name
        Get
            Return CharacterExtensions.Name(avatar)
        End Get
    End Property

    Public ReadOnly Property CanSleep As Boolean Implements IAvatarModel.CanSleep
        Get
            Return CharacterExtensions.Energy(avatar) < CharacterExtensions.MaximumEnergy(avatar)
        End Get
    End Property

    Public ReadOnly Property HasWon As Boolean Implements IAvatarModel.HasWon
        Get
            Return CharacterExtensions.HasWon(avatar)
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
        CharacterExtensions.Move(avatar, delta)
    End Sub

    Public Sub MakeChoice(index As Integer) Implements IAvatarModel.MakeChoice
        Dim choice = avatar.World.CurrentMessage.Choice(index)
        CharacterExtensions.
            Descriptor(avatar).
            RunEffectScript(WorldModel.LuaState, choice.EffectType, avatar, choice)
        avatar.World.DismissMessage()
    End Sub

    Public Sub Sleep() Implements IAvatarModel.Sleep
        CharacterExtensions.Sleep(avatar)
    End Sub
End Class
