Friend Class EnemyModel
    Implements IEnemyModel

    Private world As IWorld

    Public Sub New(world As IWorld)
        Me.world = world
    End Sub

    Public ReadOnly Property Exists As Boolean Implements IEnemyModel.Exists
        Get
            Return world.Avatar.Enemy IsNot Nothing
        End Get
    End Property

    Public Sub Attack() Implements IEnemyModel.Attack
        'TODO: this is not how to attack
        world.Avatar.RemoveStatistic(StatisticTypes.EnemyCharacterId)
    End Sub
End Class
