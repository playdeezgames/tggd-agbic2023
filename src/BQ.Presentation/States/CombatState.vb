Friend Class CombatState
    Inherits BaseGameState(Of IWorldModel)

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IWorldModel))
        MyBase.New(parent, setState, context)
    End Sub

    Public Overrides Sub HandleCommand(cmd As String)
        If cmd = Command.A Then
            Model.Enemy.Attack()
            SetState(Neutral)
        End If
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink)
        displayBuffer.Fill((0, 0), Context.ViewSize, Black)
        Dim font = Context.Font(BagelQuestFont)
        Dim enemies = Model.Enemy.Enemies
        Dim x = Context.ViewSize.Item1 \ 2 - font.TextWidth(ChrW(0)) * enemies.Count \ 2
        For Each enemy In enemies
            font.WriteText(displayBuffer, (x, 0), enemy.MaskGlyph, enemy.MaskHue)
            font.WriteText(displayBuffer, (x, 0), enemy.Glyph, enemy.Hue)
            x += font.TextWidth(ChrW(0))
        Next
    End Sub
End Class
