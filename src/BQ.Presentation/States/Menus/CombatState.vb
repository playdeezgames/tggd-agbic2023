Friend Class CombatState
    Inherits BaseGameState(Of IWorldModel)
    Private CharacterIndex As Integer
    Private ReadOnly MenuItems As New List(Of (DisplayText As String, CommandText As String))
    Private MenuItemIndex As Integer

    Public Sub New(
                  parent As IGameController,
                  setState As Action(Of String, Boolean),
                  context As IUIContext(Of IWorldModel))
        MyBase.New(parent, setState, context)
    End Sub

    Private ReadOnly commandTable As IReadOnlyDictionary(Of String, Action) =
        New Dictionary(Of String, Action) From
        {
            {Command.Up, AddressOf PreviousMenuItem},
            {Command.Down, AddressOf NextMenuItem},
            {Command.Left, AddressOf PreviousEnemy},
            {Command.Right, AddressOf NextEnemy},
            {Command.A, AddressOf ExecuteMenuItem},
            {Command.B, Sub()
                        End Sub}
        }

    Public Overrides Sub HandleCommand(cmd As String)
        commandTable(cmd).Invoke()
    End Sub

    Private Sub ExecuteMenuItem()
        Select Case MenuItems(MenuItemIndex).CommandText
            Case RunText
                SetState(GameState.Run)
            Case AttackText
                Model.Combat.Attack(CharacterIndex)
                SetState(Neutral)
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
        displayBuffer.Fill(0)
        DrawAvatar(displayBuffer)
        DrawAvatarStats(displayBuffer)
        DrawEnemies(displayBuffer)
        DrawEnemyStats(displayBuffer)
        DrawMenuItems(displayBuffer)
    End Sub

    Private Sub DrawAvatarStats(displayBuffer As IPixelSink)
        Dim font = Context.Font(UIFont)
        Dim centerX = OneThirdWidth()
        Dim y = CenterText(displayBuffer, font, centerX, OneSixthHeight() - font.HalfHeight * 3, Model.Avatar.Name, 7)
        y = CenterText(displayBuffer, font, centerX, y, Model.Avatar.Statistics.Health, 12)
        CenterText(displayBuffer, font, centerX, y, Model.Avatar.Statistics.Energy, 1)
    End Sub

    Private Function OneThirdWidth() As Integer
        Return Context.ViewSize.Width * 1 \ 3
    End Function

    Private Shared Function CenterText(displayBuffer As IPixelSink, font As Font, centerX As Integer, y As Integer, text As String, hue As Integer) As Integer
        font.WriteText(displayBuffer, (centerX - font.HalfTextWidth(text), y), text, hue)
        Return y + font.Height
    End Function

    Private Sub DrawAvatar(displayBuffer As IPixelSink)
        Dim font = Context.Font(BagelQuestFont)
        DrawCharacter(
            displayBuffer,
            font,
            OneThirdWidth() - font.HalfTextWidth(ChrW(0)),
            OneThirdHeight() - font.HalfHeight,
            font.TextWidth(ChrW(0)),
            Model.Avatar.Character)
    End Sub

    Private Sub DrawMenuItems(displayBuffer As IPixelSink)
        Dim font = Context.Font(UIFont)
        Dim y As Integer = DrawItemHilite(displayBuffer, font)
        For Each index In Enumerable.Range(0, MenuItems.Count)
            y = CenterText(
                displayBuffer,
                font,
                Context.ViewCenter.X,
                y,
                MenuItems(index).DisplayText,
                If(index = MenuItemIndex, 0, 1))
        Next
    End Sub

    Private Function DrawItemHilite(displayBuffer As IPixelSink, font As Font) As Integer
        Dim y = Context.ViewSize.Height * 3 \ 4 - font.HalfHeight * 2
        displayBuffer.Fill((0, y), (Context.ViewSize.Width, font.Height), 1)
        y -= MenuItemIndex * font.Height
        Return y
    End Function

    Private Sub DrawEnemyStats(displayBuffer As IPixelSink)
        Dim font = Context.Font(UIFont)
        Dim centerX = TwoThirdsWidth()
        Dim y = OneSixthHeight() - font.HalfHeight * 2
        Dim enemy = Model.Combat.Enemy(CharacterIndex)
        y = CenterText(displayBuffer, font, centerX, y, enemy.Name, 7)
        CenterText(displayBuffer, font, centerX, y, enemy.HealthDisplay, 12)
    End Sub

    Private Function OneSixthHeight() As Integer
        Return Context.ViewSize.Height \ 6
    End Function

    Private Sub DrawEnemies(displayBuffer As IPixelSink)
        Dim font = Context.Font(BagelQuestFont)
        Dim enemies = Model.Combat.Enemies
        Dim centerX = TwoThirdsWidth()
        Dim x = centerX - font.HalfTextWidth(ChrW(0)) * enemies.Count
        Dim y = OneThirdHeight() - font.HalfHeight * 2
        Dim glyphWidth = font.TextWidth(ChrW(0))
        DrawEnemyPointer(displayBuffer, font, x, y, glyphWidth)
        For Each enemy In enemies
            x = DrawCharacter(displayBuffer, font, x, y, glyphWidth, enemy)
        Next
    End Sub

    Private Function OneThirdHeight() As Integer
        Return Context.ViewSize.Height \ 3
    End Function

    Private Sub DrawEnemyPointer(displayBuffer As IPixelSink, font As Font, x As Integer, y As Integer, glyphWidth As Integer)
        font.WriteText(displayBuffer, (x + glyphWidth * CharacterIndex, y + font.Height), ChrW(&H1C), 15)
    End Sub

    Private Shared Function DrawCharacter(displayBuffer As IPixelSink, font As Font, x As Integer, y As Integer, glyphWidth As Integer, character As (Glyph As Char, Hue As Integer, MaskGlyph As Char, MaskHue As Integer)) As Integer
        font.WriteText(displayBuffer, (x, y), character.MaskGlyph, character.MaskHue)
        font.WriteText(displayBuffer, (x, y), character.Glyph, character.Hue)
        Return x + glyphWidth
    End Function

    Private Function TwoThirdsWidth() As Integer
        Return Context.ViewSize.Width * 2 \ 3
    End Function

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
        MenuItemIndex = 0
        MenuItems.Clear()
        MenuItems.Add((AttackText, AttackText))
        MenuItems.Add((RunText, RunText))
    End Sub
End Class
