Imports System.ComponentModel

Friend Class MessageState
    Inherits BaseGameState(Of IWorldModel)
    Private ChoiceIndex As Integer
    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IWorldModel))
        MyBase.New(parent, setState, context)
    End Sub

    Public Overrides Sub HandleCommand(cmd As String)
        Dim message = Model.Message.Current
        If Not message.HasChoices Then
            If cmd = Command.A OrElse cmd = Command.B Then
                Model.Message.Dismiss()
                OnStart()
            End If
            Return
        End If
        Select Case cmd
            Case Command.Up
                ChoiceIndex = (ChoiceIndex + message.ChoiceCount - 1) Mod message.ChoiceCount
            Case Command.Down
                ChoiceIndex = (ChoiceIndex + 1) Mod message.ChoiceCount
            Case Command.A
                Model.Avatar.MakeChoice(ChoiceIndex)
                OnStart()
            Case Command.B
                If Not ChoiceIndex = message.CancelChoice Then
                    ChoiceIndex = message.CancelChoice
                    Return
                End If
                Model.Avatar.MakeChoice(ChoiceIndex)
                OnStart()
        End Select
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink)
        displayBuffer.Fill(0)
        Dim message = Model.Message.Current
        Dim font = Context.Font(UIFont)
        ShowLines(displayBuffer, message, font)
        Dim aButtonText = ShowChoices(displayBuffer, message, font)
        Context.ShowStatusBar(displayBuffer, font, Context.ControlsText(aButtonText, Nothing), 0, 7)
    End Sub

    Private Function ShowChoices(displayBuffer As IPixelSink, message As IMessage, font As Font) As String
        If message.HasChoices Then
            Dim y = Context.ViewSize.Height * 2 \ 3 - font.Height \ 2
            displayBuffer.Fill((0, y), (Context.ViewSize.Width, font.Height), 1)
            y -= ChoiceIndex * font.Height
            Dim index = 0
            For Each choice In message.Choices
                font.WriteText(
                    displayBuffer,
                    (Context.ViewSize.Width \ 2 - font.TextWidth(choice.Text) \ 2, y),
                    choice.Text,
                    If(index = ChoiceIndex, 0, 1))
                index += 1
                y += font.Height
            Next
        End If
        Return If(message.HasChoices, SelectText, ContinueText)
    End Function

    Private Sub ShowLines(displayBuffer As IPixelSink, message As IMessage, font As Font)
        Dim y = If(message.HasChoices, Context.ViewSize.Height \ 3, Context.ViewSize.Height \ 2) - font.Height * message.LineCount \ 2
        For Each line In message.Lines
            font.WriteText(displayBuffer, (Context.ViewSize.Width \ 2 - font.TextWidth(line.Text) \ 2, y), line.Text, line.Hue)
            y += font.Height
        Next
    End Sub

    Public Overrides Sub OnStart()
        MyBase.OnStart()
        If Not Model.Message.Exists Then
            SetState(Neutral)
            Return
        End If
        PlaySfx(Model.Message.Current.Sfx)
        ChoiceIndex = 0
    End Sub
End Class
