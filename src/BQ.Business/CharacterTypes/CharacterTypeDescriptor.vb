Imports BQ.Persistence

Friend Class CharacterTypeDescriptor
    Inherits VisibleEntityDescriptor
    Friend ReadOnly Property MaskGlyph As Char
    Friend ReadOnly Property MaskHue As Integer
    Friend ReadOnly Property Statistics As IReadOnlyDictionary(Of String, Integer)
    Friend ReadOnly Property Initializer As Action(Of ICharacter)
    Friend ReadOnly Property Triggers As IReadOnlyDictionary(Of String, Action(Of ICharacter, IEffect))
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
                  Optional initializer As Action(Of ICharacter) = Nothing,
                  Optional flags As IEnumerable(Of String) = Nothing,
                  Optional triggers As IReadOnlyDictionary(Of String, Action(Of ICharacter, IEffect)) = Nothing)
        MyBase.New(name, glyph, hue)
        Me.MaskGlyph = MaskGlyph
        Me.MaskHue = maskHue
        Me.Statistics = If(statistics, New Dictionary(Of String, Integer))
        Me.Flags = New HashSet(Of String)(If(flags, New List(Of String)))
        Me.Initializer = If(initializer, AddressOf DoNothing)
        Me.Triggers = If(triggers, New Dictionary(Of String, Action(Of ICharacter, IEffect)))
    End Sub

    Private Sub DoNothing(character As ICharacter)
        'as ordered!
    End Sub
End Class
