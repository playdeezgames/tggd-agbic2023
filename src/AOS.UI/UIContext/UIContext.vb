Imports System.IO
Imports System.Text.Json
Public MustInherit Class UIContext(Of TModel)
    Implements IUIContext(Of TModel)
    Private ReadOnly fonts As New Dictionary(Of String, Font)
    ReadOnly Property ViewSize As (Integer, Integer) Implements IUIContext(Of TModel).ViewSize
    Public MustOverride ReadOnly Property AvailableWindowSizes As IEnumerable(Of (Integer, Integer)) Implements IUIContext(Of TModel).AvailableWindowSizes

    Public ReadOnly Property Model As TModel Implements IUIContext(Of TModel).Model

    Public ReadOnly Property ViewCenter As (X As Integer, Y As Integer) Implements IUIContext(Of TModel).ViewCenter
        Get
            Return (ViewSize.Item1 \ 2, ViewSize.Item2 \ 2)
        End Get
    End Property

    Sub New(game As TModel, fontFilenames As IReadOnlyDictionary(Of String, String), viewSize As (Integer, Integer))
        Me.Model = game
        Me.ViewSize = viewSize
        For Each entry In fontFilenames
            fonts(entry.Key) = New Font(JsonSerializer.Deserialize(Of FontData)(File.ReadAllText(entry.Value)))
        Next
    End Sub

    Public Sub ShowStatusBar(displayBuffer As IPixelSink, font As Font, text As String, foreground As Integer, background As Integer) Implements IUIContext(Of TModel).ShowStatusBar
        displayBuffer.Fill((0, ViewSize.Item2 - font.Height), (ViewSize.Item1, font.Height), background)
        font.WriteText(displayBuffer, (ViewSize.Item1 \ 2 - font.TextWidth(text) \ 2, ViewSize.Item2 - font.Height), text, foreground)
    End Sub

    Public Function Font(gameFont As String) As Font Implements IUIContext(Of TModel).Font
        Return fonts(gameFont)
    End Function

    Public MustOverride Sub ShowSplashContent(displayBuffer As IPixelSink, font As Font) Implements IUIContext(Of TModel).ShowSplashContent

    Public Sub ShowHeader(displayBuffer As IPixelSink, font As Font, text As String, foreground As Integer, background As Integer) Implements IUIContext(Of TModel).ShowHeader
        displayBuffer.Fill((0, 0), (ViewSize.Item1, font.Height), background)
        font.WriteText(displayBuffer, (ViewSize.Item1 \ 2 - font.TextWidth(text) \ 2, 0), text, foreground)
    End Sub

    Public Function ControlsText(aButtonText As String, bButtonText As String) As String Implements IUIContext(Of TModel).ControlsText
        Dim result As String = ""
        If Not String.IsNullOrEmpty(aButtonText) Then
            result = $"Space/(A) - {aButtonText}"
        End If
        If Not String.IsNullOrEmpty(bButtonText) Then
            If Not String.IsNullOrEmpty(result) Then
                result &= " | "
            End If
            result &= $"Esc/(B) - {bButtonText}"
        End If
        Return result
    End Function

    Public MustOverride Sub ShowAboutContent(displayBuffer As IPixelSink, font As Font) Implements IUIContext(Of TModel).ShowAboutContent
    Public MustOverride Sub AbandonGame() Implements IUIContext(Of TModel).AbandonGame
    Public MustOverride Sub LoadGame(slot As Integer) Implements IUIContext(Of TModel).LoadGame
    Public MustOverride Sub SaveGame(slot As Integer) Implements IUIContext(Of TModel).SaveGame
    Public MustOverride Function DoesSlotExist(slot As Integer) As Boolean Implements IUIContext(Of TModel).DoesSlotExist
End Class
