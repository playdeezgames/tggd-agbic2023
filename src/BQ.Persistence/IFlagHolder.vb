Public Interface IFlagHolder
    ReadOnly Property Flag(flagType As String) As Boolean
    Sub SetFlag(flagType As String, value As Boolean)
    Function GetFlag(flagType As String) As Boolean
End Interface
