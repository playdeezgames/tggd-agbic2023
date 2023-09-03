Imports System.Data.Common
Imports SPLORR.Game

Friend Class ForageState
    Inherits BaseGameState(Of IWorldModel)
    Private gridSize As (columns As Integer, rows As Integer)
    Private currentColumn As Integer
    Private currentRow As Integer
    Private ReadOnly loot As New List(Of IItem)
    Private hiddenCells(,) As Boolean
    Private items(,) As IItem

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IWorldModel))
        MyBase.New(parent, setState, context)
    End Sub

    Public Overrides Sub HandleCommand(cmd As String)
        Select Case cmd
            Case Command.B
                Model.Foraging.FinalReport(loot)
                SetState(Neutral)
            Case Command.Up
                currentRow = (currentRow + gridSize.rows - 1) Mod gridSize.rows
            Case Command.Down
                currentRow = (currentRow + 1) Mod gridSize.rows
            Case Command.Left
                currentColumn = (currentColumn + gridSize.columns - 1) Mod gridSize.columns
            Case Command.Right
                currentColumn = (currentColumn + 1) Mod gridSize.columns
            Case Command.A
                RevealCell()
        End Select
    End Sub

    Private Sub RevealCell()
        If hiddenCells(currentColumn, currentRow) AndAlso Model.Foraging.CanForage Then
            hiddenCells(currentColumn, currentRow) = False
            Dim item = Model.Foraging.Forage
            items(currentColumn, currentRow) = item
            If item IsNot Nothing Then
                loot.Add(item)
            End If
        End If
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink)
        displayBuffer.Fill(0)
        RenderGrid(displayBuffer)
        RenderLoot(displayBuffer)
        Context.ShowHeader(displayBuffer, Context.Font(UIFont), Model.Avatar.Statistics.Energy, 11, 0)
        Context.ShowStatusBar(displayBuffer, Context.Font(UIFont), Context.ControlsText("Forage", "Exit"), 0, 7)
    End Sub

    Private Sub RenderLoot(displayBuffer As IPixelSink)
        Dim bqFont = Context.Font(BagelQuestFont)
        Dim font = Context.Font(UIFont)
        Dim table = loot.GroupBy(Function(x) x.ItemType)
        Dim offsetY = Context.ViewCenter.Y - bqFont.HalfHeight * table.Count
        For Each entry In table
            Dim item = entry.First
            bqFont.WriteText(displayBuffer, (0, offsetY), ItemExtensions.Glyph(item), ItemExtensions.Hue(item))
            font.WriteText(displayBuffer, (bqFont.TextWidth(ChrW(0)), offsetY + bqFont.HalfHeight - font.HalfHeight), $"x{entry.Count}", 7)
            offsetY += bqFont.Height
        Next
    End Sub

    Private Sub RenderGrid(displayBuffer As IPixelSink)
        Dim bqFont = Context.Font(BagelQuestFont)
        Dim font = Context.Font(UIFont)
        Dim offsetY = Context.ViewCenter.Y - gridSize.rows * CellHeight \ 2
        For Each row In Enumerable.Range(0, gridSize.rows)
            Dim offsetX = Context.ViewSize.Width \ 2 - gridSize.columns * CellWidth \ 2
            For Each column In Enumerable.Range(0, gridSize.columns)
                If hiddenCells(column, row) Then
                    bqFont.WriteText(displayBuffer, (offsetX, offsetY), ChrW(254), 14)
                ElseIf items(column, row) IsNot Nothing Then
                    Dim item = items(column, row)
                    bqFont.WriteText(displayBuffer, (offsetX, offsetY), ItemExtensions.Glyph(item), ItemExtensions.Hue(item))
                End If
                If column = currentColumn AndAlso row = currentRow Then
                    bqFont.WriteText(displayBuffer, (offsetX, offsetY), ChrW(255), If(hiddenCells(column, row) AndAlso Model.Foraging.CanForage, 2, 4))
                End If
                offsetX += CellWidth
            Next
            offsetY += CellHeight
        Next
    End Sub

    Public Overrides Sub OnStart()
        MyBase.OnStart()
        InitializeGrid()
        PopulateGrid()
    End Sub

    Private Sub InitializeGrid()
        gridSize = Model.Foraging.GridSize
        currentColumn = gridSize.columns \ 2
        currentRow = gridSize.rows \ 2
        loot.Clear()
        ReDim hiddenCells(gridSize.columns, gridSize.rows)
        ReDim items(gridSize.columns, gridSize.rows)
        For Each column In Enumerable.Range(0, gridSize.columns)
            For Each row In Enumerable.Range(0, gridSize.rows)
                hiddenCells(column, row) = False
                items(column, row) = Nothing
            Next
        Next
    End Sub

    Private Sub PopulateGrid()
        Dim remaining = Model.Foraging.Remaining
        While remaining > 0
            Dim column = RNG.FromRange(0, gridSize.columns - 1)
            Dim row = RNG.FromRange(0, gridSize.rows - 1)
            If Not hiddenCells(column, row) Then
                hiddenCells(column, row) = True
                remaining -= 1
            End If
        End While
    End Sub
End Class
