Public Interface IMetadataHolder
    Function HasMetadata(identifier As String) As Boolean
    Sub SetMetadata(identifier As String, value As String)
    Function GetMetadata(identifier As String) As String
    Sub RemoveMetadata(identifier As String)
End Interface
