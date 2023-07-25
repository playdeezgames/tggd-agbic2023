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
            If cmd = Command.A Then
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
                Model.Avatar.DoChoiceTrigger(ChoiceIndex)
                OnStart()
        End Select
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink)
        displayBuffer.Fill((0, 0), Context.ViewSize, Black)
        Dim message = Model.Message.Current
        Dim font = Context.Font(UIFont)
        Dim y = If(message.HasChoices, Context.ViewSize.Item2 \ 3, Context.ViewSize.Item2 \ 2) - font.Height * message.LineCount \ 2
        For Each line In message.Lines
            font.WriteText(displayBuffer, (Context.ViewSize.Item1 \ 2 - font.TextWidth(line.Text) \ 2, y), line.Text, line.Hue)
            y += font.Height
        Next
        If message.HasChoices Then
            y = Context.ViewSize.Height * 2 \ 3 - font.Height \ 2
            displayBuffer.Fill((0, y), (Context.ViewSize.Width, font.Height), Blue)
            y -= ChoiceIndex * font.Height
            Dim index = 0
            For Each choice In message.Choices
                font.WriteText(displayBuffer, (Context.ViewSize.Width \ 2 - font.TextWidth(choice.Text) \ 2, y), choice.Text, If(index = ChoiceIndex, Black, Blue))
                index += 1
                y += font.Height
            Next
            Context.ShowStatusBar(displayBuffer, font, Context.ControlsText("Select", Nothing), Black, LightGray)
        Else
            Context.ShowStatusBar(displayBuffer, font, Context.ControlsText("Continue", Nothing), Black, LightGray)
        End If
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
