Public Class MapEffect_should
    <Fact>
    Sub hold_statistics()
        Dim subject As IMapEffect = CreateSubject()
        DoStatisticHolderTests(subject)
    End Sub

    Private Function CreateSubject() As IMapEffect
        Return (New World(New Data.WorldData())).CreateMap("MapType", (1, 1), "TerrainType").CreateEffect()
    End Function

    <Fact>
    Sub hold_metadata()
        Dim subject As IMapEffect = CreateSubject()
        DoMetadataHolderTests(subject)
    End Sub
    <Fact>
    Sub hold_flags()
        Dim subject As IMapEffect = CreateSubject()
        DoFlagHolderTests(subject)
    End Sub
    <Fact>
    Sub have_properties()
        Dim subject As IMapEffect = CreateSubject()
        subject.Id.ShouldBe(0)
    End Sub
End Class
