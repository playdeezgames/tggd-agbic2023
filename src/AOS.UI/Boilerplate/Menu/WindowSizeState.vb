Friend Class WindowSizeState(Of TModel)
    Inherits BasePickerState(Of TModel, (Integer, Integer))
    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of TModel))
        MyBase.New(parent, setState, context, "<placeholder>", context.ControlsText("Select", "Cancel"), BoilerplateState.Options)
    End Sub
    Protected Overrides Sub OnActivateMenuItem(value As (String, (Integer, Integer)))
        Dim width = value.Item2.Item1
        Dim height = value.Item2.Item2
        Parent.Size = (width, height)
        SaveConfig()
        HeaderText = $"Current Size: {Size.Item1}x{Size.Item2}"
    End Sub
    Protected Overrides Function InitializeMenuItems() As List(Of (String, (Integer, Integer)))
        Dim currentSize = $"{Size.Item1}x{Size.Item2}"
        HeaderText = $"Current Size: {currentSize}"
        Dim menuItems = Context.AvailableWindowSizes.Select(Function(x) ($"{x.Item1}x{x.Item2}", (x.Item1, x.Item2))).ToList
        MenuItemIndex = Math.Max(0, menuItems.FindIndex(Function(x) x.Item1 = currentSize))
        Return menuItems
    End Function
End Class
