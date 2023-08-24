Friend Class EquipmentState
    Inherits BasePickerState(Of IWorldModel, String)

    Public Sub New(
                  parent As IGameController,
                  setState As Action(Of String, Boolean),
                  context As IUIContext(Of IWorldModel))
        MyBase.New(
            parent,
            setState,
            context,
            EquipmentText,
            context.ControlsText(UnequipText, CancelText),
            GameState.ActionMenu)
    End Sub

    Protected Overrides Sub OnActivateMenuItem(value As (String, String))
        Model.Avatar.Unequip(value.Item2)
        SetState(Neutral)
    End Sub

    Protected Overrides Function InitializeMenuItems() As List(Of (String, String))
        Return Model.Avatar.EquipmentDisplay.ToList
    End Function
End Class
