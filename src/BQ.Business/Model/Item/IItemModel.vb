Public Interface IItemModel
    Property Name As String
    Property Count As Integer
    ReadOnly Property CanEquip As Boolean
    Sub Take()
    Sub Drop()
    Sub Equip(itemId As Integer)
    ReadOnly Property Equippables As IEnumerable(Of (fullName As String, itemId As Integer))
    ReadOnly Property VerbTypes As IEnumerable(Of (text As String, verbType As String))
    ReadOnly Property EffectTypes As IEnumerable(Of (text As String, VerbTypes As String))
    Sub DoVerb(verbType As String)
End Interface
