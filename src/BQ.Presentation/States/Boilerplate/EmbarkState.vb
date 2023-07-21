Friend Class EmbarkState
    Inherits BaseGameState(Of IWorldModel)

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IWorldModel))
        MyBase.New(parent, setState, context)
    End Sub

    Public Overrides Sub HandleCommand(cmd As String)
        Throw New NotImplementedException()
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink)
        Throw New NotImplementedException()
    End Sub
    Public Overrides Sub OnStart()
        MyBase.OnStart()
        Model.Embark()
        SetState(Neutral)
    End Sub
End Class
