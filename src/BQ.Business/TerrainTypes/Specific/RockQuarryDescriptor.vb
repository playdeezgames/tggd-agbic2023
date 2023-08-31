Friend Class RockQuarryDescriptor
    Inherits TerrainTypeDescriptor

    Public Sub New()
        MyBase.New(
            "Rock Quarry",
            ChrW(&H3E),
            DarkGray,
            True,
            depletedTerrainType:="Empty",
            initializerScript:="cell:SetStatistic(""ForageRemaining"", 50)",
            effects:=New Dictionary(Of String, EffectData) From
            {
                {"Forage", New EffectData}
            },
            foragables:=New Dictionary(Of String, Integer) From
            {
                {String.Empty, 50},
                {"Rock", 50}
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
End Class
