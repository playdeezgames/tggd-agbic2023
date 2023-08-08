Friend Class FarmDescriptor
    Inherits TerrainTypeDescriptor

    Public Sub New()
        MyBase.New(
            "Farm",
            ChrW(&H38),
            Business.Hue.Brown,
            True,
            peril:=1,
            depletedTerrainType:=TerrainTypes.Empty,
            cellInitializer:=AddressOf InitializeFarm,
            effects:=New Dictionary(Of String, EffectData) From
            {
                {EffectTypes.Forage, New EffectData}
            },
            foragables:=New Dictionary(Of String, Integer) From
            {
                {String.Empty, 1},
                {ItemTypes.PlantFiber, 1},
                {ItemTypes.Pepper, 1}
            },
            creatureTypeGenerator:=New Dictionary(Of String, Integer) From
            {
                {CharacterTypes.Scarecrow, 1}
            })
    End Sub

    Private Shared Sub InitializeFarm(cell As ICell)
        cell.Statistic(StatisticTypes.ForageRemaining) = 30
    End Sub
End Class
