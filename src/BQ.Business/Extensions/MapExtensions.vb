Imports System.Runtime.CompilerServices

Friend Module MapExtensions
    <Extension>
    Function CampingAllowed(map As IMap) As Boolean
        Return map.Flag(FlagTypes.CampingAllowed)
    End Function
End Module
