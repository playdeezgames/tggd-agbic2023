Public Class GameController
    Inherits BaseGameController(Of IWorldModel)

    Public Sub New(settings As ISettings, context As IUIContext(Of IWorldModel))
        MyBase.New(settings, context)
        SetActionsStates(context)
        SetBoilerplateStates(context)
        SetOtherStates(context)
        SetCurrentState(BoilerplateState.Splash, True)
    End Sub

    Private Sub SetOtherStates(context As IUIContext(Of IWorldModel))
        SetState(GameState.ActionMenu, New ActionMenuState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.Combat, New CombatState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.Dead, New DeadState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.Equip, New EquipState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.Equipment, New EquipmentState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.Forage, New ForageState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.Ground, New GroundState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.GroundDetail, New GroundDetailState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.Inventory, New InventoryState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.InventoryDetail, New InventoryDetailState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.Message, New MessageState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.Navigation, New NavigationState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.Statistics, New StatisticsState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.Winner, New WinnerState(Me, AddressOf SetCurrentState, context))
        SetState(GameState.Craft, New CraftState(Me, AddressOf SetCurrentState, context))
    End Sub

    Private ReadOnly movementStates As IReadOnlyDictionary(Of String, (Integer, Integer)) =
        New Dictionary(Of String, (Integer, Integer)) From
        {
            {GameState.MoveUp, (0, -1)},
            {GameState.MoveDown, (0, 1)},
            {GameState.MoveLeft, (-1, 0)},
            {GameState.MoveRight, (1, 0)}
        }
    Private Function MakeActionState(
                                    context As IUIContext(Of IWorldModel),
                                    theAction As Action(Of IWorldModel),
                                    Optional nextState As String = BoilerplateState.Neutral
                                    ) As BaseGameState(Of IWorldModel)
        Return New BaseActionState(Me, AddressOf SetCurrentState, context, theAction, nextState)
    End Function
    Private Sub SetActionsStates(context As IUIContext(Of IWorldModel))
        For Each movementState In movementStates
            SetState(movementState.Key, New MoveState(Me, AddressOf SetCurrentState, context, movementState.Value))
        Next
        SetState(GameState.CookBagel, MakeActionState(context, Sub(m) m.Avatar.Crafting.CookBagel()))
        SetState(GameState.BuildFire, MakeActionState(context, Sub(m) m.Avatar.Crafting.BuildFire()))
        SetState(GameState.BuildFurnace, MakeActionState(context, Sub(m) m.Avatar.Crafting.BuildFurnace()))
        SetState(GameState.Drop, MakeActionState(context, Sub(m) m.Item.Drop(), GameState.Inventory))
        SetState(GameState.Knap, MakeActionState(context, Sub(m) m.Avatar.Crafting.Knap()))
        SetState(GameState.MakeHatchet, MakeActionState(context, Sub(m) m.Avatar.Crafting.MakeHatchet()))
        SetState(GameState.MakeTorch, MakeActionState(context, Sub(m) m.Avatar.Crafting.MakeTorch()))
        SetState(GameState.MakeTwine, MakeActionState(context, Sub(m) m.Avatar.Crafting.MakeTwine()))
        SetState(GameState.PutOutFire, MakeActionState(context, Sub(m) m.Avatar.Crafting.PutOutFire()))
        SetState(GameState.Run, MakeActionState(context, Sub(m) m.Combat.Run()))
        SetState(GameState.Sleep, MakeActionState(context, Sub(m) m.Avatar.Sleep()))
        SetState(GameState.Take, MakeActionState(context, Sub(m) m.Item.Take(), GameState.Ground))
    End Sub

    Private Sub SetBoilerplateStates(context As IUIContext(Of IWorldModel))
        SetState(BoilerplateState.Embark, New EmbarkState(Me, AddressOf SetCurrentState, context))
        SetState(BoilerplateState.Neutral, New NeutralState(Me, AddressOf SetCurrentState, context))
    End Sub
End Class
