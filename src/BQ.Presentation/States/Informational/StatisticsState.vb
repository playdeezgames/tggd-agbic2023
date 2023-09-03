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
        displayBuffer.Fill(0)
        Dim font = Context.Font(UIFont)
        Dim y = Context.ViewSize.Height \ 2 - font.Height * 8 \ 2
        y = WriteLine(displayBuffer, font, y, Model.Avatar.Statistics.Health, 12)
        y = WriteLine(displayBuffer, font, y, Model.Avatar.Statistics.Energy, 1)
        y = WriteLine(displayBuffer, font, y, Model.Avatar.Statistics.Attack, 4)
        y = WriteLine(displayBuffer, font, y, Model.Avatar.Statistics.Defend, 2)
        y = WriteLine(displayBuffer, font, y, Model.Avatar.Statistics.XP, 3)
        y = WriteLine(displayBuffer, font, y, Model.Avatar.Statistics.XPLevel, 5)
        y = WriteLine(displayBuffer, font, y, Model.Avatar.Statistics.AdvancementPoints, 14)
        WriteLine(displayBuffer, font, y, Model.Avatar.Statistics.Jools, 10)
        Context.ShowHeader(displayBuffer, font, "Statistics", 0, 11)
        Context.ShowStatusBar(displayBuffer, font, Context.ControlsText(Nothing, "Go Back"), 0, 7)
    End Sub
    Private Function WriteLine(displayBuffer As IPixelSink, font As Font, y As Integer, text As String, hue As Integer) As Integer
        font.WriteText(displayBuffer, (Context.ViewSize.Width \ 2 - font.TextWidth(text) \ 2, y), text, hue)
        Return y + font.Height
    End Function
End Class
