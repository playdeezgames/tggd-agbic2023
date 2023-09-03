Public Module MessageTypes
    Private descriptors As IReadOnlyDictionary(Of String, MessageTypeDescriptor)
    Public Function ToMessageTypeDescriptor(messageType As String) As MessageTypeDescriptor
        Return descriptors(messageType)
    End Function

    Friend Sub Save(filename As String)
        File.WriteAllText(filename, JsonSerializer.Serialize(descriptors))
    End Sub

    Friend Sub Load(filename As String)
        descriptors = JsonSerializer.Deserialize(Of Dictionary(Of String, MessageTypeDescriptor))(File.ReadAllText(filename))
    End Sub
End Module
