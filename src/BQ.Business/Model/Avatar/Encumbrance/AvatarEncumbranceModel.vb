Imports BQ.Persistence

Friend Class AvatarEncumbranceModel
    Implements IAvatarEncumbranceModel

    Private avatar As ICharacter

    Public Sub New(avatar As ICharacter)
        Me.avatar = avatar
    End Sub

    Public ReadOnly Property Current As Integer Implements IAvatarEncumbranceModel.Current
        Get
            Return avatar.Encumbrance
        End Get
    End Property

    Public ReadOnly Property Maximum As Integer Implements IAvatarEncumbranceModel.Maximum
        Get
            Return avatar.MaximumEncumbrance
        End Get
    End Property

    Public ReadOnly Property IsOver As Boolean Implements IAvatarEncumbranceModel.IsOver
        Get
            Return avatar.IsOverencumbered
        End Get
    End Property
End Class
