Friend Class WinnerState
    Inherits BaseGameState(Of IWorldModel)

    Private showUntil As DateTimeOffset

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IWorldModel))
        MyBase.New(parent, setState, context)
    End Sub

    Public Overrides Sub HandleCommand(cmd As String)
        Select Case cmd
            Case Command.A, Command.B
                If DateTimeOffset.Now >= showUntil Then
                    Model.Abandon()
                    SetState(BoilerplateState.MainMenu)
                End If
        End Select
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink)
        displayBuffer.Fill((0, 0), Context.ViewSize, Black)
        Dim font = Context.Font(UIFont)
        Dim text = "You Win!"
        font.WriteText(displayBuffer, (Context.ViewSize.Width \ 2 - font.TextWidth(text) \ 2, Context.ViewSize.Height \ 2 - font.Height \ 2), text, LightGreen)
        If DateTimeOffset.Now >= showUntil Then
            Context.ShowStatusBar(displayBuffer, font, Context.ControlsText("Main Menu", Nothing), Black, LightGray)
        End If
    End Sub

    Public Overrides Sub OnStart()
        MyBase.OnStart()
        showUntil = DateTimeOffset.Now.AddSeconds(1)
        PlayMux(Mux.VictoryTheme)
    End Sub
End Class
