﻿Public Interface IMessage
    Inherits IStatisticsHolder
    Inherits IFlagHolder
    Inherits IMetadataHolder
    ReadOnly Property LineCount As Integer
    ReadOnly Property Lines As IEnumerable(Of IMessageLine)
    ReadOnly Property Choices As IEnumerable(Of IMessageChoice)
    Function AddLine(hue As Integer, text As String) As IMessage
    Function AddChoice(text As String, effectType As String, Optional initializer As Action(Of IMessageChoice) = Nothing) As IMessage
    Property Sfx As String
    Function SetSfx(sfx As String) As IMessage
    Function Choice(index As Integer) As IMessageChoice
    ReadOnly Property HasChoices As Boolean
    ReadOnly Property ChoiceCount As Integer
End Interface
