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
        Dim y = Context.ViewSize.Height \ 2 - font.Height * 8 \ 2
        Dim health = Model.Avatar.Health
        y = WriteLine(displayBuffer, font, y, $"Health: {health.current}/{health.maximum}", Pink)
        Dim energy = Model.Avatar.Energy
        y = WriteLine(displayBuffer, font, y, $"Energy: {energy.current}/{energy.maximum}", Blue)
        Dim attack = Model.Avatar.Attack
        y = WriteLine(displayBuffer, font, y, $"Attack: Max {attack.maximum} avg {attack.average:f2}", Red)
        Dim defend = Model.Avatar.Defend
        y = WriteLine(displayBuffer, font, y, $"Defend: Max {defend.maximum} avg {defend.average:f2}", Green)
        y = WriteLine(displayBuffer, font, y, $"XP: {Model.Avatar.XP}/{Model.Avatar.XPGoal}", Cyan)
        y = WriteLine(displayBuffer, font, y, $"Level: {Model.Avatar.XPLevel}", Purple)
        y = WriteLine(displayBuffer, font, y, $"AP: {Model.Avatar.AdvancementPoints}", Yellow)
        WriteLine(displayBuffer, font, y, $"Jools: {Model.Avatar.Jools}", Hue.LightGreen)
        Context.ShowHeader(displayBuffer, font, "Statistics", Black, Orange)
        Context.ShowStatusBar(displayBuffer, font, Context.ControlsText(Nothing, "Go Back"), Black, LightGray)
    End Sub
    Private Function WriteLine(displayBuffer As IPixelSink, font As Font, y As Integer, text As String, hue As Integer) As Integer
        font.WriteText(displayBuffer, (Context.ViewSize.Width \ 2 - font.TextWidth(text) \ 2, y), text, hue)
        Return y + font.Height
    End Function
End Class
