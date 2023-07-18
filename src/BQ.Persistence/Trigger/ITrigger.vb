Public Interface ITrigger
    ReadOnly Property Id As Integer
    ReadOnly Property TriggerType As String
    Sub Execute(character As ICharacter)
    Function SetTriggerType(triggerType As String) As ITrigger
    Function AddMessageLine(hue As Integer, text As String) As ITrigger
End Interface
