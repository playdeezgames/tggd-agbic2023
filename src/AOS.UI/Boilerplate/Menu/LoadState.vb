Friend Class LoadState(Of TModel)
    Inherits BasePickerState(Of TModel, Integer)
    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of TModel))
        MyBase.New(parent, setState, context, "Load Game", context.ControlsText("Select", "Cancel"), BoilerplateState.MainMenu)
    End Sub

    Protected Overrides Sub OnActivateMenuItem(value As (String, Integer))
        Dim slotIndex = value.Item2
        If slotIndex > 0 AndAlso Context.DoesSlotExist(slotIndex) Then
            Context.LoadGame(slotIndex)
            SetState(BoilerplateState.Neutral)
        Else
            PlaySfx(BoilerplateSfx.Failure)
            SetState(BoilerplateState.MainMenu)
        End If
    End Sub

    Protected Overrides Function InitializeMenuItems() As List(Of (String, Integer))
        Dim result = New List(Of (String, Integer))
        For slotIndex = 1 To 5
            If Context.DoesSlotExist(slotIndex) Then
                result.Add(($"Slot {slotIndex}", slotIndex))
            End If
        Next
        If Not result.Any() Then
            result.Add(("No Saves Exist!", 0))
        End If
        Return result
    End Function
End Class
