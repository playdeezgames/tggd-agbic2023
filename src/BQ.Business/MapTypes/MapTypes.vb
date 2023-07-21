Imports System.Data.Common
Imports System.Runtime.CompilerServices
Imports SPLORR.Game
Imports BQ.Persistence

Friend Module MapTypes
    Friend Const Town = "Town"
    Friend Const Wilderness = "Wilderness"
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
                        {CharacterTypes.Schmeara, 1}
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
                            1
                        }
                    })
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
