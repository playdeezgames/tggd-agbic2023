Public Class BaseGameController(Of TModel)
    Implements IGameController
    Protected ReadOnly Settings As ISettings
    Private _sizeHook As Action(Of (Integer, Integer), Boolean)
    Private ReadOnly _states As New Dictionary(Of String, BaseGameState(Of TModel))
    Private ReadOnly _stateStack As New Stack(Of String)
    Protected Sub SetCurrentState(state As String, push As Boolean)
        If Not push Then
            PopState()
        End If
        If Not String.IsNullOrEmpty(state) Then
            PushState(state)
            If StartStateEnabled Then
                _states(_stateStack.Peek).OnStart()
            End If
        End If
    End Sub
    Private Sub PushState(state As String)
        _stateStack.Push(state)
    End Sub

    Private Sub PopState()
        If _stateStack.Any Then
            _stateStack.Pop()
        End If
        If _stateStack.Any Then
            If StartStateEnabled Then
                _states(_stateStack.Peek).OnStart()
            End If
        End If
    End Sub

    Protected Sub SetState(state As String, handler As BaseGameState(Of TModel))
        _states(state) = handler
    End Sub
    Public Property Size As (Integer, Integer) Implements IGameController.Size
        Get
            Return Settings.WindowSize
        End Get
        Set(value As (Integer, Integer))
            If value.Item1 <> Settings.WindowSize.Item1 OrElse value.Item2 <> Settings.WindowSize.Item2 Then
                Settings.WindowSize = value
                _sizeHook(Settings.WindowSize, Settings.FullScreen)
            End If
        End Set
    End Property
    Public Property SfxVolume As Single Implements IGameController.SfxVolume
        Get
            Return Settings.SfxVolume
        End Get
        Set(value As Single)
            Settings.SfxVolume = Math.Clamp(value, 0.0F, 1.0F)
        End Set
    End Property
    Public ReadOnly Property QuitRequested As Boolean Implements IGameController.QuitRequested
        Get
            Return Not _stateStack.Any
        End Get
    End Property
    Public Property FullScreen As Boolean Implements IGameController.FullScreen
        Get
            Return Settings.FullScreen
        End Get
        Set(value As Boolean)
            If value <> Settings.FullScreen Then
                Settings.FullScreen = value
                _sizeHook(Settings.WindowSize, Settings.FullScreen)
            End If
        End Set
    End Property
    Public Property StartStateEnabled As Boolean = True Implements IGameController.StartStateEnabled

    Public Property MuxVolume As Single Implements IGameController.MuxVolume
        Get
            Return Settings.MuxVolume
        End Get
        Set(value As Single)
            Settings.MuxVolume = value
            If OnMuxVolume IsNot Nothing Then
                OnMuxVolume(Settings.MuxVolume)
            End If
        End Set
    End Property

    Sub New(settings As ISettings, context As IUIContext(Of TModel))
        Me.Settings = settings
        Me.Settings.Save()
        Me.SfxVolume = settings.SfxVolume
        Me.MuxVolume = settings.MuxVolume
        SetState(BoilerplateState.Splash, New SplashState(Of TModel)(Me, AddressOf SetCurrentState, context))
        SetState(BoilerplateState.MainMenu, New MainMenuState(Of TModel)(Me, AddressOf SetCurrentState, context))
        SetState(BoilerplateState.ConfirmQuit, New ConfirmQuitState(Of TModel)(Me, AddressOf SetCurrentState, context))
        SetState(BoilerplateState.About, New AboutState(Of TModel)(Me, AddressOf SetCurrentState, context))
        SetState(BoilerplateState.Options, New OptionsState(Of TModel)(Me, AddressOf SetCurrentState, context))
        SetState(BoilerplateState.WindowSize, New WindowSizeState(Of TModel)(Me, AddressOf SetCurrentState, context))
        SetState(BoilerplateState.SfxVolume, New SfxVolumeState(Of TModel)(Me, AddressOf SetCurrentState, context))
        SetState(BoilerplateState.MuxVolume, New MuxVolumeState(Of TModel)(Me, AddressOf SetCurrentState, context))
        SetState(BoilerplateState.Load, New LoadState(Of TModel)(Me, AddressOf SetCurrentState, context))
        SetState(BoilerplateState.Save, New SaveState(Of TModel)(Me, AddressOf SetCurrentState, context))
        SetState(BoilerplateState.Abandon, New ConfirmAbandonState(Of TModel)(Me, AddressOf SetCurrentState, context))
        SetState(BoilerplateState.GameMenu, New GameMenuState(Of TModel)(Me, AddressOf SetCurrentState, context))
        SetState(BoilerplateState.ScumSave, New ScumSaveState(Of TModel)(Me, AddressOf SetCurrentState, context))
        SetState(BoilerplateState.ScumLoadMainMenu, New ScumLoadState(Of TModel)(Me, AddressOf SetCurrentState, context, BoilerplateState.MainMenu))
        SetState(BoilerplateState.ScumLoadGameMenu, New ScumLoadState(Of TModel)(Me, AddressOf SetCurrentState, context, BoilerplateState.GameMenu))
    End Sub
    Private OnSfx As Action(Of String)
    Private OnMux As Action(Of String)
    Private OnMuxVolume As Action(Of Single)

    Public Sub HandleCommand(command As String) Implements IGameController.HandleCommand
        _states(_stateStack.Peek).HandleCommand(command)
    End Sub
    Public Sub Render(displayBuffer As IPixelSink) Implements IGameController.Render
        _states(_stateStack.Peek).Render(displayBuffer)
    End Sub

    Public Sub PlaySfx(sfx As String) Implements IGameController.PlaySfx
        OnSfx(sfx)
    End Sub

    Public Sub Update(elapsedTime As TimeSpan) Implements IGameController.Update
        _states(_stateStack.Peek).Update(elapsedTime)
    End Sub

    Public Sub SetSfxHook(handler As Action(Of String)) Implements IGameController.SetSfxHook
        OnSfx = handler
    End Sub

    Public Sub SetSizeHook(hook As Action(Of (Integer, Integer), Boolean)) Implements IGameController.SetSizeHook
        _sizeHook = hook
    End Sub

    Public Sub SaveConfig() Implements IGameController.SaveConfig
        Settings.Save()
    End Sub

    Public Sub SetMuxHook(handler As Action(Of String)) Implements IGameController.SetMuxHook
        OnMux = handler
    End Sub

    Public Sub PlayMux(mux As String) Implements IGameController.PlayMux
        OnMux(mux)
    End Sub

    Public Sub SetMuxVolumeHook(hook As Action(Of Single)) Implements IGameController.SetMuxVolumeHook
        OnMuxVolume = hook
    End Sub
End Class
