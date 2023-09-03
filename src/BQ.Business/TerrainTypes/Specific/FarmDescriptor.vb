Friend Class FarmDescriptor
    Inherits TerrainTypeDescriptor

    Public Sub New()
        MyBase.New(
            "Farm",
            ChrW(&H38),
            6,
            True,
            peril:=1,
            depletedTerrainType:="Empty",
            initializerScript:="cell:SetStatistic(""ForageRemaining"", 30)",
            effects:=New Dictionary(Of String, EffectData) From
            {
                {"Forage", New EffectData}
            },
            foragables:=New Dictionary(Of String, Integer) From
            {
                {String.Empty, 1},
                {"PlantFiber", 1},
                {"Pepper", 1},
                {"Wheat", 1}
            },
            creatureTypeGenerator:=New Dictionary(Of String, Integer) From
            {
                {"Scarecrow", 1}
            })
    End Sub
End Class
