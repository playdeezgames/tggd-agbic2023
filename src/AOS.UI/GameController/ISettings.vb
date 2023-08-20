Public Interface ISettings
    Property WindowSize As (width As Integer, height As Integer)
    Property FullScreen As Boolean
    Property SfxVolume As Single
    Property MuxVolume As Single
    Sub Save()
End Interface
