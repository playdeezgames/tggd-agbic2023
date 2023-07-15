Public Interface IGameController
    ReadOnly Property QuitRequested As Boolean
    Sub SaveConfig()
    Sub HandleCommand(cmd As String)
    Sub Render(displayBuffer As IPixelSink)
    Sub Update(elapsedTime As TimeSpan)
    Sub SetSfxHook(handler As Action(Of String))
    Sub PlaySfx(sfx As String)
    Property Volume As Single
    Property Size As (Integer, Integer)
    Property FullScreen As Boolean
    Sub SetSizeHook(hook As Action(Of (Integer, Integer), Boolean))
    Property StartStateEnabled As Boolean
End Interface
