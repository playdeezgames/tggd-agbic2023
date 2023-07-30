Friend Class MoveState
    Inherits BaseActionState
    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IWorldModel), delta As (Integer, Integer))
        MyBase.New(
            parent,
            setState,
            context,
            Sub(m)
                m.Avatar.Move(delta)
            End Sub,
            Neutral)
    End Sub
End Class
