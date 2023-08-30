Public Interface IMessage
    Inherits IStatisticsHolder
    Inherits IFlagHolder
    Inherits IMetadataHolder
    Property CancelChoice As Integer
    ReadOnly Property LineCount As Integer
    ReadOnly Property Lines As IEnumerable(Of IMessageLine)
    ReadOnly Property Choices As IEnumerable(Of IMessageChoice)
    Function AddLine(hue As Integer, text As String) As IMessage
    Function AddChoice(text As String, effectType As String) As IMessage
    ReadOnly Property LastChoice As IMessageChoice
    Property Sfx As String
    Function SetSfx(sfx As String) As IMessage
    Function Choice(index As Integer) As IMessageChoice
    ReadOnly Property HasChoices As Boolean
    ReadOnly Property ChoiceCount As Integer
End Interface
