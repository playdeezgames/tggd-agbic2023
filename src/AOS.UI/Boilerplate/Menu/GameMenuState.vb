Friend Class GameMenuState(Of TModel)
    Inherits BasePickerState(Of TModel, String)
    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of TModel))
        MyBase.New(parent, setState, context, "Game Menu", context.ControlsText("Select", "Cancel"), BoilerplateState.Neutral)
    End Sub
    Protected Overrides Sub OnActivateMenuItem(value As (String, String))
        Select Case value.Item2
            Case ContinueGameText
                SetState(BoilerplateState.Neutral)
            Case ScumSaveText
                SetState(BoilerplateState.ScumSave)
            Case ScumLoadText
                SetState(BoilerplateState.ScumLoadGameMenu)
            Case SaveGameText
                SetState(BoilerplateState.Save)
            Case OptionsText
                SetStates(BoilerplateState.Options, BoilerplateState.GameMenu)
            Case AbandonGameText
                SetState(BoilerplateState.Abandon)
        End Select
    End Sub
    Protected Overrides Function InitializeMenuItems() As List(Of (String, String))
        Return New List(Of (String, String)) From
            {
                (ContinueGameText, ContinueGameText),
                (ScumSaveText, ScumSaveText),
                (SaveGameText, SaveGameText),
                (ScumLoadText, ScumLoadText),
                (OptionsText, OptionsText),
                (AbandonGameText, AbandonGameText)
            }
    End Function
End Class
