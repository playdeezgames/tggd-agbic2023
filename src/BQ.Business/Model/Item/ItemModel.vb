Friend Class ItemModel
    Implements IItemModel

    Private world As IWorld

    Public Sub New(world As IWorld)
        Me.world = world
    End Sub

    Public Property Name As String Implements IItemModel.Name
        Get
            Return world.Avatar.Metadata(Metadatas.ItemName)
        End Get
        Set(value As String)
            world.Avatar.Metadata(Metadatas.ItemName) = value
        End Set
    End Property

    Public Property Count As Integer Implements IItemModel.Count
        Get
            Return world.Avatar.TryGetStatistic(StatisticTypes.ItemCount)
        End Get
        Set(value As Integer)
            world.Avatar.Statistic(StatisticTypes.ItemCount) = value
        End Set
    End Property

    Public ReadOnly Property CanEquip As Boolean Implements IItemModel.CanEquip
        Get
            Return world.Avatar.Items.Where(Function(x) x.Name = Name).Any(Function(x) x.CanEquip)
        End Get
    End Property

    Public ReadOnly Property Equippables As IEnumerable(Of (fullName As String, itemId As Integer)) Implements IItemModel.Equippables
        Get
            Return world.Avatar.Items.Where(Function(x) x.Name = Name).Select(Function(x) (x.FullName, x.Id))
        End Get
    End Property

    Public Sub Take() Implements IItemModel.Take
        Dim itemCount = Count
        Dim itemName = Name
        world.Avatar.RemoveStatistic(StatisticTypes.ItemCount)
        world.Avatar.RemoveMetadata(Metadatas.ItemName)
        Dim items = world.Avatar.Cell.Items.Where(Function(x) x.Name = itemName).Take(itemCount)
        For Each item In items
            world.Avatar.Cell.RemoveItem(item)
            world.Avatar.AddItem(item)
        Next
    End Sub

    Public Sub Drop() Implements IItemModel.Drop
        Dim itemCount = Count
        Dim itemName = Name
        world.Avatar.RemoveStatistic(StatisticTypes.ItemCount)
        world.Avatar.RemoveMetadata(Metadatas.ItemName)
        Dim items = world.Avatar.Items.Where(Function(x) x.Name = itemName).Take(itemCount)
        For Each item In items
            world.Avatar.Cell.AddItem(item)
            world.Avatar.RemoveItem(item)
        Next
    End Sub

    Public Sub Equip(itemId As Integer) Implements IItemModel.Equip
        world.Avatar.EquipItem(world.Item(itemId))
    End Sub
End Class
