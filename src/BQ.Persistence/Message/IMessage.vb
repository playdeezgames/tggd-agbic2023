Public Interface IMessage
    ReadOnly Property LineCount As Integer
    ReadOnly Property Lines As IEnumerable(Of IMessageLine)
    Function AddLine(hue As Integer, text As String) As IMessage
    Property Sfx As String
    Function SetSfx(sfx As String) As IMessage
End Interface
