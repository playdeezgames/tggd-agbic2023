﻿Friend Class InventoryDetailState
    Inherits BasePickerState(Of IWorldModel, String)

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IWorldModel))
        MyBase.New(parent, setState, context, "<placeholder>", context.ControlsText("Select", "Cancel"), GameState.Inventory)
    End Sub

    Protected Overrides Sub OnActivateMenuItem(value As (String, String))
        Select Case value.Item2
            Case DropAllText
                Model.Item.Count = Model.Avatar.ItemCount(Model.Item.Name)
                SetState(GameState.Drop)
            Case DropOneText
                Model.Item.Count = 1
                SetState(GameState.Drop)
            Case DropHalfText
                Model.Item.Count = Model.Avatar.ItemCount(Model.Item.Name) \ 2
                SetState(GameState.Drop)
            Case EquipText
                SetState(GameState.Equip)
        End Select
    End Sub

    Protected Overrides Function InitializeMenuItems() As List(Of (String, String))
        Dim result As New List(Of (String, String))
        Dim itemCount = Model.Avatar.ItemCount(Model.Item.Name)
        HeaderText = $"{Model.Item.Name}(x{itemCount})"
        If itemCount > 1 Then
            result.Add((DropAllText, DropAllText))
        End If
        If itemCount > 2 Then
            result.Add((DropHalfText, DropHalfText))
        End If
        result.Add((DropOneText, DropOneText))
        If Model.Item.CanEquip Then
            result.Add((EquipText, EquipText))
        End If
        Return result
    End Function

    Public Overrides Sub OnStart()
        MyBase.OnStart()
        Select Case Model.Avatar.ItemCount(Model.Item.Name)
            Case 0
                SetState(GameState.Inventory)
        End Select
    End Sub
End Class
