Public Module ItemTypes
    Private Sub Save(filename As String)
        File.WriteAllText(filename, JsonSerializer.Serialize(descriptors))
    End Sub
    Private descriptors As IReadOnlyDictionary(Of String, ItemTypeDescriptor)

    Public Function ToItemTypeDescriptor(itemType As String) As ItemTypeDescriptor
        Return If(descriptors.ContainsKey(itemType), descriptors(itemType), Nothing)
    End Function

    Friend Sub Load(filename As String)
        descriptors = JsonSerializer.Deserialize(Of Dictionary(Of String, ItemTypeDescriptor))(File.ReadAllText(filename))
        For Each entry In descriptors
            entry.Value.FullNameScript = File.ReadAllText(entry.Value.FullNameScript)
        Next
    End Sub

    Friend ReadOnly Property All As IEnumerable(Of String)
        Get
            Return descriptors.Keys
        End Get
    End Property
End Module
