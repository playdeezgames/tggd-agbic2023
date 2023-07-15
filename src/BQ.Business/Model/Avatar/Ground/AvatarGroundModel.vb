Imports BQ.Persistence

Friend Class AvatarGroundModel
    Implements IAvatarGroundModel
    Private ReadOnly avatar As ICharacter
    Private ReadOnly model As IAvatarModel

    Sub New(avatar As ICharacter, model As IAvatarModel)
        Me.avatar = avatar
        Me.model = model
    End Sub

    Public ReadOnly Property GroundItems As IEnumerable(Of (String, String)) Implements IAvatarGroundModel.GroundItems
        Get
            Return avatar.Cell.Items.Where(Function(x) x.CanTake).GroupBy(Function(x) x.Name).Select(Function(x) ($"{x.Key}(x{x.Count})", x.Key))
        End Get
    End Property

    Public ReadOnly Property GroundItemsByName As IEnumerable(Of (String, Integer)) Implements IAvatarGroundModel.GroundItemsByName
        Get
            Return avatar.Cell.Items.Where(Function(x) x.CanTake AndAlso x.Name = model.SelectedItemName).Select(Function(x) (x.Name, x.Id))
        End Get
    End Property

    Public ReadOnly Property HasGroundItems As Boolean Implements IAvatarGroundModel.HasGroundItems
        Get
            Return avatar.Cell.Items.Any(Function(x) x.CanTake)
        End Get
    End Property
End Class
