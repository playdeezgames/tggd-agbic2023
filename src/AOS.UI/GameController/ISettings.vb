Public Interface ISettings
    Property WindowSize As (Integer, Integer)
    Property FullScreen As Boolean
    Property SfxVolume As Single
    Property MuxVolume As Single
    Sub Save()
End Interface
