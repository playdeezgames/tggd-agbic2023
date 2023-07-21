Friend Class AvatarModel
    Implements IAvatarModel

    Private ReadOnly avatar As ICharacter

    Public Sub New(avatar As ICharacter)
        Me.avatar = avatar
    End Sub

    Public ReadOnly Property IsDead As Boolean Implements IAvatarModel.IsDead
        Get
            Return avatar.IsDead
        End Get
    End Property

    Public Sub Move(delta As (x As Integer, y As Integer)) Implements IAvatarModel.Move
        avatar.Move(delta)
    End Sub
End Class
