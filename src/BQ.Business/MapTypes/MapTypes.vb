Imports System.Data.Common
Imports System.Runtime.CompilerServices
Imports SPLORR.Game
Imports BQ.Persistence

Friend Module MapTypes
    Friend Const Initial = "Initial"
    Private ReadOnly descriptors As IReadOnlyDictionary(Of String, MapTypeDescriptor) =
        New Dictionary(Of String, MapTypeDescriptor) From
        {
            {
                Initial,
                New MapTypeDescriptor(
                    (9, 9),
                    TerrainTypes.Empty,
                    spawnCharacters:=New Dictionary(Of String, Integer) From
                                      {
                                        {CharacterTypes.Tizzy, 1},
                                        {CharacterTypes.Goblin, 12}
                                      },
                    customInitializer:=AddressOf InitializeInitialMap,
                    postProcessor:=AddressOf PostProcessInitialMap)
            }
        }

    Private Sub PostProcessInitialMap(map As IMap)
        RNG.FromEnumerable(map.Cells.Where(Function(x) x.Character IsNot Nothing AndAlso x.Character.CharacterType = CharacterTypes.Goblin).Select(Function(x) x.Character)).AddItem(ItemInitializer.CreateItem(map.World, ItemTypes.Key))
        RNG.FromEnumerable(map.Cells.Where(Function(x) x.Character IsNot Nothing AndAlso x.Character.CharacterType = CharacterTypes.Goblin).Select(Function(x) x.Character)).AddItem(ItemInitializer.CreateItem(map.World, ItemTypes.BikiniTop))
        RNG.FromEnumerable(map.Cells.Where(Function(x) x.Character IsNot Nothing AndAlso x.Character.CharacterType = CharacterTypes.Goblin).Select(Function(x) x.Character)).AddItem(ItemInitializer.CreateItem(map.World, ItemTypes.Thong))
    End Sub

    Private Sub InitializeInitialMap(map As IMap)
        For Each column In Enumerable.Range(0, map.Columns)
            map.GetCell(column, 0).TerrainType = TerrainTypes.Wall
            map.GetCell(column, map.Rows - 1).TerrainType = TerrainTypes.Wall
        Next
        For Each row In Enumerable.Range(1, map.Rows - 2)
            map.GetCell(0, row).TerrainType = TerrainTypes.Wall
            map.GetCell(map.Columns - 1, row).TerrainType = TerrainTypes.Wall
        Next
        map.GetCell(map.Columns - 1, map.Rows \ 2).TerrainType = TerrainTypes.LockedDoor
    End Sub

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
