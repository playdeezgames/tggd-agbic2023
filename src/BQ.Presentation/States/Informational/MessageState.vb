Friend Class MessageState
    Inherits BaseGameState(Of IWorldModel)

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IWorldModel))
        MyBase.New(parent, setState, context)
    End Sub

    Public Overrides Sub HandleCommand(cmd As String)
        If cmd = Command.A OrElse cmd = Command.B Then
            Model.Message.Dismiss()
            OnStart()
        End If
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink)
        displayBuffer.Fill((0, 0), Context.ViewSize, Black)
        Dim message = Model.Message.Current
        Dim font = Context.Font(UIFont)
        Dim y = Context.ViewSize.Item2 \ 2 - font.Height * message.LineCount \ 2
        For Each line In message.Lines
            font.WriteText(displayBuffer, (Context.ViewSize.Item1 \ 2 - font.TextWidth(line.Text) \ 2, y), line.Text, line.Hue)
            y += font.Height
        Next
        Context.ShowStatusBar(displayBuffer, font, Context.ControlsText("Continue", Nothing), Black, LightGray)
    End Sub
    Public Overrides Sub OnStart()
        MyBase.OnStart()
        If Not Model.Message.Exists Then
            SetState(Neutral)
            Return
        End If
        PlaySfx(Model.Message.Current.Sfx)
    End Sub
End Class
