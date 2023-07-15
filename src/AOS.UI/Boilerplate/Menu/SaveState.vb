Friend Class SaveState(Of TModel)
    Inherits BasePickerState(Of TModel, Integer)
    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of TModel))
        MyBase.New(parent, setState, context, "Save Game", context.ControlsText("Select", "Cancel"), BoilerplateState.GameMenu)
    End Sub
    Protected Overrides Sub OnActivateMenuItem(value As (String, Integer))
        Dim slotIndex = CInt(value.Item2)
        Context.SaveGame(slotIndex)
        SetState(BoilerplateState.GameMenu)
    End Sub
    Protected Overrides Function InitializeMenuItems() As List(Of (String, Integer))
        Dim result As New List(Of (String, Integer))
        For slotIndex = 1 To 5
            result.Add(($"Slot {slotIndex}{If(Context.DoesSlotExist(slotIndex), "(will overwrite)", "")}", slotIndex))
        Next
        Return result
    End Function
End Class
