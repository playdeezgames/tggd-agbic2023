Friend Class ItemEffect
    Inherits BaseEffect
    Implements IItemEffect
    Sub New(effectType As String, effectData As BaseData, item As IItem)
        MyBase.New(effectType, effectData)
        Me.Item = item
    End Sub

    Public ReadOnly Property Item As IItem Implements IItemEffect.Item
End Class
