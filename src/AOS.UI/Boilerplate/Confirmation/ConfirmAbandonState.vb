Friend Class ConfirmAbandonState(Of TModel)
    Inherits BasePickerState(Of TModel, String)
    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of TModel))
        MyBase.New(parent, setState, context, "Are you sure you want to abandon the game?", context.ControlsText("Select", "Cancel"), BoilerplateState.GameMenu)
    End Sub
    Protected Overrides Sub OnActivateMenuItem(value As (String, String))
        Select Case value.Item2
            Case NoText
                SetState(BoilerplateState.GameMenu)
            Case YesText
                Context.AbandonGame()
                SetState(BoilerplateState.MainMenu)
        End Select
    End Sub
    Protected Overrides Function InitializeMenuItems() As List(Of (String, String))
        Return New List(Of (String, String)) From
            {
                (NoText, NoText),
                (YesText, YesText)
            }
    End Function
End Class
