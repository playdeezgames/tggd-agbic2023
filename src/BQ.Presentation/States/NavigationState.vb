Friend Class NavigationState
    Inherits BaseGameState(Of IWorldModel)

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IWorldModel))
        MyBase.New(parent, setState, context)
    End Sub

    Public Overrides Sub HandleCommand(cmd As String)
        Select Case cmd
            Case Command.B
                SetState(GameMenu)
            Case Command.Up
                SetState(GameState.MoveUp)
            Case Command.Down
                SetState(GameState.MoveDown)
            Case Command.Left
                SetState(GameState.MoveLeft)
            Case Command.Right
                SetState(GameState.MoveRight)
        End Select
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink)
        displayBuffer.Fill((0, 0), Context.ViewSize, DarkGray)
        RenderMap(displayBuffer)
    End Sub
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
        Dim character = Model.Map.Character(cellXY)
        If character.HasValue Then
            font.WriteText(displayBuffer, pixelXY, character.Value.MaskGlyph, character.Value.MaskHue)
            font.WriteText(displayBuffer, pixelXY, character.Value.Glyph, character.Value.Hue)
        End If
    End Sub

    Private Sub RenderTerrain(font As Font, displayBuffer As IPixelSink, cellXY As (column As Integer, row As Integer), pixelXY As (Integer, Integer))
        displayBuffer.Fill(pixelXY, (CellWidth, CellHeight), Black)
        Dim terrain = Model.Map.Terrain(cellXY)
        font.WriteText(displayBuffer, pixelXY, terrain.Glyph, terrain.Hue)
    End Sub
End Class
