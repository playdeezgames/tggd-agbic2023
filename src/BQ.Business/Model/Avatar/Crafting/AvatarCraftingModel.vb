Friend Class AvatarCraftingModel
    Implements IAvatarCraftingModel

    Private ReadOnly avatar As ICharacter

    Public Sub New(avatar As ICharacter)
        Me.avatar = avatar
    End Sub
End Class
