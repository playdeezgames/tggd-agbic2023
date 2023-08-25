﻿Public Interface IAvatarModel
    ReadOnly Property Inventory As IAvatarInventoryModel
    ReadOnly Property Equipment As IAvatarEquipmentModel
    ReadOnly Property Statistics As IAvatarStatisticsModel
    ReadOnly Property Crafting As IAvatarCraftingModel

    Sub Move(delta As (x As Integer, y As Integer))

    Sub DoChoiceTrigger(index As Integer)

    ReadOnly Property HasWon As Boolean
    ReadOnly Property IsDead As Boolean

    ReadOnly Property Character As (Glyph As Char, Hue As Integer, MaskGlyph As Char, MaskHue As Integer)
    ReadOnly Property Name As String

    ReadOnly Property CanSleep As Boolean
    Sub Sleep()

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
