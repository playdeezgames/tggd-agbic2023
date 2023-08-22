Friend Class CraftState
    Inherits BasePickerState(Of IWorldModel, String)

    Public Sub New(
                  parent As IGameController,
                  setState As Action(Of String, Boolean),
                  context As IUIContext(Of IWorldModel))
        MyBase.New(
            parent,
            setState,
            context,
            CraftText,
            context.ControlsText(SelectText, CancelText),
            GameState.ActionMenu)
    End Sub

    Protected Overrides Sub OnActivateMenuItem(value As (String, String))
        SetState(actionTable(value.Item2))
    End Sub

    Private ReadOnly actionConditions As IReadOnlyList(
        Of (text As String, condition As Func(Of IWorldModel, Boolean), state As String)) =
        New List(Of (String, Func(Of IWorldModel, Boolean), String)) From
        {
            (MakeTwineText, Function(m) m.Avatar.CanMakeTwine, GameState.MakeTwine),
            (BuildFireText, Function(m) m.Avatar.CanBuildFire, GameState.BuildFire),
            (MakeTorchText, Function(m) m.Avatar.CanMakeTorch, GameState.MakeTorch),
            (BuildFurnaceText, Function(m) m.Avatar.CanBuildFurnace, GameState.BuildFurnace),
            (CookBagelText, Function(m) m.Avatar.CanCookBagel, GameState.CookBagel),
            (KnapText, Function(m) m.Avatar.CanKnap, GameState.Knap),
            (MakeHatchetText, Function(m) m.Avatar.CanMakeHatchet, GameState.MakeHatchet)
        }

    Private ReadOnly actionTable As IReadOnlyDictionary(Of String, String) =
        actionConditions.ToDictionary(Function(x) x.text, Function(x) x.state)

    Protected Overrides Function InitializeMenuItems() As List(Of (String, String))
        Return actionConditions.
            Where(Function(x) x.condition(Model)).
            Select(Function(x) (x.text, x.text)).
            ToList
    End Function
End Class
