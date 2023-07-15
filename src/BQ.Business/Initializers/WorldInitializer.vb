Imports BQ.Persistence

Friend Module WorldInitializer
    Friend Sub Initialize(world As IWorld)
        MapInitializer.Initialize(world)
        AvatarInitializer.Initialize(world)
    End Sub
End Module
