Imports BQ.Persistence

Friend Module AvatarInitializer
    Friend Sub Initialize(world As IWorld)
        world.Avatar = world.Characters.Single(Function(x) x.CharacterType = Schmeara)
    End Sub
End Module
