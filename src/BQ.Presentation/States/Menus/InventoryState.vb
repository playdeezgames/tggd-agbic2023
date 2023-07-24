﻿Friend Class InventoryState
    Inherits BasePickerState(Of IWorldModel, String)

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IWorldModel))
        MyBase.New(parent, setState, context, "Inventory", context.ControlsText("Select", "Cancel"), GameState.ActionMenu)
    End Sub

    Protected Overrides Sub OnActivateMenuItem(value As (String, String))
        Model.Item.Name = value.Item2
        SetState(GameState.InventoryDetail)
    End Sub

    Protected Overrides Function InitializeMenuItems() As List(Of (String, String))
        Return Model.Avatar.Items.ToList
    End Function

    Public Overrides Sub OnStart()
        MyBase.OnStart()
        If Not Model.Avatar.HasItems Then
            SetState(ActionMenu)
        End If
    End Sub
End Class