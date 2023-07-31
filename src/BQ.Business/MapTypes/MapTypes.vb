Imports System.Data.Common
Imports System.Runtime.CompilerServices
Imports SPLORR.Game
Imports BQ.Persistence

Friend Module MapTypes
    Friend Const Town = "Town"
    Friend Const Wilderness = "Wilderness"
    Friend Const Healer = "Healer"
    Friend Const HealthTrainer = "HealthTrainer"
    Friend Const DruidHouse = "DruidHouse"
    Friend Const Inn = "Inn"
    Friend Const EnergyTrainer = "EnergyTrainer"
    Friend Const Cellar = "Cellar"
    Private ReadOnly descriptors As IReadOnlyDictionary(Of String, MapTypeDescriptor) =
        New Dictionary(Of String, MapTypeDescriptor) From
        {
            {
                Town,
                New MapTypeDescriptor(
                    (TownColumns, TownRows),
                    TerrainTypes.Empty,
                    customInitializer:=AddressOf TownInitializer.Initialize,
                    spawnCharacters:=New Dictionary(Of String, Integer) From
                    {
                        {CharacterTypes.Loxy, 1}
                    })
            },
            {
                Wilderness,
                New MapTypeDescriptor(
                    (WildernessColumns, WildernessRows),
                    TerrainTypes.Grass,
                    customInitializer:=AddressOf WildernessInitializer.Initialize,
                    encounterGenerator:=New Dictionary(Of String, Integer) From
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
            },
            {
                Healer,
                New MapTypeDescriptor(
                    (HealerColumns, HealerRows),
                    TerrainTypes.Empty,
                    customInitializer:=AddressOf HealerInitializer.Initialize)
            },
            {
                HealthTrainer,
                New MapTypeDescriptor(
                    (HealthTrainerColumns, HealthTrainerRows),
                    TerrainTypes.Empty,
                    customInitializer:=AddressOf HealthTrainerInitializer.Initialize)
            },
            {
                DruidHouse,
                New MapTypeDescriptor(
                    (DruidHouseColumns, DruidHouseRows),
                    TerrainTypes.Empty,
                    customInitializer:=AddressOf DruidHouseInitializer.Initialize)
            },
            {
                Inn,
                New MapTypeDescriptor(
                    (InnColumns, InnRows),
                    TerrainTypes.Empty,
                    customInitializer:=AddressOf InnInitializer.Initialize)
            },
            {
                Cellar,
                New MapTypeDescriptor(
                    (CellarColumns, CellarRows),
                    TerrainTypes.CellarFloor,
                    customInitializer:=AddressOf CellarInitializer.Initialize)
            },
            {
                EnergyTrainer,
                New MapTypeDescriptor(
                    (EnergyTrainerColumns, EnergyTrainerRows),
                    TerrainTypes.Empty,
                    customInitializer:=AddressOf EnergyTrainerInitializer.Initialize)
            }
        }

    <Extension>
    Friend Function ToMapTypeDescriptor(mapType As String) As MapTypeDescriptor
        Return descriptors(mapType)
    End Function
    Friend ReadOnly Property All As IEnumerable(Of String)
        Get
            Return descriptors.Keys
        End Get
    End Property
End Module
