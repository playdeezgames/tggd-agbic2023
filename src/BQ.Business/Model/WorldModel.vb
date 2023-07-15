Imports BQ.Persistence

Public Class WorldModel
    Implements IWorldModel
    Public ReadOnly Property Map As IMapModel Implements IWorldModel.Map
        Get
            Return New MapModel(World)
        End Get
    End Property
    Public ReadOnly Property Avatar As IAvatarModel Implements IWorldModel.Avatar
        Get
            Return New AvatarModel(World)
        End Get
    End Property
    Public ReadOnly Property TargetCharacter As ITargetCharacterModel Implements IWorldModel.TargetCharacter
        Get
            Return New TargetCharacterModel(World)
        End Get
    End Property

    Public ReadOnly Property Messages As IMessagesModel Implements IWorldModel.Messages
        Get
            Return New MessagesModel(World)
        End Get
    End Property

    Public ReadOnly Property IsBagelQuest As Boolean Implements IWorldModel.IsBagelQuest
        Get
            Return World.Avatar.IsBagelQuest
        End Get
    End Property

    Public ReadOnly Property HasWon As Boolean Implements IWorldModel.HasWon
        Get
            Return World.Avatar.Cell.TerrainType = TerrainTypes.Door
        End Get
    End Property

    Private Property World As IWorld
    Public Sub Embark() Implements IWorldModel.Embark
        World = New World(New WorldData)
        WorldInitializer.Initialize(World)
    End Sub
    Public Sub Abandon() Implements IWorldModel.Abandon
        World = Nothing
    End Sub
    Public Sub Load(filename As String) Implements IWorldModel.Load
        World = BQ.Persistence.World.Load(filename)
    End Sub
    Public Sub Save(filename As String) Implements IWorldModel.Save
        World.Save(filename)
    End Sub
End Class
