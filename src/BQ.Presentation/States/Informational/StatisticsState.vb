Friend Class StatisticsState
    Inherits BaseGameState(Of IWorldModel)

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IWorldModel))
        MyBase.New(parent, setState, context)
    End Sub

    Public Overrides Sub HandleCommand(cmd As String)
        Select Case cmd
            Case Command.A, Command.B
                SetState(GameState.ActionMenu)
        End Select
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink)
        displayBuffer.Fill((0, 0), Context.ViewSize, Black)
        Dim font = Context.Font(UIFont)
        Dim y = Context.ViewSize.Height \ 2 - font.Height * 5 \ 2
        y = WriteLine(displayBuffer, font, y, $"Health: {Model.Avatar.Health}/{Model.Avatar.MaximumHealth}", Pink)
        y = WriteLine(displayBuffer, font, y, $"Attack: Max {Model.Avatar.MaximumAttack} avg {Model.Avatar.AverageAttack:f2}", Red)
        y = WriteLine(displayBuffer, font, y, $"Defend: Max {Model.Avatar.MaximumDefend} avg {Model.Avatar.AverageDefend:f2}", Green)
        y = WriteLine(displayBuffer, font, y, $"XP: {Model.Avatar.XP}/{Model.Avatar.XPGoal}", Cyan)
        WriteLine(displayBuffer, font, y, $"Level: {Model.Avatar.XPLevel}", Blue)
        Context.ShowHeader(displayBuffer, font, "Statistics", Black, Orange)
        Context.ShowStatusBar(displayBuffer, font, Context.ControlsText(Nothing, "Go Back"), Black, LightGray)
    End Sub
    Private Function WriteLine(displayBuffer As IPixelSink, font As Font, y As Integer, text As String, hue As Integer) As Integer
        font.WriteText(displayBuffer, (Context.ViewSize.Width \ 2 - font.TextWidth(text) \ 2, y), text, hue)
        Return y + font.Height
    End Function
End Class
