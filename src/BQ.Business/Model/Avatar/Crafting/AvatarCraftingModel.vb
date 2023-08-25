Friend Class AvatarCraftingModel
    Implements IAvatarCraftingModel

    Private ReadOnly avatar As ICharacter

    Public Sub New(avatar As ICharacter)
        Me.avatar = avatar
    End Sub

    Public ReadOnly Property CanMakeTwine As Boolean Implements IAvatarCraftingModel.CanMakeTwine
        Get
            Return avatar.Flag(FlagTypes.KnowsTwineMaking)
        End Get
    End Property

    Public ReadOnly Property CanBuildFire As Boolean Implements IAvatarCraftingModel.CanBuildFire
        Get
            Return avatar.Flag(FlagTypes.KnowsFireMaking) AndAlso avatar.Cell.CanBuildFire
        End Get
    End Property

    Public ReadOnly Property CanPutOutFire As Boolean Implements IAvatarCraftingModel.CanPutOutFire
        Get
            Return avatar.Cell.HasFire
        End Get
    End Property

    Public ReadOnly Property CanMakeTorch As Boolean Implements IAvatarCraftingModel.CanMakeTorch
        Get
            Return avatar.Flag(FlagTypes.KnowsTorchMaking) AndAlso avatar.Cell.CanMakeTorch
        End Get
    End Property

    Public ReadOnly Property CanMakeHatchet As Boolean Implements IAvatarCraftingModel.CanMakeHatchet
        Get
            Return avatar.Flag(FlagTypes.KnowsHatchetMaking)
        End Get
    End Property

    Public ReadOnly Property CanKnap As Boolean Implements IAvatarCraftingModel.CanKnap
        Get
            Return avatar.Flag(FlagTypes.KnowsKnapping)
        End Get
    End Property

    Public ReadOnly Property CanBuildFurnace As Boolean Implements IAvatarCraftingModel.CanBuildFurnace
        Get
            Return avatar.CanBuildFurnace AndAlso avatar.Cell.CanBuildFurnace
        End Get
    End Property

    Public ReadOnly Property CanCookBagel As Boolean Implements IAvatarCraftingModel.CanCookBagel
        Get
            Return avatar.CanCookBagel AndAlso avatar.Cell.CanCookBagel
        End Get
    End Property

    Public ReadOnly Property CanCraft As Boolean Implements IAvatarCraftingModel.CanCraft
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

    Public Sub MakeTwine() Implements IAvatarCraftingModel.MakeTwine
        avatar.DoMakeTwine()
    End Sub

    Public Sub BuildFire() Implements IAvatarCraftingModel.BuildFire
        avatar.DoBuildFire()
    End Sub

    Public Sub PutOutFire() Implements IAvatarCraftingModel.PutOutFire
        avatar.DoPutOutFire()
    End Sub

    Public Sub MakeTorch() Implements IAvatarCraftingModel.MakeTorch
        avatar.DoMakeTorch()
    End Sub

    Public Sub MakeHatchet() Implements IAvatarCraftingModel.MakeHatchet
        avatar.DoMakeHatchet()
    End Sub

    Public Sub BuildFurnace() Implements IAvatarCraftingModel.BuildFurnace
        avatar.DoBuildFurnace()
    End Sub

    Public Sub Knap() Implements IAvatarCraftingModel.Knap
        avatar.DoKnap()
    End Sub

    Public Sub CookBagel() Implements IAvatarCraftingModel.CookBagel
        avatar.DoCookBagel()
    End Sub
End Class
