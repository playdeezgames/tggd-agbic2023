﻿Friend Class ActionMenuState
    Inherits BasePickerState(Of IWorldModel, String)

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IWorldModel))
        MyBase.New(parent, setState, context, "Actions", context.ControlsText("Select", "Cancel"), Neutral)
    End Sub

    Protected Overrides Sub OnActivateMenuItem(value As (String, String))
        Select Case value.Item2
            Case Constants.StatisticsText
                SetState(GameState.Statistics)
            Case Constants.GroundText
                SetState(GameState.Ground)
            Case Constants.InventoryText
                SetState(GameState.Inventory)
            Case Constants.ForageText
                SetState(GameState.Forage)
            Case Constants.MakeTwineText
                SetState(GameState.MakeTwine)
            Case Constants.EquipmentText
                SetState(GameState.Equipment)
            Case Constants.SleepText
                SetState(GameState.Sleep)
            Case Constants.BuildFireText
                SetState(GameState.BuildFire)
            Case Constants.PutOutFireText
                SetState(GameState.PutOutFire)
            Case Constants.MakeTorchText
                SetState(GameState.MakeTorch)
        End Select
    End Sub

    Protected Overrides Function InitializeMenuItems() As List(Of (String, String))
        Dim result = New List(Of (String, String))
        If Model.Map.HasItems((0, 0)) Then
            result.Add((GroundText, GroundText))
        End If
        If Model.Avatar.HasItems Then
            result.Add((InventoryText, InventoryText))
        End If
        If Model.Avatar.HasEquipment Then
            result.Add((EquipmentText, EquipmentText))
        End If
        If Model.Avatar.CanSleep Then
            result.Add((SleepText, SleepText))
        End If
        If Model.Avatar.CanForage Then
            result.Add((ForageText, ForageText))
        End If
        If Model.Avatar.CanMakeTwine Then
            result.Add((MakeTwineText, MakeTwineText))
        End If
        If Model.Avatar.CanBuildFire Then
            result.Add((BuildFireText, BuildFireText))
        End If
        If Model.Avatar.CanPutOutFire Then
            result.Add((PutOutFireText, PutOutFireText))
        End If
        If Model.Avatar.CanMakeTorch Then
            result.Add((MakeTorchText, MakeTorchText))
        End If
        result.Add((StatisticsText, StatisticsText))
        Return result
    End Function
End Class
