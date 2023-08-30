Friend Module EffectTypes
    Private descriptors As IReadOnlyDictionary(Of String, EffectTypeDescriptor)
    Friend Function ToEffectTypeDescriptor(effectType As String) As EffectTypeDescriptor
        Return descriptors(effectType)
    End Function

    Friend Sub Load(filename As String)
        descriptors = JsonSerializer.Deserialize(Of Dictionary(Of String, EffectTypeDescriptor))(File.ReadAllText(filename))
    End Sub

    Friend ReadOnly Property All As IEnumerable(Of String)
        Get
            Return descriptors.Keys
        End Get
    End Property
End Module
