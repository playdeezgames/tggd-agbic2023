Friend Class AvatarCraftingModel
    Implements IAvatarCraftingModel

    Private ReadOnly avatar As ICharacter

    Public Sub New(avatar As ICharacter)
        Me.avatar = avatar
    End Sub

    Public ReadOnly Property CanMakeTwine As Boolean Implements IAvatarCraftingModel.CanMakeTwine
        Get
            Return avatar.GetFlag("KnowsTwineMaking")
        End Get
    End Property

    Public ReadOnly Property CanBuildFire As Boolean Implements IAvatarCraftingModel.CanBuildFire
        Get
            Return avatar.GetFlag("KnowsFireMaking") AndAlso CellExtensions.CanBuildFire(avatar.Cell)
        End Get
    End Property

    Public ReadOnly Property CanPutOutFire As Boolean Implements IAvatarCraftingModel.CanPutOutFire
        Get
            Return CellExtensions.HasFire(avatar.Cell)
        End Get
    End Property

    Public ReadOnly Property CanMakeTorch As Boolean Implements IAvatarCraftingModel.CanMakeTorch
        Get
            Return avatar.GetFlag("KnowsTorchMaking") AndAlso CellExtensions.CanMakeTorch(avatar.Cell)
        End Get
    End Property

    Public ReadOnly Property CanMakeHatchet As Boolean Implements IAvatarCraftingModel.CanMakeHatchet
        Get
            Return avatar.GetFlag("KnowsHatchetMaking")
        End Get
    End Property

    Public ReadOnly Property CanKnap As Boolean Implements IAvatarCraftingModel.CanKnap
        Get
            Return avatar.GetFlag("KnowsRockSharpening")
        End Get
    End Property

    Public ReadOnly Property CanBuildFurnace As Boolean Implements IAvatarCraftingModel.CanBuildFurnace
        Get
            Return CharacterExtensions.CanBuildFurnace(avatar) AndAlso CellExtensions.CanBuildFurnace(avatar.Cell)
        End Get
    End Property

    Public ReadOnly Property CanCookBagel As Boolean Implements IAvatarCraftingModel.CanCookBagel
        Get
            Return CharacterExtensions.CanCookBagel(avatar) AndAlso CellExtensions.CanCookBagel(avatar.Cell)
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
        CharacterExtensions.DoMakeTwine(avatar)
    End Sub

    Public Sub BuildFire() Implements IAvatarCraftingModel.BuildFire
        CharacterExtensions.DoBuildFire(avatar)
    End Sub

    Public Sub PutOutFire() Implements IAvatarCraftingModel.PutOutFire
        CharacterExtensions.DoPutOutFire(avatar)
    End Sub

    Public Sub MakeTorch() Implements IAvatarCraftingModel.MakeTorch
        CharacterExtensions.DoMakeTorch(avatar)
    End Sub

    Public Sub MakeHatchet() Implements IAvatarCraftingModel.MakeHatchet
        CharacterExtensions.DoMakeHatchet(avatar)
    End Sub

    Public Sub BuildFurnace() Implements IAvatarCraftingModel.BuildFurnace
        CharacterExtensions.DoBuildFurnace(avatar)
    End Sub

    Public Sub Knap() Implements IAvatarCraftingModel.Knap
        CharacterExtensions.DoKnap(avatar)
    End Sub

    Public Sub CookBagel() Implements IAvatarCraftingModel.CookBagel
        CharacterExtensions.DoCookBagel(avatar)
    End Sub
End Class
