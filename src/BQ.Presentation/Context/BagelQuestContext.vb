Imports System.IO

Public Class BagelQuestContext
    Inherits UIContext(Of IWorldModel)

    Public Sub New(fontFilenames As IReadOnlyDictionary(Of String, String), viewSize As (Integer, Integer))
        MyBase.New(New WorldModel, fontFilenames, viewSize)
    End Sub
    Private ReadOnly multipliers As IReadOnlyList(Of Integer) =
        New List(Of Integer) From
        {
            3, 4, 5, 9, 10, 14, 15, 19, 20
        }

    Public Overrides ReadOnly Property AvailableWindowSizes As IEnumerable(Of (Integer, Integer))
        Get
            Return multipliers.Select(Function(multiplier) (ViewWidth * multiplier, ViewHeight * multiplier))
        End Get
    End Property
    Private ReadOnly DeltasAndColor As IReadOnlyList(Of (deltaX As Integer, deltaY As Integer, hue As Integer)) =
        New List(Of (Integer, Integer, Integer)) From
        {
            (-1, -1, 13),
            (0, -1, 13),
            (1, -1, 13),
            (-1, 0, 13),
            (1, 0, 13),
            (-1, 1, 13),
            (0, 1, 13),
            (1, 1, 13),
            (0, 0, 6)
        }
    Public Overrides Sub ShowSplashContent(displayBuffer As IPixelSink, font As Font)
        Dim text = GameTitle
        Dim x = ViewWidth \ 2 - font.TextWidth(text) \ 2
        Dim y = ViewHeight \ 2 - font.Height \ 2
        With font
            For Each deltaAndColor In DeltasAndColor
                .WriteText(
                    displayBuffer,
                    (x + deltaAndColor.deltaX, y + deltaAndColor.deltaY),
                    text,
                    deltaAndColor.hue)
            Next
        End With
        ShowStatusBar(displayBuffer, font, ControlsText(ContinueText, Nothing), 0, 7)
    End Sub

    Private ReadOnly aboutLines As IDictionary(Of Integer, (String, Integer)) =
        New Dictionary(Of Integer, (String, Integer)) From
        {
            {0, ("About Bagel Quest", 11)},
            {2, ("Art:", 15)},
            {3, ("https://kenney.nl/assets/1-bit-pack", 15)},
            {5, ("A Production of TheGrumpyGameDev", 15)},
            {7, ("For A Game By Its Cover 2023", 15)},
            {9, ("See 'aboot.txt'", 15)}
        }

    Public Overrides Sub ShowAboutContent(displayBuffer As IPixelSink, font As Font)
        With font
            For Each aboutLine In aboutLines
                .WriteText(displayBuffer, (0, font.Height * aboutLine.Key), aboutLine.Value.Item1, aboutLine.Value.Item2)
            Next
        End With
    End Sub

    Public Overrides Sub AbandonGame()
        Model.Abandon()
    End Sub

    Public Overrides Sub LoadGame(slot As Integer)
        Model.Load(SlotFilename(slot))
    End Sub

    Public Overrides Sub SaveGame(slot As Integer)
        Model.Save(SlotFilename(slot))
    End Sub

    Public Overrides Function DoesSlotExist(slot As Integer) As Boolean
        Return File.Exists(SlotFilename(slot))
    End Function

    Private ReadOnly SlotFilename As IReadOnlyDictionary(Of Integer, String) =
        New Dictionary(Of Integer, String) From
        {
            {0, "scum.json"},
            {1, "slot1.json"},
            {2, "slot2.json"},
            {3, "slot3.json"},
            {4, "slot4.json"},
            {5, "slot5.json"}
        }
End Class
