Friend Class GroundState
    Inherits BasePickerState(Of IWorldModel, String)

    Public Sub New(
                  parent As IGameController,
                  setState As Action(Of String, Boolean),
                  context As IUIContext(Of IWorldModel))
        MyBase.New(
            parent,
            setState,
            context,
            OnTheGroundText,
            context.ControlsText(SelectText, CancelText),
            GameState.ActionMenu)
    End Sub

    Protected Overrides Sub OnActivateMenuItem(value As (String, String))
        Model.Item.Name = value.Item2
        SetState(GameState.GroundDetail)
    End Sub

    Protected Overrides Function InitializeMenuItems() As List(Of (String, String))
        Return Model.Map.GroundItems((0, 0))
    End Function

    Public Overrides Sub OnStart()
        MyBase.OnStart()
        If Not Model.Map.HasItems((0, 0)) Then
            SetState(ActionMenu)
            Return
        End If
    End Sub
End Class
