Public Interface IMetadataHolder
    Function HasMetadata(identifier As String) As Boolean
    Property Metadata(identifier As String) As String
    Sub RemoveMetadata(identifier As String)
End Interface
