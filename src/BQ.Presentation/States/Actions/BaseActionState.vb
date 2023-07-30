Friend Class BaseActionState
    Inherits BaseGameState(Of IWorldModel)

    Private ReadOnly theAction As Action(Of IWorldModel)
    Private ReadOnly nextState As String

    Friend Sub New(
                     parent As IGameController,
                     setState As Action(Of String, Boolean),
                     context As IUIContext(Of IWorldModel),
                     theAction As Action(Of IWorldModel),
                     nextState As String)
        MyBase.New(parent, setState, context)
        Me.theAction = theAction
        Me.nextState = nextState
    End Sub

    Public Overrides Sub HandleCommand(cmd As String)
        Throw New NotImplementedException("Action don't handle commands!")
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink)
        Throw New NotImplementedException("Actions don't render!")
    End Sub

    Public Overrides Sub OnStart()
        MyBase.OnStart()
        theAction(Model)
        SetState(nextState)
    End Sub
End Class
