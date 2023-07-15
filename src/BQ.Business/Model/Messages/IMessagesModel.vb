Imports BQ.Persistence

Public Interface IMessagesModel
    ReadOnly Property HasAny As Boolean
    ReadOnly Property Current As IMessage
    Sub Dismiss()
End Interface
