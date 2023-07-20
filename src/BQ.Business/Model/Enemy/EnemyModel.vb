Friend Class EnemyModel
    Implements IEnemyModel

    Private world As IWorld

    Public Sub New(world As IWorld)
        Me.world = world
    End Sub

    Public ReadOnly Property Exists As Boolean Implements IEnemyModel.Exists
        Get
            Return world.Avatar.Cell.HasOtherCharacters(world.Avatar)
        End Get
    End Property

    Public ReadOnly Property Enemies As IEnumerable(Of (Glyph As Char, Hue As Integer, MaskGlyph As Char, MaskHue As Integer)) Implements IEnemyModel.Enemies
        Get
            Return world.Avatar.Cell.
                OtherCharacters(world.Avatar).
                Select(Function(x) (x.CharacterType.ToCharacterTypeDescriptor.Glyph, x.CharacterType.ToCharacterTypeDescriptor.Hue, x.CharacterType.ToCharacterTypeDescriptor.MaskGlyph, x.CharacterType.ToCharacterTypeDescriptor.MaskHue))
        End Get
    End Property

    Public Sub Attack() Implements IEnemyModel.Attack
        'TODO: this is not how to attack
    End Sub
End Class
