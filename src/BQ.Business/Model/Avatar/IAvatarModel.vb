Public Interface IAvatarModel
    Sub Move(delta As (x As Integer, y As Integer))
    Sub DoChoiceTrigger(index As Integer)
    Sub Forage()
    Sub MakeTwine()
    Sub BuildFire()
    Sub Unequip(equipSlotType As String)
    Sub Sleep()
    Sub PutOutFire()
    Sub MakeTorch()
    Sub MakeHatchet()
    Sub BuildFurnace()
    Sub CookBagel()
    Sub Knap()
    ReadOnly Property IsDead As Boolean
    ReadOnly Property Character As (Glyph As Char, Hue As Integer, MaskGlyph As Char, MaskHue As Integer)
    ReadOnly Property Name As String
    ReadOnly Property Health As (current As Integer, maximum As Integer)
    ReadOnly Property Energy As (current As Integer, maximum As Integer)
    ReadOnly Property XP As Integer
    ReadOnly Property XPGoal As Integer
    ReadOnly Property XPLevel As Integer
    ReadOnly Property AdvancementPoints As Integer
    ReadOnly Property Attack As (average As Double, maximum As Integer)
    ReadOnly Property Defend As (average As Double, maximum As Integer)
    ReadOnly Property HasItems As Boolean
    ReadOnly Property Items As IEnumerable(Of (String, String))
    ReadOnly Property ItemCount(itemName As String) As Integer
    ReadOnly Property Jools As Integer
    ReadOnly Property CanForage As Boolean
    ReadOnly Property CanMakeTwine As Boolean
    ReadOnly Property HasEquipment As Boolean
    ReadOnly Property Equipment As IEnumerable(Of (String, String))
    ReadOnly Property CanSleep As Boolean
    ReadOnly Property CanBuildFire As Boolean
    ReadOnly Property CanPutOutFire As Boolean
    ReadOnly Property CanMakeTorch As Boolean
    ReadOnly Property CanBuildFurnace As Boolean
    ReadOnly Property CanMakeHatchet As Boolean
    ReadOnly Property CanKnap As Boolean
    ReadOnly Property CanCookBagel As Boolean
End Interface
