Imports BQ.Persistence

Public Class WorldModel
    Implements IWorldModel
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
