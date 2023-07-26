Public Interface IItemModel
    Property Name As String
    Property Count As Integer
    ReadOnly Property CanEquip As Boolean
    Sub Take()
    Sub Drop()
End Interface
