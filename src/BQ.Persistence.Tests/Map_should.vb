Public Class Map_should
    Private Const MapType As String = "MapType"
    Private Const MapColumns As Integer = 3
    Private Const MapRows As Integer = 2
    Private Const TerrainType As String = "TerrainType"

    <Fact>
    Sub hold_statistics()
        Dim subject As IMap = CreateSubject()
        DoStatisticHolderTests(subject)
    End Sub
    <Fact>
    Sub hold_metadata()
        Dim subject As IMap = CreateSubject()
        DoMetadataHolderTests(subject)
    End Sub
    <Fact>
    Sub hold_flags()
        Dim subject As IMap = CreateSubject()
        DoFlagHolderTests(subject)
    End Sub
    <Fact>
    Sub have_properties()
        Dim subject As IMap = CreateSubject()
        subject.World.ShouldNotBeNull
        subject.MapType.ShouldBe(MapType)
    End Sub
    <Fact>
    Sub create_effect()
        Dim subject As IMap = CreateSubject()
        subject.CreateEffect().ShouldNotBeNull()
    End Sub

    Private Function CreateSubject() As IMap
        Dim world As IWorld = New World(New Data.WorldData)
        Return world.CreateMap(MapType, (MapColumns, MapRows), TerrainType)
    End Function
End Class
