Friend Class WinnerState
    Inherits BaseGameState(Of IWorldModel)

    Private Const DelayInSeconds As Integer = 1
    Private showUntil As DateTimeOffset

    Public Sub New(
                  parent As IGameController,
                  setState As Action(Of String, Boolean),
                  context As IUIContext(Of IWorldModel))
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
        displayBuffer.Fill(0)
        Dim font = Context.Font(UIFont)
        Dim text = YouWinText
        font.WriteText(
            displayBuffer,
            (Context.ViewCenter.X - font.HalfTextWidth(text), Context.ViewCenter.Y - font.HalfHeight),
            text,
            10)
        If DateTimeOffset.Now >= showUntil Then
            Context.ShowStatusBar(
                displayBuffer,
                font,
                Context.ControlsText(MainMenuText, Nothing),
                0,
                7)
        End If
    End Sub

    Public Overrides Sub OnStart()
        MyBase.OnStart()
        showUntil = DateTimeOffset.Now.AddSeconds(DelayInSeconds)
        PlayMux(Mux.VictoryTheme)
    End Sub
End Class
