Public Module ItemTypes
    Private descriptors As IReadOnlyDictionary(Of String, ItemTypeDescriptor)

    Public Function ToItemTypeDescriptor(itemType As String) As ItemTypeDescriptor
        Return If(descriptors.ContainsKey(itemType), descriptors(itemType), Nothing)
    End Function

    Friend Sub Load(filename As String)
        descriptors = JsonSerializer.Deserialize(Of Dictionary(Of String, ItemTypeDescriptor))(File.ReadAllText(filename))
        For Each entry In descriptors
            Try
                entry.Value.FullNameScript = File.ReadAllText(entry.Value.FullNameScript)
            Catch
                entry.Value.FullNameScript = Nothing
            End Try
        Next
    End Sub

    Friend ReadOnly Property All As IEnumerable(Of String)
        Get
            Return descriptors.Keys
        End Get
    End Property
End Module
