Imports System.Security.Cryptography.X509Certificates

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
    <Fact>
    Sub read_and_write_sfx()
        Const Sfx = "Sfx"
        Dim subject As IMessage = CreateSubject()
        subject.Sfx = Sfx
        subject.Sfx.ShouldBe(Sfx)
    End Sub
    <Fact>
    Sub add_choice()
        Const Text = "Text"
        Const EffectType = "EffectType"
        Dim subject As IMessage = CreateSubject()
        subject.AddChoice(Text, EffectType)
        subject.HasChoices.ShouldBeTrue
        subject.Choices.ShouldNotBeEmpty
        subject.ChoiceCount.ShouldBe(1)
        subject.Choice(0).Text.ShouldBe(Text)
        subject.Choice(0).EffectType.ShouldBe(EffectType)
    End Sub
    <Fact>
    Sub add_line()
        Const Text = "Text"
        Const Hue = 1
        Dim subject As IMessage = CreateSubject()
        subject.AddLine(Hue, Text)
        subject.Lines.ShouldNotBeEmpty
        subject.LineCount.ShouldBe(1)
    End Sub
End Class
