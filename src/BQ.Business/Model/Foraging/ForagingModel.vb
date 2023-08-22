﻿Friend Class ForagingModel
    Implements IForagingModel

    Private ReadOnly world As IWorld

    Public Sub New(world As IWorld)
        Me.world = world
    End Sub

    Public ReadOnly Property GridSize As (columns As Integer, rows As Integer) Implements IForagingModel.GridSize
        Get
            Dim rows = CInt(Math.Sqrt(Remaining))
            Return ((Remaining + rows - 1) \ rows, rows)
        End Get
    End Property

    Public ReadOnly Property Remaining As Integer Implements IForagingModel.Remaining
        Get
            Return world.Avatar.Cell.Statistic(StatisticTypes.ForageRemaining)
        End Get
    End Property

    Public ReadOnly Property CanForage As Boolean Implements IForagingModel.CanForage
        Get
            Return Remaining > 0 AndAlso world.Avatar.Energy > 0
        End Get
    End Property

    Public Sub FinalReport(items As IEnumerable(Of IItem)) Implements IForagingModel.FinalReport
        If items.Any Then
            Dim table = items.GroupBy(Function(x) x.Name).ToDictionary(Function(x) x.Key, Function(x) x.Count)
            Dim msg = world.CreateMessage().
                AddLine(LightGray, $"{world.Avatar.Name} forages:")
            For Each entry In table
                msg.AddLine(LightGray, $"{entry.Key}(x{entry.Value})")
            Next
        End If
    End Sub

    Public Function Forage() As IItem Implements IForagingModel.Forage
        If Not CanForage Then
            Return Nothing
        End If
        Dim itemType = world.Avatar.Cell.GenerateForageItemType()
        world.Avatar.Cell.AddStatistic(StatisticTypes.ForageRemaining, -1)
        world.Avatar.AddEnergy(-1)
        If String.IsNullOrEmpty(itemType) Then
            Return Nothing
        End If
        Dim item = ItemInitializer.CreateItem(world, itemType)
        world.Avatar.AddItem(item)
        Return item
    End Function
End Class
