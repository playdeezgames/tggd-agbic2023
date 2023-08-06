Imports BQ.Persistence

Friend Class TerrainTypeDescriptor
    Inherits VisibleEntityDescriptor
    Friend ReadOnly DepletedTerrainType As String
    Private ReadOnly Property Effects As IReadOnlyDictionary(Of String, EffectData)
    Friend ReadOnly Property Tenable As Boolean
    Friend ReadOnly Property CanInteract As Boolean
        Get
            Return Effects.Any
        End Get
    End Property
    Friend ReadOnly Property Foragables As IReadOnlyDictionary(Of String, Integer)
    Friend ReadOnly Property AllEffectTypes As IEnumerable(Of String)
        Get
            Return Effects.Keys
        End Get
    End Property
    Private ReadOnly Property CellInitializer As Action(Of ICell)
    Friend Sub Initialize(cell As ICell)
        CellInitializer.Invoke(cell)
    End Sub
    Sub New(
           name As String,
           glyph As Char,
           hue As Integer,
           Optional tenable As Boolean = True,
           Optional depletedTerrainType As String = Nothing,
           Optional cellInitializer As Action(Of ICell) = Nothing,
           Optional foragables As IReadOnlyDictionary(Of String, Integer) = Nothing,
           Optional effects As IReadOnlyDictionary(Of String, EffectData) = Nothing)
        MyBase.New(name, glyph, hue)
        Me.Tenable = tenable
        Me.DepletedTerrainType = depletedTerrainType
        Me.CellInitializer = If(cellInitializer, AddressOf DoNothing)
        Me.Foragables = If(foragables, New Dictionary(Of String, Integer) From {{"", 1}})
        Me.Effects = If(effects, New Dictionary(Of String, EffectData))
    End Sub
    Friend Function HasEffect(effectType As String) As Boolean
        Return Effects.ContainsKey(effectType)
    End Function
    Friend Sub DoEffect(character As ICharacter, effectType As String, cell As ICell)
        Dim effect As IEffect = New TerrainEffect(effectType, Effects(effectType), cell)
        character.Descriptor.EffectHandlers(effect.EffectType).Invoke(character, effect)
    End Sub

    Private Sub DoNothing(cell As ICell)
        'as ordered!
    End Sub
End Class
