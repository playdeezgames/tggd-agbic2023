Public Interface IGameController
    ReadOnly Property QuitRequested As Boolean
    Sub SaveConfig()
    Sub HandleCommand(cmd As String)
    Sub Render(displayBuffer As IPixelSink)
    Sub Update(elapsedTime As TimeSpan)
    Sub SetSfxHook(handler As Action(Of String))
    Sub SetMuxHook(handler As Action(Of String))
    Sub PlaySfx(sfx As String)
    Sub PlayMux(mux As String)
    Property SfxVolume As Single
    Property Size As (Integer, Integer)
    Property FullScreen As Boolean
    Property MuxVolume As Single
    Sub SetMuxVolumeHook(hook As Action(Of Single))
    Sub SetSizeHook(hook As Action(Of (Integer, Integer), Boolean))
    Property StartStateEnabled As Boolean
End Interface
