Friend Module CharacterTypes
    Private descriptors As IReadOnlyDictionary(Of String, CharacterTypeDescriptor)

    <Extension>
    Friend Function ToCharacterTypeDescriptor(characterType As String) As CharacterTypeDescriptor
        Return descriptors(characterType)
    End Function

    Const baseScriptPath = "./Content/Scripts/Characters"
    Friend Sub Load(filename As String)
        descriptors = JsonSerializer.Deserialize(Of Dictionary(Of String, CharacterTypeDescriptor))(File.ReadAllText(filename))
        For Each characterEntry In descriptors
            Dim loadedScript = File.ReadAllText(characterEntry.Value.InitializeScript)
            characterEntry.Value.InitializeScript = loadedScript
            characterEntry.Value.EffectScripts = characterEntry.Value.EffectScripts.ToDictionary(Function(x) x.Key, Function(x) File.ReadAllText(x.Value))
        Next
    End Sub

    Friend ReadOnly Property All As IEnumerable(Of String)
        Get
            Return descriptors.Keys
        End Get
    End Property
End Module
