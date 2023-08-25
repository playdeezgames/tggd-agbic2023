Public Interface IAvatarModel
    ReadOnly Property Inventory As IAvatarInventoryModel
    ReadOnly Property Equipment As IAvatarEquipmentModel
    ReadOnly Property Statistics As IAvatarStatisticsModel
    ReadOnly Property Crafting As IAvatarCraftingModel
    Sub Move(delta As (x As Integer, y As Integer))
    Sub MakeChoice(index As Integer)
    ReadOnly Property HasWon As Boolean
    ReadOnly Property IsDead As Boolean
    ReadOnly Property Character As (Glyph As Char, Hue As Integer, MaskGlyph As Char, MaskHue As Integer)
    ReadOnly Property Name As String
    ReadOnly Property CanSleep As Boolean
    Sub Sleep()
End Interface
