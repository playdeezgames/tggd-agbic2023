Friend Class CombatModel
    Implements ICombatModel

    Private world As IWorld

    Public Sub New(world As IWorld)
        Me.world = world
    End Sub

    Public ReadOnly Property Exists As Boolean Implements ICombatModel.Exists
        Get
            Return world.Avatar.Cell.HasOtherCharacters(world.Avatar)
        End Get
    End Property

    Public ReadOnly Property Enemies As IEnumerable(Of (Glyph As Char, Hue As Integer, MaskGlyph As Char, MaskHue As Integer)) Implements ICombatModel.Enemies
        Get
            Return world.Avatar.Cell.
                OtherCharacters(world.Avatar).
                Select(Function(x) (x.CharacterType.ToCharacterTypeDescriptor.Glyph, x.CharacterType.ToCharacterTypeDescriptor.Hue, x.CharacterType.ToCharacterTypeDescriptor.MaskGlyph, x.CharacterType.ToCharacterTypeDescriptor.MaskHue))
        End Get
    End Property

    Public ReadOnly Property Enemy(index As Integer) As (Name As String, Health As Integer, MaximumHealth As Integer) Implements ICombatModel.Enemy
        Get
            Dim character = world.Avatar.Cell.
                OtherCharacters(world.Avatar).ElementAt(index)
            Return (character.Name, character.Health, character.MaximumHealth)
        End Get
    End Property

    Public ReadOnly Property Count As Integer Implements ICombatModel.Count
        Get
            Return world.Avatar.Cell.
                OtherCharacters(world.Avatar).
                Count
        End Get
    End Property
End Class
