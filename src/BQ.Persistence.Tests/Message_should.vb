Public Class Message_should
    Private Function CreateSubject() As IMessage
        Dim world As IWorld = New World(New Data.WorldData)
        Return world.CreateMessage
    End Function
    <Fact>
    Sub hold_statistics()
        Dim subject As IMessage = CreateSubject()
        DoStatisticHolderTests(subject)
    End Sub
    <Fact>
    Sub hold_metadata()
        Dim subject As IMessage = CreateSubject()
        DoMetadataHolderTests(subject)
    End Sub
    <Fact>
    Sub hold_flags()
        Dim subject As IMessage = CreateSubject()
        DoFlagHolderTests(subject)
    End Sub
    <Fact>
    Sub has_properties()
        Dim subject As IMessage = CreateSubject()
        subject.LineCount.ShouldBe(0)
        subject.Lines.ShouldBeEmpty
        subject.ChoiceCount.ShouldBe(0)
        subject.Choices.ShouldBeEmpty
        subject.HasChoices.ShouldBeFalse
        subject.Sfx.ShouldBeNull
    End Sub
End Class
