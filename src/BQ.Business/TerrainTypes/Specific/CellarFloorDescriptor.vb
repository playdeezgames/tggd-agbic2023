Friend Class CellarFloorDescriptor
    Inherits TerrainTypeDescriptor

    Public Sub New()
        MyBase.New(
            "Cellar Floor",
            ChrW(0),
            Black,
            True,
            peril:=1,
            creatureTypeGenerator:=New Dictionary(Of String, Integer) From
            {
                {"Rat", 1}
            })
    End Sub
End Class
