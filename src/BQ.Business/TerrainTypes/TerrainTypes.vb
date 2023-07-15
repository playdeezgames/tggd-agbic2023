Imports System.Runtime.CompilerServices
Imports BQ.Persistence

Friend Module TerrainTypes
    Friend Const Empty = "Empty"
    Friend Const Wall = "Wall"
    Friend Const LockedDoor = "LockedDoor"
    Friend Const Door = "Door"
    Private ReadOnly descriptors As IReadOnlyDictionary(Of String, TerrainTypeDescriptor) =
        New Dictionary(Of String, TerrainTypeDescriptor) From
        {
            {Empty, New TerrainTypeDescriptor("Empty", ChrW(0), Black)},
            {Wall, New TerrainTypeDescriptor("Wall", ChrW(3), LightGray, tenable:=False)},
            {LockedDoor, New TerrainTypeDescriptor("Locked Door", ChrW(4), Orange, tenable:=False)},
            {Door, New TerrainTypeDescriptor("Door", ChrW(7), Orange, tenable:=True)}
        }

    <Extension>
    Friend Function ToTerrainTypeDescriptor(terrainType As String) As TerrainTypeDescriptor
        Return descriptors(terrainType)
    End Function
    Friend ReadOnly Property All As IEnumerable(Of String)
        Get
            Return descriptors.Keys
        End Get
    End Property
End Module
