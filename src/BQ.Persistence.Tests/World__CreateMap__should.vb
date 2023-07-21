Imports BQ.Data
Imports Shouldly
Imports Xunit

Public Class World__CreateMap__should
    <Fact>
    Public Sub create_a_map()
        Const mapType = "MapType"
        Const columns = 3
        Const rows = 4
        Const terrainType = "TerrainType"
        Dim data = New Data.WorldData
        Dim subject As IWorld = New World(data)
        Dim actual = subject.CreateMap(mapType, (columns, rows), terrainType)
        actual.MapType.ShouldBe(mapType)
        actual.Columns.ShouldBe(columns)
        actual.Rows.ShouldBe(rows)
        data.Maps.ShouldHaveSingleItem()
        data.Maps.Single.Cells.ShouldAllBe(Function(x) x.TerrainType = terrainType AndAlso Not x.CharacterIds.Any)
    End Sub
End Class
