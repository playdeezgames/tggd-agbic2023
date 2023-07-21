Friend Class CombatState
    Inherits BaseGameState(Of IWorldModel)
    Private CharacterIndex As Integer
    Private MenuItems As New List(Of (DisplayText As String, CommandText As String))
    Private MenuItemIndex As Integer

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IWorldModel))
        MyBase.New(parent, setState, context)
    End Sub

    Public Overrides Sub HandleCommand(cmd As String)
        Select Case cmd
            Case Command.Up
                PreviousMenuItem()
            Case Command.Down
                NextMenuItem()
            Case Command.Left
                PreviousEnemy()
            Case Command.Right
                NextEnemy()
            Case Command.A
                ExecuteMenuItem()
        End Select
    End Sub

    Private Sub ExecuteMenuItem()
        Select Case MenuItems(MenuItemIndex).CommandText
            Case RunText
                SetState(GameState.Run)
            Case AttackText
        End Select
    End Sub

    Private Sub NextEnemy()
        CharacterIndex = (CharacterIndex + 1) Mod Model.Combat.Count
    End Sub

    Private Sub PreviousEnemy()
        CharacterIndex = (CharacterIndex + Model.Combat.Count - 1) Mod Model.Combat.Count
    End Sub

    Private Sub NextMenuItem()
        MenuItemIndex = (MenuItemIndex + MenuItems.Count - 1) Mod MenuItems.Count
    End Sub

    Private Sub PreviousMenuItem()
        MenuItemIndex = (MenuItemIndex + 1) Mod MenuItems.Count
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink)
        displayBuffer.Fill((0, 0), Context.ViewSize, Black)
        DrawEnemies(displayBuffer)
        DrawEnemyStats(displayBuffer)
        DrawMenuItems(displayBuffer)
    End Sub

    Private Sub DrawMenuItems(displayBuffer As IPixelSink)
        Dim font = Context.Font(UIFont)
        Dim y = Context.ViewSize.Height * 3 \ 4 - font.Height * 2 \ 2
        displayBuffer.Fill((0, y), (Context.ViewSize.Width, font.Height), Blue)
        y -= MenuItemIndex * font.Height
        For Each index In Enumerable.Range(0, MenuItems.Count)
            font.WriteText(displayBuffer, (Context.ViewSize.Width \ 2 - font.TextWidth(MenuItems(index).DisplayText) \ 2, y), MenuItems(index).DisplayText, If(index = MenuItemIndex, Black, Blue))
            y += font.Height
        Next
    End Sub

    Private Sub DrawEnemyStats(displayBuffer As IPixelSink)
        Dim font = Context.Font(UIFont)
        Dim y = Context.ViewSize.Height \ 6 - font.Height * 2 \ 2
        Dim enemy = Model.Combat.Enemy(CharacterIndex)
        font.WriteText(displayBuffer, (Context.ViewSize.Width \ 2 - font.TextWidth(enemy.Name) \ 2, y), enemy.Name, LightGray)
        y += font.Height
        Dim text = $"{enemy.Health}/{enemy.MaximumHealth}"
        font.WriteText(displayBuffer, (Context.ViewSize.Width \ 2 - font.TextWidth(text) \ 2, y), text, LightGray)
    End Sub

    Private Sub DrawEnemies(displayBuffer As IPixelSink)
        Dim font = Context.Font(BagelQuestFont)
        Dim enemies = Model.Combat.Enemies
        Dim x = Context.ViewSize.Width \ 2 - font.TextWidth(ChrW(0)) * enemies.Count \ 2
        Dim y = Context.ViewSize.Height \ 3 - font.Height * 2 \ 2
        Dim glyphWidth = font.TextWidth(ChrW(0))
        font.WriteText(displayBuffer, (x + glyphWidth * CharacterIndex, y + font.Height), ChrW(&H1C), White)
        For Each enemy In enemies
            font.WriteText(displayBuffer, (x, y), enemy.MaskGlyph, enemy.MaskHue)
            font.WriteText(displayBuffer, (x, y), enemy.Glyph, enemy.Hue)
            x += glyphWidth
        Next
    End Sub
    Public Overrides Sub OnStart()
        MyBase.OnStart()
        If Not Model.Combat.Exists Then
            SetState(Neutral)
            Return
        End If
        If CharacterIndex >= Model.Combat.Count Then
            CharacterIndex = 0
        End If
        RefreshMenuItems()
        PlayMux(Mux.CombatTheme)
    End Sub

    Private Sub RefreshMenuItems()
        MenuItems.Clear()
        MenuItems.Add((AttackText, AttackText))
        MenuItems.Add((RunText, RunText))
    End Sub
End Class
