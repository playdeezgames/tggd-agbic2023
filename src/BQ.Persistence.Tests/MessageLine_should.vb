Public Class MessageLine_should
    Const Text = "text"
    Const Hue = 1

    <Fact>
    Sub has_properties()
        Dim subject As IMessageLine = CreateSubject()
        subject.Text.ShouldBe(Text)
        subject.Hue.ShouldBe(Hue)
    End Sub

    Private Function CreateSubject() As IMessageLine
        Return (New World(New Data.WorldData)).CreateMessage().AddLine(Hue, Text).Lines.Single
    End Function
End Class
