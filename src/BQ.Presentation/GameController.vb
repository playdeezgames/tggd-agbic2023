Public Class GameController
    Inherits BaseGameController(Of IWorldModel)

    Public Sub New(settings As ISettings, context As IUIContext(Of IWorldModel))
        MyBase.New(settings, context)
        SetBoilerplateStates(context)
        SetState(GameState.Navigation, New NavigationState(Me, AddressOf SetCurrentState, context))
        SetActionsStates(context)

        SetState(GameState.Message, New MessageState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.Combat, New CombatState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.Dead, New DeadState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.ActionMenu, New ActionMenuState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.Statistics, New StatisticsState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.Ground, New GroundState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.GroundDetail, New GroundDetailState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.Inventory, New InventoryState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.InventoryDetail, New InventoryDetailState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.Equip, New EquipState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.Equipment, New EquipmentState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.Forage, New ForageState(Me, AddressOf SetCurrentState, context))
        SetCurrentState(BoilerplateState.Splash, True)
    End Sub

    Private Sub SetActionsStates(context As IUIContext(Of IWorldModel))
        SetState(GameState.MoveUp, New MoveState(Me, AddressOf SetCurrentState, context, (0, -1)))
        SetState(GameState.MoveDown, New MoveState(Me, AddressOf SetCurrentState, context, (0, 1)))
        SetState(GameState.MoveLeft, New MoveState(Me, AddressOf SetCurrentState, context, (-1, 0)))
        SetState(GameState.MoveRight, New MoveState(Me, AddressOf SetCurrentState, context, (1, 0)))
        SetState(GameState.Run, New BaseActionState(Me, AddressOf SetCurrentState, context, Sub(m) m.Combat.Run(), BoilerplateState.Neutral))
        SetState(GameState.Drop, New BaseActionState(Me, AddressOf SetCurrentState, context, Sub(m) m.Item.Drop(), GameState.Inventory))
        SetState(GameState.Take, New BaseActionState(Me, AddressOf SetCurrentState, context, Sub(m) m.Item.Take(), GameState.Ground))
        SetState(GameState.Sleep, New BaseActionState(Me, AddressOf SetCurrentState, context, Sub(m) m.Avatar.Sleep(), BoilerplateState.Neutral))
        SetState(GameState.MakeTwine, New BaseActionState(Me, AddressOf SetCurrentState, context, Sub(m) m.Avatar.MakeTwine(), BoilerplateState.Neutral))
        SetState(GameState.BuildFire, New BaseActionState(Me, AddressOf SetCurrentState, context, Sub(m) m.Avatar.BuildFire(), BoilerplateState.Neutral))
        SetState(GameState.PutOutFire, New BaseActionState(Me, AddressOf SetCurrentState, context, Sub(m) m.Avatar.PutOutFire(), BoilerplateState.Neutral))
        SetState(GameState.MakeTorch, New BaseActionState(Me, AddressOf SetCurrentState, context, Sub(m) m.Avatar.MakeTorch(), BoilerplateState.Neutral))
        SetState(GameState.Knap, New BaseActionState(Me, AddressOf SetCurrentState, context, Sub(m) m.Avatar.Knap(), BoilerplateState.Neutral))
        SetState(GameState.MakeHatchet, New BaseActionState(Me, AddressOf SetCurrentState, context, Sub(m) m.Avatar.MakeHatchet(), BoilerplateState.Neutral))
        SetState(GameState.BuildFurnace, New BaseActionState(Me, AddressOf SetCurrentState, context, Sub(m) m.Avatar.BuildFurnace(), BoilerplateState.Neutral))
        SetState(GameState.CookBagel, New BaseActionState(Me, AddressOf SetCurrentState, context, Sub(m) m.Avatar.CookBagel(), BoilerplateState.Neutral))
    End Sub

    Private Sub SetBoilerplateStates(context As IUIContext(Of IWorldModel))
        SetState(BoilerplateState.Embark, New EmbarkState(Me, AddressOf SetCurrentState, context))
        SetState(BoilerplateState.Neutral, New NeutralState(Me, AddressOf SetCurrentState, context))
    End Sub
End Class
