Public Interface IFlagHolder
    Sub SetFlag(flagType As String, value As Boolean)
    Function GetFlag(flagType As String) As Boolean
End Interface
