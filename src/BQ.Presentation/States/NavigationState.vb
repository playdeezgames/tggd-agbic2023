Friend Class NavigationState
    Inherits BaseGameState(Of IWorldModel)

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IWorldModel))
        MyBase.New(parent, setState, context)
    End Sub

    Public Overrides Sub HandleCommand(cmd As String)
        Select Case cmd
            Case Command.B
                SetState(GameMenu)
        End Select
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink)
        displayBuffer.Fill((0, 0), Context.ViewSize, Black)
        RenderMap(displayBuffer)
    End Sub
    Private Function Plot(column As Integer, row As Integer) As (Integer, Integer)
        Return (column * CellWidth + CenterCellX, row * CellHeight + CenterCellY)
    End Function
    Private Sub RenderMap(displayBuffer As IPixelSink)
        'Dim font = Context.Font(BagelQuestFont)
        For Each column In Enumerable.Range(LeftColumn, MapRenderColumns)
            For Each row In Enumerable.Range(TopRow, MapRenderRows)
                'displayBuffer.Fill(Plot(column, row), (CellWidth, CellHeight), Math.Abs(CInt(Math.Sqrt(row * row + column * column))) Mod 15)
            Next
        Next
    End Sub
End Class
