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

    Public Sub Recycle() Implements IItem.Recycle
        ItemData.Recycled = True
    End Sub

    Public Sub RemoveStatistic(statisticType As String) Implements IStatisticsHolder.RemoveStatistic
        ItemData.Statistics.Remove(statisticType)
    End Sub

    Public Sub RemoveMetadata(identifier As String) Implements IMetadataHolder.RemoveMetadata
        ItemData.Metadatas.Remove(identifier)
    End Sub

    Public Sub SetStatistic(statisticType As String, value As Integer) Implements IStatisticsHolder.SetStatistic
        ItemData.Statistics(statisticType) = value
    End Sub

    Public Sub SetFlag(flagType As String, value As Boolean) Implements IFlagHolder.SetFlag
        If value Then
            ItemData.Flags.Add(flagType)
        Else
            ItemData.Flags.Remove(flagType)
        End If
    End Sub

    Public Sub SetMetadata(identifier As String, value As String) Implements IMetadataHolder.SetMetadata
        ItemData.Metadatas(identifier) = value
    End Sub

    Public Function HasStatistic(statisticType As String) As Boolean Implements IStatisticsHolder.HasStatistic
        Return ItemData.Statistics.ContainsKey(statisticType)
    End Function

    Public Function HasMetadata(identifier As String) As Boolean Implements IMetadataHolder.HasMetadata
        Return ItemData.Metadatas.ContainsKey(identifier)
    End Function

    Public Function GetStatistic(statisticType As String, Optional defaultValue As Integer = 0) As Integer Implements IStatisticsHolder.GetStatistic
        Return If(HasStatistic(statisticType), ItemData.Statistics(statisticType), defaultValue)
    End Function

    Public Function AddStatistic(statisticType As String, delta As Integer) As Integer Implements IStatisticsHolder.AddStatistic
        SetStatistic(statisticType, GetStatistic(statisticType) + delta)
        Return GetStatistic(statisticType)
    End Function

    Public Function GetFlag(flagType As String) As Boolean Implements IFlagHolder.GetFlag
        Return ItemData.Flags.Contains(flagType)
    End Function

    Public Function GetMetadata(identifier As String) As String Implements IMetadataHolder.GetMetadata
        Return ItemData.Metadatas(identifier)
    End Function
End Class
