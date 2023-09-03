Imports System.Runtime.CompilerServices
Imports System.Threading

Friend Class InventoryState
    Inherits BaseGameState(Of IWorldModel)
    Private gridSize As (columns As Integer, rows As Integer)
    Private items As List(Of (glyph As Char, hue As Integer, name As String, count As Integer))
    Private currentIndex As Integer

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IWorldModel))
        MyBase.New(parent, setState, context)
    End Sub

    Public Overrides Sub HandleCommand(cmd As String)
        Select Case cmd
            Case Command.B
                SetState(ActionMenu)
            Case Command.A
                Model.Item.Name = items(currentIndex).name
                SetState(GameState.InventoryDetail)
            Case Command.Up
                currentIndex -= gridSize.columns
                If currentIndex < 0 Then
                    currentIndex += items.Count
                End If
            Case Command.Down
                currentIndex += gridSize.columns
                If currentIndex >= items.Count Then
                    currentIndex -= items.Count
                End If
            Case Command.Left
                currentIndex = (currentIndex + items.Count - 1) Mod items.Count
            Case Command.Right
                currentIndex = (currentIndex + 1) Mod items.Count
        End Select
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink)
        displayBuffer.Fill(0)
        Dim bqFont = Context.Font(BagelQuestFont)
        Dim font = Context.Font(UIFont)
        Dim rowStride = bqFont.Height * 2
        Dim columnStride = bqFont.TextWidth(ChrW(0)) * 2
        Dim offsetY = Context.ViewCenter.Y - gridSize.rows * rowStride \ 2
        Dim index = 0
        For Each row In Enumerable.Range(0, gridSize.rows)
            Dim offsetX = Context.ViewCenter.X - gridSize.columns * columnStride \ 2
            For Each column In Enumerable.Range(0, gridSize.columns)
                If index < items.Count Then
                    Dim item = items(index)
                    bqFont.WriteText(displayBuffer, (offsetX + columnStride \ 4, offsetY + rowStride \ 4), item.glyph, item.hue)
                    Dim text = $"x{item.count}"
                    font.WriteText(displayBuffer, (offsetX + columnStride \ 2 - font.TextWidth(text) \ 2, offsetY + bqFont.Height + rowStride \ 4), text, 7)
                End If
                If index = currentIndex Then
                    bqFont.WriteText(displayBuffer, (offsetX + columnStride \ 4, offsetY + rowStride \ 4), ChrW(255), 15)
                End If
                index += 1
                offsetX += columnStride
            Next
            offsetY += rowStride
        Next
        Context.ShowHeader(displayBuffer, font, items(currentIndex).name, 11, 0)
        Context.ShowStatusBar(displayBuffer, font, Context.ControlsText(Constants.SelectText, Constants.CancelText), 0, 7)
    End Sub

    Public Overrides Sub OnStart()
        MyBase.OnStart()
        gridSize = Model.Avatar.Inventory.GridSize
        items = Model.Avatar.Inventory.Display.ToList
        currentIndex = items.Count \ 2
    End Sub
End Class
