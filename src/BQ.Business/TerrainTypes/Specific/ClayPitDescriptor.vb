﻿Friend Class ClayPitDescriptor
    Inherits TerrainTypeDescriptor

    Public Sub New()
        MyBase.New(
            "Clay Pit",
            ChrW(&H3E),
            Tan,
            True,
            depletedTerrainType:="Empty",
            cellInitializer:=AddressOf InitializePit,
            effects:=New Dictionary(Of String, EffectData) From
            {
                {"Forage", New EffectData}
            },
            foragables:=New Dictionary(Of String, Integer) From
            {
                {String.Empty, 50},
                {"Clay", 50}
            },
            peril:=2,
            creatureTypeGenerator:=New Dictionary(Of String, Integer) From
            {
                {
                    "CherryGlop",
                    25
                }
            })
    End Sub

    Private Shared Sub InitializePit(cell As ICell)
        cell.SetStatistic(StatisticTypes.ForageRemaining, 30)
    End Sub
End Class
