Friend Class AvatarEquipmentModel
    Implements IAvatarEquipmentModel

    Private avatar As ICharacter

    Public Sub New(avatar As ICharacter)
        Me.avatar = avatar
    End Sub
End Class
