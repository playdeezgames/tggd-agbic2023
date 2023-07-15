Imports System.IO

Public Class BagelQuestContext
    Inherits UIContext(Of IWorldModel)

    Public Sub New(fontFilenames As IReadOnlyDictionary(Of String, String), viewSize As (Integer, Integer))
        MyBase.New(New WorldModel, fontFilenames, viewSize)
    End Sub

    Public Overrides ReadOnly Property AvailableWindowSizes As IEnumerable(Of (Integer, Integer))
        Get
            Return {
                (ViewWidth * 3, ViewHeight * 3),
                (ViewWidth * 4, ViewHeight * 4),
                (ViewWidth * 5, ViewHeight * 5),
                (ViewWidth * 6, ViewHeight * 6),
                (ViewWidth * 9, ViewHeight * 9),
                (ViewWidth * 12, ViewHeight * 12),
                (ViewWidth * 15, ViewHeight * 15),
                (ViewWidth * 18, ViewHeight * 18),
                (ViewWidth * 21, ViewHeight * 21),
                (ViewWidth * 24, ViewHeight * 24)
                }
        End Get
    End Property

    Public Overrides Sub ShowSplashContent(displayBuffer As IPixelSink, font As Font)
        Dim text = "Bagel Quest"
        Dim x = ViewWidth \ 2 - font.TextWidth(text) \ 2
        Dim y = ViewHeight \ 2 - font.Height \ 2
        With font
            .WriteText(displayBuffer, (x + 1, y - 1), text, Hue.Pink)
            .WriteText(displayBuffer, (x + 1, y), text, Hue.Pink)
            .WriteText(displayBuffer, (x + 1, y + 1), text, Hue.Pink)
            .WriteText(displayBuffer, (x - 1, y - 1), text, Hue.Pink)
            .WriteText(displayBuffer, (x - 1, y), text, Hue.Pink)
            .WriteText(displayBuffer, (x - 1, y + 1), text, Hue.Pink)
            .WriteText(displayBuffer, (x, y - 1), text, Hue.Pink)
            .WriteText(displayBuffer, (x, y + 1), text, Hue.Pink)

            .WriteText(displayBuffer, (x, y), text, Hue.Red)
        End With
        ShowStatusBar(displayBuffer, font, "Space/(A) - Continue", Hue.Black, Hue.LightGray)
    End Sub

    Public Overrides Sub ShowAboutContent(displayBuffer As IPixelSink, font As Font)
        With font
            .WriteText(displayBuffer, (0, 0), "About Bagel Quest", Hue.Orange)
            .WriteText(displayBuffer, (0, font.Height * 2), "Art:", Hue.White)
            .WriteText(displayBuffer, (0, font.Height * 3), "https://vurmux.itch.io/urizen-onebit-tileset", Hue.White)
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
            {1, "slot1.json"},
            {2, "slot2.json"},
            {3, "slot3.json"},
            {4, "slot4.json"},
            {5, "slot5.json"}
        }
End Class
