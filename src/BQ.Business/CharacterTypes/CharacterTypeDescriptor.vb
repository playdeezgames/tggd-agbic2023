Imports BQ.Persistence

Friend Class CharacterTypeDescriptor
    Inherits VisibleEntityDescriptor
    Friend ReadOnly Property MaskGlyph As Char
    Friend ReadOnly Property MaskHue As Integer
    Friend ReadOnly Property Statistics As IReadOnlyDictionary(Of String, Integer)
    Friend ReadOnly Property Initializer As Action(Of ICharacter)
    Friend ReadOnly Property Verbs As IReadOnlyDictionary(Of String, Action(Of ICharacter, ICharacter))
    Private ReadOnly Flags As HashSet(Of String)
    Friend ReadOnly Property HasFlag(flagType As String) As Boolean
        Get
            Return Flags.Contains(flagType)
        End Get
    End Property
    Friend ReadOnly Property AvailableVerbs As IEnumerable(Of String)
        Get
            Return Verbs.Keys
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
                  Optional verbs As IReadOnlyDictionary(Of String, Action(Of ICharacter, ICharacter)) = Nothing,
                  Optional flags As IEnumerable(Of String) = Nothing)
        MyBase.New(name, glyph, hue)
        Me.MaskGlyph = MaskGlyph
        Me.MaskHue = maskHue
        Me.Statistics = If(statistics, New Dictionary(Of String, Integer))
        Me.Verbs = If(verbs, New Dictionary(Of String, Action(Of ICharacter, ICharacter)))
        Me.Flags = New HashSet(Of String)(If(flags, New List(Of String)))
        Me.Initializer = If(initializer, Sub(x)
                                         End Sub)
    End Sub
End Class
