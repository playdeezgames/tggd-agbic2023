Friend Class TreeDescriptor
    Inherits TerrainTypeDescriptor

    Public Sub New()
        MyBase.New(
                    "Tree",
                    ChrW(&HA),
                    2,
                    True,
                    depletedTerrainType:="DepletedTree",
                    initializerScript:="cell:SetStatistic(""ForageRemaining"", 50)",
                    effects:=New Dictionary(Of String, EffectData) From
                    {
                        {"Forage", New EffectData}
                    },
                    foragables:=New Dictionary(Of String, Integer) From
                    {
                        {String.Empty, 75},
                        {"Stick", 100},
                        {"Rock", 25}
                    },
                    peril:=1,
                    creatureTypeGenerator:=New Dictionary(Of String, Integer) From
                    {
                        {
                            "OliveGlop",
                            75
                        },
                        {
                            "CherryGlop",
                            25
                        }
                    })
    End Sub
End Class
