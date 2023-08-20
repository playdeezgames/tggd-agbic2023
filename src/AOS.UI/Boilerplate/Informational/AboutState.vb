Friend Class AboutState(Of TModel)
    Inherits BaseGameState(Of TModel)
    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of TModel))
        MyBase.New(parent, setState, context)
    End Sub
    Public Overrides Sub HandleCommand(cmd As String)
        SetState(BoilerplateState.MainMenu)
    End Sub
    Public Overrides Sub Render(displayBuffer As IPixelSink)
        displayBuffer.Fill(BoilerplateHue.Black)
        Context.ShowAboutContent(displayBuffer, Context.Font(UIFont))
    End Sub
End Class
