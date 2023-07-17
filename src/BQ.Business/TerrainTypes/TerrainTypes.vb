Imports System.Runtime.CompilerServices
Imports BQ.Persistence

Friend Module TerrainTypes
    Friend Const Empty = "Empty"
    Friend Const Wall = "Wall"
    Friend Const Grass = "Grass"
    Friend Const Fence = "Fence"
    Friend Const Gravel = "Gravel"
    Friend Const House = "House"
    Friend Const Sign = "Sign"
    Private ReadOnly descriptors As IReadOnlyDictionary(Of String, TerrainTypeDescriptor) =
        New Dictionary(Of String, TerrainTypeDescriptor) From
        {
            {Empty, New TerrainTypeDescriptor("Empty", ChrW(0), Black, True)},
            {Wall, New TerrainTypeDescriptor("Wall", ChrW(3), Hue.LightGray, False)},
            {Grass, New TerrainTypeDescriptor("Grass", ChrW(4), Hue.Green, True)},
            {Gravel, New TerrainTypeDescriptor("Gravel", ChrW(6), Hue.DarkGray, True)},
            {Fence, New TerrainTypeDescriptor("Fence", ChrW(5), Hue.Brown, False)},
            {House, New TerrainTypeDescriptor("House", ChrW(7), Hue.Red, False)},
            {Sign, New TerrainTypeDescriptor("Sign", ChrW(8), Hue.Brown, False)}
        }

    <Extension>
    Friend Function ToTerrainTypeDescriptor(terrainType As String) As TerrainTypeDescriptor
        Return descriptors(terrainType)
    End Function
    Friend ReadOnly Property All As IEnumerable(Of String)
        Get
            Return descriptors.Keys
        End Get
    End Property
End Module
