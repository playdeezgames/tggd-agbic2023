Public Module WorldExtensions
    Function GetSingleMapByMapType(world As IWorld, mapType As String) As IMap
        Return world.Maps.Single(Function(x) x.MapType = mapType)
    End Function
End Module
