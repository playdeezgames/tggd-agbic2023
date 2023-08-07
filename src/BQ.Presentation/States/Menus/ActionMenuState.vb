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

    Private ReadOnly actionConditions As IReadOnlyList(Of (text As String, condition As Func(Of IWorldModel, Boolean))) =
        New List(Of (String, Func(Of IWorldModel, Boolean))) From
        {
            (GroundText, Function(m) m.Map.HasItems((0, 0))),
            (InventoryText, Function(m) m.Avatar.HasItems),
            (EquipmentText, Function(m) m.Avatar.HasEquipment),
            (SleepText, Function(m) m.Avatar.CanSleep),
            (ForageText, Function(m) m.Avatar.CanForage),
            (MakeTwineText, Function(m) m.Avatar.CanMakeTwine),
            (BuildFireText, Function(m) m.Avatar.CanBuildFire),
            (PutOutFireText, Function(m) m.Avatar.CanPutOutFire),
            (MakeTorchText, Function(m) m.Avatar.CanMakeTorch),
            (StatisticsText, Function(m) True)
        }


    Protected Overrides Function InitializeMenuItems() As List(Of (String, String))
        Return actionConditions.
            Where(Function(x) x.condition(Model)).
            Select(Function(x) (x.text, x.text)).
            ToList
    End Function
End Class
