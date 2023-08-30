Imports SPLORR.Game

Friend Class TreeDescriptor
    Inherits TerrainTypeDescriptor

    Public Sub New()
        MyBase.New(
                    "Tree",
                    ChrW(&HA),
                    Business.Hue.Green,
                    True,
                    depletedTerrainType:=TerrainTypes.DepletedTree,
                    cellInitializer:=AddressOf InitializeTree,
                    effects:=New Dictionary(Of String, EffectData) From
                    {
                        {"Forage", New EffectData}
                    },
                    foragables:=New Dictionary(Of String, Integer) From
                    {
                        {String.Empty, 75},
                        {ItemTypes.Stick, 100},
                        {ItemTypes.Rock, 25}
                    },
                    peril:=1,
                    creatureTypeGenerator:=New Dictionary(Of String, Integer) From
                    {
                        {
                            CharacterTypes.OliveGlop,
                            75
                        },
                        {
                            CharacterTypes.CherryGlop,
                            25
                        }
                    })
    End Sub

    Private Shared Sub InitializeTree(cell As ICell)
        cell.SetStatistic(StatisticTypes.ForageRemaining, 50)
    End Sub
End Class
