Friend Class Map
    Inherits MapDataClient
    Implements IMap
    Public Sub New(worldData As Data.WorldData, mapId As Integer)
        MyBase.New(worldData, mapId)
    End Sub
    Public ReadOnly Property Cells As IEnumerable(Of ICell) Implements IMap.Cells
        Get
            Return Enumerable.Range(0, Columns * Rows).Select(Function(x) New Cell(WorldData, MapId, x))
        End Get
    End Property
    Public ReadOnly Property Columns As Integer Implements IMap.Columns
        Get
            Return MapData.Columns
        End Get
    End Property
    Public ReadOnly Property Rows As Integer Implements IMap.Rows
        Get
            Return MapData.Rows
        End Get
    End Property
    Public ReadOnly Property World As IWorld Implements IMap.World
        Get
            Return New World(WorldData)
        End Get
    End Property
    Public ReadOnly Property Id As Integer Implements IMap.Id
        Get
            Return MapId
        End Get
    End Property

    Public ReadOnly Property MapType As String Implements IMap.MapType
        Get
            Return MapData.MapType
        End Get
    End Property

    Public Sub RemoveStatistic(statisticType As String) Implements IStatisticsHolder.RemoveStatistic
        MapData.Statistics.Remove(statisticType)
    End Sub

    Public Sub RemoveMetadata(identifier As String) Implements IMetadataHolder.RemoveMetadata
        MapData.Metadatas.Remove(identifier)
    End Sub

    Public Sub SetStatistic(statisticType As String, value As Integer) Implements IStatisticsHolder.SetStatistic
        MapData.Statistics(statisticType) = value
    End Sub

    Public Sub SetFlag(flagType As String, value As Boolean) Implements IFlagHolder.SetFlag
        If value Then
            MapData.Flags.Add(flagType)
        Else
            MapData.Flags.Remove(flagType)
        End If
    End Sub

    Public Sub SetMetadata(identifier As String, value As String) Implements IMetadataHolder.SetMetadata
        MapData.Metadatas(identifier) = value
    End Sub

    Public Function GetCell(column As Integer, row As Integer) As ICell Implements IMap.GetCell
        If column < 0 OrElse row < 0 OrElse column >= Columns OrElse row >= Rows Then
            Return Nothing
        End If
        Return New Cell(WorldData, MapId, column + row * Columns)
    End Function
    Public Function CreateEffect() As IMapEffect Implements IMap.CreateEffect
        Dim triggerId = MapData.Effects.Count
        MapData.Effects.Add(New Data.EffectData)
        Return New MapEffect(WorldData, MapId, triggerId)
    End Function

    Public Function HasStatistic(statisticType As String) As Boolean Implements IStatisticsHolder.HasStatistic
        Return MapData.Statistics.ContainsKey(statisticType)
    End Function

    Public Function HasMetadata(identifier As String) As Boolean Implements IMetadataHolder.HasMetadata
        Return MapData.Metadatas.ContainsKey(identifier)
    End Function

    Public Function GetStatistic(statisticType As String, Optional defaultValue As Integer = 0) As Integer Implements IStatisticsHolder.GetStatistic
        Return If(HasStatistic(statisticType), MapData.Statistics(statisticType), defaultValue)
    End Function

    Public Function AddStatistic(statisticType As String, delta As Integer) As Integer Implements IStatisticsHolder.AddStatistic
        SetStatistic(statisticType, GetStatistic(statisticType) + delta)
        Return GetStatistic(statisticType)
    End Function

    Public Function GetFlag(flagType As String) As Boolean Implements IFlagHolder.GetFlag
        Return MapData.Flags.Contains(flagType)
    End Function

    Public Function GetMetadata(identifier As String) As String Implements IMetadataHolder.GetMetadata
        Return MapData.Metadatas(identifier)
    End Function
End Class
