Imports BQ.Persistence
Public Class AvatarInventoryModel
    Implements IAvatarInventoryModel
    Private ReadOnly avatar As ICharacter
    Private ReadOnly model As IAvatarModel
    Public Sub New(avatar As ICharacter, model As IAvatarModel)
        Me.avatar = avatar
        Me.model = model
    End Sub
    Public ReadOnly Property Exists As Boolean Implements IAvatarInventoryModel.Exists
        Get
            Return avatar.HasItems
        End Get
    End Property
    Public ReadOnly Property Items As IEnumerable(Of (String, String)) Implements IAvatarInventoryModel.Items
        Get
            Return avatar.Items.GroupBy(Function(x) x.Name).Select(Function(x) ($"{x.Key}(x{x.Count})", x.Key))
        End Get
    End Property
    Public ReadOnly Property ItemsByName As IEnumerable(Of (String, Integer)) Implements IAvatarInventoryModel.ItemsByName
        Get
            Return avatar.Items.Where(Function(x) x.Name = model.SelectedItemName).Select(Function(x) ($"{x.Name}", x.Id))
        End Get
    End Property

    Public ReadOnly Property ItemVerbs As IEnumerable(Of (String, String)) Implements IAvatarInventoryModel.ItemVerbs
        Get
            Dim item = avatar.World.Item(model.SelectedItemId.Value)
            Return item.Descriptor.AllVerbTypes.Select(Function(x) (x.ToVerbTypeDescriptor.Name, x))
        End Get
    End Property

    Public ReadOnly Property CanEquip As Boolean Implements IAvatarInventoryModel.CanEquip
        Get
            Return avatar.World.Item(model.SelectedItemId.Value).Descriptor.CanEquip
        End Get
    End Property

    Public ReadOnly Property ItemsByFullName As IEnumerable(Of (String, Integer)) Implements IAvatarInventoryModel.ItemsByFullName
        Get
            Return avatar.Items.Where(Function(x) x.Name = model.SelectedItemName).Select(Function(x) ($"{x.FullName}", x.Id))
        End Get
    End Property

    Public Sub DropSelected() Implements IAvatarInventoryModel.DropSelected
        Dim item = avatar.World.Item(model.SelectedItemId.Value)
        avatar.DropItem(item)
        model.SelectedItemName = Nothing
        model.SelectedItemId = Nothing
    End Sub

    Public Sub DoItemVerb(verbType As String) Implements IAvatarInventoryModel.DoItemVerb
        Dim item = avatar.World.Item(model.SelectedItemId.Value)
        item.DoVerb(verbType, avatar)
    End Sub

    Public Sub Equip() Implements IAvatarInventoryModel.Equip
        Dim item = avatar.World.Item(model.SelectedItemId.Value)
        avatar.Equip(item.Descriptor.EquipSlotType, item)
    End Sub
End Class
