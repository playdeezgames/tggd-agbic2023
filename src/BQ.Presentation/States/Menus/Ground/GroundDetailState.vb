Friend Class GroundDetailState
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
            GameState.Ground)
    End Sub

    Protected Overrides Sub OnActivateMenuItem(value As (String, Integer))
        Model.Item.Count = value.Item2
        SetState(GameState.Take)
    End Sub

    Protected Overrides Function InitializeMenuItems() As List(Of (String, Integer))
        Dim itemCount = Model.Map.ItemCount((0, 0), Model.Item.Name)
        HeaderText = Model.Map.FormatItemCount((0, 0), Model.Item.Name)
        Dim result As New List(Of (String, Integer))
        If itemCount > 1 Then
            result.Add((TakeAllText, itemCount))
        End If
        If itemCount > 2 Then
            result.Add((TakeHalfText, itemCount \ 2))
        End If
        result.Add((TakeOneText, 1))
        Return result
    End Function

    Public Overrides Sub OnStart()
        MyBase.OnStart()
        Select Case Model.Map.ItemCount((0, 0), Model.Item.Name)
            Case 0
                SetState(GameState.Ground)
            Case 1
                Model.Item.Count = 1
                SetState(GameState.Take)
        End Select
    End Sub
End Class
