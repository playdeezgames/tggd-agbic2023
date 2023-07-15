Public MustInherit Class BaseGameState(Of TModel)
    Implements IGameController
    Protected ReadOnly Property Parent As IGameController
    Private ReadOnly SetCurrentState As Action(Of String, Boolean)
    Protected ReadOnly Context As IUIContext(Of TModel)
    Protected ReadOnly Property Model As TModel
        Get
            Return Context.Model
        End Get
    End Property
    Protected Const Zero = 0
    Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of TModel))
        Me.Parent = parent
        Me.SetCurrentState = setState
        Me.Context = context
    End Sub
    Protected Sub PopState()
        SetCurrentState(Nothing, False)
    End Sub
    Protected Sub SetState(state As String)
        SetCurrentState(state, False)
    End Sub
    Protected Sub SetStates(pushedState As String, nextState As String)
        Parent.StartStateEnabled = False
        SetCurrentState(nextState, False)
        Parent.StartStateEnabled = True
        SetCurrentState(pushedState, True)
    End Sub
    Public Property Volume As Single Implements IGameController.Volume
        Get
            Return Parent.Volume
        End Get
        Set(value As Single)
            Parent.Volume = value
        End Set
    End Property
    Public Property Size As (Integer, Integer) Implements IGameController.Size
        Get
            Return Parent.Size
        End Get
        Set(value As (Integer, Integer))
            Parent.Size = value
        End Set
    End Property

    Public ReadOnly Property QuitRequested As Boolean Implements IGameController.QuitRequested
        Get
            Return Parent.QuitRequested
        End Get
    End Property

    Public Property FullScreen As Boolean Implements IGameController.FullScreen
        Get
            Return Parent.FullScreen
        End Get
        Set(value As Boolean)
            Parent.FullScreen = value
        End Set
    End Property
    Public Property StartStateEnabled As Boolean Implements IGameController.StartStateEnabled
    Public MustOverride Sub HandleCommand(cmd As String) Implements IGameController.HandleCommand
    Public MustOverride Sub Render(displayBuffer As IPixelSink) Implements IGameController.Render
    Public Sub SetSfxHook(handler As Action(Of String)) Implements IGameController.SetSfxHook
        Parent.SetSfxHook(handler)
    End Sub
    Public Sub PlaySfx(sfx As String) Implements IGameController.PlaySfx
        Parent.PlaySfx(sfx)
    End Sub
    Public Sub SetSizeHook(hook As Action(Of (Integer, Integer), Boolean)) Implements IGameController.SetSizeHook
        Parent.SetSizeHook(hook)
    End Sub
    Public Overridable Sub Update(elapsedTime As TimeSpan) Implements IGameController.Update
        'default implementation: do nothing!
    End Sub
    Public Overridable Sub OnStart()
        'default: do nothing!
    End Sub

    Public Sub SaveConfig() Implements IGameController.SaveConfig
        Parent.SaveConfig()
    End Sub
End Class
