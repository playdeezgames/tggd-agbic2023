Friend Class ActionMenuState
    Inherits BasePickerState(Of IWorldModel, String)

    Public Sub New(
                  parent As IGameController,
                  setState As Action(Of String, Boolean),
                  context As IUIContext(Of IWorldModel))
        MyBase.New(
            parent,
            setState,
            context,
            ActionsText,
            context.ControlsText(SelectText, CancelText),
            Neutral)
    End Sub

    Protected Overrides Sub OnActivateMenuItem(value As (String, String))
        SetState(actionTable(value.Item2))
    End Sub

    Private ReadOnly actionConditions As IReadOnlyList(
        Of (text As String, condition As Func(Of IWorldModel, Boolean), state As String)) =
        New List(Of (String, Func(Of IWorldModel, Boolean), String)) From
        {
            (GroundText, Function(m) m.Map.HasItems((0, 0)), GameState.Ground),
            (InventoryText, Function(m) m.Avatar.HasItems, GameState.Inventory),
            (EquipmentText, Function(m) m.Avatar.HasEquipment, GameState.Equipment),
            (SleepText, Function(m) m.Avatar.CanSleep, GameState.Sleep),
            (ForageText, Function(m) m.Avatar.CanForage, GameState.Forage),
            (MakeTwineText, Function(m) m.Avatar.CanMakeTwine, GameState.MakeTwine),
            (BuildFireText, Function(m) m.Avatar.CanBuildFire, GameState.BuildFire),
            (PutOutFireText, Function(m) m.Avatar.CanPutOutFire, GameState.PutOutFire),
            (MakeTorchText, Function(m) m.Avatar.CanMakeTorch, GameState.MakeTorch),
            (BuildFurnaceText, Function(m) m.Avatar.CanBuildFurnace, GameState.BuildFurnace),
            (CookBagelText, Function(m) m.Avatar.CanCookBagel, GameState.CookBagel),
            (KnapText, Function(m) m.Avatar.CanKnap, GameState.Knap),
            (MakeHatchetText, Function(m) m.Avatar.CanMakeHatchet, GameState.MakeHatchet),
            (StatisticsText, Function(m) True, GameState.Statistics)
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
