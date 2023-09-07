Public Module TerrainTypes
    Private descriptors As IReadOnlyDictionary(Of String, TerrainTypeDescriptor)
    Private Sub Save(filename As String)
        File.WriteAllText(filename, JsonSerializer.Serialize(descriptors))
    End Sub
    Friend Sub Load(filename As String)
        descriptors = JsonSerializer.Deserialize(Of Dictionary(Of String, TerrainTypeDescriptor))(File.ReadAllText(filename))
        For Each entry In descriptors
            entry.Value.InitializerScript = File.ReadAllText(entry.Value.InitializerScript)
        Next
    End Sub
    Public Function Descriptor(cell As ICell) As TerrainTypeDescriptor
        Return descriptors(cell.TerrainType)
    End Function
End Module
