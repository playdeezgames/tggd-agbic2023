Public Interface IFlagHolder
    ReadOnly Property Flag(flagType As String) As Boolean
    Sub SetFlag(flagType As String, value As Boolean)
End Interface
