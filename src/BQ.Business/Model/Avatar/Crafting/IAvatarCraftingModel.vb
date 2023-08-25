Public Interface IAvatarCraftingModel
    ReadOnly Property CanCraft As Boolean
    ReadOnly Property CanMakeTwine As Boolean
    Sub MakeTwine()
    ReadOnly Property CanBuildFire As Boolean
    Sub BuildFire()
    ReadOnly Property CanPutOutFire As Boolean
    Sub PutOutFire()
    ReadOnly Property CanMakeTorch As Boolean
    Sub MakeTorch()
    ReadOnly Property CanBuildFurnace As Boolean
    Sub BuildFurnace()
    ReadOnly Property CanMakeHatchet As Boolean
    Sub MakeHatchet()
    ReadOnly Property CanKnap As Boolean
    Sub Knap()
    ReadOnly Property CanCookBagel As Boolean
    Sub CookBagel()
End Interface
