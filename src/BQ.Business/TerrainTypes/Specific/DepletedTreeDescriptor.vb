Friend Class DepletedTreeDescriptor
    Inherits TerrainTypeDescriptor

    Public Sub New()
        MyBase.New(
            "Tree",
            ChrW(&HA),
            Business.Hue.Brown,
            True,
            peril:=1)
    End Sub
End Class
