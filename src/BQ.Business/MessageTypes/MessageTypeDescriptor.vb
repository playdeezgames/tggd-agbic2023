Public Class MessageTypeDescriptor
    Public Property Sfx As String
    Public Property Lines As IReadOnlyList(Of MessageLineData)
    Public Property Choices As IReadOnlyList(Of MessageChoiceData)
    Sub CreateMessage(world As IWorld)
        Dim msg = world.CreateMessage().SetSfx(Sfx)
        For Each line In Lines
            msg.AddLine(line.Hue, line.Text)
        Next
        For Each choice In Choices
            msg.AddChoice(choice.Text, choice.EffectType)
        Next
    End Sub
End Class
