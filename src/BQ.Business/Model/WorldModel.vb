Imports System.IO

Public Class WorldModel
    Implements IWorldModel
    Public ReadOnly Property Map As IMapModel Implements IWorldModel.Map
        Get
            Return New MapModel(World.Avatar.Cell.Map, (World.Avatar.Cell.Column, World.Avatar.Cell.Row))
        End Get
    End Property
    Public ReadOnly Property Avatar As IAvatarModel Implements IWorldModel.Avatar
        Get
            Return New AvatarModel(World.Avatar)
        End Get
    End Property

    Public ReadOnly Property Message As IMessageModel Implements IWorldModel.Message
        Get
            Return New MessageModel(World)
        End Get
    End Property

    Public ReadOnly Property Combat As ICombatModel Implements IWorldModel.Combat
        Get
            Return New CombatModel(World)
        End Get
    End Property

    Public ReadOnly Property Item As IItemModel Implements IWorldModel.Item
        Get
            Return New ItemModel(World)
        End Get
    End Property

    Public ReadOnly Property Foraging As IForagingModel Implements IWorldModel.Foraging
        Get
            Return New ForagingModel(World)
        End Get
    End Property

    Private Const WorldIdentifier As String = "world"
    Private _world As IWorld
    Friend Shared LuaState As Lua = MakeLuaState()

    Private Shared Function MakeLuaState() As Lua
        Dim lua As New Lua()
        lua.LoadCLRPackage()
        lua.DoString("import('SPLORR.Game')
import('BQ.Business')
import = function() end")
        Return lua
    End Function

    Private Property World As IWorld
        Get
            Return _world
        End Get
        Set(value As IWorld)
            _world = value
            LuaState(WorldIdentifier) = value
        End Set
    End Property
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
        File.WriteAllText(filename, World.SerializedData)
    End Sub
End Class
