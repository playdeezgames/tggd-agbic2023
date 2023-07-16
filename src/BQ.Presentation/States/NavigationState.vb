Friend Class NavigationState
    Inherits BaseGameState(Of IWorldModel)

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IWorldModel))
        MyBase.New(parent, setState, context)
    End Sub

    Public Overrides Sub HandleCommand(cmd As String)
        Select Case cmd
            Case Command.B
                SetState(GameMenu)
        End Select
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink)
        displayBuffer.Fill((0, 0), Context.ViewSize, Purple)
        Dim font = Context.Font(BagelQuestFont)
        With font
            .WriteText(displayBuffer, (0, 0), ChrW(1), Black)
            .WriteText(displayBuffer, (0, 0), ChrW(2), Tan)
        End With
    End Sub
End Class
