Public Interface IItemModel
    Property Name As String
    Property Count As Integer
    ReadOnly Property CanEquip As Boolean
    Sub Take()
    Sub Drop()
    Sub Equip(itemId As Integer)
    ReadOnly Property Equippables As IEnumerable(Of (fullName As String, itemId As Integer))
End Interface
