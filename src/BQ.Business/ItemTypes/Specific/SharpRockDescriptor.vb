Friend Class SharpRockDescriptor
    Inherits ItemTypeDescriptor

    Friend Sub New()
        MyBase.New(
            "Sharp Rock",
            ChrW(&H31),
            7,
            flags:=New List(Of String) From {"IsCuttingTool"})
    End Sub
End Class
