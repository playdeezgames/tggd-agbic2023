Friend Class MessageTypeDescriptor
    ReadOnly Property Sfx As String
    ReadOnly Property Lines As IReadOnlyList(Of (hue As Integer, text As String))
    ReadOnly Property Choices As IReadOnlyList(Of (text As String, triggerType As String))
    Sub New(
           Optional sfx As String = Nothing,
           Optional lines As IEnumerable(Of (hue As Integer, text As String)) = Nothing,
           Optional choices As IEnumerable(Of (text As String, triggerType As String)) = Nothing)
        Me.Sfx = sfx
        Me.Lines = If(
            lines IsNot Nothing,
            New List(Of (hue As Integer, text As String))(lines),
            New List(Of (hue As Integer, Text As String)))
        Me.Choices = If(
            choices IsNot Nothing,
            New List(Of (text As String, triggerType As String))(choices),
            New List(Of (text As String, triggerType As String)))
    End Sub
    Sub CreateMessage(world As IWorld)
        Dim msg = world.CreateMessage().SetSfx(Sfx)
        For Each line In Lines
            msg.AddLine(line.hue, line.text)
        Next
        For Each choice In Choices
            msg.AddChoice(choice.text, choice.triggerType)
        Next
    End Sub
End Class
