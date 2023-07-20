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

    Public Sub Attack() Implements IEnemyModel.Attack
        'TODO: this is not how to attack
    End Sub
End Class
