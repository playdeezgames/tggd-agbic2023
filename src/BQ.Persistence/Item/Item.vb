Imports System.Net.NetworkInformation

Friend Class Item
    Inherits ItemDataClient
    Implements IItem
    Public Sub New(worldData As Data.WorldData, itemId As Integer)
        MyBase.New(worldData, itemId)
    End Sub
    Public ReadOnly Property Id As Integer Implements IItem.Id
        Get
            Return ItemId
        End Get
    End Property

    Public ReadOnly Property ItemType As String Implements IItem.ItemType
        Get
            Return ItemData.ItemType
        End Get
    End Property

    Public Property Statistic(statisticType As String) As Integer Implements IItem.Statistic
        Get
            Return ItemData.Statistics(statisticType)
        End Get
        Set(value As Integer)
            ItemData.Statistics(statisticType) = value
        End Set
    End Property

    Public ReadOnly Property HasStatistic(statisticType As String) As Boolean Implements IItem.HasStatistic
        Get
            Return ItemData.Statistics.ContainsKey(statisticType)
        End Get
    End Property

    Public Sub Recycle() Implements IItem.Recycle
        ItemData.Recycled = True
    End Sub
End Class
