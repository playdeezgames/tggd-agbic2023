Friend Class EquipState
    Inherits BasePickerState(Of IWorldModel, Integer)

    Public Sub New(
                  parent As IGameController,
                  setState As Action(Of String, Boolean),
                  context As IUIContext(Of IWorldModel))
        MyBase.New(
            parent,
            setState,
            context,
            PlaceholderText,
            context.ControlsText(SelectText, CancelText),
            GameState.InventoryDetail)
    End Sub

    Protected Overrides Sub OnActivateMenuItem(value As (String, Integer))
        Model.Item.Equip(value.Item2)
        SetState(Neutral)
    End Sub

    Protected Overrides Function InitializeMenuItems() As List(Of (String, Integer))
        Dim equippables = Model.Item.Equippables
        Select Case equippables.Count
            Case 0
                SetState(GameState.Inventory)
            Case 1
                Model.Item.Equip(equippables.Single.ItemId)
                SetState(Neutral)
        End Select
        Return equippables.ToList
    End Function
End Class
