Imports System.Runtime.CompilerServices

Public Module FluentHolderExtensions
    <Extension>
    Public Function SetMetadata(Of THolder As IMetadataHolder)(holder As THolder, identifier As String, value As String) As THolder
        holder.Metadata(identifier) = value
        Return holder
    End Function
    <Extension>
    Public Function ChangeStatisticTo(Of THolder As IStatisticsHolder)(holder As THolder, statisticType As String, value As Integer) As THolder
        holder.SetStatistic(statisticType, value)
        Return holder
    End Function
    <Extension>
    Public Function SetFlag(Of THolder As IFlagHolder)(holder As THolder, flagType As String, value As Boolean) As THolder
        holder.Flag(flagType) = value
        Return holder
    End Function
End Module
