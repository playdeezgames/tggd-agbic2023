Public Class MessageChoice_should
    Const EffectType = "EffectType"
    Const Text = "Text"
    <Fact>
    Sub hold_statistics()
        Dim subject As IMessageChoice = CreateSubject()
        DoStatisticHolderTests(subject)
    End Sub
    <Fact>
    Sub hold_metadata()
        Dim subject As IMessageChoice = CreateSubject()
        DoMetadataHolderTests(subject)
    End Sub
    <Fact>
    Sub hold_flags()
        Dim subject As IMessageChoice = CreateSubject()
        DoFlagHolderTests(subject)
    End Sub
    <Fact>
    Sub have_properties()
        Dim subject As IMessageChoice = CreateSubject()
        subject.EffectType.ShouldBe(EffectType)
        subject.Text.ShouldBe(Text)
    End Sub

    Private Function CreateSubject() As IMessageChoice
        Return (New World(New Data.WorldData)).CreateMessage().AddChoice(Text, EffectType).Choice(0)
    End Function
End Class
