Friend Module CharacterTypes
    Private descriptors As IReadOnlyDictionary(Of String, CharacterTypeDescriptor)

    <Extension>
    Friend Function ToCharacterTypeDescriptor(characterType As String) As CharacterTypeDescriptor
        Return descriptors(characterType)
    End Function

    Friend Sub Load(filename As String)
        descriptors = JsonSerializer.Deserialize(Of Dictionary(Of String, CharacterTypeDescriptor))(File.ReadAllText(filename))
    End Sub

    Friend ReadOnly Property All As IEnumerable(Of String)
        Get
            Return descriptors.Keys
        End Get
    End Property
End Module
