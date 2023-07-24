Friend Class ActionMenuState
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
        End Select
    End Sub

    Protected Overrides Function InitializeMenuItems() As List(Of (String, String))
        Dim result = New List(Of (String, String)) From
            {
                (Constants.StatisticsText, Constants.StatisticsText)
            }
        If Model.Map.HasItems((0, 0)) Then
            result.Add((GroundText, GroundText))
        End If
        If Model.Avatar.HasItems Then
            result.Add((InventoryText, InventoryText))
        End If
        Return result
    End Function
End Class
