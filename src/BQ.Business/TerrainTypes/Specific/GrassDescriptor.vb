Imports SPLORR.Game

Friend Class GrassDescriptor
    Inherits TerrainTypeDescriptor

    Public Sub New()
        MyBase.New(
                    "Grass",
                    ChrW(4),
                    2,
                    True,
                    depletedTerrainType:="DepletedGrass",
                    initializerScript:="cell:SetStatistic(""ForageRemaining"", 20)",
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
End Class
