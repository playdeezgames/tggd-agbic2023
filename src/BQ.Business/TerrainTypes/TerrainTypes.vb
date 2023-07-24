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
    Friend Const RiverN = "RiverN"
    Friend Const RiverE = "RiverE"
    Friend Const RiverW = "RiverS"
    Friend Const RiverS = "RiverW"
    Friend Const RiverNS = "RiverNS"
    Friend Const RiverEW = "RiverEW"
    Friend Const RiverNE = "RiverNE"
    Friend Const RiverSE = "RiverSE"
    Friend Const RiverSW = "RiverSW"
    Friend Const RiverNW = "RiverNW"
    Friend Const RiverNES = "RiverNES"
    Friend Const RiverESW = "RiverESW"
    Friend Const RiverSWN = "RiverSWN"
    Friend Const RiverWNE = "RiverWNE"
    Friend Const RiverNESW = "RiverNESW"
    Friend Const Tree = "Tree"
    Friend Const Town = "Town"
    Friend Const Door = "Door"
    Friend Const Basin = "Basin"
    Friend Const OldMan = "OldMan"
    Friend Const StrongMan = "StrongMan"
    Private ReadOnly descriptors As IReadOnlyDictionary(Of String, TerrainTypeDescriptor) =
        New Dictionary(Of String, TerrainTypeDescriptor) From
        {
            {Empty, New TerrainTypeDescriptor("Empty", ChrW(0), Black, True)},
            {Wall, New TerrainTypeDescriptor("Wall", ChrW(3), Hue.LightGray, False)},
            {Grass, New TerrainTypeDescriptor("Grass", ChrW(4), Hue.Green, True)},
            {Gravel, New TerrainTypeDescriptor("Gravel", ChrW(6), Hue.DarkGray, True)},
            {Fence, New TerrainTypeDescriptor("Fence", ChrW(5), Hue.Brown, False)},
            {House, New TerrainTypeDescriptor("House", ChrW(7), Hue.Red, False)},
            {Town, New TerrainTypeDescriptor("Town", ChrW(7), Hue.Red, True)},
            {Sign, New TerrainTypeDescriptor("Sign", ChrW(8), Hue.Brown, False)},
            {RiverN, New TerrainTypeDescriptor("River", ChrW(&H17), Hue.Blue, False)},
            {RiverE, New TerrainTypeDescriptor("River", ChrW(&H18), Hue.Blue, False)},
            {RiverS, New TerrainTypeDescriptor("River", ChrW(&H15), Hue.Blue, False)},
            {RiverW, New TerrainTypeDescriptor("River", ChrW(&H16), Hue.Blue, False)},
            {RiverNE, New TerrainTypeDescriptor("River", ChrW(&H10), Hue.Blue, False)},
            {RiverSE, New TerrainTypeDescriptor("River", ChrW(&HD), Hue.Blue, False)},
            {RiverSW, New TerrainTypeDescriptor("River", ChrW(&HE), Hue.Blue, False)},
            {RiverNW, New TerrainTypeDescriptor("River", ChrW(&HF), Hue.Blue, False)},
            {RiverNES, New TerrainTypeDescriptor("River", ChrW(&H11), Hue.Blue, False)},
            {RiverESW, New TerrainTypeDescriptor("River", ChrW(&H12), Hue.Blue, False)},
            {RiverSWN, New TerrainTypeDescriptor("River", ChrW(&H13), Hue.Blue, False)},
            {RiverWNE, New TerrainTypeDescriptor("River", ChrW(&H14), Hue.Blue, False)},
            {RiverNESW, New TerrainTypeDescriptor("River", ChrW(&H19), Hue.Blue, False)},
            {RiverNS, New TerrainTypeDescriptor("River", ChrW(&HB), Hue.Blue, False)},
            {RiverEW, New TerrainTypeDescriptor("River", ChrW(&HC), Hue.Blue, False)},
            {Door, New TerrainTypeDescriptor("Door", ChrW(&H1D), Hue.Orange, True)},
            {Basin, New TerrainTypeDescriptor("Basin", ChrW(&H1E), Hue.Blue, False)},
            {OldMan, New TerrainTypeDescriptor("Old Man", ChrW(&H1F), Hue.Purple, False)},
            {StrongMan, New TerrainTypeDescriptor("Strong Man", ChrW(&H23), Hue.Brown, False)},
            {
                Tree,
                New TerrainTypeDescriptor(
                    "Tree",
                    ChrW(&HA),
                    Hue.Green,
                    True,
                    cellInitializer:=AddressOf InitializeTree)}
        }

    Private Sub InitializeTree(cell As ICell)
        cell.Statistic(StatisticTypes.Peril) = 1
    End Sub

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
