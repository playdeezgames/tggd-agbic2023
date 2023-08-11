Friend Class ScumSaveState(Of TModel)
    Inherits BaseGameState(Of TModel)

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of TModel))
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
        Context.SaveGame(0)
        SetState(BoilerplateState.Neutral)
    End Sub
End Class
