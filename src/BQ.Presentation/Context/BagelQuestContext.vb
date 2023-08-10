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
            Return multipliers.Select(Function(x) (ViewWidth * x, ViewHeight * x))
        End Get
    End Property
    Private ReadOnly DeltasAndColor As IReadOnlyList(Of (Integer, Integer, Integer)) =
        New List(Of (Integer, Integer, Integer)) From
        {
            (-1, -1, Tan),
            (0, -1, Tan),
            (1, -1, Tan),
            (-1, 0, Tan),
            (1, 0, Tan),
            (-1, 1, Tan),
            (0, 1, Tan),
            (1, 1, Tan),
            (0, 0, Brown)
        }
    Public Overrides Sub ShowSplashContent(displayBuffer As IPixelSink, font As Font)
        Dim text = "Bagel Quest"
        Dim x = ViewWidth \ 2 - font.TextWidth(text) \ 2
        Dim y = ViewHeight \ 2 - font.Height \ 2
        With font
            For Each deltaAndColor In DeltasAndColor
                .WriteText(displayBuffer, (x + deltaAndColor.Item1, y + deltaAndColor.Item2), text, deltaAndColor.Item3)
            Next
        End With
        ShowStatusBar(displayBuffer, font, "Space/(A) - Continue", Hue.Black, Hue.LightGray)
    End Sub

    Public Overrides Sub ShowAboutContent(displayBuffer As IPixelSink, font As Font)
        With font
            .WriteText(displayBuffer, (0, 0), "About Bagel Quest", Hue.Orange)
            .WriteText(displayBuffer, (0, font.Height * 2), "Art:", Hue.White)
            .WriteText(displayBuffer, (0, font.Height * 3), "https://kenney.nl/assets/1-bit-pack", Hue.White)
            .WriteText(displayBuffer, (0, font.Height * 5), "A Production of TheGrumpyGameDev", Hue.White)
            .WriteText(displayBuffer, (0, font.Height * 7), "For A Game By Its Cover 2023", Hue.White)
            .WriteText(displayBuffer, (0, font.Height * 9), "See 'aboot.txt'", Hue.White)
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
