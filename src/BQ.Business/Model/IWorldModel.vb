Public Interface IWorldModel
    Sub Embark()
    Sub Abandon()
    Sub Load(filename As String)
    Sub Save(filename As String)
    ReadOnly Property Map As IMapModel
    ReadOnly Property Avatar As IAvatarModel
    ReadOnly Property Message As IMessageModel
    ReadOnly Property Combat As ICombatModel
    ReadOnly Property Item As IItemModel
    ReadOnly Property Foraging As IForagingModel
End Interface
