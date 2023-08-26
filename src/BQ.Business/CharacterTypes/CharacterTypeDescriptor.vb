Public Class CharacterTypeDescriptor
    Inherits VisibleEntityDescriptor
    Friend ReadOnly Property MaskGlyph As Char
    Friend ReadOnly Property MaskHue As Integer
    Friend ReadOnly Property Statistics As IReadOnlyDictionary(Of String, Integer)
    Friend ReadOnly Property InitializeScript As String
    Friend ReadOnly Property EffectHandlers As IReadOnlyDictionary(Of String, Action(Of ICharacter, IEffect))
    Friend ReadOnly Property EffectScripts As IReadOnlyDictionary(Of String, String)
    Private ReadOnly Flags As HashSet(Of String)
    Friend ReadOnly Property HasFlag(flagType As String) As Boolean
        Get
            Return Flags.Contains(flagType)
        End Get
    End Property
    Friend Sub New(
                  name As String,
                  glyph As Char,
                  hue As Integer,
                  Optional MaskGlyph As Char = ChrW(0),
                  Optional maskHue As Integer = Black,
                  Optional statistics As IReadOnlyDictionary(Of String, Integer) = Nothing,
                  Optional flags As IEnumerable(Of String) = Nothing,
                  Optional effectHandlers As IReadOnlyDictionary(Of String, Action(Of ICharacter, IEffect)) = Nothing,
                  Optional effectScripts As IReadOnlyDictionary(Of String, String) = Nothing,
                  Optional initializeScript As String = Nothing)
        MyBase.New(name, glyph, hue)
        Me.InitializeScript = initializeScript
        Me.MaskGlyph = MaskGlyph
        Me.MaskHue = maskHue
        Me.Statistics = If(statistics, New Dictionary(Of String, Integer))
        Me.Flags = New HashSet(Of String)(If(flags, New List(Of String)))
        Me.EffectHandlers = If(effectHandlers, New Dictionary(Of String, Action(Of ICharacter, IEffect)))
        Me.EffectScripts = If(effectScripts, New Dictionary(Of String, String))
    End Sub

    Private Sub DoNothing(character As ICharacter)
        'as ordered!
    End Sub

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
        If EffectHandlers.ContainsKey(effectType) Then
            EffectHandlers(effectType).Invoke(character, effect)
            Return
        End If
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
