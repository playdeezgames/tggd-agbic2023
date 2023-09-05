Public Class CharacterTypeDescriptor
    Inherits VisibleEntityDescriptor
    Public Property MaskGlyph As Char
    Public Property MaskHue As Integer
    Public Property Statistics As IReadOnlyDictionary(Of String, Integer)
    Public Property InitializeScript As String
    Public Property EffectScripts As IReadOnlyDictionary(Of String, String)
    Public Flags As HashSet(Of String)
    Friend Function HasFlag(flagType As String) As Boolean
        Return Flags.Contains(flagType)
    End Function

    Private Const CharacterIdentifier = "character"
    Private Const EffectIdentifier = "effect"

    Friend Sub RunInitializeScript(luaState As Lua, character As ICharacter)
        If Not String.IsNullOrEmpty(InitializeScript) Then
            luaState(CharacterIdentifier) = character
            luaState.DoString(InitializeScript)
            luaState(CharacterIdentifier) = Nothing
        End If
    End Sub

    Friend Sub RunEffectScript(luaState As Lua, effectType As String, character As ICharacter, effect As IEffect)
        If EffectScripts.ContainsKey(effectType) Then
            luaState(CharacterIdentifier) = character
            luaState(EffectIdentifier) = effect
            luaState.DoString(EffectScripts(effectType))
            luaState(EffectIdentifier) = Nothing
            luaState(CharacterIdentifier) = Nothing
            Return
        End If
        Throw New NotImplementedException($"No effect type '{effectType}' for '{CharacterExtensions.Name(character)}'(id: {character.Id})")
    End Sub
End Class
