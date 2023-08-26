Imports System.Runtime.CompilerServices

Public Module FluentHolderExtensions
    <Extension>
    Public Function ChangeMetadataTo(Of THolder As IMetadataHolder)(holder As THolder, identifier As String, value As String) As THolder
        holder.SetMetadata(identifier, value)
        Return holder
    End Function
    <Extension>
    Public Function ChangeStatisticTo(Of THolder As IStatisticsHolder)(holder As THolder, statisticType As String, value As Integer) As THolder
        holder.SetStatistic(statisticType, value)
        Return holder
    End Function
End Module
