Public Interface IWorldModel
    Sub Embark()
    Sub Abandon()
    Sub Load(filename As String)
    Sub Save(filename As String)
    ReadOnly Property Map As IMapModel
    ReadOnly Property Avatar As IAvatarModel
End Interface
