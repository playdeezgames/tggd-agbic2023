Public Interface IWorldModel
    Sub Embark()
    Sub Abandon()
    Sub Load(filename As String)
    Sub Save(filename As String)
    ReadOnly Property Map As IMapModel
    ReadOnly Property Avatar As IAvatarModel
    ReadOnly Property Message As IMessageModel
    ReadOnly Property Enemy As IEnemyModel
End Interface
