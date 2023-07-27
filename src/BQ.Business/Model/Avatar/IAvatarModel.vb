﻿Public Interface IAvatarModel
    Sub Move(delta As (x As Integer, y As Integer))
    Sub DoChoiceTrigger(index As Integer)
    Sub Forage()
    Sub MakeTwine()
    ReadOnly Property IsDead As Boolean
    ReadOnly Property Character As (Glyph As Char, Hue As Integer, MaskGlyph As Char, MaskHue As Integer)
    ReadOnly Property Name As String
    ReadOnly Property Health As Integer
    ReadOnly Property MaximumHealth As Integer
    ReadOnly Property XP As Integer
    ReadOnly Property XPGoal As Integer
    ReadOnly Property XPLevel As Integer
    ReadOnly Property AdvancementPoints As Integer
    ReadOnly Property AverageAttack As Double
    ReadOnly Property AverageDefend As Double
    ReadOnly Property MaximumAttack As Integer
    ReadOnly Property MaximumDefend As Integer
    ReadOnly Property HasItems As Boolean
    ReadOnly Property Items As IEnumerable(Of (String, String))
    ReadOnly Property ItemCount(itemName As String) As Integer
    ReadOnly Property Jools As Integer
    ReadOnly Property CanForage As Boolean
    ReadOnly Property CanMakeTwine As Boolean
    ReadOnly Property HasEquipment As Boolean
    ReadOnly Property Equipment As IEnumerable(Of (String, String))
End Interface
