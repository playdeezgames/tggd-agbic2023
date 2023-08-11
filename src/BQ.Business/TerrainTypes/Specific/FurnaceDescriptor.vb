Friend Class FurnaceDescriptor
    Inherits TerrainTypeDescriptor

    Public Sub New()
        MyBase.New("Furnace",
            ChrW(&H45),
            Business.Hue.LightGray,
            True)
    End Sub
End Class
