Friend Class AvatarModel
    Implements IAvatarModel
    Private ReadOnly world As IWorld
    Public Sub New(world As IWorld)
        Me.world = world
    End Sub
    Public ReadOnly Property MaximumAttack As Integer Implements IAvatarModel.MaximumAttack
        Get
            Return world.Avatar.MaximumAttack
        End Get
    End Property

    Public ReadOnly Property MaximumDefend As Integer Implements IAvatarModel.MaximumDefend
        Get
            Return world.Avatar.MaximumDefend
        End Get
    End Property

    Public ReadOnly Property Health As Integer Implements IAvatarModel.Health
        Get
            Return world.Avatar.Health
        End Get
    End Property

    Public ReadOnly Property MaximumHealth As Integer Implements IAvatarModel.MaximumHealth
        Get
            Return world.Avatar.MaximumHealth
        End Get
    End Property

    Public Property SelectedItemName As String Implements IAvatarModel.SelectedItemName
        Get
            Return world.Avatar.Metadata(Metadatas.SelectedItemName)
        End Get
        Set(value As String)
            world.Avatar.Metadata(Metadatas.SelectedItemName) = value
        End Set
    End Property

    Public Property SelectedItemId As Integer? Implements IAvatarModel.SelectedItemId
        Get
            If world.Avatar.HasStatistic(StatisticTypes.SelectedItemId) Then
                Return world.Avatar.Statistic(StatisticTypes.SelectedItemId)
            End If
            Return Nothing
        End Get
        Set(value As Integer?)
            If value.HasValue Then
                world.Avatar.Statistic(StatisticTypes.SelectedItemId) = value.Value
                Return
            End If
            world.Avatar.RemoveStatistic(StatisticTypes.SelectedItemId)
        End Set
    End Property

    Public ReadOnly Property Encumbrance As IAvatarEncumbranceModel Implements IAvatarModel.Encumbrance
        Get
            Return New AvatarEncumbranceModel(world.Avatar)
        End Get
    End Property

    Public ReadOnly Property IsBagelQuest As Boolean Implements IAvatarModel.IsBagelQuest
        Get
            Return world.Avatar.IsBagelQuest
        End Get
    End Property

    Public ReadOnly Property Inventory As IAvatarInventoryModel Implements IAvatarModel.Inventory
        Get
            Return New AvatarInventoryModel(world.Avatar, Me)
        End Get
    End Property

    Public Property Epitaph As String Implements IAvatarModel.Epitaph
        Get
            Return world.Avatar.Metadata(Metadatas.Epitaph)
        End Get
        Set(value As String)
            world.Avatar.Metadata(Metadatas.Epitaph) = value
        End Set
    End Property

    Public ReadOnly Property AverageAttack As Double Implements IAvatarModel.AverageAttack
        Get
            Return world.Avatar.AttackDice / 6.0
        End Get
    End Property

    Public ReadOnly Property AverageDefend As Double Implements IAvatarModel.AverageDefend
        Get
            Return world.Avatar.DefendDice / 6.0
        End Get
    End Property

    Public ReadOnly Property Ground As IAvatarGroundModel Implements IAvatarModel.Ground
        Get
            Return New AvatarGroundModel(world.Avatar, Me)
        End Get
    End Property

    Public ReadOnly Property SelectedItem As IAvatarSelectedItemModel Implements IAvatarModel.SelectedItem
        Get
            Return New AvatarSelectedItemModel()
        End Get
    End Property

    Public ReadOnly Property Combat As IAvatarCombatModel Implements IAvatarModel.Combat
        Get
            Return New AvatarCombatModel()
        End Get
    End Property

    Public ReadOnly Property Equipment As IAvatarEquipmentModel Implements IAvatarModel.Equipment
        Get
            Return New AvatarEquipmentModel(world.Avatar)
        End Get
    End Property

    Public ReadOnly Property SelectedItemFullName As String Implements IAvatarModel.SelectedItemFullName
        Get
            Return world.Item(SelectedItemId.Value).FullName
        End Get
    End Property

    Public ReadOnly Property Dignity As Integer Implements IAvatarModel.Dignity
        Get
            Return world.Avatar.Dignity
        End Get
    End Property

    Public Sub Move(delta As (Integer, Integer)) Implements IAvatarModel.Move
        If Encumbrance.IsOver Then
            world.CreateMessage().AddLine(LightGray, $"{world.Avatar.Name} is carrying too much.")
            Return
        End If
        world.Avatar.Move(delta)
    End Sub

    Public Sub TakeSelectedItem() Implements IAvatarModel.TakeSelectedItem
        Dim item = world.Item(SelectedItemId.Value)
        world.Avatar.TakeItem(item)
        SelectedItemName = Nothing
        SelectedItemId = Nothing
    End Sub
End Class
