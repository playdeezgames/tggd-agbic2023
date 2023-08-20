Public Interface IUIContext(Of TModel)
    ReadOnly Property Model As TModel
    Function Font(fontName As String) As Font
    Sub ShowStatusBar(displayBuffer As IPixelSink, font As Font, text As String, foreground As Integer, background As Integer)
    Sub ShowHeader(displayBuffer As IPixelSink, font As Font, text As String, foreground As Integer, background As Integer)
    ReadOnly Property ViewSize As (Width As Integer, Height As Integer)
    ReadOnly Property ViewCenter As (X As Integer, Y As Integer)
    Sub ShowSplashContent(displayBuffer As IPixelSink, font As Font)
    Sub ShowAboutContent(displayBuffer As IPixelSink, font As Font)
    Function ControlsText(aButtonText As String, bButtonText As String) As String
    ReadOnly Property AvailableWindowSizes As IEnumerable(Of (Width As Integer, Height As Integer))
    Sub AbandonGame()
    Sub LoadGame(slot As Integer)
    Sub SaveGame(slot As Integer)
    Function DoesSlotExist(slot As Integer) As Boolean
End Interface
