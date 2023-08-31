Public Module TerrainTypes
    Private descriptors As IReadOnlyDictionary(Of String, TerrainTypeDescriptor)
    Friend Sub Load(filename As String)
        descriptors = JsonSerializer.Deserialize(Of Dictionary(Of String, TerrainTypeDescriptor))(File.ReadAllText(filename))
    End Sub
    Public Function Descriptor(cell As ICell) As TerrainTypeDescriptor
        Return descriptors(cell.TerrainType)
    End Function
End Module
