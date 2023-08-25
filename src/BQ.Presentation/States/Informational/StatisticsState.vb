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
        displayBuffer.Fill(Black)
        Dim font = Context.Font(UIFont)
        Dim y = Context.ViewSize.Height \ 2 - font.Height * 8 \ 2
        y = WriteLine(displayBuffer, font, y, Model.Avatar.Statistics.Health, Pink)
        y = WriteLine(displayBuffer, font, y, Model.Avatar.Statistics.Energy, Blue)
        y = WriteLine(displayBuffer, font, y, Model.Avatar.Statistics.Attack, Red)
        y = WriteLine(displayBuffer, font, y, Model.Avatar.Statistics.Defend, Green)
        y = WriteLine(displayBuffer, font, y, Model.Avatar.Statistics.XP, Cyan)
        y = WriteLine(displayBuffer, font, y, Model.Avatar.Statistics.XPLevel, Purple)
        y = WriteLine(displayBuffer, font, y, Model.Avatar.Statistics.AdvancementPoints, Yellow)
        WriteLine(displayBuffer, font, y, Model.Avatar.Statistics.Jools, Hue.LightGreen)
        Context.ShowHeader(displayBuffer, font, "Statistics", Black, Orange)
        Context.ShowStatusBar(displayBuffer, font, Context.ControlsText(Nothing, "Go Back"), Black, LightGray)
    End Sub
    Private Function WriteLine(displayBuffer As IPixelSink, font As Font, y As Integer, text As String, hue As Integer) As Integer
        font.WriteText(displayBuffer, (Context.ViewSize.Width \ 2 - font.TextWidth(text) \ 2, y), text, hue)
        Return y + font.Height
    End Function
End Class
