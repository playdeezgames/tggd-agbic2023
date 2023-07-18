Public Interface IMessageModel
    ReadOnly Property Exists As Boolean
    ReadOnly Property Current As IMessage
    Sub Dismiss()
End Interface
