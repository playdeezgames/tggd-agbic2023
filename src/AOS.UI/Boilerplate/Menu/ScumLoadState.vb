Friend Class ScumLoadState(Of TModel)
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
        If Context.DoesSlotExist(0) Then
            Context.LoadGame(0)
            SetState(Neutral)
            Return
        End If
        SetState(MainMenu)
    End Sub

End Class
