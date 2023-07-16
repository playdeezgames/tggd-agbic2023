Friend Class SfxVolumeState(Of TModel)
    Inherits BasePickerState(Of TModel, Single)
    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of TModel))
        MyBase.New(parent, setState, context, "<placeholder>", context.ControlsText("Select", "Cancel"), BoilerplateState.Options)
    End Sub
    Protected Overrides Sub OnActivateMenuItem(value As (String, Single))
        SfxVolume = value.Item2
        PlaySfx(BoilerplateSfx.SfxVolumeTest)
        SaveConfig()
        HeaderText = $"Sfx Volume (Currently: {SfxVolume * 100:f0}%)"
    End Sub
    Protected Overrides Function InitializeMenuItems() As List(Of (String, Single))
        Dim currentVolume = $"{SfxVolume * 100:f0}%"
        HeaderText = $"Sfx Volume (Currently: {currentVolume})"
        Dim result As New List(Of (String, Single))
        For index = 0 To 10
            Dim menuItemText = $"{index * 10}%"
            result.Add((menuItemText, index / 10.0F))
            If menuItemText = currentVolume Then
                MenuItemIndex = index
            End If
        Next
        Return result
    End Function
End Class
