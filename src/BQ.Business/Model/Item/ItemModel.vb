Friend Class ItemModel
    Implements IItemModel

    Private world As IWorld

    Public Sub New(world As IWorld)
        Me.world = world
    End Sub

    Public Property Name As String Implements IItemModel.Name
        Get
            Return world.Avatar.GetMetadata(Metadatas.ItemName)
        End Get
        Set(value As String)
            world.Avatar.SetMetadata(Metadatas.ItemName, value)
        End Set
    End Property

    Public Property Count As Integer Implements IItemModel.Count
        Get
            Return world.Avatar.GetStatistic(StatisticTypes.ItemCount)
        End Get
        Set(value As Integer)
            world.Avatar.SetStatistic(StatisticTypes.ItemCount, value)
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

    Public ReadOnly Property EffectTypes As IEnumerable(Of (text As String, VerbTypes As String)) Implements IItemModel.EffectTypes
        Get
            Return world.Avatar.Items.First(Function(x) x.Name = Name).Descriptor.AllEffectTypes.Select(Function(x) (x.ToEffectTypeDescriptor.Name, x))
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
        Dim item = world.Item(itemId)
        world.Avatar.EquipItem(item)
        world.CreateMessage().AddLine(LightGray, $"{world.Avatar.Name} equips {item.Name}")
    End Sub

    Public Sub DoEffect(effectType As String) Implements IItemModel.DoEffect
        Dim item = world.Avatar.Items.First(Function(x) x.Name = Name)
        Dim effect = item.Descriptor.ToItemEffect(effectType, item)
        world.Avatar.Descriptor.EffectHandlers(effectType).Invoke(world.Avatar, effect)
    End Sub
End Class
