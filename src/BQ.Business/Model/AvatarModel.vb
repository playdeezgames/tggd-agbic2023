Friend Class AvatarModel
    Implements IAvatarModel

    Private ReadOnly avatar As ICharacter

    Public Sub New(avatar As ICharacter)
        Me.avatar = avatar
    End Sub

    Public Sub Move(delta As (x As Integer, y As Integer)) Implements IAvatarModel.Move
        avatar.Move(delta)
    End Sub
End Class
