Imports SPLORR.Game

Friend Class GrassDescriptor
    Inherits TerrainTypeDescriptor

    Public Sub New()
        MyBase.New(
                    "Grass",
                    ChrW(4),
                    Business.Hue.Green,
                    True,
                    depletedTerrainType:="DepletedGrass",
                    cellInitializer:=AddressOf InitializeGrass,
                    effects:=New Dictionary(Of String, EffectData) From
                    {
                        {"Forage", New EffectData}
                    },
                    foragables:=New Dictionary(Of String, Integer) From
                    {
                        {String.Empty, 1},
                        {"PlantFiber", 1}
                    })
    End Sub

    Private Shared Sub InitializeGrass(cell As ICell)
        cell.SetStatistic(StatisticTypes.ForageRemaining, 20)
    End Sub
End Class
