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

    Public Property Statistic(statisticType As String) As Integer Implements IStatisticsHolder.Statistic
        Get
            Return MapData.Statistics(statisticType)
        End Get
        Set(value As Integer)
            MapData.Statistics(statisticType) = value
        End Set
    End Property

    Public Property Flag(flagType As String) As Boolean Implements IFlagHolder.Flag
        Get
            Return MapData.Flags.Contains(flagType)
        End Get
        Set(value As Boolean)
            If value Then
                MapData.Flags.Add(flagType)
            Else
                MapData.Flags.Remove(flagType)
            End If
        End Set
    End Property

    Public Property Metadata(identifier As String) As String Implements IMetadataHolder.Metadata
        Get
            Return MapData.Metadata(identifier)
        End Get
        Set(value As String)
            MapData.Metadata(identifier) = value
        End Set
    End Property

    Public Sub RemoveStatistic(statisticType As String) Implements IStatisticsHolder.RemoveStatistic
        MapData.Statistics.Remove(statisticType)
    End Sub

    Public Sub RemoveMetadata(identifier As String) Implements IMetadataHolder.RemoveMetadata
        MapData.Metadata.Remove(identifier)
    End Sub

    Public Function GetCell(column As Integer, row As Integer) As ICell Implements IMap.GetCell
        If column < 0 OrElse row < 0 OrElse column >= Columns OrElse row >= Rows Then
            Return Nothing
        End If
        Return New Cell(WorldData, MapId, column + row * Columns)
    End Function
    Public Function CreateTrigger() As ITrigger Implements IMap.CreateTrigger
        Dim triggerId = MapData.Triggers.Count
        MapData.Triggers.Add(New Data.TriggerData)
        Return New Trigger(WorldData, MapId, triggerId)
    End Function

    Public Function HasStatistic(statisticType As String) As Boolean Implements IStatisticsHolder.HasStatistic
        Return MapData.Statistics.ContainsKey(statisticType)
    End Function

    Public Function HasMetadata(identifier As String) As Boolean Implements IMetadataHolder.HasMetadata
        Return MapData.Metadata.ContainsKey(identifier)
    End Function
End Class
