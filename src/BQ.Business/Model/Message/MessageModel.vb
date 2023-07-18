Friend Class MessageModel
    Implements IMessageModel

    Private world As IWorld

    Public Sub New(world As IWorld)
        Me.world = world
    End Sub

    Public ReadOnly Property Exists As Boolean Implements IMessageModel.Exists
        Get
            Return world.HasMessages
        End Get
    End Property

    Public ReadOnly Property Current As IMessage Implements IMessageModel.Current
        Get
            Return world.CurrentMessage
        End Get
    End Property

    Public Sub Dismiss() Implements IMessageModel.Dismiss
        world.DismissMessage()
    End Sub
End Class
