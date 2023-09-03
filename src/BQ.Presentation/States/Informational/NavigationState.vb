Friend Class NavigationState
    Inherits BaseGameState(Of IWorldModel)

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IWorldModel))
        MyBase.New(parent, setState, context)
    End Sub

    Private ReadOnly commandTable As IReadOnlyDictionary(Of String, String) =
        New Dictionary(Of String, String) From
        {
            {Command.A, ActionMenu},
            {Command.B, GameMenu},
            {Command.Up, MoveUp},
            {Command.Down, MoveDown},
            {Command.Left, MoveLeft},
            {Command.Right, MoveRight}
        }

    Public Overrides Sub HandleCommand(cmd As String)
        SetState(commandTable(cmd))
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink)
        displayBuffer.Fill(8)
        RenderMap(displayBuffer)
        RenderStatistics(displayBuffer)
        Context.ShowStatusBar(displayBuffer, Context.Font(UIFont), Context.ControlsText(ActionsText, GameMenuText), 0, 7)
    End Sub


    Private Sub RenderStatistics(displayBuffer As IPixelSink)
        Dim font = Context.Font(UIFont)
        Dim position = RenderStatistic(displayBuffer, font, (0, 0), Model.Avatar.Statistics.Health, 12)
        position = RenderStatistic(displayBuffer, font, position, Model.Avatar.Statistics.Energy, 1)
        position = RenderStatistic(displayBuffer, font, position, Model.Avatar.Statistics.XPLevel, 5)
        position = RenderStatistic(displayBuffer, font, position, Model.Avatar.Statistics.XP, 3)
        position = RenderStatistic(displayBuffer, font, position, $" {Model.Avatar.Statistics.Jools}", 10)
    End Sub

    Private Shared Function RenderStatistic(displayBuffer As IPixelSink, font As Font, position As (x As Integer, y As Integer), text As String, hue As Integer) As (Integer, Integer)
        font.WriteText(displayBuffer, (position.x + 1, position.y + 1), text, 0)
        font.WriteText(displayBuffer, (position.x, position.y), text, hue)
        Return (position.x, position.y + font.Height)
    End Function

    Private Function Plot(column As Integer, row As Integer) As (Integer, Integer)
        Return (column * CellWidth + CenterCellX, row * CellHeight + CenterCellY)
    End Function
    Private Sub RenderMap(displayBuffer As IPixelSink)
        Dim font = Context.Font(BagelQuestFont)
        For Each column In Enumerable.Range(LeftColumn, MapRenderColumns)
            For Each row In Enumerable.Range(TopRow, MapRenderRows)
                RenderCell(font, displayBuffer, (column, row), Plot(column, row))
            Next
        Next
    End Sub

    Private Sub RenderCell(font As Font, displayBuffer As IPixelSink, cellXY As (column As Integer, row As Integer), pixelXY As (Integer, Integer))
        If Not Model.Map.CellExists(cellXY) Then
            Return
        End If
        RenderTerrain(font, displayBuffer, cellXY, pixelXY)
        RenderItems(font, displayBuffer, pixelXY, Model.Map.Items(cellXY))
        RenderCharacter(font, displayBuffer, pixelXY, Model.Map.Character(cellXY))
    End Sub

    Private Shared Sub RenderItems(font As Font, displayBuffer As IPixelSink, pixelXY As (Integer, Integer), items As IEnumerable(Of (Glyph As Char, Hue As Integer)))
        For Each item In items
            font.WriteText(displayBuffer, pixelXY, item.Glyph, item.Hue)
        Next
    End Sub

    Private Shared Sub RenderCharacter(font As Font, displayBuffer As IPixelSink, pixelXY As (Integer, Integer), character As (Glyph As Char, Hue As Integer, MaskGlyph As Char, MaskHue As Integer)?)
        If character.HasValue Then
            font.WriteText(displayBuffer, pixelXY, character.Value.MaskGlyph, character.Value.MaskHue)
            font.WriteText(displayBuffer, pixelXY, character.Value.Glyph, character.Value.Hue)
        End If
    End Sub

    Private Sub RenderTerrain(font As Font, displayBuffer As IPixelSink, cellXY As (column As Integer, row As Integer), pixelXY As (Integer, Integer))
        displayBuffer.Fill(pixelXY, (CellWidth, CellHeight), 0)
        Dim terrain = Model.Map.Terrain(cellXY)
        font.WriteText(displayBuffer, pixelXY, terrain.Glyph, terrain.Hue)
    End Sub

    Public Overrides Sub OnStart()
        MyBase.OnStart()
        PlayMux(BoilerplateMux.MainTheme)
    End Sub
End Class
