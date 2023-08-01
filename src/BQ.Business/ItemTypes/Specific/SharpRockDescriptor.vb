Friend Class SharpRockDescriptor
    Inherits ItemTypeDescriptor

    Friend Sub New()
        MyBase.New(
            "Sharp Rock",
            ChrW(&H31),
            LightGray,
            flags:=New List(Of String) From {FlagTypes.IsCuttingTool})
    End Sub
End Class
