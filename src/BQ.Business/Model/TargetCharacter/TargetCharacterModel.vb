Friend Class TargetCharacterModel
    Implements ITargetCharacterModel
    Private ReadOnly world As IWorld
    Public Sub New(world As IWorld)
        Me.world = world
    End Sub
    Public ReadOnly Property Verbs As IEnumerable(Of (String, String)) Implements ITargetCharacterModel.Verbs
        Get
            Return world.Avatar.TargetCharacter.Verbs.Select(Function(x) (x.ToVerbTypeDescriptor.Name, x))
        End Get
    End Property
    Public ReadOnly Property Exists As Boolean Implements ITargetCharacterModel.Exists
        Get
            Return world.Avatar.TargetCharacter IsNot Nothing
        End Get
    End Property
    Public ReadOnly Property Name As String Implements ITargetCharacterModel.Name
        Get
            Return world.Avatar.TargetCharacter.Name
        End Get
    End Property

    Public ReadOnly Property Statistic(statisticType As String) As Integer Implements ITargetCharacterModel.Statistic
        Get
            Return world.Avatar.TargetCharacter.Statistic(statisticType)
        End Get
    End Property

    Public ReadOnly Property Health As Integer Implements ITargetCharacterModel.Health
        Get
            Return world.Avatar.TargetCharacter.Health
        End Get
    End Property

    Public ReadOnly Property MaximumHealth As Integer Implements ITargetCharacterModel.MaximumHealth
        Get
            Return world.Avatar.TargetCharacter.MaximumHealth
        End Get
    End Property

    Public Sub DoVerb(verbType As String) Implements ITargetCharacterModel.DoVerb
        world.Avatar.TargetCharacter.DoVerb(verbType, world.Avatar)
    End Sub
End Class
