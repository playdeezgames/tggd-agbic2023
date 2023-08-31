Friend Module TerrainTypes
    Friend Const CookingFire = "CookingFire"
    Friend Const Farm = "Farm"
    Friend Const ClayPit = "ClayPit"
    Friend Const RockQuarry = "RockQuarry"
    Friend Const Potter = "Potter"
    Friend Const Furnace = "Furnace"
    Private ReadOnly descriptors As IReadOnlyDictionary(Of String, TerrainTypeDescriptor) =
        New Dictionary(Of String, TerrainTypeDescriptor) From
        {
            {"Grass", New GrassDescriptor()},
            {ClayPit, New ClayPitDescriptor()},
            {RockQuarry, New RockQuarryDescriptor()},
            {"DepletedGrass", New TerrainTypeDescriptor(
                    "Grass",
                    ChrW(4),
                    Business.Hue.Brown,
                    True)},
            {"Tree", New TreeDescriptor()},
            {"DepletedTree", New DepletedTreeDescriptor()},
            {"Empty", New EmptyTerrainDescriptor()},
            {"CellarFloor", New CellarFloorDescriptor()},
            {Farm, New FarmDescriptor()},
            {"Wall", New TerrainTypeDescriptor("Wall", ChrW(3), Hue.LightGray, False)},
            {"Gravel", New TerrainTypeDescriptor("Gravel", ChrW(6), Hue.DarkGray, True)},
            {"Fence", New TerrainTypeDescriptor("Fence", ChrW(5), Hue.Brown, False)},
            {"House", New TerrainTypeDescriptor("House", ChrW(7), Hue.Red, False)},
            {Town, New TownTerrainDescriptor()},
            {"Sign", New TerrainTypeDescriptor("Sign", ChrW(8), Hue.Brown, False)},
            {"RiverN", New TerrainTypeDescriptor("River", ChrW(&H17), Hue.Blue, False, isWaterSource:=True)},
            {"RiverE", New TerrainTypeDescriptor("River", ChrW(&H18), Hue.Blue, False, isWaterSource:=True)},
            {"RiverS", New TerrainTypeDescriptor("River", ChrW(&H15), Hue.Blue, False, isWaterSource:=True)},
            {"RiverW", New TerrainTypeDescriptor("River", ChrW(&H16), Hue.Blue, False, isWaterSource:=True)},
            {"RiverNE", New TerrainTypeDescriptor("River", ChrW(&H10), Hue.Blue, False, isWaterSource:=True)},
            {"RiverSE", New TerrainTypeDescriptor("River", ChrW(&HD), Hue.Blue, False, isWaterSource:=True)},
            {"RiverSW", New TerrainTypeDescriptor("River", ChrW(&HE), Hue.Blue, False, isWaterSource:=True)},
            {"RiverNW", New TerrainTypeDescriptor("River", ChrW(&HF), Hue.Blue, False, isWaterSource:=True)},
            {"RiverNES", New TerrainTypeDescriptor("River", ChrW(&H11), Hue.Blue, False, isWaterSource:=True)},
            {"RiverESW", New TerrainTypeDescriptor("River", ChrW(&H12), Hue.Blue, False, isWaterSource:=True)},
            {"RiverSWN", New TerrainTypeDescriptor("River", ChrW(&H13), Hue.Blue, False, isWaterSource:=True)},
            {"RiverWNE", New TerrainTypeDescriptor("River", ChrW(&H14), Hue.Blue, False, isWaterSource:=True)},
            {"RiverNESW", New TerrainTypeDescriptor("River", ChrW(&H19), Hue.Blue, False, isWaterSource:=True)},
            {"RiverNS", New TerrainTypeDescriptor("River", ChrW(&HB), Hue.Blue, False, isWaterSource:=True)},
            {"RiverEW", New TerrainTypeDescriptor("River", ChrW(&HC), Hue.Blue, False, isWaterSource:=True)},
            {"Door", New TerrainTypeDescriptor("Door", ChrW(&H1D), Hue.Orange, True)},
            {"Basin", New TerrainTypeDescriptor("Basin", ChrW(&H1E), Hue.Blue, False)},
            {"OldMan", New TerrainTypeDescriptor("Old Man", ChrW(&H1F), Hue.Purple, False)},
            {"StrongMan", New TerrainTypeDescriptor("Strong Man", ChrW(&H23), Hue.Brown, False)},
            {Potter, New TerrainTypeDescriptor("Potter", ChrW(&H41), Hue.Tan, False)},
            {"Druid", New TerrainTypeDescriptor("Druid", ChrW(&H24), Hue.LightGreen, False)},
            {"Bed", New TerrainTypeDescriptor("Bed", ChrW(&H26), Hue.Tan, False)},
            {"Gorachan", New TerrainTypeDescriptor("Gorachan", ChrW(&H27), Hue.Red, False)},
            {EnergyTrainer, New TerrainTypeDescriptor("Plucky Man", ChrW(&H29), Hue.Blue, False)},
            {"StairsUp", New TerrainTypeDescriptor("Up Stairs", ChrW(&H2B), Hue.Orange, False)},
            {"StairsDown", New TerrainTypeDescriptor("Down Stairs", ChrW(&H2C), Hue.Orange, False)},
            {CookingFire, New CookingFireDescriptor()},
            {Furnace, New FurnaceDescriptor()}
        }
    <Extension>
    Friend Function Descriptor(cell As ICell) As TerrainTypeDescriptor
        Return descriptors(cell.TerrainType)
    End Function
End Module
