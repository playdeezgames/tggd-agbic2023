Imports BQ.Persistence

Friend Module AvatarInitializer
    Friend Sub Initialize(world As IWorld)
        world.Avatar = world.Characters.Single(Function(x) x.CharacterType.ToCharacterTypeDescriptor.HasFlag(FlagTypes.Avatar))
    End Sub
End Module
