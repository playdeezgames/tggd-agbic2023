Imports System.Runtime.CompilerServices

Public Module FluentHolderExtensions
    <Extension>
    Public Function ChangeMetadataTo(Of THolder As IMetadataHolder)(holder As THolder, identifier As String, value As String) As THolder
        holder.SetMetadata(identifier, value)
        Return holder
    End Function
End Module
