Public Class Cell_should
    Private Function CreateSubject() As ICell
        Dim world As IWorld = New World(New Data.WorldData)
        Return world.CreateMap("MapType", (1, 1), "TerrainType").GetCell(0, 0)
    End Function
    <Fact>
    Sub hold_statistics()
        Dim subject As ICell = CreateSubject()
        DoStatisticHolderTests(subject)
    End Sub
    <Fact>
    Sub hold_metadata()
        Dim subject As ICell = CreateSubject()
        DoMetadataHolderTests(subject)
    End Sub
    <Fact>
    Sub hold_flags()
        Dim subject As ICell = CreateSubject()
        DoFlagHolderTests(subject)
    End Sub
End Class
