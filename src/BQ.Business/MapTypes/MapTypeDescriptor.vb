Imports BQ.Persistence

Friend Class MapTypeDescriptor
    ReadOnly Property Size As (Integer, Integer)
    ReadOnly Property DefaultTerrainType As String
    ReadOnly Property SpawnCharacters As IReadOnlyDictionary(Of String, Integer)
    ReadOnly Property SpawnItems As IReadOnlyDictionary(Of String, Integer)
    ReadOnly Property CustomInitializer As Action(Of IMap)
    ReadOnly Property PostProcessor As Action(Of IMap)
    Sub New(
           size As (Integer, Integer),
           Optional defaultTerrainType As String = Nothing,
           Optional spawnCharacters As IReadOnlyDictionary(Of String, Integer) = Nothing,
           Optional spawnItems As IReadOnlyDictionary(Of String, Integer) = Nothing,
           Optional customInitializer As Action(Of IMap) = Nothing,
           Optional postProcessor As Action(Of IMap) = Nothing)
        Me.Size = size
        Me.DefaultTerrainType = defaultTerrainType
        Me.SpawnCharacters = If(spawnCharacters, New Dictionary(Of String, Integer))
        Me.CustomInitializer = If(customInitializer, AddressOf DoNothing)
        Me.PostProcessor = If(postProcessor, AddressOf DoNothing)
        Me.SpawnItems = If(spawnItems, New Dictionary(Of String, Integer))
    End Sub
    Private Sub DoNothing(map As IMap)
        'like it sez....
    End Sub
End Class
