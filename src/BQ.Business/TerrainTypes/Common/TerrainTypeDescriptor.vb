Imports BQ.Persistence

Friend Class TerrainTypeDescriptor
    Inherits VisibleEntityDescriptor
    Private ReadOnly Property Effects As IReadOnlyDictionary(Of String, EffectData)
    Friend ReadOnly Property Tenable As Boolean
    Friend ReadOnly Property CanInteract As Boolean
        Get
            Return VerbTypes.Any
        End Get
    End Property
    Friend ReadOnly Property VerbTypes As IReadOnlyDictionary(Of String, Action(Of ICharacter, ICell))
    Friend ReadOnly Property Foragables As IReadOnlyDictionary(Of String, Integer)
    Friend ReadOnly Property AllVerbs As IEnumerable(Of String)
        Get
            Return VerbTypes.Keys
        End Get
    End Property
    Friend ReadOnly Property AllEffectTypes As IEnumerable(Of String)
        Get
            Return Effects.Keys
        End Get
    End Property
    Friend ReadOnly Property CellInitializer As Action(Of ICell)
    Sub New(
           name As String,
           glyph As Char,
           hue As Integer,
           Optional tenable As Boolean = True,
           Optional verbTypes As IReadOnlyDictionary(Of String, Action(Of ICharacter, ICell)) = Nothing,
           Optional cellInitializer As Action(Of ICell) = Nothing,
           Optional foragables As IReadOnlyDictionary(Of String, Integer) = Nothing,
           Optional effects As IReadOnlyDictionary(Of String, EffectData) = Nothing)
        MyBase.New(name, glyph, hue)
        Me.Tenable = tenable
        Me.VerbTypes = If(verbTypes, New Dictionary(Of String, Action(Of ICharacter, ICell)))
        Me.CellInitializer = If(cellInitializer, AddressOf DoNothing)
        Me.Foragables = If(foragables, New Dictionary(Of String, Integer) From {{"", 1}})
        Me.Effects = If(effects, New Dictionary(Of String, EffectData))
    End Sub

    Private Sub DoNothing(cell As ICell)
        'as ordered!
    End Sub
End Class
