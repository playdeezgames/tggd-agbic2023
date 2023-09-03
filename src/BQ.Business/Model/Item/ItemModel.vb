Friend Class ItemModel
    Implements IItemModel

    Private world As IWorld

    Public Sub New(world As IWorld)
        Me.world = world
    End Sub

    Public Property Name As String Implements IItemModel.Name
        Get
            Return world.Avatar.GetMetadata("ItemName")
        End Get
        Set(value As String)
            world.Avatar.SetMetadata("ItemName", value)
        End Set
    End Property

    Public Property Count As Integer Implements IItemModel.Count
        Get
            Return world.Avatar.GetStatistic("ItemCount")
        End Get
        Set(value As Integer)
            world.Avatar.SetStatistic("ItemCount", value)
        End Set
    End Property

    Public ReadOnly Property CanEquip As Boolean Implements IItemModel.CanEquip
        Get
            Return world.Avatar.Items.Where(Function(x) ItemExtensions.Name(x) = Name).Any(AddressOf ItemExtensions.CanEquip)
        End Get
    End Property

    Public ReadOnly Property Equippables As IEnumerable(Of (fullName As String, itemId As Integer)) Implements IItemModel.Equippables
        Get
            Return world.Avatar.Items.Where(Function(x) ItemExtensions.Name(x) = Name).Select(Function(x) (ItemExtensions.FullName(x), x.Id))
        End Get
    End Property

    Public ReadOnly Property EffectTypes As IEnumerable(Of (text As String, VerbTypes As String)) Implements IItemModel.EffectTypes
        Get
            Dim item = world.Avatar.Items.First(Function(x) ItemExtensions.Name(x) = Name)
            Return ItemExtensions.Descriptor(item).AllEffectTypes.Select(Function(x) (ToEffectTypeDescriptor(x).Name, x))
        End Get
    End Property

    Public Sub Take() Implements IItemModel.Take
        Dim itemCount = Count
        Dim itemName = Name
        world.Avatar.RemoveStatistic("ItemCount")
        world.Avatar.RemoveMetadata("ItemName")
        Dim items = world.Avatar.Cell.Items.Where(Function(x) ItemExtensions.Name(x) = itemName).Take(itemCount)
        For Each item In items
            world.Avatar.Cell.RemoveItem(item)
            world.Avatar.AddItem(item)
        Next
    End Sub

    Public Sub Drop() Implements IItemModel.Drop
        Dim itemCount = Count
        Dim itemName = Name
        world.Avatar.RemoveStatistic("ItemCount")
        world.Avatar.RemoveMetadata("ItemName")
        Dim items = world.Avatar.Items.Where(Function(x) ItemExtensions.Name(x) = itemName).Take(itemCount)
        For Each item In items
            world.Avatar.Cell.AddItem(item)
            world.Avatar.RemoveItem(item)
        Next
    End Sub

    Public Sub Equip(itemId As Integer) Implements IItemModel.Equip
        Dim item = world.GetItem(itemId)
        CharacterExtensions.EquipItem(world.Avatar, item)
        world.CreateMessage().AddLine(7, $"{CharacterExtensions.Name(world.Avatar)} equips {ItemExtensions.Name(item)}")
    End Sub

    Public Sub DoEffect(effectType As String) Implements IItemModel.DoEffect
        Dim item = world.Avatar.Items.First(Function(x) ItemExtensions.Name(x) = Name)
        Dim effect = ItemExtensions.Descriptor(item).ToItemEffect(effectType, item)
        CharacterExtensions.Descriptor(world.Avatar).RunEffectScript(WorldModel.LuaState, effectType, world.Avatar, effect)
    End Sub
End Class
