Imports SPLORR.Game

Friend Class GrassDescriptor
    Inherits TerrainTypeDescriptor

    Public Sub New()
        MyBase.New(
                    "Grass",
                    ChrW(4),
                    Business.Hue.Green,
                    True,
                    depletedTerrainType:=TerrainTypes.DepletedGrass,
                    cellInitializer:=AddressOf InitializeGrass,
                    effects:=New Dictionary(Of String, EffectData) From
                    {
                        {EffectTypes.Forage, New EffectData}
                    },
                    foragables:=New Dictionary(Of String, Integer) From
                    {
                        {String.Empty, 1},
                        {ItemTypes.PlantFiber, 1}
                    })
    End Sub

    Private Shared Sub InitializeGrass(cell As ICell)
        cell.Statistic(StatisticTypes.ForageRemaining) = 20
    End Sub
End Class
