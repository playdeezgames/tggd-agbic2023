Public Interface IWorldModel
    Sub Embark()
    Sub Abandon()
    Sub Load(filename As String)
    Sub Save(filename As String)
    ReadOnly Property Map As IMapModel
    ReadOnly Property Avatar As IAvatarModel
    ReadOnly Property TargetCharacter As ITargetCharacterModel
    ReadOnly Property Messages As IMessagesModel
    ReadOnly Property IsBagelQuest As Boolean
    ReadOnly Property HasWon As Boolean
End Interface
