﻿Friend Class ForageState
    Inherits BaseGameState(Of IWorldModel)
    Private forageCells(,) As (glyph As Char, hue As Integer, itemType As String)
    Private revealedCells(,) As Boolean
    Private ReadOnly loot As New Dictionary(Of String, Integer)
    Private gridSize As (columns As Integer, rows As Integer)
    Private currentColumn As Integer
    Private currentRow As Integer

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IWorldModel))
        MyBase.New(parent, setState, context)
    End Sub

    Public Overrides Sub HandleCommand(cmd As String)
        Select Case cmd
            Case Command.B
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
        Dim itemType = forageCells(currentColumn, currentRow).itemType
        If Not revealedCells(currentColumn, currentRow) AndAlso itemType IsNot Nothing Then
            If Model.Foraging.LegacyForageItemType(itemType) Then
                If Not String.IsNullOrEmpty(itemType) Then
                    If loot.ContainsKey(itemType) Then
                        loot(itemType) += 1
                    Else
                        loot(itemType) = 1
                    End If
                End If
                revealedCells(currentColumn, currentRow) = True
            End If
        End If
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink)
        displayBuffer.Fill(Black)
        Dim bqFont = Context.Font(BagelQuestFont)
        Dim font = Context.Font(UIFont)
        Dim offsetY = Context.ViewSize.Height \ 2 - gridSize.rows * CellHeight \ 2
        Dim energy = Model.Avatar.Energy
        For Each row In Enumerable.Range(0, gridSize.rows)
            Dim offsetX = Context.ViewSize.Width \ 2 - gridSize.columns * CellWidth \ 2
            For Each column In Enumerable.Range(0, gridSize.columns)
                Dim cell = forageCells(column, row)
                If cell.itemType IsNot Nothing Then
                    If revealedCells(column, row) Then
                        bqFont.WriteText(displayBuffer, (offsetX, offsetY), cell.glyph, cell.hue)
                    Else
                        bqFont.WriteText(displayBuffer, (offsetX, offsetY), ChrW(254), Yellow)
                    End If
                End If
                If column = currentColumn AndAlso row = currentRow Then
                    bqFont.WriteText(displayBuffer, (offsetX, offsetY), ChrW(255), If(cell.itemType IsNot Nothing AndAlso energy.current > 0, Green, Red))
                End If
                offsetX += CellWidth
            Next
            offsetY += CellHeight
        Next
        offsetY = Context.ViewCenter.Y - font.HalfHeight * loot.Count
        For Each item In loot
            font.WriteText(displayBuffer, (0, offsetY), $"{item.Key}(x{item.Value})", White)
            offsetY += font.Height
        Next
        Context.ShowHeader(displayBuffer, Context.Font(UIFont), $"Energy {energy.current}/{energy.maximum}", Orange, Black)
        Context.ShowStatusBar(displayBuffer, Context.Font(UIFont), Context.ControlsText("Forage", "Exit"), Black, LightGray)
    End Sub

    Public Overrides Sub OnStart()
        MyBase.OnStart()
        loot.Clear()
        gridSize = Model.Foraging.GridSize
        forageCells = Model.Foraging.LegacyGenerateGrid()
        ReDim revealedCells(gridSize.columns, gridSize.rows)
        currentColumn = gridSize.columns \ 2
        currentRow = gridSize.rows \ 2
    End Sub
End Class
