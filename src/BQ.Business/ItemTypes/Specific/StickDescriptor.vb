Friend Class StickDescriptor
    Inherits ItemTypeDescriptor

    Friend Sub New()
        MyBase.New(
            "Stick",
            ChrW(&H25),
            Brown)
    End Sub
End Class
